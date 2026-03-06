using System;
using BuscaPromocao.Domain.Common;

namespace BuscaPromocao.Domain.Entities;

public class Produto : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
}
