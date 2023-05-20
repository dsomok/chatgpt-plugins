using ChatGPT.Plugins.Github.Models;

namespace ChatGPT.Plugins.Github.Components.Github.LinkParser;

internal class GithubLinkParser : IGithubLinkParser
{
    public GithubLink Parse(string githubLink)
    {
        var repositorySegments = new Uri(githubLink).Segments;
        if (repositorySegments.Length < 3)
        {
            throw new Exception("Failed to parse github link");
        }

        var owner = repositorySegments[1].Replace("/", string.Empty);
        var repositoryName = repositorySegments[2].Replace("/", string.Empty);

        var branch = string.Empty;
        if (repositorySegments.Length >= 5)
        {
            branch = repositorySegments[4].Replace("/", string.Empty);
        }

        var relativePath = string.Empty;
        if (repositorySegments.Length >= 6)
        {
            relativePath = repositorySegments.Skip(5).Aggregate(string.Empty, (accumulated, segment) => accumulated + segment);
        }

        return new GithubLink(owner, repositoryName, branch, relativePath);
    }
}
