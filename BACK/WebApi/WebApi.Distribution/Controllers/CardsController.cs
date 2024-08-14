namespace WebApi.Distribution.Controllers;

using Microsoft.AspNetCore.Authorization;
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

        return result.Match<IActionResult>(
            createdCard => base.StatusCode(201, createdCard),
            err => StatusCode(err.Code, err.Message))!;
    }

    [HttpGet]
    public async Task<IActionResult> Read(CancellationToken cancellationToken)
    {
        var result = await _cardService.GetAll(cancellationToken);
        return result.Match<IActionResult>(Ok, err => StatusCode(err.Code, err.Message))!;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CardDto card, CancellationToken cancellationToken)
    {
        var result = await _cardService.UpdateAsync(new Card
        {
            Id = id,
            Content = card.Content,
            Title = card.Title,
            List = card.List
        }, cancellationToken);

        return result.Match<IActionResult>(Ok, err => StatusCode(err.Code, err.Message))!;
    }

    [HttpDelete("{cardId}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _cardService.RemoveAsync(id, cancellationToken);
        return result.Match<IActionResult>(Ok, err => StatusCode(err.Code, err.Message))!;
    }
}
