namespace ChatGPT.Plugins.Github.Strategies;

public interface IGithubStrategy
{
    bool IsApplicable(string url);

    Task<string> ApplyAsync(string url, CancellationToken cancellationToken);
}
