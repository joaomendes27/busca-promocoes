using System;

namespace BuscaPromocao.Application.Eventos;

public sealed record PromocaoEncontradaEvento(
    Guid UsuarioId,
    string HandlePerfil,
    string Produto,
    string TextoTweet,
    string UrlTweet,
    DateTime PostadoEm
);
