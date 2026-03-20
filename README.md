# Articles — Academic Journal Submission System

A cloud-native microservices platform for managing the complete lifecycle of academic journal submissions — from article creation and peer review to publication and discovery.

Built with **.NET 9**, following **Clean Architecture** and **Domain-Driven Design** principles.

---

## What is this?

The Articles system is a backend platform used by academic institutions to manage journal submissions. Authors submit articles, editorial offices review them, peer reviewers evaluate them, and approved articles are published to a searchable hub.

The system consists of **5 independent microservices**, each owning its own database and communicating via **gRPC** (synchronous) and **RabbitMQ** (async events).

---

## Services at a Glance

| Service | Purpose | Port (HTTP) | Database |
|---|---|---|---|
| **Auth** | Users, login, JWT tokens, gRPC person data | 4401 | SQL Server |
| **Journals** | Journal definitions, sections, editors | 4402 | Redis Stack |
| **Submission** | Article submission & file upload workflow | 4444 | SQL Server + MongoDB |
| **Review** | Peer review invitations & evaluation | 4445 | SQL Server + MongoDB |
| **ArticleHub** | Published article discovery via GraphQL | 4443 | PostgreSQL + Hasura |

---

## Quick Start

**Prerequisites:** Docker Desktop

```bash
# Start all services and infrastructure
docker compose up -d

# Rebuild and restart a specific service
docker compose up -d --build auth-api

# Stop everything
docker compose down
```

### Swagger UI
- **Auth:** http://localhost:4401/swagger
- **Journals:** http://localhost:4402/swagger

---

## Tech Stack

| Technology | Version | Used For |
|---|---|---|
| .NET | 9.0 | All services |
| FastEndpoints | 8.1.0 | Auth & Journals API layer |
| Carter | 9.0.0 | Review & ArticleHub API layer |
| Minimal APIs | — | Submission API layer |
| Entity Framework Core | 9.0 | SQL Server ORM |
| ASP.NET Core Identity | 9.0 | User & role management |
| Redis.OM | 1.1.0 | Journals document storage |
| MassTransit | 7.3.1 | RabbitMQ async messaging |
| gRPC (protobuf-net) | 1.2.2 | Synchronous service-to-service calls |
| MongoDB GridFS | 3.6.0 | File storage (manuscripts) |
| PostgreSQL + Hasura | — | ArticleHub + GraphQL |
| MediatR | 14.0.0 | CQRS pipeline (Submission) |
| FluentValidation | 12.1.1 | Input validation |
| Mapster | 7.4.0 | Object mapping |
| JWT Bearer | — | Authentication |
| Docker Compose | — | Local orchestration |

---

## Project Structure

```
src/
├── BuildingBlocks/                        # Shared libraries (cross-cutting concerns)
│   ├── Articles.Abstractions              # Shared enums, interfaces across services
│   ├── Articles.Security                  # JWT configuration, role constants
│   ├── Articles.Grpc.Contracts            # gRPC service contracts (IPersonService, IJournalService)
│   ├── Articles.IntegrationEvents.Contracts  # RabbitMQ event contracts
│   ├── Blocks.AspNetCore                  # Middleware, filters, exception handling
│   ├── Blocks.Core                        # Extensions, Guard, options validation
│   ├── Blocks.Domain                      # Base entity, aggregate root, value objects
│   ├── Blocks.EntityFrameworkCore         # EF Core repository, migrations, caching
│   ├── Blocks.Messaging                   # MassTransit/RabbitMQ setup
│   ├── Blocks.Redis                       # Redis.OM repository abstraction
│   ├── Blocks.FastEndpoints               # FastEndpoints helpers & Mapster integration
│   ├── Blocks.MediatR                     # MediatR pipeline behaviors
│   └── Blocks.Exceptions                  # HttpException, NotFoundException, BadRequestException
│
├── Services/
│   ├── Auth/                              # Authentication & user management
│   ├── Journals/                          # Journal definitions & editors
│   ├── Submission/                        # Article submission workflow
│   ├── Review/                            # Peer review process
│   └── ArticleHub/                        # Published article discovery
│
└── Modules/
    ├── EmailService/                      # SMTP email delivery
    └── FileStorage/                       # MongoDB GridFS file storage
```

---

## Roles & Authorization

| Role | Code | Description |
|---|---|---|
| User Admin | `USERADMIN` | Create and manage user accounts |
| Editorial Office | `EOF` | Editorial operations across services |
| Author | `AUT` | Submit articles |
| Corresponding Author | `CORAUT` | Primary article contact, upload files |
| Review Editor | `REVED` | Manage peer review process |
| Reviewer | `REV` | Evaluate submitted articles |

---

## Documentation

See [`ARCHITECTURE.md`](./ARCHITECTURE.md) for a full breakdown of:
- Every service in detail
- Business workflow end-to-end
- Communication patterns (gRPC + RabbitMQ)
- Infrastructure setup
- Design patterns used

---

## Postman Collection

Import the files in the `postman/` folder to test all endpoints.

**Setup:**
1. Import both files into Postman
2. Select the **Articles - Local** environment
3. Run **Login** under `Auth & Users`
4. Copy the `jwtToken` value from the response body
5. Paste it into `admin_token` → **Current value** in the environment
6. You can now test all authenticated endpoints
