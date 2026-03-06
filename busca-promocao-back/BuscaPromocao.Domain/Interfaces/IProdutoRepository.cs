using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;

namespace BuscaPromocao.Domain.Interfaces;

public interface IProdutoRepository
{
    Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Produto>> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Produto produto, CancellationToken cancellationToken = default);
    void Remover(Produto produto);
}
