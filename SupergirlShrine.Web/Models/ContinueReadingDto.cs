namespace SupergirlShrine.Web.Models;

public record ContinueReadingDto(int Id, string Title, string? CoverImage, int ChapterId, int PageNumber, string ChapterTitle);