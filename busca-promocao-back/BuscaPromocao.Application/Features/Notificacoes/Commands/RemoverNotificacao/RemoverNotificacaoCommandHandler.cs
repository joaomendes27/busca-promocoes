using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Notificacoes.Commands.RemoverNotificacao;

public sealed class RemoverNotificacaoCommandHandler : IRequestHandler<RemoverNotificacaoCommand, bool>
{
    private readonly INotificacaoRepository _notificacaoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoverNotificacaoCommandHandler(INotificacaoRepository notificacaoRepository, IUnitOfWork unitOfWork)
    {
        _notificacaoRepository = notificacaoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoverNotificacaoCommand request, CancellationToken cancellationToken)
    {
        var notificacao = await _notificacaoRepository.ObterPorIdAsync(request.NotificacaoId, cancellationToken);

        if (notificacao == null || notificacao.UsuarioId != request.UsuarioId)
        {
            throw new Exception("Notificação não encontrada ou não pertence a este usuário.");
        }

        _notificacaoRepository.Remover(notificacao);
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return true;
    }
}
