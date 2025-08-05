namespace Fika_Installer.Models.UI
{
    public class MenuResult(string id, ConsoleKey key, bool validEntry)
    {
        public string Id = id;
        public ConsoleKey Key = key;
        public bool ValidEntry = validEntry;
    }
}
