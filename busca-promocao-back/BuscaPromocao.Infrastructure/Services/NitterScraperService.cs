using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using BuscaPromocao.Application.Features.Promocoes.DTOs;
using BuscaPromocao.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BuscaPromocao.Infrastructure.Services;

public sealed class NitterScraperService : IScraperService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NitterScraperService> _logger;
    private readonly string _nitterBaseUrl;

    public NitterScraperService(HttpClient httpClient, IConfiguration configuration, ILogger<NitterScraperService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _nitterBaseUrl = configuration["Nitter:BaseUrl"] ?? "http://localhost:8080";
    }

    public async Task<IEnumerable<PromocaoDto>> BuscarHistoricoPromocoesAsync(
        IEnumerable<string> perfis, 
        IEnumerable<string> palavrasChave, 
        int diasAtras, 
        CancellationToken cancellationToken)
    {
        var limiteData = DateTime.UtcNow.AddDays(-diasAtras);
        var promocoesEncontradas = new List<PromocaoDto>();

        foreach (var perfil in perfis)
        {
            try
            {
                var queryPalavras = string.Join(" OR ", palavrasChave);
                var query = $"from:{perfil} {queryPalavras}";
                var url = $"{_nitterBaseUrl}/search/rss?f=tweets&q={Uri.EscapeDataString(query)}";

                var response = await _httpClient.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Falha ao buscar RSS do Nitter para url {Url}. Status: {Status}", url, response.StatusCode);
                    continue;
                }

                var xmlContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var doc = XDocument.Parse(xmlContent);

                var items = doc.Descendants("item");

                foreach (var item in items)
                {
                    var pubDateStr = item.Element("pubDate")?.Value;
                    if (DateTime.TryParse(pubDateStr, out var pubDate))
                    {
                        if (pubDate.ToUniversalTime() >= limiteData)
                        {
                            var title = item.Element("title")?.Value ?? string.Empty;
                            var link = item.Element("link")?.Value ?? string.Empty;

                            bool hasKeyword = palavrasChave.Any(p => 
                            {
                                var termoBase = p.EndsWith("s", StringComparison.OrdinalIgnoreCase) && p.Length > 3 
                                    ? p.Substring(0, p.Length - 1) 
                                    : p;
                                    
                                return CultureInfo.InvariantCulture.CompareInfo.IndexOf(
                                    title, 
                                    termoBase, 
                                    CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) >= 0;
                            });
                            
                            if (hasKeyword)
                            {
                                promocoesEncontradas.Add(new PromocaoDto(
                                    Perfil: perfil,
                                    Texto: title,
                                    UrlTweet: link,
                                    DataPublicacao: pubDate.ToUniversalTime()
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar o perfil {Perfil} no Nitter.", perfil);
            }
        }

        return promocoesEncontradas.OrderByDescending(p => p.DataPublicacao);
    }

    public async Task<bool> PerfilExisteAsync(string handle, CancellationToken cancellationToken)
    {
        try
        {
            var url = $"{_nitterBaseUrl}/{Uri.EscapeDataString(handle)}/rss";
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Perfil @{Handle} não encontrado no Nitter. Status: {Status}", handle, response.StatusCode);
                return false;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var doc = XDocument.Parse(content);
            return doc.Descendants("channel").Any();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do perfil @{Handle} no Nitter.", handle);
            return false;
        }
    }
}
