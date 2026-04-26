# Copilot Instructions — MyPathfinderCampaignTracker

## Project Overview

A web application for tracking Pathfinder RPG campaign progress.
Built on .NET 10 with Blazor Server, MudBlazor 9.x, and a clean architecture back-end.

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
- **Theme:** Custom `MudTheme` defined in `MainLayout.razor` with full Pathfinder fantasy palettes for both light and dark mode. Active theme controlled by `_isDarkMode` (bool).
- **Dark mode:** Toggled via `MudIconButton` in the AppBar. Preference is **persisted in the database** (`User.IsDarkMode`) and loaded from the `"dark_mode"` cookie claim on each page load. Unauthenticated users default to light mode.
- **Enter key:** Login and Register forms respond to the Enter key (keyboard submit) via `@onkeydown` handlers on the input containers.

### Pathfinder Fantasy Theme

Fonts (loaded via Google Fonts link in `App.razor`):
- **Cinzel** — headings, nav links, table headers, buttons, dialog titles, chips
- **Cinzel Decorative** — AppBar title only
- **Crimson Text** — body text (serif, readable like a campaign journal)

Color palette:

| Element | Light mode | Dark mode |
|---------|-----------|-----------|
| Background | `#F2E8CE` (parchment) | `#0D0806` (near-black) |
| AppBar | `#6B1212` (deep crimson) | `#0D0806` (near-black) |
| Primary | `#8B1A1A` (crimson) | `#D4AF37` (aged gold) |
| Secondary | `#B8860B` (dark goldenrod) | `#C41E3A` (vivid crimson) |
| Drawer | `#2D1A0A` (dark leather) | `#100800` (very dark leather) |
| Drawer text | `#F5E6C8` (warm gold) | `#F5E6C8` (warm gold) |

The drawer is **always dark** in both light and dark mode — intentional tavern/notice-board aesthetic.

CSS utility classes (defined in `wwwroot/app.css`):
- `.fantasy-card` — crimson border + `◆` corner ornaments via `::before`/`::after`
- `.fantasy-divider` — Cinzel-lettered section separator with flanking gold lines (used in the NavMenu "Beheer" section)
- `.pf-section-title` — heading with a crimson-to-gold gradient underline accent

---

## Architecture Conventions

### Domain Layer (`MyPathfinderCampaignTracker.Domain`)
- Contains only entity classes — no dependencies on any other layer or framework.
- Entities live in `Entities/`.

### Application Layer (`MyPathfinderCampaignTracker.Application`)
- Defines interfaces in `Interfaces/`.
- Contains service implementations in `Services/` that depend only on interfaces.
- DTOs and result types live in `Models/Results.cs`.
- DI registration in `ApplicationServiceExtensions.cs` — call `services.AddApplicationServices()`.
- References `Microsoft.Extensions.DependencyInjection.Abstractions` (not the full DI package).

### Infrastructure Layer (`MyPathfinderCampaignTracker.Infrastructure`)
- `Data/AppDbContext.cs` — EF Core `DbContext` with all DbSets.
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
  - `CampaignEndpoints.cs` → `MapCampaignEndpoints()` → `/api/campaigns/*`
  - `CharacterEndpoints.cs` → `MapCharacterEndpoints()` → `/api/campaigns/{id}/characters/*`
  - `RecapEndpoints.cs` → `MapRecapEndpoints()` → `/api/campaigns/{id}/recaps/*`
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

## Domain Entities

### User
```csharp
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
- Roles derived from `IsAdmin`: admin → `"Admin"` role claim, else `"User"`.
- Unapproved users cannot log in.
- `IsDarkMode` saved via `PUT /api/profile/darkmode` on toggle; loaded from `"dark_mode"` cookie claim on layout init.

### Campaign
```csharp
public class Campaign
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Link { get; set; }              // optional URL for more info
    public string ExtensiveInformation { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }          // optional
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<User> Players { get; set; } // many-to-many with Users
}
```
- Admins: full CRUD. Other users: read-only.
- The campaign with the most recent `UpdatedAt` is shown on the Dashboard.

### Character
```csharp
public class Character
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public Campaign Campaign { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string Name { get; set; }
    public string Race { get; set; }
    public string CharacterClass { get; set; }
    public int Level { get; set; }                 // default 1
    public string Backstory { get; set; }
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }
    public bool KilledInAction { get; set; }
    public string? Alignment { get; set; }           // optional, e.g. "Wettig Goed"
    public string? Personality { get; set; }          // optional character traits/mannerisms
    public string? IdealsAndGoals { get; set; }       // optional motivations and goals
    public string? Flaws { get; set; }                // optional flaws and weaknesses
    public string? Languages { get; set; }            // optional, comma-separated languages
    public string? Appearance { get; set; }           // optional physical description
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```
- Any user in a campaign can create/edit/delete their own characters. Admins can edit/delete any character.
- Ability score modifier = `(score - 10) / 2` (displayed alongside raw scores in the UI).

### Recap
```csharp
public class Recap
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public Campaign Campaign { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public int Number { get; set; }        // auto-assigned chronological number
    public DateTime Date { get; set; }
    public string Title { get; set; }
    public string Contents { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```
- Any user in a campaign can add a recap. All users can read all recaps.
- `Number` is assigned as `(max existing number + 1)` when creating.
- Displayed as "Recap 1", "Recap 2", … in the UI.

---

## Result Types & Enums

All service operation outcomes use types in `Application/Models/Results.cs`:

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

1. **New entity** → add to `Domain/Entities/`, configure in `AppDbContext`, add EF migration.
2. **New repository** → interface in `Application/Interfaces/`, implementation in `Infrastructure/Repositories/`.
3. **New service** → interface in `Application/Interfaces/`, implementation in `Application/Services/`, register in `ApplicationServiceExtensions.cs`.
4. **New API group** → create `Web/Api/<Domain>Endpoints.cs` with `Map<Domain>Endpoints()` extension, call it in `Program.cs`.
5. **New Blazor page** → add to `Web/Components/Pages/`, add nav link in `NavMenu.razor`.
6. **Admin-only pages** → add `@attribute [Authorize(Roles = "Admin")]` and link inside `<AuthorizeView Roles="Admin">` in `NavMenu.razor`.
7. **All visible text** must be in Dutch; keep routes, code, and file names in English.
8. **Use theme classes** on new pages: `.fantasy-card` on `MudPaper`/`MudCard`, `.pf-section-title` on section headings.

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
`launchSettings.json` is in `.gitignore` — do not commit it.

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

# Run (set SQL_CONNECTION_STRING first or configure launchSettings.json)
dotnet run --project src/MyPathfinderCampaignTracker.Web
```
