using BuscaPromocao.Domain.Interfaces;
using BuscaPromocao.Infrastructure.Auth;
using BuscaPromocao.Infrastructure.Persistence;
using BuscaPromocao.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuscaPromocao.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPalavraChaveRepository, PalavraChaveRepository>();
        services.AddScoped<IPerfilRepository, PerfilRepository>();
        services.AddScoped<INotificacaoRepository, NotificacaoRepository>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtProvider, JwtProvider>();

        return services;
    }
}
