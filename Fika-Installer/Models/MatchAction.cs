using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Fika_Installer.Models
{
    public class MatchAction(string pattern, Action<Process, Match> action,
        RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase)
    {
        public Regex Pattern { get; } = new Regex(pattern, options);
        public Action<Process, Match> Action { get; } = action;
        public bool Success { get; set; }
    }
}
