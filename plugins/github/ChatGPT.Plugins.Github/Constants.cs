namespace ChatGPT.Plugins.Github;

public static class Constants
{
    public const string REPOSITORY_STRUCTURE_HINT = "You should thoroughly analyze the repository structure and try to make some assumptions on the content of each file." +
                                                    "Create a plan to answer the users question. Then query the content of at least 15 files that are mostly relevant for each step of the plan." +
                                                    "Evaluate each file if it can contain an answer to user's question. If you think that you need more information, request additional files." +
                                                    "When requesting file contents, provide the github repository URL as well." +
                                                    "Do not request file paths that are not present in the repository structure";
}
