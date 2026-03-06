using System;
using BuscaPromocao.Domain.Entities;
using FluentAssertions;

namespace BuscaPromocao.Domain.Tests.Entities;

public class ProdutoTests
{
    [Fact]
    public void Deve_Criar_Produto_Com_Valores_Padrao()
    {
        var produto = new Produto();

        produto.Nome.Should().BeEmpty();
        produto.UsuarioId.Should().Be(Guid.Empty);
        produto.Usuario.Should().BeNull();
    }

    [Fact]
    public void Deve_Atribuir_Nome_Corretamente()
    {
        var produto = new Produto { Nome = "Air Fryer" };

        produto.Nome.Should().Be("Air Fryer");
    }

    [Fact]
    public void Deve_Associar_Ao_Usuario()
    {
        var usuarioId = Guid.NewGuid();
        var produto = new Produto
        {
            Nome = "Notebook",
            UsuarioId = usuarioId
        };

        produto.UsuarioId.Should().Be(usuarioId);
    }
}

public class PerfilTests
{
    [Fact]
    public void Deve_Criar_Perfil_Com_Valores_Padrao()
    {
        var perfil = new Perfil();

        perfil.HandlePerfil.Should().BeEmpty();
        perfil.UsuarioId.Should().Be(Guid.Empty);
        perfil.Usuario.Should().BeNull();
    }

    [Fact]
    public void Deve_Atribuir_HandlePerfil_Corretamente()
    {
        var perfil = new Perfil { HandlePerfil = "xetdaspromocoes" };

        perfil.HandlePerfil.Should().Be("xetdaspromocoes");
    }

    [Fact]
    public void Deve_Associar_Ao_Usuario()
    {
        var usuarioId = Guid.NewGuid();
        var perfil = new Perfil
        {
            HandlePerfil = "ofertasdobom",
            UsuarioId = usuarioId
        };

        perfil.UsuarioId.Should().Be(usuarioId);
    }
}
