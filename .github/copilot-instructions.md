# Copilot Instructions — MyPathfinderCampaignTracker

## Project Overview

A web application for tracking Pathfinder RPG campaign progress.
Built on .NET 10 with Blazor Server, MudBlazor, and a clean architecture back-end.

---

## Solution Structure

```
MyPathfinderCampaignTracker.slnx
global.json                          ← pins SDK to 10.0.202
src/
  MyPathfinderCampaignTracker.Domain/          ← entities only, no dependencies
  MyPathfinderCampaignTracker.Application/     ← interfaces, services, DTOs/results
  MyPathfinderCampaignTracker.Infrastructure/  ← EF Core, repositories, hashing, JWT
  MyPathfinderCampaignTracker.Web/             ← Blazor Server + Minimal API
```

**Dependency flow:** `Domain ← Application ← Infrastructure ← Web`

---

## Technology Stack

| Concern | Technology |
|---------|-----------|
| UI framework | Blazor Server (.NET 10) |
| Component library | MudBlazor 9.x |
| API style | Minimal API (same process as Blazor) |
| ORM | Entity Framework Core 10 (SQL Server) |
| Database | Azure SQL Server via `SQL_CONNECTION_STRING` env var |
| Authentication | Cookie auth (Blazor UI) + JWT Bearer (API) |
| Password hashing | BCrypt.Net-Next |
| Token generation | `System.IdentityModel.Tokens.Jwt` |

---

## UI / UX Conventions

- **Language:** All visible UI text is in **Dutch**. Routes, code identifiers, and file names remain in English.
- **Component library:** MudBlazor 9.x — use MudBlazor components for all UI elements.
- **Theme:** Custom `MudTheme` defined in `MainLayout.razor` with light and dark palettes. The active theme is `_isDarkMode` (bool).
- **Dark mode:** Toggled via the `MudIconButton` in the app bar. The preference is **persisted in the database** for authenticated users and loaded from the `"dark_mode"` cookie claim on each page load. Unauthenticated users default to light mode.

---

## Architecture Conventions

### Domain Layer (`MyPathfinderCampaignTracker.Domain`)
- Contains only entity classes — no dependencies on any other layer or framework.
- Entities live in `Entities/`.

### Application Layer (`MyPathfinderCampaignTracker.Application`)
- Defines interfaces in `Interfaces/` (`IUserRepository`, `IPasswordHasher`, `ITokenService`, `IUserService`).
- Contains service implementations in `Services/` that depend only on interfaces.
- DTOs and result types live in `Models/Results.cs`.
- DI registration in `ApplicationServiceExtensions.cs` — call `services.AddApplicationServices()`.
- References `Microsoft.Extensions.DependencyInjection.Abstractions` (not the full DI package).

### Infrastructure Layer (`MyPathfinderCampaignTracker.Infrastructure`)
- `Data/AppDbContext.cs` — EF Core `DbContext` with `Users` DbSet.
- `Data/AppDbContextFactory.cs` — `IDesignTimeDbContextFactory<AppDbContext>` with a local placeholder connection string for `dotnet ef` CLI commands.
- Repositories in `Repositories/`, services in `Services/`.
- DI registration in `InfrastructureServiceExtensions.cs` — call `services.AddInfrastructureServices(configuration)`.
- Reads database connection string from `IConfiguration["SQL_CONNECTION_STRING"]`; throws if missing.
- EF migrations live in `Migrations/` and are **auto-applied on startup** via `db.Database.MigrateAsync()` in `Program.cs`.

### Web Layer (`MyPathfinderCampaignTracker.Web`)
- `Program.cs` is the composition root — registers all services, middleware, API endpoints, and Blazor.
- **API endpoints** live in `Api/` as static extension methods on `IEndpointRouteBuilder`:
  - `AuthEndpoints.cs` → `MapAuthEndpoints()` → `/api/auth/*`
  - `UserManagementEndpoints.cs` → `MapUserManagementEndpoints()` → `/api/users/*`
  - `ProfileEndpoints.cs` → `MapProfileEndpoints()` → `/api/profile/*`
  - **Convention:** group new domain endpoints in their own `*Endpoints.cs` file with a `Map*Endpoints()` extension method.
- **Blazor components** live in `Components/`.
- **Scoped service** `Services/ApiClient.cs` — wraps `IHttpClientFactory("ApiClient")`, extracts the `"access_token"` claim from auth state, and attaches it as a Bearer header. Use this for all Interactive Server component → API calls.
- **Singleton service** `Services/LoginTicketService.cs` — manages short-lived login tickets (2-min TTL) used to hand off session creation from Blazor Interactive Server to an SSR Minimal API endpoint.

---

## Authentication Architecture

