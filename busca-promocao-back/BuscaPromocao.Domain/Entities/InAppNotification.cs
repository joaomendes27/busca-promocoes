using System;
using BuscaPromocao.Domain.Common;

namespace BuscaPromocao.Domain.Entities;

public class InAppNotification : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string TweetUrl { get; set; } = string.Empty;
    public string ProfileHandle { get; set; } = string.Empty;
    public DateTime TweetPostedAt { get; set; }
    public bool IsRead { get; set; } = false;

    // Foreign Key
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
