using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Controllers;
using Api.Dtos.Employee;
using Api.Dtos.Dependent;
using Api.Models;
using Api.Services.EmployeesService;
using Api.Services.DependentsService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ApiTests.UnitTests;

public class EmployeesControllerTests
{
    private readonly Mock<IEmployeesService> _employeesServiceMock;
    private readonly Mock<IDependentsService> _dependentsServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly EmployeesController _controller;

    public EmployeesControllerTests()
    {
        _employeesServiceMock = new Mock<IEmployeesService>();
        _dependentsServiceMock = new Mock<IDependentsService>();
        _mapperMock = new Mock<IMapper>();
        _controller = new EmployeesController(
            _employeesServiceMock.Object,
            _dependentsServiceMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsEmployeeWithDependents_WhenFound()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Salary = 1000m,
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        var dependents = new List<Dependent>
        {
            new Dependent { Id = 10, FirstName = "Dep", LastName = "User", Relationship = Relationship.Child, DateOfBirth = new DateTime(2010, 1, 1) }
        };
        var employeeDto = new GetEmployeeDto
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Salary = 1000m,
            DateOfBirth = new DateTime(1990, 1, 1),
            Dependents = new List<GetDependentDto>()
        };
        var dependentDtos = new List<GetDependentDto>
        {
            new GetDependentDto { Id = 10, FirstName = "Dep", LastName = "User", Relationship = Relationship.Child, DateOfBirth = new DateTime(2010, 1, 1) }
        };

        _employeesServiceMock.Setup(s => s.GetEmployeeById(1)).ReturnsAsync(employee);
        _dependentsServiceMock.Setup(s => s.GetDependentsByEmployeeId(1)).ReturnsAsync(dependents);
        _mapperMock.Setup(m => m.Map<GetEmployeeDto>(employee)).Returns(employeeDto);
        _mapperMock.Setup(m => m.Map<List<GetDependentDto>>(dependents)).Returns(dependentDtos);

        // Act
        var result = await _controller.Get(1);

        // Assert
        var okResult = Assert.IsType<ActionResult<ApiResponse<GetEmployeeDto>>>(result);
        var response = Assert.IsType<ApiResponse<GetEmployeeDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(employeeDto.Id, response.Data.Id);
        Assert.Equal(dependentDtos.Count, response.Data.Dependents.Count);
    }

    [Fact]
    public async Task Get_ReturnsEmployeeWithNoDependents_WhenNoneExist()
    {
        // Arrange
        var employee = new Employee { Id = 2, FirstName = "NoDeps", LastName = "User", Salary = 2000m, DateOfBirth = new DateTime(1980, 1, 1) };
        var employeeDto = new GetEmployeeDto
        {
            Id = 2,
            FirstName = "NoDeps",
            LastName = "User",
            Salary = 2000m,
            DateOfBirth = new DateTime(1980, 1, 1),
            Dependents = new List<GetDependentDto>()
        };

        _employeesServiceMock.Setup(s => s.GetEmployeeById(2)).ReturnsAsync(employee);
        _dependentsServiceMock.Setup(s => s.GetDependentsByEmployeeId(2)).ThrowsAsync(new KeyNotFoundException());
        _mapperMock.Setup(m => m.Map<GetEmployeeDto>(employee)).Returns(employeeDto);
        _mapperMock.Setup(m => m.Map<List<GetDependentDto>>(It.IsAny<IEnumerable<Dependent>>())).Returns(new List<GetDependentDto>());

        // Act
        var result = await _controller.Get(2);

        // Assert
        var okResult = Assert.IsType<ActionResult<ApiResponse<GetEmployeeDto>>>(result);
        var response = Assert.IsType<ApiResponse<GetEmployeeDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data.Dependents);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        _employeesServiceMock.Setup(s => s.GetEmployeeById(999)).ThrowsAsync(new KeyNotFoundException("not found"));

        // Act
        var result = await _controller.Get(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<GetEmployeeDto>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("not found", response.Message);
    }

