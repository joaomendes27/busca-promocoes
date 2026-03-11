using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Notificacoes.Commands.RemoverNotificacao;
using BuscaPromocao.Application.Features.Perfis.Commands.RemoverPerfil;
using BuscaPromocao.Application.Features.Produtos.Commands.RemoverProduto;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BuscaPromocao.Application.Tests.Features;

public class RemoverPerfilCommandHandlerTests
{
    private readonly Mock<IPerfilRepository> _perfilRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RemoverPerfilCommandHandler _handler;

    public RemoverPerfilCommandHandlerTests()
    {
        _perfilRepoMock = new Mock<IPerfilRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new RemoverPerfilCommandHandler(_perfilRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComPerfilExistente_DeveRemoverERetornarTrue()
    {
        var usuarioId = Guid.NewGuid();
        var perfilId = Guid.NewGuid();
        var perfil = new Perfil { Id = perfilId, UsuarioId = usuarioId, HandlePerfil = "xetdaspromocoes" };
        var command = new RemoverPerfilCommand(perfilId, usuarioId);

        _perfilRepoMock.Setup(r => r.ObterPorIdAsync(perfilId, It.IsAny<CancellationToken>())).ReturnsAsync(perfil);
        _unitOfWorkMock.Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().BeTrue();
        _perfilRepoMock.Verify(r => r.Remover(perfil), Times.Once);
        _unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PerfilNaoEncontrado_DeveLancarException()
    {
        var command = new RemoverPerfilCommand(Guid.NewGuid(), Guid.NewGuid());
        _perfilRepoMock.Setup(r => r.ObterPorIdAsync(command.PerfilId, It.IsAny<CancellationToken>())).ReturnsAsync((Perfil?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task Handle_PerfilDeOutroUsuario_DeveLancarException()
    {
        var perfilId = Guid.NewGuid();
        var donoDoPerfil = Guid.NewGuid();
        var usuarioMalicioso = Guid.NewGuid();
        var perfil = new Perfil { Id = perfilId, UsuarioId = donoDoPerfil, HandlePerfil = "xetdaspromocoes" };
        var command = new RemoverPerfilCommand(perfilId, usuarioMalicioso);

        _perfilRepoMock.Setup(r => r.ObterPorIdAsync(perfilId, It.IsAny<CancellationToken>())).ReturnsAsync(perfil);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*não pertence*");
        _perfilRepoMock.Verify(r => r.Remover(It.IsAny<Perfil>()), Times.Never);
    }
}

public class RemoverProdutoCommandHandlerTests
{
    private readonly Mock<IProdutoRepository> _produtoRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RemoverProdutoCommandHandler _handler;

    public RemoverProdutoCommandHandlerTests()
    {
        _produtoRepoMock = new Mock<IProdutoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new RemoverProdutoCommandHandler(_produtoRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComProdutoExistente_DeveRemoverERetornarTrue()
    {
        var usuarioId = Guid.NewGuid();
        var produtoId = Guid.NewGuid();
        var produto = new Produto { Id = produtoId, UsuarioId = usuarioId, Nome = "Air Fryer" };
        var command = new RemoverProdutoCommand(produtoId, usuarioId);

        _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produtoId, It.IsAny<CancellationToken>())).ReturnsAsync(produto);
        _unitOfWorkMock.Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().BeTrue();
        _produtoRepoMock.Verify(r => r.Remover(produto), Times.Once);
        _unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ProdutoNaoEncontrado_DeveLancarException()
    {
        var command = new RemoverProdutoCommand(Guid.NewGuid(), Guid.NewGuid());
        _produtoRepoMock.Setup(r => r.ObterPorIdAsync(command.ProdutoId, It.IsAny<CancellationToken>())).ReturnsAsync((Produto?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task Handle_ProdutoDeOutroUsuario_DeveLancarException()
    {
        var produtoId = Guid.NewGuid();
        var donoDoProduto = Guid.NewGuid();
        var usuarioMalicioso = Guid.NewGuid();
        var produto = new Produto { Id = produtoId, UsuarioId = donoDoProduto, Nome = "Air Fryer" };
        var command = new RemoverProdutoCommand(produtoId, usuarioMalicioso);

        _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produtoId, It.IsAny<CancellationToken>())).ReturnsAsync(produto);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*não pertence*");
        _produtoRepoMock.Verify(r => r.Remover(It.IsAny<Produto>()), Times.Never);
    }
}

public class RemoverNotificacaoCommandHandlerTests
{
    private readonly Mock<INotificacaoRepository> _notificacaoRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RemoverNotificacaoCommandHandler _handler;

    public RemoverNotificacaoCommandHandlerTests()
    {
        _notificacaoRepoMock = new Mock<INotificacaoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new RemoverNotificacaoCommandHandler(_notificacaoRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComNotificacaoExistente_DeveRemoverERetornarTrue()
    {
        var usuarioId = Guid.NewGuid();
        var notificacaoId = Guid.NewGuid();
        var notificacao = new Notificacao
        {
            Id = notificacaoId,
            UsuarioId = usuarioId,
            Titulo = "Air Fryer em promoção!",
            Conteudo = "R$199",
            UrlTweet = "https://x.com/1",
            HandlePerfil = "xetdaspromocoes",
            PostadoEm = DateTime.UtcNow
        };
        var command = new RemoverNotificacaoCommand(notificacaoId, usuarioId);

        _notificacaoRepoMock.Setup(r => r.ObterPorIdAsync(notificacaoId, It.IsAny<CancellationToken>())).ReturnsAsync(notificacao);
        _unitOfWorkMock.Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().BeTrue();
        _notificacaoRepoMock.Verify(r => r.Remover(notificacao), Times.Once);
        _unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NotificacaoNaoEncontrada_DeveLancarException()
    {
        var command = new RemoverNotificacaoCommand(Guid.NewGuid(), Guid.NewGuid());
        _notificacaoRepoMock.Setup(r => r.ObterPorIdAsync(command.NotificacaoId, It.IsAny<CancellationToken>())).ReturnsAsync((Notificacao?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*não encontrada*");
    }

    [Fact]
    public async Task Handle_NotificacaoDeOutroUsuario_DeveLancarException()
    {
        var notificacaoId = Guid.NewGuid();
        var donoOriginal = Guid.NewGuid();
        var usuarioMalicioso = Guid.NewGuid();
        var notificacao = new Notificacao
        {
            Id = notificacaoId,
            UsuarioId = donoOriginal,
            Titulo = "Promoção",
            Conteudo = "R$199",
            UrlTweet = "https://x.com/1",
            HandlePerfil = "xetdaspromocoes",
            PostadoEm = DateTime.UtcNow
        };
        var command = new RemoverNotificacaoCommand(notificacaoId, usuarioMalicioso);

        _notificacaoRepoMock.Setup(r => r.ObterPorIdAsync(notificacaoId, It.IsAny<CancellationToken>())).ReturnsAsync(notificacao);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*não pertence*");
        _notificacaoRepoMock.Verify(r => r.Remover(It.IsAny<Notificacao>()), Times.Never);
    }
}
