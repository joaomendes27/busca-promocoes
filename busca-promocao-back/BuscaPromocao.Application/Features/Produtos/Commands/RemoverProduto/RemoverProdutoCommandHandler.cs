using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Produtos.Commands.RemoverProduto;

public sealed class RemoverProdutoCommandHandler : IRequestHandler<RemoverProdutoCommand, bool>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoverProdutoCommandHandler(IProdutoRepository produtoRepository, IUnitOfWork unitOfWork)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoverProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(request.ProdutoId, cancellationToken);

        if (produto == null || produto.UsuarioId != request.UsuarioId)
        {
            throw new Exception("Produto não encontrado ou não pertence a este usuário.");
        }

        _produtoRepository.Remover(produto);
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return true;
    }
}
