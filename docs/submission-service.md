# Submission Service

Handles the full article submission workflow — creating articles, assigning authors, uploading manuscripts, and advancing stage transitions.

**Port:** 4444
**Database:** SQL Server (`SubmissionDb`) + MongoDB GridFS (files)
**API Framework:** Minimal APIs
**CQRS:** MediatR + FluentValidation
**gRPC:** Client of Auth and Journals
**Messaging:** MassTransit publisher → RabbitMQ

---

## Endpoints

| Method | Route | Description | Auth |
|---|---|---|---|
| POST | `/api/articles` | Create article in a journal | AUT |
| POST | `/articles/{articleId}:submit` | Submit article for editorial review | AUT |
| POST | `/api/articles/{articleId}:approve` | Approve article for peer review | EOF |
| PUT | `/api/articles/{articleId}/authors/{authorId}` | Assign existing user as author | CORAUT |
| POST | `/api/articles/{articleId}/authors` | Create and assign new author | CORAUT |
| POST | `/api/articles/{articleId}/assets/manuscript:upload` | Upload manuscript file | CORAUT |

> Colon syntax (`:submit`, `:approve`, `:upload`) follows Google API design style for actions.

---

## Domain Model

```
Journal
├── Id, Name, Abbreviation
└── Articles[]

Article (Aggregate Root)
├── Title, Scope, Type
├── Stage (Created → Submitted → InitialApproved / InitialRejected)
├── StageTransitions[] (full audit trail)
├── Actors[]
│   ├── ArticleActor
│   │   ├── PersonId, Role (AUT / CORAUT / EOF / REVED)
│   │   └── ContributionAreas[]
│   └── ArticleAuthor : ArticleActor (extended author info)
└── Assets[]
    └── Asset
        ├── AssetName (value object)
        ├── AssetType (enum)
        └── File (value object)
            ├── OriginalName, FileServerId
            ├── Size, FileName, FileExtension

Person
└── Author : Person
    ├── Degree, Discipline
    └── UserId? (linked to Auth user if exists)

AssetTypeDefinition (lookup table, cached in memory)
├── AssetType (enum PK)
├── MaxFileSize
├── AllowedFileExtensions
└── MaxCount
```

---

## Article Stages

```
Created (101)
    │
    ▼ [POST :submit]
Submitted (103)
    │
    ├── [POST :approve] ──► InitialApproved (105)
    │                           │
    │                           └── Publishes ArticleApprovedForReviewEvent
    │
    └── [POST :reject]  ──► InitialRejected (104)
```

---

## Commands (MediatR)

| Command | Description |
|---|---|
| `CreateArticleCommand` | Creates article, assigns logged-in user as corresponding author (gRPC lookup to Auth) |
| `AssignAuthorCommand` | Assigns an existing system user as co-author (gRPC lookup to Auth) |
| `CreateAndAssignAuthorCommand` | Creates a non-system person and assigns as author |
| `SubmitArticleCommand` | Advances article from Created → Submitted |
| `ApproveArticleCommand` | Advances article to InitialApproved, publishes integration event |
| `UploadManuscriptFileCommand` | Validates file type rules, stores file in MongoDB GridFS |

Each command has a **FluentValidation validator** and a **handler**. Both run through the MediatR pipeline:

```
Request → ValidationBehavior → SetUserIdBehavior → Handler → Response
```

---

## File Upload Flow

```
POST /api/articles/{articleId}/assets/manuscript:upload
    │
    ├── Load AssetTypeDefinition from cache (MaxSize, AllowedExtensions, MaxCount)
    ├── Validate file extension is allowed
    ├── Validate file size is within limit
    ├── Validate article doesn't already have max count of this asset type
    ├── Upload to MongoDB GridFS → get FileServerId
    ├── Create Asset entity with File value object
    └── Save to SQL Server
```

---

## Integration Event

When an article is approved, Submission publishes:

```csharp
ArticleApprovedForReviewEvent
{
    Article: ArticleDto (title, type, scope, stage, actors, assets, journal)
}
```

Consumed by: **Review**, **ArticleHub**

---

## Key Technical Details

- Table Per Hierarchy (TPH) for `Person`/`Author` and `ArticleActor`/`ArticleAuthor`
- `ComplexProperty` (EF 8+) used for value objects `Asset.Name` and `Asset.File`
- `AssetTypeDefinition` loaded once and cached in memory via `CachedRepository`
- JSON serialization for `AllowedFileExtensions` list stored in database column
