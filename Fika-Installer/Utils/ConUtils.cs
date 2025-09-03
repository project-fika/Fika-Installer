namespace Fika_Installer.Utils
{
    public static class ConUtils
    {
        public static void WriteSuccess(string message, bool confirm = false)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            if (confirm)
            {
                WriteConfirm(message, true);
            }
            else
            {
                Console.WriteLine(message);
            }

            Console.ResetColor();
        }

        public static void WriteError(string message, bool confirm = false)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            if (confirm)
            {
                WriteConfirm(message, true);
            }
            else
            {
                Console.WriteLine(message);
            }

            Console.ResetColor();
        }

        public static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteConfirm(string message, bool confirmMessage = false)
        {
            Console.WriteLine(message);

            if (confirmMessage)
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to continue.");
            }

            Console.ReadKey(true);
        }

        public static void WriteCurrentLine(string message)
        {
            int top = Console.GetCursorPosition().Top;
            Console.SetCursorPosition(0, top);
            Console.Write(message);
        }
    }
}
