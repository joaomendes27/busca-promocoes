using System;
using System.Collections.Generic;
using MediatR;

namespace BuscaPromocao.Application.Features.Produtos.Queries.ObterProdutosPorUsuario;

public record ProdutoDto(Guid Id, string Nome, DateTime CriadoEm);

public record ObterProdutosPorUsuarioQuery(Guid UsuarioId) : IRequest<IEnumerable<ProdutoDto>>;