    [Fact]
    public async Task GetAll_ReturnsAllEmployeesWithDependents()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee { Id = 1, FirstName = "A", LastName = "B", Salary = 1000m, DateOfBirth = DateTime.Today },
            new Employee { Id = 2, FirstName = "C", LastName = "D", Salary = 2000m, DateOfBirth = DateTime.Today }
        };
        var dependents1 = new List<Dependent>
        {
            new Dependent { Id = 10, FirstName = "Dep1", LastName = "B", Relationship = Relationship.Child, DateOfBirth = DateTime.Today }
        };
        var dependents2 = new List<Dependent>
        {
            new Dependent { Id = 20, FirstName = "Dep2", LastName = "D", Relationship = Relationship.Spouse, DateOfBirth = DateTime.Today }
        };
        var employeeDto1 = new GetEmployeeDto { Id = 1, FirstName = "A", LastName = "B", Salary = 1000m, DateOfBirth = DateTime.Today, Dependents = new List<GetDependentDto>() };
        var employeeDto2 = new GetEmployeeDto { Id = 2, FirstName = "C", LastName = "D", Salary = 2000m, DateOfBirth = DateTime.Today, Dependents = new List<GetDependentDto>() };
        var dependentDtos1 = new List<GetDependentDto> { new GetDependentDto { Id = 10, FirstName = "Dep1", LastName = "B", Relationship = Relationship.Child, DateOfBirth = DateTime.Today } };
        var dependentDtos2 = new List<GetDependentDto> { new GetDependentDto { Id = 20, FirstName = "Dep2", LastName = "D", Relationship = Relationship.Spouse, DateOfBirth = DateTime.Today } };

        _employeesServiceMock.Setup(s => s.GetAllEmployees()).ReturnsAsync(employees);
        _mapperMock.Setup(m => m.Map<GetEmployeeDto>(employees[0])).Returns(employeeDto1);
        _mapperMock.Setup(m => m.Map<GetEmployeeDto>(employees[1])).Returns(employeeDto2);
        _dependentsServiceMock.Setup(s => s.GetDependentsByEmployeeId(1)).ReturnsAsync(dependents1);
        _dependentsServiceMock.Setup(s => s.GetDependentsByEmployeeId(2)).ReturnsAsync(dependents2);
        _mapperMock.Setup(m => m.Map<List<GetDependentDto>>(dependents1)).Returns(dependentDtos1);
        _mapperMock.Setup(m => m.Map<List<GetDependentDto>>(dependents2)).Returns(dependentDtos2);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<ActionResult<ApiResponse<List<GetEmployeeDto>>>>(result);
        var response = Assert.IsType<ApiResponse<List<GetEmployeeDto>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(2, response.Data.Count);
        Assert.Single(response.Data[0].Dependents);
        Assert.Single(response.Data[1].Dependents);
    }

    [Fact]
    public async Task GetAll_EmployeeWithNoDependents_ReturnsEmptyDependents()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee { Id = 1, FirstName = "A", LastName = "B", Salary = 1000m, DateOfBirth = DateTime.Today }
        };
        var employeeDto1 = new GetEmployeeDto { Id = 1, FirstName = "A", LastName = "B", Salary = 1000m, DateOfBirth = DateTime.Today, Dependents = new List<GetDependentDto>() };

        _employeesServiceMock.Setup(s => s.GetAllEmployees()).ReturnsAsync(employees);
        _mapperMock.Setup(m => m.Map<GetEmployeeDto>(employees[0])).Returns(employeeDto1);
        _dependentsServiceMock.Setup(s => s.GetDependentsByEmployeeId(1)).ThrowsAsync(new KeyNotFoundException());
        _mapperMock.Setup(m => m.Map<List<GetDependentDto>>(It.IsAny<IEnumerable<Dependent>>())).Returns(new List<GetDependentDto>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<ActionResult<ApiResponse<List<GetEmployeeDto>>>>(result);
        var response = Assert.IsType<ApiResponse<List<GetEmployeeDto>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Single(response.Data);
        Assert.Empty(response.Data[0].Dependents);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoEmployees()
    {
        // Arrange
        _employeesServiceMock.Setup(s => s.GetAllEmployees()).ReturnsAsync(new List<Employee>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<ActionResult<ApiResponse<List<GetEmployeeDto>>>>(result);
        var response = Assert.IsType<ApiResponse<List<GetEmployeeDto>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Empty(response.Data);
    }
}