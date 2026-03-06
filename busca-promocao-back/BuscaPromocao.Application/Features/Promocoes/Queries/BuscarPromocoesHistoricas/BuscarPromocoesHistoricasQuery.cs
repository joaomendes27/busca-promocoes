using System.Collections.Generic;
using BuscaPromocao.Application.Features.Promocoes.DTOs;
using MediatR;

namespace BuscaPromocao.Application.Features.Promocoes.Queries.BuscarPromocoesHistoricas;

public sealed record BuscarPromocoesHistoricasQuery(
    List<string> Perfis,
    List<string> PalavrasChave,
    int DiasAtras
) : IRequest<IEnumerable<PromocaoDto>>;
