using System;
using MediatR;

namespace BuscaPromocao.Application.Features.Notifications.Commands.MarkAsRead;

public record MarkNotificationAsReadCommand(Guid NotificationId) : IRequest<bool>;
