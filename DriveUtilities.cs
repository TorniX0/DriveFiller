using System.Linq;

namespace DriveFiller
{
    internal static class DriveUtilities
    {
        private static DriveInfo[] allDrives = DriveInfo.GetDrives();
        private static Dictionary<string, int> fileHistory = new();
        internal static int minSize { get; private set; } = 524288000;
        internal static int maxSize { get; private set; } = 1073741824;
        internal static int fixedSize { get; private set; } = 524288000;

        internal static DriveInfo? GetDrive(string input)
        {
            if (!input.All(c => char.IsAscii(c) && char.IsLetter(c)) || input.Length != 1)
            {
                return null;
            }

            foreach (DriveInfo drive in allDrives)
            {
                if (drive.Name.Contains(input))
                {
                    return drive;
                }
            }

            return null;
        }

        private static void AddToFileHistory(string name, int size)
        {
            bool existsAlready = File.Exists(name) && fileHistory.ContainsKey(name);

            while (existsAlready)
            {
                name = UtilsHelper.GenerateRandomName();
            }

            fileHistory.Add(name, size);
        }

        internal static async Task FillDrive(bool variableSize, DriveInfo drive)
        {
            Logger.AddSpacedLog("Write(s):");

            Random r = new();
            int counter = 0;

            while (true)
            {
                counter++;

                string name = UtilsHelper.GenerateRandomName();

                int size = variableSize ? r.Next(minSize, maxSize) : fixedSize;

                AddToFileHistory(name, size);

                byte[] bytes = UtilsHelper.GenerateRandomBytes(size);

                if (drive.AvailableFreeSpace < size)
                {
                    break;
                }

                var timeToWrite = await UtilsHelper.ExecutionTimeAsync(async Task() =>
                {
                    using (var fs = new FileStream(drive.RootDirectory + name, FileMode.CreateNew, FileAccess.Write, FileShare.None, bytes.Length, FileOptions.Asynchronous))
                    {
                        await fs.WriteAsync(bytes, 0, bytes.Length);
                    }
                });

                Console.WriteLine($"Written {name} of size {UtilsHelper.ConvertStorage(size)} in roughly {timeToWrite}ms ({counter})");

                Logger.AddLog($"Written {name} of size {UtilsHelper.ConvertStorage(size)} in roughly {timeToWrite}ms ({counter})");

                if (Console.KeyAvailable && Console.ReadKey(true).Key != default)
                {
                    break;
                }
            }

            Console.Beep();
        }

        internal static async Task CleanupMess(DriveInfo drive)
        {
            Logger.AddSpacedLog("Delete(s):");

            InputHelper.SpaceOutText("Cleaning up generated files...");

            for (int i = 0; i < fileHistory.Keys.Count; i++)
            {
                var timeToDelete = await UtilsHelper.ExecutionTimeAsync(async Task() =>
                {
                    using (var stream = new FileStream(drive.RootDirectory + fileHistory.Keys.ElementAt(i), FileMode.Open, FileAccess.Read, FileShare.None, 1, FileOptions.DeleteOnClose | FileOptions.Asynchronous))
                    {
                        await stream.FlushAsync();
                    }
                });

                Console.WriteLine($"Deleted {fileHistory.Keys.ElementAt(i)} of size {UtilsHelper.ConvertStorage(fileHistory.Values.ElementAt(i))} in roughly {timeToDelete}ms ({i + 1})");

                Logger.AddLog($"Deleted {fileHistory.Keys.ElementAt(i)} of size {UtilsHelper.ConvertStorage(fileHistory.Values.ElementAt(i))} in roughly {timeToDelete}ms ({i + 1})");
            }
        }

        internal static void UpdateSizes(int value)
        {
            fixedSize = value * 1024 * 1024;
        }

        internal static void UpdateSizes(int min, int max)
        {
            maxSize = max * 1024 * 1024;
            minSize = min * 1024 * 1024;
        }

    }
}
