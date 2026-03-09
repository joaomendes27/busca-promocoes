using System;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;
using BuscaPromocao.Application.Features.Promocoes.Queries.BuscaImediata;
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

    private Guid LerUsuarioAutenticado()
    {
        var claimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(claimId, out var usuarioId))
            return usuarioId;
            
        throw new UnauthorizedAccessException("Usuário não autenticado ou token inválido.");
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

    [HttpPost("busca-imediata")]
    public async Task<IActionResult> BuscaImediata([FromBody] BuscaImediataRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var query = new BuscaImediataQuery(LerUsuarioAutenticado(), request.Dias, request.Produtos);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Erro = "Ocorreu um erro interno na busca imediata.", Detalhe = ex.Message });
        }
    }
}

public class BuscaImediataRequest
{
    public int Dias { get; set; }
    public string[] Produtos { get; set; } = Array.Empty<string>();
}
