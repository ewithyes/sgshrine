public class Comic
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Description { get; set; }
    public string? CoverImage { get; set; }
    public int? StartYear { get; set; }
    public int? EndYear { get; set; }
    public int? LastReadChapterId { get; set; }
    public int? LastReadPageNumber { get; set; }
    public DateTime? LastReadDate { get; set; }
    public List<Chapter> Chapters { get; set; } = new List<Chapter>();
}