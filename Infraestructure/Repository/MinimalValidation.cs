using System.ComponentModel.DataAnnotations;
using Infraestructure.Repository;

public static class MinimalValidation
{
    public static bool TryValidate(object model, out List<string> errors)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
        var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

        errors = new List<string>();
        if (!isValid)
        {
            foreach (var validationResult in validationResults)
            {
                errors.Add(validationResult.ErrorMessage);
            }
        }

        return isValid;
    }
}
