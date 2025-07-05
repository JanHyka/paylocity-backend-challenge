using Api.Models;

namespace Api.Services.PaycheckServices.Calculator.ApplicabilityPolicies
{
    /// <summary>
    /// Applicability policy that determines if a <see cref="Dependent"/> is over a specified age by the end of the pay period.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DependantOverAgeFromBirthdayPolicy"/> class.
    /// </remarks>
    /// <param name="ageLowerBound">The minimum age (in years) the dependent must reach to be considered applicable.</param>
    public class DependantOverAgeFromBirthdayPolicy(int ageLowerBound) : IApplicabilityPolicy<Dependent>
    {
        private readonly int _ageLowerBound = ageLowerBound;

        /// <summary>
        /// Determines whether the specified <see cref="Dependent"/> is over the configured age by the end of the pay period.
        /// </summary>
        /// <param name="person">The <see cref="Dependent"/> to check.</param>
        /// <param name="periodStart">The start date of the pay period. (Not used in this policy.)</param>
        /// <param name="periodEnd">The end date of the pay period.</param>
        /// <returns>
        /// <c>true</c> if the dependent's <see cref="Dependent.DateOfBirth"/> plus <c>_ageLowerBound</c> years is on or before <paramref name="periodEnd"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApplicable(Dependent person, DateTime periodStart, DateTime periodEnd)
        {
            var birhdayBoundary = person.DateOfBirth.AddYears(_ageLowerBound);
            return birhdayBoundary <= periodEnd;
        }
    }
}
