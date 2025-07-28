namespace Fika_Installer.Models
{
    public class Page(Action action)
    {
        Action Action { get; set; } = action;

        public void Show()
        {
            Header.Show();
            Action.Invoke();
        }
    }
}
