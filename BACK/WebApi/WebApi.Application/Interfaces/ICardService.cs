namespace WebApi.Application.Interfaces;

using FunctionalConcepts.Options;
using FunctionalConcepts.Results;
using System.Linq;
using WebApi.Domain.Features.Cards;

public interface ICardService
{
    Task<Result<Card>> InsertAsync(Option<Card> maybeCard, CancellationToken cancellationToken);
    Task<Result<List<Card>>> GetAll(CancellationToken cancellationToken);
    Task<Result<Card>> UpdateAsync(Option<Card> maybeCard, CancellationToken cancellationToken);
    Task<Result<List<Card>>> RemoveAsync(Guid cardId, CancellationToken cancellationToken);
}
