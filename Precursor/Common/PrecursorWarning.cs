using System;

namespace Precursor.Common
{
    public class PrecursorWarning
    {
        private static readonly object Mutex = new object();

        public PrecursorWarning(string customMessage = null)
        {
            lock (Mutex)
            {
                // if we're not at the start of the line, insert a new one to avoid ugliness with Console.Write()
                if (Console.LargestWindowWidth != 0 && Console.CursorLeft > 0)
                    Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("WARNING: " + customMessage);
                Console.ResetColor();
            }
        }
    }
}
