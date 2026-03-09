using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscaPromocao.Infrastructure.Persistence.Repositories;

public sealed class NotificacaoRepository : INotificacaoRepository
{
    private readonly ApplicationDbContext _dbContext;

    public NotificacaoRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AdicionarAsync(Notificacao notificacao, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<Notificacao>().AddAsync(notificacao, cancellationToken);
    }

    public void Atualizar(Notificacao notificacao)
    {
        _dbContext.Set<Notificacao>().Update(notificacao);
    }

    public async Task<IEnumerable<Notificacao>> ObterNaoLidasPorUsuarioIdAsync(Guid usuarioId, int? dias = null, string? produto = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<Notificacao>().Where(n => n.UsuarioId == usuarioId && !n.FoiLida);

        if (dias.HasValue)
        {
            var dataCorte = DateTime.UtcNow.AddDays(-dias.Value);
            query = query.Where(n => n.PostadoEm >= dataCorte);
        }

        if (!string.IsNullOrWhiteSpace(produto))
        {
            var pUpper = produto.ToUpper();
            query = query.Where(n => n.Titulo.ToUpper().Contains(pUpper) || n.Conteudo.ToUpper().Contains(pUpper));
        }

        return await query.OrderByDescending(n => n.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<Notificacao?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Notificacao>().FindAsync([id], cancellationToken);
    }

    public void Remover(Notificacao notificacao)
    {
        _dbContext.Set<Notificacao>().Remove(notificacao);
    }
}
