using System;
using BuscaPromocao.Domain.Common;

namespace BuscaPromocao.Domain.Entities;

public class PalavraChave : BaseEntity
{
    public string Termo { get; set; } = string.Empty;
    
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
}
