namespace ChatGPT.Plugins.Github.Models.DTO.Responses;

public record StructureResponse(IList<string> Files) : BasePluginResponse;
