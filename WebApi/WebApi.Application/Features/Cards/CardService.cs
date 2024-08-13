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

    public async Task<Result<IQueryable<Card>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            return await Task.FromResult(Result.Of(_readRepository.GetAll()));
        }
        catch (Exception ex)
        {
            return ServiceUnavailableError.New("Erro inesperado na camada de serviço", ex);
        }
    }

    public async Task<Result<Guid>> InsertAsync(
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

                    return returned.Match<Result<Guid>>(
                        errors => DomainValidatorError.New("Errors de domonio", errors),
                        cardId => cardId);
                },
                () => Result.Of<Guid>(InvalidObjectError.New("Card não pode ser nulo")));
        }
        catch (Exception ex)
        {
            return Result.Of<Guid>(UnhandledError.New(ex.Message, ex));
        }
    }

    public async Task<Result<Success>> RemoveAsync(
        Guid cardId,
        CancellationToken cancellationToken)
    {
        try
        {
            var maybeCard =
                await _readRepository.FindAsync(cardId, cancellationToken);

            return await maybeCard
                .MatchAsync(
                    async card => await _writeRepository.RemoveAsync(card, cancellationToken),
                    () => NotFoundError.New("Não encontrado card com id informado."));
        }
        catch (Exception ex)
        {
            return Result.Of<Success>(UnhandledError.New(ex.Message, ex));
        }
    }

    public async Task<Result<Success>> UpdateAsync(Option<Card> maybeCard, CancellationToken cancellationToken)
    {
        try
        {
            return await maybeCard
                .MatchAsync(async cardToUpdate =>
                {
                    var returned =
                        await _writeRepository.UpdateAsync(cardToUpdate, cancellationToken);

                    return returned.Match<Result<Success>>(
                        errors => DomainValidatorError.New("Errors de domonio", errors),
                        cardId => cardId);
                },
                () => Result.Of<Success>(InvalidObjectError.New("Card não pode ser nulo")));
        }
        catch (Exception ex)
        {
            return Result.Of<Success>(UnhandledError.New(ex.Message, ex));
        }
    }
}
