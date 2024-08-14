namespace WebApi.Domain.Bases;

using FunctionalConcepts.Options;

public abstract record Entity<TEntity>
    where TEntity : Entity<TEntity>, new()
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public abstract Option<IDictionary<string, string[]>> ContainsErrors();
}
