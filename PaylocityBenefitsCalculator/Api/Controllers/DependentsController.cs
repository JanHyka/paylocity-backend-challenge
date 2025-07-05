using Api.Dtos.Dependent;
using Api.Models;
using Api.Services.DependentsServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controller for managing <see cref="Dependent"/> entities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DependentsController"/> class.
/// </remarks>
/// <param name="dependentsService">The <see cref="IDependentsService"/> to use for dependent operations.</param>
/// <param name="mapper">The <see cref="IMapper"/> to use for mapping entities to DTOs.</param>
[ApiController]
[Route("api/v1/[controller]")]
public class DependentsController(IDependentsService dependentsService, IMapper mapper) : ControllerBase
{
    private readonly IDependentsService _dependentsService = dependentsService;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Gets a <see cref="GetDependentDto"/> by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the <see cref="Dependent"/>.</param>
    /// <returns>
    /// An <see cref="ApiResponse{T}"/> containing the <see cref="GetDependentDto"/> with the specified ID,
    /// or a not found response if no such dependent exists.
    /// </returns>
    [SwaggerOperation(
        Summary = "Get dependent by id",
        Description = "Retrieves a dependent by its unique identifier."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "The dependent was found.", typeof(ApiResponse<GetDependentDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No dependent with the specified ID was found.", typeof(ApiResponse<GetDependentDto>))]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetDependentDto>>> Get(int id)
    {
        try
        {
            var dependent = await _dependentsService.GetDependentById(id);
            var dto = _mapper.Map<GetDependentDto>(dependent);

            return new ApiResponse<GetDependentDto>
            {
                Data = dto,
                Success = true
            };
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<GetDependentDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Gets all <see cref="GetDependentDto"/> entities.
    /// </summary>
    /// <returns>
    /// An <see cref="ApiResponse{T}"/> containing a list of all <see cref="GetDependentDto"/> objects.
    /// </returns>
    [SwaggerOperation(
        Summary = "Get all dependents",
        Description = "Retrieves all dependents."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "A list of all dependents.", typeof(ApiResponse<List<GetDependentDto>>))]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> GetAll()
    {
        var dependents = await _dependentsService.GetAllDependents();
        var dtos = _mapper.Map<List<GetDependentDto>>(dependents);

        return new ApiResponse<List<GetDependentDto>>
        {
            Data = dtos,
            Success = true
        };
    }
}
