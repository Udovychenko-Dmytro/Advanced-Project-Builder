// ====================================================
// Advanced Project Builder Tool - Unity Plugin
// Author: Dmytro Udovychenko
// Contact: https://www.linkedin.com/in/dmytro-udovychenko/
// License: MIT
// © 2025 Dmytro Udovychenko. All rights reserved.
// ====================================================

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public partial class AdvancedProjectBuilder
    {
        private static IPlatformSpecifics platformBase = new BasePlatformExtension();

        public static void Build(AdvancedProjectBuilderConfig buildBuilderConfig)
        {
            if (buildBuilderConfig == null)
            {
                LogError("Export settings are not specified or there are no assets to export.");
                return;
            }

            PrepareBuild(buildBuilderConfig);
        }

        private static void PrepareBuild(AdvancedProjectBuilderConfig config)
        {
            EnsurePlatformIsCorrect(config);
            SetPlatformBase(config);
            SetPlatformSpecifics(config);

            if (string.IsNullOrEmpty(config.BuildPath))
            {
                LogError("Build path is empty. Please specify a valid file path.");
                return;
            }

            string buildPath;
            if (config.IsCommandLineBuild == false)
            {
                string version       = PlayerSettings.bundleVersion;
                string buildNumber   = GetBuildNumber(config);
                string folderPath    = $"{config.BuildPath}/{config.BuildTarget.ToString()}";
                string fileExtension = GetFileExtension(config);
                buildPath = $"{folderPath}/{PlayerSettings.productName}_v{version}({buildNumber})_{config.BuildConfigType}{fileExtension}";
                LogMessage($"BuildPath: {buildPath}");
            }
            else
            {
                buildPath = config.BuildPath;
                LogMessage($"BuildPath: {config.BuildPath}");
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes  = GetScenes(),
                target  = config.BuildTarget,
                options = config.BuildOptions,
                locationPathName = buildPath
            };

            StartBuild(buildPlayerOptions, config);
        }

        private static void StartBuild(BuildPlayerOptions buildPlayerOptions, AdvancedProjectBuilderConfig config)
        {
            BuildReport report   = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                LogMessage($"Build time - {summary.totalTime.Hours}h:{summary.totalTime.Minutes}m:{summary.totalTime.Seconds}s");
                TriggerPostBuildProcess(buildPlayerOptions, report, config);
                OpenBuildFolder(buildPlayerOptions.locationPathName);
            }
            else if (summary.result == BuildResult.Failed)
            {
                LogError("Build failed");
            }
        }

        private static void EnsurePlatformIsCorrect(AdvancedProjectBuilderConfig settings)
        {
            if (settings.BuildTarget != EditorUserBuildSettings.activeBuildTarget)
            {
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(settings.BuildTarget);
                EditorUserBuildSettings.SwitchActiveBuildTarget(group, settings.BuildTarget);
                LogMessage($"Switched platform to {settings.BuildTarget}.");
            }
        }

        private static void SetPlatformBase(AdvancedProjectBuilderConfig settings)
        {
            platformBase.SetPlatformSpecifics(settings);
        }

        private static void SetPlatformSpecifics(AdvancedProjectBuilderConfig settings)
        {
            IPlatformSpecifics platformSpecifics = AdvancedProjectBuilderSettings.CreatePlatformSpecifics(settings.BuildTarget);

            if (platformSpecifics != null)
            {
                platformSpecifics.SetPlatformSpecifics(settings);
            }
        }

        private static string GetBuildNumber(AdvancedProjectBuilderConfig settings)
        {
            string buildNumber = string.Empty;
            switch (settings.BuildTarget)
            {
                case BuildTarget.Android:
                    buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
                    break;
                case BuildTarget.iOS:
                    buildNumber = PlayerSettings.iOS.buildNumber;
                    break;
                case BuildTarget.StandaloneOSX:
                    buildNumber = PlayerSettings.macOS.buildNumber;
                    break;
            }
            return buildNumber;
        }

        private static void OpenBuildFolder(string path)
        {
            string folderPath = Path.GetDirectoryName(path);
            EditorUtility.RevealInFinder(folderPath);
        }

        private static void TriggerPostBuildProcess(BuildPlayerOptions buildPlayerOptions, BuildReport report, AdvancedProjectBuilderConfig config)
        {
            LogMessage("Triggering post-build process...");
            string path = report.summary.outputPath;

            //TODO: move to platform specifics
            if (   report.summary.platform == BuildTarget.StandaloneWindows
                || report.summary.platform == BuildTarget.StandaloneWindows64)
            {
                DirectoryInfo directory = Directory.GetParent(path);
                if (directory != null)
                {
                    path = directory.FullName;
                }
            }

            TriggerPostBuildProcessPlatformSpecifics(buildPlayerOptions, report, config);
            CreateArchive(config, path);
        }

        private static void TriggerPostBuildProcessPlatformSpecifics(BuildPlayerOptions buildPlayerOptions,
                                                                     BuildReport        report,
                                                                     AdvancedProjectBuilderConfig config)
        {
            try
            {
                IPlatformSpecifics platformSpecifics = AdvancedProjectBuilderSettings.CreatePlatformSpecifics(report.summary.platform);
                if (platformSpecifics != null)
                {
                    platformSpecifics.OnPostprocessBuild(buildPlayerOptions, report, config);
                }
                else
                {
                    LogWarning($"No platform specifics found for '{report.summary.platform}'.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void CreateArchive(AdvancedProjectBuilderConfig config, string path)
        {
            if (config.CreateZip)
            {
                ArchiveUtility.CreateArchive(path);
            }
        }

        private static string[] GetScenes()
        {
            return EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        }

        private static string GetFileExtension(AdvancedProjectBuilderConfig settings)
        {
            switch (settings.BuildTarget)
            {
                case BuildTarget.Android:
                    return EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return $"/{Application.productName}.exe";
                case BuildTarget.StandaloneOSX:
                    return ".app";
                case BuildTarget.StandaloneLinux64:
                    return ".x86_64";
                default:
                    LogWarning($"File extension not defined for platform {settings.BuildTarget}");
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

        public static void LogWarning(string message)
        {
            Debug.LogWarning($"{AdvancedProjectBuilderSettings.DebugName} - {message}");
        }
    }
}