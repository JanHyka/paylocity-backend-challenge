using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dtos.Paycheck;
using Api.Models;
using Api.Services.DependentsServices;
using Api.Services.EmployeesServices;
using Api.Services.PaycheckServices;
using Api.Services.PaycheckServices.Calculator.Models;
using Api.Services.PaycheckServices.Validator;
using Moq;
using Xunit;

namespace ApiTests.UnitTests.Services.PaycheckServices;

public class PaycheckServiceTests
{
    private readonly Mock<IEmployeesService> _employeesServiceMock;
    private readonly Mock<IDependentsService> _dependentsServiceMock;
    private readonly Mock<ICalculatorModel> _calculatorModelMock;
    private readonly Mock<IEmployeeValidator> _employeeValidatorMock;
    private readonly PaycheckService _service;

    public PaycheckServiceTests()
    {
        _employeesServiceMock = new Mock<IEmployeesService>();
        _dependentsServiceMock = new Mock<IDependentsService>();
        _calculatorModelMock = new Mock<ICalculatorModel>();
        _employeeValidatorMock = new Mock<IEmployeeValidator>();
        _service = new PaycheckService(
            _employeesServiceMock.Object,
            _dependentsServiceMock.Object,
            _calculatorModelMock.Object,
            _employeeValidatorMock.Object);
    }

    [Fact]
    public async Task CalculatePaycheck_ReturnsPaycheck_WhenEmployeeAndDependentsValid()
    {
        // Arrange
        int employeeId = 1;
        var startDate = new DateTime(2024, 6, 1);
        var periodicity = PaycheckPeriodicity.BiWeekly;
        var employee = new Employee { Id = employeeId, Salary = 1000m, Dependents = [] };
        var dependents = new List<Dependent>
        {
            new() { Id = 10, FirstName = "Dep", EmployeeId = employeeId }
        };
        var expectedPaycheck = new Paycheck
        {
            EmployeeId = employeeId,
            GrossPay = 1000m,
            BenefitsCost = 100m,
            PayPeriodStart = startDate,
            PayPeriodEnd = startDate.GetBiWeekEnd()
        };

        _employeesServiceMock.Setup(s => s.GetEmployeeById(employeeId)).ReturnsAsync(employee);
        _dependentsServiceMock.Setup(s => s.GetDependentsByEmployeeId(employeeId)).ReturnsAsync(dependents);
        _employeeValidatorMock.Setup(v => v.IsValid(It.IsAny<Employee>())).Returns(true);
        _calculatorModelMock.Setup(m => m.CalculatePaycheck(employee, startDate, startDate.GetBiWeekEnd())).Returns(expectedPaycheck);

        // Act
        var result = await _service.CalculatePaycheck(employeeId, startDate, periodicity);

        // Assert
        Assert.Equal(expectedPaycheck, result);
        Assert.Equal(dependents, employee.Dependents);
    }

    [Fact]
    public async Task CalculatePaycheck_EmployeeWithNoDependents_StillValid()
    {
        // Arrange
        int employeeId = 2;
        var startDate = new DateTime(2024, 7, 1);
        var periodicity = PaycheckPeriodicity.BiWeekly;
        var employee = new Employee { Id = employeeId, Salary = 2000m, Dependents = [] };
        var expectedPaycheck = new Paycheck
        {
            EmployeeId = employeeId,
            GrossPay = 2000m,
            BenefitsCost = 0m,
            PayPeriodStart = startDate,
            PayPeriodEnd = startDate.GetBiWeekEnd()
        };

        _employeesServiceMock.Setup(s => s.GetEmployeeById(employeeId)).ReturnsAsync(employee);
        _dependentsServiceMock.Setup(s => s.GetDependentsByEmployeeId(employeeId)).ThrowsAsync(new KeyNotFoundException());
        _employeeValidatorMock.Setup(v => v.IsValid(It.IsAny<Employee>())).Returns(true);
        _calculatorModelMock.Setup(m => m.CalculatePaycheck(employee, startDate, startDate.GetBiWeekEnd())).Returns(expectedPaycheck);

        // Act
        var result = await _service.CalculatePaycheck(employeeId, startDate, periodicity);

        // Assert
        Assert.Equal(expectedPaycheck, result);
        Assert.Empty(employee.Dependents);
    }

    [Fact]
    public async Task CalculatePaycheck_ThrowsArgumentException_WhenEmployeeInvalid()
    {
        // Arrange
        int employeeId = 3;
        var startDate = new DateTime(2024, 8, 1);
        var periodicity = PaycheckPeriodicity.BiWeekly;
        var employee = new Employee { Id = employeeId, Salary = 3000m, Dependents = [] };

        _employeesServiceMock.Setup(s => s.GetEmployeeById(employeeId)).ReturnsAsync(employee);
        _dependentsServiceMock.Setup(s => s.GetDependentsByEmployeeId(employeeId)).ReturnsAsync([]);
        _employeeValidatorMock.Setup(v => v.IsValid(It.IsAny<Employee>())).Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CalculatePaycheck(employeeId, startDate, periodicity));
    }

    [Fact]
    public async Task CalculatePaycheck_PropagatesEmployeeServiceException()
    {
        // Arrange
        int employeeId = 4;
        var startDate = new DateTime(2024, 9, 1);
        var periodicity = PaycheckPeriodicity.BiWeekly;

        _employeesServiceMock.Setup(s => s.GetEmployeeById(employeeId)).ThrowsAsync(new KeyNotFoundException("not found"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.CalculatePaycheck(employeeId, startDate, periodicity));
        Assert.Equal("not found", ex.Message);
    }

    [Fact]
    public async Task CalculatePaycheck_CallsCalculatorModel_WithCorrectDates()
    {
        // Arrange
        int employeeId = 5;
        var startDate = new DateTime(2024, 10, 1);
        var periodicity = PaycheckPeriodicity.BiWeekly;
        var employee = new Employee { Id = employeeId, Salary = 4000m, Dependents = [] };
        var expectedPaycheck = new Paycheck
        {
            EmployeeId = employeeId,
            GrossPay = 4000m,
            BenefitsCost = 0m,
            PayPeriodStart = startDate,
            PayPeriodEnd = startDate.GetBiWeekEnd()
        };

        _employeesServiceMock.Setup(s => s.GetEmployeeById(employeeId)).ReturnsAsync(employee);
        _dependentsServiceMock.Setup(s => s.GetDependentsByEmployeeId(employeeId)).ReturnsAsync([]);
        _employeeValidatorMock.Setup(v => v.IsValid(It.IsAny<Employee>())).Returns(true);
        _calculatorModelMock.Setup(m => m.CalculatePaycheck(employee, startDate, startDate.GetBiWeekEnd())).Returns(expectedPaycheck);

        // Act
        var result = await _service.CalculatePaycheck(employeeId, startDate, periodicity);

        // Assert
        _calculatorModelMock.Verify(m => m.CalculatePaycheck(employee, startDate, startDate.GetBiWeekEnd()), Times.Once);
        Assert.Equal(expectedPaycheck, result);
    }
}