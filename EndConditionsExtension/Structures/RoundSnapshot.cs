using System.Collections.Generic;
using LabApi.Features.Wrappers;
using PlayerRoles;
using UncomplicatedCustomRoles.API.Features;
using UncomplicatedCustomRoles.API.Features.CustomModules;

namespace EndConditionsExtension.Structures;

internal sealed class RoundSnapshot
{
    private readonly Dictionary<int, TeamIdentity> _byPlayer = new();

    private RoundSnapshot()
    {
    }

    public Dictionary<TeamIdentity, int> Counts { get; } = new();

    public int AliveCount { get; private set; }

    public static RoundSnapshot Create()
    {
        RoundSnapshot snapshot = new();

        foreach (var player in Player.ReadyList)
        {
            if (player is null || !player.IsAlive)
                continue;

            var identity = GetIdentity(player);

            snapshot._byPlayer[player.PlayerId] = identity;
            snapshot.Counts[identity] = snapshot.Counts.TryGetValue(identity, out var count) ? count + 1 : 1;
            snapshot.AliveCount++;
        }

        return snapshot;
    }

    public bool TryGetIdentity(Player player, out TeamIdentity identity)
    {
        if (player is not null)
            return _byPlayer.TryGetValue(player.PlayerId, out identity);

        identity = default;
        return false;
    }

    private static TeamIdentity GetIdentity(Player player)
    {
        if (!SummonedCustomRole.TryGet(player, out var summoned) || summoned?.Role is null)
            return TeamIdentity.Vanilla(player.Team);

        var customTeam = GetCustomTeamName(summoned);

        if (customTeam is not null)
            return TeamIdentity.Custom(customTeam);

        var roleTeam = summoned.Role.Role.GetTeam();

        return TeamIdentity.Vanilla(summoned.Role.Team is { } fakeTeam && fakeTeam != roleTeam ? fakeTeam : roleTeam);
    }

    private static string GetCustomTeamName(SummonedCustomRole summoned)
    {
        if (!summoned.TryGetModule(out CustomTeam module))
            return null;

        var name = module.TryGetStringValue("team");
        return string.IsNullOrWhiteSpace(name) ? null : name.Trim();
    }
}