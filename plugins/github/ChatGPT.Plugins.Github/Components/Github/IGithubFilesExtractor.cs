using Octokit;

namespace ChatGPT.Plugins.Github.Components.Github;

internal interface IGithubFilesExtractor
{
    IAsyncEnumerable<RepositoryContent> GetRepositoryFilesAsync(string owner, string name, CancellationToken cancellationToken);
}
