using System.Text;
using ChatGPT.Plugins.Github.Components.Github.FilesExtractor;
using ChatGPT.Plugins.Github.Components.Github.LinkParser;
using ChatGPT.Plugins.Github.Models;
using MediatR;
using Octokit;

namespace ChatGPT.Plugins.Github.Handlers;

internal class QueryGithubRequestHandler : IRequestHandler<QueryGithubRequest, string>
{
    private readonly IGitHubClient _githubClient;
    private readonly IGithubLinkParser _githubLinkParser;
    private readonly IGithubFilesExtractor _githubFilesExtractor;

    public QueryGithubRequestHandler(IGitHubClient githubClient, IGithubLinkParser githubLinkParser, IGithubFilesExtractor githubFilesExtractor)
    {
        _githubClient = githubClient;
        _githubLinkParser = githubLinkParser;
        _githubFilesExtractor = githubFilesExtractor;
    }

    public async Task<string> Handle(QueryGithubRequest request, CancellationToken cancellationToken)
    {
        var githubLink = _githubLinkParser.Parse(request.GithubLink);

        var contents = _githubFilesExtractor.GetRepositoryFilesAsync(githubLink, cancellationToken);
        return await BuildResponse(githubLink, contents, cancellationToken);
    }

    private async Task<string> BuildResponse(GithubLink githubLink, IAsyncEnumerable<RepositoryContent> contents, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();

        await foreach (var content in contents.WithCancellation(cancellationToken))
        {
            var rawContent = await _githubClient.Repository.Content.GetRawContent(githubLink.Owner, githubLink.RepositoryName, content.Path);

            sb.AppendLine($"File {content.Name} content:");
            sb.AppendLine(Encoding.UTF8.GetString(rawContent));
            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public record QueryGithubRequest(string GithubLink) : IRequest<string>;
