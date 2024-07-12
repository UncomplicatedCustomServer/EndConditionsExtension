using Exiled.API.Features;
using System;
using ServerHandler = Exiled.Events.Handlers.Server;

namespace EndConditionsExtension
{
    internal class Plugin : Plugin<Config>
    {
        public override string Name => "EndConditionsExtension";
        public override string Author => "FoxWorn3365";
        public override string Prefix => "EndConditionsExtension";
        public override Version Version => new(0, 9, 0);
        public override Version RequiredExiledVersion => new(8, 8, 0);
        public static Plugin Istance;
        internal Handler Handler;
        public override void OnEnabled()
        {
            Istance = this;
            Handler = new();

            ServerHandler.EndingRound += Handler.OnEnding;

            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            ServerHandler.EndingRound -= Handler.OnEnding;

            Istance = null;
            Handler = null;

            base.OnDisabled();
        }
    }
}
