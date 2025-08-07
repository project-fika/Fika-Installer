namespace Fika_Installer.UI
{
    public abstract class Page
    {
        public void Show()
        {
            Header.Show();
            OnShow();
        }
        
        public abstract void OnShow();
    }
}
