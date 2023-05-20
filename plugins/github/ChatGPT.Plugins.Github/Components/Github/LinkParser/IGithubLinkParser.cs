using ChatGPT.Plugins.Github.Models;

namespace ChatGPT.Plugins.Github.Components.Github.LinkParser;

internal interface IGithubLinkParser
{
    GithubLink Parse(string githubLink);
}
