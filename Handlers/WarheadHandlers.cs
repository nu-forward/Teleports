using Exiled.API.Enums;
using Teleports.API;

namespace Teleports.Handlers
{
    internal sealed class WarheadHandlers
    {
        public void OnDetonated()
        {
            Extensions.DenyAllRooms(RoomType.Surface);
        }
    }
}
