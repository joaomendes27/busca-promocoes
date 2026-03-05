using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Perfis.Queries.ObterPerfisPorUsuario;

public class ObterPerfisPorUsuarioQueryHandler : IRequestHandler<ObterPerfisPorUsuarioQuery, IEnumerable<PerfilDto>>
{
    private readonly IPerfilRepository _perfilRepository;

    public ObterPerfisPorUsuarioQueryHandler(IPerfilRepository perfilRepository)
    {
        _perfilRepository = perfilRepository;
    }

    public async Task<IEnumerable<PerfilDto>> Handle(ObterPerfisPorUsuarioQuery request, CancellationToken cancellationToken)
    {
        var perfis = await _perfilRepository.ObterPorUsuarioIdAsync(request.UsuarioId, cancellationToken);
        
        return perfis.Select(p => new PerfilDto(p.Id, p.HandlePerfil, p.CreatedAt)).ToList();
    }
}
