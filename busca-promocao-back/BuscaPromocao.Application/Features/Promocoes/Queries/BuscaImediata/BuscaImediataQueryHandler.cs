using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Promocoes.DTOs;
using BuscaPromocao.Application.Interfaces;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Promocoes.Queries.BuscaImediata;

public class BuscaImediataQueryHandler : IRequestHandler<BuscaImediataQuery, IEnumerable<PromocaoDto>>
{
    private readonly IScraperService _scraperService;
    private readonly IPerfilRepository _perfilRepository;

    public BuscaImediataQueryHandler(IScraperService scraperService, IPerfilRepository perfilRepository)
    {
        _scraperService = scraperService;
        _perfilRepository = perfilRepository;
    }

    public async Task<IEnumerable<PromocaoDto>> Handle(BuscaImediataQuery request, CancellationToken cancellationToken)
    {
        // 1. Obter todos os perfis monitorados por esse usuário
        var perfisUsuario = await _perfilRepository.ObterPorUsuarioIdAsync(request.UsuarioId, cancellationToken);
        var handles = perfisUsuario.Select(p => p.HandlePerfil).ToList();

        if (!handles.Any() || !request.Produtos.Any())
        {
            return Enumerable.Empty<PromocaoDto>();
        }

        // 2. Chamar o serviço de Scraper On-the-fly sem passar pelo BD de Notificações
        return await _scraperService.BuscarHistoricoPromocoesAsync(
            handles, 
            request.Produtos, 
            request.DiasAtras, 
            cancellationToken);
    }
}
