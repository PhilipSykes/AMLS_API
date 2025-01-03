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
    [RegularExpression(@"^[^${}()\[\]]*$", ErrorMessage = "Invalid characters in title")]
    public string? MediaTitle { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s-_,]*$", ErrorMessage = "Invalid characters in genre")]
    public List<string> MediaGenres { get; set; } = new();

    [RegularExpression(@"^[a-zA-Z0-9\s-_,]*$", ErrorMessage = "Invalid characters in author")]
    public List<string> MediaAuthors { get; set; } = new();

    [RegularExpression(@"^[a-zA-Z0-9\s-_,]*$", ErrorMessage = "Invalid characters in director")]
    public List<string> MediaDirectors { get; set; } = new();

    [RegularExpression(@"^[a-zA-Z0-9\s-_,]*$", ErrorMessage = "Invalid characters in studio")]
    public List<string> MediaStudios { get; set; } = new();
    public string? MediaType { get; set; }

    public int? Season { get; set; }
    public int? Episodes { get; set; }
}