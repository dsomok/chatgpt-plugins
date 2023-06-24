namespace ChatGPT.Plugins.Github.Components.Files;

public interface IFileContentProcessor
{
    bool CanProcessFile(string fullPath);

    Task<string> ProcessFileContentAsync(string content, CancellationToken cancellationToken);
}