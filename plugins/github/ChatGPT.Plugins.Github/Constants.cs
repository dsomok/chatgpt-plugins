namespace ChatGPT.Plugins.Github;

public static class Constants
{
    public const string REPOSITORY_STRUCTURE_HINT =
        "You should thoroughly analyze the repository structure and try to make some assumptions on the content of each file. " +
        "ALWAYS use only files from the repository structure and NEVER request files that are not present in the repository structure. " +
        "Create a plan to answer the users question. Then for each step of the plan query the content of 20 up to 40 most relevant files. " +
        "NEVER query less than 20 files. When querying, order files ascending by relevance. Query documentation files first and then files with code samples. " +
        "Prefer to query more files over querying less. Try to elaborate the files which contain examples. Consider them as highly relevant for users question. Look only for C# code." +
        "When requesting file contents, provide the github repository URL as well. Do not request file paths that are not present in the repository structure. " +
        "If error occures when querying file contents, inform user that an error ocurred and you are not able to generate the response. ";


    public const string QUERY_REPOSITORY_FILES =
        "When answering the new users question, you must request the repository structure once again prior requesting the contants of files. " +
        "If you are uncertain how to answer user's question or you need additional information you should query the contents of more files.";

    public const string QUERY_REPOSITORY_FILES_ERROR =
        "Error occured, request once again the repository structure and fix the requested file paths. Respond that you've failed to retrieve files from the repository.";
}
