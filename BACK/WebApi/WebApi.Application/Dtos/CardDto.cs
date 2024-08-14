namespace WebApi.Application;

public record CardDto
{
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string List { get; init; } = string.Empty;
}
