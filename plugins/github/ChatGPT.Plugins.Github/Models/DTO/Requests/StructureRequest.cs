using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ChatGPT.Plugins.Github.Models.DTO.Requests;

public class StructureRequest
{
    [Required]
    [SwaggerSchema("Github repository URL")]
    public string RepositoryUrl { get; set; }

    [SwaggerParameter("Relative paths to retrieve. Do not provide if unaware of the repository global structure")]
    public IList<string> RelativePaths { get; set; }

    [SwaggerParameter("List of file extensions to retrieve")]
    public IList<string> Extensions { get; set; }
}
