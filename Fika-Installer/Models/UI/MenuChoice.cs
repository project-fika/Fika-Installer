namespace Fika_Installer.Models.UI
{
    public class MenuChoice(string id, string text, ConsoleKey key)
    {
        public string Id = id;
        public string Text = text;
        public ConsoleKey Key = key;
    }
}
