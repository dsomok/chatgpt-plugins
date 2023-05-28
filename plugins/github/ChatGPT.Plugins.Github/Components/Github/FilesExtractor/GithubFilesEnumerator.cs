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


    public async IAsyncEnumerable<string> EnumerateRepositoryFilesAsync(GithubLink githubLink, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var repository = await _githubClient.Repository.Get(githubLink.Owner, githubLink.RepositoryName);
        
        var commits = await _githubClient.Repository.Commit.GetAll(repository.Id, new ApiOptions { PageSize = 1, PageCount = 1 });
        var latestCommit = commits[0];

        var treeResponse = await _githubClient.Git.Tree.GetRecursive(repository.Id, latestCommit!.Sha);
        foreach (var treeItem in treeResponse.Tree.Where(IsIncluded))
        {
            if (string.IsNullOrEmpty(githubLink.RelativePath) || treeItem.Path.StartsWith(githubLink.RelativePath))
            {
                yield return treeItem.Path.Replace(githubLink.RelativePath ?? string.Empty, string.Empty).Trim('/');
            }
        }
    }

    private bool IsIncluded(TreeItem treeItem)
    {
        return _includedFilePatterns.Any(pattern => Regex.IsMatch(treeItem.Path, pattern));
    }
}
