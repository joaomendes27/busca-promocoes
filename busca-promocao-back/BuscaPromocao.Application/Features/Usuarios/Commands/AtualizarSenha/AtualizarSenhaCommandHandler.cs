using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Usuarios.Commands.AtualizarSenha;

public sealed class AtualizarSenhaCommandHandler : IRequestHandler<AtualizarSenhaCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarSenhaCommandHandler(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AtualizarSenhaCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new System.Exception("Usuário não encontrado.");

        if (!_passwordHasher.Verify(request.SenhaAtual, usuario.SenhaHash))
            throw new System.Exception("Senha atual incorreta.");

        usuario.SenhaHash = _passwordHasher.Hash(request.NovaSenha);
        _usuarioRepository.Atualizar(usuario);
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }
}
