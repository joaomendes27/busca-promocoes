using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Auth.Commands.Login;

public sealed record LoginResponseDto(string Token, Guid UsuarioId, string Nome);

public sealed record LoginCommand(string Email, string Senha) : IRequest<LoginResponseDto>;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(
        IUsuarioRepository usuarioRepository, 
        IPasswordHasher passwordHasher, 
        IJwtProvider jwtProvider)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email, cancellationToken);
        if (usuario is null)
        {
            throw new Exception("Email ou senha inválidos.");
        }

        var senhaValida = _passwordHasher.Verify(request.Senha, usuario.SenhaHash);
        if (!senhaValida)
        {
            throw new Exception("Email ou senha inválidos.");
        }

        var token = _jwtProvider.Generate(usuario);

        return new LoginResponseDto(token, usuario.Id, usuario.Nome);
    }
}
