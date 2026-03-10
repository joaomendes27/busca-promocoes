using System;
using MediatR;

namespace BuscaPromocao.Application.Features.Usuarios.Commands.AtualizarSenha;

public sealed record AtualizarSenhaCommand(Guid UsuarioId, string SenhaAtual, string NovaSenha) : IRequest;
