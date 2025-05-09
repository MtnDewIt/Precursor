using System;

namespace Precursor.Common
{
    public class PrecursorError
    {
        private static readonly object Mutex = new object();

        public PrecursorError(string customMessage = null)
        {
            lock (Mutex)
            {
                if (Console.LargestWindowWidth != 0 && Console.CursorLeft > 0)
                    Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("ERROR: " + customMessage);
                Console.ResetColor();
            }
        }
    }
}
