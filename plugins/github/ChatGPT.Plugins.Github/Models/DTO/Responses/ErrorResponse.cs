namespace ChatGPT.Plugins.Github.Models.DTO.Responses;

public record ErrorResponse : BasePluginResponse
{
    public bool Error { get; } = true;
}
