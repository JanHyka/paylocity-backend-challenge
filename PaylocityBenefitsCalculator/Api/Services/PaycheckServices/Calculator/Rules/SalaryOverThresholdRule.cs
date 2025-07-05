using Api.Models;
using Api.Services.PaycheckServices.Calculator.ApplicabilityPolicies;

namespace Api.Services.PaycheckServices.Calculator.Rules
{
    /// <summary>
    /// Paycheck rule that applies an additional benefits cost if an <see cref="Employee"/>'s salary is over a specified threshold.
    /// The cost is calculated as a percentage of the employee's salary and prorated for the pay period.
    /// </summary>
    public class SalaryOverThresholdRule(decimal costPercentage, IApplicabilityPolicy<Employee> applicabilityPolicy) : IPaycheckRule
    {
        private readonly decimal _costPercentage = costPercentage;
        private readonly IApplicabilityPolicy<Employee> _applicabilityPolicy = applicabilityPolicy;

        /// <summary>
        /// Applies the salary-over-threshold rule to the specified <see cref="Paycheck"/> for the given <see cref="Employee"/>.
        /// If the employee's salary is over the configured threshold, an additional benefits cost is added, prorated for the pay period.
        /// </summary>
        /// <param name="paycheck">The <see cref="Paycheck"/> to which the rule will be applied.</param>
        /// <param name="employee">The <see cref="Employee"/> for whom the paycheck is being calculated.</param>
        public void Apply(Paycheck paycheck, Employee employee)
        {
            if (!_applicabilityPolicy.IsApplicable(employee, paycheck.PayPeriodStart, paycheck.PayPeriodEnd))
            {
                return; // Skip rule if the employee does not meet the applicability criteria
            }

            decimal annualCost = employee.Salary * _costPercentage;

            if (paycheck.PayPeriodStart.Year == paycheck.PayPeriodEnd.Year)
            {
                int periodDays = (paycheck.PayPeriodEnd - paycheck.PayPeriodStart).Days + 1;
                int daysInYear = DateTime.IsLeapYear(paycheck.PayPeriodStart.Year) ? 366 : 365;
                var surcharge = annualCost * periodDays / daysInYear;

                paycheck.BenefitsCost += Math.Round(surcharge, 2);

                return;
            }

            // If the pay period spans multiple years, calculate the cost for each year separately
            int startYearDays = (new DateTime(paycheck.PayPeriodStart.Year, 12, 31) - paycheck.PayPeriodStart).Days + 1;
            int startYearLength = DateTime.IsLeapYear(paycheck.PayPeriodStart.Year) ? 366 : 365;
            var startYearSurcharge = annualCost * startYearDays / startYearLength;

            int endYearDays = (paycheck.PayPeriodEnd - new DateTime(paycheck.PayPeriodEnd.Year, 1, 1)).Days + 1;
            int endYearLength = DateTime.IsLeapYear(paycheck.PayPeriodEnd.Year) ? 366 : 365;
            var endYearSurcharge = annualCost * endYearDays / endYearLength;

            paycheck.BenefitsCost += Math.Round(startYearSurcharge + endYearSurcharge, 2);
        }
    }
}
