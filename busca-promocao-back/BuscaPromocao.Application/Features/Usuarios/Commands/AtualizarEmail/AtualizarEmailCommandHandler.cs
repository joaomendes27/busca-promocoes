using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Usuarios.Commands.AtualizarEmail;

public sealed class AtualizarEmailCommandHandler : IRequestHandler<AtualizarEmailCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarEmailCommandHandler(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AtualizarEmailCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new System.Exception("Usuário não encontrado.");

        if (!_passwordHasher.Verify(request.SenhaAtual, usuario.SenhaHash))
            throw new System.Exception("Senha atual incorreta.");

        var emailJaExiste = await _usuarioRepository.ObterPorEmailAsync(request.NovoEmail, cancellationToken);
        if (emailJaExiste is not null && emailJaExiste.Id != request.UsuarioId)
            throw new System.Exception("Esse e-mail já está em uso por outro usuário.");

        usuario.Email = request.NovoEmail;
        _usuarioRepository.Atualizar(usuario);
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }
}
