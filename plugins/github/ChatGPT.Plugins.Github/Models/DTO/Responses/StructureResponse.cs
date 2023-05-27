using Swashbuckle.AspNetCore.Annotations;

namespace ChatGPT.Plugins.Github.Models.DTO.Responses;

public record StructureResponse(
    [SwaggerSchema("Files of the requested Github repository")] IList<string> Files
) : BasePluginResponse;
