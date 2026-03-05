using System;
using System.Collections.Generic;
using MediatR;

namespace BuscaPromocao.Application.Features.Notifications.Queries.GetUnreadNotifications;

public record NotificationDto(
    Guid Id, 
    string Title, 
    string Content, 
    string TweetUrl, 
    string ProfileHandle, 
    DateTime TweetPostedAt);

public record GetUnreadNotificationsQuery(Guid UserId) : IRequest<IEnumerable<NotificationDto>>;
