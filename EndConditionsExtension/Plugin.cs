using System;
using System.Collections.Generic;
using EndConditionsExtension.Elements;
using EndConditionsExtension.Manager;
using EndConditionsExtension.Manager.NET;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using MEC;

namespace EndConditionsExtension;

internal class Plugin : Plugin<Config>
{
    public static Plugin Singleton;
    internal static HttpManager HttpManager;

    public override string Name => "EndConditionsExtension";

    public override string Description => "EndConditionsExtension";

    public override string Author => "FoxWorn3365 && MedveMarci";

    public override Version Version => new(1, 0, 0);

    public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);

    public override void Enable()
    {
        Singleton = this;

        ValidateConfig();

        HttpManager = new HttpManager("ece");
        Timing.RunCoroutine(VersionManager.Init(), WebQuery.CoroutineTag);

        ServerEvents.RoundEnding += EventHandler.OnRoundEnding;
        ServerEvents.WaitingForPlayers += EventHandler.OnWaitingForPlayer;
    }

    public override void Disable()
    {
        ServerEvents.RoundEnding -= EventHandler.OnRoundEnding;
        ServerEvents.WaitingForPlayers -= EventHandler.OnWaitingForPlayer;

        Timing.KillCoroutines(WebQuery.CoroutineTag);
        HttpManager = null;

        Singleton = null;
    }

    private void ValidateConfig()
    {
        if (Config.EndConditions is not { Count: > 0 })
            return;

        foreach (KeyValuePair<int, EndCondition> pair in Config.EndConditions)
        {
            EndCondition condition = pair.Value;

            if (condition is null)
            {
                LogManager.Warn($"The end condition of the CustomRole {pair.Key} is empty and will be ignored!");
                continue;
            }

            if (condition.MaxPlayersToEnd < 0)
            {
                LogManager.Warn(
                    $"'max_players_to_end' of the CustomRole {pair.Key} is negative, it has been clamped to 0!");
                condition.MaxPlayersToEnd = 0;
            }

            if (condition.MustRemainOnlyOneTeam &&
                (condition.RemainingTeams is { Count: > 0 } || condition.RemainingCustomTeams is { Count: > 0 }))
                LogManager.Warn(
                    $"The CustomRole {pair.Key} has 'must_remain_only_one_team' enabled, every remaining team limit will be ignored!");
        }
    }
}