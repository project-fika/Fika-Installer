namespace Fika_Installer.Models.Spt;

public sealed record SptProfile(
    string ProfileId,
    string Name,
    bool Headless
);