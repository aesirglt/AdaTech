namespace WebApi.Distribution.Controllers;

using Microsoft.AspNetCore.Mvc;
using WebApi.Application;
using WebApi.Application.Interfaces;
using WebApi.Domain.Features.Cards;

[Controller]
[Route("[controller]")]
public class CardsController : ControllerBase
{
    private ICardService _cardService;

    public CardsController(ICardService cardService)
    {
        _cardService = cardService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CardDto card, CancellationToken cancellationToken)
    {
        var result = await _cardService.InsertAsync(new Card
        {
            Title = card.Title,
            List = card.List,
            Content = card.Content,
            Id = Guid.NewGuid()
        }, cancellationToken);

        return result.Match<IActionResult>(id => Ok(id), err => StatusCode(err.Code))!;
    }

    [HttpGet]
    public async Task<IActionResult> Read()
    {
        var result = await _cardService.GetAll(default);
        return result.Match<IActionResult>(Ok, err => StatusCode(err.Code))!;
    }

    [HttpPut("{cardId}")]
    public async Task<IActionResult> Update([FromRoute] Guid cardId, [FromBody] CardDto card, CancellationToken cancellationToken)
    {
        var result = await _cardService.UpdateAsync(new Card
        {
            Id = cardId,
            Content = card.Content,
            Title = card.Title,
            List = card.List
        }, cancellationToken);

        return result.Match<IActionResult>(_ => Ok(), err => StatusCode(err.Code))!;
    }

    [HttpDelete("{cardId}")]
    public async Task<IActionResult> Delete([FromRoute] Guid cardId, CancellationToken cancellationToken)
    {
        var result = await _cardService.RemoveAsync(cardId, cancellationToken);
        return result.Match<IActionResult>(_ => Ok(), err => StatusCode(err.Code))!;
    }
}
