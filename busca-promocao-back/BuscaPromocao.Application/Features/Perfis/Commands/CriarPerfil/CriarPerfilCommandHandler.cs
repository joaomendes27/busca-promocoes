using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Perfis.Commands.CriarPerfil;

public class CriarPerfilCommandHandler : IRequestHandler<CriarPerfilCommand, Guid>
{
    private readonly IPerfilRepository _perfilRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarPerfilCommandHandler(IPerfilRepository perfilRepository, IUnitOfWork unitOfWork)
    {
        _perfilRepository = perfilRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CriarPerfilCommand request, CancellationToken cancellationToken)
    {
        var perfil = new Perfil
        {
            Id = Guid.NewGuid(),
            UsuarioId = request.UsuarioId,
            HandlePerfil = request.HandlePerfil,
            CreatedAt = DateTime.UtcNow
        };

        await _perfilRepository.AdicionarAsync(perfil, cancellationToken);
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return perfil.Id;
    }
}
