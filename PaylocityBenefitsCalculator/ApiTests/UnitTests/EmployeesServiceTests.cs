using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Services.EmployeesService;
using Xunit;

namespace ApiTests.UnitTests
{
    public class EmployeesServiceTests
    {
        private readonly EmployeesService _service = new();

        [Fact]
        public async Task GetAllEmployees_ReturnsAllEmployees()
        {
            // Act
            var result = await _service.GetAllEmployees();

            // Assert
            Assert.NotNull(result);
            var employees = result.ToList();
            Assert.Equal(3, employees.Count);

            Assert.Contains(employees, e => e.Id == 1 && e.FirstName == "LeBron" && e.LastName == "James");
            Assert.Contains(employees, e => e.Id == 2 && e.FirstName == "Ja" && e.LastName == "Morant");
            Assert.Contains(employees, e => e.Id == 3 && e.FirstName == "Michael" && e.LastName == "Jordan");
        }

        [Theory]
        [InlineData(1, "LeBron", "James", 75420.99, 1984, 12, 30)]
        [InlineData(2, "Ja", "Morant", 92365.22, 1999, 8, 10)]
        [InlineData(3, "Michael", "Jordan", 143211.12, 1963, 2, 17)]
        public async Task GetEmployeeById_ValidId_ReturnsCorrectEmployee(
            int id, string expectedFirstName, string expectedLastName, decimal expectedSalary, int year, int month, int day)
        {
            // Act
            var employee = await _service.GetEmployeeById(id);

            // Assert
            Assert.NotNull(employee);
            Assert.Equal(id, employee.Id);
            Assert.Equal(expectedFirstName, employee.FirstName);
            Assert.Equal(expectedLastName, employee.LastName);
            Assert.Equal(expectedSalary, employee.Salary);
            Assert.Equal(new DateTime(year, month, day), employee.DateOfBirth);
        }

        [Fact]
        public async Task GetEmployeeById_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            int invalidId = 999;

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetEmployeeById(invalidId));
            Assert.Contains($"Employee with ID {invalidId} not found.", ex.Message);
        }

        [Fact]
        public async Task Employees_AreImmutable()
        {
            // Arrange
            var employees = await _service.GetAllEmployees();

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => ((ICollection<Employee>)employees).Add(new Employee()));
        }
    }
}