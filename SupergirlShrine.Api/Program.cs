using Microsoft.EntityFrameworkCore;
using SupergirlShrine.Infrastructure.Dtos;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

var connectionString = builder.Configuration.GetConnectionString("ComicDb");
builder.Services.AddDbContext<ComicDatabaseContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/comics", async (ComicDatabaseContext db) =>
{
    var comics = await db.Comics
        .Select(c => new ComicSummaryDto(c.Id, c.Title, c.Author, c.Description, c.StartYear, c.EndYear, c.CoverImage))
        .ToListAsync();
    return Results.Ok(comics);
});

app.MapGet("/api/comics/continue-reading", async (ComicDatabaseContext db) =>
{
    var comics = await db.Comics
        .Where(c => c.LastReadDate != null)
        .OrderByDescending(c => c.LastReadDate)
        .Select(c => new ContinueReadingDto(
            c.Id,
            c.Title,
            c.CoverImage,
            c.LastReadChapterId!.Value,
            c.LastReadPageNumber!.Value,
            c.Chapters.FirstOrDefault(ch => ch.Id == c.LastReadChapterId)!.Title
        ))
        .ToListAsync();

    return Results.Ok(comics);
});

app.MapGet("/api/comics/{id}", async (int id, ComicDatabaseContext db) =>
{
    var comic = await db.Comics
        .Where(c => c.Id == id)
        .Select(c => new ComicDetailDto(
            c.Id, c.Title, c.Author, c.Description, c.StartYear, c.EndYear, c.CoverImage,
            c.Chapters.OrderBy(ch => ch.Order)
                .Select(ch => new ChapterSummaryDto(ch.Id, ch.Title, ch.Order, ch.Pages.OrderBy(p => p.PageNumber)
                    .Select(p => p.ImagePath)
                    .FirstOrDefault()))
                .ToList()))
        .FirstOrDefaultAsync();

    return comic is null ? Results.NotFound() : Results.Ok(comic);
});

app.MapGet("/api/comics/{comicId}/chapters/{chapterId}", async (int comicId, int chapterId, ComicDatabaseContext db) =>
{
    var chapter = await db.Chapters
        .Where(ch => ch.Id == chapterId && ch.ComicId == comicId)
        .Select(ch => new ChapterDetailDto(
            ch.Id, ch.Title, ch.Order,
            ch.Pages.OrderBy(p => p.PageNumber)
                .Select(p => new PageDto(p.Id, p.PageNumber, p.ImagePath))
                .ToList()))
        .FirstOrDefaultAsync();

    return chapter is null ? Results.NotFound() : Results.Ok(chapter);
});

app.MapPost("/api/comics/{comicId}/progress", async (int comicId, SaveProgressRequest request, ComicDatabaseContext db) =>
{
    var comic = await db.Comics.FindAsync(comicId);
    if (comic is null)
    {
        return Results.NotFound();
    }

    comic.LastReadChapterId = request.ChapterId;
    comic.LastReadPageNumber = request.PageNumber;
    comic.LastReadDate = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok();
});


app.Run();