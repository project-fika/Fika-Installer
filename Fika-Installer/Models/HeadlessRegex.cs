using System.Text.RegularExpressions;

namespace Fika_Installer
{
    public static partial class HeadlessRegex
    {
        [GeneratedRegex(@"Start_headless_([^.]+)", RegexOptions.IgnoreCase)]
        public static partial Regex GeneratedLaunchScriptRegex();
    }
}
