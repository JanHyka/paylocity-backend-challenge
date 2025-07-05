using Api.Dtos.Employee;
using Api.Dtos.Dependent;
using Api.Models;
using Api.Services.EmployeesService;
using Api.Services.DependentsService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controller for managing <see cref="Employee"/> entities.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeesService _employeesService;
    private readonly IDependentsService _dependentsService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeesController"/> class.
    /// </summary>
    /// <param name="employeesService">The <see cref="IEmployeesService"/> to use for employee operations.</param>
    /// <param name="dependentsService">The <see cref="IDependentsService"/> to use for dependent operations.</param>
    /// <param name="mapper">The <see cref="IMapper"/> to use for mapping entities to DTOs.</param>
    public EmployeesController(
        IEmployeesService employeesService,
        IDependentsService dependentsService,
        IMapper mapper)
    {
        _employeesService = employeesService;
        _dependentsService = dependentsService;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets an <see cref="GetEmployeeDto"/> by its unique identifier, including dependents.
    /// </summary>
    /// <param name="id">The unique identifier of the <see cref="Employee"/>.</param>
    /// <returns>
    /// An <see cref="ApiResponse{T}"/> containing the <see cref="GetEmployeeDto"/> with the specified ID,
    /// or a not found response if no such employee exists.
    /// </returns>
    [SwaggerOperation(
        Summary = "Get employee by id",
        Description = "Retrieves an employee by its unique identifier, including dependents."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "The employee was found.", typeof(ApiResponse<GetEmployeeDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No employee with the specified ID was found.", typeof(ApiResponse<GetEmployeeDto>))]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
    {
        try
        {
            var employee = await _employeesService.GetEmployeeById(id);
            IEnumerable<Dependent> dependents;
            try
            {
                dependents = await _dependentsService.GetDependentsByEmployeeId(id);
            }
            catch (KeyNotFoundException)
            {
                dependents = Array.Empty<Dependent>();
            }

            var dto = _mapper.Map<GetEmployeeDto>(employee);
            dto.Dependents = _mapper.Map<List<GetDependentDto>>(dependents);

            return new ApiResponse<GetEmployeeDto>
            {
                Data = dto,
                Success = true
            };
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<GetEmployeeDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Gets all <see cref="GetEmployeeDto"/> entities, including dependents.
    /// </summary>
    /// <returns>
    /// An <see cref="ApiResponse{T}"/> containing a list of all <see cref="GetEmployeeDto"/> objects.
    /// </returns>
    [SwaggerOperation(
        Summary = "Get all employees",
        Description = "Retrieves all employees, including their dependents."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "A list of all employees.", typeof(ApiResponse<List<GetEmployeeDto>>))]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll()
    {
        var employees = await _employeesService.GetAllEmployees();
        var result = new List<GetEmployeeDto>();

        foreach (var employee in employees)
        {
            var dto = _mapper.Map<GetEmployeeDto>(employee);

            try
            {
                var dependents = await _dependentsService.GetDependentsByEmployeeId(employee.Id);
                dto.Dependents = _mapper.Map<List<GetDependentDto>>(dependents);
            }
            catch (KeyNotFoundException)
            {
                dto.Dependents = new List<GetDependentDto>();
            }

            result.Add(dto);
        }

        return new ApiResponse<List<GetEmployeeDto>>
        {
            Data = result,
            Success = true
        };
    }
}
