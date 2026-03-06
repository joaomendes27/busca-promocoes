using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Promocoes.Queries.BuscarPromocoesHistoricas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscaPromocao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PromocoesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PromocoesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("historico")]
    public async Task<IActionResult> BuscarHistorico([FromQuery] BuscarPromocoesHistoricasQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Erro = "Ocorreu um erro interno ao buscar o histórico de promoções.", Detalhe = ex.Message });
        }
    }
}
