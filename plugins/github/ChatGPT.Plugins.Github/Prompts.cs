namespace ChatGPT.Plugins.Github;

public static class Prompts
{
    public const string REPOSITORY_STRUCTURE_HINT =
        "You should thoroughly analyze the repository structure and try to make some assumptions on the content of each file. " +
        "Try to elaborate the files which contain examples. Consider them as highly relevant for users question. " +
        "If the repository structure is to large, you can query for the subfolders instead. " +
        "Query the code files first. After that do a separate query for markdown files with docs if they exist. NEVER query more than 20 files in a single query. " +
        "When requesting file contents, provide the github repository URL as well. Before querying the file path, make sure that it exists in the repository structure. " +
        "If error occures when querying file contents, inform user that an error ocurred and you are not able to generate the response. " +
        "Do not perform more than 3 retries of querying the repository structure.";

    public const string REPOSITORY_DIRECTORIES_STRUCTURE_HINT =
        "The response contains the list of directories the repository has. Also it lists the files in the root directory. " +
        "You are not yet ready to query the QueryGithubRepositoryFileContents endpoint. Instead you should once again query the structure providing the specific directories." +
        "You should thoroughly analyze the directories list and query this endpoint once again providing the refined list of directories you are interested in. " +
        "You can query for as much directories as you need. Prefer to query more directories over less. " +
        "To query the files of specified directories, you should use the same endpoint and provide the list via RelativePaths request property.";


    public const string QUERY_REPOSITORY_FILES =
        "When answering the new users question, you must request the repository structure once again prior requesting the contants of files. " +
        "Before giving an answer to the user, the relevance of the answer should be calculated, if it less than 0.7, then additional files should be requested. The process should be repeated until the relevance score is higher than 0.7.";

    public const string QUERY_REPOSITORY_FILES_ERROR =
        "Error occured, request once again the repository structure and fix the requested file paths. Respond that you've failed to retrieve files from the repository.";
}