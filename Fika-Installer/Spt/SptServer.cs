using Fika_Installer.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Timer = System.Threading.Timer;

namespace Fika_Installer.Spt
{
    public class SptServer
    {
        private readonly List<MatchAction> _matchActions = new();
        private readonly Regex _sptErrorRegex = new(@"Error", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public string ExePath { get; set; }
        public TimeSpan KillAfter { get; set; } = Timeout.InfiniteTimeSpan;

        public SptServer(SptInstance sptInstance)
        {
            ExePath = sptInstance.ServerExePath;
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

        public void AddMatchAction(MatchAction matchAction)
        {
            _matchActions.Add(matchAction);
        }

        private void HandleOutput(Process process, string line)
        {
            foreach (var matchAction in _matchActions)
            {
                Match match = matchAction.Pattern.Match(line);

                if (match.Success)
                {
                    try
                    {
                        matchAction.Action(process, match);
                        matchAction.Success = true;
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

        private void HandleError(Process process, string line)
        {
            if (!process.HasExited)
            {
                process.Kill();
            }
        }
    }
}
