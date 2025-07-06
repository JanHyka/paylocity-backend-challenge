using Api.Models;

namespace Api.Services.PaycheckServices.Calculator.ProrationPolicies
{
    /// <summary>
    /// Proration policy that calculates the number of days a <see cref="Dependent"/> is over a specified age within a pay period.
    /// The proration is based on the dependent's <see cref="Dependent.DateOfBirth"/> and the configured age lower bound.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ProrationByAgePolicy"/> class.
    /// </remarks>
    /// <param name="ageLowerBound">The minimum age (in years) the dependent must reach for policy to apply.</param>
    public class ProrationByAgePolicy(int ageLowerBound) : IProrationPolicy<Dependent>
    {
        private readonly int _ageLowerBound = ageLowerBound;

        /// <summary>
        /// Calculates the number of days within the pay period that the <see cref="Dependent"/> is over the configured age.
        /// Returns 0 if the dependent does not reach the age during the period.
        /// Returns the full period if the dependent was already over the age at the start.
        /// Otherwise, returns the number of days from the birthday boundary to the end of the period.
        /// </summary>
        /// <param name="person">The <see cref="Dependent"/> for which to calculate proration.</param>
        /// <param name="periodStart">The start date of the pay period.</param>
        /// <param name="periodEnd">The end date of the pay period.</param>
        /// <returns>
        /// The number of days in the pay period that the dependent is over the specified age.
        /// </returns>
        public int GetProrationFactorDays(Dependent person, DateTime periodStart, DateTime periodEnd)
        {
            var applyDate = person.DateOfBirth.AddYears(_ageLowerBound);

            if (applyDate > periodEnd)
            {
                return 0; // No proration if apply date is past the period
            }

            if (applyDate < periodStart)
            {
                return (int)(periodEnd - periodStart).TotalDays + 1; // Full proration if apply date is before the period
            }

            // Calculate the number of days from the apply date to the end of the period
            var daysFromApplyDateToEnd = (int)(periodEnd - applyDate).TotalDays + 1;

            return daysFromApplyDateToEnd;
        }
    }
}