### Render mode constraint
`<Routes @rendermode="InteractiveServer" />` is set in `App.razor` — the **entire app runs as Interactive Server**. Interactive components cannot access `HttpContext` directly. Cookie sign-in/out must happen in Minimal API (SSR) endpoints.

### Login-ticket pattern
Because Blazor is fully interactive, login/logout cannot call `HttpContext.SignInAsync` directly. Instead:

1. `Login.razor` calls `IUserService.LoginAsync` to validate credentials.
2. On success, calls `LoginTicketService.CreateTicket(userId, username, token, isAdmin, isDarkMode)` to get a short-lived ticket ID.
3. Navigates with `forceLoad: true` to `/api/auth/login-callback?ticket=<id>`.
4. The SSR Minimal API endpoint redeems the ticket, calls `HttpContext.SignInAsync` with all claims, and redirects to `/dashboard`.

Logout: `Navigation.NavigateTo("/api/auth/signout", forceLoad: true)` → SSR endpoint calls `HttpContext.SignOutAsync`.

### Two schemes, one app

| Scheme | Used by | How |
|--------|---------|-----|
| `CookieAuthenticationDefaults.AuthenticationScheme` | Blazor pages | `HttpContext.SignInAsync` in SSR login-callback endpoint |
| `JwtBearerDefaults.AuthenticationScheme` | Minimal API endpoints | `Authorization: Bearer <token>` header |

### Authorization policies (defined in `Program.cs`)
- `"ApiAuth"` — JWT Bearer + authenticated user → for general API endpoints.
- `"ApiAdmin"` — JWT Bearer + `Admin` role → for admin-only API endpoints.

### Cookie claims set at login
```csharp
new Claim(ClaimTypes.NameIdentifier, userId)
new Claim(ClaimTypes.Name, username)
new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User")
new Claim("access_token", jwtToken)
new Claim("dark_mode", isDarkMode ? "true" : "false")
```

### First-user-as-admin rule
In `UserService.RegisterAsync`: if no users exist yet (`!await userRepository.AnyAsync()`), the new user is set `IsAdmin = true` and `IsApproved = true` automatically.

---

## Blazor Render Mode Rules

All pages are Interactive Server (inherited from the root `<Routes @rendermode="InteractiveServer" />`). Do **not** use `[CascadingParameter] HttpContext` in any Blazor component — it is `null` in Interactive Server mode. Use `ApiClient` for data access and the login-ticket pattern for auth actions.

---

## User Entity & Business Rules

```csharp
// Domain/Entities/User.cs
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }     // unique index in EF
    public string PasswordHash { get; set; } // BCrypt
    public bool IsAdmin { get; set; }
    public bool IsApproved { get; set; }
    public bool IsDarkMode { get; set; }     // persisted UI preference
    public DateTime CreatedAt { get; set; }
}
```

- Roles are derived from `IsAdmin`: admin → `"Admin"` role claim, else `"User"`.
- Unapproved users cannot log in (checked in `UserService.LoginAsync`).
- Admins can approve, promote (→ admin), or demote (→ user) other users.
- `IsDarkMode` is saved via `PUT /api/profile/darkmode` whenever the user toggles the theme button, and loaded from the `"dark_mode"` cookie claim on layout init.

---

## Result Types & Enums

All service operation outcomes use the types in `Application/Models/Results.cs`:

```csharp
public enum RegisterResult  { Success, UserAlreadyExists }
public enum ApproveResult   { Success, UserNotFound, AlreadyApproved }
public enum PromoteResult   { Success, UserNotFound, AlreadyInRole }  // also used for Demote

public class LoginResult
{
    bool Success; string? Token; UserDto? User; string? Error;
    static LoginResult Ok(string token, UserDto user);
    static LoginResult Fail(string error);
}

public class UserDto
{
    Guid Id; string Username; bool IsAdmin; bool IsApproved; bool IsDarkMode; DateTime CreatedAt;
}
```

---

## Adding New Features — Checklist

1. **New entity** → add to `Domain/Entities/`, configure in `AppDbContext`, add migration.
2. **New repository** → interface in `Application/Interfaces/`, implementation in `Infrastructure/Repositories/`.
3. **New service** → interface in `Application/Interfaces/`, implementation in `Application/Services/`, register in `ApplicationServiceExtensions.cs`.
4. **New API group** → create `Web/Api/<Domain>Endpoints.cs` with `Map<Domain>Endpoints()` extension, call it in `Program.cs`.
5. **New Blazor page** → add to `Web/Components/Pages/`, add nav link in `NavMenu.razor`.
6. **Admin-only pages** → add `@attribute [Authorize(Roles = "Admin")]` and link inside `<AuthorizeView Roles="Admin">` in `NavMenu.razor`.
7. **All visible text** must be in Dutch; keep routes, code, and file names in English.

---

## Key Configuration

