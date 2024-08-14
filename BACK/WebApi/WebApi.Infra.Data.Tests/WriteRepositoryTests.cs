namespace WebApi.Tests;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Features.Cards;
using WebApi.Infra.Data.Bases;

public class WriteRepositoryTests
{
    private readonly KanbanContext _kanbanContext;
    private readonly WriteRepository<Card> _writeRepository;

    public WriteRepositoryTests()
    {
        var options =
            new DbContextOptionsBuilder<KanbanContext>()
            .UseInMemoryDatabase("WriteRepositoryTests")
            .Options;

        _kanbanContext = new(options);
        _writeRepository = new(_kanbanContext);
    }

    [TearDown]
    public void TearDown()
    {
        _kanbanContext.Cards.RemoveRange(_kanbanContext.Cards.AsNoTracking().ToList());
        _kanbanContext.SaveChanges();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _kanbanContext.Dispose();
    }

    [Test]
    public async Task WriteRepositoryTests_InsertAsync_ShouldBeOk()
    {
        // Arrange
        var cardToInsert = new Card
        {
            Title = "CardTest",
            Content = "Content test",
            List = "list test"
        };

        // Action
        var maybeCard = await _writeRepository.InsertAsync(cardToInsert, default);

        // Verify
        maybeCard.IsLeft.Should().BeFalse();
        maybeCard.IsRight.Should().BeTrue();
        var allCards = _kanbanContext.Cards.AsNoTracking().ToList();
        allCards.Count.Should().Be(expected: 1);
        maybeCard.ThenRight(id =>
        {
            id.Should().NotBe(Guid.Empty);
            allCards.FirstOrDefault()!.Id.Should().Be(id);
        });
    }

    [Test]
    public async Task WriteRepositoryTests_InsertAsync_TitleEmpty_ShouldBeError()
    {
        // Arrange
        var cardToInsert = new Card
        {
            Title = "",
            Content = "Content test",
            List = "list test"
        };

        // Action
        var maybeCard = await _writeRepository.InsertAsync(cardToInsert, default);

        // Verify
        maybeCard.IsLeft.Should().BeTrue();
        maybeCard.IsRight.Should().BeFalse();
        _kanbanContext.Cards.ToList().Should().BeEmpty();

        maybeCard.ThenLeft(errs =>
        {
            errs[nameof(Card.Title)].Length.Should().Be(expected: 1);
            errs[nameof(Card.Title)][0].Should().Be("Title cant be empty.");
        });
    }

    [Test]
    public async Task WriteRepositoryTests_InsertAsync_ContentEmpty_ShouldBeError()
    {
        // Arrange
        var cardToInsert = new Card
        {
            Title = "Title",
            Content = "",
            List = "list test"
        };

        // Action
        var maybeCard = await _writeRepository.InsertAsync(cardToInsert, default);

        // Verify
        maybeCard.IsLeft.Should().BeTrue();
        maybeCard.IsRight.Should().BeFalse();
        _kanbanContext.Cards.ToList().Should().BeEmpty();

        maybeCard.ThenLeft(errs =>
        {
            errs[nameof(Card.Content)].Length.Should().Be(expected: 1);
            errs[nameof(Card.Content)][0].Should().Be("Content cant be empty.");
        });
    }

    [Test]
    public async Task WriteRepositoryTests_InsertAsync_ListEmpty_ShouldBeError()
    {
        // Arrange
        var cardToInsert = new Card
        {
            Title = "Title",
            Content = "Content",
            List = ""
        };

        // Action
        var maybeCard = await _writeRepository.InsertAsync(cardToInsert, default);

        // Verify
        maybeCard.IsLeft.Should().BeTrue();
        maybeCard.IsRight.Should().BeFalse();
        _kanbanContext.Cards.ToList().Should().BeEmpty();

        maybeCard.ThenLeft(errs =>
        {
            errs[nameof(Card.List)].Length.Should().Be(expected: 1);
            errs[nameof(Card.List)][0].Should().Be("List cant be empty.");
        });
    }

    [Test]
    public async Task WriteRepositoryTests_UpdateAsync_ShouldBeOk()
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
        entity.State = EntityState.Detached;
        var id = entity.Entity.Id;

        var cardOnDb = await _kanbanContext.Cards.FindAsync(id);
        _kanbanContext.Entry(cardOnDb!).State = EntityState.Detached;
        var newTitle = "Updated Title";
        var cardToUpdate = cardOnDb! with
        {
            Title = newTitle
        };
        
        // Action
        var maybeCard = await _writeRepository.UpdateAsync(cardToUpdate, default);

