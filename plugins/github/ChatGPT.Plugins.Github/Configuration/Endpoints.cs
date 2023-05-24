using ChatGPT.Plugins.Github.Handlers;
using ChatGPT.Plugins.Github.Models.DTO.Requests;
using ChatGPT.Plugins.Github.Models.DTO.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static ChatGPT.Plugins.Github.Constants;

namespace ChatGPT.Plugins.Github.Configuration;

public static class Endpoints
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {

        app.MapGet("/api/chatgpt-plugins/github/structure", async (string link, IMediator mediator) =>
           {
               var fileStructure = await mediator.Send(new GithubRepositoryStructureRequest(link));
               var response = new StructureResponse(fileStructure)
               {
                   AssistantHint = REPOSITORY_STRUCTURE_HINT
               };

               return TypedResults.Json(response);
           })
           .Produces<StructureResponse>()
           .WithOpenApi(operation => new(operation)
           {
               OperationId = "QueryGithubRepositoryStructure",
               Summary = "Retrieves the github repository file structure to analyze it and be able to query only relevant files"
           });


        app.MapPost("/api/chatgpt-plugins/github/query", async ([FromBody] QueryRequest request, IMediator mediator) =>
           {
               var files = await mediator.Send(new GithubRepositoryFilesRequest(request.Url, request.FilePaths));
               var response = new QueryResponse(files);
               return TypedResults.Json(response);
           })
           .Produces<QueryResponse>()
           .WithOpenApi(operation => new(operation)
           {
               OperationId = "QueryGithubRepositoryFileContents",
               Summary = "Retrieves github repository file contents, possibly filtered by names"
           });

        return app;
    }
}