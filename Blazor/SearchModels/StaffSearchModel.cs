using System.ComponentModel.DataAnnotations;

namespace Blazor.SearchModels;



public class StaffSearchValidator : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var model = (StaffSearchModel)validationContext.ObjectInstance;

        if (string.IsNullOrWhiteSpace(model.Id) 
            && string.IsNullOrWhiteSpace(model.FirstName) 
            && string.IsNullOrWhiteSpace(model.LastName)
            && string.IsNullOrWhiteSpace(model.Role)
            && string.IsNullOrWhiteSpace(model.Branch))
        {
            return new ValidationResult("At least one search field must be filled.");
        }
        return ValidationResult.Success;
    }
}

[StaffSearchValidator]
public class StaffSearchModel
{
    [RegularExpression(@"^[0-9a-fA-F]{24}$", ErrorMessage = "Invalid ID format")]
    public string? Id { get; set; }

    [RegularExpression(@"^[^${}()\[\]]*$", ErrorMessage = "Invalid characters in first name")]
    public string? FirstName { get; set; }

    [RegularExpression(@"^[^${}()\[\]]*$", ErrorMessage = "Invalid characters in last name")]
    public string? LastName { get; set; }
    public string? Role { get; set; }
    public string? Branch { get; set; }

}