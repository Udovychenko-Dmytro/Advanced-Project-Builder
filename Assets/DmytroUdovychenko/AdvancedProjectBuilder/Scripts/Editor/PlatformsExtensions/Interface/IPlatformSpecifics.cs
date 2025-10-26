using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public interface IPlatformSpecifics
    {
        void ReadPlatformSpecifics(SerializedObject serializedObject, Object target);
        void DrawPlatformSpecifics(SerializedObject serializedObject, Object target);
        void SetPlatformSpecifics(AdvancedProjectBuilderConfig config);
        void OnPostprocessBuild(BuildPlayerOptions buildPlayerOptions, BuildReport report, AdvancedProjectBuilderConfig config);
    }
}