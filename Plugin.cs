using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TeleportAPI
{
    public class Plugin : Plugin<Config>
    {
        public override string Prefix => "TeleportAPI";
        public override string Name => "TeleportAPI";
        public override string Author => "Timersky";
        public static Plugin api;
        public override Version Version { get; } = new Version(4, 7, 12);
        public override void OnEnabled()
        {
            api = this;
            Exiled.Events.Handlers.Player.ChangingRole += Player_ChangingRole;
            Exiled.Events.Handlers.Player.Left += Player_Left;
            base.OnEnabled();
        }

        internal List<Room> TpAllowedRooms = new List<Room>();
        internal List<string> TpAllowedPlayers = new List<string>();

        public bool PlayerAllowedTP(Player player)
        {
            string userid = player.UserId;
            if (TpAllowedPlayers.Contains(userid))
            {
                Log.Debug("Какой-то плагин проверил лист, результат оказался удачным");
                return (true);
            }
            else
            {
                Log.Debug("Какой-то плагин проверил лист, результат оказался неудачным");
                return (false);
            }
        }

        public void TeleportPlayerToPlayer(Player whom, Player to_whom)
        {
            whom.Teleport(to_whom);
        }

        private void Player_Left(Exiled.Events.EventArgs.Player.LeftEventArgs ev)
        {
            TpAllowedPlayers.Remove(ev.Player.UserId);
        }

        private void Player_ChangingRole(Exiled.Events.EventArgs.Player.ChangingRoleEventArgs ev)
        {
            if (ev.NewRole != RoleTypeId.None || ev.Player.Role.Type != RoleTypeId.Spectator || ev.Player.Role.Type != RoleTypeId.Overwatch || ev.Player.Role.Type != RoleTypeId.Tutorial || ev.Player.Role.Type != RoleTypeId.CustomRole)
            {
                TpAllowedPlayers.Add(ev.Player.UserId);
            }
            else
            {
                TpAllowedPlayers.Remove(ev.Player.UserId);
            }
        }

        private void WarheadDetonated()
        {
            if (Warhead.IsDetonated)
            {
                TpAllowedRooms.Clear();
                TpAllowedRooms.Add(Room.Get(RoomType.Surface));
                Log.Debug("Боеголовка взорвалась убираю все комнаты");
            }
        }

        private void LczDecontaminated()
        {
            if (Map.IsLczDecontaminated)
            {
                if (TpAllowedRooms.Contains(Room.Get(RoomType.Lcz330)))
                {
                    TpAllowedRooms.Remove(Room.Get(RoomType.Lcz330));
                    TpAllowedRooms.Remove(Room.Get(RoomType.LczAirlock));
                    TpAllowedRooms.Remove(Room.Get(RoomType.LczArmory));
                    TpAllowedRooms.Remove(Room.Get(RoomType.LczCafe));
                    TpAllowedRooms.Remove(Room.Get(RoomType.LczCrossing));
                    TpAllowedRooms.Remove(Room.Get(RoomType.LczCurve));
                    TpAllowedRooms.Remove(Room.Get(RoomType.LczPlants));
                    TpAllowedRooms.Remove(Room.Get(RoomType.LczStraight));
                    TpAllowedRooms.Remove(Room.Get(RoomType.LczToilets));
                    TpAllowedRooms.Remove(Room.Get(RoomType.LczClassDSpawn));
                    TpAllowedRooms.Remove(Room.Get(RoomType.LczGlassBox));
                    TpAllowedRooms.Remove(Room.Get(RoomType.LczCheckpointA));
                    TpAllowedRooms.Remove(Room.Get(RoomType.LczCheckpointB));
                    TpAllowedRooms.Remove(Room.Get(RoomType.LczTCross));
                    Log.Debug("Лёгкая зона деконтаминирована убираю все её комнаты");
                }
                else
                {
                    Log.Debug("Лёгкая зона деконтаминирована и все комнаты уже убраны");
                }
            }
        }

        public void TeleportZone(Player ev, ZoneType zoneType)
        {
            ev.Teleport(Room.Random(zoneType));
            return;
        }

        public void TeleportRoom(Player ev, RoomType room)
        {
            ev.Teleport(room);
            return;
        }

        public void TeleportRandomPlayer(Player ev)
        {
            string rplayer = TpAllowedPlayers.ToList().RandomItem();
            if (rplayer == null)
            {
                Log.Error("System.NullReferenceException, тп не будет :(");
                ev.ShowHint("Не твой день");
                return;
            }
            if (rplayer != ev.UserId)
            {
                ev.ShowHint("Телепортация");
                ev.Teleport(rplayer);
            }
            else
            {
                TeleportRandomPlayer(ev);
            }
            return;
        }

        public void TeleportRandomRoom(Player ev)
        {
            TpAllowedRooms.AddRange(Room.List.ToList());
            TpAllowedRooms.Remove(Room.Get(RoomType.EzShelter));
            TpAllowedRooms.Remove(Room.Get(RoomType.EzCollapsedTunnel));
            TpAllowedRooms.Remove(Room.Get(RoomType.HczTestRoom));
            TpAllowedRooms.Remove(Room.Get(RoomType.Lcz173));
            TpAllowedRooms.Remove(Room.Get(RoomType.HczTesla));
            api.LczDecontaminated();
            api.WarheadDetonated();
            Room tpRoom = TpAllowedRooms.ToList().RandomItem();
            if (tpRoom == null)
            {
                Log.Error("System.NullReferenceException, тп не будет :(");
                ev.ShowHint("Не твой день");
                return;
            }
            if (tpRoom.Type == RoomType.Pocket)
            {
                ev.EnableEffect(EffectType.Corroding);
            }
            ev.Teleport(tpRoom);
            return;
        }

        public override void OnDisabled()
        {
            api = null;
            Exiled.Events.Handlers.Player.ChangingRole -= Player_ChangingRole;
            Exiled.Events.Handlers.Player.Left -= Player_Left;
            base.OnDisabled();
        }
    }
}
