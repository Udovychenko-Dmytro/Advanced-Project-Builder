using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public class StandaloneOSXExtension : IPlatformSpecifics
    {
        private SerializedProperty m_enableAutomaticSigning;
        private SerializedProperty m_appleDeveloperTeamID;
        //TODO: add create xcode project option

        public void ReadPlatformSpecifics(SerializedObject serializedObject, Object target)
        {
            m_enableAutomaticSigning = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.EnableAutomaticSigning));
            m_appleDeveloperTeamID   = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.AppleDeveloperTeamID));
        }

        public void DrawPlatformSpecifics(SerializedObject serializedObject, Object target)
        {
            EditorGUILayout.LabelField("StandaloneOSX Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_enableAutomaticSigning, new GUIContent("Enable Automatic Signing"));

            if (m_enableAutomaticSigning.boolValue)
            {
                EditorGUILayout.PropertyField(m_appleDeveloperTeamID, new GUIContent("Apple Developer Team ID"));
            }
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

        }
    }
}