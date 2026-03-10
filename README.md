# 🚀 Busca Promoção - Guia de Execução

Este projeto utiliza **Docker** para orquestrar todos os serviços (Banco de Dados, Fila, Cache, Backend e Frontend).

## 🛠️ Como Iniciar o Projeto

Para rodar o sistema completo pela primeira vez ou após baixar atualizações:

```powershell
# Sobe todos os containers em background e recompila os códigos se necessário
docker compose up --build -d
```

Nas próximas vezes, se não houver mudanças no código, basta:

```powershell
docker compose up -d
```

---

## 🔗 Acessos Locais

| Serviço | URL |
| :--- | :--- |
| **Frontend (Angular)** | [http://localhost:4200](http://localhost:4200) |
| **Backend (Swagger)** | [http://localhost:5247/swagger](http://localhost:5247/swagger) |
| **RabbitMQ (Queue)** | [http://localhost:15672](http://localhost:15672) (guest/guest) |

---

## 📋 Comandos Úteis

### Parar o Projeto
```powershell
docker compose down
```

### Verificar Status dos Containers
```powershell
docker compose ps
```

### Ver Logs em Tempo Real
```powershell
# Ver logs de todos os serviços
docker compose logs -f

# Ver logs de um serviço específico (ex: api)
docker compose logs -f api
```

### Resetar Banco de Dados (Cuidado!)
```powershell
docker compose down -v
```

---

## 🛠️ Desenvolvimento Estruturado

- **Frontend**: Angular 17+ (Standalone Components)
- **Backend**: .NET 8 (Clean Architecture + MediatR)
- **Crawler**: Worker Service para monitoramento do X/Twitter via Nitter.
- **Banco**: PostgreSQL + Redis para Cache de Duplicidade.