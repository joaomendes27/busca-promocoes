using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Produtos.Queries.ObterProdutosPorUsuario;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BuscaPromocao.Application.Tests.Features.Produtos;

public class ObterProdutosPorUsuarioQueryHandlerTests
{
    private readonly Mock<IProdutoRepository> _repoMock;
    private readonly ObterProdutosPorUsuarioQueryHandler _handler;

    public ObterProdutosPorUsuarioQueryHandlerTests()
    {
        _repoMock = new Mock<IProdutoRepository>();
        _handler = new ObterProdutosPorUsuarioQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ComProdutosExistentes_DeveRetornarLista()
    {
        var usuarioId = Guid.NewGuid();
        var produtos = new List<Produto>
        {
            new() { Id = Guid.NewGuid(), Nome = "Air Fryer", UsuarioId = usuarioId, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Nome = "Notebook", UsuarioId = usuarioId, CreatedAt = DateTime.UtcNow }
        };

        _repoMock.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produtos);

        var resultado = await _handler.Handle(new ObterProdutosPorUsuarioQuery(usuarioId), CancellationToken.None);

        resultado.Should().HaveCount(2);
        resultado.First().Nome.Should().Be("Air Fryer");
    }

    [Fact]
    public async Task Handle_SemProdutos_DeveRetornarListaVazia()
    {
        var usuarioId = Guid.NewGuid();
        _repoMock.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Produto>());

        var resultado = await _handler.Handle(new ObterProdutosPorUsuarioQuery(usuarioId), CancellationToken.None);

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_DeveMapearCorretamenteParaDto()
    {
        var usuarioId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var dataCriacao = DateTime.UtcNow;
        var produtos = new List<Produto>
        {
            new() { Id = id, Nome = "Microondas", UsuarioId = usuarioId, CreatedAt = dataCriacao }
        };

        _repoMock.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produtos);

        var resultado = (await _handler.Handle(new ObterProdutosPorUsuarioQuery(usuarioId), CancellationToken.None)).ToList();

        resultado.First().Id.Should().Be(id);
        resultado.First().Nome.Should().Be("Microondas");
        resultado.First().CriadoEm.Should().Be(dataCriacao);
    }
}
