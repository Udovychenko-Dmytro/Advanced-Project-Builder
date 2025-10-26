using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public partial class AdvancedProjectBuilderConfig
    {
        public bool   EnableAutomaticSigning;
        public string AppleDeveloperTeamID;
        public bool   BuildCocoaPods;
    }

    public class iOSExtension : IPlatformSpecifics
    {
        private SerializedProperty m_enableAutomaticSigning;
        private SerializedProperty m_appleDeveloperTeamID;
        private SerializedProperty m_buildCocoaPods;

        public void ReadPlatformSpecifics(SerializedObject serializedObject, Object target)
        {
            m_enableAutomaticSigning = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.EnableAutomaticSigning));
            m_appleDeveloperTeamID   = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.AppleDeveloperTeamID));
            m_buildCocoaPods         = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.BuildCocoaPods));
        }

        public void DrawPlatformSpecifics(SerializedObject serializedObject, Object target)
        {
            EditorGUILayout.LabelField("iOS Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_enableAutomaticSigning, new GUIContent("Enable Automatic Signing"));

            if (m_enableAutomaticSigning.boolValue)
            {
                EditorGUILayout.PropertyField(m_appleDeveloperTeamID, new GUIContent("Apple Developer Team ID"));
            }

            EditorGUILayout.PropertyField(m_buildCocoaPods, new GUIContent("Build CocoaPods (*.workspace)"));
        }

        public void SetPlatformSpecifics(AdvancedProjectBuilderConfig config)
        {
            PlayerSettings.iOS.appleEnableAutomaticSigning = config.EnableAutomaticSigning;

            AdvancedProjectBuilder.LogMessage($"Set appleEnableAutomaticSigning: {config.EnableAutomaticSigning}");

            if (config.EnableAutomaticSigning)
            {
                PlayerSettings.iOS.appleDeveloperTeamID = config.AppleDeveloperTeamID;
                AdvancedProjectBuilder.LogMessage($"Set appleDeveloperTeamID: {config.AppleDeveloperTeamID}");
            }
        }
        public void OnPostprocessBuild(BuildPlayerOptions buildPlayerOptions, BuildReport report, AdvancedProjectBuilderConfig config)
        {
            if (config.BuildCocoaPods)
            {
                PodsPostBuild.OnPostprocessBuild(report);
            }
        }
    }
}