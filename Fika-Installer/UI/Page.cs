namespace Fika_Installer.UI
{
    public abstract class Page
    {
        public void Show()
        {
            Header.Show();
            Draw();
        }

        public abstract void Draw();
    }
}
