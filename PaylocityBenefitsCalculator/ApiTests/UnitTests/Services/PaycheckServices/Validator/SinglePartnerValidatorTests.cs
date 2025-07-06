using System;
using System.Collections.Generic;
using Api.Models;
using Api.Services.PaycheckServices.Validator;
using Xunit;

namespace ApiTests.UnitTests.Services.PaycheckServices.Validator;

public class SinglePartnerValidatorTests
{
    private readonly SinglePartnerValidator _validator = new();

    [Fact]
    public void IsValid_ReturnsTrue_WhenNoDependents()
    {
        var employee = new Employee
        {
            Dependents = []
        };

        Assert.True(_validator.IsValid(employee));
    }

    [Fact]
    public void IsValid_ReturnsTrue_WhenNoPartnerDependents()
    {
        var employee = new Employee
        {
            Dependents =
            [
                new Dependent { Relationship = Relationship.Child },
                new Dependent { Relationship = Relationship.Child }
            ]
        };

        Assert.True(_validator.IsValid(employee));
    }

    [Fact]
    public void IsValid_ReturnsTrue_WhenOneSpouse()
    {
        var employee = new Employee
        {
            Dependents =
            [
                new Dependent { Relationship = Relationship.Spouse }
            ]
        };

        Assert.True(_validator.IsValid(employee));
    }

    [Fact]
    public void IsValid_ReturnsTrue_WhenOneDomesticPartner()
    {
        var employee = new Employee
        {
            Dependents =
            [
                new Dependent { Relationship = Relationship.DomesticPartner }
            ]
        };

        Assert.True(_validator.IsValid(employee));
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenSpouseAndDomesticPartner()
    {
        var employee = new Employee
        {
            Dependents =
            [
                new Dependent { Relationship = Relationship.Spouse },
                new Dependent { Relationship = Relationship.DomesticPartner }
            ]
        };

        Assert.False(_validator.IsValid(employee));
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenMultipleSpouses()
    {
        var employee = new Employee
        {
            Dependents =
            [
                new Dependent { Relationship = Relationship.Spouse },
                new Dependent { Relationship = Relationship.Spouse }
            ]
        };

        Assert.False(_validator.IsValid(employee));
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenMultipleDomesticPartners()
    {
        var employee = new Employee
        {
            Dependents =
            [
                new Dependent { Relationship = Relationship.DomesticPartner },
                new Dependent { Relationship = Relationship.DomesticPartner }
            ]
        };

        Assert.False(_validator.IsValid(employee));
    }

    [Fact]
    public void IsValid_ReturnsTrue_WhenOnePartnerAndOtherChildren()
    {
        var employee = new Employee
        {
            Dependents =
            [
                new Dependent { Relationship = Relationship.Spouse },
                new Dependent { Relationship = Relationship.Child },
                new Dependent { Relationship = Relationship.Child }
            ]
        };

        Assert.True(_validator.IsValid(employee));
    }
}