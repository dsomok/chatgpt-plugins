﻿using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ChatGPT.Plugins.Github.Models;
using LazyCache;
using Octokit;

namespace ChatGPT.Plugins.Github.Components.Github.FilesExtractor;

internal class GithubFilesEnumerator : IGithubFilesEnumerator
{
    private readonly TimeSpan CACHE_EXPIRATION = TimeSpan.FromMinutes(5);

    private readonly IGitHubClient _githubClient;
    private readonly IAppCache _cache;

    private readonly List<string> _excludedFilePatterns = new()
    {
        @"^.+\.exe$",
        @"^.+\.bin$",
        @"^.+\.dll$",
        @"^.+\.csproj$",
        @"^.+\.sln$",
        @"^.+ignore$"
    };


    public GithubFilesEnumerator(IGitHubClient githubClient, IAppCache cache)
    {
        _githubClient = githubClient;
        _cache = cache;
    }


    public IAsyncEnumerable<string> EnumerateRepositoryFilesAsync(GithubLink githubLink, CancellationToken cancellationToken)
    {
        return EnumerateRepositoryFilesAsync(githubLink, null, null, cancellationToken);
    }

    public async IAsyncEnumerable<string> EnumerateRepositoryFilesAsync(
        GithubLink githubLink,
        IList<string> relativePaths,
        IList<string> fileExtensions,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        var treeResponse = await GetRepositoryItemsAsync(githubLink, cancellationToken);
        var treeItems = treeResponse.Tree
                                    .Where(IsIncluded)
                                    .Where(treeItem => IfOfSpecifiedExtension(treeItem, fileExtensions))
                                    .Where(treeItem => ContainsInRequestedFilePaths(treeItem, relativePaths));

        foreach (var treeItem in treeItems)
        {
            if (string.IsNullOrEmpty(githubLink.RelativePath) || treeItem.Path.StartsWith(githubLink.RelativePath))
            {
                yield return GetFilePath(treeItem, githubLink);
            }
        }
    }

    public async IAsyncEnumerable<string> EnumerateRepositoryDirectoriesAsync(
        GithubLink githubLink,
        IList<string> relativePaths,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        var treeResponse = await GetRepositoryItemsAsync(githubLink, cancellationToken);
        var treeItems = treeResponse.Tree
                                    .Where(IsDirectory)
                                    .Where(treeItem => ContainsInRequestedFilePaths(treeItem, relativePaths)); ;

        foreach (var treeItem in treeItems)
        {
            if (string.IsNullOrEmpty(githubLink.RelativePath) || treeItem.Path.StartsWith(githubLink.RelativePath))
            {
                yield return GetFilePath(treeItem, githubLink);
            }
        }
    }


    private Task<TreeResponse> GetRepositoryItemsAsync(GithubLink githubLink, CancellationToken cancellationToken)
    {
        var key = $"repo-files-{githubLink.Owner}-{githubLink.RepositoryName}";
        return _cache.GetOrAddAsync(key, async () =>
        {
            var repository = await _githubClient.Repository.Get(githubLink.Owner, githubLink.RepositoryName);

            var commits = await _githubClient.Repository.Commit.GetAll(
                repository.Id,
                new ApiOptions { PageSize = 1, PageCount = 1 }
            );

            var latestCommit = commits[0];

            return await _githubClient.Git.Tree.GetRecursive(repository.Id, latestCommit!.Sha);
        }, DateTimeOffset.UtcNow.Add(CACHE_EXPIRATION), ExpirationMode.ImmediateEviction);
    }

    private bool IsDirectory(TreeItem treeItem) 
        => treeItem.Size == 0;

    private bool IsIncluded(TreeItem treeItem) => 
        !IsDirectory(treeItem) && _excludedFilePatterns.All(pattern => !Regex.IsMatch(treeItem.Path, pattern));

    private bool ContainsInRequestedFilePaths(TreeItem treeItem, IList<string> relativePaths)
    {
        if (relativePaths == null || relativePaths.Count == 0)
        {
            return true;
        }

        return relativePaths.Any(treeItem.Path.StartsWith);
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
