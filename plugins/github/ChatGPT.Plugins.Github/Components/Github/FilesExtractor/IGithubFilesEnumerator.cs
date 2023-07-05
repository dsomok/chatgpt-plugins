using ChatGPT.Plugins.Github.Models;

namespace ChatGPT.Plugins.Github.Components.Github.FilesExtractor;

internal interface IGithubFilesEnumerator
{
    IAsyncEnumerable<string> EnumerateRepositoryFilesAsync(
        GithubLink githubLink, CancellationToken cancellationToken
    );

    IAsyncEnumerable<string> EnumerateRepositoryFilesAsync(
        GithubLink githubLink,
        IList<string> relativePaths,
        IList<string> fileExtensions,
        CancellationToken cancellationToken
    );

    IAsyncEnumerable<string> EnumerateRepositoryDirectoriesAsync(
        GithubLink githubLink,
        IList<string> relativePaths,
        CancellationToken cancellationToken
    );
}
