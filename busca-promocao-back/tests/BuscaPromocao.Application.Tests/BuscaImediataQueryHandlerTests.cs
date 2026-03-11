using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Promocoes.DTOs;
using BuscaPromocao.Application.Features.Promocoes.Queries.BuscaImediata;
using BuscaPromocao.Application.Interfaces;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BuscaPromocao.Application.Tests.Features.Promocoes;

public class BuscaImediataQueryHandlerTests
{
    private readonly Mock<IScraperService> _scraperServiceMock;
    private readonly Mock<IPerfilRepository> _perfilRepoMock;
    private readonly BuscaImediataQueryHandler _handler;

    public BuscaImediataQueryHandlerTests()
    {
        _scraperServiceMock = new Mock<IScraperService>();
        _perfilRepoMock = new Mock<IPerfilRepository>();
        _handler = new BuscaImediataQueryHandler(_scraperServiceMock.Object, _perfilRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ComPerfisEProdutos_DeveRetornarPromocoes()
    {
        var usuarioId = Guid.NewGuid();
        var perfis = new List<Perfil>
        {
            new() { Id = Guid.NewGuid(), UsuarioId = usuarioId, HandlePerfil = "xetdaspromocoes" }
        };
        var promocoesEsperadas = new List<PromocaoDto>
        {
            new("xetdaspromocoes", "Air Fryer por R$199!", "https://x.com/1", DateTime.UtcNow)
        };
        var query = new BuscaImediataQuery(usuarioId, 7, new List<string> { "Air Fryer" });

        _perfilRepoMock.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(perfis);
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
            It.Is<IEnumerable<string>>(h => h.Contains("xetdaspromocoes")),
            It.Is<IEnumerable<string>>(p => p.Contains("Air Fryer")),
            7,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UsuarioSemPerfis_DeveRetornarListaVazia()
    {
        var usuarioId = Guid.NewGuid();
        var query = new BuscaImediataQuery(usuarioId, 7, new List<string> { "Air Fryer" });

        _perfilRepoMock.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Perfil>());

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Should().BeEmpty();
        _scraperServiceMock.Verify(s => s.BuscarHistoricoPromocoesAsync(
            It.IsAny<IEnumerable<string>>(),
            It.IsAny<IEnumerable<string>>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SemProdutos_DeveRetornarListaVazia()
    {
        var usuarioId = Guid.NewGuid();
        var perfis = new List<Perfil>
        {
            new() { Id = Guid.NewGuid(), UsuarioId = usuarioId, HandlePerfil = "xetdaspromocoes" }
        };
        var query = new BuscaImediataQuery(usuarioId, 7, new List<string>());

        _perfilRepoMock.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(perfis);

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Should().BeEmpty();
        _scraperServiceMock.Verify(s => s.BuscarHistoricoPromocoesAsync(
            It.IsAny<IEnumerable<string>>(),
            It.IsAny<IEnumerable<string>>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ComMultiplosPerfis_DevePassarTodosHandlesParaScraper()
    {
        var usuarioId = Guid.NewGuid();
        var perfis = new List<Perfil>
        {
            new() { Id = Guid.NewGuid(), UsuarioId = usuarioId, HandlePerfil = "perfil_a" },
            new() { Id = Guid.NewGuid(), UsuarioId = usuarioId, HandlePerfil = "perfil_b" },
            new() { Id = Guid.NewGuid(), UsuarioId = usuarioId, HandlePerfil = "perfil_c" }
        };
        var query = new BuscaImediataQuery(usuarioId, 3, new List<string> { "Notebook" });

        _perfilRepoMock.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(perfis);
        _scraperServiceMock
            .Setup(s => s.BuscarHistoricoPromocoesAsync(
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PromocaoDto>());

        await _handler.Handle(query, CancellationToken.None);

        _scraperServiceMock.Verify(s => s.BuscarHistoricoPromocoesAsync(
            It.Is<IEnumerable<string>>(h => h.Contains("perfil_a") && h.Contains("perfil_b") && h.Contains("perfil_c")),
            It.IsAny<IEnumerable<string>>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ScraperRetornaVazio_DeveRetornarListaVazia()
    {
        var usuarioId = Guid.NewGuid();
        var perfis = new List<Perfil>
        {
            new() { Id = Guid.NewGuid(), UsuarioId = usuarioId, HandlePerfil = "xetdaspromocoes" }
        };
        var query = new BuscaImediataQuery(usuarioId, 7, new List<string> { "Produto Raro" });

        _perfilRepoMock.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(perfis);
        _scraperServiceMock
            .Setup(s => s.BuscarHistoricoPromocoesAsync(
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PromocaoDto>());

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Should().BeEmpty();
    }
}
