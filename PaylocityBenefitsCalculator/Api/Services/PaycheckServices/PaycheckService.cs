using Api.Dtos.Paycheck;
using Api.Models;
using Api.Services.DependentsServices;
using Api.Services.EmployeesServices;
using Api.Services.PaycheckServices.Calculator.Models;
using Api.Services.PaycheckServices.Validator;

namespace Api.Services.PaycheckServices;

/// <summary>
/// Provides methods for calculating <see cref="Paycheck"/>s for employees, including dependents and validation.
/// </summary>
public class PaycheckService(
    IEmployeesService employeesService,
    IDependentsService dependentsService,
    ICalculatorModel calculatorModel,
    IEmployeeValidator employeeValidator) : IPaycheckService
{
    private readonly IEmployeesService _employeesService = employeesService;
    private readonly IDependentsService _dependentsService = dependentsService;
    private readonly ICalculatorModel _calculatorModel = calculatorModel;
    private readonly IEmployeeValidator _employeeValidator = employeeValidator;

    /// <summary>
    /// Calculates a <see cref="Paycheck"/> for the specified employee and pay period.
    /// Retrieves the employee and their dependents, validates the employee, and delegates calculation to the configured model.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="startDate">The start date of the pay period.</param>
    /// <param name="periodicity">The <see cref="PaycheckPeriodicity"/> for the pay period.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains the calculated <see cref="Paycheck"/>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if the employee data is invalid for paycheck calculation.</exception>
    public async Task<Paycheck> CalculatePaycheck(int employeeId, DateTime startDate, PaycheckPeriodicity periodicity)
    {
        Employee employee = await _employeesService.GetEmployeeById(employeeId);

        try
        {
            var dependents = await _dependentsService.GetDependentsByEmployeeId(employeeId);
            employee.Dependents = [.. dependents];
        }
        catch (KeyNotFoundException)
        {
            // noop, valid scenario where an employee has no dependents
        }

        if (!_employeeValidator.IsValid(employee))
        {
            throw new ArgumentException("Invalid employee data.");
        }

        return _calculatorModel.CalculatePaycheck(employee, startDate, startDate.GetBiWeekEnd());
    }
}
