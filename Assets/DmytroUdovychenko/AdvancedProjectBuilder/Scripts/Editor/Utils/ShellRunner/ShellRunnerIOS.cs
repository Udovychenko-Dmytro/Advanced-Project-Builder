#if UNITY_EDITOR

using System.Diagnostics;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public static class ShellRunnerIOS
    {
        private const string ShellPath      = "/bin/zsh";
        private const string ShellArguments = "-l -c"; // l-login shell, c-command
        private const string ShellEncoding  = "en_US.UTF-8";
        private const string ShelLogName    = "[ShellRunner]";

        public static int Run(string workingDir, string command)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                Arguments        = $"{ShellArguments} \"cd '{workingDir}' && {command}\"",
                FileName         = ShellPath,
                WorkingDirectory = workingDir,
                UseShellExecute  = false,
                CreateNoWindow   = true,

                RedirectStandardOutput = true,
                RedirectStandardError  = true
            };

            processStartInfo.EnvironmentVariables["LANG"]   = ShellEncoding;
            processStartInfo.EnvironmentVariables["LC_ALL"] = ShellEncoding;

            using Process process = new Process();
            process.StartInfo     = processStartInfo;

            process.OutputDataReceived += OnProcessOnOutputDataReceived;
            process.ErrorDataReceived  += OnProcessOnErrorDataReceived;

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            return process.ExitCode;
        }

        private static void OnProcessOnOutputDataReceived(object _, DataReceivedEventArgs eventArgs)
        {
            if (!string.IsNullOrEmpty(eventArgs.Data))
            {
                AdvancedProjectBuilder.LogMessage($"{ShelLogName}: {eventArgs.Data}");
            }
        }

        private static void OnProcessOnErrorDataReceived(object _, DataReceivedEventArgs eventArgs)
        {
            if (!string.IsNullOrEmpty(eventArgs.Data))
            {
                AdvancedProjectBuilder.LogWarning($"{ShelLogName}: Error - {eventArgs.Data}");
            }
        }
    }
}
#endif