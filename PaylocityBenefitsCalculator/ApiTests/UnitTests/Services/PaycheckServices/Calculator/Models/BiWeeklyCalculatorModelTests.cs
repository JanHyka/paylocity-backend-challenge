using System;
using System.Collections.Generic;
using Api.Models;
using Api.Services.PaycheckServices.Calculator.Models;
using Xunit;

namespace ApiTests.UnitTests.Services.PaycheckServices.Calculator.Models
{
    public class BiWeeklyCalculatorModelTests
    {
        [Fact]
        public void CalculatePaycheck_BasicEmployee_NoDependents_UnderThreshold()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Salary = 70000m,
                DateOfBirth = new DateTime(1980, 1, 1),
                Dependents = []
            };
            var periodStart = new DateTime(2024, 6, 1);
            var periodEnd = periodStart.AddDays(13); // Bi-weekly (14 days)

            var model = new BiWeeklyCalculatorModel();

            // Act
            var paycheck = model.CalculatePaycheck(employee, periodStart, periodEnd);

            // Assert
            // GrossPay: 70000/366*14 = 2677.60
            Assert.Equal(2677.60m, paycheck.GrossPay);
            // BaseCostSurchargeRule: 1000/30*14 = 466.67 (June has 30 days)
            Assert.Equal(466.67m, paycheck.BenefitsCost);
        }

        [Fact]
        public void CalculatePaycheck_Employee_OverSalaryThreshold()
        {
            var employee = new Employee
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Salary = 90000m,
                DateOfBirth = new DateTime(1985, 2, 2),
                Dependents = []
            };
            var periodStart = new DateTime(2024, 6, 1);
            var periodEnd = periodStart.AddDays(13);

            var model = new BiWeeklyCalculatorModel();

            var paycheck = model.CalculatePaycheck(employee, periodStart, periodEnd);

            // GrossPay: 90000/366*14 = 3442.62
            Assert.Equal(3442.62m, paycheck.GrossPay);
            // BaseCost: 1000/30*14 = 466.67
            // SalaryOverThresholdRule: 90000*0.02 = 1800 annual, 14/366 = 68.85
            // Total = 466.67 + 68.85 = 535.52
            Assert.Equal(535.52m, paycheck.BenefitsCost);
        }

        [Fact]
        public void CalculatePaycheck_Employee_Dependent_BornInStartMonth()
        {
            var employee = new Employee
            {
                Id = 3,
                FirstName = "Alice",
                LastName = "Brown",
                Salary = 70000m,
                DateOfBirth = new DateTime(1990, 3, 3),
                Dependents =
                [
                    new() {
                        Id = 1,
                        FirstName = "Child",
                        DateOfBirth = new DateTime(2024, 6, 5),
                        Relationship = Relationship.Child
                    }
                ]
            };
            var periodStart = new DateTime(2024, 6, 1);
            var periodEnd = periodStart.AddDays(13);

            var model = new BiWeeklyCalculatorModel();

            var paycheck = model.CalculatePaycheck(employee, periodStart, periodEnd);

            // GrossPay: 70000/366*14 = 2677.60
            Assert.Equal(2677.60m, paycheck.GrossPay);
            // BaseCost: 1000/30*14 = 466.67
            // DependentWasBornInStartMonthPolicy: applies, FullProrationPolicy: 14 days, 600*14/30=280
            // Total = 466.67 + 280 = 746.67
            Assert.Equal(746.67m, paycheck.BenefitsCost);
        }

        [Fact]
        public void CalculatePaycheck_Employee_Dependent_BornBeforeStartMonth()
        {
            var employee = new Employee
            {
                Id = 3,
                FirstName = "Alice",
                LastName = "Brown",
                Salary = 70000m,
                DateOfBirth = new DateTime(1990, 3, 3),
                Dependents =
                [
                    new() {
                        Id = 1,
                        FirstName = "Child",
                        DateOfBirth = new DateTime(2024, 1, 5),
                        Relationship = Relationship.Child
                    }
                ]
            };
            var periodStart = new DateTime(2024, 6, 1);
            var periodEnd = periodStart.AddDays(13);

            var model = new BiWeeklyCalculatorModel();

            var paycheck = model.CalculatePaycheck(employee, periodStart, periodEnd);

            // GrossPay: 70000/366*14 = 2677.60
            Assert.Equal(2677.60m, paycheck.GrossPay);
            // DependentWasBornBeforeMonthPolicy: applies, FullProrationPolicy: 14 days, 600*14/30=280
            // Total = 466.67 + 280 = 746.67
            Assert.Equal(746.67m, paycheck.BenefitsCost);
        }

        [Fact]
        public void CalculatePaycheck_Employee_Dependent_OverAge50()
        {
            var employee = new Employee
            {
                Id = 4,
                FirstName = "Bob",
                LastName = "Gray",
                Salary = 70000m,
                DateOfBirth = new DateTime(1960, 1, 1),
                Dependents =
                [
                    new() {
                        Id = 2,
                        FirstName = "Parent",
                        DateOfBirth = new DateTime(1960, 1, 1),
                        Relationship = Relationship.DomesticPartner
                    }
                ]
            };
            var periodStart = new DateTime(2024, 6, 1);
            var periodEnd = periodStart.AddDays(13);

            var model = new BiWeeklyCalculatorModel();

            var paycheck = model.CalculatePaycheck(employee, periodStart, periodEnd);

            // GrossPay: 70000/366*14 = 2677.60
            Assert.Equal(2677.60m, paycheck.GrossPay);
            // DependantOverAgeFromBeforeMonthPolicy: applies, FullProrationByAgePolicy: 600*14/30=280.00
            // DependantOverAgeFromBirthdayPolicy(50): applies, ProrationByAgePolicy(50): full 14 days, 200*14/30=93.33
            // Total = 466.67 + 280 + 93.33 = 840
            Assert.Equal(840m, paycheck.BenefitsCost);
        }

        [Fact]
        public void CalculatePaycheck_Employee_AllRulesApply()
        {
            var employee = new Employee
            {
                Id = 5,
                FirstName = "Eve",
                LastName = "White",
                Salary = 90000m,
                DateOfBirth = new DateTime(1970, 1, 1),
                Dependents =
                [
                    new() {
                        Id = 3,
                        FirstName = "Child",
                        DateOfBirth = new DateTime(2024, 6, 2),
                        Relationship = Relationship.Child
                    },
                    new() {
                        Id = 4,
                        FirstName = "Parent",
                        DateOfBirth = new DateTime(1950, 1, 1),
                        Relationship = Relationship.DomesticPartner
                    }
                ]
            };
            var periodStart = new DateTime(2024, 6, 1);
            var periodEnd = periodStart.AddDays(13);

            var model = new BiWeeklyCalculatorModel();

            var paycheck = model.CalculatePaycheck(employee, periodStart, periodEnd);

            // GrossPay: 90000/366*14 = 3442.62
            Assert.Equal(3442.62m, paycheck.GrossPay);
            // BaseCost: 1000/30*14 = 466.67
            // Dependent 1: Born in start month, FullProration: 600*14/30=280
            // Dependent 2: Born before start month, FullProration: 600*14/30=280
            // Dependent 2: Over 50, ProrationByAge: 200*14/30=93.33
            // SalaryOverThreshold: 90000*0.02=1800, 14/366=68.85
            // Total = 466.67 + 280 + 280 + 93.33 + 68.85 = 1188.85
            Assert.Equal(1188.85m, paycheck.BenefitsCost);
        }

        [Fact]
        public void CalculatePaycheck_Employee_Dependent_BornAfterStartMonth_NoSurcharge()
        {
            var employee = new Employee
            {
                Id = 6,
                FirstName = "Tom",
                LastName = "Blue",
                Salary = 70000m,
                DateOfBirth = new DateTime(1980, 1, 1),
                Dependents =
                [
                    new() {
                        Id = 5,
                        FirstName = "Child",
                        DateOfBirth = new DateTime(2024, 7, 1),
                        Relationship = Relationship.Child
                    }
                ]
            };
            var periodStart = new DateTime(2024, 6, 1);
            var periodEnd = periodStart.AddDays(13);

            var model = new BiWeeklyCalculatorModel();

            var paycheck = model.CalculatePaycheck(employee, periodStart, periodEnd);

            // GrossPay: 70000/366*14 = 2677.60
            Assert.Equal(2677.60m, paycheck.GrossPay);
            // Dependent not born in start month, so no 600 surcharge
            // Not over 50, so no 200 surcharge
            // Salary under threshold, so no salary surcharge
            Assert.Equal(466.67m, paycheck.BenefitsCost);
        }

        [Fact]
        public void CalculatePaycheck_Employee_Dependent_Turns50DuringPeriod_PartialSurcharge()
        {
            var employee = new Employee
            {
                Id = 7,
                FirstName = "Sam",
                LastName = "Green",
                Salary = 70000m,
                DateOfBirth = new DateTime(1970, 1, 1),
                Dependents =
                [
                    new() {
                        Id = 6,
                        FirstName = "Parent",
                        DateOfBirth = new DateTime(1974, 6, 8),
                        Relationship = Relationship.DomesticPartner
                    }
                ]
            };
            var periodStart = new DateTime(2024, 6, 1);
            var periodEnd = periodStart.AddDays(13); // 1-14 June

            var model = new BiWeeklyCalculatorModel();

            var paycheck = model.CalculatePaycheck(employee, periodStart, periodEnd);

            // GrossPay: 70000/366*14 = 2677.60
            Assert.Equal(2677.60m, paycheck.GrossPay);
            // Turns 50 on 2024-6-8, so 8,9,10,11,12,13,14 = 7 days
            // ProrationByAge: 200*7/30=46.67
            // BaseCost: 1000*14/30=466.67
            // Dependent was born before start month, so 600*14/30=280.00
            // Total = 466.67 + 280 + 46.67 = 793.34
            Assert.Equal(793.34m, paycheck.BenefitsCost);
        }

        [Fact]
        public void CalculatePaycheck_Employee_NoRulesApply()
        {
            var employee = new Employee
            {
                Id = 8,
                FirstName = "No",
                LastName = "Rules",
                Salary = 1000m,
                DateOfBirth = new DateTime(2000, 1, 1),
                Dependents = []
            };
            var periodStart = new DateTime(2024, 2, 1);
            var periodEnd = periodStart.AddDays(13);

            var model = new BiWeeklyCalculatorModel();

            var paycheck = model.CalculatePaycheck(employee, periodStart, periodEnd);

            // GrossPay: 1000/366*14 = 38.25 (Feb 2024 is leap year)
            Assert.Equal(38.25m, paycheck.GrossPay);
            // BaseCost: 1000/29*14 = 482.76 (Feb 2024 is leap year)
            Assert.Equal(482.76m, paycheck.BenefitsCost);
        }

        [Fact]
        public void CalculatePaycheck_Employee_PeriodSpansTwoYears()
        {
            var employee = new Employee
            {
                Id = 9,
                FirstName = "Year",
                LastName = "Spanner",
                Salary = 36500m,
                DateOfBirth = new DateTime(1990, 1, 1),
                Dependents = []
            };
            var periodStart = new DateTime(2023, 12, 28);
            var periodEnd = new DateTime(2024, 1, 4); // 8 days: 4 in 2023, 4 in 2024

            var model = new BiWeeklyCalculatorModel();

            var paycheck = model.CalculatePaycheck(employee, periodStart, periodEnd);

            // 2023: 4 days, 365 days in year, 36500/365*4 = 400
            // 2024: 4 days, 366 days in year, 36500/366*4 = 398.91
            // Total = 798.91
            Assert.Equal(798.91m, paycheck.GrossPay);
        }
    }
}