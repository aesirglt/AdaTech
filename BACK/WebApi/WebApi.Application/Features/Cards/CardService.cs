namespace WebApi.Application.Features.Cards;

using FunctionalConcepts;
using FunctionalConcepts.Errors;
using FunctionalConcepts.Options;
using FunctionalConcepts.Results;
using System;
using System.Linq;
using WebApi.Application.Interfaces;
using WebApi.Domain.Bases;
using WebApi.Domain.Features.Cards;
using WebApi.Domain.Interfaces;

public class CardService : ICardService
{
    private IWriteRepository<Card> _writeRepository;
    private IReadRepository<Card> _readRepository;

    public CardService(
        IWriteRepository<Card> writeRepository,
        IReadRepository<Card> readRepository)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
    }

    public async Task<Result<List<Card>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            return await _readRepository.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            return ServiceUnavailableError.New("Erro inesperado na camada de serviço", ex);
        }
    }

    public async Task<Result<Card>> InsertAsync(
        Option<Card> maybeCard,
        CancellationToken cancellationToken)
    {
        try
        {
            return await maybeCard
                .MatchAsync(async cardToAdd =>
                {
                    var returned =
                        await _writeRepository.InsertAsync(maybeCard, cancellationToken);

                    return returned.Match<Result<Card>>(
                        errors => DomainValidatorError.New("Errors de domonio", errors),
                        cardId => cardToAdd with { Id = cardId });
                },
                () => Result.Of<Card>(InvalidObjectError.New("Card não pode ser nulo")));
        }
        catch (Exception ex)
        {
            return Result.Of<Card>(UnhandledError.New(ex.Message, ex));
        }
    }

    public async Task<Result<List<Card>>> RemoveAsync(
        Guid cardId,
        CancellationToken token)
    {
        try
        {
            var maybeCard =
                await _readRepository.FindAsync(cardId, token);

            return await maybeCard
                .MatchAsync(
                    async card =>
                    {
                        var maybeRemoved = await _writeRepository.RemoveAsync(card, token);
                        return await maybeRemoved.MapAsync(async _ => await _readRepository.GetAll(token));
                    },
                    () => NotFoundError.New("Não encontrado card com id informado."));
        }
        catch (Exception ex)
        {
            return UnhandledError.New(ex.Message, ex);
        }
    }

    public async Task<Result<Card>> UpdateAsync(Option<Card> maybeCard, CancellationToken cancellationToken)
    {
        try
        {
            return await maybeCard
                .MatchAsync(async cardToUpdate =>
                {
                    var cardOnDb = await _readRepository.FindAsync(cardToUpdate.Id, cancellationToken);

                    if (cardOnDb.IsNone)
                        return NotFoundError.New("Não foi encontrado card com id informado.");

                    var returned =
                        await _writeRepository.UpdateAsync(cardToUpdate, cancellationToken);

                    return returned.Match<Result<Card>>(
                        errors => DomainValidatorError.New("Errors de domonio", errors),
                        _ => cardToUpdate);
                },
                () => Result.Of<Card>(InvalidObjectError.New("Card não pode ser nulo")));
        }
        catch (Exception ex)
        {
            return Result.Of<Card>(UnhandledError.New(ex.Message, ex));
        }
    }
}
