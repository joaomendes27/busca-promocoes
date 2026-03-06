using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Promocoes.DTOs;

namespace BuscaPromocao.Application.Interfaces;

public interface IScraperService
{
    Task<IEnumerable<PromocaoDto>> BuscarHistoricoPromocoesAsync(
        IEnumerable<string> perfis, 
        IEnumerable<string> palavrasChave, 
        int diasAtras, 
        CancellationToken cancellationToken);
}
