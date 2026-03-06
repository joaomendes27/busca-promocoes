using System;
using System.Threading.Tasks;
using BuscaPromocao.Application.Eventos;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Infrastructure.Persistence;
using MailKit.Net.Smtp;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace BuscaPromocao.Notifier;

public sealed class PromocaoEncontradaConsumer : IConsumer<PromocaoEncontradaEvento>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PromocaoEncontradaConsumer> _logger;

    public PromocaoEncontradaConsumer(
        ApplicationDbContext dbContext,
        IConfiguration configuration,
        ILogger<PromocaoEncontradaConsumer> logger)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PromocaoEncontradaEvento> context)
    {
        var evento = context.Message;

        _logger.LogInformation(
            "Notificação recebida: Perfil={Handle}, Termo={Termo}, Tweet={Url}",
            evento.HandlePerfil, evento.Termo, evento.UrlTweet);

        await SalvarNotificacaoNoBancoAsync(evento);
        await EnviarEmailAsync(evento);
    }

    private async Task SalvarNotificacaoNoBancoAsync(PromocaoEncontradaEvento evento)
    {
        var notificacao = new Notificacao
        {
            Id = Guid.NewGuid(),
            Titulo = $"Promoção de {evento.Termo} encontrada!",
            Conteudo = evento.TextoTweet,
            UrlTweet = evento.UrlTweet,
            HandlePerfil = evento.HandlePerfil,
            PostadoEm = evento.PostadoEm,
            FoiLida = false,
            UsuarioId = evento.UsuarioId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Set<Notificacao>().Add(notificacao);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Notificação salva no banco para o usuário {UsuarioId}.", evento.UsuarioId);
    }

    private async Task EnviarEmailAsync(PromocaoEncontradaEvento evento)
    {
        try
        {
            var usuario = await _dbContext.Set<BuscaPromocao.Domain.Entities.Usuario>()
                .FirstOrDefaultAsync(u => u.Id == evento.UsuarioId);

            if (usuario == null)
            {
                _logger.LogWarning("Usuário {UsuarioId} não encontrado para envio de email.", evento.UsuarioId);
                return;
            }

            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var remetenteEmail = _configuration["Email:RemetenteEmail"] ?? string.Empty;
            var remetenteSenha = _configuration["Email:RemetenteSenha"] ?? string.Empty;
            var remetenteNome = _configuration["Email:RemetenteNome"] ?? "Busca Promoção";

            if (string.IsNullOrWhiteSpace(remetenteEmail) || string.IsNullOrWhiteSpace(remetenteSenha))
            {
                _logger.LogWarning("Configurações de email SMTP não definidas. Email não enviado.");
                return;
            }

            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress(remetenteNome, remetenteEmail));
            mensagem.To.Add(new MailboxAddress(usuario.Nome, usuario.Email));
            mensagem.Subject = $"🔥 Promoção encontrada: {evento.Termo}";

            mensagem.Body = new TextPart("html")
            {
                Text = $@"
                    <h2>Promoção encontrada!</h2>
                    <p><strong>Produto:</strong> {evento.Termo}</p>
                    <p><strong>Perfil:</strong> @{evento.HandlePerfil}</p>
                    <p><strong>Tweet:</strong> {evento.TextoTweet}</p>
                    <p><a href='{evento.UrlTweet}'>Ver tweet original</a></p>
                    <hr/>
                    <p><small>Busca Promoção - Monitoramento de promoções</small></p>"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(remetenteEmail, remetenteSenha);
            await client.SendAsync(mensagem);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email enviado para {Email} sobre promoção de {Termo}.", usuario.Email, evento.Termo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar email de notificação.");
        }
    }
}
