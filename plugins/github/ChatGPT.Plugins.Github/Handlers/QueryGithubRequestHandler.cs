using System.Text;
using ChatGPT.Plugins.Github.Services.Github;
using MediatR;
using Octokit;

namespace ChatGPT.Plugins.Github.Handlers;

internal class QueryGithubRequestHandler : IRequestHandler<QueryGithubRequest, string>
{
    private readonly IGitHubClient _githubClient;
    private readonly IGithubService _githubService;

    public QueryGithubRequestHandler(IGitHubClient githubClient, IGithubService githubService)
    {
        _githubClient = githubClient;
        _githubService = githubService;
    }

    public async Task<string> Handle(QueryGithubRequest request, CancellationToken cancellationToken)
    {
        var repositorySegments = new Uri(request.GithubLink).Segments;

        var owner = repositorySegments[1].Replace("/", string.Empty);
        var name = repositorySegments[2].Replace("/", string.Empty);

        var contents = _githubService.GetRepositoryFilesAsync(owner, name, cancellationToken);
        return await BuildResponse(owner, name, contents, cancellationToken);
    }

    private async Task<string> BuildResponse(string owner, string name, IAsyncEnumerable<RepositoryContent> contents, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();

        await foreach (var content in contents.WithCancellation(cancellationToken))
        {
            var rawContent = await _githubClient.Repository.Content.GetRawContent(owner, name, content.Path);

            sb.AppendLine($"File {content.Name} content:");
            sb.AppendLine(Encoding.UTF8.GetString(rawContent));
            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public record QueryGithubRequest(string GithubLink) : IRequest<string>;
