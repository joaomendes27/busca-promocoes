using System;
using System.Collections.Generic;
using MediatR;

namespace BuscaPromocao.Application.Features.Profiles.Queries.GetProfilesByUser;

public record ProfileDto(Guid Id, string Handle, DateTime CreatedAt);

public record GetProfilesByUserQuery(Guid UserId) : IRequest<IEnumerable<ProfileDto>>;
