// ====================================================
// Advanced Project Builder Tool - Unity Plugin
// Author: Dmytro Udovychenko
// Contact: https://www.linkedin.com/in/dmytro-udovychenko/
// License: MIT
// © 2025 Dmytro Udovychenko. All rights reserved.
// ====================================================

using UnityEngine;
using UnityEditor;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public static class AdvancedProjectBuilderUtility
    {
        public static bool EnsureFoldersExist(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return true;
            }
            
            // TODO: use System.IO.Directory.CreateDirectory(path, recursiveOrSOmething: true).
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
                        Debug.LogError($"Can't crate new folder: {currentPath}");
                    }
                }
            }

            return AssetDatabase.IsValidFolder(folderPath);
        }
    }
}