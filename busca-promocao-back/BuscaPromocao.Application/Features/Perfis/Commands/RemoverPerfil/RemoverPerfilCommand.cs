using System;
using MediatR;

namespace BuscaPromocao.Application.Features.Perfis.Commands.RemoverPerfil;

public record RemoverPerfilCommand(Guid PerfilId, Guid UsuarioId) : IRequest<bool>;
