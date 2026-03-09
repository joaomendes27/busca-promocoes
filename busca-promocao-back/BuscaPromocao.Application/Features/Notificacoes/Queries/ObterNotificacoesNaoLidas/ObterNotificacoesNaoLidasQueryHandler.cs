using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Notificacoes.Queries.ObterNotificacoesNaoLidas;

public class ObterNotificacoesNaoLidasQueryHandler : IRequestHandler<ObterNotificacoesNaoLidasQuery, IEnumerable<NotificacaoDto>>
{
    private readonly INotificacaoRepository _notificacaoRepository;

    public ObterNotificacoesNaoLidasQueryHandler(INotificacaoRepository notificacaoRepository)
    {
        _notificacaoRepository = notificacaoRepository;
    }

    public async Task<IEnumerable<NotificacaoDto>> Handle(ObterNotificacoesNaoLidasQuery request, CancellationToken cancellationToken)
    {
        var notificacoes = await _notificacaoRepository.ObterNaoLidasPorUsuarioIdAsync(request.UsuarioId, request.Dias, request.Produto, cancellationToken);
        
        return notificacoes.Select(n => new NotificacaoDto(
            n.Id, 
            n.Titulo, 
            n.Conteudo, 
            n.UrlTweet, 
            n.HandlePerfil, 
            n.PostadoEm)).ToList();
    }
}
