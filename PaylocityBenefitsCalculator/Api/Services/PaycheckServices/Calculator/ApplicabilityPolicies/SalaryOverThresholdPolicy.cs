using Api.Models;

namespace Api.Services.PaycheckServices.Calculator.ApplicabilityPolicies
{
    /// <summary>
    /// Applicability policy that determines if an <see cref="Employee"/>'s salary is over a specified threshold.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SalaryOverThresholdPolicy"/> class.
    /// </remarks>
    /// <param name="salaryThreshold">The salary threshold to compare against.</param>
    public class SalaryOverThresholdPolicy(decimal salaryThreshold) : IApplicabilityPolicy<Employee>
    {
        private readonly decimal _salaryThreshold = salaryThreshold;

        /// <summary>
        /// Determines whether the specified <see cref="Employee"/>'s salary is greater than the configured threshold.
        /// </summary>
        /// <param name="person">The <see cref="Employee"/> to check.</param>
        /// <param name="periodStart">The start date of the pay period. (Not used in this policy.)</param>
        /// <param name="periodEnd">The end date of the pay period. (Not used in this policy.)</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Employee"/>'s <see cref="Employee.Salary"/> is greater than the threshold; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApplicable(Employee person, DateTime periodStart, DateTime periodEnd)
        {
            return person.Salary > _salaryThreshold;
        }
    }
}
