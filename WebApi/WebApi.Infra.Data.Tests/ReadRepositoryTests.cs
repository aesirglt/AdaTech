namespace WebApi.Tests;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Features.Cards;
using WebApi.Infra.Data.Bases;

public class ReadRepositoryTests
{
    private readonly KanbanContext _kanbanContext;
    private readonly ReadRepository<Card> _readRepository;

    public ReadRepositoryTests()
    {
        var options =
            new DbContextOptionsBuilder<KanbanContext>()
            .UseInMemoryDatabase("ReadRepositoryTests")
            .Options;

        _kanbanContext = new(options);
        _readRepository = new(_kanbanContext);
    }
    [TearDown]
    public void TearDown()
    {
        _kanbanContext.Cards.RemoveRange(_kanbanContext.Cards.ToList());
        _kanbanContext.SaveChanges();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _kanbanContext.Dispose();
    }

    [Test]
    public async Task ReadRepositoryTests_FindAsync_ShouldBeOk()
    {
        // Arrange
        var cardToInsert = new Card
        {
            Title = "CardTest",
            Content = "Content test",
            List = "list test"
        };
        var entity = _kanbanContext.Cards.Add(cardToInsert);
        _kanbanContext.SaveChanges();
        var id = entity.Entity.Id;
        entity.State = EntityState.Detached;

        // Action
        var maybeCard = await _readRepository.FindAsync(id, default);

        // Verify
        maybeCard.IsNone.Should().BeFalse();
        maybeCard.IsSome.Should().BeTrue();
        maybeCard.Then(card =>
        {
            card.Title.Should().Be(cardToInsert.Title);
            card.Content.Should().Be(cardToInsert.Content);
            card.List.Should().Be(cardToInsert.List);
        });
    }

    [Test]
    public async Task ReadRepositoryTests_FindAsync_NotFound_ShouldBeNone()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Action
        var maybeCard = await _readRepository.FindAsync(id, default);

        // Verify
        maybeCard.IsNone.Should().BeTrue();
        maybeCard.IsSome.Should().BeFalse();
    }

    [Test]
    public async Task ReadRepositoryTests_GetAllAsync_ShouldBeOk()
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
        var insertedCount = 3;
        await _kanbanContext.SaveChangesAsync();

        // Action
        var maybeCard = _readRepository.GetAll().ToList();

        // Verify
        maybeCard.Count.Should().Be(expected: insertedCount);
        maybeCard.Any(x => x.Title.Equals(title1)).Should().BeTrue();
        maybeCard.Any(x => x.Title.Equals(title2)).Should().BeTrue();
        maybeCard.Any(x => x.Title.Equals(title3)).Should().BeTrue();
    }

    [Test]
    public void ReadRepositoryTests_GetAllAsync_EmptyList_ShouldBeOk()
    {
        // Action
        var maybeCard = _readRepository.GetAll().ToList();

        // Verify
        maybeCard.Count.Should().Be(expected: 0);
    }
}