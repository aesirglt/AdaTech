namespace WebApi.Infra.Data.Bases;

using FunctionalConcepts;
using FunctionalConcepts.Choices;
using FunctionalConcepts.Options;
using FunctionalConcepts.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Domain.Bases;
using WebApi.Domain.Interfaces;
using UpdateValidateType = FunctionalConcepts.Choices.Choice<IDictionary<string, string[]>, FunctionalConcepts.Success>;
using InsertValidateType = FunctionalConcepts.Choices.Choice<IDictionary<string, string[]>, Guid>;

public class WriteRepository<TEntity>
    : IWriteRepository<TEntity> where TEntity : Entity<TEntity>, new()
{
    private readonly KanbanContext _context;

    public WriteRepository(KanbanContext context)
    {
        _context = context;
    }
    private Choice<IDictionary<string, string[]>, TRight> WhenEntityNotFound<TRight>()
        => new Dictionary<string, string[]>
        {
                { nameof(TEntity), [$"{nameof(TEntity)} não pode ser nulo"]}
        };

    private Choice<IDictionary<string, string[]>, TRight> ErrorToChoice<TRight>(
        IDictionary<string, string[]> errors)
            => Choice.Of<IDictionary<string, string[]>, TRight>(errors);

    private async ValueTask<Choice<IDictionary<string, string[]>, TRight>> ValidateEntity<TRight>(
        TEntity entity,
        Func<TEntity, Task<Choice<IDictionary<string, string[]>, TRight>>> executeWhenValid)
        => await entity.ContainsErrors()
                  .MatchAsync(ErrorToChoice<TRight>,
                              async () => await executeWhenValid(entity));

    public async Task<InsertValidateType> InsertAsync(Option<TEntity> maybeEntity, CancellationToken cancellationToken)
    {
        var saveOnDb = async (TEntity entity) =>
        {
            var entry = await _context.AddAsync(entity);
            await _context.SaveChangesAsync(cancellationToken);
            entry.State = EntityState.Detached;
            return Choice.Of<IDictionary<string, string[]>, Guid>(entry.Entity.Id);
        };

        return await maybeEntity
            .MatchAsync(async entity => await ValidateEntity(entity, saveOnDb),
                        WhenEntityNotFound<Guid>);
    }

    public async Task<UpdateValidateType> UpdateAsync(Option<TEntity> maybeEntity, CancellationToken cancellationToken)
    {
        var updateOnDb = async (TEntity entity) =>
        {
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
            entry.State = EntityState.Detached;
            return Choice.Of<IDictionary<string, string[]>, Success>(Result.Success);
        };

        return await maybeEntity
            .MatchAsync(async entity => await ValidateEntity(entity, updateOnDb), WhenEntityNotFound<Success>);
    }

    public async Task<Result<Success>> RemoveAsync(TEntity entity, CancellationToken cancellationToken)
    {
        var entry = _context.Set<TEntity>().Entry(entity);
        entry.State = EntityState.Deleted;
        
        await _context.SaveChangesAsync();

        return Result.Success;
    }
}
