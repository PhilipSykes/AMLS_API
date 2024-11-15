namespace Blazor.Models;

//TODO Use filter objects in the model

public class MediaSearchModel
{
    public string mediaTitle { get; set; }
    public List<string> mediaGenres { get; set; } = new List<string>();
    
    public bool isValid =>
    !string.IsNullOrWhiteSpace(mediaTitle) ||
    !mediaGenres.Any();
}