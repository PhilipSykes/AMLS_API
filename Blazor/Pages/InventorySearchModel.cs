using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Blazor.Models;
using System.ComponentModel.DataAnnotations;


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
    public string? MediaTitle { get; set; }
    public string? Branch { get; set; }
    public string? Id { get; set; }
    public string? MediaType { get; set; }
}