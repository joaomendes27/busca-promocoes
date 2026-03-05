using System;
using MediatR;

namespace BuscaPromocao.Application.Features.PalavrasChave.Commands.CriarPalavraChave;

public record CriarPalavraChaveCommand(Guid UsuarioId, string Termo) : IRequest<Guid>;