        // Verify
        maybeCard.IsLeft.Should().BeFalse();
        maybeCard.IsRight.Should().BeTrue();
        var allCards = _kanbanContext.Cards.AsNoTracking().ToList();
        allCards.Count.Should().Be(expected: 1);
        maybeCard.ThenRight(_ =>
        {
            id.Should().NotBe(Guid.Empty);
            allCards.FirstOrDefault()!.Title.Should().Be(newTitle);
        });
    }

    [Test]
    public async Task WriteRepositoryTests_UpdateAsync_TitleEmpty_ShouldBeError()
    {
        // Arrange
        var title = "CardTest";
        var cardToInsert = new Card
        {
            Title = title,
            Content = "Content test",
            List = "list test"
        };
        var entity = _kanbanContext.Cards.Add(cardToInsert);
        _kanbanContext.SaveChanges();
        entity.State = EntityState.Detached;
        var id = entity.Entity.Id;

        var cardOnDb = await _kanbanContext.Cards.FindAsync(id);
        _kanbanContext.Entry(cardOnDb!).State = EntityState.Detached;
        var cardToUpdate = cardOnDb! with
        {
            Title = ""
        };

        // Action
        var maybeCard = await _writeRepository.UpdateAsync(cardToUpdate, default);

        // Verify
        maybeCard.IsLeft.Should().BeTrue();
        maybeCard.IsRight.Should().BeFalse();
        var list = _kanbanContext.Cards.AsNoTracking().ToList();
        list.Should().NotBeNullOrEmpty();
        list.First().Title.Should().Be(title);
        maybeCard.ThenLeft(errs =>
        {
            errs[nameof(Card.Title)].Length.Should().Be(expected: 1);
            errs[nameof(Card.Title)][0].Should().Be("Title cant be empty.");
        });
    }

    [Test]
    public async Task WriteRepositoryTests_UpdateAsync_ContentEmpty_ShouldBeError()
    {
        // Arrange
        var content = "Content test";
        var cardToInsert = new Card
        {
            Title = "CardTest",
            Content = content,
            List = "list test"
        };
        var entity = _kanbanContext.Cards.Add(cardToInsert);
        _kanbanContext.SaveChanges();
        entity.State = EntityState.Detached;
        var id = entity.Entity.Id;

        var cardOnDb = await _kanbanContext.Cards.FindAsync(id);
        _kanbanContext.Entry(cardOnDb!).State = EntityState.Detached;
        var cardToUpdate = cardOnDb! with
        {
            Title = ""
        };

        // Action
        var maybeCard = await _writeRepository.UpdateAsync(cardToUpdate, default);

        // Verify
        maybeCard.IsLeft.Should().BeTrue();
        maybeCard.IsRight.Should().BeFalse();
        var list = _kanbanContext.Cards.AsNoTracking().ToList();
        list.Should().NotBeNullOrEmpty();
        list.First().Content.Should().Be(content);
        maybeCard.ThenLeft(errs =>
        {
            errs[nameof(Card.Content)].Length.Should().Be(expected: 1);
            errs[nameof(Card.Content)][0].Should().Be("Content cant be empty.");
        });
    }

    [Test]
    public async Task WriteRepositoryTests_UpdateAsync_ListEmpty_ShouldBeError()
    {
        // Arrange
        var listText = "list test";
        var cardToInsert = new Card
        {
            Title = "CardTest",
            Content = "Content test",
            List = listText
        };
        var entity = _kanbanContext.Cards.Add(cardToInsert);
        _kanbanContext.SaveChanges();
        entity.State = EntityState.Detached;
        var id = entity.Entity.Id;

        var cardOnDb = await _kanbanContext.Cards.FindAsync(id);
        _kanbanContext.Entry(cardOnDb!).State = EntityState.Detached;
        var cardToUpdate = cardOnDb! with
        {
            Title = ""
        };

        // Action
        var maybeCard = await _writeRepository.UpdateAsync(cardToUpdate, default);

        // Verify
        maybeCard.IsLeft.Should().BeTrue();
        maybeCard.IsRight.Should().BeFalse();
        var list = _kanbanContext.Cards.AsNoTracking().ToList();
        list.Should().NotBeNullOrEmpty();
        list.First().List.Should().Be(listText);
        maybeCard.ThenLeft(errs =>
        {
            errs[nameof(Card.List)].Length.Should().Be(expected: 1);
            errs[nameof(Card.List)][0].Should().Be("List cant be empty.");
        });
    }


    [Test]
    public async Task WriteRepositoryTests_RemoveAsync_ShouldBeOk()
    {
        // Arrange
        var cardToInsert = new Card
        {
            Title = "Title",
            Content = "Content test",
            List = "list test"
        };

        _ = await _writeRepository.InsertAsync(cardToInsert, default);

        // Action
        var response = await _writeRepository.RemoveAsync(cardToInsert, default);

        // Verify
        response.IsFail.Should().BeFalse();
        response.IsSuccess.Should().BeTrue();
        _kanbanContext.Cards.AsNoTracking().ToList().Should().BeEmpty();
    }
}