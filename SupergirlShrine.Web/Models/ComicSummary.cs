namespace SupergirlShrine.Web.Models
{
    public record ComicSummary(
        int Id,
        string Title,
        string? Author,
        string? Description,
        int? StartYear,
        int? EndYear,
        string? CoverImage
    );
}