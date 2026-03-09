using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Notificacoes.Commands.MarcarComoLida;
using BuscaPromocao.Application.Features.Notificacoes.Commands.RemoverNotificacao;
using BuscaPromocao.Application.Features.Notificacoes.Queries.ObterNotificacoesNaoLidas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscaPromocao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class NotificacoesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificacoesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid LerUsuarioAutenticado()
    {
        var claimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(claimId, out var usuarioId))
            return usuarioId;
            
        throw new UnauthorizedAccessException("Usuário não autenticado ou token inválido.");
    }

    [HttpGet("nao-lidas")]
    public async Task<IActionResult> ObterNaoLidas([FromQuery] int? dias, [FromQuery] string? produto, CancellationToken cancellationToken)
    {
        try
        {
            var query = new ObterNotificacoesNaoLidasQuery(LerUsuarioAutenticado(), dias, produto);
            var resultado = await _mediator.Send(query, cancellationToken);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
    }

    [HttpPatch("{id}/lida")]
    public async Task<IActionResult> MarcarComoLida(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            // Em um sistema real o Handler também validaria se a notificação pertence ao UsuarioId
            var command = new MarcarComoLidaCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoverNotificacao(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new RemoverNotificacaoCommand(id, LerUsuarioAutenticado());
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
    }
}
