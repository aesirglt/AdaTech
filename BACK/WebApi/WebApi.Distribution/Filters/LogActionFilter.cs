namespace WebApi.Distribution.Filters;

using Microsoft.AspNetCore.Mvc.Filters;

public class LogActionFilter : IActionFilter
{
    private readonly ILogger<LogActionFilter> _logger;

    public LogActionFilter(ILogger<LogActionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Request.Method == HttpMethod.Put.Method ||
            context.HttpContext.Request.Method == HttpMethod.Delete.Method)
        {
            var id = context.ActionArguments["id"];
            string titulo = "ação de remoção";

            if(context.ActionArguments["card"] is WebApi.Application.CardDto card)
            {
                titulo = card.Title;
            }

            _logger.LogDebug($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - Card {id} - {titulo} - {context.HttpContext.Request.Method}");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
