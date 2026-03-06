namespace BuscaPromocao.Domain.Interfaces;

public interface IJwtProvider
{
    string Generate(Entities.Usuario usuario);
}
