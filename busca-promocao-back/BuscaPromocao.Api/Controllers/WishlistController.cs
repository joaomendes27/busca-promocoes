using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Produtos.Commands.CriarProduto;
using BuscaPromocao.Application.Features.Produtos.Queries.ObterProdutosPorUsuario;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscaPromocao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class WishlistController : ControllerBase
{
    private readonly IMediator _mediator;

    public WishlistController(IMediator mediator)
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
    public async Task<IActionResult> ListarProdutosWishlist(CancellationToken cancellationToken)
    {
        try
        {
            var query = new ObterProdutosPorUsuarioQuery(LerUsuarioAutenticado());
            var resultado = await _mediator.Send(query, cancellationToken);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AdicionarProdutoWishlist([FromBody] CriarProdutoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CriarProdutoCommand(LerUsuarioAutenticado(), request.NomeProduto);
            var result = await _mediator.Send(command, cancellationToken);
            return StatusCode(201, new { Id = result, Mensagem = "Produto adicionado à sua wishlist com sucesso." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
    }
}

public sealed record CriarProdutoRequest(string NomeProduto);
