using System.Collections.Generic;
using PlayerRoles;

namespace EndConditionsExtension.Structures;

internal interface IEndCondition
{
    /// <summary>
    ///     Gets or sets whether only the CustomRole's own team has to remain alive to end the round.
    /// </summary>
    bool MustRemainOnlyOneTeam { get; set; }

    /// <summary>
    ///     Gets or sets the vanilla teams that are allowed to be alive and their maximum member count.
    /// </summary>
    Dictionary<Team, int> RemainingTeams { get; set; }

    /// <summary>
    ///     Gets or sets the UCR CustomTeam names that are allowed to be alive and their maximum member count.
    /// </summary>
    Dictionary<string, int> RemainingCustomTeams { get; set; }

    /// <summary>
    ///     Gets or sets the vanilla teams that are completely ignored while evaluating the condition.
    /// </summary>
    List<Team> IgnoredTeams { get; set; }

    /// <summary>
    ///     Gets or sets the maximum amount of players (of the other teams) that may be alive to end the round.
    /// </summary>
    int MaxPlayersToEnd { get; set; }

    /// <summary>
    ///     Gets or sets the team that wins the round when this condition is met.
    /// </summary>
    RoundSummary.LeadingTeam WinningTeam { get; set; }

    /// <summary>
    ///     Gets or sets the priority of this condition when more than one condition is met at the same time.
    /// </summary>
    int Priority { get; set; }
}