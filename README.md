# Supergirl Shrine

A personal digital archive and reader dedicated to Kara Zor-El, built as a learning project ahead of an ASP.NET Core internship. Comics are imported from PDF scans, stored in the cloud, and read through a custom Blazor front end styled as a quiet, star-lit shrine rather than a generic app.

## Stack

- **API:** ASP.NET Core (Minimal APIs), C# / .NET 10
- **Web front end:** Blazor Server (interactive server rendering, no JavaScript)
- **Database:** PostgreSQL via EF Core, hosted on Supabase
- **Storage:** Cloudflare R2 (S3-compatible) for comic page images, served publicly via an `.r2.dev` domain
- **Import pipeline:** PDFs → `pdftoppm` (via a bash script) → JPGs → a custom .NET console tool → uploaded to R2, cataloged in the database

## Project Structure

```
SupergirlShrine/
├── SupergirlShrine.Api/              # Minimal API — all HTTP endpoints
├── SupergirlShrine.Core/              # Domain entities (Comic, Chapter, Page)
├── SupergirlShrine.Infrastructure/    # EF Core DbContext, migrations, DTOs
├── SupergirlShrine.ImportTool/        # Standalone console app for importing comics
├── SupergirlShrine.Web/               # Blazor Server front end
├── scripts/
│   └── convert-pdfs.sh                # PDF → JPG batch conversion script
└── SupergirlShrine.sln
```

## Data Model

`Comic` → `Chapter` (issue/annual) → `Page`

Each `Comic` represents a distinct Supergirl run, volume, or one-shot (e.g. *Woman of Tomorrow*, *Supergirl (2016)*). `Chapter`s are individual issues within that run. `Page`s are individual page images, stored in R2 with their object key referenced in the database.

`Comic` also tracks reading state (`LastReadChapterId`, `LastReadPageNumber`, `LastReadDate`), which powers the "Continue Reading" feature.

## Pages

- **Home** (`/`) — the Kara Zor-El introduction, a "Continue Reading" carousel pulling from your actual reading progress, and an "Archive Ledger" side panel showing live stats (comics catalogued, issues catalogued, last visited).
- **All Comics** (`/comics`) — the full searchable, sortable collection grid.
- **Comic Detail** (`/comics/{id}`) — an issue grid for one comic, each card showing that chapter's first page as a thumbnail.
- **Reader** (`/comics/{comicId}/chapters/{chapterId}`) — the actual comic reader. Supports both continuous scroll and page-by-page modes (toggleable), chapter-to-chapter navigation, and a fullscreen mode. Reading progress is saved automatically as you read, and can resume on the exact page via a query parameter (`?page=`).
- **The Karchive** (`/karchive`) — a curated, illustrated timeline of Kara Zor-El's full publication history since her 1959 debut, with entries lighting up and linking directly into the reader if that run exists in your catalog.

## Import Pipeline

Comics are added via a two-step manual process: PDFs are converted to page images, then imported into the database and cloud storage.

### 1. Download & convert PDFs

Download comic issue PDFs manually (one folder per comic run, optionally with a `titles.txt` listing real issue titles in order), then convert them to zero-padded chapter folders of JPGs:

```bash
./scripts/convert-pdfs.sh <pdf-folder> <output-folder>
```

**Example:**
```bash
./scripts/convert-pdfs.sh ~/Documents/comics/pdfs-inbox ~/Documents/comics/NewComicTitle
```

Notes:
- Put all PDFs for one comic run in a single input folder first.
- Name PDFs so alphabetical order matches issue order (`01-issue.pdf`, `02-issue.pdf`, ...).
- Output: `Chapter01/`, `Chapter02/`, ... each full of `page-01.jpg`, `page-02.jpg`, ...
- If a `titles.txt` file (one real issue title per line, matching PDF order) is present in the input folder, it's carried through automatically and used for chapter titles instead of the generic `ChapterXX` name.

### 2. Import into R2 + Supabase

```bash
dotnet run --project SupergirlShrine.ImportTool -- "<Comic Title>" <converted-folder-path>
```

**Example:**
```bash
dotnet run --project SupergirlShrine.ImportTool -- "Rebirth" ~/Documents/comics/Rebirth
```

This uploads every page image to Cloudflare R2 and writes the `Comic` → `Chapter` → `Page` records to Supabase in one run.

### Full workflow, start to finish

```bash
# 1. Download PDFs manually into one inbox folder (with an optional titles.txt)

# 2. Convert
./scripts/convert-pdfs.sh ~/Documents/comics/pdfs-inbox ~/Documents/comics/<ComicName>

# 3. Import
dotnet run --project SupergirlShrine.ImportTool -- "<Comic Title>" ~/Documents/comics/<ComicName>
```

All commands are run from the solution root.

## Setup

### Prerequisites

- .NET 10 SDK
- A Supabase project (free tier)
- A Cloudflare R2 bucket, with public access enabled, plus an API token

### Configuration

This project uses [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for local credentials — never commit connection strings or API keys.

```bash
cd SupergirlShrine.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:ComicDb" "<your Supabase connection string>"
```

R2 credentials for `SupergirlShrine.ImportTool` are configured directly in that project (not committed — see `.gitignore`).

### Database migrations

```bash
dotnet ef database update --project SupergirlShrine.Infrastructure --startup-project SupergirlShrine.Api
```

### Run

Both the API and the Web front end need to be running simultaneously:

```bash
# Terminal 1
dotnet run --project SupergirlShrine.Api

# Terminal 2
dotnet run --project SupergirlShrine.Web
```

## Status

🚧 Actively evolving — core reading experience is complete and in daily use; next up is deploying it somewhere it can be reached outside localhost.

## Roadmap

- [x] Core domain model + EF Core migrations
- [x] Cloudflare R2 storage setup
- [x] Console import tool (batch upload, PDF→JPG conversion script, custom chapter titles)
- [x] Minimal API endpoints
- [x] Reader front end (scroll + page-by-page modes, chapter navigation)
- [x] Reading progress tracking + "Continue Reading"
- [x] Full visual redesign (starfield background, custom typography, Archive Ledger, Karchive timeline)
- [x] Responsive layout pass
- [ ] Deployment (evaluating hosting options for the API + Blazor Server front end)
- [ ] Fill in remaining comic metadata (author, description, cover images) for older imports
