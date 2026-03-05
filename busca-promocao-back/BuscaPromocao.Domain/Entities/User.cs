using System.Collections.Generic;
using BuscaPromocao.Domain.Common;

namespace BuscaPromocao.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<Keyword> Keywords { get; set; } = new List<Keyword>();
    public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
    public ICollection<InAppNotification> Notifications { get; set; } = new List<InAppNotification>();
}
