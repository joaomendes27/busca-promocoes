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
            entity.ToTable("usuario");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("criado_em");
            entity.Property(e => e.UpdatedAt).HasColumnName("atualizado_em");

            entity.Property(e => e.Name).HasColumnName("nome").IsRequired().HasMaxLength(150);
            entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(150);
            entity.Property(e => e.PasswordHash).HasColumnName("senha_hash");
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Keyword>(entity =>
        {
            entity.ToTable("palavra_chave");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("criado_em");
            entity.Property(e => e.UpdatedAt).HasColumnName("atualizado_em");

            entity.Property(e => e.Term).HasColumnName("termo").IsRequired().HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("usuario_id");
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Keywords)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.ToTable("perfil");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("criado_em");
            entity.Property(e => e.UpdatedAt).HasColumnName("atualizado_em");

            entity.Property(e => e.Handle).HasColumnName("handle_perfil").IsRequired().HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("usuario_id");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Profiles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InAppNotification>(entity =>
        {
            entity.ToTable("notificacao");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("criado_em");
            entity.Property(e => e.UpdatedAt).HasColumnName("atualizado_em");

            entity.Property(e => e.Title).HasColumnName("titulo").IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).HasColumnName("conteudo").IsRequired().HasMaxLength(1000);
            entity.Property(e => e.TweetUrl).HasColumnName("url_tweet").HasMaxLength(500);
            entity.Property(e => e.ProfileHandle).HasColumnName("handle_perfil");
            entity.Property(e => e.TweetPostedAt).HasColumnName("postado_em");
            entity.Property(e => e.IsRead).HasColumnName("foi_lida");
            entity.Property(e => e.UserId).HasColumnName("usuario_id");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
