namespace Fika_Installer.UI
{
    public class MenuChoice
    {
        public string Id { get; }
        public string Text { get; }

        private readonly Action _action;

        public MenuChoice(string text, Action action, string id = "")
        {
            Id = id;
            Text = text;
            _action = action;
        }

        public MenuChoice(string text, Page page, string id = "")
        {
            Id = id;
            Text = text;
            _action = page.Show;
        }

        public MenuChoice(string text, Menu menu, string id = "")
        {
            Id = id;
            Text = text;
            _action = () => menu.Show();
        }

        public MenuChoice(string text, string id = "")
        {
            Id = id;
            Text = text;
            _action = () => { };
        }

        public void Execute()
        {
            _action.Invoke();
        }
    }
}
