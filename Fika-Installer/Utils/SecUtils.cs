using System.Security.Principal;

namespace Fika_Installer.Utils;

public class SecUtils
{
    public static bool IsRunAsAdmin()
    {
        using var identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
