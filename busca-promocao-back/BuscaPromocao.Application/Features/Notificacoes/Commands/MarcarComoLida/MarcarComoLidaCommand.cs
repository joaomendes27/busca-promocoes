using System;
using MediatR;

namespace BuscaPromocao.Application.Features.Notificacoes.Commands.MarcarComoLida;

public record MarcarComoLidaCommand(Guid NotificacaoId) : IRequest<bool>;
