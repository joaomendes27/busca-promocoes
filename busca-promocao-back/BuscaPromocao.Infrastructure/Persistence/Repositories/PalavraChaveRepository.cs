using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscaPromocao.Infrastructure.Persistence.Repositories;

public sealed class PalavraChaveRepository : IPalavraChaveRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PalavraChaveRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AdicionarAsync(PalavraChave palavraChave, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<PalavraChave>().AddAsync(palavraChave, cancellationToken);
    }

    public async Task<PalavraChave?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<PalavraChave>().FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<PalavraChave>> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<PalavraChave>()
            .Where(p => p.UsuarioId == usuarioId)
            .ToListAsync(cancellationToken);
    }

    public void Remover(PalavraChave palavraChave)
    {
        _dbContext.Set<PalavraChave>().Remove(palavraChave);
    }
}
