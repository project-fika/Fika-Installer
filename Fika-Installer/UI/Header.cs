using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fika_Installer.UI
{
    public static class Header
    {
        public static void Show()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;

            string fikaInstallerVersionString = Constants.FikaInstallerVersionString;

            int margin = 5;
            int headerBackgroundLength = fikaInstallerVersionString.Length + margin * 2;

            string headerBackground = new(' ', headerBackgroundLength);
            string headerTextSpacer = new(' ', margin);

            string headerText = $"{headerTextSpacer}{fikaInstallerVersionString}{headerTextSpacer}";

            Console.WriteLine(headerBackground);
            Console.WriteLine(headerText);
            Console.WriteLine(headerBackground);

            Console.WriteLine();

            Console.ResetColor();
        }
    }
}
