# Artikelinskickningssystem - Projektguide

## Vad bygger vi?

Ett system för att hantera inskickning av vetenskapliga artiklar till tidskrifter. Tänk dig flödet som en forskare går igenom: man skriver en artikel, väljer en tidskrift, skickar in den, laddar upp manuskript och tilldelar medförfattare med olika roller.

Systemet består av tre tjänster: **Auth** (autentisering och användare), **Submission** (artikelinskickning) och **Journals** (tidskriftshantering, tidig fas). Dessutom finns två moduler för e-post och fillagring.

## Övergripande arkitektur

Projektet följer **Clean Architecture** med **Domain-Driven Design (DDD)**. Det innebär att koden är uppdelad i tydliga lager där varje lager har ett specifikt ansvar. Det innersta lagret (Domain) vet ingenting om det yttersta (API).

Varje tjänst följer samma lagerstruktur:

```
┌─────────────────────────────────────┐
│              Service.API            │  ← HTTP-endpoints, tar emot requests
├─────────────────────────────────────┤
│          Service.Application        │  ← Kommandon, validering, affärsflöden
├─────────────────────────────────────┤
│            Service.Domain           │  ← Entiteter, affärsregler, value objects
├─────────────────────────────────────┤
│          Service.Persistence        │  ← Databas, EF Core, repositories
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│           BuildingBlocks            │  ← Delade bibliotek som alla tjänster använder
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│             Modules                 │  ← E-post, fillagring (konsumeras av tjänster)
└─────────────────────────────────────┘
```

---

## Mappstruktur

```
src/
├── BuildingBlocks/                  # Återanvändbara bibliotek
│   ├── Articles.Abstractions/       # Enums och gemensamma typer för artikeldomänen
│   ├── Articles.Security/           # JWT-konfiguration, rollbaserad behörighet
│   ├── Blocks.AspNetCore/           # HTTP-hjälpare (BaseUrl, GetClientIpAddress)
│   ├── Blocks.Core/                 # Guard, MaxLength-konstanter, extensions, cache
│   ├── Blocks.Domain/               # IEntity, Entity, ValueObject, DomainException
│   ├── Blocks.EntityFramework/      # Repository, CachedRepository, EF-hjälpare
│   ├── Blocks.Exceptions/           # HTTP-exceptions (NotFoundException, BadRequestException)
│   └── Blocks.MediatR/              # MediatR pipeline (validering, användarhantering)
│
├── Modules/                         # Fristående moduler
│   ├── EmailService/
│   │   ├── EmailService.Contracts/  # IEmailService, EmailMessage, EmailOptions
│   │   └── EmailService.Smtp/       # MailKit-implementation
│   └── FileStorage/
│       ├── FileStorage.Contracts/   # IFileService, UploadResponse
│       └── FileStorage.MongoGridFS/ # MongoDB GridFS-implementation
│
└── Services/
    ├── Auth/                        # Autentiseringstjänst
    │   ├── Auth.API/                # FastEndpoints: POST /users, POST /login
    │   ├── Auth.Application/        # TokenFactory (JWT + RefreshToken)
    │   ├── Auth.Domain/             # User, Role, UserRole, RefreshToken
    │   └── Auth.Persistence/        # AuthDBContext (ASP.NET Core Identity)
    │
    ├── Journals/                    # Tidskriftstjänst (tidig fas)
    │   ├── Journals.API/            # POST /journals (ej färdigimplementerat)
    │   ├── Journals.Domain/         # (tom)
    │   └── Journals.Persistence/    # (tom)
    │
    └── Submission/                  # Inskickningstjänst (mest komplett)
        ├── Submission.API/          # Minimal API-endpoints
        ├── Submission.Application/  # CQRS-kommandon med FluentValidation
        ├── Submission.Domain/       # Domänmodell med entiteter och value objects
        └── Submission.Persistence/  # EF Core, repositories, konfigurationer
```

---

## Tjänst: Auth

Hanterar användare, roller, inloggning och JWT-tokens. Bygger på **ASP.NET Core Identity**.

### Domänmodell