### `appsettings.json`
```json
{
  "Jwt": {
    "Secret": "...",
    "Issuer": "MyPathfinderCampaignTracker",
    "Audience": "MyPathfinderCampaignTracker",
    "ExpirationMinutes": 60
  },
  "ApiBaseUrl": "https://localhost:7279"
}
```
`ApiBaseUrl` must match the `https` profile port in `launchSettings.json`.

### `launchSettings.json` environment variables
```json
"SQL_CONNECTION_STRING": "Server=...;Database=...;..."
```

### EF Migrations
```sh
dotnet ef migrations add <Name> \
  --project src/MyPathfinderCampaignTracker.Infrastructure \
  --startup-project src/MyPathfinderCampaignTracker.Web
```
Migrations are applied automatically on startup — no manual `dotnet ef database update` needed.

---

## Build & Run

```sh
# Build
dotnet build

# Run (set SQL_CONNECTION_STRING first or edit launchSettings.json)
dotnet run --project src/MyPathfinderCampaignTracker.Web
```


## Project Overview

A web application for tracking Pathfinder RPG campaign progress.
Built on .NET 10 with Blazor Server, MudBlazor, and a clean architecture back-end.

---

## Solution Structure

```
MyPathfinderCampaignTracker.slnx
global.json                          ← pins SDK to 10.0.202
src/
  MyPathfinderCampaignTracker.Domain/          ← entities only, no dependencies
  MyPathfinderCampaignTracker.Application/     ← interfaces, services, DTOs/results
  MyPathfinderCampaignTracker.Infrastructure/  ← EF Core, repositories, hashing, JWT
  MyPathfinderCampaignTracker.Web/             ← Blazor Server + Minimal API
```

**Dependency flow:** `Domain ← Application ← Infrastructure ← Web`

---

## Technology Stack

| Concern | Technology |
|---------|-----------|
| UI framework | Blazor Server (.NET 10) |
| Component library | MudBlazor 9.x |
| API style | Minimal API (same process as Blazor) |
| ORM | Entity Framework Core 10 (SQL Server) |
| Database | Azure SQL Server via `SQL_CONNECTION_STRING` env var |
| Authentication | Cookie auth (Blazor UI) + JWT Bearer (API) |
| Password hashing | BCrypt.Net-Next |
| Token generation | `System.IdentityModel.Tokens.Jwt` |

---

## Architecture Conventions

### Domain Layer (`MyPathfinderCampaignTracker.Domain`)
- Contains only entity classes — no dependencies on any other layer or framework.
- Entities live in `Entities/`.

### Application Layer (`MyPathfinderCampaignTracker.Application`)
- Defines interfaces in `Interfaces/` (`IUserRepository`, `IPasswordHasher`, `ITokenService`, `IUserService`).
- Contains service implementations in `Services/` that depend only on interfaces.
- DTOs and result types live in `Models/Results.cs`.
- DI registration in `ApplicationServiceExtensions.cs` — call `services.AddApplicationServices()`.
- References `Microsoft.Extensions.DependencyInjection.Abstractions` (not the full DI package).

### Infrastructure Layer (`MyPathfinderCampaignTracker.Infrastructure`)
- `Data/AppDbContext.cs` — EF Core `DbContext` with `Users` DbSet.
- `Data/AppDbContextFactory.cs` — `IDesignTimeDbContextFactory<AppDbContext>` with a local placeholder connection string for `dotnet ef` CLI commands.
- Repositories in `Repositories/`, services in `Services/`.
- DI registration in `InfrastructureServiceExtensions.cs` — call `services.AddInfrastructureServices(configuration)`.
- Reads database connection string from `IConfiguration["SQL_CONNECTION_STRING"]`; throws if missing.
- EF migrations live in `Migrations/` and are **auto-applied on startup** via `db.Database.MigrateAsync()` in `Program.cs`.

### Web Layer (`MyPathfinderCampaignTracker.Web`)
- `Program.cs` is the composition root — registers all services, middleware, API endpoints, and Blazor.
- **API endpoints** live in `Api/` as static extension methods on `IEndpointRouteBuilder`:
  - `AuthEndpoints.cs` → `MapAuthEndpoints()` → `/api/auth/*`
  - `UserManagementEndpoints.cs` → `MapUserManagementEndpoints()` → `/api/users/*`
  - **Convention:** group new domain endpoints in their own `*Endpoints.cs` file with a `Map*Endpoints()` extension method.
- **Blazor components** live in `Components/`.
- **Scoped service** `Services/ApiClient.cs` — wraps `IHttpClientFactory("ApiClient")`, extracts the `"access_token"` claim from auth state, and attaches it as a Bearer header. Use this for all Interactive Server component → API calls.

---

## Authentication Architecture

### Two schemes, one app

| Scheme | Used by | How |
|--------|---------|-----|
| `CookieAuthenticationDefaults.AuthenticationScheme` | Blazor pages | `HttpContext.SignInAsync` on SSR login page |
| `JwtBearerDefaults.AuthenticationScheme` | Minimal API endpoints | `Authorization: Bearer <token>` header |

