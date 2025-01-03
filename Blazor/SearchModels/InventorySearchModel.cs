using System.ComponentModel.DataAnnotations;

namespace Blazor.SearchModels;



public class InventorySearchValidator : ValidationAttribute
{

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var model = (InventorySearchModel)validationContext.ObjectInstance;

        if (string.IsNullOrWhiteSpace(model.MediaTitle) 
            && string.IsNullOrWhiteSpace(model.Branch) 
            && string.IsNullOrWhiteSpace(model.MediaType) 
            && string.IsNullOrWhiteSpace(model.Id))
        {
            return new ValidationResult("At least one field must be filled.");
        }
        return ValidationResult.Success;

    }
}

[InventorySearchValidator]
public class InventorySearchModel
{
    [RegularExpression(@"^[^${}()\[\]]*$", ErrorMessage = "Invalid characters in title")]
    public string? MediaTitle { get; set; }
    public string? Branch { get; set; }
    [RegularExpression(@"^[^${}()\[\]]*$", ErrorMessage = "Invalid characters in ID")]
    public string? Id { get; set; }
    public string? MediaType { get; set; }
}