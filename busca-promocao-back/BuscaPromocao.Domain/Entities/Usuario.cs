using System.Collections.Generic;
using BuscaPromocao.Domain.Common;

namespace BuscaPromocao.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;

    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    public ICollection<Perfil> Perfis { get; set; } = new List<Perfil>();
    public ICollection<Notificacao> Notificacoes { get; set; } = new List<Notificacao>();
}
