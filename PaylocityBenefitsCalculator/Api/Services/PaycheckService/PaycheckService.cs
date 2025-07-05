using Api.Dtos.Paycheck;
using Api.Models;

namespace Api.Services.PaycheckService;

/// <summary>
/// Mock implementation of a service for calculating <see cref="Paycheck"/>s.
/// Most likely, this will be replaced with a real implementation that interacts with business logic, APIs, or a database.
/// </summary>
public class PaycheckService : IPaycheckService
{
    /// <inheritdoc />
    public Task<Paycheck> CalculatePaycheck(int employeeId, DateTime startDate, PaycheckPeriodicity periodicity)
    {
        // For simplicity, let's assume a fixed gross pay and benefits cost.
        // In a real application, these values would be fetched from a database or calculated based on various factors.
        decimal grossPay = 2000.00m; // Example gross pay
        decimal benefitsCost = 300.00m; // Example benefits cost
        // Calculate the pay period end date based on the periodicity
        DateTime payPeriodEnd;
        switch (periodicity)
        {
            case PaycheckPeriodicity.BiWeekly:
                payPeriodEnd = startDate.AddDays(14);
                break;
            default:
                throw new NotImplementedException("This periodicity is not implemented yet.");
        }
        return Task.FromResult(new Paycheck
        {
            EmployeeId = employeeId,
            GrossPay = grossPay,
            BenefitsCost = benefitsCost,
            PayPeriodStart = startDate,
            PayPeriodEnd = payPeriodEnd
        });
    }
}
