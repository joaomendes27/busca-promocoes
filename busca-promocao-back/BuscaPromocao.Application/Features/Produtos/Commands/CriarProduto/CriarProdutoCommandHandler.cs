using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Produtos.Commands.CriarProduto;

public class CriarProdutoCommandHandler : IRequestHandler<CriarProdutoCommand, Guid>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarProdutoCommandHandler(IProdutoRepository produtoRepository, IUnitOfWork unitOfWork)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CriarProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            UsuarioId = request.UsuarioId,
            Nome = request.Nome,
            CreatedAt = DateTime.UtcNow
        };

        await _produtoRepository.AdicionarAsync(produto, cancellationToken);
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return produto.Id;
    }
}
