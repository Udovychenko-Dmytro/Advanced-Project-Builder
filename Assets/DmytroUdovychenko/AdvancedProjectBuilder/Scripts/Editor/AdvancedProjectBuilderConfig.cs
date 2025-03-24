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
    public partial class AdvancedProjectBuilderConfig : ScriptableObject
    {
        public BuildTarget BuildTarget;
        public BuildOptions BuildOptions;
        public BuildConfigType BuildConfigType;
        public string[] DefineSymbols;
        
        public bool   OverrideProductName;
        public string ProductName;
        
        public bool   OverrideUnityServiceId;
        public string UnityProjectId;
        public string UnityProjectOrganizationId;
        public string UnityProjectName;
        
        public bool   OverrideBundleId;
        public string BundleId;
        
        public bool IsCommandLineBuild { get; set; }
        public string BuildPath { get; set; }
        public string BuildVersion { get; set; }
        public string BundleVersionNumber { get; set; }
    }
}