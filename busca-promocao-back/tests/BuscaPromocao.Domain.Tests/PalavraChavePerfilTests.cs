using System;
using BuscaPromocao.Domain.Entities;
using FluentAssertions;

namespace BuscaPromocao.Domain.Tests.Entities;

public class PalavraChaveTests
{
    [Fact]
    public void Deve_Criar_PalavraChave_Com_Valores_Padrao()
    {
        var palavraChave = new PalavraChave();

        palavraChave.Termo.Should().BeEmpty();
        palavraChave.UsuarioId.Should().Be(Guid.Empty);
        palavraChave.Usuario.Should().BeNull();
    }

    [Fact]
    public void Deve_Atribuir_Termo_Corretamente()
    {
        var palavraChave = new PalavraChave { Termo = "Air Fryer" };

        palavraChave.Termo.Should().Be("Air Fryer");
    }

    [Fact]
    public void Deve_Associar_Ao_Usuario()
    {
        var usuarioId = Guid.NewGuid();
        var palavraChave = new PalavraChave
        {
            Termo = "Notebook",
            UsuarioId = usuarioId
        };

        palavraChave.UsuarioId.Should().Be(usuarioId);
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