### Authorization policies (defined in `Program.cs`)
- `"ApiAuth"` — JWT Bearer + authenticated user → for general API endpoints.
- `"ApiAdmin"` — JWT Bearer + `Admin` role → for admin-only API endpoints.

### JWT storage
The JWT is embedded as an `"access_token"` claim **inside the auth cookie** at login:
```csharp
new Claim("access_token", result.Token!)
```
Interactive Server components retrieve it via `AuthenticationStateProvider` → `ApiClient`.

### Auth flow
1. **Login** (`/login`, SSR): form POST → `IUserService.LoginAsync` → cookie sign-in with JWT embedded as claim → redirect to `/dashboard`.
2. **Register** (`/register`, SSR): form POST → `IUserService.RegisterAsync` → success/error message (no auto-login; awaits admin approval unless first user).
3. **Logout** (`/logout`, SSR): `NavigateTo("/logout", forceLoad: true)` → `HttpContext.SignOutAsync` → redirect to `/login`.
4. **API calls** (Interactive Server): `ApiClient` extracts `"access_token"` claim → sets `Authorization: Bearer` header → calls same-process API.

### First-user-as-admin rule
In `UserService.RegisterAsync`: if no users exist yet (`!await userRepository.AnyAsync()`), the new user is set `IsAdmin = true` and `IsApproved = true` automatically.

---

## Blazor Render Mode Rules

| Page type | Render mode | Reason |
|-----------|-------------|--------|
| Login, Register, Logout | None (SSR default) | Needs `HttpContext` for cookie sign-in/out |
| Dashboard | `@rendermode InteractiveServer` | Needs auth state for username display |
| Admin/Users | `@rendermode InteractiveServer` | Needs `ApiClient` for data loading |

**Do not add `@rendermode` to SSR pages** — the absence of the directive is what makes them SSR.

---

## User Entity & Business Rules

```csharp
// Domain/Entities/User.cs
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }    // unique index in EF
    public string PasswordHash { get; set; } // BCrypt
    public bool IsAdmin { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

- Roles are derived from `IsAdmin`: admin → `"Admin"` role claim, else `"User"`.
- Unapproved users cannot log in (checked in `UserService.LoginAsync`).
- Admins can approve, promote (→ admin), or demote (→ user) other users.

---

## Result Types & Enums

All service operation outcomes use the types in `Application/Models/Results.cs`:

```csharp
public enum RegisterResult  { Success, UserAlreadyExists }
public enum ApproveResult   { Success, UserNotFound, AlreadyApproved }
public enum PromoteResult   { Success, UserNotFound, AlreadyInRole }  // also used for Demote

public class LoginResult
{
    bool Success; string? Token; UserDto? User; string? Error;
    static LoginResult Ok(string token, UserDto user);
    static LoginResult Fail(string error);
}
```

---

## Adding New Features — Checklist

1. **New entity** → add to `Domain/Entities/`, configure in `AppDbContext`, add migration.
2. **New repository** → interface in `Application/Interfaces/`, implementation in `Infrastructure/Repositories/`.
3. **New service** → interface in `Application/Interfaces/`, implementation in `Application/Services/`, register in `ApplicationServiceExtensions.cs`.
4. **New API group** → create `Web/Api/<Domain>Endpoints.cs` with `Map<Domain>Endpoints()` extension, call it in `Program.cs`.
5. **New Blazor page** → add to `Web/Components/Pages/`, add nav link in `NavMenu.razor`.
6. **Admin-only pages** → add `@attribute [Authorize(Roles = "Admin")]` and link inside `<AuthorizeView Roles="Admin">` in `NavMenu.razor`.

---

## Key Configuration

### `appsettings.json`
```json
{
  "Jwt": {
    "Secret": "...",
    "Issuer": "MyPathfinderCampaignTracker",
    "Audience": "MyPathfinderCampaignTracker",
    "ExpirationMinutes": 60
  },
  "ApiBaseUrl": "https://localhost:7279"
}
```
`ApiBaseUrl` must match the `https` profile port in `launchSettings.json`.

### `launchSettings.json` environment variables
```json
"SQL_CONNECTION_STRING": "Server=...;Database=...;..."
```

### EF Migrations
```sh
dotnet ef migrations add <Name> \
  --project src/MyPathfinderCampaignTracker.Infrastructure \
  --startup-project src/MyPathfinderCampaignTracker.Web
```
Migrations are applied automatically on startup — no manual `dotnet ef database update` needed.

---

## Build & Run

```sh
# Build
dotnet build

# Run (set SQL_CONNECTION_STRING first or edit launchSettings.json)
dotnet run --project src/MyPathfinderCampaignTracker.Web
```
