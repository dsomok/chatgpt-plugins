using System.Text;
using System.Threading;
using ChatGPT.Plugins.Github.Components.Files;
using ChatGPT.Plugins.Github.Components.Github.FilesExtractor;
using ChatGPT.Plugins.Github.Components.Github.LinkParser;
using ChatGPT.Plugins.Github.Components.Response;
using ChatGPT.Plugins.Github.Models;
using MediatR;
using Octokit;

namespace ChatGPT.Plugins.Github.Handlers;

internal class GithubRepositoryFilesRequestHandler : IRequestHandler<GithubRepositoryFilesRequest, IList<GithubFile>>
{
    private readonly IGitHubClient _githubClient;
    private readonly IGithubLinkParser _githubLinkParser;
    private readonly IGithubFilesExtractor _githubFilesExtractor;
    private readonly IEnumerable<IFileContentProcessor> _fileContentProcessors;
    private readonly IEnumerable<IResponseProcessor> _responseProcessors;

    public GithubRepositoryFilesRequestHandler(
        IGitHubClient githubClient,
        IGithubLinkParser githubLinkParser,
        IGithubFilesExtractor githubFilesExtractor,
        IEnumerable<IFileContentProcessor> fileContentProcessors,
        IEnumerable<IResponseProcessor> responseProcessors
    )
    {
        _githubClient = githubClient;
        _githubLinkParser = githubLinkParser;
        _githubFilesExtractor = githubFilesExtractor;
        _fileContentProcessors = fileContentProcessors;
        _responseProcessors = responseProcessors;
    }

    public async Task<IList<GithubFile>> Handle(GithubRepositoryFilesRequest request, CancellationToken cancellationToken)
    {
        var githubLink = _githubLinkParser.Parse(request.GithubUrl);
        var files = _githubFilesExtractor.GetRepositoryFilesAsync(githubLink, cancellationToken);

        return await BuildResponseAsync(request, githubLink, files, cancellationToken);
    }

    private async Task<IList<GithubFile>> BuildResponseAsync(
        GithubRepositoryFilesRequest request,
        GithubLink githubLink,
        IAsyncEnumerable<RepositoryContent> files,
        CancellationToken cancellationToken
    )
    {
        var githubFiles = new List<GithubFile>();
        
        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = 4
        };

        await Parallel.ForEachAsync(files, parallelOptions, async (file, ct) =>
        {
            if (request.Files != null && request.Files.Any() && !request.Files.Contains(file.Name))
            {
                return;
            }

            var rawContentBytes = await _githubClient.Repository.Content.GetRawContent(githubLink.Owner, githubLink.RepositoryName, file.Path);
            var rawContent = Encoding.UTF8.GetString(rawContentBytes);
            var fileContent = await ProcessFileContentAsync(rawContent, ct);

            githubFiles.Add(new GithubFile(file.Name, file.Path, fileContent));
        });

        return await ProcessResponseAsync(githubFiles, cancellationToken);
    }

    private async Task<string> ProcessFileContentAsync(string content, CancellationToken cancellationToken)
    {
        foreach (var processor in _fileContentProcessors)
        {
            content = await processor.ProcessFileContentAsync(content, cancellationToken);
        }

        return content;
    }

    private async Task<IList<GithubFile>> ProcessResponseAsync(IList<GithubFile> response, CancellationToken cancellationToken)
    {
        foreach (var processor in _responseProcessors)
        {
            response = await processor.ProcessResponseAsync(response, cancellationToken);
        }

        return response;
    }
}

public record GithubRepositoryFilesRequest(string GithubUrl, IList<string> Files = null) : IRequest<IList<GithubFile>>;
