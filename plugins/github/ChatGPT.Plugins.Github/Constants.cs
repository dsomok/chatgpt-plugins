namespace ChatGPT.Plugins.Github;

public static class Constants
{
    public const string REPOSITORY_STRUCTURE_HINT =
        "You should thoroughly analyze the repository structure and try to make some assumptions on the content of each file." +
        "Create a plan to answer the users question. Then query the content of 20 up to 40 files that are mostly relevant for each step of the plan." +
        "Queried files count must not be less than 20. Prefer to query more files over querying less so you'll have more information to respond the users question." +
        "Evaluate each file if it contains an answer to user's question. If you think that you need more information, send one more request to query additional files." +
        "When requesting file contents, provide the github repository URL as well. Do not request file paths that are not present in the repository structure";


    public const string QUERY_REPOSITORY_FILES =
        "When answering the new users question, you must request the repository structure once again prior requesting the contants of files. " +
        "If you are uncertain how to answer user's question or you need additional information you should query the contents of more files.";
}
