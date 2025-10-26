using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public partial class AdvancedProjectBuilderConfig
    {
        public bool   UseAppBundle;
        public bool   SplitApplicationBinary;
        public bool   UseKeystore;
        public string AndroidKeystorePath;
        public string AndroidKeystorePass;
        public string AndroidKeyaliasName;
        public string AndroidKeyaliasPass;
    }

    public class AndroidExtension : IPlatformSpecifics
    {
        private SerializedProperty m_useAppBundle;
        private SerializedProperty m_splitApplicationBinary;
        private SerializedProperty m_useKeystore;
        private SerializedProperty m_androidKeystorePath;
        private SerializedProperty m_androidKeystorePass;
        private SerializedProperty m_androidKeyaliasName;
        private SerializedProperty m_androidKeyaliasPass;

        public void ReadPlatformSpecifics(SerializedObject serializedObject, Object target)
        {
            m_useAppBundle           = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.UseAppBundle));
            m_splitApplicationBinary = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.SplitApplicationBinary));

            m_useKeystore         = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.UseKeystore));
            m_androidKeystorePath = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.AndroidKeystorePath));
            m_androidKeystorePass = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.AndroidKeystorePass));
            m_androidKeyaliasName = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.AndroidKeyaliasName));
            m_androidKeyaliasPass = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.AndroidKeyaliasPass));
        }

        public void DrawPlatformSpecifics(SerializedObject serializedObject, Object target)
        {
            EditorGUILayout.LabelField("Android Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_useAppBundle, new GUIContent("Build App Bundle (AAB)"));
            EditorGUILayout.PropertyField(m_splitApplicationBinary,  new GUIContent("Split Application Binary (OBB)"));
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(m_useKeystore,  new GUIContent("Use Keystore"));

            if (m_useKeystore.boolValue)
            {
                EditorGUILayout.PropertyField(m_androidKeystorePath,  new GUIContent("Keystore Path"));
                EditorGUILayout.PropertyField(m_androidKeystorePass,  new GUIContent("Keystore Pass"));
                EditorGUILayout.PropertyField(m_androidKeyaliasName,  new GUIContent("Keyalias Name"));
                EditorGUILayout.PropertyField(m_androidKeyaliasPass,  new GUIContent("Keyalias Pass")); 
            }
        }

        public void SetPlatformSpecifics(AdvancedProjectBuilderConfig config)
        {
            EditorUserBuildSettings.buildAppBundle      = config.UseAppBundle;
            PlayerSettings.Android.useAPKExpansionFiles = config.SplitApplicationBinary;
            PlayerSettings.Android.useCustomKeystore    = config.UseKeystore;

            AdvancedProjectBuilder.LogMessage($"Set buildAppBundle: {config.UseAppBundle}");
            AdvancedProjectBuilder.LogMessage($"Set useAPKExpansionFiles: {config.SplitApplicationBinary}");
            AdvancedProjectBuilder.LogMessage($"Set useCustomKeystore: {config.UseKeystore}");

            if (config.UseKeystore)
            {
                PlayerSettings.Android.keystoreName = config.AndroidKeystorePath;
                PlayerSettings.Android.keystorePass = config.AndroidKeystorePass;
                PlayerSettings.Android.keyaliasName = config.AndroidKeyaliasName;
                PlayerSettings.Android.keyaliasPass = config.AndroidKeyaliasPass;
            }
        }
        public void OnPostprocessBuild(BuildPlayerOptions buildPlayerOptions, BuildReport report, AdvancedProjectBuilderConfig config)
        {

        }
    }
}