using System.ComponentModel.DataAnnotations;

namespace Blazor.SearchModels;



public class MemberSearchValidator : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var model = (MemberSearchModel)validationContext.ObjectInstance;

        if (string.IsNullOrWhiteSpace(model.Id) 
            && string.IsNullOrWhiteSpace(model.FirstName) 
            && string.IsNullOrWhiteSpace(model.LastName)
            && string.IsNullOrWhiteSpace(model.Branch))
        {
            return new ValidationResult("At least one search field must be filled.");
        }
        return ValidationResult.Success;
    }
}

[MemberSearchValidator]
public class MemberSearchModel
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Branch { get; set; }

}