using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public partial class AdvancedProjectBuilderConfig
    {
        public BuildTarget     BuildTarget;
        public BuildOptions    BuildOptions;
        public BuildConfigType BuildConfigType;

        public string[] DefineSymbols;

        public bool   CreateZip;

        public bool   OverrideProductName;
        public string ProductName;

        public bool   OverrideUnityServiceId;
        public string UnityProjectId;
        public string UnityProjectOrganizationId;
        public string UnityProjectName;

        public bool   OverrideBundleId;
        public string BundleId;

        public bool   IsCommandLineBuild  { get; set; }
        public string BuildPath           { get; set; }
        public string BuildVersion        { get; set; }
        public string BundleVersionNumber { get; set; }
    }

    public class BasePlatformExtension : IPlatformSpecifics
    {
        private SerializedProperty m_buildTarget;
        private SerializedProperty m_buildOptions;
        private SerializedProperty m_buildConfigType;
        private SerializedProperty m_buildDefine;
        private SerializedProperty m_createZip;

        private SerializedProperty m_overrideProductName;
        private SerializedProperty m_productName;

        private SerializedProperty m_overrideCustomServiceId;
        private SerializedProperty m_unityProjectId;
        private SerializedProperty m_unityProjectOrganizationId;
        private SerializedProperty m_unityProjectName;

        private SerializedProperty m_overrideBundleId;
        private SerializedProperty m_bundleId;

        public void ReadPlatformSpecifics(SerializedObject serializedObject, Object target)
        {
            m_buildTarget     = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.BuildTarget));
            m_buildOptions    = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.BuildOptions));
            m_buildDefine     = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.DefineSymbols));
            m_buildConfigType = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.BuildConfigType));
            m_createZip       = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.CreateZip));

            m_overrideProductName = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.OverrideProductName));
            m_productName         = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.ProductName));

            m_overrideCustomServiceId    = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.OverrideUnityServiceId));
            m_unityProjectId             = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.UnityProjectId));
            m_unityProjectOrganizationId = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.UnityProjectOrganizationId));
            m_unityProjectName           = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.UnityProjectName));

            m_overrideBundleId = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.OverrideBundleId));
            m_bundleId         = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.BundleId));
        }

        public void DrawPlatformSpecifics(SerializedObject serializedObject, Object target)
        {
            EditorGUILayout.PropertyField(m_buildTarget,     new GUIContent("Build Platform"));
            EditorGUILayout.PropertyField(m_buildOptions,    new GUIContent("Build Options"));
            EditorGUILayout.PropertyField(m_buildConfigType, new GUIContent("Build Config Type"));

            DrawDefineSymbols(serializedObject, target);
            DrawCreateZip();
            DrawUnityServiceId();
            DrawBundleId();
            DrawProductName();
        }

        public void SetPlatformSpecifics(AdvancedProjectBuilderConfig config)
        {
            SetProjectName(config);
            SetDefineSymbols(config);
            SetUnityCloudId(config);
            SetBundleId(config);
            SetBuildVersion(config);
        }
        public void OnPostprocessBuild(BuildPlayerOptions buildPlayerOptions, BuildReport report, AdvancedProjectBuilderConfig config)
        {

        }

        private static void SetProjectName(AdvancedProjectBuilderConfig settings)
        {
            if (settings.OverrideProductName)
            {
                SerializedObject   projectSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(AdvancedProjectBuilderSettings.ProjectSettingsPath)[0]);
                SerializedProperty projectNameProp = projectSettings.FindProperty(AdvancedProjectBuilderSettings.ProductNameProperty);

                projectNameProp.stringValue = settings.ProductName;
                projectSettings.ApplyModifiedProperties();

                AdvancedProjectBuilder.LogMessage($"Set ProjectName: '{settings.ProductName}'");
            }
        }

        private static void SetDefineSymbols(AdvancedProjectBuilderConfig settings)
        {
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(settings.BuildTarget);
            DefineSymbolsEditor.SetDefineSymbols(targetGroup, settings.DefineSymbols);
        }

        private static void SetUnityCloudId(AdvancedProjectBuilderConfig settings)
        {
            if (settings.OverrideUnityServiceId)
            {
                SerializedObject   projectSettings      = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(AdvancedProjectBuilderSettings.ProjectSettingsPath)[0]);
                SerializedProperty projectIDProp        = projectSettings.FindProperty(AdvancedProjectBuilderSettings.CloudProjectIDProperty);
                SerializedProperty organizationProperty = projectSettings.FindProperty(AdvancedProjectBuilderSettings.CloudOrganizationIDProperty);
                SerializedProperty productNameProperty  = projectSettings.FindProperty(AdvancedProjectBuilderSettings.CloudProjectNameProperty);

                projectIDProp.stringValue        = settings.UnityProjectId;
                organizationProperty.stringValue = settings.UnityProjectOrganizationId;
                productNameProperty.stringValue  = settings.UnityProjectName;
                projectSettings.ApplyModifiedProperties();
                AdvancedProjectBuilder.LogMessage($"Set UnityCloudId: '{settings.UnityProjectId}' / '{settings.UnityProjectOrganizationId}' / '{settings.UnityProjectName}'.");
            }
        }

        private static void SetBundleId(AdvancedProjectBuilderConfig settings)
        {
            if (settings.OverrideBundleId)
            {
                PlayerSettings.applicationIdentifier = settings.BundleId;
                AdvancedProjectBuilder.LogMessage($"Set applicationIdentifier: {settings.BundleId}");
            }
        }

        private static void SetBuildVersion(AdvancedProjectBuilderConfig settings)
        {
            if (!string.IsNullOrEmpty(settings.BuildVersion))
            {
                PlayerSettings.bundleVersion = settings.BuildVersion;
                AdvancedProjectBuilder.LogMessage($"Set BuildVersion: {settings.BuildVersion}");
            }
            if (!string.IsNullOrEmpty(settings.BundleVersionNumber))
            {
                if (settings.BuildTarget == BuildTarget.Android)
                {
                    if (int.TryParse(settings.BundleVersionNumber, out int bundleCode))
                    {
                        PlayerSettings.Android.bundleVersionCode = bundleCode;
                        AdvancedProjectBuilder.LogMessage($"Set Android bundleVersionCode: {bundleCode}");
                    }
                    else
                    {
                        AdvancedProjectBuilder.LogError($"Invalid bundle version number: {settings.BundleVersionNumber}");
                    }
                }
                if (settings.BuildTarget == BuildTarget.iOS)
                {
                    PlayerSettings.iOS.buildNumber = settings.BundleVersionNumber;
                    AdvancedProjectBuilder.LogMessage($"Set iOS buildNumber: {settings.BundleVersionNumber}");
                }
                if (settings.BuildTarget == BuildTarget.StandaloneOSX)
                {
                    PlayerSettings.macOS.buildNumber = settings.BundleVersionNumber;
                    AdvancedProjectBuilder.LogMessage($"Set Macos buildNumber: {settings.BundleVersionNumber}");
                }
            }
        }

        private void DrawDefineSymbols(SerializedObject serializedObject, Object target)
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Define Symbols", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            for (int i = 0; i < m_buildDefine.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                SerializedProperty define = m_buildDefine.GetArrayElementAtIndex(i);
                define.stringValue = EditorGUILayout.TextField($"Define {i + 1}", define.stringValue);

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    m_buildDefine.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Define Symbol"))
            {
                m_buildDefine.arraySize++;
            }

            GUILayout.Space(15);
            if (GUILayout.Button("Import Defines from Player Settings"))
            {
                ImportDefines(serializedObject, target);
            }

            if (GUILayout.Button("Export Defines to Player Settings"))
            {
                ExportDefines();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCreateZip()
        {
            GUILayout.Space(10);

            EditorGUILayout.PropertyField(m_createZip, new GUIContent("Create ZIP"));
        }

        private void DrawUnityServiceId()
        {
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Unity Service ID", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_overrideCustomServiceId, new GUIContent("Override Service ID"));

            if (m_overrideCustomServiceId.boolValue)
            {
                EditorGUILayout.PropertyField(m_unityProjectId,             new GUIContent("Unity Project ID"));
                EditorGUILayout.PropertyField(m_unityProjectOrganizationId, new GUIContent("Unity Project Organization ID"));
                EditorGUILayout.PropertyField(m_unityProjectName,           new GUIContent("Unity Project Name"));
            }
        }

        private void DrawBundleId()
        {
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Bundle ID", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_overrideBundleId, new GUIContent("Override Bundle ID"));

            if (m_overrideBundleId.boolValue)
            {
                EditorGUILayout.PropertyField(m_bundleId, new GUIContent("Bundle ID"));
            }
        }

        private void DrawProductName()
        {
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Override Product Name", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_overrideProductName, new GUIContent("Override Product Name"));

            if (m_overrideProductName.boolValue)
            {
                EditorGUILayout.PropertyField(m_productName, new GUIContent("Product Name"));
            }
        }

        private void ImportDefines(SerializedObject serializedObject, Object target)
        {
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string           defines     = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            if (string.IsNullOrEmpty(defines))
            {
                EditorUtility.DisplayDialog("Import Defines", "No Define Symbols for export.", "OK");
                return;
            }

            string[] defineArray = defines.Split(';');

            if (defineArray.Length == 0 || (defineArray.Length == 1 && string.IsNullOrEmpty(defineArray[0]))) {
                EditorUtility.DisplayDialog("Import Defines", "No Define Symbols for import.", "OK");
                return;
            }

            m_buildDefine.arraySize = defineArray.Length;

            for (int i = 0; i < defineArray.Length; i++)
            {
                m_buildDefine.GetArrayElementAtIndex(i).stringValue = defineArray[i];
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            EditorUtility.DisplayDialog("Import Defines", "Define Symbols imported from Player Settings.", "OK"); // review: could include the count of imported/exported symbols.
        }

        private void ExportDefines()
        {
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            string[] defines = GetDefinesFromSerializedProperty(m_buildDefine);
            DefineSymbolsEditor.SetDefineSymbols(targetGroup, defines);

            EditorUtility.DisplayDialog("Apply Defines", "Define Symbols exported in to the Player Settings.", "OK");
        }

        /// <summary>
        /// Converts a SerializedProperty representing an array or list of strings into a string array.
        /// </summary>
        /// <param name="property">The SerializedProperty representing an array or list of strings.</param>
        /// <returns>An array of strings.</returns>
        private string[] GetDefinesFromSerializedProperty(SerializedProperty property)
        {
            List<string> definesList = new List<string>();

            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty element = property.GetArrayElementAtIndex(i);
                definesList.Add(element.stringValue);
            }

            return definesList.ToArray();
        }
    }
}