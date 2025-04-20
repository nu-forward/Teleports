using Exiled.Events.Handlers;
using System;
using Teleports.Handlers;

namespace Teleports
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        private MapHandlers _map;
        private PlayerHandlers _player;
        private WarheadHandlers _warhead;

        public override string Prefix => "Teleports";

        public override string Name => "Teleports";

        public override string Author => "wexelsdev & NotAloneAgain";

        public override Version Version { get; } = new Version(5, 0, 2);

        public override Version RequiredExiledVersion { get; } = new Version(8, 4, 0);

        public override void OnEnabled()
        {
            _map = new();
            _player = new();
            _warhead = new();

            Map.Generated += _map.OnGenerated;
            Map.Decontaminating += _map.OnDecontaminating;

            Player.ChangingRole += _player.OnChangingRole;
            Player.Left += _player.OnLeft;

            Warhead.Detonated += _warhead.OnDetonated;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Warhead.Detonated -= _warhead.OnDetonated;

            Player.Left -= _player.OnLeft;
            Player.ChangingRole -= _player.OnChangingRole;

            Map.Generated -= _map.OnGenerated;
            Map.Decontaminating -= _map.OnDecontaminating;

            _map = null;
            _player = null;
            _warhead = null;

            base.OnDisabled();
        }

        public override void OnReloaded() { }

        public override void OnRegisteringCommands() { }

        public override void OnUnregisteringCommands() { }
    }
}
