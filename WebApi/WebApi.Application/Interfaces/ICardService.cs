namespace WebApi.Application.Interfaces;

using FunctionalConcepts;
using FunctionalConcepts.Options;
using FunctionalConcepts.Results;
using System.Linq;
using WebApi.Domain.Features.Cards;

public interface ICardService
{
    Task<Result<Guid>> InsertAsync(Option<Card> maybeCard, CancellationToken cancellationToken);
    Task<Result<IQueryable<Card>>> GetAll(CancellationToken cancellationToken);
    Task<Result<Success>> UpdateAsync(Option<Card> maybeCard, CancellationToken cancellationToken);
    Task<Result<Success>> RemoveAsync(Guid cardId, CancellationToken cancellationToken);
}
