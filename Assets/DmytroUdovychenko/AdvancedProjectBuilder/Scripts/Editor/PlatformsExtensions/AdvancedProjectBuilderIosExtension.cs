using UnityEngine;
using UnityEditor;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public partial class AdvancedProjectBuilderConfig
    {
        public bool   EnableAutomaticSigning;
        public string AppleDeveloperTeamID;
    }

    public partial class AdvancedProjectBuilderSettingsEditor
    {
        private SerializedProperty m_enableAutomaticSigning;
        private SerializedProperty m_appleDeveloperTeamID;

        private void ReadParametersIos()
        {
            m_enableAutomaticSigning = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.EnableAutomaticSigning));
            m_appleDeveloperTeamID   = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.AppleDeveloperTeamID));
        }
        
        private void DrawParametersIos()
        {
            EditorGUILayout.LabelField("iOS Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_enableAutomaticSigning, new GUIContent("Enable Automatic Signing"));

            if (m_enableAutomaticSigning.boolValue)
            {
                EditorGUILayout.PropertyField(m_appleDeveloperTeamID, new GUIContent("Apple Developer Team ID"));
            }
        }
    }

    public partial class AdvancedProjectBuilder
    {
        private static void SetPlatformSpecificsIos(AdvancedProjectBuilderConfig settings)
        {
            PlayerSettings.iOS.appleEnableAutomaticSigning = settings.EnableAutomaticSigning;
            
            LogMessage($"Set appleEnableAutomaticSigning: {settings.EnableAutomaticSigning}");
            
            if (settings.EnableAutomaticSigning)
            {
                PlayerSettings.iOS.appleDeveloperTeamID = settings.AppleDeveloperTeamID;
                LogMessage($"Set appleDeveloperTeamID: {settings.AppleDeveloperTeamID}");
            }
        }
    }
}