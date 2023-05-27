using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ChatGPT.Plugins.Github.Components.Github.FilesExtractor;
using ChatGPT.Plugins.Github.Configuration.Models;
using ChatGPT.Plugins.Github.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Octokit.Internal;
using Octokit;
using ChatGPT.Plugins.Github.Components.Github.LinkParser;

namespace ChatGPT.Plugins.Github.Benchmarks.Benchmarks;

[SimpleJob(RuntimeMoniker.Net70, warmupCount: 1, invocationCount: 5)]
public class GithubFilesEnumeratorBenchmark
{
    private GithubLink _githubLink;
    private IGithubFilesEnumerator _filesEnumerator;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.Configure<GithubConfiguration>(config =>
        {
            // TODO: Inject the github token
            config.Token = "";
        });

        services.AddSingleton<IGitHubClient>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<GithubConfiguration>>();
            var credentials = new InMemoryCredentialStore(new Credentials(config.Value.Token));

            return new GitHubClient(new ProductHeaderValue("dsomok"), credentials);
        });

        var serviceProvider = services.BuildServiceProvider();

        _githubLink = new GithubLinkParser().Parse("https://github.com/dotnet/BenchmarkDotNet");

        _filesEnumerator = new GithubFilesEnumerator(serviceProvider.GetRequiredService<IGitHubClient>());
    }

    [Benchmark]
    public async Task GithubFilesEnumerator()
    {
        await foreach (var _ in _filesEnumerator.EnumerateRepositoryFilesAsync(_githubLink, CancellationToken.None))
        {
            await Task.Yield();
        }
    }
}