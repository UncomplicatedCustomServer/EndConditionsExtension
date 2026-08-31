using System;
using System.Collections.Generic;
using EndConditionsExtension.Elements;
using EndConditionsExtension.Manager;
using EndConditionsExtension.Structures;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using PlayerRoles;
using UncomplicatedCustomRoles.API.Features;

namespace EndConditionsExtension;

internal class EventHandler
{
    private static bool _welcomeShown;

    public static void OnRoundEnding(RoundEndingEventArgs ev)
    {
        Dictionary<int, EndCondition> conditions = Plugin.Singleton?.Config.EndConditions;

        if (conditions is null || conditions.Count == 0 || SummonedCustomRole.List.IsEmpty)
            return;

        RoundSnapshot snapshot = null;
        bool hasWinner = false;
        int winnerPriority = int.MinValue;
        RoundSummary.LeadingTeam winningTeam = RoundSummary.LeadingTeam.Draw;

        foreach (SummonedCustomRole summoned in SummonedCustomRole.List.Values)
        {
            Player player = summoned?.Player;

            if (player is null || summoned.Role is null || !player.IsAlive)
                continue;

            if (!conditions.TryGetValue(summoned.Role.Id, out EndCondition condition) || condition is null)
                continue;

            snapshot ??= RoundSnapshot.Create();

            if (!Evaluate(player, condition, snapshot))
            {
                LogManager.Debug(
                    $"The round can't end: the condition of the CustomRole {summoned.Role.Id} [{summoned.Role.Name}] of {player.Nickname} is not met.");
                ev.IsAllowed = false;
                return;
            }

            if (hasWinner && condition.Priority <= winnerPriority)
                continue;

            hasWinner = true;
            winnerPriority = condition.Priority;
            winningTeam = condition.WinningTeam;
        }

        if (!hasWinner)
            return;

        LogManager.Debug($"Every end condition is met, the round will end with {winningTeam} as the leading team.");
        ev.LeadingTeam = winningTeam;
    }

    public static void OnWaitingForPlayer()
    {
        if (_welcomeShown) return;
        _welcomeShown = true;
        LogManager.Info(
            $"Thanks for using {Plugin.Singleton.Name} v{Plugin.Singleton.Version} by {Plugin.Singleton.Author}!",
            ConsoleColor.Blue);
        LogManager.Info(
            "To receive support and to stay up-to-date, join our official Discord server: https://discord.gg/5StRGu8EJV",
            ConsoleColor.DarkYellow);
    }


    private static bool Evaluate(Player player, IEndCondition condition, RoundSnapshot snapshot)
    {
        if (!snapshot.TryGetIdentity(player, out TeamIdentity own))
            return true;

        bool hasLimits = condition.RemainingTeams is { Count: > 0 } ||
                         condition.RemainingCustomTeams is { Count: > 0 };
        int ownCount = 0;
        int othersCount = 0;

        foreach (KeyValuePair<TeamIdentity, int> pair in snapshot.Counts)
        {
            if (pair.Key.Equals(own))
            {
                ownCount += pair.Value;
                continue;
            }

            if (IsIgnored(condition, pair.Key))
                continue;

            othersCount += pair.Value;

            if (condition.MustRemainOnlyOneTeam)
                return false;

            if (hasLimits && (!TryGetLimit(condition, pair.Key, out int limit) || pair.Value > limit))
                return false;
        }

        if (ownCount == 0)
            return false;

        return condition.MustRemainOnlyOneTeam ? othersCount == 0 : othersCount <= condition.MaxPlayersToEnd;
    }

    private static bool IsIgnored(IEndCondition condition, TeamIdentity identity)
    {
        if (identity.IsCustom || condition.IgnoredTeams is not { Count: > 0 })
            return false;

        foreach (Team t in condition.IgnoredTeams)
            if (t == identity.Team)
                return true;

        return false;
    }

    private static bool TryGetLimit(IEndCondition condition, TeamIdentity identity, out int limit)
    {
        limit = 0;

        if (!identity.IsCustom)
            return condition.RemainingTeams is not null &&
                   condition.RemainingTeams.TryGetValue(identity.Team, out limit);

        if (condition.RemainingCustomTeams is null)
            return false;

        foreach (KeyValuePair<string, int> pair in condition.RemainingCustomTeams)
        {
            if (!string.Equals(pair.Key?.Trim(), identity.CustomTeam, StringComparison.OrdinalIgnoreCase))
                continue;

            limit = pair.Value;
            return true;
        }

        return false;
    }
}