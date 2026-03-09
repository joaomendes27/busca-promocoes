using System;
using System.Collections.Generic;
using BuscaPromocao.Application.Features.Promocoes.DTOs;
using MediatR;

namespace BuscaPromocao.Application.Features.Promocoes.Queries.BuscaImediata;

public record BuscaImediataQuery(Guid UsuarioId, int DiasAtras, IEnumerable<string> Produtos) : IRequest<IEnumerable<PromocaoDto>>;
