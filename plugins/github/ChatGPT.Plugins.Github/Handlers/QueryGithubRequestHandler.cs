using System.Text;
using ChatGPT.Plugins.Github.Components.Files;
using ChatGPT.Plugins.Github.Components.Github.FilesExtractor;
using ChatGPT.Plugins.Github.Components.Github.LinkParser;
using ChatGPT.Plugins.Github.Models;
using MediatR;
using Octokit;

namespace ChatGPT.Plugins.Github.Handlers;

internal class QueryGithubRequestHandler : IRequestHandler<QueryGithubRequest, IList<GithubFile>>
{
    private readonly IGitHubClient _githubClient;
    private readonly IGithubLinkParser _githubLinkParser;
    private readonly IGithubFilesExtractor _githubFilesExtractor;
    private readonly IEnumerable<IFileContentProcessor> _fileContentProcessors;

    public QueryGithubRequestHandler(
        IGitHubClient githubClient,
        IGithubLinkParser githubLinkParser,
        IGithubFilesExtractor githubFilesExtractor,
        IEnumerable<IFileContentProcessor> fileContentProcessors
    )
    {
        _githubClient = githubClient;
        _githubLinkParser = githubLinkParser;
        _githubFilesExtractor = githubFilesExtractor;
        _fileContentProcessors = fileContentProcessors;
    }

    public async Task<IList<GithubFile>> Handle(QueryGithubRequest request, CancellationToken cancellationToken)
    {
        var githubLink = _githubLinkParser.Parse(request.GithubLink);
        var files = _githubFilesExtractor.GetRepositoryFilesAsync(githubLink, cancellationToken);

        return await BuildResponse(githubLink, files, cancellationToken);
    }

    private async Task<IList<GithubFile>> BuildResponse(GithubLink githubLink, IAsyncEnumerable<RepositoryContent> files, CancellationToken cancellationToken)
    {
        var githubFiles = new List<GithubFile>();
        await foreach (var file in files.WithCancellation(cancellationToken))
        {
            var rawContentBytes = await _githubClient.Repository.Content.GetRawContent(githubLink.Owner, githubLink.RepositoryName, file.Path);
            var rawContent = Encoding.UTF8.GetString(rawContentBytes);
            var fileContent = await ProcessFileContent(rawContent, cancellationToken);

            githubFiles.Add(new GithubFile(file.Name, fileContent));
        }

        return githubFiles;
    }

    private async Task<string> ProcessFileContent(string content, CancellationToken cancellationToken)
    {
        foreach (var processor in _fileContentProcessors)
        {
            content = await processor.ProcessFileContentAsync(content, cancellationToken);
        }

        return content;
    }
}

public record QueryGithubRequest(string GithubLink) : IRequest<IList<GithubFile>>;
