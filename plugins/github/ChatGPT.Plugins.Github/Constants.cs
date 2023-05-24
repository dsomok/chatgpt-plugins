namespace ChatGPT.Plugins.Github;

public static class Constants
{
    public const string REPOSITORY_STRUCTURE_HINT = "You should thoroughly analyze the repository structure to try to make some assumptions on the content of each file." +
                                                    "To answer the users question select at least 15 most relevant files. Evaluate each file if it can contain an answer to user's question." +
                                                    "Do not request file paths that are not present in the repository structure";
}
