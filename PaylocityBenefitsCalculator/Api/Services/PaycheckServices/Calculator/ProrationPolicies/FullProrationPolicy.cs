using Api.Models;

namespace Api.Services.PaycheckServices.Calculator.ProrationPolicies
{
    /// <summary>
    /// Proration policy that always returns the full number of days in the pay period for a <see cref="Dependent"/>.
    /// </summary>
    public class FullProrationPolicy : IProrationPolicy<Dependent>
    {
        /// <summary>
        /// Returns the total number of days in the pay period, including both the start and end dates.
        /// </summary>
        /// <param name="person">The <see cref="Dependent"/> for which to calculate the proration. (Not used in this policy.)</param>
        /// <param name="periodStart">The start date of the pay period.</param>
        /// <param name="periodEnd">The end date of the pay period.</param>
        /// <returns>
        /// The total number of days in the pay period, including both <paramref name="periodStart"/> and <paramref name="periodEnd"/>.
        /// </returns>
        public int GetProrationFactorDays(Dependent person, DateTime periodStart, DateTime periodEnd)
        {
            return (periodEnd - periodStart).Days + 1; // Full proration for the entire period
        }
    }
}
