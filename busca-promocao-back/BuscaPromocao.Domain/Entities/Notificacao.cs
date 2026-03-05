using System;
using BuscaPromocao.Domain.Common;

namespace BuscaPromocao.Domain.Entities;

public class Notificacao : BaseEntity
{
    public string Titulo { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public string UrlTweet { get; set; } = string.Empty;
    public string HandlePerfil { get; set; } = string.Empty;
    public DateTime PostadoEm { get; set; }
    public bool FoiLida { get; set; } = false;

    // Foreign Key
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
}
