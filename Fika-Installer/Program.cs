﻿using Fika_Installer.UI;

namespace Fika_Installer
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            while (true)
            {
                Menus.MainMenu();
            }
        }
    }
}