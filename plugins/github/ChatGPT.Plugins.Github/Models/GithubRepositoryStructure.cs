namespace ChatGPT.Plugins.Github.Models;

public record GithubRepositoryStructure(List<string> FilePaths)
{
    public IReadOnlyList<GithubFileMetadata> FilesMetadata  =>
        FilePaths.Select(filePath => new GithubFileMetadata(filePath)).ToList();

    public IReadOnlyList<GithubFileMetadata> RootFiles => FilesMetadata.Where(file => file.IsRootFile).ToList();

    public int FilePathsCharactersCount => 
        FilePaths.Sum(filePath => filePath.Length);

    public void AddFiles(IEnumerable<GithubFileMetadata> files)
    {
        var filesToAdd = files
                         .Select(file => file.Path)
                         .Where(path => !FilePaths.Contains(path));

        FilePaths.AddRange(filesToAdd);
    }
}
