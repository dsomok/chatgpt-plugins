namespace ChatGPT.Plugins.Github.Models.DTO;

public record QueryResponse(IList<GithubFile> Files);