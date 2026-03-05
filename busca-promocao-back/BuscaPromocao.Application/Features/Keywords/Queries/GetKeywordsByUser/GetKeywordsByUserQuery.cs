using System;
using System.Collections.Generic;
using MediatR;

namespace BuscaPromocao.Application.Features.Keywords.Queries.GetKeywordsByUser;

public record KeywordDto(Guid Id, string Term, DateTime CreatedAt);

public record GetKeywordsByUserQuery(Guid UserId) : IRequest<IEnumerable<KeywordDto>>;
