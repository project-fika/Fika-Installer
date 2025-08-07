using Fika_Installer.Models.UI;

namespace Fika_Installer.UI
{
    public class Menu
    {
        public string Message { get; }
        public List<MenuChoice> Choices { get; }

        private int _totalLines = 0;

        public Menu(List<MenuChoice> choices)
        {
            Choices = choices;
        }

        public Menu(string message, List<MenuChoice> choices)
        {
            Message = message;
            Choices = choices;
        }

        public MenuChoice Show()
        {
            while (true)
            {
                Header.Show();

                if (!string.IsNullOrEmpty(Message))
                {
                    Console.WriteLine(Message);
                    Console.WriteLine();
                    _totalLines += 2;
                }

                for (int i = 0; i < Choices.Count; i++)
                {
                    string choiceText = Choices[i].Text;
                    Console.WriteLine($"[{i + 1}] {choiceText}");
                    _totalLines++;
                }

                ConsoleKeyInfo keyInfoPressed = Console.ReadKey(true);

                string keyPressed = keyInfoPressed.KeyChar.ToString();

                if (int.TryParse(keyPressed, out int choiceNumber))
                {
                    if (choiceNumber >= 1 && choiceNumber <= Choices.Count)
                    {
                        int choiceIndex = choiceNumber - 1;

                        MenuChoice choice = Choices[choiceIndex];

                        ClearMenu(_totalLines);
                        choice.Execute();
                        
                        return choice;
                    }
                }
            }
        }

        private void ClearMenu(int lines)
        {
            int currentLine = Console.CursorTop;

            for (int i = 0; i < lines; i++)
            {
                Console.SetCursorPosition(0, currentLine - i - 1);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, currentLine - lines);
        }
    }
}
