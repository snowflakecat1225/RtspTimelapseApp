using System.Diagnostics;

namespace RTSP_Timelapse_App
{
    public class Bash
    {
        private static string appPath = "/bin/bash";
        public static string AppPath
        { 
            get { return appPath; }
            set { appPath = value; }
        }

        private static readonly ProcessStartInfo startInfo = new()
        {
            FileName = AppPath,
            RedirectStandardOutput = true
        };

        public static void Execute(string command)
        {
            startInfo.Arguments = $"-c \"{command}\"";
            var process = Process.Start(startInfo);
            process?.WaitForExit();
        }

        public static void SudoExecute(string command)
        {
            startInfo.Arguments = $"-c \"sudo {command}\"";
            var process = Process.Start(startInfo);
            process?.WaitForExit();
        }

        public static string ExecuteWithResult(string command)
        {
            startInfo.Arguments = $"-c \"{command}\"";
            var process = Process.Start(startInfo);
            string result = process?.StandardOutput.ReadToEnd();
            process?.WaitForExit();
            if (result == null)
                return string.Empty;
            return result;
        }

        public static string SudoExecuteWithResult(string command)
        {
            startInfo.Arguments = $"-c \"sudo {command}\"";
            var process = Process.Start(startInfo);
            string result = process?.StandardOutput.ReadToEnd();
            process?.WaitForExit();
            if (result == null)
                return string.Empty;
            return result;
        }
    }
}