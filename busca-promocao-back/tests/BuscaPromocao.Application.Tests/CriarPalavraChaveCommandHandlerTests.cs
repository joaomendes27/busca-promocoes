using BuscaPromocao.Application.Features.PalavrasChave.Commands.CriarPalavraChave;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BuscaPromocao.Application.Tests.Features.PalavrasChave;

public class CriarPalavraChaveCommandHandlerTests
{
    private readonly Mock<IPalavraChaveRepository> _palavraChaveRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CriarPalavraChaveCommandHandler _handler;

    public CriarPalavraChaveCommandHandlerTests()
    {
        _palavraChaveRepoMock = new Mock<IPalavraChaveRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CriarPalavraChaveCommandHandler(_palavraChaveRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Deve_Criar_PalavraChave_E_Retornar_Id()
    {
        var command = new CriarPalavraChaveCommand(Guid.NewGuid(), "Air Fryer");
        _unitOfWorkMock.Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().NotBeEmpty();
        _palavraChaveRepoMock.Verify(r => r.AdicionarAsync(
            It.Is<PalavraChave>(p => p.Termo == "Air Fryer" && p.UsuarioId == command.UsuarioId),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Associar_PalavraChave_Ao_UsuarioId_Correto()
    {
        var usuarioId = Guid.NewGuid();
        var command = new CriarPalavraChaveCommand(usuarioId, "Notebook");
        _unitOfWorkMock.Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _palavraChaveRepoMock.Verify(r => r.AdicionarAsync(
            It.Is<PalavraChave>(p => p.UsuarioId == usuarioId),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
