using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Usuarios.Commands.AtualizarEmail;
using BuscaPromocao.Application.Features.Usuarios.Commands.AtualizarSenha;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BuscaPromocao.Application.Tests.Features.Usuarios;

public class AtualizarEmailCommandHandlerTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AtualizarEmailCommandHandler _handler;

    public AtualizarEmailCommandHandlerTests()
    {
        _usuarioRepoMock = new Mock<IUsuarioRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new AtualizarEmailCommandHandler(
            _usuarioRepoMock.Object,
            _passwordHasherMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComDadosValidos_DeveAtualizarEmail()
    {
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario { Id = usuarioId, Email = "antigo@email.com", SenhaHash = "hash_atual" };
        var command = new AtualizarEmailCommand(usuarioId, "novo@email.com", "senha_correta");

        _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync("novo@email.com", It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);
        _passwordHasherMock.Setup(p => p.Verify("senha_correta", "hash_atual")).Returns(true);
        _unitOfWorkMock.Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        usuario.Email.Should().Be("novo@email.com");
        _usuarioRepoMock.Verify(r => r.Atualizar(usuario), Times.Once);
        _unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UsuarioNaoEncontrado_DeveLancarException()
    {
        var command = new AtualizarEmailCommand(Guid.NewGuid(), "novo@email.com", "senha");
        _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(command.UsuarioId, It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task Handle_ComSenhaIncorreta_DeveLancarException()
    {
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario { Id = usuarioId, Email = "antigo@email.com", SenhaHash = "hash_atual" };
        var command = new AtualizarEmailCommand(usuarioId, "novo@email.com", "senha_errada");

        _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.Verify("senha_errada", "hash_atual")).Returns(false);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*incorreta*");
    }

    [Fact]
    public async Task Handle_EmailJaUsadoPorOutroUsuario_DeveLancarException()
    {
        var usuarioId = Guid.NewGuid();
        var outroUsuario = new Usuario { Id = Guid.NewGuid(), Email = "novo@email.com" };
        var usuario = new Usuario { Id = usuarioId, Email = "antigo@email.com", SenhaHash = "hash_atual" };
        var command = new AtualizarEmailCommand(usuarioId, "novo@email.com", "senha_correta");

        _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync("novo@email.com", It.IsAny<CancellationToken>())).ReturnsAsync(outroUsuario);
        _passwordHasherMock.Setup(p => p.Verify("senha_correta", "hash_atual")).Returns(true);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*já está em uso*");
    }

    [Fact]
    public async Task Handle_EmailJaUsadoPeloProprioUsuario_DeveAtualizar()
    {
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario { Id = usuarioId, Email = "mesmo@email.com", SenhaHash = "hash_atual" };
        var command = new AtualizarEmailCommand(usuarioId, "mesmo@email.com", "senha_correta");

        _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync("mesmo@email.com", It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.Verify("senha_correta", "hash_atual")).Returns(true);
        _unitOfWorkMock.Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class AtualizarSenhaCommandHandlerTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AtualizarSenhaCommandHandler _handler;

    public AtualizarSenhaCommandHandlerTests()
    {
        _usuarioRepoMock = new Mock<IUsuarioRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new AtualizarSenhaCommandHandler(
            _usuarioRepoMock.Object,
            _passwordHasherMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComDadosValidos_DeveAtualizarSenhaComHash()
    {
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario { Id = usuarioId, SenhaHash = "hash_antigo" };
        var command = new AtualizarSenhaCommand(usuarioId, "senha_atual", "nova_senha_forte");

        _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.Verify("senha_atual", "hash_antigo")).Returns(true);
        _passwordHasherMock.Setup(p => p.Hash("nova_senha_forte")).Returns("hash_novo");
        _unitOfWorkMock.Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        usuario.SenhaHash.Should().Be("hash_novo");
        _usuarioRepoMock.Verify(r => r.Atualizar(usuario), Times.Once);
        _unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UsuarioNaoEncontrado_DeveLancarException()
    {
        var command = new AtualizarSenhaCommand(Guid.NewGuid(), "senha_atual", "nova_senha");
        _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(command.UsuarioId, It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task Handle_ComSenhaAtualIncorreta_DeveLancarException()
    {
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario { Id = usuarioId, SenhaHash = "hash_antigo" };
        var command = new AtualizarSenhaCommand(usuarioId, "senha_errada", "nova_senha");

        _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.Verify("senha_errada", "hash_antigo")).Returns(false);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*incorreta*");
    }

    [Fact]
    public async Task Handle_NaoDeveSalvarSenhaEmTextoPlano()
    {
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario { Id = usuarioId, SenhaHash = "hash_antigo" };
        var command = new AtualizarSenhaCommand(usuarioId, "senha_atual", "nova_senha_forte");

        _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(usuarioId, It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.Verify("senha_atual", "hash_antigo")).Returns(true);
        _passwordHasherMock.Setup(p => p.Hash("nova_senha_forte")).Returns("$2a$11$hash_bcrypt_seguro");
        _unitOfWorkMock.Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        usuario.SenhaHash.Should().NotBe("nova_senha_forte");
        _passwordHasherMock.Verify(p => p.Hash("nova_senha_forte"), Times.Once);
    }
}