- **User** - Utökar `IdentityUser<int>` med `FirstName`, `LastName`, `Gender`, `HonorificTitle` (value object), `ProfessionalProfile` (value object), `PictureUrl`, `RegistrationDate`, `LastLogin`. Factory-metod: `User.Create(IUserCreationInfo)`.
- **Role** - Utökar `IdentityRole<int>` med `Type: UserRoleType` och `Description`.
- **UserRole** - Utökar `IdentityUserRole<int>` med `StartDate` och `ExpiringDate`. Validerar att startdatum är idag eller framåt.
- **RefreshToken** - `Token`, `CreatedByIp`, `ExpiresOn`, `RevokedOn`.

### Value objects (Auth)

- **HonorificTitle** - (Mr, Mrs, Dr, Prof) som `StringValueObject`.
- **ProfessionalProfile** - Position, CompanyName, Affiliation som `ValueObject`.

### Domain Events

- **UserCreated** - Publiceras vid ny användare. Hanteras av `SendConfirmationEmailOnUserCreatedHandler` som skickar bekräftelsemail med lösenordsåterställningslänk.

### Endpoints (Auth)

| Metod | URL | Beskrivning | Kräver roll |
|---|---|---|---|
| POST | `/users` | Skapa ny användare | USERADMIN |
| POST | `/login` | Logga in, få JWT + refresh token | Anonym |

### Tekniska detaljer

- Använder **FastEndpoints** (inte Minimal API).
- JWT-tokens genereras med HS256 (claims: sub, email, jti, iat, name, role).
- Refresh tokens: 64 byte kryptografiskt slumpmässiga, 7 dagars utgångstid.
- **Mapster** används för objektmappning i `UserRole.Create()`.
- Identity-konfiguration: 5 misslyckade inloggningsförsök → 5 min lockout.

---

## Tjänst: Submission

Den mest kompletta tjänsten. Hanterar artikelinskickning, författartilldelning och filuppladdning.

### Domänmodell

#### Entiteter och hur de hör ihop

```
Journal (Tidskrift)
 │  namn, förkortning
 │
 └──► Article (Artikel)
      │  titel, scope, typ, stadie
      │
      ├──► ArticleActor (Koppling: Artikel ↔ Person + Roll)
      │    │  roll (AUT, CORAUT, EOF)
      │    │
      │    └──► ArticleAuthor (Utökning med bidragsområden)
      │           t.ex. OriginalDraft, Methodology, Investigation
      │
      └──► Asset (Bifogad fil)
           │  namn, typ (Manuscript, Figure, etc.)
           │
           └──► File (value object: filnamn, storlek, extension, serverId)

AssetTypeDefinition (Uppslagstabell)
   maxFilstorlek, tillåtna filändelser, maxAntal

Person (Grundperson)
 │  förnamn, efternamn, e-post, tillhörighet, userId?
 │
 └──► Author (Utökning med akademisk info)
        examen, disciplin
```

#### Förklaring av entiteterna

- **Journal** - En vetenskaplig tidskrift. Skapar artiklar via sin `CreateArticle()`-metod.
- **Article** - En artikel som skickas in. Har titel, scope, typ och stadie. Hanterar författartilldelning (`AssignAuthor`) och skapande av assets (`CreateAsset`).
- **Person** - En person i systemet med kontaktuppgifter. Kan vara kopplad till en systemanvändare via `UserId`.
- **Author** - En person med akademisk info (examen, disciplin). Ärver från Person. Factory-metod: `Author.Create()`.
- **ArticleActor** - Kopplingstabell: artikel ↔ person + roll. Composite PK: `(ArticleId, PersonId, Role)`.
- **ArticleAuthor** - En ArticleActor som också har bidragsområden.
- **Asset** - En fil kopplad till en artikel (t.ex. manuskript). Har `AssetName` och `File` som value objects.
- **AssetTypeDefinition** - Uppslagsentitet (`EnumEntity<AssetType>`) som definierar regler per filtyp: max storlek, tillåtna filändelser, max antal. Cacheas via `CachedRepository`.

### Value objects (Submission)

