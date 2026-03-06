using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Auth.Commands.RegistrarUsuario;

public sealed record RegistrarUsuarioCommand(string Nome, string Email, string Senha) : IRequest<Guid>;

public sealed class RegistrarUsuarioCommandHandler : IRequestHandler<RegistrarUsuarioCommand, Guid>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository, 
        IPasswordHasher passwordHasher, 
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegistrarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var emailExiste = await _usuarioRepository.ObterPorEmailAsync(request.Email, cancellationToken);
        if (emailExiste is not null)
        {
            throw new System.Exception("Esse email já está em uso.");
        }

        var senhaHash = _passwordHasher.Hash(request.Senha);

        var novoUsuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            SenhaHash = senhaHash
        };

        await _usuarioRepository.AdicionarAsync(novoUsuario, cancellationToken);
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return novoUsuario.Id;
    }
}
