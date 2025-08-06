namespace Fika_Installer
{
    public class Program
    {
        [STAThread]
        
        static void Main(string[] args)
        {
            Console.Title = Constants.FikaInstallerVersionString;

            AppController appController = new AppController();
            appController.Start();
        }
    }
}