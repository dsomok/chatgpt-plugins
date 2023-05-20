using Octokit;

namespace ChatGPT.Plugins.Github.Services.Github;

internal interface IGithubService
{
    IAsyncEnumerable<RepositoryContent> GetRepositoryFilesAsync(string owner, string name, CancellationToken cancellationToken);
}
