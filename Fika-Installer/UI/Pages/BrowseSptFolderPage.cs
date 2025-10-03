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

            string sptDir = FileUtils.BrowseFolder("Please select your SPT installation folder.");

            if (string.IsNullOrEmpty(sptDir))
            {
                return;
            }

            if (!SptUtils.IsSptInstalled(sptDir))
            {
                ConUtils.WriteError("The selected folder does not contain a valid SPT installation.", true);
                return;
            }

            Result = sptDir;
        }
    }
}