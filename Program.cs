namespace DriveFiller
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            StandardMessages.SetWindowTitle("DriveFiller");

            string driveLetter = StandardMessages.AskQuestion("Drive letter for the drive you want to fill: ");
            Logger.SetActive(StandardMessages.YesNo("Log the results?"));
            bool random = StandardMessages.YesNo("Variable file-size?");

            if (random)
            {
                string minValue = StandardMessages.AskQuestion($"Minimum value for the variable file-size (measured in MB) (default {Helper.ConvertStorage(DriveUtilities.minSize)}): ");
                string maxValue = StandardMessages.AskQuestion($"Maximum value for the variable file-size (measured in MB) (default {Helper.ConvertStorage(DriveUtilities.maxSize)}): ");

                DriveUtilities.UpdateSizes(minValue, maxValue);
            }
            else
            {
                string fixedValue = StandardMessages.AskQuestion($"Fixed value for the file-size (measured in MB) (default {Helper.ConvertStorage(DriveUtilities.fixedSize)}): ");

                DriveUtilities.UpdateSizes(fixedValue);
            }

            var drive = DriveUtilities.GetDrive(driveLetter);

            if (drive == null)
            {
                StandardMessages.ExitApplication("Drive does not exist. Exiting...");
                return;
            }

            Console.Clear();

            bool continueAnswer = StandardMessages.YesNoLoop("The drive filler can be stopped anytime by pressing any key on the command prompt window, keep in mind, use the program at your own risk, do you wish to continue?");

            if (!continueAnswer) return;

            Console.Clear();

            if (Logger.active) 
            {
                Logger.AddLog($"Started drive filler on drive {drive.Name} at {DateTime.Now}");
                Logger.AddLog("Write(s):");
            }

            DriveUtilities.FillDrive(random, drive);

            DriveUtilities.CleanupMess(drive);

            if (Logger.active)
            {
                Logger.AddLog($"Finished at {DateTime.Now}");
                Logger.WriteLogToDisk();
            }

            StandardMessages.ExitApplication("Finished the work!", true);
        }
    }
}