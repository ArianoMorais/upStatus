# UpStatus

Dashboard de monitoramento ativo de URLs/APIs com atualização em tempo real, histórico, incidentes e métricas de uptime.

## Estrutura

```
upStatus/
├── backend/    # API .NET 8 (FastEndpoints + MongoDB + Redis + SignalR)
├── frontend/   # Vue 3 + Vite + TypeScript + PrimeVue
└── docker-compose.yml
```

## Stack

- **Backend**: .NET 8, FastEndpoints, MongoDB 7, Redis 7, SignalR, JWT + BCrypt
- **Frontend**: Vue 3 (`<script setup>`), Vite, Pinia, PrimeVue 4, Chart.js, `@microsoft/signalr`
- **Infra**: Docker Compose, Nginx (servindo o front + proxy de `/api` e `/hubs`)

## Pré-requisitos

- Docker Desktop (Windows/Mac) ou Docker Engine + Compose plugin (Linux)
- Pra desenvolver fora do Docker: .NET 8 SDK e Node.js 20+

---

## Rodando e testando em 5 passos

### 1. Clonar e configurar

```bash
git clone <repo>
cd upStatus
cp backend/.env.example backend/.env
```

Os defaults do `.env` já funcionam pra ambiente local. Variáveis que você pode querer trocar:

| Variável | Default | O que é |
|---|---|---|
| `Jwt__Secret` | string fixa | Segredo do JWT (mínimo 32 chars) |
| `Seed__AdminEmail` | `admin@upstatus.local` | E-mail do admin semeado no startup |
| `Seed__AdminPassword` | `admin123` | Senha do admin |

### 2. Subir o stack

```bash
docker compose up -d --build
```

Sobe Mongo, Redis, API, Frontend e um alvo de teste (`demo-target`).

### 3. Acessar

- **Frontend**: http://localhost:8080
- **Swagger**: http://localhost:5000/swagger
- **Login**: `admin@upstatus.local` / `admin123`

### 4. Cadastrar um monitor de teste

Na tela **Monitores → Novo monitor**:

- Nome: `Demo`
- URL: `http://demo-target/`
- Intervalo: `15` segundos (pra acelerar o teste)

Em ~15s o status vira **Operacional**.

### 5. Derrubar o alvo e ver a aplicação reagir

```bash
docker compose stop demo-target
```

Em ~45s (3 falhas consecutivas) você vai ver, sem dar refresh:

- Status do monitor virando **Fora do ar**
- Toast vermelho aparecendo
- Badge da sidebar de Incidentes incrementando
- Incidente aparecendo na lista

Pra simular a recuperação:

```bash
docker compose start demo-target
```

Após 2 sucessos seguidos o incidente fecha sozinho e o status volta pra **Operacional**.

---

## Comandos úteis

```bash
docker compose logs -f api          # logs da API
docker compose up -d --build api    # rebuild só da API
docker compose down                 # para tudo
docker compose down -v              # para tudo e zera Mongo/Redis
```

## Modo desenvolvimento (hot reload)

Sobe só as dependências no Docker e roda API e front nativos:

```bash
docker compose up -d mongo redis demo-target
```

**Backend** (porta `5137`):

```bash
cd backend
dotnet run --project UpStatus.Api
```

**Frontend** (porta `5173`):

```bash
cd frontend
npm install
npm run dev
```

> O Vite já está com proxy pra `localhost:5137`. Mantenha `Cors__AllowedOrigins__0=http://localhost:5173` no `.env`.

---

## Como o monitoramento funciona

1. Você cadastra um monitor (URL, intervalo, timeout, regras de incidente).
2. Um worker em background roda os checks devidos a cada tick (`HEAD`/`GET`/`POST` configurável).
3. Cada check vira um registro (success/degraded/failure/timeout) e o status do monitor é recalculado.
4. Após `N` falhas consecutivas (default 3) abre incidente. Após `M` sucessos (default 2) o incidente é resolvido automaticamente.
5. Toda mudança é empurrada via SignalR (`/hubs/monitoring`) pros clientes conectados.

## Endpoints principais

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/auth/login` · `/auth/me` | Auth |
| `GET` `POST` `PUT` `DELETE` | `/monitors` · `/monitors/{id}` | CRUD de monitores |
| `POST` | `/monitors/{id}/pause` · `/resume` | Pausa/retoma |
| `GET` | `/monitors/{id}/checks` · `/uptime?range=1h\|24h\|7d\|30d` | Histórico e métricas |
| `GET` | `/incidents` · `/incidents/{id}` | Incidentes |
| `POST` | `/incidents/{id}/acknowledge` · `/resolve` · `/comments` | Ações no incidente |

Hub SignalR: `/hubs/monitoring`. Eventos: `monitor.status_changed`, `check.recorded`, `incident.opened`, `incident.resolved`, `incident.acknowledged`, `incident.commented`.

## Troubleshooting

**Monitorar algo rodando na minha máquina (`localhost:3000`).**
Dentro do container, `localhost` é o próprio container. Use `http://host.docker.internal:<porta>/` (Docker Desktop) ou adicione `extra_hosts: ["host.docker.internal:host-gateway"]` no serviço `api` (Linux).

**Erro de CORS no navegador.**
Confira se a origem (`http://localhost:8080` ou `http://localhost:5173`) está em `Cors__AllowedOrigins`.

**Portas 27017/6379/5000/8080 ocupadas.**
Pare o serviço local que está usando ou ajuste o mapeamento no `docker-compose.yml`.
