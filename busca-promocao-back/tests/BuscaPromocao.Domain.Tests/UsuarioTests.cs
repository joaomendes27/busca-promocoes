using BuscaPromocao.Domain.Entities;
using FluentAssertions;

namespace BuscaPromocao.Domain.Tests.Entities;

public class UsuarioTests
{
    [Fact]
    public void Deve_Criar_Usuario_Com_Propriedades_Padrao()
    {
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "João Mendes",
            Email = "joao@email.com",
            SenhaHash = "hash123"
        };

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
        var usuario = new Usuario();

        usuario.Produtos.Should().NotBeNull().And.BeEmpty();
        usuario.Perfis.Should().NotBeNull().And.BeEmpty();
        usuario.Notificacoes.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Deve_Adicionar_Produto_Ao_Usuario()
    {
        var usuario = new Usuario { Id = Guid.NewGuid(), Nome = "Teste", Email = "teste@email.com" };
        var produto = new Produto { Id = Guid.NewGuid(), Nome = "Air Fryer", UsuarioId = usuario.Id };

        usuario.Produtos.Add(produto);

        usuario.Produtos.Should().HaveCount(1);
        usuario.Produtos.First().Nome.Should().Be("Air Fryer");
    }

    [Fact]
    public void Deve_Adicionar_Perfil_Ao_Usuario()
    {
        var usuario = new Usuario { Id = Guid.NewGuid(), Nome = "Teste", Email = "teste@email.com" };
        var perfil = new Perfil { Id = Guid.NewGuid(), HandlePerfil = "xetdaspromocoes", UsuarioId = usuario.Id };

        usuario.Perfis.Add(perfil);

        usuario.Perfis.Should().HaveCount(1);
        usuario.Perfis.First().HandlePerfil.Should().Be("xetdaspromocoes");
    }
}
