using Api.Models;

namespace Api.Services.PaycheckServices.Calculator.ApplicabilityPolicies
{
    /// <summary>
    /// Defines a policy for determining whether a given entity of type <typeparamref name="T"/> is applicable
    /// for a specific pay period.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity to which the applicability policy applies, such as <see cref="Employee"/> or <see cref="Dependent"/>.
    /// </typeparam>
    public interface IApplicabilityPolicy<T>
    {
        /// <summary>
        /// Determines whether the specified <paramref name="person"/> is applicable for the given pay period.
        /// </summary>
        /// <param name="person">The entity of type <typeparamref name="T"/> to check applicability for.</param>
        /// <param name="periodStart">The start date of the pay period.</param>
        /// <param name="periodEnd">The end date of the pay period.</param>
        /// <returns>
        /// <c>true</c> if the entity is applicable for the specified period; otherwise, <c>false</c>.
        /// </returns>
        bool IsApplicable(T person, DateTime periodStart, DateTime periodEnd);
    }
}