| Value object | Beskrivning |
|---|---|
| `EmailAddress` | E-post med regex-validering. Privat konstruktor + `Create()`. |
| `AssetName` | Namn härlett från asset-typen. |
| `FileName` | Filnamn på formatet `{AssetName}.{extension}`. |
| `FileExtension` | Filände, validerad mot `AssetTypeDefinition.AllowedFileExtensions`. |
| `FileExtensions` | Omsluter `IReadOnlyList<string>`. Tom lista = alla filändelser tillåtna. |
| `File` | Sammansatt VO: `OriginalName`, `FileServerId`, `Size`, `FileName`, `FileExtension`. |

### Kommandon (Submission)

| Kommando | Beskrivning |
|---|---|
| `CreateArticleCommand` | Skapar en artikel i en tidskrift och tilldelar inloggad användare som korresponderande författare. |
| `AssignAuthorCommand` | Tilldelar en befintlig författare till en artikel med roll och bidragsområden. |
| `CreateAndAssignAuthorCommand` | Skapar en ny författare (ej systemanvändare) och tilldelar till artikel i ett steg. |
| `UploadManuscriptFileCommand` | Laddar upp en manuskriptfil till en artikel. Validerar att `AssetType` är tillåten. |

Varje kommando har en **validator** (FluentValidation) och en **handler**.

### Endpoints (Submission)

| Metod | URL | Beskrivning | Kräver roll |
|---|---|---|---|
| POST | `/api/articles` | Skapa ny artikel | AUT |
| PUT | `/api/articles/{articleId}/authors/{authorId}` | Tilldela befintlig författare | CORAUT |
| PUT | `/api/articles/{articleId}/authors/{authorId}` | Skapa och tilldela ny författare | CORAUT |
| POST | `/api/articles/{articleId}/assets/manuscript:upload` | Ladda upp manuskript | CORAUT |

### Tekniska detaljer

- Använder **Minimal APIs** (inte FastEndpoints).
- TPH (Table Per Hierarchy) för Person/Author och ArticleActor/ArticleAuthor.
- `ComplexProperty` (EF 8+) för `Asset.Name` och `Asset.File` value objects.
- JSON-serialisering för `AssetTypeDefinition.AllowedFileExtensions`.
- Kolon-syntax i URL:er (`manuscript:upload`) följer Google API-designstil.

---

## Tjänst: Journals (tidig fas)

Skelettstruktur finns med tre projekt (API, Domain, Persistence). Endast ett endpoint är påbörjat:

| Metod | URL | Beskrivning | Kräver roll |
|---|---|---|---|
| POST | `/journals` | Skapa tidskrift | EOF (Redaktör) |

`CreateJournalCommand` finns med ISSN-validering (regex `\d{4}-\d{3}[\dX]`), men handler-kroppen är tom.

---

## Moduler

### EmailService

Fristående modul för att skicka e-post.

- **Contracts**: `IEmailService` med `SendEmailAsync(EmailMessage)`. `EmailMessage` har Subject, Content (Text/Html), From/To.
- **SMTP-implementation**: Använder **MailKit**. Konfigureras via `EmailOptions` i appsettings (Host, Port, Username, Password, SSL).
- Registreras med `AddSmtpEmailService()`. Options valideras med Data Annotations vid uppstart.

### FileStorage

Fristående modul för fillagring.

- **Contracts**: `IFileService` med `UploadFileAsync`, `DownloadFileAsync`, `TryDeleteFileAsync`. Returnerar `UploadResponse` (FilePath, FileName, FileSize, FileId).
- **MongoDB GridFS-implementation**: Använder **MongoDB.Driver**. Lagrar filsökväg och content-type som metadata. Stöder taggar.
- Konfigureras via `MongoGridFsFileStorageOptions` (ConnectionStringName, DatabaseName, BucketName, ChunkSize, FileSizeLimit).
- Registreras med `AddMongoFileStorage()`.

---

## Mönster och konventioner

### Partial classes separerar data från beteende

Entiteter delas upp i två filer:
- `Entities/Article.cs` — properties och relationer
- `Behaviors/Article.cs` — factory-metoder och affärslogik

Samma mönster för User, UserRole, Author, Journal, Asset, File.

### Domänvalidering utan ramverk

