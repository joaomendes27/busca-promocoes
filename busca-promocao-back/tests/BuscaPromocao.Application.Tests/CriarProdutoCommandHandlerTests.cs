using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Produtos.Commands.CriarProduto;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BuscaPromocao.Application.Tests.Features.Produtos;

public class CriarProdutoCommandHandlerTests
{
    private readonly Mock<IProdutoRepository> _produtoRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CriarProdutoCommandHandler _handler;

    public CriarProdutoCommandHandlerTests()
    {
        _produtoRepoMock = new Mock<IProdutoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CriarProdutoCommandHandler(_produtoRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Deve_Criar_Produto_E_Retornar_Id()
    {
        var command = new CriarProdutoCommand(Guid.NewGuid(), "Air Fryer");
        _unitOfWorkMock.Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().NotBeEmpty();
        _produtoRepoMock.Verify(r => r.AdicionarAsync(
            It.Is<Produto>(p => p.Nome == "Air Fryer" && p.UsuarioId == command.UsuarioId),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Associar_Produto_Ao_UsuarioId_Correto()
    {
        var usuarioId = Guid.NewGuid();
        var command = new CriarProdutoCommand(usuarioId, "Notebook");
        _unitOfWorkMock.Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _produtoRepoMock.Verify(r => r.AdicionarAsync(
            It.Is<Produto>(p => p.UsuarioId == usuarioId),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
