using ChatGPT.Plugins.Github.Models;

namespace ChatGPT.Plugins.Github.Components.Repository;

internal class HardLimitRepositoryStructureReducer : IRepositoryStructureReducer
{
    private readonly ILogger _logger;

    public HardLimitRepositoryStructureReducer(ILogger<HardLimitRepositoryStructureReducer> logger)
    {
        _logger = logger;
    }

    public Task<GithubRepositoryStructure> ReduceRepositoryStructureAsync(GithubRepositoryStructure repositoryStructure, int maxCharacters, CancellationToken cancellationToken)
    {
        if (repositoryStructure.FilePathsCharactersCount < maxCharacters)
        {
            return Task.FromResult(repositoryStructure);
        }

        _logger.LogInformation("Applying {RepositoryStructureReducer} repository structure reducer", nameof(HardLimitRepositoryStructureReducer));

        var reducedFilePaths = new List<string>();
        var charactersLeft = maxCharacters;
        foreach (var filePath in repositoryStructure.FilePaths)
        {
            if (filePath.Length < charactersLeft)
            {
                charactersLeft -= filePath.Length;
                reducedFilePaths.Add(filePath);
            }
        }

        // ReSharper disable once WithExpressionModifiesAllMembers
        repositoryStructure = repositoryStructure with { FilePaths = reducedFilePaths };

        _logger.LogInformation(
            "{RepositoryStructureReducer} reduced repository structure down to {ReducerReducedRepositoryStructureCharactersCount}", 
            nameof(HardLimitRepositoryStructureReducer), repositoryStructure.FilePathsCharactersCount
        );

        return Task.FromResult(repositoryStructure);
    }
}
