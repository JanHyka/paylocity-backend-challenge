using Api.Models;

namespace Api.Services.PaycheckServices.Calculator.Models
{
    /// <summary>
    /// Defines a model for calculating a <see cref="Paycheck"/> for a given <see cref="Employee"/> and pay period.
    /// </summary>
    public interface ICalculatorModel
    {
        /// <summary>
        /// Calculates a <see cref="Paycheck"/> for the specified <see cref="Employee"/> and pay period.
        /// </summary>
        /// <param name="employee">The <see cref="Employee"/> for whom the paycheck is being calculated.</param>
        /// <param name="periodStart">The start date of the pay period.</param>
        /// <param name="periodEnd">The end date of the pay period.</param>
        /// <returns>
        /// The calculated <see cref="Paycheck"/> for the specified employee and period.
        /// </returns>
        Paycheck CalculatePaycheck(
            Employee employee,
            DateTime periodStart,
            DateTime periodEnd);
    }
}
