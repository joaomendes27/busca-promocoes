using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Notificacoes.Queries.ObterNotificacoesNaoLidas;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BuscaPromocao.Application.Tests.Features.Notificacoes;

public class ObterNotificacoesNaoLidasQueryHandlerTests
{
    private readonly Mock<INotificacaoRepository> _repoMock;
    private readonly ObterNotificacoesNaoLidasQueryHandler _handler;

    public ObterNotificacoesNaoLidasQueryHandlerTests()
    {
        _repoMock = new Mock<INotificacaoRepository>();
        _handler = new ObterNotificacoesNaoLidasQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ComNotificacoesNaoLidas_DeveRetornarLista()
    {
        var usuarioId = Guid.NewGuid();
        var notificacoes = new List<Notificacao>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Titulo = "Promoção de Air Fryer encontrada!",
                Conteudo = "Air Fryer por R$199",
                UrlTweet = "https://twitter.com/1",
                HandlePerfil = "xetdaspromocoes",
                PostadoEm = DateTime.UtcNow,
                FoiLida = false,
                UsuarioId = usuarioId
            }
        };

        _repoMock.Setup(r => r.ObterNaoLidasPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notificacoes);

        var resultado = await _handler.Handle(new ObterNotificacoesNaoLidasQuery(usuarioId), CancellationToken.None);

        resultado.Should().HaveCount(1);
        resultado.First().Titulo.Should().Contain("Air Fryer");
    }

    [Fact]
    public async Task Handle_SemNotificacoes_DeveRetornarListaVazia()
    {
        var usuarioId = Guid.NewGuid();
        _repoMock.Setup(r => r.ObterNaoLidasPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notificacao>());

        var resultado = await _handler.Handle(new ObterNotificacoesNaoLidasQuery(usuarioId), CancellationToken.None);

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_DeveMapearCorretamenteParaDto()
    {
        var usuarioId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var dataPostagem = DateTime.UtcNow.AddHours(-2);
        var notificacoes = new List<Notificacao>
        {
            new()
            {
                Id = id,
                Titulo = "Promoção de Notebook!",
                Conteudo = "Notebook Dell por R$2999",
                UrlTweet = "https://twitter.com/42",
                HandlePerfil = "ofertasdobom",
                PostadoEm = dataPostagem,
                FoiLida = false,
                UsuarioId = usuarioId
            }
        };

        _repoMock.Setup(r => r.ObterNaoLidasPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notificacoes);

        var resultado = (await _handler.Handle(new ObterNotificacoesNaoLidasQuery(usuarioId), CancellationToken.None)).ToList();

        resultado.First().Id.Should().Be(id);
        resultado.First().Titulo.Should().Be("Promoção de Notebook!");
        resultado.First().Conteudo.Should().Be("Notebook Dell por R$2999");
        resultado.First().UrlTweet.Should().Be("https://twitter.com/42");
        resultado.First().HandlePerfil.Should().Be("ofertasdobom");
        resultado.First().PostadoEm.Should().Be(dataPostagem);
    }
}
