using System;
using System.Collections.Generic;
using MediatR;

namespace BuscaPromocao.Application.Features.Notificacoes.Queries.ObterNotificacoesNaoLidas;

public record NotificacaoDto(
    Guid Id, 
    string Titulo, 
    string Conteudo, 
    string UrlTweet, 
    string HandlePerfil, 
    DateTime PostadoEm);

public record ObterNotificacoesNaoLidasQuery(Guid UsuarioId, int? Dias, string? Produto) : IRequest<IEnumerable<NotificacaoDto>>;