Domänlagret använder **inga valideringsramverk**. Affärsregler skyddas genom:
- **Privata konstruktorer** + statiska factory-metoder (`Create`, `FromFileName`, etc.)
- **Guard clauses** (`Guard.ThrowIfNullOrWhiteSpace`, `Guard.ThrowIfNotEqual`)
- **DomainException** vid brutna affärsregler
- **Inkapslade collections** (`private List<Asset> _assets` med `IReadOnlyList<Asset>` utåt)

### FluentValidation för input-validering

Application-lagret använder FluentValidation för alla commands. Validatorerna körs automatiskt i MediatR-pipelinen via `ValidationBehavior`.

### Data Annotations enbart för konfiguration

Options-klasser (t.ex. `EmailOptions`, `MongoGridFsFileStorageOptions`) använder `[Required]` för att valideras vid uppstart via `.ValidateDataAnnotations().ValidateOnStart()`.

### MediatR Pipeline

Alla kommandon passerar genom en pipeline:

```
Request → ValidationBehavior → SetUserIdBehavior → Handler → Response
              ↓                       ↓
        FluentValidation         Sätter användar-ID
```

### Repository Pattern

- `IRepository<T>` / `Repository<T>` — generiskt CRUD.
- `CachedRepository<TDbContext, TEntity, TId>` — för uppslagsentiteter som `AssetTypeDefinition`. Cachear alla poster i minnet vid första anrop.
- `ArticleRepository` — specialiserat repository.
- `FindByIdOrThrowAsync` / `GetByIdOrThrowAsync` — extension methods som kastar `NotFoundException`.

### EnumEntity + CachedRepository

Uppslagstabeller (t.ex. `AssetTypeDefinition`) har enum som primärnyckel via `EnumEntity<TEnum>`. Kombineras med `CachedRepository` och `ICacheable`-interface för att laddas en gång och serveras från minne.

### Två API-stilar

- **Auth** och **Journals** använder **FastEndpoints** (klassbaserade endpoints).
- **Submission** använder **Minimal APIs** (statiska extension methods).

### IUserCreationInfo-mönster

Auth-domänen exponerar interface (`IUserCreationInfo`, `IUserRole`) som commands implementerar. Det håller domänen oberoende av specifika command-typer.

### GlobalUsing för typ-alias

`GlobalUsing.cs` i Submission.Application definierar typ-alias:
```csharp
global using AssetTypeDefinitionRepository = CachedRepository<SubmissionDbContext, AssetTypeDefinition, AssetType>;
```

---

## Viktiga enums

| Enum | Värden | Beskrivning |
|---|---|---|
| `UserRoleType` | EOF (1), AUT (11), CORAUT (12), USERADMIN (91) | Roller i systemet |
| `ContributionArea` | OriginalDraft, ReviewAndEditing, Conceptualization, FormalAnalysis, Investigation, Methodology, Visualization | Författarbidrag |
| `ArticleStage` | Created | Artikelns livscykelstadie (fler planeras) |
| `ArticleActionType` | Create, CreateAuthor, AssignAuthor, Upload, Submit, Approve, Reject | Händelser på en artikel |
| `AssetType` | Manuscript (1), SupplementaryFile (10), Figure (11), DataSheet (12) | Typer av bifogade filer |
| `Gender` | NotDeclared, Male, Female, Neutral | Kön (Auth) |
| `Honorific` | Mr, Mrs, Dr, Prof | Titel (Auth) |

---

## BuildingBlocks i detalj

### Blocks.Domain
Grundstenarna. `IEntity` / `Entity` (int-nyckel), `Entity<TPrimaryKey>` (generisk nyckel), `EnumEntity<TEnum>` (enum som PK). `ValueObject` med equality via `GetEqualityComponents()`. `StringValueObject` med implicit string-konvertering. `DomainException` för brutna affärsregler. `IAuditableAction` för spårning av vem/när/vad.

### Blocks.Core
Praktiska hjälpare. `Guard`-klassen för parametrar-validering. `MaxLength`-konstanter (C8, C32, C64...C2048). `ConfigurationExtensions` med `AddAndValidateOptions<T>()` (fail-fast vid uppstart). `MemoryCacheExtensions` för cachehantering. `ICacheable` marker-interface. `DateTimeExtensions.ToUniEpochDate()` för JWT. String- och Enumerable-extensions.

