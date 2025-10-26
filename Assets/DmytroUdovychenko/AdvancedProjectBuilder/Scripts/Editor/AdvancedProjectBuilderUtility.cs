// ====================================================
// Advanced Project Builder Tool - Unity Plugin
// Author: Dmytro Udovychenko
// Contact: https://www.linkedin.com/in/dmytro-udovychenko/
// License: MIT
// © 2025 Dmytro Udovychenko. All rights reserved.
// ====================================================

using System.IO;
using UnityEngine;
using UnityEditor;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public static class AdvancedProjectBuilderUtility
    {
        /// <summary>
        /// Ensures that the specified folder path exists in the Unity project.
        /// Creates any missing folders in the path.
        /// </summary>
        /// <param name="folderPath">The folder path relative to the Assets folder</param>
        /// <returns>True if the folder exists or was successfully created</returns>
        public static bool EnsureFoldersExist(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return true;
            }

            // Ensure path starts with Assets/
            if (!folderPath.StartsWith("Assets/"))
            {
                folderPath = $"Assets/{folderPath}";
            }

            string[] folders = folderPath.Split('/');
            string currentPath = folders[0];

            for (int i = 1; i < folders.Length; i++)
            {
                string folder = folders[i];
                string parentPath = currentPath;
                currentPath = $"{currentPath}/{folder}";

                if (!AssetDatabase.IsValidFolder(currentPath))
                {
                    string newFolderPath = AssetDatabase.CreateFolder(parentPath, folder);
                    if (string.IsNullOrEmpty(newFolderPath))
                    {
                        Debug.LogError($"Can't create new folder: {currentPath}");
                        return false;
                    }
                }
            }

            return AssetDatabase.IsValidFolder(folderPath);
        }

        /// <summary>
        /// Creates a directory and all parent directories in the file system.
        /// </summary>
        /// <param name="directoryPath">The full directory path</param>
        /// <returns>True if the directory exists or was successfully created</returns>
        public static bool EnsureDirectoryExists(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    return true;
                }

                Directory.CreateDirectory(directoryPath);
                return Directory.Exists(directoryPath);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create directory: {directoryPath}. Error: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ensures both Unity project folders and filesystem directories exist.
        /// Useful for build paths that need to exist both in the project and file system.
        /// </summary>
        /// <param name="projectPath">Path relative to Assets folder</param>
        /// <param name="absolutePath">Full filesystem path</param>
        /// <returns>True if both paths were created successfully</returns>
        public static bool EnsureProjectAndFileSystemPathsExist(string projectPath, string absolutePath)
        {
            bool projectSuccess    = EnsureFoldersExist(projectPath);
            bool fileSystemSuccess = EnsureDirectoryExists(absolutePath);

            return projectSuccess && fileSystemSuccess;
        }

        /// <summary>
        /// Combines multiple path segments into a single path with correct separators
        /// for the current platform.
        /// </summary>
        /// <param name="pathSegments">Path segments to combine</param>
        /// <returns>Combined path</returns>
        public static string CombinePath(params string[] pathSegments)
        {
            return Path.Combine(pathSegments);
        }
    }
}