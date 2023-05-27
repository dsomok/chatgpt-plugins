using ChatGPT.Plugins.Github.Models;

namespace ChatGPT.Plugins.Github.Components.Github.FilesExtractor;

internal interface IGithubFilesEnumerator
{
    IAsyncEnumerable<string> EnumerateRepositoryFilesAsync(GithubLink githubLink, CancellationToken cancellationToken);
}
