using System.Threading;
using System.Threading.Tasks;

namespace BuscaPromocao.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
