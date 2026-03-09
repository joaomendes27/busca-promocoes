using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Perfis.Commands.CriarPerfil;
using BuscaPromocao.Application.Features.Perfis.Commands.RemoverPerfil;
using BuscaPromocao.Application.Features.Perfis.Queries.ObterPerfisPorUsuario;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscaPromocao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PerfisController : ControllerBase
{
    private readonly IMediator _mediator;

    public PerfisController(IMediator mediator)
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

    [HttpGet]
    public async Task<IActionResult> ListarPerfisMonitorados(CancellationToken cancellationToken)
    {
        try
        {
            var query = new ObterPerfisPorUsuarioQuery(LerUsuarioAutenticado());
            var resultado = await _mediator.Send(query, cancellationToken);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AdicionarPerfilMonitorado([FromBody] CriarPerfilRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CriarPerfilCommand(LerUsuarioAutenticado(), request.HandlePerfil);
            var result = await _mediator.Send(command, cancellationToken);
            return StatusCode(201, new { Id = result, Mensagem = "Perfil de promoções adicionado para monitoramento." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoverPerfilMonitorado(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new RemoverPerfilCommand(id, LerUsuarioAutenticado());
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
    }
}

public sealed record CriarPerfilRequest(string HandlePerfil);
