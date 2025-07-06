namespace Api.Services.PaycheckServices.Calculator.ProrationPolicies;

/// <summary>
/// Defines a policy for calculating the proration factor (in days) for a given entity of type <typeparamref name="T"/> over a pay period.
/// </summary>
/// <typeparam name="T">
/// The type of entity to which the proration policy applies, such as <see cref="Api.Models.Dependent"/> or <see cref="Api.Models.Employee"/>.
/// </typeparam>
public interface IProrationPolicy<T>
{
    /// <summary>
    /// Calculates the proration factor, in days, for the specified entity over the given pay period.
    /// </summary>
    /// <param name="person">The entity of type <typeparamref name="T"/> for which to calculate the proration.</param>
    /// <param name="periodStart">The start date of the pay period.</param>
    /// <param name="periodEnd">The end date of the pay period.</param>
    /// <returns>
    /// The number of days within the pay period for which the proration applies.
    /// </returns>
    int GetProrationFactorDays(T person, DateTime periodStart, DateTime periodEnd);
}
