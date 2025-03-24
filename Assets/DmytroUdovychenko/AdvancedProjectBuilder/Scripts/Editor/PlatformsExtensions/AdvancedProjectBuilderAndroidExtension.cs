using UnityEngine;
using UnityEditor;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public partial class AdvancedProjectBuilderConfig
    {
        public bool UseAppBundle;
        public bool SplitApplicationBinary;
        public bool UseKeystore;
        public string AndroidKeystorePath;
        public string AndroidKeystorePass;
        public string AndroidKeyaliasName;
        public string AndroidKeyaliasPass;
    }

    public partial class AdvancedProjectBuilderSettingsEditor
    {
        private SerializedProperty m_useAppBundle;
        private SerializedProperty m_splitApplicationBinary;
        private SerializedProperty m_useKeystore;
        private SerializedProperty m_androidKeystorePath;
        private SerializedProperty m_androidKeystorePass;
        private SerializedProperty m_androidKeyaliasName;
        private SerializedProperty m_androidKeyaliasPass;

        private void ReadParametersAndroid()
        {
            m_useAppBundle           = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.UseAppBundle));
            m_splitApplicationBinary = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.SplitApplicationBinary));
            
            m_useKeystore         = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.UseKeystore));
            m_androidKeystorePath = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.AndroidKeystorePath));
            m_androidKeystorePass = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.AndroidKeystorePass));
            m_androidKeyaliasName = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.AndroidKeyaliasName));
            m_androidKeyaliasPass = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.AndroidKeyaliasPass));
        }
        
        private void DrawParametersAndroid()
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
    }

    public partial class AdvancedProjectBuilder
    {
        private static void SetPlatformSpecificsAndroid(AdvancedProjectBuilderConfig settings)
        {
            EditorUserBuildSettings.buildAppBundle = settings.UseAppBundle;
            PlayerSettings.Android.useAPKExpansionFiles = settings.SplitApplicationBinary;
            PlayerSettings.Android.useCustomKeystore = settings.UseKeystore;
            
            LogMessage($"Set buildAppBundle: {settings.UseAppBundle}");
            LogMessage($"Set useAPKExpansionFiles: {settings.SplitApplicationBinary}");
            LogMessage($"Set useCustomKeystore: {settings.UseKeystore}");
            
            if (settings.UseKeystore)
            {
                PlayerSettings.Android.keystoreName = settings.AndroidKeystorePath;
                PlayerSettings.Android.keystorePass = settings.AndroidKeystorePass;
                PlayerSettings.Android.keyaliasName = settings.AndroidKeyaliasName;
                PlayerSettings.Android.keyaliasPass = settings.AndroidKeyaliasPass;
            }
        }
    }
}