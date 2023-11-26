using Exiled.API.Enums;
using Exiled.Events.EventArgs.Map;
using Teleports.API;

namespace Teleports.Handlers
{
    internal sealed class MapHandlers
    {
        public void OnGenerated() => TeleportsExtensions.AllowAllRooms();

        public void OnDecontaminating(DecontaminatingEventArgs ev)
        {
            if (!ev.IsAllowed)
            {
                return;
            }

            foreach (var room in TeleportsExtensions.Rooms.ToHashSet())
            {
                if (room == null || room.Zone != ZoneType.LightContainment)
                {
                    continue;
                }

                room.DenyTeleport();
            }
        }
    }
}
