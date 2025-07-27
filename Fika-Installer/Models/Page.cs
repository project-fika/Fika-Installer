namespace Fika_Installer.Models
{
    public class Page(Action action)
    {
        Action Action { get; set; } = action;

        public void Show()
        {
            InitPage();
            Action.Invoke();
        }

        private static void InitPage()
        {
            Console.Clear();
            Console.WriteLine(Constants.FikaInstallerVersionString);
            Console.WriteLine();
        }
    }
}
