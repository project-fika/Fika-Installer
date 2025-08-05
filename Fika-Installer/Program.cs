namespace Fika_Installer
{
    public class Program
    {
        [STAThread]
        
        static void Main(string[] args)
        {
            AppController uiController = new AppController();
            uiController.Start();
        }
    }
}