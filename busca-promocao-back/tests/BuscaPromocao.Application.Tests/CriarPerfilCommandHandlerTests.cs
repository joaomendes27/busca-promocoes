using BuscaPromocao.Application.Features.Perfis.Commands.CriarPerfil;
using BuscaPromocao.Application.Interfaces;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BuscaPromocao.Application.Tests.Features.Perfis;

public class CriarPerfilCommandHandlerTests
{
    private readonly Mock<IPerfilRepository> _perfilRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IScraperService> _scraperServiceMock;
    private readonly CriarPerfilCommandHandler _handler;

    public CriarPerfilCommandHandlerTests()
    {
        _perfilRepoMock = new Mock<IPerfilRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _scraperServiceMock = new Mock<IScraperService>();
        _handler = new CriarPerfilCommandHandler(_perfilRepoMock.Object, _unitOfWorkMock.Object, _scraperServiceMock.Object);
    }

    [Fact]
    public async Task Deve_Criar_Perfil_E_Retornar_Id()
    {
        var command = new CriarPerfilCommand(Guid.NewGuid(), "xetdaspromocoes");
        _scraperServiceMock.Setup(s => s.PerfilExisteAsync("xetdaspromocoes", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().NotBeEmpty();
        _perfilRepoMock.Verify(r => r.AdicionarAsync(
            It.Is<Perfil>(p => p.HandlePerfil == "xetdaspromocoes" && p.UsuarioId == command.UsuarioId),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Se_Perfil_Nao_Existe()
    {
        var command = new CriarPerfilCommand(Guid.NewGuid(), "perfil_inexistente_xyz");
        _scraperServiceMock.Setup(s => s.PerfilExisteAsync("perfil_inexistente_xyz", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*@perfil_inexistente_xyz*não foi encontrado*");
        _perfilRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Perfil>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
