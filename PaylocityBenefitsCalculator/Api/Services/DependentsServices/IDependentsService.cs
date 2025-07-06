using Api.Models;

namespace Api.Services.DependentsServices;

/// <summary>
/// Defines methods for managing <see cref="Dependent"/> entities.
/// </summary>
public interface IDependentsService
{
    /// <summary>
    /// Retrieves all <see cref="Dependent"/> entities.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the asynchronous operation.
    /// The task result contains a collection of <see cref="Dependent"/> objects.
    /// </returns>
    Task<IEnumerable<Dependent>> GetAllDependents();

    /// <summary>
    /// Retrieves a <see cref="Dependent"/> by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the <see cref="Dependent"/>.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the asynchronous operation.
    /// The task result contains the <see cref="Dependent"/>.
    /// </returns>
    Task<Dependent> GetDependentById(int id);

    /// <summary>
    /// Retrieves all <see cref="Dependent"/> entities for a specific <see cref="Employee"/>.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the <see cref="Employee"/>.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the asynchronous operation.
    /// The task result contains a collection of <see cref="Dependent"/> objects.
    /// </returns>
    Task<IEnumerable<Dependent>> GetDependentsByEmployeeId(int employeeId);
}