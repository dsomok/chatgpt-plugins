using ChatGPT.Plugins.Github.HttpClients;
using MediatR;

namespace ChatGPT.Plugins.Github.Handlers;

internal class QueryGithubRequestHandler : IRequestHandler<QueryGithubRequest, string>
{
    private readonly IGithubHttpClient _githubHttpClient;

    public QueryGithubRequestHandler(IGithubHttpClient githubHttpClient)
    {
        _githubHttpClient = githubHttpClient;
    }

    public Task<string> Handle(QueryGithubRequest request, CancellationToken cancellationToken)
    {
        var relativeUri = new Uri(request.GithubLink).PathAndQuery;
        return _githubHttpClient.GetRawContentAsync(relativeUri, cancellationToken);
    }
}

public record QueryGithubRequest(string UserQuestion, string GithubLink) : IRequest<string>;
