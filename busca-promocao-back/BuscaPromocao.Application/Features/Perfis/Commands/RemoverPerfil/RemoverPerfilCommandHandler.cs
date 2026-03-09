using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Perfis.Commands.RemoverPerfil;

public sealed class RemoverPerfilCommandHandler : IRequestHandler<RemoverPerfilCommand, bool>
{
    private readonly IPerfilRepository _perfilRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoverPerfilCommandHandler(IPerfilRepository perfilRepository, IUnitOfWork unitOfWork)
    {
        _perfilRepository = perfilRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoverPerfilCommand request, CancellationToken cancellationToken)
    {
        var perfil = await _perfilRepository.ObterPorIdAsync(request.PerfilId, cancellationToken);

        if (perfil == null || perfil.UsuarioId != request.UsuarioId)
        {
            throw new Exception("Perfil não encontrado ou não pertence a este usuário.");
        }

        _perfilRepository.Remover(perfil);
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return true;
    }
}
