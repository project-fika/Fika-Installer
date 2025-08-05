using Fika_Installer.UI;

namespace Fika_Installer.Controllers
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
