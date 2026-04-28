# UpStatus — Backend

Backend do **UpStatus**, dashboard de monitoramento ativo de URLs/APIs.

## Stack

- **.NET 8** + **FastEndpoints** (CQRS via command bus nativo)
- **MongoDB 7**
- **Redis 7**
- **SignalR**
- **JWT** + **BCrypt**
- **Docker** + **docker-compose**

## Arquitetura

Clean Architecture com 4 projetos:

```
UpStatus.Domain
UpStatus.Application
UpStatus.Infrastructure
UpStatus.Api
```

## Build local

```bash
# Da raiz do monorepo
cd backend
dotnet build UpStatus.Api/UpStatus.Api.csproj
dotnet run --project UpStatus.Api
```

## Build via Docker

Da raiz do monorepo (não desta pasta):

```bash
docker compose up --build
```
