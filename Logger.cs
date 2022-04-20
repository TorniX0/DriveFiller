using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace DriveFiller
{
    internal static class Logger
    {
        private static StringBuilder logger = new();
        internal static bool active { get; private set; } = false;

        private static string logFileName { 
            get 
            { 
                return Regex.Replace($"DriveFiller-{DateTime.Now}.log", "[\\/:*?\"<>|]", "-").Replace(" ", "_"); 
            } 
        }

        internal static void SetActive(bool value)
        {
            active = value;
            logger.Clear();
        }

        private static string GetLogPath()
        {
            string? dir = Path.GetDirectoryName(Environment.ProcessPath);
            if (dir != null) return Path.Combine(dir, logFileName);
            else return string.Empty;
        }

        internal static void AddLog(string info)
        {
            logger.AppendLine(info);
        }

        internal static void WriteLogToDisk(string path = "")
        {
            if (path == string.Empty) path = GetLogPath();
            File.WriteAllText(path, logger.ToString());
            StandardMessages.SpaceOutText($"Saved log file in {path}!");
        }

    }
}
