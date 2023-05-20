using ChatGPT.Plugins.Github.Handlers;
using ChatGPT.Plugins.Github.Models;
using MediatR;

namespace ChatGPT.Plugins.Github.Configuration;

public static class Endpoints
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapGet("/api/chatgpt-plugins/github/query", async (string link, IMediator mediator) =>
           {
               var content = await mediator.Send(new QueryGithubRequest(link));
               return new QueryResponse(content);
           })
           .Produces<QueryResponse>()
           .WithOpenApi(operation => new(operation)
           {
               OperationId = "QueryGithubPlugin",
               Summary = "Retrieves information related to the users question from the provided github link"
           });

        return app;
    }
}