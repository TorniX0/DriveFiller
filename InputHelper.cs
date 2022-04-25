using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace DriveFiller
{
    internal static class InputHelper
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
                if (key != null && key.ToLower() is "y" or "n") 
                {
                    break;
                }

                Console.Clear();
                Console.Write($"{question} (Y/N): ");

                key = Console.ReadLine();
            }

            return key.ToLower() switch
            {
                "y" => true,
                "n" => false,
                _ => false,
            };
        }

        internal static int? ValidInteger(string input)
        {
            if (!double.TryParse(input, out double fixedDouble))
            {
                return null;
            }

            if (Math.Round(fixedDouble) <= 0)
            {
                return null;
            }

            return (int)Math.Round(fixedDouble);
        }

        internal static string GetAnswer(string question)
        {
            while (true)
            {
                Console.Write(question);
                string input = Console.ReadLine() ?? string.Empty;

                if (input != string.Empty)
                {
                    return input;
                }

                Console.WriteLine("Invalid answer. Please try again.");
            }
        }

        internal static T GetValidatedAnswer<T>(string question, Func<string, T?> validator) where T : struct
        {
            while (true)
            {
                Console.Write(question);
                string input = Console.ReadLine() ?? string.Empty;

                var valid = validator(input);

                if (valid != null)
                {
                    return valid.Value;
                }

                Console.WriteLine("Invalid answer. Please try again.");
            }
        }

        internal static T GetValidatedAnswer<T>(string question, Func<string, T?> validator)
        {
            while (true)
            {
                Console.Write(question);
                string input = Console.ReadLine() ?? string.Empty;

                var valid = validator(input);

                if (valid != null)
                {
                    return valid;
                }

                Console.WriteLine("Invalid answer. Please try again.");
            }
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

                if (!prompt) 
                {
                    Thread.Sleep(4000);
                }
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
