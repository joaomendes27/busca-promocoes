using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Promocoes.DTOs;
using BuscaPromocao.Application.Features.Promocoes.Queries.BuscarPromocoesHistoricas;
using BuscaPromocao.Application.Interfaces;
using FluentAssertions;
using Moq;

namespace BuscaPromocao.Application.Tests.Features.Promocoes;

public class BuscarPromocoesHistoricasQueryHandlerTests
{
    private readonly Mock<IScraperService> _scraperServiceMock;
    private readonly BuscarPromocoesHistoricasQueryHandler _handler;

    public BuscarPromocoesHistoricasQueryHandlerTests()
    {
        _scraperServiceMock = new Mock<IScraperService>();
        _handler = new BuscarPromocoesHistoricasQueryHandler(_scraperServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ComParametrosValidos_DeveRetornarPromocoes()
    {
        var query = new BuscarPromocoesHistoricasQuery(
            new List<string> { "xetdaspromocoes" },
            new List<string> { "Air Fryer" },
            7);

        var promocoesEsperadas = new List<PromocaoDto>
        {
            new("xetdaspromocoes", "Air Fryer por R$199!", "https://twitter.com/1", DateTime.UtcNow)
        };

        _scraperServiceMock
            .Setup(s => s.BuscarHistoricoPromocoesAsync(
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<IEnumerable<string>>(),
                7,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(promocoesEsperadas);

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Should().HaveCount(1);
        resultado.First().Perfil.Should().Be("xetdaspromocoes");
        _scraperServiceMock.Verify(s => s.BuscarHistoricoPromocoesAsync(
            It.IsAny<IEnumerable<string>>(),
            It.IsAny<IEnumerable<string>>(),
            7,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_SemPerfis_DeveLancarArgumentException()
    {
        var query = new BuscarPromocoesHistoricasQuery(
            new List<string>(),
            new List<string> { "Notebook" },
            5);

        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*perfil*");
    }

    [Fact]
    public async Task Handle_SemPalavrasChave_DeveLancarArgumentException()
    {
        var query = new BuscarPromocoesHistoricasQuery(
            new List<string> { "xetdaspromocoes" },
            new List<string>(),
            5);

        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*palavra-chave*");
    }

    [Fact]
    public async Task Handle_DiasAtrasZero_DeveLancarArgumentException()
    {
        var query = new BuscarPromocoesHistoricasQuery(
            new List<string> { "xetdaspromocoes" },
            new List<string> { "Air Fryer" },
            0);

        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*maior que zero*");
    }

    [Fact]
    public async Task Handle_ScraperRetornaVazio_DeveRetornarListaVazia()
    {
        var query = new BuscarPromocoesHistoricasQuery(
            new List<string> { "xetdaspromocoes" },
            new List<string> { "Produto Inexistente" },
            7);

        _scraperServiceMock
            .Setup(s => s.BuscarHistoricoPromocoesAsync(
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<IEnumerable<string>>(),
                7,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PromocaoDto>());

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ComMultiplosPerfisEProdutos_DevePassarTodosParaScraper()
    {
        var perfis = new List<string> { "xetdaspromocoes", "ofertasdobom" };
        var produtos = new List<string> { "Air Fryer", "Notebook" };
        var query = new BuscarPromocoesHistoricasQuery(perfis, produtos, 14);

        _scraperServiceMock
            .Setup(s => s.BuscarHistoricoPromocoesAsync(
                It.Is<IEnumerable<string>>(p => p.Count() == 2),
                It.Is<IEnumerable<string>>(p => p.Count() == 2),
                14,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PromocaoDto>());

        await _handler.Handle(query, CancellationToken.None);

        _scraperServiceMock.Verify(s => s.BuscarHistoricoPromocoesAsync(
            It.Is<IEnumerable<string>>(p => p.Count() == 2),
            It.Is<IEnumerable<string>>(p => p.Count() == 2),
            14,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
