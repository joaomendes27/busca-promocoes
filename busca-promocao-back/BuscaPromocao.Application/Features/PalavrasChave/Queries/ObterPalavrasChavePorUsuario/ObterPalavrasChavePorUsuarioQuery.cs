using System;
using System.Collections.Generic;
using MediatR;

namespace BuscaPromocao.Application.Features.PalavrasChave.Queries.ObterPalavrasChavePorUsuario;

public record PalavraChaveDto(Guid Id, string Termo, DateTime CriadoEm);

public record ObterPalavrasChavePorUsuarioQuery(Guid UsuarioId) : IRequest<IEnumerable<PalavraChaveDto>>;
