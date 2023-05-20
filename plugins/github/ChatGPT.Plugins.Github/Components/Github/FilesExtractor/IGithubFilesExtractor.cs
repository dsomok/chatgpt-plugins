using ChatGPT.Plugins.Github.Models;
using Octokit;

namespace ChatGPT.Plugins.Github.Components.Github.FilesExtractor;

internal interface IGithubFilesExtractor
{
    IAsyncEnumerable<RepositoryContent> GetRepositoryFilesAsync(GithubLink githubLink, CancellationToken cancellationToken);
}
