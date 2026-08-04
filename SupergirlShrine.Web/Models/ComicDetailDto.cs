namespace SupergirlShrine.Web.Models
{
    public record ComicDetailDto(
        int Id,
        string Title,
        string? Author,
        string? Description,
        int? StartYear,
        string? CoverImage,
        List<ChapterSummary> Chapters
    );
}