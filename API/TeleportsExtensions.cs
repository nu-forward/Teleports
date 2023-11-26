using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Teleports.API
{
    public static class TeleportsExtensions
    {
        private static HashSet<Room> _rooms;
        private static HashSet<Player> _players;

        static TeleportsExtensions()
        {
            _rooms = new HashSet<Room>(100);

            _players = new HashSet<Player>(100);
        }

        public static IReadOnlyCollection<Room> Rooms => _rooms;

        public static IReadOnlyCollection<Player> Players => _players;

        public static void AllowTeleport(this Room room) => _rooms.Add(room);

        public static void AllowTeleport(this Player player) => _players.Add(player);

        public static void DenyTeleport(this Room room) => _rooms.Remove(room);

        public static void DenyTeleport(this Player player) => _players.Remove(player);

        public static void AllowAllRooms(params RoomType[] ignoredRooms)
        {
            foreach (var room in Room.List)
            {
                if (room == null || ignoredRooms.Contains(room.Type))
                {
                    continue;
                }

                _rooms.Add(room);
            }
        }

        public static void AllowAllPlayers(params RoleTypeId[] ignoredRoles)
        {
            foreach (var player in Player.List)
            {
                if (player == null || player.IsHost || player.IsNPC || ignoredRoles.Contains(player.Role.Type))
                {
                    continue;
                }

                _players.Add(player);
            }
        }

        public static void DenyAllRooms(params RoomType[] ignoredRooms)
        {
            foreach (var room in Room.List)
            {
                if (room == null || !_rooms.Contains(room) || ignoredRooms.Contains(room.Type))
                {
                    continue;
                }

                _rooms.Remove(room);
            }
        }

        public static void DenyAllPlayers(params RoleTypeId[] ignoredRoles)
        {
            foreach (var player in Player.List)
            {
                if (player == null || !_players.Contains(player) || player.IsHost || player.IsNPC || ignoredRoles.Contains(player.Role.Type))
                {
                    continue;
                }

                _players.Remove(player);
            }
        }

        public static void DenyAllRooms() => _rooms.Clear();

        public static void DenyAllPlayers() => _players.Clear();

        public static void TeleportToRandomPlayer(this Player player)
        {
            if (Player.List.Count(ply => ply != null && !ply.IsHost && !ply.IsNPC) == 1 || _players.Count(ply => ply.UserId != player.UserId) < 1)
            {
                player.ShowHint("Недостаточно игроков!");

                return;
            }

            Player target = _players.GetRandomValue();

            if (target == null || target.IsHost || target.IsNPC)
            {
                player.ShowHint("Не твой день");

                return;
            }

            while (target.UserId == player.UserId)
            {
                target = _players.GetRandomValue();
            }

            player.ShowHint("Телепортация");

            player.Position = target.Position + Vector3.up;
        }

        public static void TeleportToRandomRoom(this Player player)
        {
            Room room = Rooms.GetRandomValue();

            if (room == null)
            {
                player.ShowHint("Не твой день");

                return;
            }

            if (room.Type == RoomType.Pocket)
            {
                player.EnableEffect(EffectType.Corroding);
            }

            player.Position = room.GetSafePosition() + Vector3.up * 2;
        }

        public static Vector3 GetSafePosition(this Room room)
        {
            if (!room.Type.IsDangerous())
            {
                return room.Position;
            }

            return room.Doors.GetRandomValue().Position;
        }

        public static bool IsDangerous(this RoomType room) => room switch
        {
            RoomType.EzShelter or RoomType.EzCollapsedTunnel or RoomType.HczTestRoom or RoomType.HczTestRoom or RoomType.Lcz173 or RoomType.Lcz330 => true,
            _ => false
        };
    }
}
