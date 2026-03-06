using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Produtos.Queries.ObterProdutosPorUsuario;

public class ObterProdutosPorUsuarioQueryHandler : IRequestHandler<ObterProdutosPorUsuarioQuery, IEnumerable<ProdutoDto>>
{
    private readonly IProdutoRepository _produtoRepository;

    public ObterProdutosPorUsuarioQueryHandler(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<IEnumerable<ProdutoDto>> Handle(ObterProdutosPorUsuarioQuery request, CancellationToken cancellationToken)
    {
        var produtos = await _produtoRepository.ObterPorUsuarioIdAsync(request.UsuarioId, cancellationToken);
        
        return produtos.Select(p => new ProdutoDto(p.Id, p.Nome, p.CreatedAt)).ToList();
    }
}
