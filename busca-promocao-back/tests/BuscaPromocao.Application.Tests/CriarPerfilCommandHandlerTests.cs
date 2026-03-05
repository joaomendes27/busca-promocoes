using BuscaPromocao.Application.Features.Perfis.Commands.CriarPerfil;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BuscaPromocao.Application.Tests.Features.Perfis;

public class CriarPerfilCommandHandlerTests
{
    private readonly Mock<IPerfilRepository> _perfilRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CriarPerfilCommandHandler _handler;

    public CriarPerfilCommandHandlerTests()
    {
        _perfilRepoMock = new Mock<IPerfilRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CriarPerfilCommandHandler(_perfilRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Deve_Criar_Perfil_E_Retornar_Id()
    {
        var command = new CriarPerfilCommand(Guid.NewGuid(), "xetdaspromocoes");
        _unitOfWorkMock.Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().NotBeEmpty();
        _perfilRepoMock.Verify(r => r.AdicionarAsync(
            It.Is<Perfil>(p => p.HandlePerfil == "xetdaspromocoes" && p.UsuarioId == command.UsuarioId),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
