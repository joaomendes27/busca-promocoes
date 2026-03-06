using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Auth.Commands.Login;
using BuscaPromocao.Application.Auth.Commands.RegistrarUsuario;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BuscaPromocao.Application.Tests.Auth;

public class RegistrarUsuarioCommandHandlerTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly RegistrarUsuarioCommandHandler _handler;

    public RegistrarUsuarioCommandHandlerTests()
    {
        _usuarioRepoMock = new Mock<IUsuarioRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _handler = new RegistrarUsuarioCommandHandler(_usuarioRepoMock.Object, _passwordHasherMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComDadosValidos_DeveCriarUsuarioComSenhaHash()
    {
        var command = new RegistrarUsuarioCommand("João", "joao@email.com", "Senha123!");
        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync(command.Email, It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);
        _passwordHasherMock.Setup(p => p.Hash(command.Senha)).Returns("hash_seguro");
        _unitOfWorkMock.Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _usuarioRepoMock.Verify(r => r.AdicionarAsync(
            It.Is<Usuario>(u => u.Email == "joao@email.com" && u.SenhaHash == "hash_seguro"),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ComEmailJaExistente_DeveLancarException()
    {
        var command = new RegistrarUsuarioCommand("João", "joao@email.com", "Senha123!");
        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario { Email = "joao@email.com" });

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*email*");
    }
}

public class LoginCommandHandlerTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtProvider> _jwtProviderMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _usuarioRepoMock = new Mock<IUsuarioRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtProviderMock = new Mock<IJwtProvider>();
        _handler = new LoginCommandHandler(_usuarioRepoMock.Object, _passwordHasherMock.Object, _jwtProviderMock.Object);
    }

    [Fact]
    public async Task Handle_ComCredenciaisValidas_DeveRetornarLoginResponseDto()
    {
        var usuario = new Usuario { Id = Guid.NewGuid(), Nome = "João", Email = "joao@email.com", SenhaHash = "hash" };
        var command = new LoginCommand("joao@email.com", "Senha123!");

        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync(command.Email, It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.Verify(command.Senha, usuario.SenhaHash)).Returns(true);
        _jwtProviderMock.Setup(j => j.Generate(usuario)).Returns("token_jwt_valido");

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Token.Should().Be("token_jwt_valido");
        resultado.UsuarioId.Should().Be(usuario.Id);
        resultado.Nome.Should().Be("João");
    }

    [Fact]
    public async Task Handle_ComEmailInvalido_DeveLancarException()
    {
        var command = new LoginCommand("naoexiste@email.com", "Senha123!");
        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync(command.Email, It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*inválidos*");
    }

    [Fact]
    public async Task Handle_ComSenhaIncorreta_DeveLancarException()
    {
        var usuario = new Usuario { Id = Guid.NewGuid(), Email = "joao@email.com", SenhaHash = "hash" };
        var command = new LoginCommand("joao@email.com", "SenhaErrada!");

        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync(command.Email, It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.Verify(command.Senha, usuario.SenhaHash)).Returns(false);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*inválidos*");
    }
}
