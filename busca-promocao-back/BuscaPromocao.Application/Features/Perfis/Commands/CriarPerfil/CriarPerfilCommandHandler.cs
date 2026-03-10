using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Interfaces;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Perfis.Commands.CriarPerfil;

public class CriarPerfilCommandHandler : IRequestHandler<CriarPerfilCommand, Guid>
{
    private readonly IPerfilRepository _perfilRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IScraperService _scraperService;

    public CriarPerfilCommandHandler(
        IPerfilRepository perfilRepository, 
        IUnitOfWork unitOfWork,
        IScraperService scraperService)
    {
        _perfilRepository = perfilRepository;
        _unitOfWork = unitOfWork;
        _scraperService = scraperService;
    }

    public async Task<Guid> Handle(CriarPerfilCommand request, CancellationToken cancellationToken)
    {
        var perfilExiste = await _scraperService.PerfilExisteAsync(request.HandlePerfil, cancellationToken);
        if (!perfilExiste)
            throw new InvalidOperationException($"O perfil @{request.HandlePerfil} não foi encontrado no X (Twitter).");

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
