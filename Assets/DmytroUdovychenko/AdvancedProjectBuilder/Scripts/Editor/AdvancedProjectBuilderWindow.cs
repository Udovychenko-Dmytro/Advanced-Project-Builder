// ====================================================
// Advanced Project Builder Tool - Unity Plugin
// Author: Dmytro Udovychenko
// Contact: https://www.linkedin.com/in/dmytro-udovychenko/
// License: MIT
// © 2025 Dmytro Udovychenko. All rights reserved.
// ====================================================

using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.IO;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public class AdvancedProjectBuilderWindow : EditorWindow
    {
        private string m_buildPath;
        
        private enum ViewMode { ListViewMode, EditMode }

        private ViewMode m_currentMode = ViewMode.ListViewMode;
        private AdvancedProjectBuilderConfig m_currentExportSettings;
        
        private List<AdvancedProjectBuilderConfig> m_buildSettingsList = new List<AdvancedProjectBuilderConfig>();
        private ReorderableList m_configurationsList;
        private AdvancedProjectBuilderSettingsMain m_settingsMain;
        private Vector2 m_editScrollPos = Vector2.zero; // Persistent scroll position

        [MenuItem(AdvancedProjectBuilderSettings.MenuButtonPath)]
        public static void ShowWindow()
        {
            AdvancedProjectBuilderWindow window = GetWindow<AdvancedProjectBuilderWindow>(AdvancedProjectBuilderSettings.Name);
            window.minSize = new Vector2(600, 400); // Set a suitable minimum size
        }

        private void OnEnable()
        {
            m_buildPath = EditorPrefs.GetString(AdvancedProjectBuilderSettings.GetSavedPathId());
            LoadSettingsOrder();
            LoadConfigurations();
            InitializeOrderedSettings();
        }

        private void OnGUI()
        {
            switch (m_currentMode)
            {
                case ViewMode.ListViewMode:
                    DrawListViewMode();
                    break;
                case ViewMode.EditMode:
                    DrawEditMode();
                    break;
            }
        }

        /// <summary>
        /// Initializes the ReorderableList with appropriate callbacks.
        /// </summary>
        private void InitializeOrderedSettings()
        {
            m_configurationsList = new ReorderableList(
                m_buildSettingsList,
                typeof(AdvancedProjectBuilderConfig),
                true,  // Draggable
                true,  // Display header
                false, // Don't display add button
                false  // Don't display remove button
            );
            
            m_configurationsList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Build Tool Configurations");
            };
            
            m_configurationsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= m_buildSettingsList.Count) return;

                AdvancedProjectBuilderConfig settings = m_buildSettingsList[index];

                // Adjust rect for padding
                float padding = 5f;
                rect.y += padding;
                rect.height -= padding * 1f;

                // Calculate widths
                float buildButton = 80f;
                float openButtonWidth = 70f;
                float duplicateButtonWidth = 70f;
                float deleteButtonWidth = 50f;
                float spacing = 50f;
                float objectFieldWidth = rect.width - (buildButton + openButtonWidth + duplicateButtonWidth + deleteButtonWidth + spacing);

                // Object field
                Rect objectFieldRect = new Rect(rect.x, rect.y, objectFieldWidth, rect.height);
                EditorGUI.ObjectField(objectFieldRect, settings, typeof(AdvancedProjectBuilderConfig), false);
                Color originalColor = GUI.backgroundColor;
                
                // "Build" button
                Rect buildButtonRect = new Rect(rect.x + objectFieldWidth + 10f, rect.y, buildButton, rect.height);
                GUI.backgroundColor = Color.green;
                if (GUI.Button(buildButtonRect, "Build"))
                {
                    m_currentExportSettings = settings;
                    MakeBuild(m_currentExportSettings);
                }
                GUI.backgroundColor = originalColor;

                // "Open" button
                Rect openButtonRect = new Rect(rect.x + objectFieldWidth + buildButton + 20f, rect.y, buildButton, rect.height);
                if (GUI.Button(openButtonRect, "Open"))
                {
                    m_currentExportSettings = settings;
                    m_currentMode = ViewMode.EditMode;
                }

                // "Duplicate" button
                Rect duplicateButtonRect = new Rect(rect.x + objectFieldWidth + buildButton + openButtonWidth + 40f, rect.y, duplicateButtonWidth, rect.height);
                GUI.backgroundColor = Color.blue;
                if (GUI.Button(duplicateButtonRect, "Duplicate"))
                {
                    DuplicateConfiguration(settings);
                }
                GUI.backgroundColor = originalColor;

                // "Delete" button
                Rect deleteButtonRect = new Rect(rect.x + objectFieldWidth + buildButton + openButtonWidth + duplicateButtonWidth + 50f, rect.y, deleteButtonWidth, rect.height);
                GUI.backgroundColor = Color.red;
                if (GUI.Button(deleteButtonRect, "Delete"))
                {
                    if (EditorUtility.DisplayDialog("Confirm Delete", $"Are you sure you want to delete '{settings.name}'?", "Yes", "No"))
                    {
                        DeleteConfiguration(settings);
                    }
                }
                GUI.backgroundColor = originalColor;
            };
            
            m_configurationsList.onReorderCallback = (ReorderableList list) =>
            {
                SaveSettingsOrder();
            };
        }

        /// <summary>
        /// Draws the list view mode using ReorderableList.
        /// </summary>
        private void DrawListViewMode()
        {
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);

            if (GUILayout.Button("Create New Configuration"))
            {
                CreateNewConfiguration();
            }
            
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField("Build Path:", GUILayout.Width(70));
            
            if (GUILayout.Button("Select Folder...", GUILayout.Width(120)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Build Folder", m_buildPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    m_buildPath = selectedPath;
                    EditorPrefs.SetString(AdvancedProjectBuilderSettings.GetSavedPathId(), m_buildPath);
                    EditorUtility.SetDirty(this);
                }
            }
            
            EditorGUILayout.LabelField(string.IsNullOrEmpty(m_buildPath) ? "Not Selected" : m_buildPath);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            
            m_configurationsList.DoLayoutList();
        }


        /// <summary>
        /// Draws the edit mode for a selected configuration.
        /// </summary>
        private void DrawEditMode()
        {
            m_editScrollPos = EditorGUILayout.BeginScrollView(m_editScrollPos);
            
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            Color originalColor = GUI.backgroundColor;
            
            if (GUILayout.Button("Back"))
            {
                m_currentMode = ViewMode.ListViewMode;
                LoadConfigurations();
            }
            
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Build"))
            {
                MakeBuild(m_currentExportSettings);
            }
            GUI.backgroundColor = originalColor;
            
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (m_currentExportSettings == null)
            {
                EditorGUILayout.LabelField("No configuration selected.");
                EditorGUILayout.EndScrollView();
                return;
            }

            // Edit Configuration Name
            string assetPath = AssetDatabase.GetAssetPath(m_currentExportSettings);
            string assetName = Path.GetFileNameWithoutExtension(assetPath);
            string newAssetName = EditorGUILayout.TextField("Config Name:", assetName);
            if (newAssetName != assetName && !string.IsNullOrEmpty(newAssetName))
            {
                string newAssetPath = Path.Combine(Path.GetDirectoryName(assetPath), newAssetName + ".asset");
                if (AssetDatabase.MoveAsset(assetPath, newAssetPath).StartsWith("Assets/"))
                {
                    AssetDatabase.SaveAssets();
                    m_currentExportSettings = AssetDatabase.LoadAssetAtPath<AdvancedProjectBuilderConfig>(newAssetPath);
                }
            }

            GUILayout.Space(5);
            
            if (m_currentExportSettings != null)
            {
                EditorGUI.BeginChangeCheck();

                Editor settingsEditor = Editor.CreateEditor(m_currentExportSettings);
                settingsEditor.OnInspectorGUI();

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(m_currentExportSettings);
                }
            }
            
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Loads the BuildToolSettingsOrder asset.
        /// </summary>
        private void LoadSettingsOrder()
        {
            string orderAssetPath = Path.Combine(AdvancedProjectBuilderSettings.GetConfigFolder(), "BuildToolSettingsOrder.asset");
            m_settingsMain = AssetDatabase.LoadAssetAtPath<AdvancedProjectBuilderSettingsMain>(orderAssetPath);

            if (m_settingsMain == null)
            {
                m_settingsMain = CreateInstance<AdvancedProjectBuilderSettingsMain>();
                AssetDatabase.CreateAsset(m_settingsMain, orderAssetPath);
                AssetDatabase.SaveAssets();
                Debug.Log($"{AdvancedProjectBuilderSettings.DebugName}: Created new BuildToolSettingsOrder asset at {orderAssetPath}");
            }
        }

        /// <summary>
        /// Saves the current order of configurations to BuildToolSettingsOrder asset.
        /// </summary>
        private void SaveSettingsOrder()
        {
            if (m_settingsMain != null)
            {
                m_settingsMain.orderedSettings = new List<AdvancedProjectBuilderConfig>(m_buildSettingsList);
                EditorUtility.SetDirty(m_settingsMain);
                AssetDatabase.SaveAssets();
                Debug.Log($"{AdvancedProjectBuilderSettings.DebugName}: Saved new configuration order.");
            }
        }
        
        /// <summary>
        /// Loads all BuildToolSettings configurations based on the saved order.
        /// </summary>
        private void LoadConfigurations()
        {
            m_buildSettingsList.Clear();
            
            //If has list of settings
            if (m_settingsMain != null && m_settingsMain.orderedSettings != null && m_settingsMain.orderedSettings.Count > 0)
            {
                foreach (AdvancedProjectBuilderConfig settings in m_settingsMain.orderedSettings)
                {
                    if (settings != null)
                    {
                        m_buildSettingsList.Add(settings);
                        
                    }
                    
                }
                
                List<AdvancedProjectBuilderConfig> foundInFolder = AdvancedProjectBuilderSettings.FindAllBuildToolSettings();
                
                foreach (AdvancedProjectBuilderConfig settings in foundInFolder)
                {
                    if (!m_buildSettingsList.Contains(settings))
                    {
                        m_buildSettingsList.Add(settings);
                        m_settingsMain.orderedSettings.Add(settings);
                        
                    }
                }
            }
            else
            {
                List<AdvancedProjectBuilderConfig> foundInFolder = AdvancedProjectBuilderSettings.FindAllBuildToolSettings();
                
                foreach (AdvancedProjectBuilderConfig settings in foundInFolder)
                {
                    m_buildSettingsList.Add(settings);
                    
                    if (m_settingsMain != null)
                    {
                        if (m_settingsMain.orderedSettings == null)
                        {
                            m_settingsMain.orderedSettings = new List<AdvancedProjectBuilderConfig>();
                        }
                        m_settingsMain.orderedSettings.Add(settings);
                    }
                }
            }
            
            if (m_configurationsList != null)
            {
                m_configurationsList.list = m_buildSettingsList;
                m_configurationsList.index = -1;
            }
            
            SaveSettingsOrder();
        }


        /// <summary>
        /// Creates a new BuildToolSettings configuration.
        /// </summary>
        private void CreateNewConfiguration()
        {
            Debug.Log($"{AdvancedProjectBuilderSettings.DebugName}: Attempting to create a new configuration.");
            string folderPath = AdvancedProjectBuilderSettings.GetConfigFolder();

            if (!AdvancedProjectBuilderUtility.EnsureFoldersExist(folderPath))
            {
                Debug.LogError($"{AdvancedProjectBuilderSettings.DebugName}: Failed to ensure folder exists at {folderPath}");
                return;
            }

            AdvancedProjectBuilderConfig newSettings = ScriptableObject.CreateInstance<AdvancedProjectBuilderConfig>();
            string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{AdvancedProjectBuilderSettings.NewConfigName}.asset");

            AssetDatabase.CreateAsset(newSettings, uniqueAssetPath);
            AssetDatabase.SaveAssets();

            m_buildSettingsList.Add(newSettings);
            m_settingsMain.orderedSettings.Add(newSettings);
            SaveSettingsOrder();

            Debug.Log($"{AdvancedProjectBuilderSettings.DebugName}: Created new configuration at {uniqueAssetPath}");
        }

        /// <summary>
        /// Duplicates the specified BuildToolSettings configuration.
        /// </summary>
        /// <param name="original">The original configuration to duplicate.</param>
        private void DuplicateConfiguration(AdvancedProjectBuilderConfig original)
        {
            if (original == null)
            {
                Debug.LogError($"{AdvancedProjectBuilderSettings.DebugName}: Original configuration is null. Cannot duplicate.");
                return;
            }

            // Create a new instance
            AdvancedProjectBuilderConfig duplicate = ScriptableObject.CreateInstance<AdvancedProjectBuilderConfig>();

            // Copy serialized data
            EditorUtility.CopySerialized(original, duplicate);

            // Generate a unique asset path
            string originalPath = AssetDatabase.GetAssetPath(original);
            string directory = Path.GetDirectoryName(originalPath);
            string originalName = Path.GetFileNameWithoutExtension(originalPath);
            string newName = AssetDatabase.GenerateUniqueAssetPath($"{directory}/{originalName}_Copy.asset");

            // Create the asset
            AssetDatabase.CreateAsset(duplicate, newName);
            AssetDatabase.SaveAssets();

            // Add to the list and order
            m_buildSettingsList.Add(duplicate);
            m_settingsMain.orderedSettings.Add(duplicate);
            SaveSettingsOrder();

            Debug.Log($"{AdvancedProjectBuilderSettings.DebugName}: Duplicated configuration '{original.name}' to '{duplicate.name}'.");
        }

        /// <summary>
        /// Deletes the specified BuildToolSettings configuration.
        /// </summary>
        /// <param name="settings">The configuration to delete.</param>
        private void DeleteConfiguration(AdvancedProjectBuilderConfig settings)
        {
            if (settings == null)
            {
                Debug.LogError($"{AdvancedProjectBuilderSettings.DebugName}: Configuration is null. Cannot delete.");
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(settings);
            if (AssetDatabase.DeleteAsset(assetPath))
            {
                Debug.Log($"{AdvancedProjectBuilderSettings.DebugName}: Deleted configuration at {assetPath}");
                m_buildSettingsList.Remove(settings);
                m_settingsMain.orderedSettings.Remove(settings);
                SaveSettingsOrder();

                // If the deleted configuration was currently open, switch back to list view
                if (m_currentExportSettings == settings)
                {
                    m_currentExportSettings = null;
                    m_currentMode = ViewMode.ListViewMode;
                }
            }
            else
            {
                Debug.LogError($"{AdvancedProjectBuilderSettings.DebugName}: Failed to delete configuration at {assetPath}");
            }
        }

        /// <summary>
        /// Initiates the build process using the current configuration.
        /// </summary>
        private void MakeBuild(AdvancedProjectBuilderConfig advancedProjectBuilderConfig)
        {
            if (advancedProjectBuilderConfig == null)
            {
                Debug.LogError($"{AdvancedProjectBuilderSettings.DebugName}: No configuration selected for build.");
                return;
            }

            advancedProjectBuilderConfig.BuildPath = EditorPrefs.GetString(AdvancedProjectBuilderSettings.GetSavedPathId());
            advancedProjectBuilderConfig.IsCommandLineBuild = false;

            if (string.IsNullOrEmpty(advancedProjectBuilderConfig.BuildPath))
            {
                Debug.LogError($"Build path is empty.");
            }
            
            Debug.Log($"{AdvancedProjectBuilderSettings.DebugName}: Build process initiated for '{advancedProjectBuilderConfig.name}'.");
            
            AdvancedProjectBuilder.Build(advancedProjectBuilderConfig);
        }
    }
}