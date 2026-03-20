# Journals Service

Manages academic journal definitions, sections, and editorial assignments.

**Port:** 4402
**Database:** Redis Stack (JSON documents via Redis.OM)
**API Framework:** FastEndpoints
**gRPC:** Client of Auth, Server exposing `IJournalService`

---

## Endpoints

| Method | Route | Description | Auth |
|---|---|---|---|
| POST | `/journals` | Create a new journal | EOF |

---

## Domain Model

```
Journal (Redis Document)
├── Id, Name, Description
├── ISSN (format: XXXX-XXXXX)
├── ChiefEditorId
└── Sections[]
    └── Section
        ├── Name
        └── SectionEditorId

Editor
├── PersonId, UserId
├── Affiliation
└── JournalAssignments[]
```

---

## gRPC Service

Journals exposes `IJournalService` for other services:

```
GetJournalByIdAsync(GetJournalByIdRequest)
IsEditorAssignedToJournalAsync(IsEditorAssignedToJournalRequest)
```

---

## Create Journal Flow

```
POST /journals (EOF token required)
    │
    ├── Validate ISSN format (regex: \d{4}-\d{3}[\dX])
    ├── Validate chief editor via gRPC → Auth service
    └── Save journal to Redis
```

---

## Key Behaviors

- Redis.OM handles index creation automatically on first access — no EF migrations needed
- Searchable and indexed fields for fast lookup
- Chief editor validated via gRPC call to Auth service on creation
- No SQL database — all data stored as JSON documents in Redis Stack
