using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.Keywords.Queries.GetKeywordsByUser;

public class GetKeywordsByUserQueryHandler : IRequestHandler<GetKeywordsByUserQuery, IEnumerable<KeywordDto>>
{
    private readonly IKeywordRepository _keywordRepository;

    public GetKeywordsByUserQueryHandler(IKeywordRepository keywordRepository)
    {
        _keywordRepository = keywordRepository;
    }

    public async Task<IEnumerable<KeywordDto>> Handle(GetKeywordsByUserQuery request, CancellationToken cancellationToken)
    {
        var keywords = await _keywordRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        
        return keywords.Select(k => new KeywordDto(k.Id, k.Term, k.CreatedAt)).ToList();
    }
}
