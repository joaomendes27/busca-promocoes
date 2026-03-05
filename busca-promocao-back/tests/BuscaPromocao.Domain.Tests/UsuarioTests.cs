using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Common;
using FluentAssertions;

namespace BuscaPromocao.Domain.Tests.Entities;

public class UsuarioTests
{
    [Fact]
    public void Deve_Criar_Usuario_Com_Propriedades_Padrao()
    {
        // Arrange & Act
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "João Mendes",
            Email = "joao@email.com",
            SenhaHash = "hash123"
        };

        // Assert
        usuario.Nome.Should().Be("João Mendes");
        usuario.Email.Should().Be("joao@email.com");
        usuario.SenhaHash.Should().Be("hash123");
        usuario.Id.Should().NotBeEmpty();
        usuario.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        usuario.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Deve_Inicializar_Colecoes_Vazias()
    {
        // Arrange & Act
        var usuario = new Usuario();

        // Assert
        usuario.PalavrasChave.Should().NotBeNull().And.BeEmpty();
        usuario.Perfis.Should().NotBeNull().And.BeEmpty();
        usuario.Notificacoes.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Deve_Adicionar_PalavraChave_Ao_Usuario()
    {
        // Arrange
        var usuario = new Usuario { Id = Guid.NewGuid(), Nome = "Teste", Email = "teste@email.com" };
        var palavraChave = new PalavraChave { Id = Guid.NewGuid(), Termo = "Air Fryer", UsuarioId = usuario.Id };

        // Act
        usuario.PalavrasChave.Add(palavraChave);

        // Assert
        usuario.PalavrasChave.Should().HaveCount(1);
        usuario.PalavrasChave.First().Termo.Should().Be("Air Fryer");
    }

    [Fact]
    public void Deve_Adicionar_Perfil_Ao_Usuario()
    {
        // Arrange
        var usuario = new Usuario { Id = Guid.NewGuid(), Nome = "Teste", Email = "teste@email.com" };
        var perfil = new Perfil { Id = Guid.NewGuid(), HandlePerfil = "xetdaspromocoes", UsuarioId = usuario.Id };

        // Act
        usuario.Perfis.Add(perfil);

        // Assert
        usuario.Perfis.Should().HaveCount(1);
        usuario.Perfis.First().HandlePerfil.Should().Be("xetdaspromocoes");
    }
}
