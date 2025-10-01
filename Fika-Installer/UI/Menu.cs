namespace Fika_Installer.UI
{
    public class Menu
    {
        public string Message { get; }
        public List<MenuChoice> Choices { get; }

        private int _paginationIndex = 0;
        private int _menuStartPos = 0;

        public Menu(List<MenuChoice> choices)
        {
            Choices = choices;
            _menuStartPos = Console.CursorTop;
        }

        public Menu(string message, List<MenuChoice> choices)
        {
            Message = message;
            Choices = choices;
            _menuStartPos = Console.CursorTop;
        }

        public MenuChoice Show()
        {
            while (true)
            {
                bool paging = Choices.Count > 9;
                int pageSize = paging ? 8 : 9;

                if (!string.IsNullOrEmpty(Message))
                {
                    Console.WriteLine(Message);
                    Console.WriteLine();
                }

                int remaining = Choices.Count - _paginationIndex;
                int pageChoiceCount = Math.Min(pageSize, remaining);
                int nextChoiceNumber = pageChoiceCount + 1;

                for (int i = 0; i < pageChoiceCount; i++)
                {
                    string choiceText = Choices[_paginationIndex + i].Text;
                    Console.WriteLine($"[{i + 1}] {choiceText}");
                }

                if (paging)
                {
                    Console.WriteLine($"[{nextChoiceNumber}] Next");
                }

                ConsoleKeyInfo keyInfoPressed = Console.ReadKey(true);

                string keyPressed = keyInfoPressed.KeyChar.ToString();

                if (int.TryParse(keyPressed, out int choiceNumber))
                {
                    if (paging)
                    {
                        if (choiceNumber == nextChoiceNumber)
                        {
                            ClearMenu();

                            _paginationIndex += pageChoiceCount;

                            if (_paginationIndex >= Choices.Count)
                            {
                                _paginationIndex = 0;
                            }

                            continue;
                        }
                    }

                    if (choiceNumber >= 1 && choiceNumber <= pageChoiceCount)
                    {
                        ClearMenu();

                        int choiceIndex = _paginationIndex + choiceNumber - 1;
                        MenuChoice choice = Choices[choiceIndex];

                        choice.Execute();

                        return choice;
                    }
                }

                ClearMenu();
            }
        }

        private void ClearMenu()
        {
            int currentPos = Console.CursorTop;

            for (int i = currentPos; i >= _menuStartPos; i--)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, _menuStartPos);
        }
    }
}
