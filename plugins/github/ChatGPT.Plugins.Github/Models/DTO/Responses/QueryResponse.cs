namespace ChatGPT.Plugins.Github.Models.DTO.Responses;

public record QueryResponse(IList<GithubFile> Files);