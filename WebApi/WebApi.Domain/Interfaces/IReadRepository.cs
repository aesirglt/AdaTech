namespace WebApi.Domain.Interfaces;

using FunctionalConcepts.Options;
using WebApi.Domain.Bases;

public interface IReadRepository<TEntity> where TEntity : Entity<TEntity>, new()
{
    Task<Option<TEntity>> FindAsync(Guid id, CancellationToken cancellationToken);
    IQueryable<TEntity> GetAll();
}
