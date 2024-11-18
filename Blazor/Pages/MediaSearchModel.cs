namespace Blazor.Models;

public class MediaSearchModel
{
    public string mediaTitle { get; set; }
    public List<string> MediaGenres { get; set; } = new List<string>();
    
    public bool IsValid =>
    !string.IsNullOrWhiteSpace(mediaTitle) ||
    !MediaGenres.Any();
    
}