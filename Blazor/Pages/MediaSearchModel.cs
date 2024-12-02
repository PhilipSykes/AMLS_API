using System.ComponentModel.DataAnnotations;

namespace Blazor.Models;

public class AtLeastOneFieldRequiredAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var model = (MediaSearchModel)validationContext.ObjectInstance;

        if (string.IsNullOrWhiteSpace(model.MediaTitle) && !model.MediaGenres.Any())
            return new ValidationResult("At least one field must be filled.");
        return ValidationResult.Success;
    }
}

[AtLeastOneFieldRequired]
public class MediaSearchModel
{
    public string? MediaTitle { get; set; }
    public List<string> MediaGenres { get; set; } = new();
    public Dictionary<string, List<string>> MediaPublisher { get; set; } = new();
    public Dictionary<string, List<string>> MediaAuthor { get; set; } = new();
}