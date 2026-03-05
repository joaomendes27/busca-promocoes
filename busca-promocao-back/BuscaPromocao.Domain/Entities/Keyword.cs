using System;
using BuscaPromocao.Domain.Common;

namespace BuscaPromocao.Domain.Entities;

public class Keyword : BaseEntity
{
    public string Term { get; set; } = string.Empty;
    
    // Foreign Key
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
