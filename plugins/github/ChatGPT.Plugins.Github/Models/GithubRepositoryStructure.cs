namespace ChatGPT.Plugins.Github.Models;

public record GithubRepositoryStructure(IList<string> FilePaths)
{
    public IList<GithubFileMetadata> FilesMetadata => FilePaths.Select(filePath => new GithubFileMetadata(filePath)).ToList();

    public int FilePathsCharactersCount => FilePaths.Sum(filePath => filePath.Length);
}
