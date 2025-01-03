using System.ComponentModel.DataAnnotations;

namespace Blazor.Models;

public class MediaInfoEditModel
{
    [RegularExpression(@"^[^${}()\[\]]*$")]
    public string Title { get; set; } = string.Empty;
    [RegularExpression(@"^[^${}()\[\]]*$")]
    public string Language { get; set; } = string.Empty;
    [RegularExpression(@"^[^${}()\[\]]*$")]
    public string Description { get; set; } = string.Empty;
    [RegularExpression(@"^[^${}()\[\]]*$")]
    public string Isbn { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    [RegularExpression(@"^[^${}()\[\]]*$")]
    public string Type { get; set; } = string.Empty;
    [RegularExpression(@"^[^${}()\[\]]*$")]
    public string Author { get; set; } = string.Empty; 
    [RegularExpression(@"^[^${}()\[\]]*$")]
    public string Publisher { get; set; } = string.Empty;
    [RegularExpression(@"^[^${}()\[\]]*$")]
    public string Director { get; set; } = string.Empty;
    [RegularExpression(@"^[^${}()\[\]]*$")]
    public string Studio { get; set; } = string.Empty;
    [RegularExpression(@"^[^${}()\[\]]*$")]
    public string Creator { get; set; } = string.Empty;
    [RegularExpression(@"^[^${}()\[\]]*$")]
    public string Network { get; set; } = string.Empty;
    public int Season { get; set; }
    public int Episodes { get; set; }
}
