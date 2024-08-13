namespace WebApi.Domain.Features.Cards;

using FunctionalConcepts.Options;
using System.Collections.Generic;
using WebApi.Domain.Bases;

public record Card : Entity<Card>
{
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string List { get; init; } = string.Empty;

    public override Option<IDictionary<string, string[]>> ContainsErrors()
    {
        var erros = new Dictionary<string, List<string>>()
        {
            { nameof(Title), [] },
            { nameof(Content), [] },
            { nameof(List), [] }
        };

        if (Title is null)
            erros[nameof(Title)].Add("Title cant be null.");
        if (Title is "")
            erros[nameof(Title)].Add("Title cant be empty.");

        if (Content is null or "")
            erros[nameof(Content)].Add("Content cant be null or empty");

        if (List is null or "")
            erros[nameof(List)].Add("Content cant be null or empty");

        return erros.Any(x => x.Value.Count > 0)
            ? erros.Where(x => x.Value.Count > 0)
                   .ToDictionary(x => x.Key, x => x.Value.ToArray())
            : NoneType.Value;
    }
}
