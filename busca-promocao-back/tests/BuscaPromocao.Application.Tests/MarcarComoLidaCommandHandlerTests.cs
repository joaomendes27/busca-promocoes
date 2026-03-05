using BuscaPromocao.Application.Features.Notificacoes.Commands.MarcarComoLida;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BuscaPromocao.Application.Tests.Features.Notificacoes;

public class MarcarComoLidaCommandHandlerTests
{
    private readonly Mock<INotificacaoRepository> _notificacaoRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly MarcarComoLidaCommandHandler _handler;

    public MarcarComoLidaCommandHandlerTests()
    {
        _notificacaoRepoMock = new Mock<INotificacaoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new MarcarComoLidaCommandHandler(_notificacaoRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Deve_Marcar_Notificacao_Como_Lida_E_Retornar_True()
    {
        // Arrange
        var notificacaoId = Guid.NewGuid();
        var notificacao = new Notificacao
        {
            Id = notificacaoId,
            Titulo = "Promo",
            Conteudo = "Conteúdo",
            FoiLida = false,
            UsuarioId = Guid.NewGuid()
        };

        _notificacaoRepoMock
            .Setup(r => r.ObterPorIdAsync(notificacaoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notificacao);
        _unitOfWorkMock
            .Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new MarcarComoLidaCommand(notificacaoId);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeTrue();
        notificacao.FoiLida.Should().BeTrue();
        _notificacaoRepoMock.Verify(r => r.Atualizar(notificacao), Times.Once);
        _unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Retornar_False_Quando_Notificacao_Nao_Existe()
    {
        // Arrange
        var notificacaoId = Guid.NewGuid();
        _notificacaoRepoMock
            .Setup(r => r.ObterPorIdAsync(notificacaoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notificacao?)null);

        var command = new MarcarComoLidaCommand(notificacaoId);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeFalse();
        _notificacaoRepoMock.Verify(r => r.Atualizar(It.IsAny<Notificacao>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
