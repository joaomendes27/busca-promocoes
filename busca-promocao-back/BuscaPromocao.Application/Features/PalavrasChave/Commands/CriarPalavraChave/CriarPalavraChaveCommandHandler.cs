using System;
using System.Threading;
using System.Threading.Tasks;
using BuscaPromocao.Domain.Entities;
using BuscaPromocao.Domain.Interfaces;
using MediatR;

namespace BuscaPromocao.Application.Features.PalavrasChave.Commands.CriarPalavraChave;

public class CriarPalavraChaveCommandHandler : IRequestHandler<CriarPalavraChaveCommand, Guid>
{
    private readonly IPalavraChaveRepository _palavraChaveRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarPalavraChaveCommandHandler(IPalavraChaveRepository palavraChaveRepository, IUnitOfWork unitOfWork)
    {
        _palavraChaveRepository = palavraChaveRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CriarPalavraChaveCommand request, CancellationToken cancellationToken)
    {
        var palavraChave = new PalavraChave
        {
            Id = Guid.NewGuid(),
            UsuarioId = request.UsuarioId,
            Termo = request.Termo,
            CreatedAt = DateTime.UtcNow
        };

        await _palavraChaveRepository.AdicionarAsync(palavraChave, cancellationToken);
        await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return palavraChave.Id;
    }
}
