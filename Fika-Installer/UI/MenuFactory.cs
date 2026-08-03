using Fika_Installer.Models.Enums;
using Fika_Installer.Models.Spt;
using Fika_Installer.UI.Pages;

namespace Fika_Installer.UI;

public class MenuFactory(string installDir)
{
    public Menu CreateMainMenu()
    {
        List<MenuChoice> mainMenuChoices = [];

        var fikaDetected = File.Exists(Installer.FikaCorePath(installDir));

        if (fikaDetected)
        {
            UpdateFikaPage updateFikaPage = new(installDir);

            MenuChoice updateFikaChoice = new("Update Fika", updateFikaPage);
            mainMenuChoices.Add(updateFikaChoice);

            UninstallFikaPage uninstallFikaPage = new(this, installDir);

            MenuChoice uninstallFikaChoice = new("Uninstall Fika", uninstallFikaPage);
            mainMenuChoices.Add(uninstallFikaChoice);
        }
        else
        {
            InstallFikaPage installFikaPage = new(installDir);

            MenuChoice installFikaChoice = new("Install Fika", installFikaPage);
            mainMenuChoices.Add(installFikaChoice);
        }

        MenuChoice advancedChoice = new("Advanced Options", CreateAdvancedOptionsMenu());
        mainMenuChoices.Add(advancedChoice);

        return new(mainMenuChoices);
    }

    public Menu CreateAdvancedOptionsMenu()
    {
        List<MenuChoice> advancedMenuChoices = [];

        var fikaHeadlessDetected = File.Exists(Installer.FikaHeadlessPath(installDir));

        if (fikaHeadlessDetected)
        {
            UpdateFikaHeadlessPage updateFikaHeadlessPage = new(installDir);

            MenuChoice updateFikaHeadlessChoice = new("Update Fika Headless", updateFikaHeadlessPage);
            advancedMenuChoices.Add(updateFikaHeadlessChoice);
        }
        else
        {
            InstallFikaHeadlessPage installFikaHeadlessPage = new(this, installDir);

            MenuChoice installFikaHeadlessChoice = new("Install Fika Headless", installFikaHeadlessPage);
            advancedMenuChoices.Add(installFikaHeadlessChoice);
        }

        var fikaCoreDetected = File.Exists(Installer.FikaCorePath(installDir));

        if (!fikaCoreDetected)
        {
            InstallFikaCurrentDirPage installFikaCurrentDirPage = new(this, installDir);

            MenuChoice installFikaInCurrentFolder = new("Install Fika in current folder", installFikaCurrentDirPage);
            advancedMenuChoices.Add(installFikaInCurrentFolder);
        }

        AddFirewallRulesPage addFirewallRulesPage = new(installDir);

        MenuChoice addFirewallRulesChoice = new("Add firewall rules for Fika", addFirewallRulesPage);
        advancedMenuChoices.Add(addFirewallRulesChoice);

        MenuChoice backChoice = new("Back", () => { });
        advancedMenuChoices.Add(backChoice);

        return new(advancedMenuChoices);
    }

    public Menu CreateProfileSelectionMenu(List<SptProfile> sptProfiles)
    {
        List<MenuChoice> profileSelectionMenuChoices = [];

        foreach (var profile in sptProfiles)
        {
            MenuChoice menuChoice = new(profile.ProfileId);
            profileSelectionMenuChoices.Add(menuChoice);
        }

        MenuChoice createNewHeadlessProfile = new("Create a new headless profile", "createNewHeadlessProfile");
        profileSelectionMenuChoices.Add(createNewHeadlessProfile);

        return new("Please choose the headless profile to use for your headless client:", profileSelectionMenuChoices);
    }

    public Menu CreateInstallMethodMenu()
    {
        List<MenuChoice> choices = [];

        foreach (var installMethod in Enum.GetNames<InstallMethod>())
        {
            MenuChoice choice = new(installMethod);
            choices.Add(choice);
        }

        return new("Please choose your installation method. This will determine how the SPT folder will be copied.", choices);
    }

    public Menu CreateConfirmUninstallFikaMenu()
    {
        List<MenuChoice> choices = [];

        MenuChoice choiceYes = new("Yes");
        choices.Add(choiceYes);

        MenuChoice choiceNo = new("No");
        choices.Add(choiceNo);

        return new("Are you sure you want to uninstall Fika? Your Fika settings will be lost.", choices);
    }
}
