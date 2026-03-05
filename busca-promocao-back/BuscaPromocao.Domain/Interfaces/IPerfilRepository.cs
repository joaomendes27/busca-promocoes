using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;

namespace BuscaPromocao.Domain.Interfaces;

public interface IPerfilRepository
{
    Task<Perfil?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Perfil>> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Perfil>> ObterTodosAtivosAsync(CancellationToken cancellationToken = default);
    Task AdicionarAsync(Perfil perfil, CancellationToken cancellationToken = default);
    void Remover(Perfil perfil);
}
