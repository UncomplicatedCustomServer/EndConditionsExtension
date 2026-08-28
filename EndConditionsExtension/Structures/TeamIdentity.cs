using System;
using PlayerRoles;

namespace EndConditionsExtension.Structures;

internal readonly struct TeamIdentity : IEquatable<TeamIdentity>
{
    private TeamIdentity(Team team, string customTeam)
    {
        Team = team;
        CustomTeam = customTeam;
    }

    public Team Team { get; }

    public string CustomTeam { get; }

    public bool IsCustom => CustomTeam is not null;

    public static TeamIdentity Vanilla(Team team)
    {
        return new TeamIdentity(team, null);
    }

    public static TeamIdentity Custom(string customTeam)
    {
        return new TeamIdentity(Team.OtherAlive, customTeam);
    }

    public bool Equals(TeamIdentity other)
    {
        if (IsCustom || other.IsCustom)
            return IsCustom && other.IsCustom &&
                   string.Equals(CustomTeam, other.CustomTeam, StringComparison.OrdinalIgnoreCase);

        return Team == other.Team;
    }

    public override bool Equals(object obj)
    {
        return obj is TeamIdentity other && Equals(other);
    }

    public override int GetHashCode()
    {
        return IsCustom ? StringComparer.OrdinalIgnoreCase.GetHashCode(CustomTeam) : (int)Team;
    }

    public override string ToString()
    {
        return IsCustom ? $"CustomTeam:{CustomTeam}" : Team.ToString();
    }
}