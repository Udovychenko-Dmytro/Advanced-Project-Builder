#if UNITY_EDITOR

using UnityEditor.Build.Reporting;
using System.Threading;
using UnityEngine;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public static class PodsPostBuild
    {
        private const string InitCommand    = "pod init";
        private const string InstallCommand = "pod install";
        private const int    WaitTimeoutMs  = 2000;

        public static void OnPostprocessBuild(BuildReport report)
        {
            if (Application.platform != RuntimePlatform.OSXEditor)
            {
                return;
            }

            string outputPath = report.summary.outputPath;

            RunPodCommand(InitCommand, outputPath);

            Thread.Sleep(WaitTimeoutMs);

            RunPodCommand(InstallCommand, outputPath);
        }

        private static void RunPodCommand(string command, string path)
        {
            AdvancedProjectBuilder.LogMessage($"[PodsPostBuild] {command}");

            if (ShellRunner.Run(path, command) != 0)
            {
                throw new System.Exception($"{command} failed)");
            }
        }
    }
}
#endif