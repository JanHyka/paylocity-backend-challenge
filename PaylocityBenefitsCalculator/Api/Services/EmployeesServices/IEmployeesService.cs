using Api.Models;

namespace Api.Services.EmployeesServices;

/// <summary>
/// Defines methods for managing <see cref="Employee"/> entities.
/// </summary>
public interface IEmployeesService
{
    /// <summary>
    /// Retrieves all <see cref="Employee"/> entities.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the asynchronous operation. 
    /// The task result contains a collection of <see cref="Employee"/> objects.
    /// </returns>
    Task<IEnumerable<Employee>> GetAllEmployees();

    /// <summary>
    /// Retrieves an <see cref="Employee"/> by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the <see cref="Employee"/>.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the asynchronous operation. 
    /// The task result contains the <see cref="Employee"/>.
    /// </returns>
    /// <exception cref="KeyNotFoundException">Thrown when the <see cref="Employee"/> could not be found.</exception>
    Task<Employee> GetEmployeeById(int id);
}