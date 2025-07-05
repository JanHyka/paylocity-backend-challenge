using Api.Models;
using System.Collections.Frozen;

namespace Api.Services.DependentsService
{
    /// <summary>
    /// Mock implementation of a service for managing dependents.
    /// Most likely, this will be replaced with a real implementation that interacts with RESTFul APIs or a database.
    /// </summary>
    public class DependentsService : IDependentsService
    {
        private static readonly FrozenDictionary<int, Dependent> _dependents =
            new Dictionary<int, Dependent>
            {
                {
                    1, new Dependent
                    {
                        Id = 1,
                        FirstName = "Spouse",
                        LastName = "Morant",
                        Relationship = Relationship.Spouse,
                        DateOfBirth = new DateTime(1998, 3, 3),
                        EmployeeId = 2
                    }
                },
                {
                    2, new Dependent
                    {
                        Id = 2,
                        FirstName = "Child1",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2020, 6, 23),
                        EmployeeId = 2
                    }
                },
                {
                    3, new Dependent
                    {
                        Id = 3,
                        FirstName = "Child2",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2021, 5, 18),
                        EmployeeId = 2
                    }
                },
                {
                    4, new Dependent
                    {
                        Id = 4,
                        FirstName = "DP",
                        LastName = "Jordan",
                        Relationship = Relationship.DomesticPartner,
                        DateOfBirth = new DateTime(1974, 1, 2),
                        EmployeeId = 3
                    }
                }
            }.ToFrozenDictionary();

        // Intermediate collection for fast lookup by EmployeeId
        private static readonly FrozenDictionary<int, FrozenSet<Dependent>> _dependentsByEmployeeId =
            _dependents.Values
                .GroupBy(d => d.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToFrozenSet())
                .ToFrozenDictionary();

        /// <inheritdoc />
        public Task<IEnumerable<Dependent>> GetAllDependents()
        {
            return Task.FromResult<IEnumerable<Dependent>>(_dependents.Values);
        }

        /// <inheritdoc />
        public Task<Dependent> GetDependentById(int id)
        {
            if (_dependents.TryGetValue(id, out var dependent))
            {
                return Task.FromResult(dependent);
            }
            throw new KeyNotFoundException($"Dependent with ID {id} not found.");
        }

        /// <inheritdoc />
        public Task<IEnumerable<Dependent>> GetDependentsByEmployeeId(int employeeId)
        {
            if (_dependentsByEmployeeId.TryGetValue(employeeId, out var dependents) && dependents.Any())
            {
                return Task.FromResult<IEnumerable<Dependent>>(dependents);
            }
            throw new KeyNotFoundException($"No dependents found for Employee ID {employeeId}.");
        }
    }
}
