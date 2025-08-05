using Fika_Installer.Models.UI;
using Fika_Installer.UI;

namespace Fika_Installer.Controllers
{
    public class Menu
    {
        public string Message;
        public List<MenuChoice> Choices;

        public Menu(List<MenuChoice> choices)
        {
            Choices = choices;
        }

        public Menu(string message, List<MenuChoice> choices)
        {
            Message = message;
            Choices = choices;
        }

        public MenuResult Show()
        {
            MenuResult menuResult = new("InvalidEntry", ConsoleKey.D0, false);

            while (!menuResult.ValidEntry)
            {
                Console.Clear();
                Header.Show();

                if (!string.IsNullOrEmpty(Message))
                {
                    Console.WriteLine(Message);
                }

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
                        string id = choice.Id;
                        menuResult = new(id, inputKey.Key, true);

                        Header.Show(); // clears the menu
                    }
                }
            }

            return menuResult;
        }
    }
}
