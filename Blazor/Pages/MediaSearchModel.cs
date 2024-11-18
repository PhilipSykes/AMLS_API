namespace Blazor.Models;
using System.ComponentModel.DataAnnotations;


public class AtLeastOneFieldRequiredAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var model = (MediaSearchModel)validationContext.ObjectInstance;

        if (string.IsNullOrWhiteSpace(model.mediaTitle) && !model.MediaGenres.Any())
        {
            return new ValidationResult("At least one field must be filled.");
        }

        return ValidationResult.Success;
    }
}

[AtLeastOneFieldRequired]
public class MediaSearchModel
{
    public string mediaTitle { get; set; }
    public List<string> MediaGenres { get; set; } = new List<string>();
    
}