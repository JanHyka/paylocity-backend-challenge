using Api.Dtos.Paycheck;
using Api.Models;

namespace Api.Services.PaycheckService;

/// <summary>
/// Defines methods for calculating <see cref="Paycheck"/>s.
/// </summary>
public interface IPaycheckService
{
    /// <summary>
    /// Calculates a <see cref="Paycheck"/> for the specified employee and pay period.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="startDate">The start date of the pay period.</param>
    /// <param name="periodicity">The <see cref="PaycheckPeriodicity"/> for the pay period.</param>
    /// <returns>The calculated <see cref="Paycheck"/>.</returns>
    Task<Paycheck> CalculatePaycheck(int employeeId, DateTime startDate, PaycheckPeriodicity periodicity);
}