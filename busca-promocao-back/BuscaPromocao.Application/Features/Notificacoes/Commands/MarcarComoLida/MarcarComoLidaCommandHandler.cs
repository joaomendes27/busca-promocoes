using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Notificacoes.Commands.MarcarComoLida;

public class MarcarComoLidaCommandHandler : IRequestHandler<MarcarComoLidaCommand, bool>
{
    private readonly INotificacaoRepository _notificacaoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarcarComoLidaCommandHandler(
        INotificacaoRepository notificacaoRepository, 
        IUnitOfWork unitOfWork)
    {
        _notificacaoRepository = notificacaoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(MarcarComoLidaCommand request, CancellationToken cancellationToken)
    {
        var notificacao = await _notificacaoRepository.ObterPorIdAsync(request.NotificacaoId, cancellationToken);
        
        if (notificacao == null)
            return false;

        notificacao.FoiLida = true;
        
        _notificacaoRepository.Atualizar(notificacao);
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return true;
    }
}
