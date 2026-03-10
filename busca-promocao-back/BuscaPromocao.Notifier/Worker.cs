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
            evento.HandlePerfil, evento.Produto, evento.UrlTweet);

        await SalvarNotificacaoNoBancoAsync(evento);
        await EnviarEmailAsync(evento);
    }

    private async Task SalvarNotificacaoNoBancoAsync(PromocaoEncontradaEvento evento)
    {
        var notificacao = new Notificacao
        {
            Id = Guid.NewGuid(),
            Titulo = $"Promoção de {evento.Produto} encontrada!",
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
            mensagem.Subject = $"🔥 Promoção encontrada: {evento.Produto}";

            mensagem.Body = new TextPart("html")
            {
                Text = $@"<!DOCTYPE html>
<html lang='pt-BR'>
<head>
  <meta charset='UTF-8' />
  <meta name='viewport' content='width=device-width, initial-scale=1.0' />
  <title>Promoção Encontrada</title>
</head>
<body style='margin:0;padding:0;background-color:#050505;font-family:Inter,Arial,sans-serif;color:#F3F4F6;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#050505;padding:24px 0;'>
    <tr>
      <td align='center'>
        <table width='380' cellpadding='0' cellspacing='0' style='max-width:380px;width:100%;'>

          <!-- Header -->
          <tr>
            <td style='border-bottom:2px solid #CDFA4C;padding-bottom:10px;margin-bottom:14px;'>
              <span style='font-size:18px;font-weight:700;letter-spacing:-0.02em;text-transform:uppercase;color:#F3F4F6;'>
                Busca <span style='color:#CDFA4C;'>Promoção</span>
              </span>
              <span style='display:block;font-size:10px;letter-spacing:2px;text-transform:uppercase;color:#9CA3AF;margin-top:2px;'>Radar de alertas</span>
            </td>
          </tr>

          <!-- Badge + título -->
          <tr>
            <td style='padding:14px 0 10px;'>
              <span style='display:inline-block;background:#CDFA4C;color:#050505;font-size:10px;font-weight:700;letter-spacing:2px;text-transform:uppercase;padding:3px 8px;margin-bottom:10px;'>🔥 PROMOÇÃO ENCONTRADA</span>
              <h1 style='margin:0;font-size:22px;font-weight:700;letter-spacing:-0.02em;text-transform:uppercase;color:#F3F4F6;line-height:1.1;'>{System.Net.WebUtility.HtmlEncode(evento.Produto)}</h1>
            </td>
          </tr>

          <!-- Card principal -->
          <tr>
            <td style='background:#121212;border:1px solid #333333;padding:16px;'>
              <table width='100%' cellpadding='0' cellspacing='0'>
                <tr>
                  <td style='padding-bottom:12px;border-bottom:1px solid #333333;'>
                    <span style='font-size:10px;letter-spacing:2px;text-transform:uppercase;color:#9CA3AF;'>Perfil monitorado</span>
                    <p style='margin:4px 0 0;font-size:15px;font-weight:600;color:#CDFA4C;'>&#64;{System.Net.WebUtility.HtmlEncode(evento.HandlePerfil)}</p>
                  </td>
                </tr>
                <tr>
                  <td style='padding:12px 0;border-bottom:1px solid #333333;'>
                    <span style='font-size:10px;letter-spacing:2px;text-transform:uppercase;color:#9CA3AF;'>Texto do post</span>
                    <p style='margin:4px 0 0;font-size:14px;color:#F3F4F6;line-height:1.5;'>&ldquo;{System.Net.WebUtility.HtmlEncode(evento.TextoTweet)}&rdquo;</p>
                  </td>
                </tr>
                <tr>
                  <td style='padding-top:12px;'>
                    <span style='font-size:10px;letter-spacing:2px;text-transform:uppercase;color:#9CA3AF;'>Publicado em</span>
                    <p style='margin:4px 0 0;font-size:13px;color:#F3F4F6;'>{evento.PostadoEm.ToLocalTime():dd/MM/yyyy HH:mm}</p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- CTA -->
          <tr>
            <td style='padding:16px 0 14px;'>
              <a href='{evento.UrlTweet}' target='_blank'
                style='display:inline-block;background:#CDFA4C;color:#050505;font-size:12px;font-weight:700;letter-spacing:2px;text-transform:uppercase;padding:12px 24px;text-decoration:none;'>
                VER POST ORIGINAL ↗
              </a>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style='border-top:1px solid #333333;padding-top:12px;'>
              <p style='margin:0;font-size:11px;color:#9CA3AF;'>Você recebeu este email porque está monitorando promoções no <strong style='color:#F3F4F6;'>Busca Promoção</strong>.</p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(remetenteEmail, remetenteSenha);
            await client.SendAsync(mensagem);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email enviado para {Email} sobre promoção de {Produto}.", usuario.Email, evento.Produto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar email de notificação.");
        }
    }
}
