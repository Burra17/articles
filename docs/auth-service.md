# Auth Service

Manages user identities, authentication, and exposes person data to other services via gRPC.

**Port:** 4401
**Database:** SQL Server (`AuthDb`)
**API Framework:** FastEndpoints

---

## Endpoints

| Method | Route | Description | Auth |
|---|---|---|---|
| POST | `/login` | Login with email + password, returns JWT & refresh token | Anonymous |
| POST | `/users` | Create a new user with roles | USERADMIN |

---

## Domain Model

```
Person
├── Id, FirstName, LastName, Gender, Email, Nickname
├── HonorificTitle (value object: Mr / Mrs / Dr / Prof)
├── ProfessionalProfile (value object: Position, CompanyName, Affiliation)
├── PictureUrl
└── User? (navigation)

User : IdentityUser<int>
├── RegistrationDate, LastLogin
├── PersonId → Person
├── UserRoles[] → UserRole
└── RefreshTokens[] → RefreshToken

UserRole : IdentityUserRole<int>
├── RoleId → Role
├── StartDate?, ExpiringDate?
└── Validation: StartDate must be today or in the future

Role : IdentityRole<int>
├── Type: UserRoleType (EOF, AUT, CORAUT, REVED, REV, USERADMIN)
└── Description

RefreshToken
├── Token (64-byte cryptographically random)
├── CreatedByIp, CreatedOn
├── ExpiresOn (7 days)
└── RevokedOn?
```

---

## JWT Token

Generated with `HmacSha256`. Claims included:

| Claim | Value |
|---|---|
| `sub` | User ID |
| `email` | Email address |
| `name` | Full name |
| `role` | Role name(s) |
| `jti` | Unique token ID |
| `iat` | Issued at (unix epoch) |

Configuration in `appsettings.json`:
```json
"JwtOptions": {
  "Issuer": "Articles",
  "Audience": "Articles",
  "Secret": "this-is-a-very-strong-secret-123!",
  "ValidForInMinutes": 432000
}
```

---

## gRPC Service

Auth exposes `IPersonService` for other services to query person data:

```
GetPersonByIdAsync(GetPersonRequest)
GetPersonByUserIdAsync(GetPersonByUserIdRequest)
GetPersonByEmailAsync(GetPersonByEmailRequest)
GetOrCreatePersonAsync(CreatePersonRequest)
```

---

## Create User Flow

```
POST /users (USERADMIN token required)
    │
    ├── Check if person with email already exists
    │     └── If person.User != null → 400 already exists
    │
    ├── Begin DB transaction
    │
    ├── Create Person (if not exists)
    │
    ├── Create User (User.Create)
    │     └── Set PersonId = person.Id
    │
    ├── userManager.CreateAsync(user)
    │
    ├── userManager.AddToRoleAsync(user, role) per role
    │
    ├── person.AssignUser(user)
    │
    ├── Commit transaction
    │
    ├── Publish domain event: UserCreated
    │     └── Handler sends confirmation email with password reset token
    │
    └── Return { email, userId, resetToken }
```

---

## Login Flow

```
POST /login (anonymous)
    │
    ├── Get person by email (404 if not found)
    ├── Get user from person (404 if not found)
    ├── CheckPasswordSignInAsync → 400 if invalid
    ├── Get user roles
    ├── Generate JWT token (HmacSha256)
    ├── Generate refresh token (64-byte random, 7-day TTL)
    └── Return { email, jwtToken, refreshToken }
```

---

## Key Behaviors

- Password lockout: 5 failed attempts → 5-minute lockout
- All creates wrapped in a DB transaction (committed explicitly)
- `GlobalExceptionMiddleware` maps exceptions to correct HTTP status codes
- `JsonStringEnumConverter` registered so enum fields accept string values in JSON (e.g. `"Mrs"`, `"AUT"`)

---

## Seed Data (Development)

Runs automatically on startup in Development environment:

| Field | Value |
|---|---|
| Email | `dotnetlabx@articles.test` |
| Password | `Pass.123!` |
| Role | `USERADMIN` |
