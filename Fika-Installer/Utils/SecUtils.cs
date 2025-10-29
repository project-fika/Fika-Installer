using System.Security.Principal;

namespace Fika_Installer.Utils
{
    public class SecUtils
    {
        public static bool IsRunAsAdmin()
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
