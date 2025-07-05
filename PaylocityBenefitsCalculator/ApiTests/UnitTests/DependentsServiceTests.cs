using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Services.DependentsService;
using Xunit;

namespace ApiTests.UnitTests;

public class DependentsServiceTests
{
    private readonly DependentsService _service = new();

    [Fact]
    public async Task GetAllDependents_ReturnsAllDependents()
    {
        // Act
        var result = await _service.GetAllDependents();

        // Assert
        Assert.NotNull(result);
        var dependents = result.ToList();
        Assert.Equal(4, dependents.Count);

        Assert.Contains(dependents, d => d.Id == 1 && d.FirstName == "Spouse");
        Assert.Contains(dependents, d => d.Id == 2 && d.FirstName == "Child1");
        Assert.Contains(dependents, d => d.Id == 3 && d.FirstName == "Child2");
        Assert.Contains(dependents, d => d.Id == 4 && d.FirstName == "DP");
    }

    [Theory]
    [InlineData(1, "Spouse", Relationship.Spouse, 2)]
    [InlineData(2, "Child1", Relationship.Child, 2)]
    [InlineData(3, "Child2", Relationship.Child, 2)]
    [InlineData(4, "DP", Relationship.DomesticPartner, 3)]
    public async Task GetDependentById_ValidId_ReturnsCorrectDependent(int id, string expectedFirstName, Relationship expectedRelationship, int expectedEmployeeId)
    {
        // Act
        var dependent = await _service.GetDependentById(id);

        // Assert
        Assert.NotNull(dependent);
        Assert.Equal(id, dependent.Id);
        Assert.Equal(expectedFirstName, dependent.FirstName);
        Assert.Equal(expectedRelationship, dependent.Relationship);
        Assert.Equal(expectedEmployeeId, dependent.EmployeeId);
    }

    [Fact]
    public async Task GetDependentById_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        int invalidId = 999;

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetDependentById(invalidId));
        Assert.Contains($"Dependent with ID {invalidId} not found.", ex.Message);
    }

    [Theory]
    [InlineData(2, 3)] // EmployeeId 2 has 3 dependents
    [InlineData(3, 1)] // EmployeeId 3 has 1 dependent
    public async Task GetDependentsByEmployeeId_ValidEmployeeId_ReturnsDependents(int employeeId, int expectedCount)
    {
        // Act
        var dependents = await _service.GetDependentsByEmployeeId(employeeId);

        // Assert
        Assert.NotNull(dependents);
        var list = dependents.ToList();
        Assert.Equal(expectedCount, list.Count);
        Assert.All(list, d => Assert.Equal(employeeId, d.EmployeeId));
    }

    [Fact]
    public async Task GetDependentsByEmployeeId_InvalidEmployeeId_ThrowsKeyNotFoundException()
    {
        // Arrange
        int invalidEmployeeId = 999;

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetDependentsByEmployeeId(invalidEmployeeId));
        Assert.Contains($"No dependents found for Employee ID {invalidEmployeeId}.", ex.Message);
    }

    [Fact]
    public async Task Dependents_AreImmutable()
    {
        // Arrange
        var dependents = await _service.GetAllDependents();

        // Act & Assert
        foreach (var dependent in dependents)
        {
            Assert.Throws<NotSupportedException>(() => ((ICollection<Dependent>)dependents).Add(new Dependent()));
        }
    }
}