using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Profiles.Queries.GetProfilesByUser;

public class GetProfilesByUserQueryHandler : IRequestHandler<GetProfilesByUserQuery, IEnumerable<ProfileDto>>
{
    private readonly IProfileRepository _profileRepository;

    public GetProfilesByUserQueryHandler(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<IEnumerable<ProfileDto>> Handle(GetProfilesByUserQuery request, CancellationToken cancellationToken)
    {
        var profiles = await _profileRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        
        return profiles.Select(p => new ProfileDto(p.Id, p.Handle, p.CreatedAt)).ToList();
    }
}
