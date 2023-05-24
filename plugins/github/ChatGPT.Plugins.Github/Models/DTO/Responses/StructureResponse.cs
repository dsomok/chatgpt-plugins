namespace ChatGPT.Plugins.Github.Models.DTO.Responses;

public record StructureResponse(IList<GithubFileMetadata> Files) : BasePluginResponse;
