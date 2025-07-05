using Api.Models;
using System.Collections.Frozen;

namespace Api.Services.EmployeesService
{
    /// <summary>
    /// Mock implementation of a service for managing <see cref="Employee"/> entities.
    /// Most likely, this will be replaced with a real implementation that interacts with RESTful APIs or a database.
    /// </summary>
    public class EmployeesService : IEmployeesService
    {
        private static readonly FrozenDictionary<int, Employee> _employees =
            new Dictionary<int, Employee>
            {
                {
                    1, new Employee
                    {
                        Id = 1,
                        FirstName = "LeBron",
                        LastName = "James",
                        Salary = 75420.99m,
                        DateOfBirth = new DateTime(1984, 12, 30)
                    }
                },
                {
                    2, new Employee
                    {
                        Id = 2,
                        FirstName = "Ja",
                        LastName = "Morant",
                        Salary = 92365.22m,
                        DateOfBirth = new DateTime(1999, 8, 10)
                    }
                },
                {
                    3, new Employee
                    {
                        Id = 3,
                        FirstName = "Michael",
                        LastName = "Jordan",
                        Salary = 143211.12m,
                        DateOfBirth = new DateTime(1963, 2, 17)
                    }
                }
            }.ToFrozenDictionary();

        /// <inheritdoc />
        public Task<IEnumerable<Employee>> GetAllEmployees()
        {
            return Task.FromResult<IEnumerable<Employee>>(_employees.Values);
        }

        /// <inheritdoc />
        public Task<Employee> GetEmployeeById(int id)
        {
            if (_employees.TryGetValue(id, out var employee))
            {
                return Task.FromResult(employee);
            }
            throw new KeyNotFoundException($"Employee with ID {id} not found.");
        }
    }
}
