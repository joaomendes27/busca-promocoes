using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;

namespace BuscaPromocao.Domain.Interfaces;

public interface IInAppNotificationRepository
{
    Task<IEnumerable<InAppNotification>> GetUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<InAppNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(InAppNotification notification, CancellationToken cancellationToken = default);
    void Update(InAppNotification notification);
}
