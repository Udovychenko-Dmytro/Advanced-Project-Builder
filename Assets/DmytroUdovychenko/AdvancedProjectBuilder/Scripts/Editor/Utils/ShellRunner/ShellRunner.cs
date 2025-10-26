#if UNITY_EDITOR

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public static class ShellRunner
    {
        public static int Run(string workingDir, string command)
        {
#if UNITY_EDITOR_OSX
            return ShellRunnerIOS.Run(workingDir, command);
#endif

            return 0;
        }
    }
}
#endif