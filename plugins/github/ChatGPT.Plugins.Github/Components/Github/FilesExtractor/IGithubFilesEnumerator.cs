using ChatGPT.Plugins.Github.Models;
using Octokit;

namespace ChatGPT.Plugins.Github.Components.Github.FilesExtractor;

internal interface IGithubFilesEnumerator
{
    IAsyncEnumerable<RepositoryContent> EnumerateRepositoryFilesAsync(GithubLink githubLink, CancellationToken cancellationToken);
}
