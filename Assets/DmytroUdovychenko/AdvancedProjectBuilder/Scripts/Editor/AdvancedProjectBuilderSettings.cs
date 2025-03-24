// ====================================================
// Advanced Project Builder Tool - Unity Plugin
// Author: Dmytro Udovychenko
// Contact: https://www.linkedin.com/in/dmytro-udovychenko/
// License: MIT
// © 2025 Dmytro Udovychenko. All rights reserved.
// ====================================================

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public enum BuildConfigType
    {
        None,
        Dev,
        Staging,
        Release
    }
    
    public static class AdvancedProjectBuilderSettings
    {
        public const string Name = "AdvancedProjectBuilder";
        public const string DebugName = "[" + Name +"]";
        public const string FolderPath = "Assets/DmytroUdovychenko/" + Name;
        public const string ConfigFolderName = "Configs";
        
        public const string MenuButtonPath = "Tools/DmytroUdovychenko/" + Name;
        public const string NewConfigName  = "NewBuildToolSettings";
        public const string PathSettingId  = "SavePath";
        
        //Project Settings properties
        public const string ProjectSettingsPath      = "ProjectSettings/ProjectSettings.asset";
        public const string CloudProjectIDProperty       = "cloudProjectId";
        public const string CloudOrganizationIDProperty  = "organizationId";
        public const string CloudProjectNameProperty     = "projectName";
        public const string ProductNameProperty          = "productName";
        
        public static string GetSavedPathId()
        {
            return PathSettingId + Application.dataPath;
        }
        
        /// <summary>
        /// Returns the path to the configurations folder.
        /// </summary>
        /// <returns>The configurations folder path.</returns>
        public static string GetConfigFolder() // could be expression body  GetConfigFolder() => $"xxx";
        {
            return $"{FolderPath}/{ConfigFolderName}";
        }
        
        /// <summary>
        /// Searches for all BuildToolSettings objects in the specified folder (or the entire project, if needed).
        /// </summary>
        /// <returns>The list of BuildToolSettings found.</returns>
        public static List<AdvancedProjectBuilderConfig> FindAllBuildToolSettings()
        {
            List<AdvancedProjectBuilderConfig> foundSettings = new List<AdvancedProjectBuilderConfig>();
            string configFolder = GetConfigFolder();
            string lookupType = nameof(AdvancedProjectBuilderConfig);
            
            string[] guids = string.IsNullOrEmpty(configFolder)
                ? AssetDatabase.FindAssets($"t:{lookupType}")
                : AssetDatabase.FindAssets($"t:{lookupType}", new[] { configFolder });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                AdvancedProjectBuilderConfig settings = AssetDatabase.LoadAssetAtPath<AdvancedProjectBuilderConfig>(assetPath);
                if (settings != null)
                {
                    foundSettings.Add(settings);
                }
            }

            return foundSettings;
        }
    }
}