using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.EntityFrameworkCore;
using SupergirlShrine.Infrastructure.Data;

if (args.Length < 2)
{
    Console.WriteLine("Usage: dotnet run --project SupergirlShrine.ImportTool -- \"<Comic Title>\" \"<inbox folder path>\"");
    return;
}

var comicTitle = args[0];
var inboxFolderPath = args[1];

if (!Directory.Exists(inboxFolderPath))
{
    Console.WriteLine($"Error: folder not found at '{inboxFolderPath}'");
    return;
}

// --- R2 setup ---
var r2Config = new AmazonS3Config
{
    ServiceURL = "https://dad13a730b7044d1983c79649ef2cb93.r2.cloudflarestorage.com",
    ForcePathStyle = true
};
var s3Client = new AmazonS3Client("c4c2c79fb6ac01291a67d482ea4dea40", "e0b06262e59fcfb396952b057501efb6870c6aa7100b451362103453e832a973", r2Config);
var transferUtility = new TransferUtility(s3Client);
var bucketName = "supergirl";

// --- Database setup ---
var connectionString = "Host=aws-0-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.vhiqdtgtxrrtcbnbmcof;Password=caePrmEpycAy6DkP;SSL Mode=Require;Trust Server Certificate=true";
var optionsBuilder = new DbContextOptionsBuilder<ComicDatabaseContext>();
optionsBuilder.UseNpgsql(connectionString);
using var db = new ComicDatabaseContext(optionsBuilder.Options);

// --- Build the Comic entity ---
var comic = new Comic { Title = comicTitle };
db.Comics.Add(comic);

var chapterFolders = Directory.GetDirectories(inboxFolderPath)
    .OrderBy(path => Path.GetFileName(path))
    .ToList();

int chapterOrder = 1;
foreach (var chapterFolder in chapterFolders)
{
    var chapterName = Path.GetFileName(chapterFolder);
    var chapter = new Chapter
    {
        Title = chapterName,
        Order = chapterOrder++,
        Comic = comic
    };
    comic.Chapters.Add(chapter);

    var pageFiles = Directory.GetFiles(chapterFolder, "*.jpg")
        .OrderBy(path => Path.GetFileName(path))
        .ToList();

    int pageNumber = 1;
    foreach (var pageFile in pageFiles)
    {
        var fileName = Path.GetFileName(pageFile);
        var objectKey = $"comics/{Sanitize(comicTitle)}/{Sanitize(chapterName)}/{fileName}";

        Console.WriteLine($"  Uploading {chapterName}/{fileName}...");
        var uploadRequest = new TransferUtilityUploadRequest
        {
            FilePath = pageFile,
            BucketName = bucketName,
            Key = objectKey,
            DisablePayloadSigning = true
        };
        await transferUtility.UploadAsync(uploadRequest);
        chapter.Pages.Add(new Page
        {
            ImagePath = objectKey,
            PageNumber = pageNumber++,
            Chapter = chapter
        });
    }

    Console.WriteLine($"  {chapterName}: {pageFiles.Count} pages uploaded.");
}

await db.SaveChangesAsync();
Console.WriteLine($"Done. Imported '{comic.Title}' with {comic.Chapters.Count} chapters.");

static string Sanitize(string input) => input.Replace(" ", "-").ToLowerInvariant();