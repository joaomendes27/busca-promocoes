using BuscaPromocao.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using BuscaPromocao.Application.Common.Interfaces;

namespace BuscaPromocao.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Keyword> Keywords => Set<Keyword>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<InAppNotification> Notifications => Set<InAppNotification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Keyword>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Term).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Keywords)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Handle).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Profiles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InAppNotification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.TweetUrl).HasMaxLength(500);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
