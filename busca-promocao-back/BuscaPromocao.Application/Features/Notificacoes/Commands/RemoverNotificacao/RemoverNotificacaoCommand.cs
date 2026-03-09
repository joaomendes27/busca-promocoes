using System;
using MediatR;

namespace BuscaPromocao.Application.Features.Notificacoes.Commands.RemoverNotificacao;

public record RemoverNotificacaoCommand(Guid NotificacaoId, Guid UsuarioId) : IRequest<bool>;
