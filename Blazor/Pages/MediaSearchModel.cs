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
    public string? MediaType { get; set; }

    public string? MediaCreator { get; set; }  
    public string? MediaCompany { get; set; }  

    public int? Season { get; set; }
    public int? Episodes { get; set; }
}