using ChatGPT.Plugins.Github.Components.Github;
using ChatGPT.Plugins.Github.Configuration.Models;
using ChatGPT.Plugins.Github.Handlers;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Octokit;
using Octokit.Internal;

namespace ChatGPT.Plugins.Github.Configuration;

public static class Dependencies
{
    public static IServiceCollection RegisterDependencies(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<GithubConfiguration>(configuration.GetSection("Github"));
        services.AddSingleton<IGitHubClient>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<GithubConfiguration>>();
            var credentials = new InMemoryCredentialStore(new Credentials(config.Value.Token));

            return new GitHubClient(new ProductHeaderValue("dsomok"), credentials);
        });

        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(QueryGithubRequestHandler).Assembly));

        services.AddEndpointsApiExplorer()
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Github Plugin",
                        Description = "Plugin to explain the code from Github and assist with its usage. Works with the provided Github file link."
                    });
                });

        services.AddTransient<IGithubFilesExtractor, GithubFilesExtractor>();

        return services;
    }
}
