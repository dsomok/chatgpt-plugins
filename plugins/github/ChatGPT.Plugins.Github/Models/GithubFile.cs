namespace ChatGPT.Plugins.Github.Models;

public record GithubFile(string Path, string Content, string Error) 
    : GithubFileMetadata(Path);