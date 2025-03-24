// ====================================================
// Advanced Project Builder Tool - Unity Plugin
// Author: Dmytro Udovychenko
// Contact: https://www.linkedin.com/in/dmytro-udovychenko/
// License: MIT
// © 2025 Dmytro Udovychenko. All rights reserved.
// ====================================================

using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public partial class AdvancedProjectBuilder
    {
        public static void Build(AdvancedProjectBuilderConfig buildBuilderConfig)
        {
            if (buildBuilderConfig == null)
            {
                LogError("Export settings are not specified or there are no assets to export.");
                return;
            }

            PrepareBuild(buildBuilderConfig);
        }
        
        private static void PrepareBuild(AdvancedProjectBuilderConfig settings)
        {
            EnsurePlatformIsAndroid(settings);
            SetProjectName(settings);
            SetDefineSymbols(settings);
            SetUnityCloudId(settings);
            SetBundleId(settings);
            SetBuildVersion(settings);
            
            SetPlatformSpecifics(settings);
            
            if (string.IsNullOrEmpty(settings.BuildPath))
            {
                LogError("Build path is empty. Please specify a valid file path.");
                return;
            }

            string buildPath;
            
            if (settings.IsCommandLineBuild == false)
            {
                string version  = PlayerSettings.bundleVersion;
                string buildNumber = GetBuildNumber(settings);
                string folderPath = $"{settings.BuildPath}/{settings.BuildTarget.ToString()}";
                string fileExtension = GetFileExtension(settings);
                buildPath = $"{folderPath}/{PlayerSettings.productName}_v{version}({buildNumber})_{settings.BuildConfigType}{fileExtension}";
                LogMessage($"BuildPath: {buildPath}");
            }
            else
            {
                buildPath = settings.BuildPath;
                LogMessage($"BuildPath: {settings.BuildPath}");
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = GetScenes(),
                locationPathName = buildPath,
                target  = settings.BuildTarget,
                options = settings.BuildOptions
            };

            StartBuild(buildPlayerOptions);
        }
        
        private static void StartBuild(BuildPlayerOptions buildPlayerOptions)
        {
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                LogMessage($"Build time - {summary.totalTime.Hours}h:{summary.totalTime.Minutes}m:{summary.totalTime.Seconds}s");
                OpenBuildFolder(buildPlayerOptions.locationPathName);
            }
            
            if (summary.result == BuildResult.Failed)
            {
                LogError("Build failed");
            }
        }
        
        private static void EnsurePlatformIsAndroid(AdvancedProjectBuilderConfig settings)
        {
            if (settings.BuildTarget != EditorUserBuildSettings.activeBuildTarget)
            {
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(settings.BuildTarget);
                
                EditorUserBuildSettings.SwitchActiveBuildTarget(group, settings.BuildTarget);
                LogMessage($"Switched platform to {settings.BuildTarget}.");
            }
        }

        private static void SetPlatformSpecifics(AdvancedProjectBuilderConfig settings)
        {
            switch (settings.BuildTarget)
            {
                case BuildTarget.Android:
                    SetPlatformSpecificsAndroid(settings);
                    break;
                case BuildTarget.StandaloneOSX:
                    SetPlatformSpecificsIos(settings);
                    break;
                default:
                    break;
            }
        }
        
        private static void SetUnityCloudId(AdvancedProjectBuilderConfig settings)
        {
            if (settings.OverrideUnityServiceId)
            {
                SerializedObject projectSettings        = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(AdvancedProjectBuilderSettings.ProjectSettingsPath)[0]);
                SerializedProperty projectIDProp        = projectSettings.FindProperty(AdvancedProjectBuilderSettings.CloudProjectIDProperty);
                SerializedProperty organizationProperty = projectSettings.FindProperty(AdvancedProjectBuilderSettings.CloudOrganizationIDProperty);
                SerializedProperty productNameProperty  = projectSettings.FindProperty(AdvancedProjectBuilderSettings.CloudProjectNameProperty);
                
                projectIDProp.stringValue        = settings.UnityProjectId;
                organizationProperty.stringValue = settings.UnityProjectOrganizationId;
                productNameProperty.stringValue  = settings.UnityProjectName;
                projectSettings.ApplyModifiedProperties();
                
                LogMessage($"Set UnityCloudId: '{settings.UnityProjectId}' / '{settings.UnityProjectOrganizationId}' / '{settings.UnityProjectName}'.");
            }
        }

        private static void SetBundleId(AdvancedProjectBuilderConfig settings)
        {
            if (settings.OverrideBundleId)
            {
                PlayerSettings.applicationIdentifier = settings.BundleId;
                LogMessage($"Set applicationIdentifier: {settings.BundleId}");
            }
        }
        
        private static void SetBuildVersion(AdvancedProjectBuilderConfig settings)
        {
            if (!string.IsNullOrEmpty(settings.BuildVersion))
            {
                PlayerSettings.bundleVersion = settings.BuildVersion;
                LogMessage($"Set BuildVersion: {settings.BuildVersion}");
            }
            
            if (!string.IsNullOrEmpty(settings.BundleVersionNumber))
            {
                if (settings.BuildTarget == BuildTarget.Android)
                {
                    int bundleCode = int.Parse(settings.BundleVersionNumber);
                    PlayerSettings.Android.bundleVersionCode = bundleCode;
                    LogMessage($"Set Android bundleVersionCode: {bundleCode}");
                }
                
                if (settings.BuildTarget == BuildTarget.iOS)
                {
                    PlayerSettings.iOS.buildNumber = settings.BundleVersionNumber;
                    LogMessage($"Set iOS buildNumber: {settings.BundleVersionNumber}");
                }
                
                if (settings.BuildTarget == BuildTarget.StandaloneOSX)
                {
                    PlayerSettings.macOS.buildNumber = settings.BundleVersionNumber;
                    LogMessage($"Set Macos buildNumber: {settings.BundleVersionNumber}");
                }
            }
        }

        private static string GetBuildNumber(AdvancedProjectBuilderConfig settings)
        {
            string buildNumber = string.Empty;
            
            if (settings.BuildTarget == BuildTarget.Android)
            {
                buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
            }
                
            if (settings.BuildTarget == BuildTarget.iOS)
            {
                buildNumber = PlayerSettings.iOS.buildNumber;
            }
                
            if (settings.BuildTarget == BuildTarget.StandaloneOSX)
            {
                buildNumber = PlayerSettings.macOS.buildNumber;
            }
            
            return buildNumber;
        }
        
        private static void SetProjectName(AdvancedProjectBuilderConfig settings)
        {
            if (settings.OverrideProductName)
            {
                SerializedObject projectSettings   = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(AdvancedProjectBuilderSettings.ProjectSettingsPath)[0]);
                SerializedProperty projectNameProp = projectSettings.FindProperty(AdvancedProjectBuilderSettings.ProductNameProperty);
                
                projectNameProp.stringValue = settings.ProductName;
                projectSettings.ApplyModifiedProperties();
                
                LogMessage($"Set ProjectName: '{settings.ProductName}'");
            }
        }

        public static void SetDefineSymbols(BuildTargetGroup targetGroup, string[] define)
        {
            if (define == null)
            {
                LogError($"{AdvancedProjectBuilderSettings.DebugName}: define == NULL.");
                return;
            }
            string defineSymbols = string.Join(";", define);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbols);
            LogMessage($"Set DefineSymbols: {defineSymbols}");
        }
        
        private static void SetDefineSymbols(AdvancedProjectBuilderConfig settings)
        {
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(settings.BuildTarget);
            SetDefineSymbols(targetGroup, settings.DefineSymbols);
        }
        
        private static void OpenBuildFolder(string path)
        {
            string folderPath = System.IO.Path.GetDirectoryName(path);
            EditorUtility.RevealInFinder(folderPath);
        }

        private static string[] GetScenes()
        {
            return EditorBuildSettings.scenes.Where(
                scene => scene.enabled).Select(scene => scene.path).ToArray();
        }

        private static string GetFileExtension(AdvancedProjectBuilderConfig settings)
        {
            switch (settings.BuildTarget)
            {
                case BuildTarget.Android:
                    if (EditorUserBuildSettings.buildAppBundle)
                        return ".aab";
                    else
                        return ".apk";

                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return ".exe";

                case BuildTarget.StandaloneOSX:
                    return ".app";

                case BuildTarget.StandaloneLinux64:
                    return ".x86_64";

                default:
                    return "";
            }
        }

        public static void LogMessage(string message)
        {
            Debug.Log($"{AdvancedProjectBuilderSettings.DebugName} - {message}");
        }

        public static void LogError(string message)
        {
            Debug.LogError($"{AdvancedProjectBuilderSettings.DebugName} - {message}");
        }
    }
}