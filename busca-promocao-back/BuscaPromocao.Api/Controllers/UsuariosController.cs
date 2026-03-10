using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Usuarios.Commands.AtualizarEmail;
using BuscaPromocao.Application.Features.Usuarios.Commands.AtualizarSenha;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscaPromocao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsuariosController(IMediator mediator)
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

    [HttpPut("AtualizarEmail")]
    public async Task<IActionResult> AtualizarEmail([FromBody] AtualizarEmailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new AtualizarEmailCommand(LerUsuarioAutenticado(), request.NovoEmail, request.SenhaAtual);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Mensagem = ex.Message });
        }
    }

    [HttpPut("AtualizarSenha")]
    public async Task<IActionResult> AtualizarSenha([FromBody] AtualizarSenhaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new AtualizarSenhaCommand(LerUsuarioAutenticado(), request.SenhaAtual, request.NovaSenha);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Mensagem = ex.Message });
        }
    }
}

public sealed class AtualizarEmailRequest
{
    public string NovoEmail { get; set; } = string.Empty;
    public string SenhaAtual { get; set; } = string.Empty;
}

public sealed class AtualizarSenhaRequest
{
    public string SenhaAtual { get; set; } = string.Empty;
    public string NovaSenha { get; set; } = string.Empty;
}
