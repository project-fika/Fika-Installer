using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Fika_Installer.Models
{
    public class MatchAction
    {
        public Regex Pattern { get; }
        public Action<Process, Match> Action { get; }
        public bool Success { get; set; }

        public MatchAction(string pattern, Action<Process, Match> action,
            RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase)
        {
            Pattern = new Regex(pattern, options);
            Action = action;
        }
    }
}