### Blocks.EntityFramework
Generiskt `Repository<T>` och `CachedRepository<T>`. `EntityConfiguration<T>` basklass. `BuilderExtensions` med `HasEnumConversion()`, `HasJsonCollectionConversion<T>()`, `HasColumnNameSameAsProperty()`. `RepositoryExtensions` med `FindByIdOrThrowAsync` och `GetByIdOrThrowAsync`.

### Blocks.Exceptions
HTTP-medvetna undantag. `HttpException` (basklass), `NotFoundException` (404), `BadRequestException` (400).

### Blocks.MediatR
`ValidationBehavior` — kör alla FluentValidation-validators asynkront. `SetUserIdBehavior` — sätter `CreatedById` på `IAuditableAction`-requests (just nu hårdkodat till 1).

### Blocks.AspNetCore
HTTP-hjälpare. `BaseUrl(HttpRequest)` returnerar bas-URL. `GetClientIpAddress(HttpContext)` läser `X-Forwarded-For` eller faller tillbaka på `RemoteIpAddress`.

### Articles.Abstractions
Domänspecifika abstraktioner delade mellan tjänster. Alla enums, `IdResponse`-record, `IArticleAction`-interface.

### Articles.Security
`JwtOptions` (Issuer, Audience, Secret, ValidForInMinutes). `AddJwtAuthentication()` för att registrera JWT Bearer. `RequireRoleAuthorization()` för rollbaserad behörighet på endpoints.

---

## Teknikstack

| Teknik | Version | Används för |
|---|---|---|
| .NET | 9.0 | Ramverk |
| ASP.NET Core | 9.0 | Web API |
| Entity Framework Core | 9.0 | ORM / Databas |
| ASP.NET Core Identity | 9.0 | Användarhantering (Auth) |
| SQL Server | - | Databas (Auth + Submission) |
| MongoDB GridFS | - | Fillagring (Submission) |
| FastEndpoints | 8.0.1 | Endpoints (Auth, Journals) |
| MediatR | 14.0.0 | CQRS / Pipeline (Submission) |
| FluentValidation | 12.1.1 | Validering |
| Mapster | 7.4.0 | Objektmappning (Auth) |
| MailKit | 4.15.0 | SMTP e-post |
| MongoDB.Driver | 3.6.0 | GridFS-fillagring |
| Flurl.Http | 4.0.2 | HTTP-klient |
| Swagger | - | API-dokumentation |

---

## Beroendekedjan mellan projekten

```
Auth.API
├── Articles.Security → Articles.Abstractions → Blocks.Domain
├── Auth.Application → Articles.Security
├── Auth.Persistence → Auth.Domain → Blocks.Domain
├── Blocks.AspNetCore
├── EmailService.Smtp → EmailService.Contracts
└── Blocks.Core

Submission.API
├── Articles.Security → Articles.Abstractions → Blocks.Domain
└── Submission.Application
    ├── Articles.Abstractions → Blocks.Domain
    ├── Blocks.Exceptions
    ├── Blocks.MediatR → Blocks.Domain
    ├── FileStorage.Contracts
    └── Submission.Persistence
        ├── Blocks.EntityFramework → Blocks.Domain, Blocks.Exceptions
        ├── FileStorage.MongoGridFS → FileStorage.Contracts
        └── Submission.Domain
            ├── Articles.Abstractions
            ├── Blocks.Core
            └── Blocks.Domain
```

Regeln: pilar pekar bara **inåt/nedåt**, aldrig uppåt. API känner till Application, men Application känner inte till API. Det håller koden lösare kopplad.

---

## Kända begränsningar / TODOs

- `SetUserIdBehavior` hårdkodar `CreatedById = 1` — läser inte från JWT-claims ännu.
- `ValidationBehavior` samlar valideringsfel men kastar inte — fel sväljs tyst.
- JWT-validering är inte konfigurerad i Submission-tjänsten (tom options-lambda).
- `CreateAndAssignAuthorCommandHandler` — grenen "författare är systemanvändare" är tom.
- Domain events är planerade men inte implementerade på de flesta ställen.
- `SubmissionDbContext` har ingen connection string konfigurerad.
- Journals-tjänsten är i skelettstadium.
