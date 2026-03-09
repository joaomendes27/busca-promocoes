# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /src

# Copia a solution e todos os arquivos de projeto (.csproj) para restaurar dependências
COPY ["BuscaPromocao.sln", "./"]
COPY ["BuscaPromocao.Api/BuscaPromocao.Api.csproj", "BuscaPromocao.Api/"]
COPY ["BuscaPromocao.Application/BuscaPromocao.Application.csproj", "BuscaPromocao.Application/"]
COPY ["BuscaPromocao.Domain/BuscaPromocao.Domain.csproj", "BuscaPromocao.Domain/"]
COPY ["BuscaPromocao.Infrastructure/BuscaPromocao.Infrastructure.csproj", "BuscaPromocao.Infrastructure/"]
COPY ["BuscaPromocao.Crawler/BuscaPromocao.Crawler.csproj", "BuscaPromocao.Crawler/"]
COPY ["BuscaPromocao.Notifier/BuscaPromocao.Notifier.csproj", "BuscaPromocao.Notifier/"]

# Restaura pacotes (layer cache)
RUN dotnet restore "BuscaPromocao.Api/BuscaPromocao.Api.csproj"

# Copia todo o código-fonte restante
COPY . .

# Compila e publica a aplicação em modo Release
WORKDIR "/src/BuscaPromocao.Api"
RUN dotnet publish "BuscaPromocao.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080

# Adicionar usuário não-root por segurança
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

# Copia os artefatos compilados do estágio anterior
COPY --from=builder /app/publish .

# Define as variáveis de ambiente base
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Ponto de entrada
ENTRYPOINT ["dotnet", "BuscaPromocao.Api.dll"]
