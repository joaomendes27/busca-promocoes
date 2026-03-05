using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Keywords.Commands.CreateKeyword;

public class CreateKeywordCommandHandler : IRequestHandler<CreateKeywordCommand, Guid>
{
    private readonly IKeywordRepository _keywordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateKeywordCommandHandler(IKeywordRepository keywordRepository, IUnitOfWork unitOfWork)
    {
        _keywordRepository = keywordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateKeywordCommand request, CancellationToken cancellationToken)
    {
        var keyword = new Keyword
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Term = request.Term,
            CreatedAt = DateTime.UtcNow
        };

        await _keywordRepository.AddAsync(keyword, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return keyword.Id;
    }
}
