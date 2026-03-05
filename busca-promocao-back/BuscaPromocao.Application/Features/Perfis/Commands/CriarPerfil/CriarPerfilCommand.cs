using System;
using MediatR;

namespace BuscaPromocao.Application.Features.Perfis.Commands.CriarPerfil;

public record CriarPerfilCommand(Guid UsuarioId, string HandlePerfil) : IRequest<Guid>;
