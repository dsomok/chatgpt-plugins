using ChatGPT.Plugins.Github.Handlers;
using ChatGPT.Plugins.Github.HttpClients;
using Microsoft.OpenApi.Models;

namespace ChatGPT.Plugins.Github.Configuration;

public static class Dependencies
{
    public static IServiceCollection RegisterDependencies(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHttpClient<IGithubHttpClient, GithubHttpClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri("https://raw.githubusercontent.com");
        });

        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(QueryGithubRequestHandler).Assembly));

        return services.AddEndpointsApiExplorer()
                       .AddSwaggerGen(options =>
                       {
                           options.SwaggerDoc("v1", new OpenApiInfo
                           {
                               Version = "v1",
                               Title = "Github Plugin",
                               Description = "Plugin to explain the code from Github and assist with its usage. Works with the provided Github file link."
                           });
                       });
    }
}
