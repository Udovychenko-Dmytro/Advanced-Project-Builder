// ====================================================
// Advanced Project Builder Tool - Unity Plugin
// Author: Dmytro Udovychenko
// Contact: https://www.linkedin.com/in/dmytro-udovychenko/
// License: MIT
// © 2025 Dmytro Udovychenko. All rights reserved.
// ====================================================

using System.Collections.Generic;
using UnityEngine;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    /// <summary>
    /// Stores the order of BuildToolSettings configurations.
    /// </summary>
    public class AdvancedProjectBuilderSettingsMain : ScriptableObject
    {
        public List<AdvancedProjectBuilderConfig> orderedSettings = new List<AdvancedProjectBuilderConfig>();
        public string Path;
    }
}