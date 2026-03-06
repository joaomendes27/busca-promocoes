using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Application.Features.Perfis.Queries.ObterPerfisPorUsuario;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BuscaPromocao.Application.Tests.Features.Perfis;

public class ObterPerfisPorUsuarioQueryHandlerTests
{
    private readonly Mock<IPerfilRepository> _repoMock;
    private readonly ObterPerfisPorUsuarioQueryHandler _handler;

    public ObterPerfisPorUsuarioQueryHandlerTests()
    {
        _repoMock = new Mock<IPerfilRepository>();
        _handler = new ObterPerfisPorUsuarioQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ComPerfisExistentes_DeveRetornarLista()
    {
        var usuarioId = Guid.NewGuid();
        var perfis = new List<Perfil>
        {
            new() { Id = Guid.NewGuid(), HandlePerfil = "xetdaspromocoes", UsuarioId = usuarioId, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), HandlePerfil = "ofertasdobom", UsuarioId = usuarioId, CreatedAt = DateTime.UtcNow }
        };

        _repoMock.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(perfis);

        var resultado = await _handler.Handle(new ObterPerfisPorUsuarioQuery(usuarioId), CancellationToken.None);

        resultado.Should().HaveCount(2);
        resultado.First().HandlePerfil.Should().Be("xetdaspromocoes");
    }

    [Fact]
    public async Task Handle_SemPerfis_DeveRetornarListaVazia()
    {
        var usuarioId = Guid.NewGuid();
        _repoMock.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Perfil>());

        var resultado = await _handler.Handle(new ObterPerfisPorUsuarioQuery(usuarioId), CancellationToken.None);

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_DeveMapearCorretamenteParaDto()
    {
        var usuarioId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var dataCriacao = DateTime.UtcNow;
        var perfis = new List<Perfil>
        {
            new() { Id = id, HandlePerfil = "xetdaspromocoes", UsuarioId = usuarioId, CreatedAt = dataCriacao }
        };

        _repoMock.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(perfis);

        var resultado = (await _handler.Handle(new ObterPerfisPorUsuarioQuery(usuarioId), CancellationToken.None)).ToList();

        resultado.First().Id.Should().Be(id);
        resultado.First().HandlePerfil.Should().Be("xetdaspromocoes");
        resultado.First().CriadoEm.Should().Be(dataCriacao);
    }
}
