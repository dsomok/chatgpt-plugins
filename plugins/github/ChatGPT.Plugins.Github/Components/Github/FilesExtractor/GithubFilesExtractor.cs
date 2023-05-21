using System.Text.RegularExpressions;
using ChatGPT.Plugins.Github.Models;
using Octokit;

namespace ChatGPT.Plugins.Github.Components.Github.FilesExtractor;

internal class GithubFilesExtractor : IGithubFilesExtractor
{
    private const int MAX_FILES_COUNT = 100;
    private readonly IGitHubClient _githubClient;

    private readonly List<string> _includedFilePatterns = new()
    {
        @"^.+\.cs$",
        @"^.+\.md$"
    };

    public GithubFilesExtractor(IGitHubClient githubClient)
    {
        _githubClient = githubClient;
    }

    public IAsyncEnumerable<RepositoryContent> GetRepositoryFilesAsync(GithubLink githubLink, CancellationToken cancellationToken)
    {
        return GetContent(githubLink.Owner, githubLink.RepositoryName, githubLink.RelativePath, 0, cancellationToken);
    }

    private async IAsyncEnumerable<RepositoryContent> GetContent(string owner, string name, string path, int processedCount, CancellationToken cancellationToken)
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

                yield return content;

                processedCount++;
                if (processedCount > MAX_FILES_COUNT)
                {
                    yield break;
                }
            }
            else if (content.Type.Value == ContentType.Dir)
            {
                await foreach (var repositoryContent in GetContent(owner, name, content.Path, processedCount, cancellationToken))
                    yield return repositoryContent;
            }
        }
    }

    private bool IsIncluded(RepositoryContent file)
    {
        return _includedFilePatterns.Any(pattern => Regex.IsMatch(file.Name, pattern));
    }
}
