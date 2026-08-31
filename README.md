<div align="center"><a href="https://github.com/UncomplicatedCustomServer/EndConditionsExtension/releases/latest"><img src="https://img.shields.io/github/v/release/UncomplicatedCustomServer/EndConditionsExtension"></a> <a href="https://github.com/UncomplicatedCustomServer/EndConditionsExtension/releases/latest"><img src="https://img.shields.io/github/downloads/UncomplicatedCustomServer/EndConditionsExtension/total"></a> <a href="https://github.com/UncomplicatedCustomServer/EndConditionsExtension/pulls"><img src="https://img.shields.io/github/issues-pr/UncomplicatedCustomServer/EndConditionsExtension"></a> <a href="https://github.com/UncomplicatedCustomServer/EndConditionsExtension/pulls"><img src="https://img.shields.io/github/issues-pr-closed/UncomplicatedCustomServer/EndConditionsExtension"></a> <a href="https://github.com/UncomplicatedCustomServer/EndConditionsExtension/commits/main/"><img src="https://badgen.net/github/commits/UncomplicatedCustomServer/EndConditionsExtension/main"></a> <a href="https://opencollective.com/ucs"><img src="https://img.shields.io/opencollective/all/ucs?label=OpenCollective%20backers&color=7FADF2"></a>

  <h1>EndConditionsExtension</h1>
  <i>Decide when - and for whom - the round ends, straight from your UncomplicatedCustomRoles!</i>
</div>

## Requirements
- **LabAPI** >= `v1.1.7`
- **UncomplicatedCustomRoles** >= `v9.6.0`

<br>

## What's EndConditionsExtension
**EndConditionsExtension** is an extension for [**UncomplicatedCustomRoles**](https://github.com/UncomplicatedCustomServer/UncomplicatedCustomRoles) that lets you decide, with YAML, **when the round is allowed to end** while one of your custom roles is still alive - and **which team wins** when it does.

By default SCP:SL ends the round as soon as only one vanilla team is left standing, which is rarely what you want when a custom role is a third faction, custom team or squad that has to be killed before the round can be over.\
With this extension every custom role gets its own end condition: as long as a player with that role is alive, the round ends **only** if the condition of that role is met.

## Features
### 🛑 The round ends when you say so
While a player with a custom role is alive, the round is blocked until that role's condition is satisfied. No more rounds ending while your third faction is still fighting.
### 🏆 Custom winning team
Choose which team is announced as the winner (`FacilityForces`, `ChaosInsurgency`, `Anomalies` or `Draw`) when the condition of a role is met.
### 👥 Vanilla teams, CustomTeams and fake teams
Players are grouped by their **UCR `CustomTeam` module** if they have one, by the **fake team** of their custom role if it has one, and by their normal team otherwise - so a disguised role counts as the team it is pretending to be, exactly like the rest of UCR sees it.
### 🎚️ Per team limits
Allow a team to be alive only up to a certain amount of players, cap the total amount of survivors of the other teams, or simply require that nothing but your own team is left.
### 🙈 Ignored teams
Tutorials and other spectator-ish roles can be excluded from the evaluation, so they can neither block nor end the round.
### 🥇 Priorities
When more than one custom role meets its condition at the same time, the one with the highest priority decides the winning team.
### 🔄 Update checker
The same version manager used by UCR: on start the plugin asks the UCS cloud whether a newer release is out, tells you when you are running a pre-release, verifies the hash of the file you installed and warns you if that build has been recalled.
### 🗂️ YAML based
Just like UCR, everything is configured through a simple `yml` file - no code needed.

## Configuration
The config file is created at `LabAPI/configs/<port>/EndConditionsExtension/config.yml`.

```yml
# Do enable the debug (developer) mode?
debug: false
# A list of conditions for each CustomRole - the key is the CustomRole Id
end_conditions:
  1:
    # Decide if to end the round must remain only the CustomRole's team, no matter the number
    must_remain_only_one_team: false
    # Which vanilla teams are allowed to be alive and their maximum number of members
    remaining_teams:
      ClassD: 5
      Scientists: 1
    # The same as remaining_teams but for the UCR 'CustomTeam' custom module
    remaining_custom_teams:
      SerpentsHand: 2
    # Vanilla teams that are completely ignored while evaluating this condition
    ignored_teams:
    - OtherAlive
    # How many players of the other teams may be alive to let the round end
    max_players_to_end: 0
    # The team that wins the round when this condition is met
    winning_team: Draw
    # If more than one condition is met at the same time the highest priority decides the winner
    priority: 0
```

| Key | Type | Description |
| --- | --- | --- |
| `must_remain_only_one_team` | `bool` | If `true` the round ends only when **nothing but the role's own team** is alive, no matter how many players it has. Every remaining team limit is ignored. |
| `remaining_teams` | `Team: int` | The vanilla teams that are allowed to still be alive, with the maximum amount of members each of them can have. You don't need to include the role's own team. |
| `remaining_custom_teams` | `string: int` | The same, but for the teams declared through the UCR `CustomTeam` custom module. The name is case insensitive. |
| `ignored_teams` | `Team[]` | Teams that are completely ignored: they can neither block nor end the round. The role's own team is never ignored. |
| `max_players_to_end` | `int` | The maximum total amount of players of the **other** teams that may still be alive to let the round end. |
| `winning_team` | `LeadingTeam` | `FacilityForces`, `ChaosInsurgency`, `Anomalies` or `Draw`. |
| `priority` | `int` | Used to pick the winning team when several conditions are met at once. |

> [!NOTE]
> If both `remaining_teams` and `remaining_custom_teams` are left empty, **every** team is allowed to be alive and only `max_players_to_end` is taken into account.

> [!TIP]
> A dead player never blocks the round: the condition of a custom role is evaluated only while a player with that role is still alive.

## Example
A "Serpent's Hand" custom role (Id `12`) that wins as the Chaos Insurgency, but only once every other team is gone except at most two Class-D:

```yml
end_conditions:
  12:
    must_remain_only_one_team: false
    remaining_teams:
      ClassD: 2
    remaining_custom_teams: {}
    ignored_teams:
    - OtherAlive
    max_players_to_end: 2
    winning_team: ChaosInsurgency
    priority: 10
```

## Installation
1. Install [**UncomplicatedCustomRoles**](https://github.com/UncomplicatedCustomServer/UncomplicatedCustomRoles) - check its [documentation](https://docs.ucr.ucserver.it/getting-started/installation)
2. Drop `EndConditionsExtension.dll` into `LabAPI/plugins/<port>` (or `LabAPI/plugins/global`)
3. Start the server once to generate the config file, then edit it and reload

## If you use our plugins, please consider making a donation
Every plugin made by the **UCS Collective** is **free** and **open-source**, and it always will be.\
What there is, is the time we spend writing the plugins, answering your questions on Discord and keeping everything working after every SCP:SL update.\
If our plugins are running on your server, **please consider donating something through OpenCollective** - every contribution, however small, goes straight back into the plugins you are using:

<a href="https://opencollective.com/ucs"><img height="15" src="https://raw.githubusercontent.com/UncomplicatedCustomServer/UncomplicatedCustomRoles/refs/heads/resources/oc_icon.png">&nbsp;&nbsp;Donate</a>

## Contacts
### UCS - UncomplicatedCustomServer
  **Discord:** [https://discord.gg/5StRGu8EJV](https://discord.gg/5StRGu8EJV)

### FoxWorn3365
  **Discord:** `@foxworn`\
  **Email:** `foxworn3365@gmail.com`
### MedveMarci
  **Discord:** `medvemarci`
