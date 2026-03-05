using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuscaPromocao.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Keyword> Keywords { get; }
    DbSet<Profile> Profiles { get; }
    DbSet<InAppNotification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
