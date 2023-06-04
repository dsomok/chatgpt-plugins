using ChatGPT.Plugins.Github.Models;

namespace ChatGPT.Plugins.Github.Components.Response;

internal class OrderingProcessor : IResponseProcessor
{
    public Task<IList<GithubFile>> ProcessResponseAsync(IList<GithubFile> files, CancellationToken cancellationToken)
    {
        var result = files.OrderBy(file => file.Path, new GithubFilePathComparer()).ToList();
        return Task.FromResult<IList<GithubFile>>(result);
    }

    private class GithubFilePathComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (IsCsharpFile(x))
            {
                return IsCsharpFile(y) ? 0 : 1;
            }

            if (IsMarkdownFile(x))
            {
                return IsCsharpFile(y)
                    ? -1
                    : IsMarkdownFile(y) ? 0 : 1;
            }

            return 0;
        }

        private bool IsCsharpFile(string path) => path.EndsWith(".cs");
        private bool IsMarkdownFile(string path) => path.EndsWith(".md");
    }
}
