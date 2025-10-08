using Fika_Installer.Spt;
using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public class BrowseSptFolderPage : Page
    {
        public string? Result { get; set; }

        public override void OnShow()
        {
            ConUtils.WriteConfirm("SPT not detected. Press ENTER to browse for your SPT folder.");

            string selectedFolderPath = FileUtils.BrowseFolder("Please select your SPT installation folder.");

            if (string.IsNullOrEmpty(selectedFolderPath))
            {
                return;
            }

            if (!SptUtils.IsSptInstalled(selectedFolderPath))
            {
                ConUtils.WriteError("The selected folder does not contain a valid SPT installation.", true);
                return;
            }

            Result = selectedFolderPath;
        }
    }
}