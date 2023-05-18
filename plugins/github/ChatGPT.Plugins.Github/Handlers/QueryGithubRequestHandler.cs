using System.Text;
using MediatR;
using Octokit;

namespace ChatGPT.Plugins.Github.Handlers;

internal class QueryGithubRequestHandler : IRequestHandler<QueryGithubRequest, string>
{
    private const int MAX_FILES_COUNT = 100;
    private readonly IGitHubClient _githubClient;

    public QueryGithubRequestHandler(IGitHubClient githubClient)
    {
        _githubClient = githubClient;
    }

    public async Task<string> Handle(QueryGithubRequest request, CancellationToken cancellationToken)
    {
        var repositorySegments = new Uri(request.GithubLink).Segments;

        var owner = repositorySegments[1].Replace("/", string.Empty);
        var name = repositorySegments[2].Replace("/", string.Empty);
        var processedCount = 0;

        var contents = GetContent(owner, name, string.Empty, processedCount);
        return await BuildResponse(owner, name, contents);
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
            else if(content.Type.Value == ContentType.Dir)
            {
                await foreach (var repositoryContent in GetContent(owner, name, content.Path, processedCount)) 
                    yield return repositoryContent;
            }
        }
    }

    private async Task<string> BuildResponse(string owner, string name, IAsyncEnumerable<RepositoryContent> contents)
    {
        var sb = new StringBuilder();

        await foreach (var content in contents)
        {
            var rawContent = await _githubClient.Repository.Content.GetRawContent(owner, name, content.Path);

            sb.AppendLine($"File {content.Name} content:");
            sb.AppendLine(Encoding.UTF8.GetString(rawContent));
            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public record QueryGithubRequest(string UserQuestion, string GithubLink) : IRequest<string>;
