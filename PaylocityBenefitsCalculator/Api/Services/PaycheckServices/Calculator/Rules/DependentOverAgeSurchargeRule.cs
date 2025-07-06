using Api.Models;
using Api.Services.PaycheckServices.Calculator.ApplicabilityPolicies;
using Api.Services.PaycheckServices.Calculator.ProrationPolicies;

namespace Api.Services.PaycheckServices.Calculator.Rules;

/// <summary>
/// Paycheck rule that applies a monthly surcharge to the <see cref="Paycheck.BenefitsCost"/> for each <see cref="Dependent"/>
/// who meets the specified applicability policy and is over a certain age.
/// The surcharge is prorated by the number of days in each month if the pay period spans multiple months.
/// </summary>
/// <remarks>
/// If the pay period is within a single month, the surcharge is applied proportionally for that month.
/// If the pay period spans two months, the surcharge is split and prorated for each month separately.
/// </remarks>
/// <param name="monthlySurcharge">The monthly surcharge to apply for each applicable dependent.</param>
/// <param name="applicabilityPolicy">The policy that determines if a dependent is eligible for the surcharge.</param>
/// <param name="prorationPolicy">The policy that determines how the surcharge is prorated by days.</param>
public class DependentOverAgeSurchargeRule(
    decimal monthlySurcharge,
    IApplicabilityPolicy<Dependent> applicabilityPolicy,
    IProrationPolicy<Dependent> prorationPolicy) : IPaycheckRule
{
    private readonly decimal _monthlySurcharge = monthlySurcharge;

    private readonly IApplicabilityPolicy<Dependent> _applicabilityPolicy = applicabilityPolicy;

    private readonly IProrationPolicy<Dependent> prorationPolicy = prorationPolicy;

    /// <summary>
    /// Applies the dependent over-age surcharge to the specified <see cref="Paycheck"/> for the given <see cref="Employee"/>.
    /// For each dependent that meets the applicability policy, a prorated surcharge is added to <see cref="Paycheck.BenefitsCost"/>.
    /// </summary>
    /// <param name="paycheck">The <see cref="Paycheck"/> to which the surcharge will be applied.</param>
    /// <param name="employee">The <see cref="Employee"/> whose dependents are evaluated for the surcharge.</param>
    public void Apply(Paycheck paycheck, Employee employee)
    {
        foreach (var dependent in employee.Dependents)
        {
            if (_applicabilityPolicy.IsApplicable(dependent, paycheck.PayPeriodStart, paycheck.PayPeriodEnd))
            {
                if (paycheck.PayPeriodStart.Month == paycheck.PayPeriodEnd.Month)
                // If the pay period is within the same month, we can apply the surcharge directly.
                {
                    int daysInMonth = DateTime.DaysInMonth(paycheck.PayPeriodStart.Year, paycheck.PayPeriodStart.Month);
                    var surcharge = _monthlySurcharge * prorationPolicy.GetProrationFactorDays(dependent, paycheck.PayPeriodStart, paycheck.PayPeriodEnd) / daysInMonth;
                    paycheck.BenefitsCost += Math.Round(surcharge, 2);
                    continue;
                }
                else
                {
                    // If the pay period spans multiple months, we need to apply the surcharge for each month.
                    var startMonth = paycheck.PayPeriodStart.Month;
                    var startMonthDays = DateTime.DaysInMonth(paycheck.PayPeriodStart.Year, startMonth);
                    var startMonthEnd = new DateTime(paycheck.PayPeriodStart.Year, startMonth, startMonthDays);
                    var startMonthSurcharge = _monthlySurcharge * prorationPolicy.GetProrationFactorDays(dependent, paycheck.PayPeriodStart, startMonthEnd) / startMonthDays;

                    var endMonthStart = new DateTime(paycheck.PayPeriodEnd.Year, paycheck.PayPeriodEnd.Month, 1);
                    var endMonthDays = DateTime.DaysInMonth(paycheck.PayPeriodEnd.Year, paycheck.PayPeriodEnd.Month);
                    var endMonthSurcharge = _monthlySurcharge * prorationPolicy.GetProrationFactorDays(dependent, endMonthStart, paycheck.PayPeriodEnd) / endMonthDays;
                    paycheck.BenefitsCost += Math.Round(startMonthSurcharge + endMonthSurcharge, 2);
                }
            }
        }
    }
}
