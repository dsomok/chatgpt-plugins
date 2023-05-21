using System.Text.RegularExpressions;

namespace ChatGPT.Plugins.Github.Components.Files;

internal class FileContentMinifier : IFileContentProcessor
{
    public Task<string> ProcessFileContentAsync(string content, CancellationToken cancellationToken)
    {
        var result = content.Replace(Environment.NewLine, string.Empty)
                            .Replace("\r", string.Empty)
                            .Replace("\n", string.Empty);

        result = Regex.Replace(result, @"\s+", " ");

        return Task.FromResult(result);
    }
}
