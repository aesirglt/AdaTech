namespace WebApi.Infra.Data.Bases;

using FunctionalConcepts.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebApi.Domain.Bases;
using WebApi.Domain.Interfaces;

public class ReadRepository<TEntity> : IReadRepository<TEntity> where TEntity : Entity<TEntity>, new()
{
    private readonly KanbanContext _context;

    public ReadRepository(KanbanContext context)
    {
        _context = context;
    }

    public async Task<Option<TEntity>> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Set<TEntity>().FindAsync([id], cancellationToken: cancellationToken);

        if (entity is null)
            return NoneType.Value;

        _context.Entry(entity).State = EntityState.Detached;

        return entity;
    }

    public async Task<List<TEntity>> GetAll(CancellationToken cancellation)
    {
        return await _context.Set<TEntity>().AsNoTracking().ToListAsync(cancellation);
    }
}
