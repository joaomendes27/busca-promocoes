using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Auth.Commands.Login;
using BuscaPromocao.Application.Auth.Commands.RegistrarUsuario;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscaPromocao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("registrar")]
    [AllowAnonymous]
    public async Task<IActionResult> RegistrarUsuario([FromBody] RegistrarUsuarioCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var usuarioId = await _mediator.Send(command, cancellationToken);
            return StatusCode(201, new { UsuarioId = usuarioId, Mensagem = "Usuário registrado com sucesso." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { Erro = ex.Message });
        }
    }
}
