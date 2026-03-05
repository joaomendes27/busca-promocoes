using System;
using BuscaPromocao.Domain.Common;

namespace BuscaPromocao.Domain.Entities;

public class Perfil : BaseEntity
{
    public string HandlePerfil { get; set; } = string.Empty;
    
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
}
