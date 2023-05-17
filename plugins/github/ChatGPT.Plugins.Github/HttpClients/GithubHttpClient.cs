namespace ChatGPT.Plugins.Github.HttpClients;

public class GithubHttpClient : IGithubHttpClient
{
    private readonly HttpClient _httpClient;

    public GithubHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetRawContentAsync(string relativeUrl, CancellationToken cancellationToken)
    {
        relativeUrl = relativeUrl.Replace("/blob", string.Empty);

        var response = await _httpClient.GetAsync(relativeUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
