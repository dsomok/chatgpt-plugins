using ChatGPT.Plugins.Github.Models;

namespace ChatGPT.Plugins.Github.Components.Repository;

public interface IRepositoryStructureReducer
{
    Task<GithubRepositoryStructure> ReduceRepositoryStructureAsync(GithubRepositoryStructure repositoryStructure, int maxCharacters, CancellationToken cancellationToken);
}