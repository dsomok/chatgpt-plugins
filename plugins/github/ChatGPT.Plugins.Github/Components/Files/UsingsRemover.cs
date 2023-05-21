using System.Text.RegularExpressions;

namespace ChatGPT.Plugins.Github.Components.Files;

// ReSharper disable once IdentifierTypo
internal class UsingsRemover : IFileContentProcessor
{
    public Task<string> ProcessFileContentAsync(string content, CancellationToken cancellationToken)
    {
        var result = Regex.Replace(content, @"^\s*using.+$", string.Empty, RegexOptions.Multiline);
        return Task.FromResult(result);
    }
}
