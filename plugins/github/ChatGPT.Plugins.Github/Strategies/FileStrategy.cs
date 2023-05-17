using ChatGPT.Plugins.Github.HttpClients;

namespace ChatGPT.Plugins.Github.Strategies;

internal class FileStrategy : IGithubStrategy
{
    private readonly IGithubHttpClient _githubHttpClient;

    public FileStrategy(IGithubHttpClient githubHttpClient)
    {
        _githubHttpClient = githubHttpClient;
    }

    public bool IsApplicable(string url)
    {
        return new Uri(url).Segments.Last().Contains('.');
    }

    public Task<string> ApplyAsync(string url, CancellationToken cancellationToken)
    {
        var relativeUri = new Uri(url).PathAndQuery;
        return _githubHttpClient.GetRawContentAsync(relativeUri, cancellationToken);
    }
}