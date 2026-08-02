public class Page {
    public int Id { get; set; }
    public string? ImagePath { get; set; }
    public int PageNumber { get; set; }
    public int ChapterId { get; set; }
    public Chapter? Chapter { get; set; }
}