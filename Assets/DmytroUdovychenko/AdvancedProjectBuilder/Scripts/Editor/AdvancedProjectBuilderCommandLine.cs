// ====================================================
// Advanced Project Builder Tool - Unity Plugin
// Author: Dmytro Udovychenko
// Contact: https://www.linkedin.com/in/dmytro-udovychenko/
// License: MIT
// © 2025 Dmytro Udovychenko. All rights reserved.
// ====================================================

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public static class AdvancedProjectBuilderCommandLine
    {
        private const string ArgumentStartSign = "-";
        private const string ArgumentEqualSign = "=";
        private const string ArgumentSplitSign = ";";

        private const string BuildConfigName = "BUILD_CONFIGURATION_NAME";
        private const string BuildPath = "BUILD_PATH";
        private const string BuildPlatformParam = "BUILD_PLATFORM";
        private const string BuildOptionsParam = "BUILD_OPTION";
        private const string BuildDefines = "BUILD_DEFINES";
        
        private const string BundleId = "BUNDLE_ID";
        private const string BuildVersion = "BUILD_VERSION";
        private const string BundleVersionNumber = "BUILD_VERSION_NUMBER";
        private const string ProductName = "BUILD_PRODUCT_NAME";
        
        private const string OverrideUnityServiceId = "UNITY_SERVICE_ID_OVERRIDE";
        private const string UnityProjectId = "UNITY_SERVICE_PROJECT_ID";
        private const string UnityProjectOrganizationId = "UNITY_SERVICE_ORGANIZATION_ID";
        
        private const string AndroidBuildAppBundle = "ANDROID_APP_BUNDLE";
        private const string AndroidSplitAppBinary = "ANDROID_SPLIT_BINARY";
        private const string AndroidUseKeystore  = "ANDROID_USE_KEYSTORE";
        private const string AndroidKeystorePath = "ANDROID_KEYSTORE_PATH";
        private const string AndroidKeystorePass = "ANDROID_KEYSTORE_PASS";
        private const string AndroidKeyaliasName = "ANDROID_KEYALIAS_NAME";
        private const string AndroidKeyaliasPass = "ANDROID_KEYALIAS_PASS";
        
        private const string AppleDeveloperTeamID = "APPLE_DEVELOPER_TEAM_ID";
        
        private static Dictionary<string, string> commandLineArguments = new Dictionary<string, string>();
        
        public static void Build()
        {
            commandLineArguments = GetCommandlineArguments();
            string buildConfiguration = GetParamValue(BuildConfigName);

            TryGetBuildConfigByName(buildConfiguration, out AdvancedProjectBuilderConfig buildConfig);

            if (buildConfig == null)
            {
                buildConfig = ScriptableObject.CreateInstance<AdvancedProjectBuilderConfig>();
                buildConfig.name = "Custom";
                AdvancedProjectBuilder.LogMessage($"Create New Build Config:`{buildConfiguration}`");
            }
            else
            {
                AdvancedProjectBuilder.LogMessage($"Config `{buildConfiguration}` found");
            }
            
            AdvancedProjectBuilder.LogMessage($"BuildConfig: {buildConfig.name}. Updating build configuration from command line arguments.");
            
            UpdateBuildConfig(buildConfig);
            
            AdvancedProjectBuilder.LogMessage("BUILD");
            
            AdvancedProjectBuilder.Build(buildConfig);
        }

        private static bool TryGetBuildConfigByName(string targetConfigName, out AdvancedProjectBuilderConfig targetBuilderConfig)
        {
            List<AdvancedProjectBuilderConfig> configList = AdvancedProjectBuilderSettings.FindAllBuildToolSettings();
            
            foreach (AdvancedProjectBuilderConfig config in configList)
            {
                if (config.name == targetConfigName)
                {
                    targetBuilderConfig = config;
                    return true;
                }
            }

            targetBuilderConfig = null;
            return false;
        }

        private static Dictionary<string, string>  GetCommandlineArguments()
        {
            string[] commandLineRawArguments = Environment.GetCommandLineArgs();
            
            Dictionary<string, string> arguments = new Dictionary<string, string>();

            foreach (string argument in commandLineRawArguments)
            {
                if (argument.StartsWith(ArgumentStartSign) && argument.Contains(ArgumentEqualSign))
                {
                    //remove "-"
                    string noDashArgument = argument.Substring(1);

                    // Split by "="
                    string[] keyValuePair = noDashArgument.Split(ArgumentEqualSign);
                    
                    if (keyValuePair.Length == 2)
                    {
                        string key = keyValuePair[0];
                        string value = keyValuePair[1];

                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                        {
                            arguments[key] = value;
                            
                            if (key.Contains("PASS"))
                            {
                                AdvancedProjectBuilder.LogMessage($"CommandLine Argument Found:{key}=***");
                            }
                            else
                            {
                                AdvancedProjectBuilder.LogMessage($"CommandLine Argument Found:{key}={value}");
                            }
                        }
                    }
                }
            }

            return arguments;
        }

        private static string GetParamValue(string argumentName)
        {
            commandLineArguments.TryGetValue(argumentName, out string argumentValue);
            return argumentValue;
        }

        private static void UpdateBuildConfig(AdvancedProjectBuilderConfig buildBuilderConfig)
        {
            buildBuilderConfig.BuildPath = GetParamValue(BuildPath);
            buildBuilderConfig.IsCommandLineBuild = true;
            AdvancedProjectBuilder.LogMessage($"BuildPath: '{buildBuilderConfig.BuildPath}'");

            SetBuildTarget(buildBuilderConfig);
            SetBuildOptions(buildBuilderConfig);
            SetDefines(buildBuilderConfig);
            SetProductName(buildBuilderConfig);
            SetBundleId(buildBuilderConfig);
            SetBuildVersion(buildBuilderConfig);
            SetUnityServiceId(buildBuilderConfig);
            UpdateAndroidBuildConfig(buildBuilderConfig);
            UpdateIosBuildConfig(buildBuilderConfig);
        }

        private static void SetBuildTarget(AdvancedProjectBuilderConfig buildBuilderConfig)
        {
            string buildTargetRaw = GetParamValue(BuildPlatformParam);
            AdvancedProjectBuilder.LogMessage($"BuildPlatform: '{buildTargetRaw}'");

            if (!string.IsNullOrEmpty(buildTargetRaw))
            {
                if (Enum.TryParse<BuildTarget>(buildTargetRaw, true, out BuildTarget parsedTarget))
                {
                    AdvancedProjectBuilder.LogMessage($"BuildPlatform SET successfully: '{parsedTarget}'");
                    buildBuilderConfig.BuildTarget = parsedTarget;
                }
                else
                {
                    AdvancedProjectBuilder.LogMessage($"Invalid BuildTarget: '{buildTargetRaw}'");
                }
            }
        }

        private static void SetDefines(AdvancedProjectBuilderConfig buildBuilderConfig)
        {
            string buildDefinesRaw = GetParamValue(BuildDefines);

            if (string.IsNullOrEmpty(buildDefinesRaw))
            {
                return;
            }
            
            string[] definesList = SplitParameter(buildDefinesRaw);
            
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildBuilderConfig.BuildTarget);
            AdvancedProjectBuilder.SetDefineSymbols(targetGroup, definesList);
            AdvancedProjectBuilder.LogMessage($"Define: '{GetOnStringList(definesList)}'");
        }
        
        private static void SetProductName(AdvancedProjectBuilderConfig buildBuilderConfig)
        {
            string productName = GetParamValue(ProductName);

            if (!string.IsNullOrEmpty(productName))
            {
                buildBuilderConfig.OverrideProductName = true;
                buildBuilderConfig.ProductName = productName;
            
                AdvancedProjectBuilder.LogMessage($"Product Name: '{productName}'");
            }
        }
        
        private static void SetBundleId(AdvancedProjectBuilderConfig buildBuilderConfig)
        {
            string bundleId = GetParamValue(BundleId);

            if (!string.IsNullOrEmpty(bundleId))
            {
                buildBuilderConfig.OverrideBundleId = true;
                buildBuilderConfig.BundleId = bundleId;
            
                AdvancedProjectBuilder.LogMessage($"Bundle ID: '{bundleId}'");
            }
        }
        
        private static void SetBuildVersion(AdvancedProjectBuilderConfig buildBuilderConfig)
        {
            string buildVersion = GetParamValue(BuildVersion);
            string bundleVersionNumber = GetParamValue(BundleVersionNumber);

            if (string.IsNullOrEmpty(buildVersion))
            {
                return;
            }

            buildBuilderConfig.BuildVersion = buildVersion;
            AdvancedProjectBuilder.LogMessage($"BuildVersion: '{buildVersion}'");
            
            if (string.IsNullOrEmpty(bundleVersionNumber))
            {
                return;
            }

            buildBuilderConfig.BundleVersionNumber = bundleVersionNumber;
            AdvancedProjectBuilder.LogMessage($"BundleVersionNumber: '{bundleVersionNumber}'");
        }
        
        private static void SetUnityServiceId(AdvancedProjectBuilderConfig buildBuilderConfig)
        {
            string overrideUnityServiceIdRaw = GetParamValue(OverrideUnityServiceId);
            string unityProjectId = GetParamValue(UnityProjectId);
            string unityProjectOrganizationId = GetParamValue(UnityProjectOrganizationId);

            if (   string.IsNullOrEmpty(overrideUnityServiceIdRaw) 
                || string.IsNullOrEmpty(unityProjectId)
                || string.IsNullOrEmpty(unityProjectOrganizationId))
            {
                return;
            }

            if (!bool.TryParse(overrideUnityServiceIdRaw, out bool overrideUnityService))
            {
                AdvancedProjectBuilder.LogError($" Override Unity Service Id is wrong: '{overrideUnityServiceIdRaw}'");
            }

            buildBuilderConfig.OverrideUnityServiceId = overrideUnityService;
            buildBuilderConfig.UnityProjectId = unityProjectId;
            buildBuilderConfig.UnityProjectOrganizationId = unityProjectOrganizationId;
            
            AdvancedProjectBuilder.LogMessage($"Unity Service Id: enable-{overrideUnityService}, '{unityProjectId}' / '{unityProjectOrganizationId}'");
        }

        private static void SetBuildOptions(AdvancedProjectBuilderConfig buildBuilderConfig)
        {
            string buildOptionsRaw = GetParamValue(BuildOptionsParam);
            if (string.IsNullOrEmpty(buildOptionsRaw))
            {
                return;
            }
            
            string[] optionsParts = SplitParameter(buildOptionsRaw);

            BuildOptions finalOptions = BuildOptions.None;
            foreach (string part in optionsParts)
            {
                string trimmed = part.Trim();
                
                if (Enum.TryParse<BuildOptions>(trimmed, true, out BuildOptions parsedOption))
                {
                    finalOptions |= parsedOption; 
                }
                else
                {
                    Debug.LogWarning($"Unknown build option: '{trimmed}'");
                }
            }

            buildBuilderConfig.BuildOptions = finalOptions;
    
            Debug.Log($"Final BuildOptions: '{finalOptions}'");
        }
        
        private static void UpdateAndroidBuildConfig(AdvancedProjectBuilderConfig buildBuilderConfig)
        {
            string useAppBundleRaw = GetParamValue(AndroidBuildAppBundle);
            
            if (bool.TryParse(useAppBundleRaw, out bool useAppBundle))
            {
                buildBuilderConfig.UseAppBundle = useAppBundle;
                AdvancedProjectBuilder.LogMessage($"UseAppBundle: '{useAppBundle}'");
            }
            else
            {
                AdvancedProjectBuilder.LogError($"Wrong UseAppBundle: '{useAppBundleRaw}'");
            }

            string splitBinaryRaw = GetParamValue(AndroidSplitAppBinary);
            
            if (bool.TryParse(splitBinaryRaw, out bool splitBinary))
            {
                buildBuilderConfig.SplitApplicationBinary = splitBinary;
                AdvancedProjectBuilder.LogMessage($"SplitApplicationBinary: '{splitBinary}'");
            }
            else
            {
                AdvancedProjectBuilder.LogError($"Wrong SplitApplicationBinary: '{splitBinaryRaw}'");
            }

            string useKeystoreRaw = GetParamValue(AndroidUseKeystore);
            string androidKeystorePath = GetParamValue(AndroidKeystorePath);
            string androidKeystorePass = GetParamValue(AndroidKeystorePass);
            string androidKeyaliasName = GetParamValue(AndroidKeyaliasName);
            string androidKeyaliasPass = GetParamValue(AndroidKeyaliasPass);
            
            if (bool.TryParse(useKeystoreRaw, out bool useKeystore))
            {
                buildBuilderConfig.UseKeystore = useKeystore;
                AdvancedProjectBuilder.LogMessage($"UseKeystore: '{useKeystore}'");
                
                if (!useKeystore) return;
            }
            else
            {
                AdvancedProjectBuilder.LogError($"Wrong UseKeystore: '{splitBinaryRaw}'");
                return;
            }

            if (   !string.IsNullOrEmpty(androidKeystorePath)
                || !string.IsNullOrEmpty(androidKeystorePass)
                || !string.IsNullOrEmpty(androidKeyaliasName)
                || !string.IsNullOrEmpty(androidKeyaliasPass))
            {
                buildBuilderConfig.AndroidKeystorePath = androidKeystorePath;
                buildBuilderConfig.AndroidKeystorePass = androidKeystorePass;
                buildBuilderConfig.AndroidKeyaliasName = androidKeyaliasName;
                buildBuilderConfig.AndroidKeyaliasPass = androidKeyaliasPass;
            }
            else
            {
                AdvancedProjectBuilder.LogError($"Wrong KeystoreData");
            }
        }
        
        private static void UpdateIosBuildConfig(AdvancedProjectBuilderConfig buildBuilderConfig)
        {
            string appleDeveloperTeamId = GetParamValue(AppleDeveloperTeamID);
            
            if (!string.IsNullOrEmpty(appleDeveloperTeamId))
            {
                buildBuilderConfig.EnableAutomaticSigning = true;
                buildBuilderConfig.AppleDeveloperTeamID = appleDeveloperTeamId;
                AdvancedProjectBuilder.LogMessage($"EnableAutomaticSigning: ID-'{AppleDeveloperTeamID}'");
            }
        }

        private static string GetOnStringList(string[] array)
        {
            string text = string.Empty;

            for (int i = 0; i < array.Length; i++)
            {
                text += array[i] + ArgumentSplitSign;
            }

            return text;
        }
        
        private static string[] SplitParameter(string parameter)
        {
            return parameter.Split(ArgumentSplitSign);
        }
    }
}