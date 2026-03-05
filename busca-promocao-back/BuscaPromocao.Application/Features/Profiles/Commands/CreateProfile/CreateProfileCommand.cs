using System;
using MediatR;

namespace BuscaPromocao.Application.Features.Profiles.Commands.CreateProfile;

public record CreateProfileCommand(Guid UserId, string Handle) : IRequest<Guid>;
