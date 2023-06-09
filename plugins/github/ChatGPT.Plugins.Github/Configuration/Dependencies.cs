﻿using System.Runtime.CompilerServices;
using ChatGPT.Plugins.Github.Components;
using ChatGPT.Plugins.Github.Configuration.Models;
using ChatGPT.Plugins.Github.Handlers;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Octokit;
using Octokit.Internal;

[assembly:InternalsVisibleTo("ChatGPT.Plugins.Github.Benchmarks")]

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

        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GithubRepositoryFilesRequestHandler).Assembly));
        services.AddLazyCache();

        services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = false;
                });

        services.AddEndpointsApiExplorer()
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(Constants.Version, new OpenApiInfo
                    {
                        Version = Constants.Version,
                        Title = "AskTheCode Plugin",
                        Description = "Plugin to explain the code from Github and assist with its usage. Works with the provided Github file link."
                    });

                    options.EnableAnnotations();
                });

        services.AddComponents();

        return services;
    }
}
