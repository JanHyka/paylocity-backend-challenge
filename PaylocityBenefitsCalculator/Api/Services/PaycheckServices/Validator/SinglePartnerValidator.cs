using Api.Models;
using System.Collections.Frozen;

namespace Api.Services.PaycheckServices.Validator
{
    /// <summary>
    /// Validates that an <see cref="Employee"/> has at most one partner-type dependent
    /// (either <see cref="Relationship.Spouse"/> or <see cref="Relationship.DomesticPartner"/>).
    /// </summary>
    public class SinglePartnerValidator : IEmployeeValidator
    {
        private readonly FrozenSet<Relationship> _partnerRelationships = new HashSet<Relationship>
        {
            Relationship.Spouse,
            Relationship.DomesticPartner
        }.ToFrozenSet();

        /// <summary>
        /// Determines whether the specified <see cref="Employee"/> has at most one partner-type dependent.
        /// </summary>
        /// <param name="employee">The <see cref="Employee"/> to validate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Employee"/> has zero or one dependent with a
        /// <see cref="Relationship"/> of <see cref="Relationship.Spouse"/> or <see cref="Relationship.DomesticPartner"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid(Employee employee)
        {
            return employee.Dependents.Count(dependent => _partnerRelationships.Contains(dependent.Relationship)) <= 1;
        }
    }
}
