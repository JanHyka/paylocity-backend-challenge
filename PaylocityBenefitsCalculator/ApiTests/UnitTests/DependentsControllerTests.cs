using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Controllers;
using Api.Dtos.Dependent;
using Api.Models;
using Api.Services.DependentsService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ApiTests.UnitTests;

public class DependentsControllerTests
{
    private readonly Mock<IDependentsService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DependentsController _controller;

    public DependentsControllerTests()
    {
        _serviceMock = new Mock<IDependentsService>();
        _mapperMock = new Mock<IMapper>();
        _controller = new DependentsController(_serviceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsDependent_WhenFound()
    {
        // Arrange
        var dependent = new Dependent
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            DateOfBirth = new DateTime(2000, 1, 1),
            Relationship = Relationship.Child
        };
        var dto = new GetDependentDto
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            DateOfBirth = new DateTime(2000, 1, 1),
            Relationship = Relationship.Child
        };

        _serviceMock.Setup(s => s.GetDependentById(1)).ReturnsAsync(dependent);
        _mapperMock.Setup(m => m.Map<GetDependentDto>(dependent)).Returns(dto);

        // Act
        var result = await _controller.Get(1);

        // Assert
        var okResult = Assert.IsType<ActionResult<ApiResponse<GetDependentDto>>>(result);
        var response = Assert.IsType<ApiResponse<GetDependentDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(dto.Id, response.Data!.Id);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetDependentById(999)).ThrowsAsync(new KeyNotFoundException("not found"));

        // Act
        var result = await _controller.Get(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<GetDependentDto>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("not found", response.Message);
    }

    [Fact]
    public async Task GetAll_ReturnsAllDependents()
    {
        // Arrange
        var dependents = new List<Dependent>
        {
            new() { Id = 1, FirstName = "A", LastName = "B", DateOfBirth = DateTime.Today, Relationship = Relationship.Spouse },
            new() { Id = 2, FirstName = "C", LastName = "D", DateOfBirth = DateTime.Today, Relationship = Relationship.Child }
        };
        var dtos = new List<GetDependentDto>
        {
            new() { Id = 1, FirstName = "A", LastName = "B", DateOfBirth = DateTime.Today, Relationship = Relationship.Spouse },
            new() { Id = 2, FirstName = "C", LastName = "D", DateOfBirth = DateTime.Today, Relationship = Relationship.Child }
        };

        _serviceMock.Setup(s => s.GetAllDependents()).ReturnsAsync(dependents);
        _mapperMock.Setup(m => m.Map<List<GetDependentDto>>(dependents)).Returns(dtos);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<ActionResult<ApiResponse<List<GetDependentDto>>>>(result);
        var response = Assert.IsType<ApiResponse<List<GetDependentDto>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data!.Count);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoDependents()
    {
        // Arrange
        var dependents = new List<Dependent>();
        var dtos = new List<GetDependentDto>();

        _serviceMock.Setup(s => s.GetAllDependents()).ReturnsAsync(dependents);
        _mapperMock.Setup(m => m.Map<List<GetDependentDto>>(dependents)).Returns(dtos);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<ActionResult<ApiResponse<List<GetDependentDto>>>>(result);
        var response = Assert.IsType<ApiResponse<List<GetDependentDto>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data);
    }
}