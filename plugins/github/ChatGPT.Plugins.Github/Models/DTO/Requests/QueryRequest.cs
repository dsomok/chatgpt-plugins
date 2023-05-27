using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ChatGPT.Plugins.Github.Models.DTO.Requests;

public class QueryRequest
{
    [Required]
    [SwaggerSchema("Github repository URL")]
    public string RepositoryUrl { get; set; }

    [Required]
    [SwaggerSchema("Files to query the content of")]
    public List<string> FilePaths { get; set; }
}
