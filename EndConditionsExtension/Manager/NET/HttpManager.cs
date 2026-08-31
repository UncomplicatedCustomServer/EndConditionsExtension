using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MEC;

namespace EndConditionsExtension.Manager.NET;

internal class HttpManager
{
    private const string GitHubReleases =
        "https://github.com/UncomplicatedCustomServer/EndConditionsExtension/releases";

    private const string GitHubLatestRelease = GitHubReleases + "/latest";

    private const string DiscordInvite = "https://discord.gg/5StRGu8EJV";

    /// <summary>
    ///     Gets the prefix of the plugin for our APIs
    /// </summary>
    public string Prefix { get; }

    /// <summary>
    ///     Gets the UCS APIs endpoint
    /// </summary>
    public string Endpoint { get; } = "https://api.ucserver.it/v3/plugin";

    /// <summary>
    ///     Gets every version of the plugin known by the UCS cloud, empty until <see cref="LoadVersions" /> is done
    /// </summary>
    public List<VersionInfo> Versions { get; private set; } = [];

    /// <summary>
    ///     Gets the latest <see cref="Version" /> of the plugin, pre-releases included, loaded by the UCS cloud
    /// </summary>
    public Version LatestVersion { get; private set; } = new();

    /// <summary>
    ///     Gets the latest stable (non pre-release) <see cref="Version" /> of the plugin, loaded by the UCS cloud.
    /// </summary>
    public Version LatestStableVersion { get; private set; } = new();

    /// <summary>
    ///     Gets the latest pre-release <see cref="Version" /> of the plugin, loaded by the UCS cloud.
    /// </summary>
    public Version LatestPreRelease { get; private set; } = new();

    /// <summary>
    ///     Gets whether the running build is a pre-release
    /// </summary>
    public bool IsPreRelease => IsPreReleaseVersion(Plugin.Singleton.Version);

    /// <summary>
    ///     Creates a new instance of the <see cref="HttpManager" />.
    /// </summary>
    /// <param name="prefix">The prefix of the plugin for the UCS APIs.</param>
    public HttpManager(string prefix)
    {
        Prefix = prefix;
    }

    internal static int CompareReleases(Version left, Version right)
    {
        int release = new Version(left.Major, left.Minor, Math.Max(left.Build, 0))
            .CompareTo(new Version(right.Major, right.Minor, Math.Max(right.Build, 0)));

        if (release != 0)
            return release;

        int leftPreRelease = Math.Max(left.Revision, 0);
        int rightPreRelease = Math.Max(right.Revision, 0);

        if (leftPreRelease == rightPreRelease)
            return 0;

        if (leftPreRelease is 0)
            return 1;

        return rightPreRelease is 0 ? -1 : leftPreRelease.CompareTo(rightPreRelease);
    }

    public bool IsPreReleaseVersion(Version version)
    {
        return TryGetVersionInfo(version, out VersionInfo info) ? info.PreRelease != 0 : version.Revision > 0;
    }

    public CoroutineHandle LoadVersions()
    {
        return Timing.RunCoroutine(LoadVersionsCoroutine(), WebQuery.CoroutineTag);
    }

    private IEnumerator<float> LoadVersionsCoroutine()
    {
        Versions = [];
        LatestVersion = new Version();
        LatestStableVersion = new Version();
        LatestPreRelease = new Version();

        yield return Timing.WaitUntilDone(WebQuery.Get($"{Endpoint}/{Prefix}/versions", LoadVersionList));

        if (Versions.Count is 0)
            yield return Timing.WaitUntilDone(WebQuery.Get($"{Endpoint}/{Prefix}/versions/latest@text/plain",
                LoadLatestVersionFallback));
    }

    private void LoadVersionList(HttpResponse response)
    {
        try
        {
            Versions = JsonSerializer.Deserialize<List<VersionInfo>>(response.Body) ?? [];
        }
        catch
        {
            LogManager.Debug(
                $"Failed to load the version list from the UCS cloud ({response.Reason}): '{response.Body}'");
            Versions = [];
            return;
        }

        foreach (VersionInfo version in Versions)
        {
            if (!Version.TryParse(version.Name, out Version parsed))
                continue;

            if (CompareReleases(parsed, LatestVersion) > 0)
                LatestVersion = parsed;

            if (version.PreRelease == 0)
            {
                if (CompareReleases(parsed, LatestStableVersion) > 0)
                    LatestStableVersion = parsed;
            }
            else if (CompareReleases(parsed, LatestPreRelease) > 0)
            {
                LatestPreRelease = parsed;
            }
        }
    }

    /// <summary>
    ///     Loads the latest version from the single-value endpoint, used when the version list is unavailable.
    /// </summary>
    private void LoadLatestVersionFallback(HttpResponse response)
    {
        string answer = response.Body;

        try
        {
            if (string.IsNullOrEmpty(answer) || !answer.Contains("."))
            {
                LogManager.Debug($"The UCS cloud gave us no latest version to fall back on ({response.Reason})");
                return;
            }

            LatestVersion = new Version(answer.Trim());

            if (LatestVersion.Revision <= 0)
                LatestStableVersion = LatestVersion;
            else
                LatestPreRelease = LatestVersion;
        }
        catch
        {
            LogManager.Debug($"Failed to parse the latest version received from the UCS cloud: '{answer}'");
            LatestVersion = new Version();
        }
    }

    /// <summary>
    ///     Tries to get the cloud informations about the given version of the plugin
    /// </summary>
    public bool TryGetVersionInfo(Version version, out VersionInfo info)
    {
        info = Versions.FirstOrDefault(v =>
            Version.TryParse(v.Name, out Version parsed) && CompareReleases(parsed, version) is 0);
        return info is not null;
    }

    private Version ResolveChannelTarget()
    {
        Version target = LatestStableVersion;

        if (IsPreRelease && CompareReleases(LatestPreRelease, target) > 0)
            target = LatestPreRelease;

        return target;
    }

    /// <summary>
    ///     Gets the release the current installation should be updated to, or <see langword="null" /> if there's
    ///     nothing newer to install.
    /// </summary>
    public Version GetUpdateTarget()
    {
        Version target = ResolveChannelTarget();
        return CompareReleases(target, Plugin.Singleton.Version) > 0 ? target : null;
    }

    public string GetDownloadHint(Version version)
    {
        TryGetVersionInfo(version, out VersionInfo info);

        string link = string.IsNullOrWhiteSpace(info?.SourceLink) ? null : info.SourceLink.Trim();

        return info?.Source?.Trim().ToLowerInvariant() switch
        {
            "discord" => $"Download it from our Discord server: {link ?? DiscordInvite}",
            "other" when link is not null => $"Download it from: {link}",
            _ =>
                $"Download it from GitHub: {link ?? (IsPreReleaseVersion(version) ? GitHubReleases : GitHubLatestRelease)}"
        };
    }

    internal CoroutineHandle VersionInfo(Action<HttpResponse> callback)
    {
        return WebQuery.Get($"{Endpoint}/{Prefix}/versions/{Plugin.Singleton.Version}", callback);
    }
}