namespace ChatGPT.Plugins.Github.Models.DTO.Requests;

public class QueryRequest
{
    public string Url { get; set; }

    public List<string> FilePaths { get; set; }
}
