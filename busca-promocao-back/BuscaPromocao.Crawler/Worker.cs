using System;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using BuscaPromocao.Application.Eventos;
using BuscaPromocao.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace BuscaPromocao.Crawler;

public sealed class CrawlerWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IConnectionMultiplexer _redis;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CrawlerWorker> _logger;

    public CrawlerWorker(
        IServiceScopeFactory scopeFactory,
        IPublishEndpoint publishEndpoint,
        IConnectionMultiplexer redis,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<CrawlerWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _publishEndpoint = publishEndpoint;
        _redis = redis;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervaloMinutos = _configuration.GetValue<int>("Crawler:IntervaloMinutos");
        if (intervaloMinutos <= 0) intervaloMinutos = 10;

        _logger.LogInformation("Crawler iniciado. Intervalo de execução: {Intervalo} minutos.", intervaloMinutos);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecutarVarreduraAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante a varredura do Crawler.");
            }

            await Task.Delay(TimeSpan.FromMinutes(intervaloMinutos), stoppingToken);
        }
    }

    private async Task ExecutarVarreduraAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando varredura às {DataHora}.", DateTime.UtcNow);

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var perfis = await dbContext.Set<BuscaPromocao.Domain.Entities.Perfil>()
            .Include(p => p.Usuario)
            .ToListAsync(cancellationToken);

        if (!perfis.Any())
        {
            _logger.LogInformation("Nenhum perfil cadastrado para monitorar.");
            return;
        }

        var handlesUnicos = perfis.Select(p => p.HandlePerfil).Distinct().ToList();
        var nitterBaseUrl = _configuration["Nitter:BaseUrl"] ?? "http://localhost:8080";
        var redisDb = _redis.GetDatabase();
        var httpClient = _httpClientFactory.CreateClient();

        foreach (var handle in handlesUnicos)
        {
            try
            {
                await ProcessarPerfilAsync(handle, perfis, dbContext, redisDb, httpClient, nitterBaseUrl, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar o perfil {Handle}.", handle);
            }
        }

        _logger.LogInformation("Varredura finalizada às {DataHora}.", DateTime.UtcNow);
    }

    private async Task ProcessarPerfilAsync(
        string handle,
        List<BuscaPromocao.Domain.Entities.Perfil> todosPerfis,
        ApplicationDbContext dbContext,
        IDatabase redisDb,
        HttpClient httpClient,
        string nitterBaseUrl,
        CancellationToken cancellationToken)
    {
        var url = $"{nitterBaseUrl}/{handle}/rss";
        var response = await httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Falha ao buscar RSS de {Handle}. Status: {Status}", handle, response.StatusCode);
            return;
        }

        var xmlContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var doc = XDocument.Parse(xmlContent);
        var items = doc.Descendants("item");

        var usuariosComEstePerfil = todosPerfis
            .Where(p => p.HandlePerfil == handle)
            .Select(p => p.UsuarioId)
            .Distinct()
            .ToList();

        foreach (var item in items)
        {
            var tweetId = item.Element("guid")?.Value ?? item.Element("link")?.Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(tweetId)) continue;

            var jaProcessado = await redisDb.KeyExistsAsync($"tweet:{tweetId}");
            if (jaProcessado) continue;

            var titulo = item.Element("title")?.Value ?? string.Empty;
            var link = item.Element("link")?.Value ?? string.Empty;
            var pubDateStr = item.Element("pubDate")?.Value;
            DateTime.TryParse(pubDateStr, out var pubDate);

            foreach (var usuarioId in usuariosComEstePerfil)
            {
                var produtos = await dbContext.Set<BuscaPromocao.Domain.Entities.Produto>()
                    .Where(pc => pc.UsuarioId == usuarioId)
                    .ToListAsync(cancellationToken);

                foreach (var produto in produtos)
                {
                    if (titulo.Contains(produto.Nome, StringComparison.InvariantCultureIgnoreCase))
                    {
                        _logger.LogInformation(
                            "Promoção encontrada! Perfil: {Handle}, Produto: {Produto}, Tweet: {Link}",
                            handle, produto.Nome, link);

                        await _publishEndpoint.Publish(new PromocaoEncontradaEvento(
                            UsuarioId: usuarioId,
                            HandlePerfil: handle,
                            Produto: produto.Nome,
                            TextoTweet: titulo,
                            UrlTweet: link,
                            PostadoEm: pubDate.ToUniversalTime()
                        ), cancellationToken);
                    }
                }
            }

            await redisDb.StringSetAsync($"tweet:{tweetId}", "1", TimeSpan.FromDays(7));
        }
    }
}
