#if UNITY_EDITOR

using UnityEditor;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public static class DefineSymbolsEditor
    {
        public static void SetDefineSymbols(BuildTargetGroup targetGroup, string[] define)
        {
            if (define == null)
            {
                AdvancedProjectBuilder.LogError($"{AdvancedProjectBuilderSettings.DebugName}: define == NULL.");
                return;
            }

            string defineSymbols = string.Join(";", define);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbols);
            AdvancedProjectBuilder.LogMessage($"Set DefineSymbols: {defineSymbols}");
        }
    }
}

#endif