using Api.Dtos.Paycheck;
using Api.Models;
using Api.Services.PaycheckServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controller for calculating and retrieving <see cref="GetPaycheckDto"/> information.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PaychecksController"/> class.
/// </remarks>
/// <param name="paychecksService">The <see cref="IPaycheckService"/> to use for paycheck calculations.</param>
/// <param name="mapper">The <see cref="IMapper"/> to use for mapping entities to DTOs.</param>
[ApiController]
[Route("api/v1/[controller]")]
public class PaychecksController(IPaycheckService paychecksService, IMapper mapper) : Controller
{
    private readonly IPaycheckService _paychecksService = paychecksService;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Calculates and retrieves a <see cref="GetPaycheckDto"/> for a specific employee and pay period.
    /// </summary>
    /// <param name="userId">The unique identifier of the employee.</param>
    /// <param name="startDate">The start date of the pay period.</param>
    /// <param name="periodicity">The <see cref="PaycheckPeriodicity"/> for the pay period.</param>
    /// <returns>
    /// An <see cref="ApiResponse{T}"/> containing the calculated <see cref="GetPaycheckDto"/>,
    /// or a not found response if the employee does not exist.
    /// </returns>
    [SwaggerOperation(
        Summary = "Get paycheck for employee and pay period",
        Description = "Calculates and retrieves a paycheck for the specified employee and pay period."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "The paycheck was calculated successfully.", typeof(ApiResponse<GetPaycheckDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No employee with the specified ID was found.", typeof(ApiResponse<GetPaycheckDto>))]
    [HttpGet("user/{userId:int}/from/{startDate:datetime}/periodicity/{periodicity}")]
    public async Task<ActionResult<ApiResponse<GetPaycheckDto>>> Get(int userId, DateTime startDate, PaycheckPeriodicity periodicity)
    {
        try
        {
            var paycheck = await _paychecksService.CalculatePaycheck(userId, startDate, periodicity);
            var dto = _mapper.Map<GetPaycheckDto>(paycheck);
            return new ApiResponse<GetPaycheckDto>
            {
                Data = dto,
                Success = true
            };
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<GetPaycheckDto>
            {
                Message = ex.Message,
                Success = false
            });
        }
        catch (ArgumentException ex)
        {
            
            return UnprocessableEntity(new ApiResponse<GetPaycheckDto>
            {
                Message = ex.Message,
                Success = false
            });
        }
    }
}
