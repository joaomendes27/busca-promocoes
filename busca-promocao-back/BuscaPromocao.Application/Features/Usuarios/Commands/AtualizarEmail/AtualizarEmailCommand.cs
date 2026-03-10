using System;
using MediatR;

namespace BuscaPromocao.Application.Features.Usuarios.Commands.AtualizarEmail;

public sealed record AtualizarEmailCommand(Guid UsuarioId, string NovoEmail, string SenhaAtual) : IRequest;
