using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;

namespace BuscaPromocao.Domain.Interfaces;

public interface IPalavraChaveRepository
{
    Task<PalavraChave?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PalavraChave>> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(PalavraChave palavraChave, CancellationToken cancellationToken = default);
    void Remover(PalavraChave palavraChave);
}
