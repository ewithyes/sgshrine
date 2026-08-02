namespace SupergirlShrine.Infrastructure.Dtos;
public record ComicSummaryDto(int Id, string Title, string Author, string Description, int? StartYear, string CoverImage);
public record ComicDetailDto(int Id, string Title, string Author, string Description, int? StartYear, string CoverImage, List<ChapterSummaryDto> Chapters);
public record ChapterSummaryDto(int Id, string Title, int Order);
public record ChapterDetailDto(int Id, string Title, int Order, List<PageDto> Pages);
public record PageDto(int Id, int PageNumber, string ImagePath);