using EndConditionsExtension.Structures;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UncomplicatedCustomRoles.API.Features;
using UncomplicatedCustomRoles.Structures;

namespace EndConditionsExtension
{
    internal class Handler
    {
        protected LeadingTeam? Leading { get; set; } = LeadingTeam.Draw;
        public void OnEnding(EndingRoundEventArgs Ending)
        {
            bool CanEnd = true;
            foreach (KeyValuePair<int, int> Element in Manager.GetAlive())
            {
                CanEnd = CanEnd && EvaluateEndConditions(Player.Get(Element.Key), Manager.Get(Element.Value));
            }
            if (Leading is not null) {
                Ending.LeadingTeam = (LeadingTeam)Leading;
            }
            Ending.IsAllowed = CanEnd;
        }
        public bool EvaluateEndConditions(Player Player, ICustomRole Role)
        {
            if (Plugin.Istance.Config.EndConditions.ContainsKey(Role.Id))
            {
                IEndCondition EndCondition = Plugin.Istance.Config.EndConditions[Role.Id];
                if (EndCondition.MustRemainOnlyOneTeam)
                {
                    if (Player.List.Count == Player.List.Where(player => player.Role.Team == Player.Role.Team && player.IsAlive).Count())
                    {
                        Leading = EndCondition.WinningTeam;
                        return true;
                    }
                } 
                else
                {
                    List<Team> AliveTeams = new();
                    foreach (Player PseudoPlayer in Player.List.Where(player => player.IsAlive))
                    {
                        if (!AliveTeams.Contains(PseudoPlayer.Role.Team))
                        {
                            AliveTeams.Add(PseudoPlayer.Role.Team);
                        }
                    }
                    if (AliveTeams == EndCondition.RemainingTeams.Keys.ToList())
                    {
                        bool Can = true;
                        int Total = 0;

                        // Let's count the values
                        foreach (Team Team in AliveTeams)
                        {
                            Total += Player.List.Where(player => player.IsAlive && player.Role.Team == Team).Count();
                            Can = Can && Player.List.Where(player => player.IsAlive && player.Role.Team == Team).Count() <= EndCondition.RemainingTeams[Team];
                        }

                        Can = Can && Total <= EndCondition.MaxPlayersToEnd;

                        Leading = EndCondition.WinningTeam;
                        return Can;
                    }
                }
            } 
            else
            {
                return true;
            }
            return false;
        }
    }
}
