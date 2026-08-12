using Fika_Installer.Fika;
using Fika_Installer.Models.Enums;
using Fika_Installer.Spt;
using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages;

public partial class PageFunctions
{
    public static void InstallHeadless(string installDir, string sptFolder, string? headlessProfileId, InstallMethod installMethod)
    {
        Logger.Log("Installing Fika Headless...");

        var headlessDetected = File.Exists(Installer.FikaHeadlessPath(installDir));

        if (headlessDetected)
        {
            Logger.Error("Fika Headless is already installed.", true);
            return;
        }

        SptInstance sptInstance;

        var isSptInstalled = SptUtils.IsSptInstalled(installDir);

        if (isSptInstalled)
        {
            sptInstance = new(installDir);
        }
        else
        {
            sptInstance = new(sptFolder);
        }

        FikaHeadless fikaHeadless = new(sptInstance);

        if (!fikaHeadless.IsFikaServerInstalled())
        {
            Logger.Error("Fika-Server must be installed before installing Fika-Headless.", true);
            return;
        }

        if (headlessProfileId == null || headlessProfileId == "new")
        {
            headlessProfileId = fikaHeadless.CreateHeadlessProfile();

            if (string.IsNullOrWhiteSpace(headlessProfileId))
            {
                Logger.Error("An error occurred while creating the headless profile. Please check the SPT Server logs.", true);
                return;
            }
        }

        if (string.IsNullOrWhiteSpace(headlessProfileId))
        {
            Logger.Error("Headless profile ID unexpectedly null.", true);
            return;
        }

        if (!fikaHeadless.CopyHeadlessConfig(headlessProfileId, installDir))
        {
            Logger.Error("Failed to copy headless profile config.", true);
            return;
        }

        var isSptFolderDetected = SptUtils.IsSptFolderDetected(installDir);

        if (!isSptInstalled && !isSptFolderDetected)
        {
            SptInstaller selectedSptInstaller = new(sptFolder);

            if (!selectedSptInstaller.InstallSpt(installDir, installMethod, true))
            {
                return;
            }
        }

        SptInstaller sptInstaller = new(installDir);

        if (!sptInstaller.InstallSptRequirements(installDir))
        {
            Logger.Error("Failed to install SPT requirements.", true);
            return;
        }

        FikaInstaller fikaInstaller = new(installDir);

        if (!fikaInstaller.InstallReleaseList(FikaReleaseLists.HeadlessFika))
        {
            return;
        }

        FwUtils.CreateFirewallRules(installDir);

        Logger.Success("Fika Headless installed successfully!", true);
    }
}

public class InstallFikaHeadlessPage(MenuFactory menuFactory, string installDir) : Page
{
    private string? _headlessProfileId;

    public override void OnShow()
    {
        SptInstance? sptInstance;
        var installMethod = InstallMethod.HardCopy;

        var isSptInstalled = SptUtils.IsSptInstalled(installDir);

        if (isSptInstalled)
        {
            sptInstance = new(installDir);
        }
        else
        {
            BrowseSptFolderPage browseSptFolderPage = new();
            browseSptFolderPage.Show();

            if (browseSptFolderPage.Result == null)
            {
                return;
            }

            sptInstance = new(browseSptFolderPage.Result);
        }

        var sptProfiles = sptInstance.GetHeadlessProfiles();

        var profileSelectionMenu = menuFactory.CreateProfileSelectionMenu(sptProfiles);
        var profileSelectionChoice = profileSelectionMenu.Show();

        if (profileSelectionChoice.Id == "createNewHeadlessProfile")
        {
            _headlessProfileId = null;
        }
        else
        {
            _headlessProfileId = profileSelectionChoice.Text;
        }

        if (!isSptInstalled)
        {
            var installMethodMenu = menuFactory.CreateInstallMethodMenu();
            var installMethodMenuChoice = installMethodMenu.Show();

            if (!Enum.TryParse(installMethodMenuChoice.Text, out installMethod))
            {
                // This should never happen
                return;
            }
        }

        PageFunctions.InstallHeadless(installDir, sptInstance.ClientPath, _headlessProfileId, installMethod);
    }
}
