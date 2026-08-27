using System;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;

namespace EndConditionsExtension;

internal class Plugin : Plugin<Config>
{
    public override string Name => "EndConditionsExtension";
    public override string Description => "EndConditionsExtension";
    public override string Author => "FoxWorn3365 && MedveMarci";
    public override Version Version => new(1, 0, 0);
    public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);
    public static Plugin Singleton;
    private Handler _handler;
    public override void Enable()
    {
        Singleton = this;
        _handler = new Handler();

        ServerEvents.RoundEnding += _handler.OnEnding;
    }
    public override void Disable()
    {
        ServerEvents.RoundEnding -= _handler.OnEnding;

        Singleton = null;
        _handler = null;
    }
}