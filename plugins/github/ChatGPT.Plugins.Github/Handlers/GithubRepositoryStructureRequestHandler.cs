using ChatGPT.Plugins.Github.Components.Github.FilesExtractor;
using ChatGPT.Plugins.Github.Components.Github.LinkParser;
using ChatGPT.Plugins.Github.Components.Repository;
using ChatGPT.Plugins.Github.Models;
using MediatR;

namespace ChatGPT.Plugins.Github.Handlers;

internal class GithubRepositoryStructureRequestHandler : IRequestHandler<GithubRepositoryStructureRequest, IList<GithubFileMetadata>>
{
    private const int MAX_CHARACTERS_COUNT = 93000;

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

    public async Task<IList<GithubFileMetadata>> Handle(GithubRepositoryStructureRequest request, CancellationToken cancellationToken)
    {
        var githubLink = _githubLinkParser.Parse(request.GithubUrl);
        var repositoryFiles = _githubFilesEnumerator.EnumerateRepositoryFilesAsync(githubLink, cancellationToken);
        var filePaths = await repositoryFiles.ToListAsync(cancellationToken);

        var repositoryStructure = new GithubRepositoryStructure(filePaths);
        repositoryStructure = await ReduceRepositoryStructureIfNeededAsync(repositoryStructure, cancellationToken);

        return repositoryStructure.FilesMetadata;
    }

    private async Task<GithubRepositoryStructure> ReduceRepositoryStructureIfNeededAsync(GithubRepositoryStructure repositoryStructure, CancellationToken cancellationToken)
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
}

public record GithubRepositoryStructureRequest(string GithubUrl) : IRequest<IList<GithubFileMetadata>>;
