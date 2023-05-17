using ChatGPT.Plugins.Github.HttpClients;

namespace ChatGPT.Plugins.Github.Strategies;

internal class RepoStrategy : IGithubStrategy
{
    private readonly IGithubHttpClient _githubHttpClient;

    public RepoStrategy(IGithubHttpClient githubHttpClient)
    {
        _githubHttpClient = githubHttpClient;
    }

    public bool IsApplicable(string url)
    {
        return !new Uri(url).Segments.Last().Contains('.');
    }

    public Task<string> ApplyAsync(string url, CancellationToken cancellationToken)
    {
        var relativeUri = $"{new Uri(url).PathAndQuery}/HEAD/README.md";
        return _githubHttpClient.GetRawContentAsync(relativeUri, cancellationToken);
    }
}
