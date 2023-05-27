using ChatGPT.Plugins.Github.Components.Github.FilesExtractor;
using ChatGPT.Plugins.Github.Components.Github.LinkParser;
using ChatGPT.Plugins.Github.Models;
using MediatR;

namespace ChatGPT.Plugins.Github.Handlers;

internal class GithubRepositoryStructureRequestHandler : IRequestHandler<GithubRepositoryStructureRequest, IList<GithubFileMetadata>>
{
    private const int MAX_FILES_COUNT = 98000;

    private readonly IGithubLinkParser _githubLinkParser;
    private readonly IGithubFilesEnumerator _githubFilesEnumerator;

    public GithubRepositoryStructureRequestHandler(IGithubLinkParser githubLinkParser, IGithubFilesEnumerator githubFilesEnumerator)
    {
        _githubLinkParser = githubLinkParser;
        _githubFilesEnumerator = githubFilesEnumerator;
    }

    public async Task<IList<GithubFileMetadata>> Handle(GithubRepositoryStructureRequest request, CancellationToken cancellationToken)
    {
        var githubLink = _githubLinkParser.Parse(request.GithubUrl);
        var repositoryFiles = _githubFilesEnumerator.EnumerateRepositoryFilesAsync(githubLink, cancellationToken);

        var files = (await repositoryFiles.Take(MAX_FILES_COUNT).ToListAsync(cancellationToken))
            .Select(filePath => new GithubFileMetadata(filePath)).ToList();

        return files;
    }
}

public record GithubRepositoryStructureRequest(string GithubUrl) : IRequest<IList<GithubFileMetadata>>;
