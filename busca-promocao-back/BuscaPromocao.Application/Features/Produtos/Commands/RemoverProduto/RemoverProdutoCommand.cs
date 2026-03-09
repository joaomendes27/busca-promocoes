using System;
using MediatR;

namespace BuscaPromocao.Application.Features.Produtos.Commands.RemoverProduto;

public record RemoverProdutoCommand(Guid ProdutoId, Guid UsuarioId) : IRequest<bool>;
