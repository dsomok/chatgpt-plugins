﻿namespace ChatGPT.Plugins.Github;

public static class Constants
{
    public const string REPOSITORY_STRUCTURE_HINT =
        "You should thoroughly analyze the repository structure and try to make some assumptions on the content of each file. " +
        "ALWAYS use only files from the repository structure and NEVER request files that are not present in the repository structure. " +
        "Try to elaborate the files which contain examples. Consider them as highly relevant for users question. " +
        "Query the code files first. After that do a separate query for markdown files with docs if they exist. " +
        "When requesting file contents, provide the github repository URL as well. Do not request file paths that are not present in the repository structure. " +
        "If error occures when querying file contents, inform user that an error ocurred and you are not able to generate the response. ";


    public const string QUERY_REPOSITORY_FILES =
        "When answering the new users question, you must request the repository structure once again prior requesting the contants of files. " +
        "Before giving an answer to the user, the relevance of the answer should be calculated, if it less than 0.7, then additional files should be requested. The process should be repeated until the relevance score is higher than 0.7.";

    public const string QUERY_REPOSITORY_FILES_ERROR =
        "Error occured, request once again the repository structure and fix the requested file paths. Respond that you've failed to retrieve files from the repository.";
}
