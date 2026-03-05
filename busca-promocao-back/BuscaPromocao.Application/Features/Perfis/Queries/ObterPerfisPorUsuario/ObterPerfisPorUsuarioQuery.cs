using System;
using System.Collections.Generic;
using MediatR;

namespace BuscaPromocao.Application.Features.Perfis.Queries.ObterPerfisPorUsuario;

public record PerfilDto(Guid Id, string HandlePerfil, DateTime CriadoEm);

public record ObterPerfisPorUsuarioQuery(Guid UsuarioId) : IRequest<IEnumerable<PerfilDto>>;
