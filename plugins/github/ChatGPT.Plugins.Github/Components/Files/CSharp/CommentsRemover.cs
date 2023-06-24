using System.Text.RegularExpressions;

namespace ChatGPT.Plugins.Github.Components.Files.CSharp;

internal class CommentsRemover : IFileContentProcessor
{
    public bool CanProcessFile(string fullPath)
    {
        return fullPath.EndsWith(Constants.Csharp.CodeFileExtension);
    }

    public Task<string> ProcessFileContentAsync(string content, CancellationToken cancellationToken)
    {
        var result = Regex.Replace(content, @"\s*//.+$", string.Empty, RegexOptions.Multiline);
        return Task.FromResult(result);
    }
}
