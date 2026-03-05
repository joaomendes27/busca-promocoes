using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Notifications.Queries.GetUnreadNotifications;

public class GetUnreadNotificationsQueryHandler : IRequestHandler<GetUnreadNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly IInAppNotificationRepository _notificationRepository;

    public GetUnreadNotificationsQueryHandler(IInAppNotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetUnreadNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _notificationRepository.GetUnreadByUserIdAsync(request.UserId, cancellationToken);
        
        return notifications.Select(n => new NotificationDto(
            n.Id, 
            n.Title, 
            n.Content, 
            n.TweetUrl, 
            n.ProfileHandle, 
            n.TweetPostedAt)).ToList();
    }
}
