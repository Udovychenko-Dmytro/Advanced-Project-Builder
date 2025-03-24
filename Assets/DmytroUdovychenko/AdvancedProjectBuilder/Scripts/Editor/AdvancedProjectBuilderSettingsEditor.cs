// ====================================================
// Advanced Project Builder Tool - Unity Plugin
// Author: Dmytro Udovychenko
// Contact: https://www.linkedin.com/in/dmytro-udovychenko/
// License: MIT
// © 2025 Dmytro Udovychenko. All rights reserved.
// ====================================================

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    [CustomEditor(typeof(AdvancedProjectBuilderConfig))]
    public partial class AdvancedProjectBuilderSettingsEditor : Editor
    {
        private SerializedProperty m_buildTarget;
        private SerializedProperty m_buildOptions;
        private SerializedProperty m_buildConfigType;
        private SerializedProperty m_buildDefine;
        
        private SerializedProperty m_overrideProductName;
        private SerializedProperty m_productName;
        
        private SerializedProperty m_overrideCustomServiceId;
        private SerializedProperty m_unityProjectId;
        private SerializedProperty m_unityProjectOrganizationId;
        private SerializedProperty m_unityProjectName;
        
        private SerializedProperty m_overrideBundleId;
        private SerializedProperty m_bundleId;
        
        private void OnEnable()
        {
            m_buildTarget  = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.BuildTarget));
            m_buildOptions = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.BuildOptions));
            m_buildDefine  = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.DefineSymbols));
            m_buildConfigType = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.BuildConfigType));
            
            m_overrideProductName = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.OverrideProductName));
            m_productName = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.ProductName));
            
            m_overrideCustomServiceId  = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.OverrideUnityServiceId));
            m_unityProjectId  = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.UnityProjectId));
            m_unityProjectOrganizationId  = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.UnityProjectOrganizationId));
            m_unityProjectName  = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.UnityProjectName));
            
            m_overrideBundleId  = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.OverrideBundleId));
            m_bundleId = serializedObject.FindProperty(nameof(AdvancedProjectBuilderConfig.BundleId));

            ReadPlatformSpecifics();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(10);
            
            EditorGUILayout.PropertyField(m_buildTarget, new GUIContent("Build Platform"));
            EditorGUILayout.PropertyField(m_buildOptions, new GUIContent("Build Options"));
            EditorGUILayout.PropertyField(m_buildConfigType, new GUIContent("Build Config Type"));

            DrawDefineSymbols();
            DrawUnityServiceId();
            DrawBundleId();
            DrawProductName();
            
            DrawPlatformSpecifics();
            GUILayout.Space(10);
            
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawDefineSymbols()
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
                ImportDefines();
            }
            
            if (GUILayout.Button("Export Defines to Player Settings"))
            {
                ExportDefines();
            }
        }

        private void DrawUnityServiceId()
        {
            GUILayout.Space(10);
            
            EditorGUILayout.LabelField("Unity Service ID", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_overrideCustomServiceId, new GUIContent("Override Service ID"));

            if (m_overrideCustomServiceId.boolValue)
            {
                EditorGUILayout.PropertyField(m_unityProjectId, new GUIContent("Unity Project ID"));
                EditorGUILayout.PropertyField(m_unityProjectOrganizationId, new GUIContent("Unity Project Organization ID"));
                EditorGUILayout.PropertyField(m_unityProjectName, new GUIContent("Unity Project Name"));  
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
        
        private void ImportDefines()
        {
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            if (string.IsNullOrEmpty(defines))
            {
                EditorUtility.DisplayDialog("Import Defines", "No Define Symbols for export.", "OK");
                return;
            }
            
            string[] defineArray = defines.Split(';');
            m_buildDefine.arraySize = defineArray.Length;
            
            for (int i = 0; i < defineArray.Length; i++)
            {
                m_buildDefine.GetArrayElementAtIndex(i).stringValue = defineArray[i];
            }
            
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            EditorUtility.DisplayDialog("Import Defines", "Define Symbols imported from Player Settings.", "OK"); // review: could include the count of imported/exported symbols.
        }
        
        private void DrawPlatformSpecifics()
        {
            GUILayout.Space(10);
            
            BuildTarget buildTarget = (BuildTarget)m_buildTarget.intValue;

            switch (buildTarget)
            {
                case BuildTarget.Android:
                    DrawParametersAndroid();
                    break;
                
                case BuildTarget.iOS:
                    DrawParametersIos();
                    break;
            }
        }
        
        private void ExportDefines()
        {
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            string[] defines = GetDefinesFromSerializedProperty(m_buildDefine);
            AdvancedProjectBuilder.SetDefineSymbols(targetGroup, defines);
            
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

        // TODO: could use inheritance and override to get the platform specific data.
        private void ReadPlatformSpecifics()
        {
            ReadParametersAndroid();
            ReadParametersIos();
        }
    }
}