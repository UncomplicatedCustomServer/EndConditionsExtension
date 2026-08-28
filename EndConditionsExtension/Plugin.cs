using System;
using System.Collections.Generic;
using EndConditionsExtension.Elements;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;

namespace EndConditionsExtension;

internal class Plugin : Plugin<Config>
{
    public static Plugin Singleton;
    private EventHandler _eventHandler;
    public override string Name => "EndConditionsExtension";
    public override string Description => "EndConditionsExtension";
    public override string Author => "FoxWorn3365 && MedveMarci";
    public override Version Version => new(2, 0, 0);
    public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);

    public override void Enable()
    {
        Singleton = this;
        _eventHandler = new EventHandler();

        ValidateConfig();

        ServerEvents.RoundEnding += EventHandler.OnRoundEnding;
    }

    public override void Disable()
    {
        if (_eventHandler is not null)
            ServerEvents.RoundEnding -= EventHandler.OnRoundEnding;

        Singleton = null;
        _eventHandler = null;
    }
    
    internal static void Debug(string message)
    {
        if (Singleton?.Config.Debug is true)
            Logger.Debug($"[{nameof(EndConditionsExtension)}] {message}");
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
                Logger.Warn($"[{Name}] The end condition of the CustomRole {pair.Key} is empty and will be ignored!");
                continue;
            }

            if (condition.MaxPlayersToEnd < 0)
            {
                Logger.Warn(
                    $"[{Name}] 'max_players_to_end' of the CustomRole {pair.Key} is negative, it has been clamped to 0!");
                condition.MaxPlayersToEnd = 0;
            }

            if (condition.MustRemainOnlyOneTeam &&
                (condition.RemainingTeams is { Count: > 0 } || condition.RemainingCustomTeams is { Count: > 0 }))
                Logger.Warn(
                    $"[{Name}] The CustomRole {pair.Key} has 'must_remain_only_one_team' enabled, every remaining team limit will be ignored!");
        }
    }
}