namespace WebApi.Domain.Interfaces;

using FunctionalConcepts;
using FunctionalConcepts.Choices;
using FunctionalConcepts.Options;
using FunctionalConcepts.Results;
using System.Threading.Tasks;
using WebApi.Domain.Bases;

public interface IWriteRepository<TEntity> where TEntity : Entity<TEntity>, new()
{
    Task<Result<Success>> RemoveAsync(TEntity maybeEntity, CancellationToken cancellationToken);
    Task<Choice<IDictionary<string, string[]>, Guid>> InsertAsync(Option<TEntity> maybeEntity, CancellationToken cancellationToken);
    Task<Choice<IDictionary<string, string[]>, Success>> UpdateAsync(Option<TEntity> maybeEntity, CancellationToken cancellationToken);
}
