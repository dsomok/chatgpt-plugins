using System.Text;
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
    private const int MAX_CHARACTERS_COUNT = 98000;

    private readonly IGitHubClient _githubClient;
    private readonly IGithubLinkParser _githubLinkParser;
    private readonly IGithubFilesEnumerator _githubFilesEnumerator;
    private readonly IEnumerable<IFileContentProcessor> _fileContentProcessors;
    private readonly IEnumerable<IResponseProcessor> _responseProcessors;

    public GithubRepositoryFilesRequestHandler(
        IGitHubClient githubClient,
        IGithubLinkParser githubLinkParser,
        IGithubFilesEnumerator githubFilesEnumerator,
        IEnumerable<IFileContentProcessor> fileContentProcessors,
        IEnumerable<IResponseProcessor> responseProcessors
    )
    {
        _githubClient = githubClient;
        _githubLinkParser = githubLinkParser;
        _githubFilesEnumerator = githubFilesEnumerator;
        _fileContentProcessors = fileContentProcessors;
        _responseProcessors = responseProcessors;
    }

    public async Task<IList<GithubFile>> Handle(GithubRepositoryFilesRequest request, CancellationToken cancellationToken)
    {
        var githubLink = _githubLinkParser.Parse(request.GithubUrl);
        var filesStream = request.Files != null && request.Files.Any()
            ? request.Files.ToAsyncEnumerable()
            : _githubFilesEnumerator.EnumerateRepositoryFilesAsync(githubLink, cancellationToken);

        return await BuildResponseAsync(githubLink, filesStream, cancellationToken);
    }

    private async Task<IList<GithubFile>> BuildResponseAsync(
        GithubLink githubLink,
        IAsyncEnumerable<string> filePaths,
        CancellationToken cancellationToken
    )
    {
        var githubFiles = new List<GithubFile>();
        
        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = 4
        };

        var charactersLeft = MAX_CHARACTERS_COUNT;
        await Parallel.ForEachAsync(filePaths, parallelOptions, async (filePath, ct) =>
        {
            var rawContentBytes = await _githubClient.Repository.Content.GetRawContent(githubLink.Owner, githubLink.RepositoryName, filePath);
            var rawContent = Encoding.UTF8.GetString(rawContentBytes);
            var fileContent = await ProcessFileContentAsync(rawContent, ct);

            if (fileContent.Length < charactersLeft)
            {
                githubFiles.Add(new GithubFile(filePath, fileContent));
                Interlocked.Add(ref charactersLeft, -1 * fileContent.Length);
            }
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
