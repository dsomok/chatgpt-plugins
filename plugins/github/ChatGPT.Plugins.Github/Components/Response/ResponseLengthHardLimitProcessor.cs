using ChatGPT.Plugins.Github.Models;

namespace ChatGPT.Plugins.Github.Components.Response;

internal class ResponseLengthHardLimitProcessor : IResponseProcessor
{
    private const int MAX_RESPONSE_CHARACTERS = 95000;

    public Task<IList<GithubFile>> ProcessResponseAsync(IList<GithubFile> files, CancellationToken cancellationToken)
    {
        var result = new List<GithubFile>();
        var totalContentSize = 0;

        foreach (var file in files)
        {
            if (totalContentSize + file.Content.Length <= MAX_RESPONSE_CHARACTERS)
            {
                totalContentSize += file.Content.Length;
                result.Add(file);
            }
        }

        return Task.FromResult<IList<GithubFile>>(result);
    }
}