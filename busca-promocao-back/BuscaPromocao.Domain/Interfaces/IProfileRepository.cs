using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;

namespace BuscaPromocao.Domain.Interfaces;

public interface IProfileRepository
{
    Task<Profile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Profile>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Profile>> GetAllActiveProfilesAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Profile profile, CancellationToken cancellationToken = default);
    void Remove(Profile profile);
}
