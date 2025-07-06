using Api.Models;
using Api.Services.PaycheckServices.Calculator.ApplicabilityPolicies;
using Api.Services.PaycheckServices.Calculator.ProrationPolicies;
using Api.Services.PaycheckServices.Calculator.Rules;

namespace Api.Services.PaycheckServices.Calculator.Models;

/// <summary>
/// Calculator model for bi-weekly pay periods.
/// Applies a set of <see cref="IPaycheckRule"/>s to calculate a <see cref="Paycheck"/> for a given <see cref="Employee"/> and period.
/// The rules include base cost surcharges, dependent surcharges, and salary-based surcharges, each with their own applicability and proration logic.
/// </summary>
public class BiWeeklyCalculatorModel : ICalculatorModel
{
    private readonly List<IPaycheckRule> _rules;

    /// <summary>
    /// Initializes a new instance of the <see cref="BiWeeklyCalculatorModel"/> class with a predefined set of paycheck rules.
    /// </summary>
    public BiWeeklyCalculatorModel()
    {
        _rules =
        [
            new BaseCostSurchargeRule(1000.0m),
            new DependentOverAgeSurchargeRule(
                600.0m,
                new DependentWasBornInStartMonthPolicy(),
                new FullProrationPolicy()),
            new SalaryOverThresholdRule(
                0.02m,
                new SalaryOverThresholdPolicy(80000)),
            new DependentOverAgeSurchargeRule(
                200.0m,
                new DependantOverAgeFromBirthdayPolicy(50),
                new ProrationByAgePolicy(50))
        ];
    }

    /// <summary>
    /// Calculates a <see cref="Paycheck"/> for the specified <see cref="Employee"/> and pay period by applying all configured rules.
    /// </summary>
    /// <param name="employee">The <see cref="Employee"/> for whom the paycheck is being calculated.</param>
    /// <param name="periodStart">The start date of the pay period.</param>
    /// <param name="periodEnd">The end date of the pay period.</param>
    /// <returns>
    /// The calculated <see cref="Paycheck"/> for the specified employee and period.
    /// </returns>
    public Paycheck CalculatePaycheck(Employee employee, DateTime periodStart, DateTime periodEnd)
    {
        var paycheck = new Paycheck
        {
            EmployeeId = employee.Id,
            PayPeriodStart = periodStart,
            PayPeriodEnd = periodEnd,
            GrossPay = CalculateGrossPay(employee.Salary, periodStart, periodEnd),
            BenefitsCost = 0.0m
        };

        foreach (var rule in _rules)
        {
            rule.Apply(paycheck, employee);
        }

        return paycheck;
    }

    private static decimal CalculateGrossPay(Decimal salary, DateTime periodStart, DateTime periodEnd)
    {
        if (periodStart.Year == periodEnd.Year)
        {
            int daysInPeriod = (periodEnd - periodStart).Days + 1;
            int daysInYear = DateTime.IsLeapYear(periodStart.Year) ? 366 : 365;
            // Calculate the daily salary and multiply by the number of days in the pay period
            decimal dailySalary = salary / daysInYear;
            return Math.Round(dailySalary * daysInPeriod, 2);
        }
        else
        {
            int startYearDays = (new DateTime(periodStart.Year, 12, 31) - periodStart).Days + 1;
            int startYearLength = DateTime.IsLeapYear(periodStart.Year) ? 366 : 365;
            decimal startYearGross = salary * startYearDays / startYearLength;

            int endYearDays = (periodEnd - new DateTime(periodEnd.Year, 1, 1)).Days + 1;
            int endYearLength = DateTime.IsLeapYear(periodEnd.Year) ? 366 : 365;
            decimal endYearGross = salary * endYearDays / endYearLength;

            return Math.Round(startYearGross + endYearGross, 2);
        }

    }
}
