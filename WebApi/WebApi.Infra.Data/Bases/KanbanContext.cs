namespace WebApi.Infra.Data.Bases;

using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Features.Cards;
using WebApi.Infra.Data.Features.Cards;

public class KanbanContext : DbContext
{

    public KanbanContext(DbContextOptions<KanbanContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Card> Cards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CardEntityConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}
