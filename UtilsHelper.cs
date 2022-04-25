using System.Diagnostics;

namespace DriveFiller
{
    internal static class UtilsHelper
    {

        internal static double ExecutionTime(Action act)
        {
            Stopwatch watch = new();
            watch.Restart();
            act.Invoke();
            watch.Stop();
            return watch.Elapsed.TotalMilliseconds;
        }

        internal static async Task<double> ExecutionTimeAsync(Func<Task> func)
        {
            Stopwatch watch = new();
            watch.Restart();
            await func.Invoke();
            watch.Stop();
            return watch.Elapsed.TotalMilliseconds;
        }

        internal static byte[] GenerateRandomBytes(int size)
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

        internal static string GenerateRandomName()
        {
            return "drivefiller_" + Path.GetRandomFileName().Replace(".", "");
        }

        internal static string ConvertStorage(int size)
        {
            return size switch
            {
                >= 1024 and < 1024000 => string.Concat(size / 1024, " KB"),
                >= 1024000 and < 1024000000 => string.Concat(size / 1024 / 1024, " MB"),
                >= 1024000000 => string.Concat(size / 1024 / 1024 / 1024, " GB"),
                _ => string.Concat(size, " B"),
            };
        }

    }
}
