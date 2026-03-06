using BuscaPromocao.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using BuscaPromocao.Application.Common.Interfaces;

namespace BuscaPromocao.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Perfil> Perfis => Set<Perfil>();
    public DbSet<Notificacao> Notificacoes => Set<Notificacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuario");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("criado_em");
            entity.Property(e => e.UpdatedAt).HasColumnName("atualizado_em");

            entity.Property(e => e.Nome).HasColumnName("nome").IsRequired().HasMaxLength(150);
            entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(150);
            entity.Property(e => e.SenhaHash).HasColumnName("senha_hash");
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("produto");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("criado_em");
            entity.Property(e => e.UpdatedAt).HasColumnName("atualizado_em");

            entity.Property(e => e.Nome).HasColumnName("nome").IsRequired().HasMaxLength(100);
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            
            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Produtos)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Perfil>(entity =>
        {
            entity.ToTable("perfil");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("criado_em");
            entity.Property(e => e.UpdatedAt).HasColumnName("atualizado_em");

            entity.Property(e => e.HandlePerfil).HasColumnName("handle_perfil").IsRequired().HasMaxLength(50);
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Perfis)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Notificacao>(entity =>
        {
            entity.ToTable("notificacao");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("criado_em");
            entity.Property(e => e.UpdatedAt).HasColumnName("atualizado_em");

            entity.Property(e => e.Titulo).HasColumnName("titulo").IsRequired().HasMaxLength(200);
            entity.Property(e => e.Conteudo).HasColumnName("conteudo").IsRequired().HasMaxLength(1000);
            entity.Property(e => e.UrlTweet).HasColumnName("url_tweet").HasMaxLength(500);
            entity.Property(e => e.HandlePerfil).HasColumnName("handle_perfil");
            entity.Property(e => e.PostadoEm).HasColumnName("postado_em");
            entity.Property(e => e.FoiLida).HasColumnName("foi_lida");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Notificacoes)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
