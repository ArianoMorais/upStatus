# UpStatus

Dashboard de monitoramento ativo de URLs/APIs com atualização em tempo real, histórico, incidentes e métricas de uptime.

## Estrutura do monorepo

```
upStatus/
├── backend/    # API .NET 8 (FastEndpoints + MongoDB + Redis + SignalR)
├── frontend/   # Vue 3 + Vite + TypeScript
└── docker-compose.yml
```

## Stack

### Backend
- **.NET 8** + **FastEndpoints**
- **MongoDB 7**
- **Redis 7**
- **SignalR**
- **JWT** + **BCrypt**

### Frontend (planejado)
- **Vue 3** + **Vite** + **TypeScript**

### Infra
- **Docker** + **docker-compose**

## Como rodar

```bash
# Subir tudo (Mongo + Redis + API)
docker compose up --build

# Health check
curl http://localhost:5000/health
```

