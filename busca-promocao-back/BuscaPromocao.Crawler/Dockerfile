# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /src

# Restauração eficiente de dependências
COPY ["BuscaPromocao.sln", "./"]
COPY ["BuscaPromocao.Api/BuscaPromocao.Api.csproj", "BuscaPromocao.Api/"]
COPY ["BuscaPromocao.Application/BuscaPromocao.Application.csproj", "BuscaPromocao.Application/"]
COPY ["BuscaPromocao.Domain/BuscaPromocao.Domain.csproj", "BuscaPromocao.Domain/"]
COPY ["BuscaPromocao.Infrastructure/BuscaPromocao.Infrastructure.csproj", "BuscaPromocao.Infrastructure/"]
COPY ["BuscaPromocao.Crawler/BuscaPromocao.Crawler.csproj", "BuscaPromocao.Crawler/"]
COPY ["BuscaPromocao.Notifier/BuscaPromocao.Notifier.csproj", "BuscaPromocao.Notifier/"]

RUN dotnet restore "BuscaPromocao.Crawler/BuscaPromocao.Crawler.csproj"

# Copia código fonte
COPY . .

# Compilação
WORKDIR "/src/BuscaPromocao.Crawler"
RUN dotnet publish "BuscaPromocao.Crawler.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

# Segurança
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

ENV DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=builder /app/publish .
ENTRYPOINT ["dotnet", "BuscaPromocao.Crawler.dll"]
