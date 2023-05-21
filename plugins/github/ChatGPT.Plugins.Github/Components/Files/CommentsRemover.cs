using System.Text.RegularExpressions;

namespace ChatGPT.Plugins.Github.Components.Files;

internal class CommentsRemover : IFileContentProcessor
{
    public Task<string> ProcessFileContentAsync(string content, CancellationToken cancellationToken)
    {
        var result = Regex.Replace(content, @"\s*//.+$", string.Empty, RegexOptions.Multiline);
        return Task.FromResult(result);
    }
}
