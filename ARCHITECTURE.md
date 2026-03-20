# Architecture — Articles System

Overview of the system architecture, communication patterns, infrastructure, and design decisions.
For detailed documentation on each service, see the `docs/` folder.

---

## Service Documentation

| Service | Description | Doc |
|---|---|---|
| Auth | Users, login, JWT, gRPC person data | [docs/auth-service.md](./docs/auth-service.md) |
| Journals | Journal definitions, sections, editors | [docs/journals-service.md](./docs/journals-service.md) |
| Submission | Article submission & file upload workflow | [docs/submission-service.md](./docs/submission-service.md) |
| Review | Peer review invitations & evaluation | [docs/review-service.md](./docs/review-service.md) |
| ArticleHub | Published article discovery via GraphQL | [docs/articlehub-service.md](./docs/articlehub-service.md) |

---

## System Overview

```
┌─────────────────────────────────────────────────────────────────────┐
│                          CLIENT / POSTMAN                           │
└─────┬──────────┬──────────────┬──────────────┬──────────────────────┘
      │          │              │              │
   [4401]     [4402]         [4444]         [4445]        [4443]
      │          │              │              │              │
 ┌────▼────┐ ┌───▼────┐  ┌─────▼──────┐ ┌────▼────┐  ┌─────▼──────┐
 │  Auth   │ │Journals│  │ Submission │ │ Review  │  │ ArticleHub │
 │  .API   │ │  .API  │  │    .API    │ │  .API   │  │    .API    │
 └────┬────┘ └───┬────┘  └─────┬──────┘ └────┬────┘  └─────┬──────┘
      │          │              │              │              │
 [SQL Server] [Redis]    [SQL Server]    [SQL Server]   [PostgreSQL]
                         [MongoDB]       [MongoDB]      [Hasura/GraphQL]
```

---

## Communication Patterns

### Synchronous — gRPC

Used when a service needs data from another service **during request handling**.

```
Submission  ──► Auth     "Get person info for userId=5"
Submission  ──► Journals "Is editor X assigned to journal Y?"
Review      ──► Auth     "Get person info for reviewer email=..."
Journals    ──► Auth     "Get or create person for chief editor"
```

All gRPC contracts are defined code-first using `protobuf-net.Grpc`.
Internal Docker DNS used for service discovery (e.g. `http://auth-api:8081`).

### Asynchronous — RabbitMQ + MassTransit

Used for events that don't require an immediate response.

```
Submission publishes: ArticleApprovedForReviewEvent
    │
    ├──► Review    (receive article, prepare for peer review)
    └──► ArticleHub (create denormalized read record)
```

Queue names follow snake_case with service suffix:
`article_approved_for_review_submission`

---

## Infrastructure

| Container | Image | Port | Used By |
|---|---|---|---|
| `sqlserver` | `mcr.microsoft.com/mssql/server` | 1433 | Auth, Submission, Review |
| `journals-redisdb` | `redis/redis-stack` | 6379 | Journals |
| `mongo-gridfs` | `mongo:6.0` | 27017 | Submission, Review (files) |
| `postgres` | `postgres` | 5432 | ArticleHub |
| `articlehub-hasura` | `hasura/graphql-engine` | 8080 | ArticleHub (GraphQL) |
| `rabbitmq` | `rabbitmq:management` | 5672 / 15672 | Submission, Review, ArticleHub |

---

## Article Lifecycle

```
1. SETUP
   USERADMIN → POST /users      (create authors, editors, reviewers)
   EOF       → POST /journals   (create journal with sections)

2. SUBMISSION
   AUT    → POST /api/articles                        (create draft)
   CORAUT → POST /api/articles/{id}/authors           (assign co-authors)
   CORAUT → POST /api/articles/{id}/assets/manuscript:upload
   AUT    → POST /articles/{id}:submit                (submit for review)

3. EDITORIAL REVIEW
   EOF    → POST /api/articles/{id}:approve
            Stage: InitialApproved
            Publishes: ArticleApprovedForReviewEvent

4. PEER REVIEW (Review service)
   REVED  → POST /api/articles/{id}/invitations       (invite reviewer)
   REV    → POST /articles/{id}/invitations/{token}:accept

5. ARTICLEHUB SYNC (event-driven)
   Consumer receives ArticleApprovedForReviewEvent
   Creates denormalized article record in PostgreSQL
```

---

## BuildingBlocks

Shared libraries used across all services.

| Project | Purpose |
|---|---|
| `Articles.Abstractions` | Shared enums and interfaces across services |
| `Articles.Security` | JWT config, role constants, `AddJwtAuthentication()` |
| `Articles.Grpc.Contracts` | gRPC service contracts (`IPersonService`, `IJournalService`) |
| `Articles.IntegrationEvents.Contracts` | RabbitMQ event contracts and DTOs |
| `Blocks.AspNetCore` | `GlobalExceptionMiddleware`, filters, gRPC interceptors |
| `Blocks.Core` | `Guard`, options validation, `MaxLength` constants |
| `Blocks.Domain` | `Entity`, `AggregateRoot`, `ValueObject`, `DomainException` |
| `Blocks.EntityFrameworkCore` | `Repository<T>`, `CachedRepository`, EF extensions |
| `Blocks.Messaging` | MassTransit + RabbitMQ setup |
| `Blocks.Redis` | Redis.OM repository abstraction |
| `Blocks.MediatR` | `ValidationBehavior`, `SetUserIdBehavior` |
| `Blocks.Exceptions` | `BadRequestException`, `NotFoundException` |

---

## Design Patterns

- **Clean Architecture** — API → Application → Domain ← Persistence
- **Domain-Driven Design** — Aggregate roots, value objects, factory methods, domain events
- **CQRS with MediatR** — Commands and queries separated, pipeline behaviors
- **Partial classes** — Entities split into data (`Entities/`) and behavior (`Behaviors/`)
- **Repository pattern** — `Repository<T>` and `CachedRepository<T>` for lookup tables
- **Options validation** — All config validated at startup (fail-fast)
- **Event-driven** — Services decoupled via RabbitMQ integration events
