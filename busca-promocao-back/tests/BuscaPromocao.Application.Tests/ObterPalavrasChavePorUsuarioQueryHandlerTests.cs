using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.PalavrasChave.Queries.ObterPalavrasChavePorUsuario;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BuscaPromocao.Application.Tests.Features.PalavrasChave;

public class ObterPalavrasChavePorUsuarioQueryHandlerTests
{
    private readonly Mock<IPalavraChaveRepository> _repoMock;
    private readonly ObterPalavrasChavePorUsuarioQueryHandler _handler;

    public ObterPalavrasChavePorUsuarioQueryHandlerTests()
    {
        _repoMock = new Mock<IPalavraChaveRepository>();
        _handler = new ObterPalavrasChavePorUsuarioQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ComPalavrasExistentes_DeveRetornarLista()
    {
        var usuarioId = Guid.NewGuid();
        var palavras = new List<PalavraChave>
        {
            new() { Id = Guid.NewGuid(), Termo = "Air Fryer", UsuarioId = usuarioId, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Termo = "Notebook", UsuarioId = usuarioId, CreatedAt = DateTime.UtcNow }
        };

        _repoMock.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(palavras);

        var resultado = await _handler.Handle(new ObterPalavrasChavePorUsuarioQuery(usuarioId), CancellationToken.None);

        resultado.Should().HaveCount(2);
        resultado.First().Termo.Should().Be("Air Fryer");
    }

    [Fact]
    public async Task Handle_SemPalavras_DeveRetornarListaVazia()
    {
        var usuarioId = Guid.NewGuid();
        _repoMock.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PalavraChave>());

        var resultado = await _handler.Handle(new ObterPalavrasChavePorUsuarioQuery(usuarioId), CancellationToken.None);

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_DeveMapearCorretamenteParaDto()
    {
        var usuarioId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var dataCriacao = DateTime.UtcNow;
        var palavras = new List<PalavraChave>
        {
            new() { Id = id, Termo = "Microondas", UsuarioId = usuarioId, CreatedAt = dataCriacao }
        };

        _repoMock.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(palavras);

        var resultado = (await _handler.Handle(new ObterPalavrasChavePorUsuarioQuery(usuarioId), CancellationToken.None)).ToList();

        resultado.First().Id.Should().Be(id);
        resultado.First().Termo.Should().Be("Microondas");
        resultado.First().CriadoEm.Should().Be(dataCriacao);
    }
}
