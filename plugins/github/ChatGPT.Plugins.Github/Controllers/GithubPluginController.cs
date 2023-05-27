using ChatGPT.Plugins.Github.Handlers;
using ChatGPT.Plugins.Github.Models.DTO.Requests;
using ChatGPT.Plugins.Github.Models.DTO.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static ChatGPT.Plugins.Github.Constants;

namespace ChatGPT.Plugins.Github.Controllers;

[Route("api/chatgpt-plugins/github")]
[ApiController]
public class GithubPluginController : ControllerBase
{
    private readonly IMediator _mediator;


    public GithubPluginController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    [HttpGet]
    [Route("structure")]
    [ProducesResponseType(200, Type = typeof(StructureResponse))]
    [SwaggerOperation(OperationId = "QueryGithubRepositoryStructure", Summary = "Retrieves the github repository file structure to analyze it and be able to query only relevant files")]
    public async Task<IActionResult> GetRepositoryStructure(string link, CancellationToken cancellationToken)
    {
        var fileStructure = await _mediator.Send(new GithubRepositoryStructureRequest(link), cancellationToken);
        var filePaths = fileStructure.Select(f => f.Path).ToList();

        var response = new StructureResponse(filePaths)
        {
            AssistantHint = REPOSITORY_STRUCTURE_HINT
        };

        return Ok(response);
    }
    

    [HttpPost]
    [Route("query")]
    [ProducesResponseType(200, Type = typeof(QueryResponse))]
    [SwaggerOperation(OperationId = "QueryGithubRepositoryFileContents", Summary = "Retrieves github repository file contents, possibly filtered by names")]
    public async Task<IActionResult> QueryRepositoryFiles([FromBody] QueryRequest request, CancellationToken cancellationToken)
    {
        var files = await _mediator.Send(new GithubRepositoryFilesRequest(request.Url, request.FilePaths), cancellationToken);
        var response = new QueryResponse(files);
        return Ok(response);
    }
}
