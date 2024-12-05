namespace Blazor.Models;

public class MediaInfoEditModel
{
    public string Title { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public double Rating { get; set; }
    public DateTime PublishDate { get; set; }
    public string Type { get; set; } = string.Empty;
}