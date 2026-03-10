using System;

namespace BuscaPromocao.Application.Eventos;

public sealed class PromocaoEncontradaEvento
{
    public Guid UsuarioId { get; set; }
    public string HandlePerfil { get; set; } = string.Empty;
    public string Produto { get; set; } = string.Empty;
    public string TextoTweet { get; set; } = string.Empty;
    public string UrlTweet { get; set; } = string.Empty;
    public DateTime PostadoEm { get; set; }
}
