namespace ChatGPT.Plugins.Github.Models;

public record GithubFileMetadata(string Path)
{
    public bool IsRootFile => !Path.Contains('/');
};
