using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.PalavrasChave.Queries.ObterPalavrasChavePorUsuario;

public class ObterPalavrasChavePorUsuarioQueryHandler : IRequestHandler<ObterPalavrasChavePorUsuarioQuery, IEnumerable<PalavraChaveDto>>
{
    private readonly IPalavraChaveRepository _palavraChaveRepository;

    public ObterPalavrasChavePorUsuarioQueryHandler(IPalavraChaveRepository palavraChaveRepository)
    {
        _palavraChaveRepository = palavraChaveRepository;
    }

    public async Task<IEnumerable<PalavraChaveDto>> Handle(ObterPalavrasChavePorUsuarioQuery request, CancellationToken cancellationToken)
    {
        var palavrasChave = await _palavraChaveRepository.ObterPorUsuarioIdAsync(request.UsuarioId, cancellationToken);
        
        return palavrasChave.Select(k => new PalavraChaveDto(k.Id, k.Termo, k.CreatedAt)).ToList();
    }
}
