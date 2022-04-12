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

        private static int minSize = 524288000;
        private static int maxSize = 1073741824;
        private static int fixedSize = 524288000;

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
                >= 1024 and < 1024000 => string.Concat(size / 1024, " KB"),
                >= 1024000 and < 1024000000 => string.Concat(size / 1024 / 1024, " MB"),
                >= 1024000000 => string.Concat(size / 1024 / 1024 / 1024, " GB"),
                _ => string.Concat(size, " B"),
            };
        }

        private static bool YesNo(string question)
        {
            Console.Write($"{question} (Y/N): ");
            string? answerText = Console.ReadLine();
            return answerText?.ToLower() == "y";
        }

        private static bool YesNoLoop(string question)
        {
            Console.Write($"{question} (Y/N): ");
            string? key = Console.ReadLine();

            while (true)
            {
                if (key != null && key.ToLower() is "y" or "n") break;
                else
                {
                    Console.WriteLine();
                    Console.Write($"{question} (Y/N): ");
                    key = Console.ReadLine();
                }
            }

            return key.ToLower() switch
            {
                "y" => true,
                "n" => false,
                _ => false,
            };
        }

        private static string CustomQuestion(string question)
        {
            Console.Write(question);
            string? driveLetter = Console.ReadLine();

            if (driveLetter != null) return driveLetter;
            else return string.Empty;
        }

        private static void SpaceOutText(string text)
        {
            Console.WriteLine();
            Console.WriteLine(text);
            Console.WriteLine();
        }


        private static void Main(string[] args)
        {
            Console.Title = "DriveFiller";

            DriveInfo? drive = null;

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            string driveLetter = CustomQuestion("Drive letter for the drive you want to fill: ");
            bool log = YesNo("Log the results?");
            bool random = YesNo("Variable file-size?");

            if (random)
            {
                string minValue = CustomQuestion($"Minimum value for the variable file-size (measured in MB) (default {ConvertStorage(minSize)}): ");
                string maxValue = CustomQuestion($"Maximum value for the variable file-size (measured in MB) (default {ConvertStorage(maxSize)}): ");

                bool parse = !int.TryParse(minValue, out minSize) || !int.TryParse(maxValue, out maxSize);
                bool negative = minSize <= 0 || maxSize <= 0;

                if (parse || negative)
                {
                    Console.WriteLine("Invalid value(s). Exiting...");
                    Thread.Sleep(3000);
                    return;
                }

                maxSize *= 1024 * 1024;
                minSize *= 1024 * 1024;
            }
            else
            {
                string fixedValue = CustomQuestion($"Fixed value for the file-size (measured in MB) (default {ConvertStorage(fixedSize)}): ");

                bool parse = !int.TryParse(fixedValue, out fixedSize);
                bool negative = fixedSize <= 0;

                if (parse || negative)
                {
                    Console.WriteLine("Invalid value(s). Exiting...");
                    Thread.Sleep(3000);
                    return;
                }

                fixedSize *= 1024 * 1024;
            }


            foreach (DriveInfo _drv in allDrives)
            {
                if (driveLetter == string.Empty) break;
                if (_drv.Name.Contains(driveLetter))
                {
                    drive = _drv;
                    break;
                }
            }

            if (drive == null)
            {
                SpaceOutText("Drive does not exist. Exiting...");
                Thread.Sleep(3000);
                return;
            }
            else
            {
                Console.Clear();

                bool continueAnswer = YesNoLoop("The drive filler can be stopped anytime by pressing any key on the command prompt window, keep in mind, use the program at your own risk, do you wish to continue?");

                if (!continueAnswer) return;

                Console.Clear();

                if (log) 
                {
                    logger.AppendLine($"Started drive filler on drive {drive.Name} at {DateTime.Now}"); 
                    logger.AppendLine("Write(s):");
                }

                Random r = new();
                int counter = 0;

                while (true)
                {
                    counter++;

                    string name = GenerateRandomName();

                    bool existsAlready = File.Exists(name) && nameHistory.Contains(name);

                    while (existsAlready)
                    {
                        name = GenerateRandomName();
                    }

                    nameHistory.Add(name);

                    int size = 0;

                    if (random) size = r.Next(minSize, maxSize);
                    else size = fixedSize;

                    byte[] bytes = GenerateRandomBytes(size);

                    if (drive.AvailableFreeSpace < size) break;

                    stpWatch.Start();

                    using (var fs = new FileStream(drive.RootDirectory + name, FileMode.CreateNew))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }

                    stpWatch.Stop();

                    Console.WriteLine($"Written {name} of size {ConvertStorage(size)} in roughly {stpWatch.ElapsedMilliseconds}ms ({counter})");

                    if (log) logger.AppendLine($"Written {ConvertStorage(size)} in roughly {stpWatch.ElapsedMilliseconds}ms ({counter})");

                    stpWatch.Reset();

                    if (Console.KeyAvailable && Console.ReadKey(true).Key != default) break;
                }
            }

            Console.Beep();

            SpaceOutText("Cleaning up generated files...");

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
                SpaceOutText($"Saved log file in {logPath}!");
            }

            Console.WriteLine("Finished! Press any key to exit...");

            Console.ReadKey();
        }
    }
}