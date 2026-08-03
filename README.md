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
## Importing Comics

Comics are added via a two-step manual pipeline: PDFs are converted to page images, then imported into the database and cloud storage.

### 1. Download & convert PDFs

Download comic issue PDFs manually (one folder per comic run), then convert them to zero-padded chapter folders of JPGs:

```bash
./scripts/convert-pdfs.sh  
```

**Example:**
```bash
./scripts/convert-pdfs.sh ~/Documents/comics/pdfs-inbox ~/Documents/comics/NewComicTitle
```

Notes:
- Put all PDFs for one comic run in a single input folder first
- Name PDFs so alphabetical order matches issue order (`01-issue.pdf`, `02-issue.pdf`, ...)
- Output: `Chapter01/`, `Chapter02/`, ... each full of `page-01.jpg`, `page-02.jpg`, ...

### 2. Import into R2 + Supabase

```bash
dotnet run --project SupergirlShrine.ImportTool -- "" 
```

**Example:**
```bash
dotnet run --project SupergirlShrine.ImportTool -- "Rebirth" ~/Documents/comics/Rebirth
```

This uploads every page image to Cloudflare R2 and writes the `Comic` → `Chapter` → `Page` records to Supabase in one run.

### Full workflow, start to finish

```bash
# 1. Download PDFs manually into one inbox folder

# 2. Convert
./scripts/convert-pdfs.sh ~/Documents/comics/pdfs-inbox ~/Documents/comics/

# 3. Import
dotnet run --project SupergirlShrine.ImportTool -- "" ~/Documents/comics/
```

Both commands are run from the solution root.

## Status
🚧 Work in progress — import pipeline complete, building out the API and reader UI next.

## Roadmap
- [x] Core domain model + EF Core migrations
- [x] Cloudflare R2 storage setup
- [x] Console import tool (batch upload, PDF→JPG conversion script)
- [ ] Minimal API endpoints
- [ ] Reader frontend
- [ ] Potentially: a user interface for adding comics through the app itself, instead of just scripts
 
