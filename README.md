# Supergirl Shrine

A personal digital archive and reader for Supergirl comics, built as a learning project ahead of an ASP.NET Core internship.

## Stack
- **Backend:** ASP.NET Core (Minimal APIs), C# / .NET 10
- **Database:** PostgreSQL via EF Core, hosted on Supabase
- **Storage:** Cloudflare R2 (S3-compatible) for comic page images
- **Import pipeline:** PDFs → `pdftoppm` → JPGs → custom import console tool → R2 + database

## Project Structure

<img width="628" height="125" alt="Screenshot 2026-08-02 at 13 38 30" src="https://github.com/user-attachments/assets/c40e8a30-9065-418d-8f3f-4834173f3b4c" />

## Data Model
`Comic` → `Chapter` (issue/volume) → `Page`

Each comic represents a distinct Supergirl run/series (e.g. Woman of Tomorrow, Rebirth). Chapters are individual issues. Pages are stored as image files in R2, with paths referenced in the database.

## Setup

### Prerequisites
- .NET 10 SDK
- A Supabase project (free tier)
- A Cloudflare R2 bucket + API token

### Configuration
This project uses [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for local credentials, so I never commit connection strings or API keys.

```bash
cd SupergirlShrine.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:ComicDb" "<your Supabase connection string>"
```

### Database migrations
```bash
dotnet ef database update --project ../SupergirlShrine.Infrastructure --startup-project .
```

### Run
```bash
dotnet run --project SupergirlShrine.Api
```

## Status
Work in progress; currently building the import pipeline.

## Roadmap
- [x] Core domain model + EF Core migrations
- [x] Cloudflare R2 storage setup
- [ ] Console import tool (natural sort, batch upload)
- [ ] Minimal API endpoints
- [ ] Reader frontend + UI for manual import eventually
