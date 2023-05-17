using ChatGPT.Plugins.Github.Strategies;
using MediatR;

namespace ChatGPT.Plugins.Github.Handlers;

internal class QueryGithubRequestHandler : IRequestHandler<QueryGithubRequest, string>
{
    private readonly IEnumerable<IGithubStrategy> _strategies;

    public QueryGithubRequestHandler(IEnumerable<IGithubStrategy> strategies)
    {
        _strategies = strategies;
    }

    public Task<string> Handle(QueryGithubRequest request, CancellationToken cancellationToken)
    {
        var strategy = _strategies.FirstOrDefault(strategy => strategy.IsApplicable(request.GithubLink));
        if (strategy == null)
        {
            throw new Exception("Failed to process the provided Github link");
        }

        return strategy.ApplyAsync(request.GithubLink, cancellationToken);
    }
}

public record QueryGithubRequest(string UserQuestion, string GithubLink) : IRequest<string>;
