using Api.Models;

namespace Api.Services.PaycheckServices.Validator;

/// <summary>
/// Defines a method for validating <see cref="Employee"/> entities before paycheck calculation.
/// </summary>
public interface IEmployeeValidator
{
    /// <summary>
    /// Validates if the specified <see cref="Employee"/> is eligible for paycheck calculation.
    /// </summary>
    /// <param name="employee">The <see cref="Employee"/> model to validate.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="Employee"/> model is valid; otherwise, <c>false</c>.
    /// </returns>
    bool IsValid(Employee employee);
}
