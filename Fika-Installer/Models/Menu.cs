namespace Fika_Installer.Models
{
    public class Menu(List<MenuChoice> choices)
    {
        public List<MenuChoice> Choices = choices;

        public void Show()
        {
            Console.Clear();
            Console.WriteLine(Constants.FikaInstallerVersionString);
            Console.WriteLine();
            
            foreach (MenuChoice choice in Choices)
            {
                string text = choice.Text;
                ConsoleKey key = choice.Key;

                Console.WriteLine($"[{(char)key}] {text}");
            }

            ConsoleKeyInfo inputKey = Console.ReadKey(true);

            foreach (MenuChoice choice in Choices)
            {
                if (inputKey.Key == choice.Key)
                {
                    choice.Action?.Invoke();
                }
            }
        }
    }

    public class MenuChoice(string text, ConsoleKey key, Action action)
    {
        public string Text = text;
        public ConsoleKey Key = key;
        public Action Action = action;
    }
}
