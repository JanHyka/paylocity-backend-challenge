using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Controllers;
using Api.Dtos.Paycheck;
using Api.Models;
using Api.Services.PaycheckService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ApiTests.UnitTests;

public class PaychecksControllerTests
{
    private readonly Mock<IPaycheckService> _paycheckServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PaychecksController _controller;

    public PaychecksControllerTests()
    {
        _paycheckServiceMock = new Mock<IPaycheckService>();
        _mapperMock = new Mock<IMapper>();
        _controller = new PaychecksController(_paycheckServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsPaycheck_WhenFound()
    {
        // Arrange
        int userId = 1;
        DateTime startDate = new DateTime(2024, 1, 1);
        PaycheckPeriodicity periodicity = PaycheckPeriodicity.BiWeekly;
        var paycheck = new Paycheck
        {
            EmployeeId = userId,
            GrossPay = 2000m,
            BenefitsCost = 300m,
            PayPeriodStart = startDate,
            PayPeriodEnd = startDate.AddDays(14)
        };
        var dto = new GetPaycheckDto
        {
            EmployeeId = userId,
            GrossPay = 2000m,
            BenefitsCost = 300m,
            NetPay = 1700m,
            PayPeriodStart = startDate,
            PayPeriodEnd = startDate.AddDays(14)
        };

        _paycheckServiceMock.Setup(s => s.CalculatePaycheck(userId, startDate, periodicity))
            .ReturnsAsync(paycheck);
        _mapperMock.Setup(m => m.Map<GetPaycheckDto>(paycheck)).Returns(dto);

        // Act
        var result = await _controller.Get(userId, startDate, periodicity);

        // Assert
        var okResult = Assert.IsType<ActionResult<ApiResponse<GetPaycheckDto>>>(result);
        var response = Assert.IsType<ApiResponse<GetPaycheckDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(dto.EmployeeId, response.Data!.EmployeeId);
        Assert.Equal(dto.GrossPay, response.Data.GrossPay);
        Assert.Equal(dto.BenefitsCost, response.Data.BenefitsCost);
        Assert.Equal(dto.NetPay, response.Data.NetPay);
        Assert.Equal(dto.PayPeriodStart, response.Data.PayPeriodStart);
        Assert.Equal(dto.PayPeriodEnd, response.Data.PayPeriodEnd);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        int userId = 999;
        DateTime startDate = new DateTime(2024, 1, 1);
        PaycheckPeriodicity periodicity = PaycheckPeriodicity.BiWeekly;
        var exception = new KeyNotFoundException("not found");

        _paycheckServiceMock.Setup(s => s.CalculatePaycheck(userId, startDate, periodicity))
            .ThrowsAsync(exception);

        // Act
        var result = await _controller.Get(userId, startDate, periodicity);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<GetPaycheckDto>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("not found", response.Message);
    }
}