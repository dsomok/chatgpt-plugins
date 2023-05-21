using ChatGPT.Plugins.Github.Models;

namespace ChatGPT.Plugins.Github.Components.Response;

public interface IResponseProcessor
{
    Task<IList<GithubFile>> ProcessResponseAsync(IList<GithubFile> files, CancellationToken cancellationToken);
}