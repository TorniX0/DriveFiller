namespace DriveFiller
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            InputHelper.SetWindowTitle("DriveFiller");

            string driveLetter = InputHelper.GetAnswer("Drive letter for the drive you want to fill: ");
            Logger.SetActive(InputHelper.YesNo("Log the results?"));
            bool random = InputHelper.YesNo("Variable file-size?");

            if (random)
            {
                int minValue = InputHelper.GetValidatedAnswer($"Minimum value for the variable file-size (measured in MB) (default {UtilsHelper.ConvertStorage(DriveUtilities.minSize)}): ", InputHelper.ValidInteger);
                int maxValue = InputHelper.GetValidatedAnswer($"Maximum value for the variable file-size (measured in MB) (default {UtilsHelper.ConvertStorage(DriveUtilities.maxSize)}): ", InputHelper.ValidInteger);

                while (minValue > maxValue)
                {
                    Console.WriteLine("Invalid answer. Minimum value is bigger than maximum. Please try again.");

                    minValue = InputHelper.GetValidatedAnswer($"Minimum value for the variable file-size (measured in MB) (default {UtilsHelper.ConvertStorage(DriveUtilities.minSize)}): ", InputHelper.ValidInteger);
                    maxValue = InputHelper.GetValidatedAnswer($"Maximum value for the variable file-size (measured in MB) (default {UtilsHelper.ConvertStorage(DriveUtilities.maxSize)}): ", InputHelper.ValidInteger);
                }

                DriveUtilities.UpdateSizes(minValue, maxValue);
            }
            else
            {
                int fixedValue = InputHelper.GetValidatedAnswer($"Fixed value for the file-size (measured in MB) (default {UtilsHelper.ConvertStorage(DriveUtilities.fixedSize)}): ", InputHelper.ValidInteger);

                DriveUtilities.UpdateSizes(fixedValue);
            }

            var drive = DriveUtilities.GetDrive(driveLetter);

            if (drive == null)
            {
                InputHelper.ExitApplication("Drive does not exist. Exiting...");
                return;
            }

            Console.Clear();

            bool continueAnswer = InputHelper.YesNoLoop("The drive filler can be stopped anytime by pressing any key on the command prompt window, keep in mind, use the program at your own risk, do you wish to continue?");

            if (!continueAnswer)
            {
                InputHelper.ExitApplication("User cancelled action. Exiting...");
                return;
            }

            Console.Clear();

            Logger.AddLog($"Started drive filler on drive {drive.Name} at {DateTime.Now}");
            Logger.AddLog($"Settings: VARIABLE_FILESIZE={random};");
            
            if (random)
            {
                Logger.AddLog($"MIN_SIZE={UtilsHelper.ConvertStorage(DriveUtilities.minSize)}; MAX_SIZE={UtilsHelper.ConvertStorage(DriveUtilities.maxSize)};");
            }
            else
            {
                Logger.AddLog($"FIXED_SIZE={UtilsHelper.ConvertStorage(DriveUtilities.fixedSize)}");
            }

            await DriveUtilities.FillDrive(random, drive);

            await DriveUtilities.CleanupMess(drive);

            Logger.AddSpacedLog($"Finished at {DateTime.Now}");
            Logger.WriteLogToDisk();

            InputHelper.ExitApplication("Finished the work!", true);
        }
    }
}