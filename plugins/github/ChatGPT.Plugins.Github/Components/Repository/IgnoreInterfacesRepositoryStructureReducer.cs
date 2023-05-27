using System.Text.RegularExpressions;
using ChatGPT.Plugins.Github.Models;

namespace ChatGPT.Plugins.Github.Components.Repository;

internal partial class IgnoreInterfacesRepositoryStructureReducer : IRepositoryStructureReducer
{
    private readonly ILogger _logger;

    public IgnoreInterfacesRepositoryStructureReducer(ILogger<IgnoreInterfacesRepositoryStructureReducer> logger)
    {
        _logger = logger;
    }

    public Task<GithubRepositoryStructure> ReduceRepositoryStructureAsync(GithubRepositoryStructure repositoryStructure, int maxCharacters, CancellationToken cancellationToken)
    {
        if (repositoryStructure.FilePathsCharactersCount < maxCharacters)
        {
            return Task.FromResult(repositoryStructure);
        }

        _logger.LogInformation("Applying {RepositoryStructureReducer} repository structure reducer", nameof(IgnoreInterfacesRepositoryStructureReducer));

        var filePathsToIgnore = new List<string>();

        var currentCharactersCount = repositoryStructure.FilePathsCharactersCount;
        foreach (var filePath in repositoryStructure.FilePaths)
        {
            if (!IsInterfaceRegex().IsMatch(filePath))
            {
                continue;
            }

            filePathsToIgnore.Add(filePath);
            currentCharactersCount -= filePath.Length;

            if (currentCharactersCount <= maxCharacters)
            {
                break;
            }
        }

        if (!filePathsToIgnore.Any())
        {
            return Task.FromResult(repositoryStructure);
        }

        var reducedFilePaths = repositoryStructure.FilePaths
                                                  .Where(filePath => !filePathsToIgnore.Contains(filePath))
                                                  .ToList();

        // ReSharper disable once WithExpressionModifiesAllMembers
        repositoryStructure = repositoryStructure with { FilePaths = reducedFilePaths };

        _logger.LogInformation(
            "{RepositoryStructureReducer} reduced repository structure down to {ReducerReducedRepositoryStructureCharactersCount}",
            nameof(IgnoreInterfacesRepositoryStructureReducer), repositoryStructure.FilePathsCharactersCount
        );

        return Task.FromResult(repositoryStructure);
    }


    [GeneratedRegex(@"I[A-Z][A-Za-z0-9_]*\.cs$")]
    private static partial Regex IsInterfaceRegex();
}
