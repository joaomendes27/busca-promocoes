using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;

namespace BuscaPromocao.Domain.Interfaces;

public interface INotificacaoRepository
{
    Task<IEnumerable<Notificacao>> ObterNaoLidasPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<Notificacao?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Notificacao notificacao, CancellationToken cancellationToken = default);
    void Atualizar(Notificacao notificacao);
}
