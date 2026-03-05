using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;

namespace BuscaPromocao.Domain.Interfaces;

public interface IKeywordRepository
{
    Task<Keyword?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Keyword>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Keyword keyword, CancellationToken cancellationToken = default);
    void Remove(Keyword keyword);
}
