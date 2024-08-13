namespace WebApi.Tests;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Application.Features.Cards;
using WebApi.Domain.Bases;
using WebApi.Domain.Features.Cards;
using WebApi.Infra.Data.Bases;

[TestFixture]
public class CardServiceTests
{
    private CardService _cardService;
    private readonly KanbanContext _kanbanContext;

    public CardServiceTests()
    {
        var options =
            new DbContextOptionsBuilder<KanbanContext>()
            .UseInMemoryDatabase("CardServiceTests")
            .Options;

        _kanbanContext = new(options);
        var writeRepository = new WriteRepository<Card>(_kanbanContext);
        var readRepository = new ReadRepository<Card>(_kanbanContext);

        _cardService = new CardService(writeRepository, readRepository);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _kanbanContext.Cards.RemoveRange(_kanbanContext.Cards.ToList());
        _kanbanContext.SaveChanges();
    }

    [Test]
    public async Task CardServiceTests_GetAll_ShoulBeOk()
    {
        // Arrange
        var title1 = "CardTest1";
        var title2 = "CardTest2";
        var title3 = "CardTest3";

        var cardToInsert = new Card
        {
            Title = title1,
            Content = "Content test",
            List = "list test"
        };
        var cardToInsert2 = new Card
        {
            Title = title2,
            Content = "Content test",
            List = "list test"
        };
        var cardToInsert3 = new Card
        {
            Title = title3,
            Content = "Content test",
            List = "list test"
        };
        _ = _kanbanContext.Cards.Add(cardToInsert);
        _ = _kanbanContext.Cards.Add(cardToInsert2);
        _ = _kanbanContext.Cards.Add(cardToInsert3);
        await _kanbanContext.SaveChangesAsync();

        // Action
        var maybeCard = await _cardService.GetAll(default);

        // Verify
        maybeCard.IsFail.Should().BeFalse();
        maybeCard.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task CardServiceTests_InsertAsync_ShoulBeOk()
    {
        // Arrange
        var title1 = "CardTest1";

        var cardToInsert = new Card
        {
            Title = title1,
            Content = "Content test",
            List = "list test"
        };

        // Action
        var maybeCard = await _cardService.InsertAsync(cardToInsert, default);

        // Verify
        maybeCard.IsFail.Should().BeFalse();
        maybeCard.IsSuccess.Should().BeTrue();
        maybeCard.Then(id => id.Should().NotBe(Guid.Empty));
    }

    [Test]
    public async Task CardServiceTests_UpdateAsync_ShoulBeOk()
    {
        // Arrange
        var title1 = "CardTest1";

        var cardToInsert = new Card
        {
            Title = title1,
            Content = "Content test",
            List = "list test"
        };
        var maybeCard = await _cardService.InsertAsync(cardToInsert, default);
        var cardId = Guid.NewGuid();
        maybeCard.Then(card => cardId = card.Id);

        // Action
        var response = await _cardService.UpdateAsync(cardToInsert with
        {
            Id = cardId,
            Title = "updated",
        }, default);

        // Verify
        response.IsFail.Should().BeFalse();
        response.IsSuccess.Should().BeTrue();
        _kanbanContext.Cards.AsNoTracking().First(x => x.Id == cardId).Title.Should().Be("updated");
    }

    [Test]
    public async Task CardServiceTests_InsertAsync_Title_DomainError_ShoulBeError()
    {
        // Arrange
        var title1 = "";

        var cardToInsert = new Card
        {
            Title = title1,
            Content = "Content test",
            List = "list test"
        };

        // Action
        var maybeCard = await _cardService.InsertAsync(cardToInsert, default);

        // Verify
        maybeCard.IsFail.Should().BeTrue();
        maybeCard.IsSuccess.Should().BeFalse();
        maybeCard.Else(errs =>
        {
            var errors =
                errs.Should()
                .BeOfType<DomainValidatorError>()
                .Subject
                .Errors;

            errors[nameof(Card.Title)].Length.Should().BeGreaterThan(0);
            errors.FirstOrDefault(x => x.Key != nameof(Card.Title)).Should().Be(default);
        });
    }

    [Test]
    public async Task CardServiceTests_RemoveAsync_ShoulBeOk()
    {
        // Arrange
        var cardToInsert = new Card
        {
            Title = "title1",
            Content = "Content test",
            List = "list test"
        };
        var maybeCard = await _cardService.InsertAsync(cardToInsert, default);
        Guid cardId = default;
        maybeCard.Then(card => cardId = card.Id);

        // Action
        var response = await _cardService.RemoveAsync(cardId, default);

        // Verify
        response.IsFail.Should().BeFalse();
        response.IsSuccess.Should().BeTrue();
    }
}
