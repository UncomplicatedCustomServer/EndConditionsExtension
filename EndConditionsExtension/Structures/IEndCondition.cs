using Exiled.API.Enums;
using PlayerRoles;
using System.Collections.Generic;

namespace EndConditionsExtension.Structures
{
    internal interface IEndCondition
    {
        public abstract bool MustRemainOnlyOneTeam { get; set; }
        public abstract Dictionary<Team, int> RemainingTeams { get; set; }
        public abstract int MaxPlayersToEnd { get; set; }
        public abstract LeadingTeam WinningTeam { get; set; }
    }
}
