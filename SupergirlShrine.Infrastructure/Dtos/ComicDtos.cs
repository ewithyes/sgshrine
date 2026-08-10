namespace SupergirlShrine.Infrastructure.Dtos;
public record ComicSummaryDto(int Id, string Title, string Author, string Description, int? StartYear, int? EndYear, string CoverImage);
public record ComicDetailDto(int Id, string Title, string Author, string Description, int? StartYear,  int? EndYear, string CoverImage, List<ChapterSummaryDto> Chapters);
public record ChapterSummaryDto(int Id, string Title, int Order, string? CoverImagePath);
public record ChapterDetailDto(int Id, string Title, int Order, List<PageDto> Pages);
public record PageDto(int Id, int PageNumber, string ImagePath);
public record SaveProgressRequest(int ChapterId, int PageNumber);
public record ContinueReadingDto(int Id, string Title, string? CoverImage, int ChapterId, int PageNumber, string ChapterTitle);
public record ArchiveStatsDto(int TotalComics, int TotalChapters, DateTime? LastVisited);