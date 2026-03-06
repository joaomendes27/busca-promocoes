using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscaPromocao.Infrastructure.Persistence.Repositories;

public sealed class ProdutoRepository : IProdutoRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProdutoRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AdicionarAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<Produto>().AddAsync(produto, cancellationToken);
    }

    public async Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Produto>().FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<Produto>> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Produto>()
            .Where(p => p.UsuarioId == usuarioId)
            .ToListAsync(cancellationToken);
    }

    public void Remover(Produto produto)
    {
        _dbContext.Set<Produto>().Remove(produto);
    }
}
