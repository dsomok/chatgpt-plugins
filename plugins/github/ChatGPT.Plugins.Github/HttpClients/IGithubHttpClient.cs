namespace ChatGPT.Plugins.Github.HttpClients;

public interface IGithubHttpClient
{
    Task<string> GetRawContentAsync(string relativeUrl, CancellationToken cancellationToken);
}
