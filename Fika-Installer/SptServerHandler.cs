using Fika_Installer.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Timer = System.Threading.Timer;

namespace Fika_Installer
{
    public class SptServerHandler
    {
        public string ExePath { get; set; }
        public TimeSpan KillAfter { get; set; } = Timeout.InfiniteTimeSpan;
        public bool Success { get; private set; } = false;

        private readonly List<MatchAction> _matchActions = new();
        private readonly Regex _sptErrorRegex = new(@"Error", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public SptServerHandler(string path) 
        {
            ExePath = path;
        }

        public void AddMatchAction(MatchAction matchAction)
        {
            _matchActions.Add(matchAction);
        }

        public void HandleOutput(Process process, string line)
        {
            foreach (var matchAction in _matchActions)
            {
                Match match = matchAction.Pattern.Match(line);
                if (match.Success)
                {
                    try
                    {
                        matchAction.Action(process, match);
                        Success = true;
                    }
                    catch
                    {

                    }
                }

                if (_sptErrorRegex.IsMatch(line))
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
            }
        }

        public void HandleError(Process process, string line)
        {
            if (!process.HasExited)
            {
                process.Kill();
            }
        }

        public void Start()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ExePath,
                WorkingDirectory = Path.GetDirectoryName(ExePath),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                Timer timeoutTimer = new(_ =>
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }, null, KillAfter, Timeout.InfiniteTimeSpan);

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        HandleOutput(process, e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    HandleError(process, e.Data);
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
                timeoutTimer.Dispose();
            }
        }
    }
}
