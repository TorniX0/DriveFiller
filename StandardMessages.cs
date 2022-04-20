namespace DriveFiller
{
    internal static class StandardMessages
    {
        internal static bool YesNo(string question)
        {
            Console.Write($"{question} (Y/N): ");
            string? answerText = Console.ReadLine();
            return answerText?.ToLower() == "y";
        }

        internal static bool YesNoLoop(string question)
        {
            Console.Write($"{question} (Y/N): ");
            string? key = Console.ReadLine();

            while (true)
            {
                if (key != null && key.ToLower() is "y" or "n") break;
                else
                {
                    key = Console.ReadLine();
                }

                Console.Clear();
                Console.Write($"{question} (Y/N): ");
            }

            return key.ToLower() switch
            {
                "y" => true,
                "n" => false,
                _ => false,
            };
        }

        internal static string AskQuestion(string question)
        {
            Console.Write(question);
            string? answer = Console.ReadLine();

            if (answer != null) return answer;
            else return string.Empty;
        }

        internal static void SpaceOutText(string text)
        {
            Console.WriteLine();
            Console.WriteLine(text);
            Console.WriteLine();
        }

        internal static void SetWindowTitle(string title)
        {
            Console.Title = title;
        }

        internal static void ExitApplication(string reason = "", bool prompt = false)
        {
            if (reason != string.Empty) 
            { 
                SpaceOutText(reason);
                if (!prompt) Thread.Sleep(3000);
            }

            if (prompt)
            {
                SpaceOutText("Press any key to exit...");
                Console.ReadKey();
            }

            Environment.Exit(0);
        }

    }
}
