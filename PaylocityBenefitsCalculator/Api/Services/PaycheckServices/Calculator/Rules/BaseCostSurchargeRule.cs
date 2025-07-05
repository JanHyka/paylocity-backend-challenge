using Api.Models;

namespace Api.Services.PaycheckServices.Calculator.Rules
{
    /// <summary>
    /// Paycheck rule that applies a base cost surcharge to a <see cref="Paycheck"/>.
    /// The surcharge is added to <see cref="Paycheck.GrossPay"/> if the pay period is within a single month,
    /// or split proportionally between months if the pay period spans multiple months.
    /// </summary>
    public class BaseCostSurchargeRule(decimal baseCost) : IPaycheckRule
    {
        private readonly decimal _baseCost = baseCost;

        /// <summary>
        /// Applies the base cost surcharge to the specified <see cref="Paycheck"/> for the given <see cref="Employee"/>.
        /// If the pay period is within a single month, the full surcharge is added to <see cref="Paycheck.BenefitsCost"/>. proportional to period lenght in the affected month.
        /// If the pay period spans multiple months, the surcharge is split and added to <see cref="Paycheck.BenefitsCost"/> for each month, proportional to the number of days in each month.
        /// </summary>
        /// <param name="paycheck">The <see cref="Paycheck"/> to which the surcharge will be applied.</param>
        /// <param name="employee">The <see cref="Employee"/> for whom the paycheck is being calculated.</param>
        public void Apply(Paycheck paycheck, Employee employee)
        {
            if (paycheck.PayPeriodStart.Month == paycheck.PayPeriodEnd.Month)
            {
                int daysInMonth = DateTime.DaysInMonth(paycheck.PayPeriodStart.Year, paycheck.PayPeriodStart.Month);
                var surcharge = _baseCost * (int)((paycheck.PayPeriodEnd - paycheck.PayPeriodStart).TotalDays + 1) / daysInMonth;
                paycheck.BenefitsCost += Math.Round(surcharge, 2);
            }
            else
            {
                // If the pay period spans multiple months, we need to apply the surcharge for each month.
                var startMonth = paycheck.PayPeriodStart.Month;
                var startMonthDays = DateTime.DaysInMonth(paycheck.PayPeriodStart.Year, startMonth);
                var startMonthEnd = new DateTime(paycheck.PayPeriodStart.Year, startMonth, startMonthDays);
                var startMonthSurcharge = _baseCost * (int)((startMonthEnd - paycheck.PayPeriodStart).TotalDays + 1) / startMonthDays;

                var endMonthStart = new DateTime(paycheck.PayPeriodEnd.Year, paycheck.PayPeriodEnd.Month, 1);
                var endMonthDays = DateTime.DaysInMonth(paycheck.PayPeriodEnd.Year, paycheck.PayPeriodEnd.Month);
                var endMonthSurcharge = _baseCost * (int)((paycheck.PayPeriodEnd - endMonthStart).TotalDays + 1) / endMonthDays;
                paycheck.BenefitsCost += Math.Round(startMonthSurcharge + endMonthSurcharge, 2);
            }
        }
    }
}
