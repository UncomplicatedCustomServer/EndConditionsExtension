using System.Collections.Generic;
using System.ComponentModel;
using EndConditionsExtension.Structures;
using PlayerRoles;

namespace EndConditionsExtension.Elements;

internal class EndCondition : IEndCondition
{
    [Description("Decide if to end the round must remain only the CustomRole's team, no matter the number")]
    public bool MustRemainOnlyOneTeam { get; set; } = false;

    [Description(
        "If must_remain_only_one_team is false here you can decide which vanilla teams are allowed to be alive and the maximum number of members that they can have. Leave both this and remaining_custom_teams empty to allow every team\n# You don't need to include here the role's own team")]
    public Dictionary<Team, int> RemainingTeams { get; set; } = new()
    {
        { Team.ClassD, 5 },
        { Team.Scientists, 1 }
    };

    [Description(
        "The same as remaining_teams but for the UCR 'CustomTeam' custom module - the key is the team name (case insensitive)")]
    public Dictionary<string, int> RemainingCustomTeams { get; set; } = new();

    [Description(
        "Vanilla teams that are completely ignored while evaluating this condition (they can neither block nor end the round)")]
    public List<Team> IgnoredTeams { get; set; } = [Team.OtherAlive];

    [Description(
        "Here you can decide how many people of the other teams are needed to keep the round going (this will be effective if must_remain_only_one_team is false)")]
    public int MaxPlayersToEnd { get; set; }

    [Description("Set the team who will win if this condition will be true")]
    public RoundSummary.LeadingTeam WinningTeam { get; set; } = RoundSummary.LeadingTeam.Draw;

    [Description(
        "If more than one condition is met at the same time the one with the highest priority decides the winner")]
    public int Priority { get; set; }
}