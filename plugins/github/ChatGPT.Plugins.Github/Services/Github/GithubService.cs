using Octokit;

namespace ChatGPT.Plugins.Github.Services.Github;

internal class GithubService : IGithubService
{
    private const int MAX_FILES_COUNT = 100;
    private readonly IGitHubClient _githubClient;

    public GithubService(IGitHubClient githubClient)
    {
        _githubClient = githubClient;
    }

    public IAsyncEnumerable<RepositoryContent> GetRepositoryFilesAsync(string owner, string name, CancellationToken cancellationToken)
    {
        return GetContent(owner, name, string.Empty, 0);
    }

    private async IAsyncEnumerable<RepositoryContent> GetContent(string owner, string name, string path, int processedCount)
    {
        var contents = string.IsNullOrWhiteSpace(path)
            ? await _githubClient.Repository.Content.GetAllContents(owner, name)
            : await _githubClient.Repository.Content.GetAllContents(owner, name, path);

        foreach (var content in contents)
        {
            if (content.Type.Value == ContentType.File)
            {
                yield return content;

                processedCount++;
                if (processedCount > MAX_FILES_COUNT)
                {
                    yield break;
                }
            }
            else if (content.Type.Value == ContentType.Dir)
            {
                await foreach (var repositoryContent in GetContent(owner, name, content.Path, processedCount))
                    yield return repositoryContent;
            }
        }
    }
}
