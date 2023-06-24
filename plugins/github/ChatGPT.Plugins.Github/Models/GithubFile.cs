namespace ChatGPT.Plugins.Github.Models;

public record GithubFile(string Path, string Content, string Error)
    : GithubFileMetadata(Path)
{
    public bool IsValid => string.IsNullOrWhiteSpace(Error) && 
                           !string.IsNullOrWhiteSpace(Content);
}