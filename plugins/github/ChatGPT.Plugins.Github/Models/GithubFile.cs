namespace ChatGPT.Plugins.Github.Models;

public record GithubFile(string Name, string Path, string Content) 
    : GithubFileMetadata(Name, Path);