using ChatGPT.Plugins.Github.Components.Github.FilesExtractor;
using ChatGPT.Plugins.Github.Components.Github.LinkParser;
using ChatGPT.Plugins.Github.Models;
using MediatR;

namespace ChatGPT.Plugins.Github.Handlers;

internal class GithubRepositoryStructureRequestHandler : IRequestHandler<GithubRepositoryStructureRequest, IList<GithubFileMetadata>>
{
    private readonly IGithubLinkParser _githubLinkParser;
    private readonly IGithubFilesExtractor _githubFilesExtractor;

    public GithubRepositoryStructureRequestHandler(IGithubLinkParser githubLinkParser, IGithubFilesExtractor githubFilesExtractor)
    {
        _githubLinkParser = githubLinkParser;
        _githubFilesExtractor = githubFilesExtractor;
    }

    public async Task<IList<GithubFileMetadata>> Handle(GithubRepositoryStructureRequest request, CancellationToken cancellationToken)
    {
        var githubLink = _githubLinkParser.Parse(request.GithubUrl);
        var repositoryFiles = _githubFilesExtractor.GetRepositoryFilesAsync(githubLink, cancellationToken);

        var files = (await repositoryFiles.ToListAsync(cancellationToken))
            .Select(file => new GithubFileMetadata(file.Name, file.Path)).ToList();

        return files;
    }
}

public record GithubRepositoryStructureRequest(string GithubUrl) : IRequest<IList<GithubFileMetadata>>;
