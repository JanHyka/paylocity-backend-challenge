using Api.Models;

namespace Api.Services.PaycheckServices.Calculator.Rules
{
    /// <summary>
    /// Defines a rule that can be applied to a <see cref="Paycheck"/> calculation for a given <see cref="Employee"/>.
    /// </summary>
    public interface IPaycheckRule
    {
        /// <summary>
        /// Applies the rule to the specified <see cref="Paycheck"/> and <see cref="Employee"/>.
        /// Implementations may modify the <paramref name="paycheck"/> based on the employee's data and rule logic.
        /// </summary>
        /// <param name="paycheck">The <see cref="Paycheck"/> to which the rule will be applied.</param>
        /// <param name="employee">The <see cref="Employee"/> for whom the paycheck is being calculated.</param>
        void Apply(Paycheck paycheck, Employee employee);
    }
}
