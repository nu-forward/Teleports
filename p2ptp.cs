using CommandSystem;
using Exiled.API.Features;
using System;

namespace TeleportAPI.RA_Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]

    class p2ptp : ParentCommand
    {
        public override string Command { get; } = "TeleportPlayerToPlayer";

        public override string[] Aliases { get; } = { "tpp2p" };

        public override string Description { get; } = "Телепортирует игрока к игроку";

        public override void LoadGeneratedCommands()
        {

        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 2 && int.TryParse(arguments.At(0), out int playerIdWhom) && int.TryParse(arguments.At(1), out int playerIdToWhom))
            {
                Player Whom = Player.Get(playerIdWhom);
                Player ToWhom = Player.Get(playerIdToWhom);
                Plugin.api.TeleportPlayerToPlayer(Whom, ToWhom);
            }
            response = "Телепортация прошла успешно";
            return true;
        }
    }
}