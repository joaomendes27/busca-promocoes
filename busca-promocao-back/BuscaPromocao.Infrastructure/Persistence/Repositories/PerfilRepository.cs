using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscaPromocao.Infrastructure.Persistence.Repositories;

public sealed class PerfilRepository : IPerfilRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PerfilRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AdicionarAsync(Perfil perfil, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<Perfil>().AddAsync(perfil, cancellationToken);
    }

    public async Task<Perfil?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Perfil>().FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<Perfil>> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Perfil>()
            .Where(p => p.UsuarioId == usuarioId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Perfil>> ObterTodosAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Perfil>()
            .ToListAsync(cancellationToken);
    }

    public void Remover(Perfil perfil)
    {
        _dbContext.Set<Perfil>().Remove(perfil);
    }
}
