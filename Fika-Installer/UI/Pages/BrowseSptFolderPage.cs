using Fika_Installer.Spt;
using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public class BrowseSptFolderPage(ILogger logger) : Page(logger)
    {
        public string? Result;
        
        public override void OnShow()
        {
            CompositeLogger.Confirm("SPT not detected. Press ENTER to browse for your SPT folder.");

            string sptDir = FileUtils.BrowseFolder("Please select your SPT installation folder.");

            if (string.IsNullOrEmpty(sptDir))
            {
                return;
            }

            if (!SptUtils.IsSptInstalled(sptDir))
            {
                CompositeLogger.Error("The selected folder does not contain a valid SPT installation.", true);
                return;
            }

            Result = sptDir;
        }
    }
}