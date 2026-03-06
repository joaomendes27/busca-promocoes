using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Promocoes.DTOs;
using BuscaPromocao.Application.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Promocoes.Queries.BuscarPromocoesHistoricas;

public sealed class BuscarPromocoesHistoricasQueryHandler : IRequestHandler<BuscarPromocoesHistoricasQuery, IEnumerable<PromocaoDto>>
{
    private readonly IScraperService _scraperService;

    public BuscarPromocoesHistoricasQueryHandler(IScraperService scraperService)
    {
        _scraperService = scraperService;
    }

    public async Task<IEnumerable<PromocaoDto>> Handle(BuscarPromocoesHistoricasQuery request, CancellationToken cancellationToken)
    {
        if (request.Perfis == null || request.Perfis.Count == 0)
            throw new ArgumentException("Pelo menos um perfil deve ser informado.");
            
        if (request.PalavrasChave == null || request.PalavrasChave.Count == 0)
            throw new ArgumentException("Pelo menos uma palavra-chave deve ser informada.");
            
        if (request.DiasAtras <= 0)
            throw new ArgumentException("Dias atrás deve ser maior que zero.");

        var promocoes = await _scraperService.BuscarHistoricoPromocoesAsync(
            request.Perfis,
            request.PalavrasChave,
            request.DiasAtras,
            cancellationToken);

        return promocoes;
    }
}
