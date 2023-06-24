using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ChatGPT.Plugins.Github.Models;
using Octokit;

namespace ChatGPT.Plugins.Github.Components.Github.FilesExtractor;

internal class GithubFilesEnumerator : IGithubFilesEnumerator
{
    private readonly IGitHubClient _githubClient;

    private readonly List<string> _excludedFilePatterns = new()
    {
        @"^.+\.exe$",
        @"^.+\.bin$",
        @"^.+\.dll$",
        @"^.+\.csproj$",
        @"^.+\.sln$",
        @"*ignore$"
    };


    public GithubFilesEnumerator(IGitHubClient githubClient)
    {
        _githubClient = githubClient;
    }


    public IAsyncEnumerable<string> EnumerateRepositoryFilesAsync(GithubLink githubLink, CancellationToken cancellationToken)
    {
        return EnumerateRepositoryFilesAsync(githubLink, null, cancellationToken);
    }

    public async IAsyncEnumerable<string> EnumerateRepositoryFilesAsync(
        GithubLink githubLink,
        IList<string> fileExtensions,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        var repository = await _githubClient.Repository.Get(githubLink.Owner, githubLink.RepositoryName);
        
        var commits = await _githubClient.Repository.Commit.GetAll(repository.Id, new ApiOptions { PageSize = 1, PageCount = 1 });
        var latestCommit = commits[0];

        var treeResponse = await _githubClient.Git.Tree.GetRecursive(repository.Id, latestCommit!.Sha);
        var treeItems = treeResponse.Tree.Where(IsIncluded)
                                    .Where(treeItem => IfOfSpecifiedExtension(treeItem, fileExtensions));

        foreach (var treeItem in treeItems)
        {
            if (string.IsNullOrEmpty(githubLink.RelativePath) || treeItem.Path.StartsWith(githubLink.RelativePath))
            {
                yield return GetFilePath(treeItem, githubLink);
            }
        }
    }

    private bool IsIncluded(TreeItem treeItem)
    {
        return treeItem.Size > 0 && _excludedFilePatterns.All(pattern => !Regex.IsMatch(treeItem.Path, pattern));
    }

    private bool IfOfSpecifiedExtension(TreeItem treeItem, IList<string> extensions)
    {
        if (extensions == null || extensions.Count == 0)
        {
            return true;
        }

        return extensions.Any(treeItem.Path.EndsWith);
    }

    private string GetFilePath(TreeItem treeItem, GithubLink githubLink)
    {
        var path = treeItem.Path;
        if (!string.IsNullOrEmpty(githubLink.RelativePath))
        {
            path = path.Replace(githubLink.RelativePath, string.Empty);
        }

        return path.Trim('/');
    }
}
