using ChatGPT.Plugins.Github.Handlers;
using ChatGPT.Plugins.Github.Models.DTO.Requests;
using ChatGPT.Plugins.Github.Models.DTO.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ChatGPT.Plugins.Github.Controllers;

[Route("api/chatgpt-plugins/askthecode")]
[ApiController]
public class AskTheCodePluginController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;


    public AskTheCodePluginController(IMediator mediator, ILogger<AskTheCodePluginController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }


    [HttpPost]
    [Route("structure")]
    [SwaggerOperation(
        OperationId = "QueryGithubRepositoryStructure",
        Summary = "Retrieves the github repository file structure to analyze it and be able to query only relevant files"
    )]
    [SwaggerResponse(200, "Returns the github repository structure", typeof(StructureResponse))]
    public async Task<IActionResult> GetRepositoryStructure([FromBody] StructureRequest request, CancellationToken cancellationToken)
    {
        var fileStructureResponse = await _mediator.Send(
            new GithubRepositoryStructureRequest(request.RepositoryUrl, request.RelativePaths, request.Extensions),
            cancellationToken
        );
        
        var response = new StructureResponse(fileStructureResponse.Response.FilePaths)
        {
            AssistantHint = fileStructureResponse.AssistantHint
        };

        return Ok(response);
    }


    [HttpPost]
    [Route("query")]
    [SwaggerOperation(
        OperationId = "QueryGithubRepositoryFileContents",
        Summary = "Retrieves github repository file contents, possibly filtered by names"
    )]
    [SwaggerResponse(200, "Returns the contents of the requested files", typeof(QueryResponse))]
    public async Task<IActionResult> QueryRepositoryFiles([FromBody] QueryRequest request, CancellationToken cancellationToken)
    {
        var filesResponse = await _mediator.Send(
            new GithubRepositoryFilesRequest(request.RepositoryUrl, request.FilePaths),
            cancellationToken
        );

        BasePluginResponse response = filesResponse.IsSuccess
            ? new QueryResponse(filesResponse.Response) { AssistantHint = filesResponse.AssistantHint }
            : new ErrorResponse { AssistantHint = filesResponse.AssistantHint };

        return Ok(response);
    }
}
