# ArticleHub Service

Read-only repository of approved and published articles. Provides flexible search via GraphQL powered by Hasura.

**Port:** 4443
**Database:** PostgreSQL + Hasura GraphQL Engine
**API Framework:** Carter
**Messaging:** MassTransit consumer ← RabbitMQ

---

## Endpoints

| Method | Route | Description | Auth |
|---|---|---|---|
| POST | `/api/articles/graphql` | Search and query articles via GraphQL | Required |

---

## Domain Model

Denormalized read model — optimized for querying, never written to directly.

```
Article
├── Id, Title, Type, Scope
├── Stage (Submitted / Accepted / Published)
├── DOI?
├── JournalId → Journal
└── Actors[]
    └── ArticleActor
        ├── PersonId → Person
        └── Role

Journal
└── Id, Name, ISSN

Person
└── Id, FullName, Email
```

---

## Event Consumer

ArticleHub is updated exclusively through events — never via direct API writes.

```
ArticleApprovedForReviewConsumer
    Receives: ArticleApprovedForReviewEvent (from Submission service)
    Action:
        ├── Create Article record in PostgreSQL
        ├── Create or link Journal record
        ├── Create or link Person records (authors, editors)
        └── Create ArticleActor records with roles
```

---

## GraphQL

Hasura automatically generates a full GraphQL API over PostgreSQL.

Example query capabilities:
- Filter by stage, journal, author, scope
- Paginate results
- Fetch nested actors and journal info in a single query

---

## Key Behaviors

- **Event-driven** — data synchronized from Submission via RabbitMQ, never written directly
- **Read-only** — no mutation endpoints exposed to clients
- **Denormalized** — data copied from multiple services into a single queryable store
- Hasura manages the GraphQL layer automatically
