using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ChatGPT.Plugins.Github.Models;
using Octokit;

namespace ChatGPT.Plugins.Github.Components.Github.FilesExtractor;

internal class GithubFilesEnumerator : IGithubFilesEnumerator
{
    private readonly IGitHubClient _githubClient;

    private readonly List<string> _includedFilePatterns = new()
    {
        @"^.+\.cs$",
        @"^.+\.md$",
        @"^.+\.json",
        @"^.+\.txt$"
    };


    public GithubFilesEnumerator(IGitHubClient githubClient)
    {
        _githubClient = githubClient;
    }


    public IAsyncEnumerable<string> EnumerateRepositoryFilesAsync(GithubLink githubLink, CancellationToken cancellationToken)
    {

        return EnumerateFiles(githubLink.Owner, githubLink.RepositoryName, githubLink.RelativePath, cancellationToken);
    }


    private async IAsyncEnumerable<string> EnumerateFiles(string owner, string name, string path, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var contents = string.IsNullOrWhiteSpace(path)
            ? await _githubClient.Repository.Content.GetAllContents(owner, name)
            : await _githubClient.Repository.Content.GetAllContents(owner, name, path);

        foreach (var content in contents)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            if (content.Type.Value == ContentType.File)
            {
                if (!IsIncluded(content))
                {
                    continue;
                }

                yield return content.Path;
            }
            else if (content.Type.Value == ContentType.Dir)
            {
                await foreach (var repositoryContent in EnumerateFiles(owner, name, content.Path, cancellationToken))
                    yield return repositoryContent;
            }
        }
    }

    private bool IsIncluded(RepositoryContent file)
    {
        return _includedFilePatterns.Any(pattern => Regex.IsMatch(file.Name, pattern));
    }
}