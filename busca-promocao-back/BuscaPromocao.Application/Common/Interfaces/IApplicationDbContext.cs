using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuscaPromocao.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Usuario> Usuarios { get; }
    DbSet<PalavraChave> PalavrasChave { get; }
    DbSet<Perfil> Perfis { get; }
    DbSet<Notificacao> Notificacoes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
