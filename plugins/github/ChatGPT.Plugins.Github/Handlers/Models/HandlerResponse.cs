namespace ChatGPT.Plugins.Github.Handlers.Models;

public record HandlerResponse<TResponse>(TResponse Response, string Error, string AssistantHint)
{
    public bool IsSuccess => string.IsNullOrWhiteSpace(Error);
};
