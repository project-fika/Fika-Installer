using System.Diagnostics;
using System.Text.Json.Nodes;
using Fika_Installer.Spt;
using Fika_Installer.Utils;

namespace Fika_Installer.Fika;

public sealed class FikaHeadless
{
    private readonly SptInstance _sptInstance;
    private readonly string _fikaServerModPath;
    private readonly string _fikaServerScriptsFolder;
    private readonly string _fikaServerConfigPath;
    private readonly SptServer _sptServer;
    private readonly int _headlessProfileCount;
    private string? _headlessProfileId;

    public FikaHeadless(SptInstance sptInstance)
    {
        _sptInstance = sptInstance;
        _fikaServerModPath = Path.Combine(_sptInstance.SptPath, @"user\mods\fika-server");
        _fikaServerScriptsFolder = Path.Combine(_fikaServerModPath, @"assets\scripts");
        _fikaServerConfigPath = Path.Combine(_fikaServerModPath, @"assets\configs\fika.jsonc");
        _sptServer = new(sptInstance);
        _headlessProfileCount = _sptInstance.GetHeadlessProfiles().Count;
    }

    public string? CreateHeadlessProfile()
    {
        var sptProcessName = "SPT.Server";
        var sptServerRunning = Process.GetProcessesByName(sptProcessName).Length != 0;

        /* Make sure SPT server is not running */
        if (sptServerRunning)
        {
            Logger.Warning("SPT Server is currently running. Please close it to continue the installation.");
        }

        while (sptServerRunning)
        {
            Thread.Sleep(500);
            sptServerRunning = Process.GetProcessesByName(sptProcessName).Length != 0;
        }

        var fikaServerConfig = LoadFikaServerConfig();

        if (fikaServerConfig == null)
        {
            return null;
        }

        /* Ensure that we set the headless amount to the current value to avoid generating multiple headless profiles */
        SetHeadlessAmount(fikaServerConfig, _headlessProfileCount);

        var httpConfig = fikaServerConfig["server"]?["SPT"]?["http"];
        var ip = httpConfig?["ip"]?.GetValue<string>();
        var port = httpConfig?["port"]?.GetValue<int>();
        var apiKey = fikaServerConfig["server"]?["apiKey"]?.GetValue<string>();

        if (string.IsNullOrEmpty(ip) || port == null || string.IsNullOrEmpty(apiKey))
        {
            Logger.Error("Invalid configuration in Fika Server config file.");
            return null;
        }

        if (ip == "0.0.0.0")
        {
            ip = "127.0.0.1";
        }

        Logger.Log("Creating headless profile...");

        /* Start SPT Server and test the connection */
        _sptServer.Start();

        FikaRequestHandler fikaRequestHandler = new($"https://{ip}:{port}", apiKey);

        if (!fikaRequestHandler.WaitForConnection(TimeSpan.FromMinutes(1)))
        {
            Logger.Error("Connection to SPT Server failed.");
            return null;
        }

        /* Create headless profile and stop SPT Server */
        try
        {
            var createHeadlessProfileResponse = fikaRequestHandler.CreateHeadlessProfile();

            if (createHeadlessProfileResponse == null)
            {
                return null;
            }

            _headlessProfileId = createHeadlessProfileResponse.Id;
        }
        catch (Exception ex)
        {
            Logger.Error($"An error occurred when requesting CreateHeadlessProfile. {ex.Message}");
        }

        _sptServer.Stop();

        if (string.IsNullOrEmpty(_headlessProfileId))
        {
            return null;
        }

        /* Increase the headless profile amount */
        SetHeadlessAmount(fikaServerConfig, _headlessProfileCount + 1);

        return _headlessProfileId;
    }

    public bool CopyHeadlessConfig(string profileId, string destPath)
    {
        var headlessConfigFileName = "HeadlessConfig.json";
        var headlessConfigDirPath = Path.Combine(_fikaServerScriptsFolder, profileId);
        var headlessConfigPath = Path.Combine(headlessConfigDirPath, headlessConfigFileName);

        if (File.Exists(headlessConfigPath))
        {
            var headlessConfigDestPath = Path.Combine(destPath, headlessConfigFileName);
            File.Copy(headlessConfigPath, headlessConfigDestPath, true);
        }
        else
        {
            Logger.Error($"Couldn't find HeadlessConfig.json for profile id {profileId}!", true);
            return false;
        }

        return true;
    }

    public bool IsFikaServerInstalled()
    {
        // This is a lazy check and is not indicative of a proper Fika-Server installation.
        return Directory.Exists(_fikaServerModPath);
    }

    private JsonObject? LoadFikaServerConfig()
    {
        if (!IsFikaServerConfigFound())
        {
            Logger.Log("Generating Fika config file...");

            _sptServer.Start();

            var fikaConfigCreated = WaitForFikaConfigCreate(TimeSpan.FromMinutes(1));

            _sptServer.Stop();

            if (!fikaConfigCreated)
            {
                Logger.Error("Fika Server config file was not found.");
                return null;
            }
        }

        return JsonUtils.DeserializeFromFile<JsonObject>(_fikaServerConfigPath);
    }

    private bool SetHeadlessAmount(JsonObject fikaServerConfig, int amount)
    {
        fikaServerConfig["headless"]["profiles"]["amount"] = amount;

        return JsonUtils.SerializeToFile(_fikaServerConfigPath, fikaServerConfig);
    }

    private bool IsFikaServerConfigFound()
    {
        return File.Exists(_fikaServerConfigPath);
    }

    private bool WaitForFikaConfigCreate(TimeSpan timeout)
    {
        var checkInterval = TimeSpan.FromMilliseconds(500);

        var startTime = DateTime.Now;

        while (!IsFikaServerConfigFound())
        {
            Thread.Sleep(checkInterval);

            if (DateTime.Now - startTime > timeout)
            {
                return false;
            }
        }

        // Extra sleep time to ensure the file is fully saved
        Thread.Sleep(TimeSpan.FromSeconds(1));

        return true;
    }
}
