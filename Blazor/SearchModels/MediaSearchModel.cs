using System.ComponentModel.DataAnnotations;

namespace Blazor.SearchModels;

public class AtLeastOneFieldRequiredAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var model = (MediaSearchModel)validationContext.ObjectInstance;

        if (string.IsNullOrWhiteSpace(model.MediaTitle) && 
            !model.MediaGenres.Any() &&
            !model.MediaAuthors.Any() &&
            !model.MediaDirectors.Any() &&
            !model.MediaStudios.Any())
            return new ValidationResult("At least one field must be filled.");
        return ValidationResult.Success;
    }
}

[AtLeastOneFieldRequired]
public class MediaSearchModel
{
    public string? MediaTitle { get; set; }
    public List<string> MediaGenres { get; set; } = new();
    public List<string> MediaAuthors { get; set; } = new();
    public List<string> MediaDirectors { get; set; } = new();
    public List<string> MediaStudios { get; set; } = new();
    public string? MediaType { get; set; }

    public int? Season { get; set; }
    public int? Episodes { get; set; }
}