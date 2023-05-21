namespace ChatGPT.Plugins.Github.Components.Files;

public interface IFileContentProcessor
{
    Task<string> ProcessFileContentAsync(string content, CancellationToken cancellationToken);
}