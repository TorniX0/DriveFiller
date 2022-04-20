namespace DriveFiller
{
    internal static class DriveUtilities
    {
        private static DriveInfo[] allDrives = DriveInfo.GetDrives();
        private static HashSet<string> nameHistory = new();
        internal static int minSize { get; private set; } = 524288000;
        internal static int maxSize { get; private set; } = 1073741824;
        internal static int fixedSize { get; private set; } = 524288000;

        internal static DriveInfo? GetDrive(string driveLetter)
        {
            foreach (DriveInfo _drv in allDrives)
            {
                if (driveLetter == string.Empty) return null;
                if (_drv.Name.Contains(driveLetter))
                {
                    return _drv;
                }
            }

            return null;
        }

        private static void AddToNameHistory(string name)
        {
            bool existsAlready = File.Exists(name) && nameHistory.Contains(name);

            while (existsAlready)
            {
                name = Helper.GenerateRandomName();
            }

            nameHistory.Add(name);
        }

        internal static void FillDrive(bool variableSize, DriveInfo drive)
        {
            Random r = new();
            int counter = 0;

            while (true)
            {
                counter++;

                string name = Helper.GenerateRandomName();

                AddToNameHistory(name);

                int size = 0;

                if (variableSize) size = r.Next(minSize, maxSize);
                else size = fixedSize;

                byte[] bytes = Helper.GenerateRandomBytes(size);

                if (drive.AvailableFreeSpace < size) break;

                var timeToWrite = Helper.ExecutionTime(() =>
                {
                    using (var fs = new FileStream(drive.RootDirectory + name, FileMode.CreateNew))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                });

                Console.WriteLine($"Written {name} of size {Helper.ConvertStorage(size)} in roughly {timeToWrite}ms ({counter})");

                if (Logger.active) Logger.AddLog($"Written {Helper.ConvertStorage(size)} in roughly {timeToWrite}ms ({counter})");

                if (Console.KeyAvailable && Console.ReadKey(true).Key != default) break;
            }

            Console.Beep();
        }

        internal static void CleanupMess(DriveInfo drive)
        {
            StandardMessages.SpaceOutText("Cleaning up generated files...");

            for (int i = 0; i < nameHistory.Count; i++)
            {
                var timeToDelete = Helper.ExecutionTime(() =>
                {
                    File.Delete(drive.RootDirectory + nameHistory.ElementAt(i));
                });

                Console.WriteLine($"Deleted {nameHistory.ElementAt(i)} in roughly {timeToDelete}ms ({i + 1})");
            }
        }

        internal static void UpdateSizes(string value)
        {
            float fixedFloat = 0f;

            bool parse = !float.TryParse(value, out fixedFloat);
            bool negative = fixedSize <= 0;

            if (parse || negative)
            {
                StandardMessages.ExitApplication("Invalid value(s). Exiting...");
            }

            fixedSize = (int)(fixedFloat * 1024f * 1024f);
        }

        internal static void UpdateSizes(string min, string max)
        {
            float minFloat = 0f;
            float maxFloat = 0f;

            bool parse = !float.TryParse(min, out minFloat) || !float.TryParse(max, out maxFloat);
            bool negative = minFloat <= 0f || maxFloat <= 0f;

            if (parse || negative)
            {
                StandardMessages.ExitApplication("Invalid value(s). Exiting...");
            }

            maxSize = (int)(maxFloat * 1024f * 1024f);
            minSize = (int)(minFloat * 1024f * 1024f);
        }

    }
}
