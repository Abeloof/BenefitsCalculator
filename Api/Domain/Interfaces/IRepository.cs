namespace Api.Domain.Interfaces;

public interface IRepository<TEntitty> where TEntitty : BaseEntity
{
    /// <summary>
    /// Fetches all available records as <typeparam name="TEntitty">BaseEntity</typeparam> from datastore
    /// </summary>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>List of <typeparam name="TEntitty"></typeparam>BaseEntity</returns>
    Task<IList<TEntitty>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Fetchs a record as <typeparam name="TEntitty">BaseEntity</typeparam> from datastore
    /// </summary>
    /// <param name="id">Record Identifier</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>BaseEntity</returns>
    Task<TEntitty?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
