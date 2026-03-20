# Review Service

Manages peer review invitations, reviewer assignments, and review evaluations.

**Port:** 4445
**Database:** SQL Server (`ReviewDb`) + MongoDB GridFS (files)
**API Framework:** Carter
**gRPC:** Client of Auth
**Messaging:** MassTransit consumer ← RabbitMQ

---

## Endpoints

| Method | Route | Description | Auth |
|---|---|---|---|
| POST | `/api/articles/{articleId}/invitations` | Invite a reviewer to evaluate an article | REVED / EOF |
| POST | `/articles/{articleId}/invitations/{token}:accept` | Accept a review invitation via token | Anonymous |

---

## Domain Model

```
Article
├── Title, Type, Scope, SubmittedDate
├── Stage
├── Editor (assigned review editor)
└── ReviewInvitations[]
    └── ReviewInvitation
        ├── ReviewerId
        ├── Token (unique per invite, used in accept link)
        ├── SentAt, AcceptedAt?
        └── Status (Pending / Accepted / Declined)

Reviewer
├── PersonId, UserId
└── Specializations[]
    └── ReviewerSpecialization
        └── ContributionArea (matches article scope)

Asset
└── Review documents
```

---

## Invite Reviewer Flow

```
POST /api/articles/{articleId}/invitations (REVED / EOF token required)
    │
    ├── Validate article exists and is in correct stage
    ├── Fetch reviewer person info via gRPC → Auth service
    ├── Generate unique invitation token
    ├── Create ReviewInvitation record
    ├── Send invitation email to reviewer (with accept link containing token)
    └── Save to SQL Server
```

---

## Accept Invitation Flow

```
POST /articles/{articleId}/invitations/{token}:accept (anonymous)
    │
    ├── Look up invitation by articleId + token
    ├── Validate token is valid and not expired
    ├── Mark invitation as Accepted
    ├── Assign reviewer to article
    └── Return confirmation
```

---

## Event Consumer

Receives `ArticleApprovedForReviewEvent` from Submission service via RabbitMQ:

```
ArticleApprovedForReviewConsumer
    Receives: ArticleApprovedForReviewEvent
    Action:   Creates Article record in ReviewDb with all actors and journal info
```

---

## Key Behaviors

- Invitation tokens are unique per reviewer per article
- Reviewers accept anonymously via token link (typically sent in email)
- Separate MongoDB GridFS bucket for review files (isolated from submission files)
- Reviewer specializations matched against article contribution areas
- Email notifications sent to reviewers on invitation
