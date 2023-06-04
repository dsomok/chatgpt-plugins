using ChatGPT.Plugins.Github.Components.Files;
using ChatGPT.Plugins.Github.Components.Github.FilesExtractor;
using ChatGPT.Plugins.Github.Components.Github.LinkParser;
using ChatGPT.Plugins.Github.Components.Repository;
using ChatGPT.Plugins.Github.Components.Response;

namespace ChatGPT.Plugins.Github.Components;

internal static class Dependencies
{
    public static IServiceCollection AddComponents(this IServiceCollection services)
    {
        return services.AddSingleton<IFileContentProcessor, CommentsRemover>()
                       .AddSingleton<IFileContentProcessor, UsingsRemover>()
                       .AddSingleton<IFileContentProcessor, FileContentMinifier>()
                       .AddSingleton<IResponseProcessor, ResponseLengthHardLimitProcessor>()
                       .AddSingleton<IRepositoryStructureReducer, IgnoreInterfacesRepositoryStructureReducer>()
                       .AddSingleton<IRepositoryStructureReducer, HardLimitRepositoryStructureReducer>()
                       .AddSingleton<IGithubFilesEnumerator, GithubFilesEnumerator>()
                       .AddSingleton<IGithubLinkParser, GithubLinkParser>();
    }
}
