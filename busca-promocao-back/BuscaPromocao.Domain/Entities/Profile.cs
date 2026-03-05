using System;
using BuscaPromocao.Domain.Common;

namespace BuscaPromocao.Domain.Entities;

public class Profile : BaseEntity
{
    public string Handle { get; set; } = string.Empty; // e.g. "xetdaspromocoes"
    
    // Foreign Key
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
