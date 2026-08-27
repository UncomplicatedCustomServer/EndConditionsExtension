using EndConditionsExtension.Structures;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using UncomplicatedCustomRoles.API.Features;
using UncomplicatedCustomRoles.API.Interfaces;

namespace EndConditionsExtension;

internal class Handler
{
    protected RoundSummary.LeadingTeam? Leading { get; set; } = RoundSummary.LeadingTeam.Draw;

    public void OnEnding(RoundEndingEventArgs ev)
    {
        bool canEnd = true;
        foreach (KeyValuePair<string, SummonedCustomRole> element in SummonedCustomRole.List)
            canEnd = canEnd && EvaluateEndConditions(Player.Get(element.Key), element.Value.Role);

        if (Leading is not null)
            ev.LeadingTeam = (RoundSummary.LeadingTeam)Leading;

        ev.IsAllowed = canEnd;
    }
    public bool EvaluateEndConditions(Player player, ICustomRole role)
    {
        if (Plugin.Singleton.Config.EndConditions.TryGetValue(role.Id, out var endCondition1))
        {
            IEndCondition endCondition = endCondition1;
            if (endCondition.MustRemainOnlyOneTeam)
            {
                if (Player.ReadyList.Count() == Player.ReadyList.Count(player2 => player2.Team == player.Team && player.IsAlive))
                {
                    Leading = endCondition.WinningTeam;
                    return true;
                }
            } 
            else
            {
                List<Team> aliveTeams = new();
                foreach (Player pseudoPlayer in Player.ReadyList.Where(player2 => player2.IsAlive))
                {
                    if (!aliveTeams.Contains(pseudoPlayer.Team))
                    {
                        aliveTeams.Add(pseudoPlayer.Team);
                    }
                }
                if (aliveTeams == endCondition.RemainingTeams.Keys.ToList())
                {
                    bool can = true;
                    int total = 0;

                    foreach (Team team in aliveTeams)
                    {
                        total += Player.ReadyList.Count(player2 => player2.IsAlive && player2.Team == team);
                        can = can && Player.ReadyList.Count(player2 => player2.IsAlive && player2.Team == team) <= endCondition.RemainingTeams[team];
                    }

                    can = can && total <= endCondition.MaxPlayersToEnd;

                    Leading = endCondition.WinningTeam;
                    return can;
                }
            }
        } 
        else
            return true;

        return false;
    }
}