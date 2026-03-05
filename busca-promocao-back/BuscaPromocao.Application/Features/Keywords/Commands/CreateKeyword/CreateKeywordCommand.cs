using System;
using MediatR;

namespace BuscaPromocao.Application.Features.Keywords.Commands.CreateKeyword;

public record CreateKeywordCommand(Guid UserId, string Term) : IRequest<Guid>;
