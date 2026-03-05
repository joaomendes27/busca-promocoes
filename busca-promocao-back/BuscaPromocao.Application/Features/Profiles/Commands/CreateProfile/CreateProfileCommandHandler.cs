using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Profiles.Commands.CreateProfile;

public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, Guid>
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProfileCommandHandler(IProfileRepository profileRepository, IUnitOfWork unitOfWork)
    {
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = new Profile
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Handle = request.Handle,
            CreatedAt = DateTime.UtcNow
        };

        await _profileRepository.AddAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return profile.Id;
    }
}
