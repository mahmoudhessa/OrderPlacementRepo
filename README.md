# Talabeyah Order Management System

A full-stack order management system built with Angular frontend and .NET Core backend, featuring real-time notifications, Kafka messaging, and Azure SQL Edge database.

## 🏗️ Architecture

- **Frontend**: Angular 16 with Angular Material UI
- **Backend**: .NET 8 with Clean Architecture
- **Database**: Azure SQL Edge (Mac) / MS SQL Server (Windows)
- **Messaging**: Apache Kafka
- **Real-time**: SignalR
- **Containerization**: Docker & Docker Compose

## 🚀 Quick Start

### Prerequisites

- **Docker Desktop** (with Docker Compose)
- **Node.js** (v18 or higher) - for local frontend development
- **.NET 8 SDK** - for local backend development (optional)

### 1. Clone the Repository

```bash
git clone <your-repository-url>
cd Talabeyah.OrderManagement
```

### 2. Quick Setup (Recommended)

#### For Mac/Linux:
```bash
./setup.sh
```

#### For Windows:
```cmd
setup.bat
```

### 3. Manual Setup (Alternative)

If you prefer to set up manually:

#### Install Frontend Dependencies
```bash
cd frontend
npm install
```

#### Run with Docker Compose

#### For Mac (Azure SQL Edge):
```bash
docker-compose -f docker-compose.azure-sql-edge.yml up --build -d
```

#### For Windows (MS SQL Server):
```bash
docker-compose up --build -d
```

### 4. Access the Application

- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5001
- **Swagger UI**: http://localhost:5001/swagger
- **Database**: localhost:1433

## 🛠️ Development Setup

### Frontend Development

```bash
cd frontend
npm install          # Install dependencies
npm start           # Start development server
```

### Backend Development

```bash
cd backend
dotnet restore      # Restore packages
dotnet build        # Build solution
cd API
dotnet run          # Run API
```

## 📁 Project Structure

```
├── frontend/                 # Angular application
│   ├── src/
│   │   ├── app/
│   │   │   ├── auth/        # Authentication components
│   │   │   ├── orders/      # Order management
│   │   │   ├── products/    # Product catalog
│   │   │   └── audit-logs/  # Audit trail
│   │   └── styles.scss      # Global styles
│   ├── package.json         # Frontend dependencies
│   └── angular.json         # Angular configuration
├── backend/                  # .NET solution
│   ├── API/                 # Web API project
│   ├── Application/         # Application layer (CQRS)
│   ├── Domain/              # Domain layer (entities, interfaces)
│   ├── Infrastructure/      # Infrastructure layer (EF, Kafka)
│   └── Worker/              # Background services
├── docker-compose.yml       # Windows configuration
├── docker-compose.azure-sql-edge.yml  # Mac configuration
└── README.md               # This file
```

## 🔧 Configuration

### Environment Variables

The application uses the following environment variables:

```env
# Database
ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=OrderManagement;User Id=sa;Password=YourStrongPassword123;TrustServerCertificate=true;

# JWT Authentication
JWT__SecretKey=YourSuperSecretKeyHereThatIsAtLeast32CharactersLong
JWT__Issuer=YourApp
JWT__Audience=YourApp
JWT__ExpirationMinutes=60

# Kafka
Kafka__BootstrapServers=kafka:29092
```

### Docker Compose Files

- **`docker-compose.yml`**: Windows configuration with MS SQL Server
- **`docker-compose.azure-sql-edge.yml`**: Mac configuration with Azure SQL Edge

## 🧪 Testing

### Frontend Tests
```bash
cd frontend
npm test
```

### Backend Tests
```bash
cd backend
dotnet test
```

## 📦 Dependencies

### Frontend Dependencies
- Angular 16
- Angular Material
- Angular CDK
- SignalR Client
- JWT Authentication
- RxJS

### Backend Dependencies
- .NET 8
- Entity Framework Core
- MediatR (CQRS)
- SignalR
- Confluent.Kafka
- FluentValidation
- Swagger/OpenAPI

## 🔄 Database Migrations

The database is automatically initialized when the application starts. If you need to run migrations manually:

```bash
cd backend/API
dotnet ef database update
```

## 🚨 Troubleshooting

### Common Issues

1. **Port conflicts**: Ensure ports 4200, 5001, and 1433 are available
2. **Database connection**: Check if the SQL Server container is running
3. **Frontend not loading**: Verify `npm install` was run in the frontend directory
4. **Docker issues**: Restart Docker Desktop and try again

### Logs

```bash
# View all container logs
docker-compose logs

# View specific service logs
docker-compose logs backend
docker-compose logs frontend
docker-compose logs sqlserver
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

## 🆘 Support

For issues and questions, please create an issue in the repository.

## Conclusion

This solution fully aligns with the Talabeyah Full Stack Challenge requirements:
- All core features (order creation, inventory management, Kafka integration, order listing, auto-cancel, Docker orchestration) are implemented and tested.
- The architecture strictly follows Clean Architecture and SOLID principles:
  - The API layer is responsible for HTTP and web concerns, using DTOs for all request/response payloads.
  - The Application layer is agnostic to transport and UI, containing only business logic and use cases (CQRS commands/queries, domain services).
  - The Domain layer models the core business entities, value objects, and rules.
  - This separation ensures maintainability, testability, and extensibility for future requirements or interfaces.
- The codebase is clean, well-structured, and easy to navigate, with meaningful commit history and clear boundaries between layers.

### User Accounts & Permission Matrix

| Email                | Password   | Roles                | Permissions                                 |
|----------------------|------------|----------------------|---------------------------------------------|
| admin@demo.com       | admin123   | Admin                | Full access: orders, products, audit logs   |
| sales@demo.com       | sales123   | Sales                | Create/view orders                          |
| auditor@demo.com     | auditor123 | Auditor              | View audit logs                             |
| inventory@demo.com   | inventory123| InventoryManager     | View/add products                           |

- Each user role is mapped to specific permissions in the frontend and backend, ensuring proper access control throughout the system.

### Platform
- This solution was built and tested on **MacOS** (Apple Silicon/ARM) using Azure SQL Edge for full compatibility.

---
For setup instructions, see the sections above. For any questions or access requests, please contact the repository owner.
