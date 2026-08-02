public class Chapter
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ComicId { get; set; }
    public Comic? Comic { get; set; }
    public int Order { get; set; }
    public List<Page> Pages { get; set; } = new List<Page>();
}