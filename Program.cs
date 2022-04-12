using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace DriveFiller
{
    internal class Program
    {
        private static HashSet<string> nameHistory = new();
        private static Stopwatch stpWatch = new();
        private static StringBuilder logger = new();
        private static readonly string logFileName = Regex.Replace($"DriveFiller-{DateTime.Now}.log", "[\\/:*?\"<>|]", "-").Replace(" ", "_");
        private static readonly string logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), logFileName);

        private static byte[] GenerateRandomBytes(int size)
        {
            char[] characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_".ToCharArray();
            Random random = new();
            byte[] bytes = new byte[size];


            for (int i = 0; i < size; i++)
            {
                bytes[i] = (byte)characters[random.Next(characters.Length)];
            }

            return bytes;
        }

        private static string GenerateRandomName()
        {
            return "drivefiller_" + Path.GetRandomFileName().Replace(".", "");
        }

        private static string ConvertStorage(int size)
        {
            return size switch
            {
                >= 1024 and < 1024000 => string.Concat((size / 1024d).ToString("N1"), " KB"),
                >= 1024000 and < 1024000000 => string.Concat((size / 1024d / 1024d).ToString("N1"), " MB"),
                >= 1024000000 => string.Concat((size / 1024d / 1024d / 1024d).ToString("N1"), " GB"),
                _ => string.Concat(size, " B"),
            };
        }


        private static void Main(string[] args)
        {
            Console.Title = "DriveFiller";

            DriveInfo? drive = null;

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            Console.Write("Drive letter for the drive you want to fill: ");

            string? driveLetter = Console.ReadLine();

            Console.Write("Log the results? (Y/N): ");

            string? logString = Console.ReadLine();

            bool log = logString is not null && logString.ToLower() is "y";

            foreach (DriveInfo _drv in allDrives)
            {
                if (driveLetter is null) break;
                if (_drv.Name.Contains(driveLetter))
                {
                    drive = _drv;
                    break;
                }
            }

            if (drive is null)
            {
                Console.WriteLine("Drive does not exist. Exiting...");
                Thread.Sleep(3000);
                return;
            }
            else
            {
                Console.Clear();
                Console.Write("The drive filler can be stopped anytime by pressing any key on the command prompt window, keep in mind this probably does stress your drive, do you wish to continue? (Y/N): ");
                string? key = Console.ReadLine();

                while (true)
                {
                    if (key is not null && key.ToLower() is "y" or "n") break;
                    else
                    {
                        Console.WriteLine();
                        Console.Write("The drive filler can be stopped anytime by pressing any key on the command prompt window, keep in mind this probably does stress your drive, do you wish to continue? (Y/N): ");
                        key = Console.ReadLine();
                    }
                }

                switch (key.ToLower())
                {
                    case "y":
                        break;
                    case "n":
                        return;
                    default:
                        return;
                }

                Console.Clear();

                if (log) 
                {
                    logger.AppendLine($"Started drive filler on drive {drive.Name} at {DateTime.Now}"); 
                    logger.AppendLine($"Write(s):");
                }

                Random r = new();
                int counter = 0;

                while (drive.AvailableFreeSpace > 594000000)
                {
                    counter++;

                    string name = GenerateRandomName();

                    if (nameHistory.Contains(name))
                    {
                        while (nameHistory.Contains(name))
                        {
                            name = GenerateRandomName();
                        }
                    }

                    nameHistory.Add(name);

                    int rand = r.Next(9999999, 99999999);

                    stpWatch.Start();

                    using (var fs = new FileStream(drive.RootDirectory + name, FileMode.CreateNew))
                    {
                        byte[] bytes = GenerateRandomBytes(rand);
                        fs.Write(bytes, 0, bytes.Length);
                    }

                    stpWatch.Stop();

                    Console.WriteLine($"Written {name} of size {ConvertStorage(rand)} in roughly {stpWatch.ElapsedMilliseconds}ms ({counter})");

                    if (log) logger.AppendLine($"Written {ConvertStorage(rand)} in roughly {stpWatch.ElapsedMilliseconds}ms ({counter})");

                    stpWatch.Reset();

                    if (Console.KeyAvailable && Console.ReadKey(true).Key != default) break;
                }
            }

            Console.Beep();

            Console.WriteLine();
            Console.WriteLine("Cleaning up generated files...");
            Console.WriteLine();

            for (int i = 0; i < nameHistory.Count; i++)
            {
                stpWatch.Start();

                File.Delete(drive.RootDirectory + nameHistory.ElementAt(i));

                stpWatch.Stop();

                Console.WriteLine($"Deleted {nameHistory.ElementAt(i)} in roughly {stpWatch.ElapsedMilliseconds}ms ({i + 1})");

                stpWatch.Reset();
            }

            if (log)
            {
                logger.AppendLine($"Finished at {DateTime.Now}");
                File.WriteAllText(logPath, logger.ToString());
                Console.WriteLine($"Saved log file in {logPath}!");
            }

            Console.WriteLine();
            Console.WriteLine("Finished! Press any key to exit...");

            Console.ReadKey();
        }
    }
}