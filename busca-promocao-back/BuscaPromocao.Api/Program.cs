using System.Reflection;
using BuscaPromocao.Application;
using BuscaPromocao.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Banco de dados ──
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Application Layer (MediatR / CQRS) ──
builder.Services.AddApplicationServices();

// ── Controllers ──
builder.Services.AddControllers();

// ── Swagger / OpenAPI ──
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Inclui comentários XML na documentação do Swagger
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// ── Pipeline HTTP ──
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Busca Promoção API v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz: http://localhost:PORT/
    });
}

app.UseHttpsRedirection();

// app.UseAuthentication(); // TODO: Habilitar após implementar JWT
// app.UseAuthorization();

app.MapControllers();

app.Run();
