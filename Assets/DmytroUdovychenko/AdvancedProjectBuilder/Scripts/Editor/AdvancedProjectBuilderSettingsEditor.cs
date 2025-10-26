// ====================================================
// Advanced Project Builder Tool - Unity Plugin
// Author: Dmytro Udovychenko
// Contact: https://www.linkedin.com/in/dmytro-udovychenko/
// License: MIT
// © 2025 Dmytro Udovychenko. All rights reserved.
// ====================================================

using UnityEditor;
using UnityEngine;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    [CustomEditor(typeof(AdvancedProjectBuilderConfig))]
    public class AdvancedProjectBuilderSettingsEditor : Editor
    {
        private SerializedProperty m_buildTarget;

        private IPlatformSpecifics m_platformBase = new BasePlatformExtension();
        private IPlatformSpecifics m_platformSpecifics;
        private BuildTarget        m_lastBuildTarget = BuildTarget.NoTarget;

        private void OnEnable()
        {
            m_buildTarget = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.BuildTarget));

            m_platformBase.ReadPlatformSpecifics(serializedObject, target);

            BuildTarget buildTarget = AdvancedProjectBuilderSettings.GetBuildTargetFromSerializedProperty(m_buildTarget);

            if (buildTarget != m_lastBuildTarget)
            {
                m_lastBuildTarget   = buildTarget;
                m_platformSpecifics = AdvancedProjectBuilderSettings.CreatePlatformSpecifics(buildTarget);
            }

            if (m_platformSpecifics != null)
            {
                m_platformSpecifics.ReadPlatformSpecifics(serializedObject, target);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();

            GUILayout.Space(10);

            m_platformBase.DrawPlatformSpecifics(serializedObject, target);

            if (m_platformSpecifics != null)
            {
                GUILayout.Space(10);
                m_platformSpecifics.DrawPlatformSpecifics(serializedObject, target);
            }

            GUILayout.Space(10);

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}