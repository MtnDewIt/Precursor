using System;

namespace PrecursorShell.Common
{
    public class PrecursorWarning
    {
        private static readonly object Mutex = new object();

        public PrecursorWarning(string customMessage = null)
        {
            lock (Mutex)
            {
                if (Console.LargestWindowWidth != 0 && Console.CursorLeft > 0)
                    Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("WARNING: " + customMessage);
                Console.ResetColor();
            }
        }
    }
}
