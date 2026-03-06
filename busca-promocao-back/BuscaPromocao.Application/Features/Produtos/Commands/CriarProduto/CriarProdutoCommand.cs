using System;
using MediatR;

namespace BuscaPromocao.Application.Features.Produtos.Commands.CriarProduto;

public record CriarProdutoCommand(Guid UsuarioId, string Nome) : IRequest<Guid>;
