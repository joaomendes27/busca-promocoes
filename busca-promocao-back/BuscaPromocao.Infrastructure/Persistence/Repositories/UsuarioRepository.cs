using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscaPromocao.Infrastructure.Persistence.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UsuarioRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<Usuario>().AddAsync(usuario, cancellationToken);
    }

    public void Atualizar(Usuario usuario)
    {
        _dbContext.Set<Usuario>().Update(usuario);
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Usuario>().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Usuario>().FindAsync([id], cancellationToken);
    }
}
