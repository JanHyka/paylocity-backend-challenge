using Api.Models;

namespace Api.Services.PaycheckServices.Calculator.ApplicabilityPolicies
{
    /// <summary>
    /// Applicability policy that determines if a <see cref="Dependent"/> was born in the month of period start
    /// </summary>
    public class DependentWasBornInStartMonthPolicy : IApplicabilityPolicy<Dependent>
    {
        /// <summary>
        /// Determines whether the specified <see cref="Dependent"/> was born before the end of <paramref name="periodStart"/> month.
        /// </summary>
        /// <param name="person">The <see cref="Dependent"/> to check.</param>
        /// <param name="periodStart">The start date of the pay period.</param>
        /// <param name="periodEnd">The end date of the pay period. (Not used in this policy.)</param>
        /// <returns>
        /// <c>true</c> if the dependent's <see cref="Dependent.DateOfBirth"/> is on or after the first day of the month of <paramref name="periodStart"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool IsApplicable(Dependent person, DateTime periodStart, DateTime periodEnd)
            => person.DateOfBirth <= new DateTime(periodStart.Year, periodStart.Month, DateTime.DaysInMonth(periodStart.Year, periodStart.Month));
    }
}
