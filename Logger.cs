using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace DriveFiller
{
    internal static class Logger
    {
        private static StringBuilder logger = new();
        private static bool active = false;

        private static string logFileName
        {
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

            if (dir == null)
            {
                return Path.GetTempPath();
            }
            
            return Path.Combine(dir, logFileName);
        }

        internal static void AddLog(string info)
        {
            if (!active)
            {
                return;
            }

            logger.AppendLine(info);
        }

        internal static void AddSpacedLog(string info)
        {
            if (!active)
            {
                return;
            }

            logger.AppendLine();
            logger.AppendLine(info);
            logger.AppendLine();
        }

        internal static void WriteLogToDisk(string path = "")
        {
            if (!active)
            {
                return;
            }

            if (path == string.Empty)
            {
                path = GetLogPath();
            }

            File.WriteAllText(path, logger.ToString());
            InputHelper.SpaceOutText($"Saved log file in {path}!");
        }

    }
}
