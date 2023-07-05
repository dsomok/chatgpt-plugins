using ChatGPT.Plugins.Github.Components.Github.FilesExtractor;
using ChatGPT.Plugins.Github.Components.Github.LinkParser;
using ChatGPT.Plugins.Github.Components.Repository;
using ChatGPT.Plugins.Github.Handlers.Models;
using ChatGPT.Plugins.Github.Models;
using MediatR;
using static ChatGPT.Plugins.Github.Prompts;

namespace ChatGPT.Plugins.Github.Handlers;

internal class GithubRepositoryStructureRequestHandler
    : IRequestHandler<GithubRepositoryStructureRequest, HandlerResponse<GithubRepositoryStructure>>
{
    private const int MAX_CHARACTERS_COUNT = 93000;
    private const int MAX_FILES_COUNT = 500;

    private readonly IGithubLinkParser _githubLinkParser;
    private readonly IGithubFilesEnumerator _githubFilesEnumerator;
    private readonly IEnumerable<IRepositoryStructureReducer> _repositoryStructureReducers;
    private readonly ILogger _logger;

    public GithubRepositoryStructureRequestHandler(
        IGithubLinkParser githubLinkParser,
        IGithubFilesEnumerator githubFilesEnumerator,
        IEnumerable<IRepositoryStructureReducer> repositoryStructureReducers,
        ILogger<GithubRepositoryStructureRequestHandler> logger
    )
    {
        _githubLinkParser = githubLinkParser;
        _githubFilesEnumerator = githubFilesEnumerator;
        _repositoryStructureReducers = repositoryStructureReducers;
        _logger = logger;
    }

    public async Task<HandlerResponse<GithubRepositoryStructure>> Handle(
        GithubRepositoryStructureRequest request, CancellationToken cancellationToken
    )
    {
        var githubLink = _githubLinkParser.Parse(request.GithubUrl);

        var repositoryFilesStructure = await GetRepositoryFilesStructureAsync(githubLink, request, cancellationToken);
        repositoryFilesStructure = await ReduceRepositoryStructureIfNeededAsync(repositoryFilesStructure, cancellationToken);

        if (repositoryFilesStructure.FilePaths.Count <= MAX_FILES_COUNT)
        {
            return new HandlerResponse<GithubRepositoryStructure>(
                repositoryFilesStructure, null, REPOSITORY_STRUCTURE_HINT
            );
        }

        var repositoryDirectoriesStructure = await GetRepositoryDirectoriesStructureAsync(
            githubLink, request, cancellationToken);

        repositoryDirectoriesStructure.AddFiles(repositoryFilesStructure.RootFiles);

        return new HandlerResponse<GithubRepositoryStructure>(
            repositoryDirectoriesStructure, null, REPOSITORY_DIRECTORIES_STRUCTURE_HINT
        );
    }

    private async Task<GithubRepositoryStructure> GetRepositoryFilesStructureAsync(
        GithubLink githubLink,
        GithubRepositoryStructureRequest request,
        CancellationToken cancellationToken
    )
    {
        var repositoryFiles = _githubFilesEnumerator.EnumerateRepositoryFilesAsync(
            githubLink, request.RelativePaths, request.FileExtensions, cancellationToken);

        var filePaths = await repositoryFiles.ToListAsync(cancellationToken);

        return new GithubRepositoryStructure(filePaths);
    }

    private async Task<GithubRepositoryStructure> ReduceRepositoryStructureIfNeededAsync(
        GithubRepositoryStructure repositoryStructure,
        CancellationToken cancellationToken
    )
    {
        if (repositoryStructure.FilePathsCharactersCount <= MAX_CHARACTERS_COUNT)
        {
            return repositoryStructure;
        }

        _logger.LogInformation("Reducing repository structure size. Initial size is {InitialRepositoryStructureCharactersCount} characters", repositoryStructure.FilePathsCharactersCount);

        foreach (var reducer in _repositoryStructureReducers)
        {
            repositoryStructure = await reducer.ReduceRepositoryStructureAsync(repositoryStructure, MAX_CHARACTERS_COUNT, cancellationToken);
            if (repositoryStructure.FilePathsCharactersCount <= MAX_CHARACTERS_COUNT)
            {
                break;
            }
        }

        _logger.LogInformation("Reduced repository structure down to {ReducedRepositoryStructureCharactersCount} characters in file paths", repositoryStructure.FilePathsCharactersCount);

        return repositoryStructure;
    }

    private async Task<GithubRepositoryStructure> GetRepositoryDirectoriesStructureAsync(
        GithubLink githubLink,
        GithubRepositoryStructureRequest request,
        CancellationToken cancellationToken
    )
    {
        var repositoryDirectories = _githubFilesEnumerator.EnumerateRepositoryDirectoriesAsync(
            githubLink, request.RelativePaths, cancellationToken);

        var filePaths = await repositoryDirectories.ToListAsync(cancellationToken);

        return new GithubRepositoryStructure(filePaths);
    }
}

public record GithubRepositoryStructureRequest(string GithubUrl, IList<string> RelativePaths, IList<string> FileExtensions)
    : IRequest<HandlerResponse<GithubRepositoryStructure>>;
