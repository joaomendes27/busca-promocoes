using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace BuscaPromocao.Application.Tests.Infrastructure;

public class NitterScraperServiceTests
{
    private readonly Mock<ILogger<NitterScraperService>> _loggerMock;
    private readonly IConfiguration _configuration;

    public NitterScraperServiceTests()
    {
        _loggerMock = new Mock<ILogger<NitterScraperService>>();
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Nitter:BaseUrl", "http://localhost:8080" }
            })
            .Build();
    }

    private NitterScraperService CriarServiceComResposta(string xmlContent, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handler = new FakeHttpMessageHandler(xmlContent, statusCode);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:8080") };
        return new NitterScraperService(httpClient, _configuration, _loggerMock.Object);
    }

    [Fact]
    public async Task BuscarHistorico_ComMatchDePalavraChave_DeveRetornarPromocao()
    {
        var xml = CriarRssXml("Air Fryer Mondial por apenas R$199!", DateTime.UtcNow.AddDays(-1), "https://twitter.com/1");
        var service = CriarServiceComResposta(xml);

        var resultado = await service.BuscarHistoricoPromocoesAsync(
            new[] { "xetdaspromocoes" },
            new[] { "Air Fryer" },
            7,
            CancellationToken.None);

        resultado.Should().HaveCount(1);
        resultado.First().Texto.Should().Contain("Air Fryer");
        resultado.First().Perfil.Should().Be("xetdaspromocoes");
    }

    [Fact]
    public async Task BuscarHistorico_SemMatch_DeveRetornarVazio()
    {
        var xml = CriarRssXml("Promoção de tênis Nike", DateTime.UtcNow.AddDays(-1), "https://twitter.com/2");
        var service = CriarServiceComResposta(xml);

        var resultado = await service.BuscarHistoricoPromocoesAsync(
            new[] { "xetdaspromocoes" },
            new[] { "Air Fryer" },
            7,
            CancellationToken.None);

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task BuscarHistorico_TweetAntigoDemais_DeveSerFiltrado()
    {
        var xml = CriarRssXml("Air Fryer baratinha!", DateTime.UtcNow.AddDays(-30), "https://twitter.com/3");
        var service = CriarServiceComResposta(xml);

        var resultado = await service.BuscarHistoricoPromocoesAsync(
            new[] { "xetdaspromocoes" },
            new[] { "Air Fryer" },
            7,
            CancellationToken.None);

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task BuscarHistorico_ComFalhaHttp_DeveRetornarVazio()
    {
        var service = CriarServiceComResposta("", HttpStatusCode.InternalServerError);

        var resultado = await service.BuscarHistoricoPromocoesAsync(
            new[] { "xetdaspromocoes" },
            new[] { "Air Fryer" },
            7,
            CancellationToken.None);

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task BuscarHistorico_ComMultiplosPerfis_DeveConsultarTodos()
    {
        var xml = CriarRssXml("Notebook Dell i7 por R$2999!", DateTime.UtcNow.AddHours(-5), "https://twitter.com/4");
        var service = CriarServiceComResposta(xml);

        var resultado = await service.BuscarHistoricoPromocoesAsync(
            new[] { "xetdaspromocoes", "ofertasdobom" },
            new[] { "Notebook" },
            7,
            CancellationToken.None);

        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task BuscarHistorico_DeveFiltrarCaseInsensitive()
    {
        var xml = CriarRssXml("AIR FRYER por R$149", DateTime.UtcNow.AddDays(-1), "https://twitter.com/5");
        var service = CriarServiceComResposta(xml);

        var resultado = await service.BuscarHistoricoPromocoesAsync(
            new[] { "xetdaspromocoes" },
            new[] { "air fryer" },
            7,
            CancellationToken.None);

        resultado.Should().HaveCount(1);
    }

    private static string CriarRssXml(string titulo, DateTime pubDate, string link)
    {
        return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version=""2.0"">
  <channel>
    <title>Test Feed</title>
    <item>
      <title>{titulo}</title>
      <link>{link}</link>
      <pubDate>{pubDate:R}</pubDate>
      <guid>{link}</guid>
    </item>
  </channel>
</rss>";
    }
}

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly string _responseContent;
    private readonly HttpStatusCode _statusCode;

    public FakeHttpMessageHandler(string responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        _responseContent = responseContent;
        _statusCode = statusCode;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_responseContent)
        };
        return Task.FromResult(response);
    }
}
