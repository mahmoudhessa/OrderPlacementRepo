# Talabeyah Order Management System

## 🏗️ Tech Stack
- .NET Core 8 (Clean Architecture, DDD, CQRS, MediatR, SignalR)
- Angular (frontend)
- SQL Server
- Kafka
- Docker Compose

## 🚀 Quick Start (Docker Compose)
```bash
docker-compose up --build
```
- Frontend: http://localhost:4200
- Backend API/Swagger: http://localhost:5000/swagger

## 👤 Demo Users & Roles
| Username              | Password   | Role(s)             |
|-----------------------|------------|---------------------|
| admin@demo.com        | Passw0rd!  | Admin               |
| sales@demo.com        | Passw0rd!  | Sales               |
| auditor@demo.com      | Passw0rd!  | Auditor             |
| inventory@demo.com    | Passw0rd!  | InventoryManager    |

### Role Access Matrix
| Role              | Orders | Products | Audit Logs | Users |
|-------------------|--------|----------|------------|-------|
| **Admin**         | ✅     | ✅ (CRUD) | ✅         | ✅    |
| **Sales**         | ✅     | ✅ (view) | ❌         | ❌    |
| **Auditor**       | ❌     | ❌       | ✅         | ❌    |
| **InventoryManager** | ❌  | ✅ (CRUD) | ❌         | ❌    |

## 📝 API Endpoints
- `/api/auth/login` (POST): Login, returns JWT
- `/api/orders` (GET/POST): List/create orders
- `/api/products` (GET): List products
- `/api/auditlogs` (GET): List audit logs
- `/orderHub`: SignalR real-time order updates

## 🧪 Testing
- Unit tests: `src/Tests/` (NUnit)
- To run tests:
  ```bash
  dotnet test src/Tests/Tests.csproj
  ```
- Concurrency is tested by simulating two orders for the last item; only one should succeed.

## 🛠️ Manual Run (Dev)
- Backend: `dotnet run --project src/API`
- Frontend: `cd frontend && npm install && ng serve`
- Worker: `dotnet run --project src/Worker`

## 🗃️ Database Seeding
- Products and demo users/roles are seeded automatically on first run.

## 📡 Real-Time
- Orders are broadcast to all clients via SignalR after creation.

## 🛡️ Security
- JWT authentication, RBAC, CORS, CSP, and validation throughout.

---
**For any issues, see the code comments or contact the maintainer.**
