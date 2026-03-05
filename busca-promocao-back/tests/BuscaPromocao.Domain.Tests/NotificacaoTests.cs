using BuscaPromocao.Domain.Entities;
using FluentAssertions;

namespace BuscaPromocao.Domain.Tests.Entities;

public class NotificacaoTests
{
    [Fact]
    public void Deve_Criar_Notificacao_Com_Valores_Padrao()
    {
        var notificacao = new Notificacao
        {
            Id = Guid.NewGuid(),
            Titulo = "Promoção encontrada!",
            Conteudo = "Air Fryer por R$ 199",
            UrlTweet = "https://twitter.com/xet/status/123",
            HandlePerfil = "xetdaspromocoes",
            PostadoEm = DateTime.UtcNow,
            UsuarioId = Guid.NewGuid()
        };

        notificacao.FoiLida.Should().BeFalse();
        notificacao.Titulo.Should().Be("Promoção encontrada!");
        notificacao.Conteudo.Should().Contain("Air Fryer");
        notificacao.UrlTweet.Should().StartWith("https://");
        notificacao.HandlePerfil.Should().Be("xetdaspromocoes");
    }

    [Fact]
    public void Deve_Marcar_Notificacao_Como_Lida()
    {
        var notificacao = new Notificacao { Id = Guid.NewGuid(), Titulo = "Teste", Conteudo = "Teste" };

        notificacao.FoiLida = true;

        notificacao.FoiLida.Should().BeTrue();
    }
}
