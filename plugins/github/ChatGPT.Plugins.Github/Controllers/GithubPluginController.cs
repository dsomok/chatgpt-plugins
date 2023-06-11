using ChatGPT.Plugins.Github.Handlers;
using ChatGPT.Plugins.Github.Models.DTO.Requests;
using ChatGPT.Plugins.Github.Models.DTO.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static ChatGPT.Plugins.Github.Constants;

namespace ChatGPT.Plugins.Github.Controllers;

[Route("api/chatgpt-plugins/askthecode")]
[ApiController]
public class GithubPluginController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;


    public GithubPluginController(IMediator mediator, ILogger<GithubPluginController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    

    [HttpGet]
    [Route("structure")]
    [SwaggerOperation(
        OperationId = "QueryGithubRepositoryStructure",
        Summary = "Retrieves the github repository file structure to analyze it and be able to query only relevant files"
    )]
    [SwaggerResponse(200, "Returns the github repository structure", typeof(StructureResponse))]
    public async Task<IActionResult> GetRepositoryStructure(
        [SwaggerParameter("Github repository URL", Required = true)] string repositoryUrl, 
        CancellationToken cancellationToken
    )
    {
        var fileStructure = await _mediator.Send(new GithubRepositoryStructureRequest(repositoryUrl), cancellationToken);
        var filePaths = fileStructure.Select(f => f.Path).ToList();

        var response = new StructureResponse(filePaths)
        {
            AssistantHint = REPOSITORY_STRUCTURE_HINT
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
        try
        {
            var files = await _mediator.Send(new GithubRepositoryFilesRequest(request.RepositoryUrl, request.FilePaths), cancellationToken);
            var response = new QueryResponse(files)
            {
                AssistantHint = QUERY_REPOSITORY_FILES
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to query repository files");

            var response = new ErrorResponse
            {
                AssistantHint = QUERY_REPOSITORY_FILES_ERROR
            };

            return Ok(response);
        }
    }
}
