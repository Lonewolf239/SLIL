using System;
using System.Linq;
using System.Reflection;
using MaseGenerator.Rooms;
using System.Collections.Generic;

namespace MazeGenerator
{
    internal class Generator
    {
        private Room[] AllPossibleRooms { get; }
        private readonly List<Rectangle> OccupiedCoordinates;
        private readonly Random Rand;
        private bool HasShop = false;
        private bool HasFinish = false;

        internal Generator()
        {
            Rand = new Random(Guid.NewGuid().GetHashCode());
            OccupiedCoordinates = new List<Rectangle>();
            AllPossibleRooms = GetFinalRooms();
        }

        private static Room[] GetFinalRooms()
        {
            var roomType = typeof(Room);
            var derivedTypes = Assembly.GetAssembly(roomType).GetTypes().Where(t => roomType.IsAssignableFrom(t) && !t.IsAbstract);
            var finalRooms = new List<Room>();
            foreach (var type in derivedTypes)
            {
                var roomInstance = (Room)Activator.CreateInstance(type, 0, 0);
                if (roomInstance.IsFinalRoom) finalRooms.Add(roomInstance);
            }
            return finalRooms.ToArray();
        }

        private List<Room> GetRooms(RoomType type)
        {
            var rooms = new List<Room>();
            switch (type)
            {
                case RoomType.Start: rooms.AddRange(AllPossibleRooms.OfType<StartRoom>()); break;
                case RoomType.Secret: rooms.AddRange(AllPossibleRooms.OfType<SecretRoom>()); break;
                case RoomType.Simple: rooms.AddRange(AllPossibleRooms.OfType<SimpleRoom>()); break;
                case RoomType.Hallway: rooms.AddRange(AllPossibleRooms.OfType<HallwayRoom>()); break;
                case RoomType.Shop: rooms.AddRange(AllPossibleRooms.OfType<ShopRoom>()); break;
                case RoomType.Finish: rooms.AddRange(AllPossibleRooms.OfType<FinishRoom>()); break;
            }
            if (rooms.Count == 0) throw new InvalidOperationException("No rooms available of the specified type.");
            return rooms;
        }

        private bool GetRoom(List<Room> rooms, ref Room randomRoom, DoorDirections doorDirections, bool useRandom = true)
        {
            for (int attempts = 0; attempts < 5000; attempts++)
            {
                randomRoom = rooms[Rand.Next(rooms.Count)];
                if (useRandom && Rand.NextDouble() > randomRoom.RoomVariationSpawnChance) continue;
                if (randomRoom.ThereIsMatchingDoor(doorDirections)) return true;
            }
            return false;
        }

        private (bool, Room) GetRandomRoom(RoomType type, bool useRandom = true, DoorDirections doorDirections = DoorDirections.None)
        {
            var rooms = GetRooms(type);
            var randomRoom = rooms[Rand.Next(rooms.Count)];
            bool ok = true;
            if (doorDirections != DoorDirections.None)
                ok = GetRoom(rooms, ref randomRoom, doorDirections, useRandom);
            var newRoom = (Room)Activator.CreateInstance(randomRoom.GetType(), 0, 0);
            return (ok, newRoom);
        }

        private Coordinates GetNewRoomCoordinates(Room currentRoom, Room newRoom, DoorDirections doorCRDirection, Coordinates doorCoordinate)
        {
            const int overlapOffset = 1;
            switch (doorCRDirection)
            {
                case DoorDirections.Up: return new Coordinates(currentRoom.Left + doorCoordinate.X - newRoom.GetDoorCoordinates(DoorDirections.Down).X, currentRoom.Top - newRoom.Height + overlapOffset);
                case DoorDirections.Down: return new Coordinates(currentRoom.Left + doorCoordinate.X - newRoom.GetDoorCoordinates(DoorDirections.Up).X, currentRoom.Bottom - overlapOffset);
                case DoorDirections.Left: return new Coordinates(currentRoom.Left - newRoom.Width + overlapOffset, currentRoom.Top + doorCoordinate.Y - newRoom.GetDoorCoordinates(DoorDirections.Right).Y);
                default: return new Coordinates(currentRoom.Right - overlapOffset, currentRoom.Top + doorCoordinate.Y - newRoom.GetDoorCoordinates(DoorDirections.Left).Y);
            }
        }

        private static DoorDirections GetReverseDoorDirections(DoorDirections doorDirection)
        {
            switch (doorDirection)
            {
                case DoorDirections.Up: return DoorDirections.Down;
                case DoorDirections.Down: return DoorDirections.Up;
                case DoorDirections.Left: return DoorDirections.Right;
                default: return DoorDirections.Left;
            }
        }

        private (bool, Room, Coordinates[]) GetCurrentRoom(ref Queue<Room> queue)
        {
            if (queue.Count == 0) return (false, null, null);
            var currentRoom = queue.Dequeue();
            if (currentRoom.DoorsCount == 0) return (false, null, null);
            var doorCRCoordinates = currentRoom.DoorCoordinates;
            return (true, currentRoom, doorCRCoordinates);
        }

        private bool CanPlaceRoom(Room newRoom, Coordinates coordinates)
        {
            var newRoomRectangle = new Rectangle(coordinates, newRoom.Width, newRoom.Height);
            foreach (var occupied in OccupiedCoordinates)
            {
                if (occupied.Overlaps(newRoomRectangle) || newRoomRectangle.Overlaps(occupied))
                    return false;
            }
            return true;
        }

        private void AddRoom(RoomType typeRoom, bool useRandom, Room currentRoom, Coordinates doorCRCoordinate, DoorDirections doorCRDirection, ref List<Room> rooms, ref Queue<Room> queue, ref int roomsCount)
        {
            var (randomOk, newRandomRoom) = GetRandomRoom(typeRoom, useRandom, doorCRDirection);
            if (!randomOk) return;
            var newRoomCoordinates = GetNewRoomCoordinates(currentRoom, newRandomRoom, doorCRDirection, doorCRCoordinate);
            if (CanPlaceRoom(newRandomRoom, newRoomCoordinates))
            {
                newRandomRoom.Coordinates = newRoomCoordinates;
                OccupiedCoordinates.Add(new Rectangle(newRandomRoom.Coordinates, newRandomRoom.Width, newRandomRoom.Height));
                var doorNRCoordinates = newRandomRoom.GetDoorCoordinates(GetReverseDoorDirections(doorCRDirection));
                if (doorNRCoordinates.ValidValue()) newRandomRoom.RemoveDoor('d', doorNRCoordinates);
                currentRoom.RemoveDoor('d', doorCRCoordinate);
                rooms.Add(newRandomRoom);
                queue.Enqueue(newRandomRoom);
                roomsCount++;
                if (newRandomRoom is ShopRoom) HasShop = true;
                if (newRandomRoom is FinishRoom) HasFinish = true;
            }
        }

        private Room[] GenerateRooms(int maxRoomsCount)
        {
            HasShop = false;
            HasFinish = false;
            OccupiedCoordinates.Clear();
            var startRoom = GetRandomRoom(RoomType.Start).Item2;
            OccupiedCoordinates.Add(new Rectangle(startRoom.Coordinates, startRoom.Width, startRoom.Height));
            var rooms = new List<Room> { startRoom };
            var queue = new Queue<Room>();
            queue.Enqueue(startRoom);
            int roomsCount = 1;
            while (queue.Count > 0 && roomsCount < maxRoomsCount)
            {
                bool needAddShop = false;
                bool needAddFinish = false;
                if (roomsCount + 2 == maxRoomsCount)
                {
                    if (!HasShop) needAddShop = true;
                    else if (!HasFinish) needAddFinish = true;
                }
                if (roomsCount + 1 == maxRoomsCount) { if (!HasFinish) needAddFinish = true; }
                var (ok, currentRoom, doorCRCoordinates) = GetCurrentRoom(ref queue);
                if (!ok) continue;
                foreach (var doorCRCoordinate in doorCRCoordinates)
                {
                    double chanceOfContinuedGeneration = currentRoom.ChanceOfContinuedGeneration * (1.0 - (roomsCount / (double)maxRoomsCount));
                    if (Rand.NextDouble() > chanceOfContinuedGeneration)
                    {
                        currentRoom.RemoveDoor('Q', doorCRCoordinate);
                        continue;
                    }
                    var doorCRDirection = currentRoom.GetDoorDirection(doorCRCoordinate);
                    foreach (var type in currentRoom.PossibleConnections)
                    {
                        if (needAddShop) AddRoom(RoomType.Shop, false, currentRoom, doorCRCoordinate, doorCRDirection, ref rooms, ref queue, ref roomsCount);
                        else if (needAddFinish) AddRoom(RoomType.Finish, false, currentRoom, doorCRCoordinate, doorCRDirection, ref rooms, ref queue, ref roomsCount);
                        else
                        {
                            if (Rand.NextDouble() > Room.GetRoomFromType(type).RoomGenerationChance) continue;
                            AddRoom(type, true, currentRoom, doorCRCoordinate, doorCRDirection, ref rooms, ref queue, ref roomsCount);
                        }
                    }
                }
            }
            return ShiftAllRoomsIfNegative(rooms);
        }

        private Room[] GenerateRooms(Room[] roomsArray, int maxRoomsCount)
        {
            HasShop = roomsArray.Any(room => room is ShopRoom);
            HasFinish = roomsArray.Any(room => room is FinishRoom);
            OccupiedCoordinates.Clear();
            var rooms = new List<Room>();
            var queue = new Queue<Room>();
            int roomsCount = 0;
            foreach (var room in roomsArray)
            {
                room.Restore();
                OccupiedCoordinates.Add(new Rectangle(room.Coordinates, room.Width, room.Height));
                queue.Enqueue(room);
                roomsCount++;
            }
            rooms.AddRange(roomsArray);
            while (queue.Count > 0 && roomsCount < maxRoomsCount)
            {
                bool needAddShop = false;
                bool needAddFinish = false;
                if (roomsCount + 2 == maxRoomsCount)
                {
                    if (!HasShop) needAddShop = true;
                    else if (!HasFinish) needAddFinish = true;
                }
                if (roomsCount + 1 == maxRoomsCount) { if (!HasFinish) needAddFinish = true; }
                var (ok, currentRoom, doorCRCoordinates) = GetCurrentRoom(ref queue);
                if (!ok) continue;
                foreach (var doorCRCoordinate in doorCRCoordinates)
                {
                    var doorCRDirection = currentRoom.GetDoorDirection(doorCRCoordinate);
                    foreach (var type in currentRoom.PossibleConnections)
                    {
                        if (needAddShop) AddRoom(RoomType.Shop, false, currentRoom, doorCRCoordinate, doorCRDirection, ref rooms, ref queue, ref roomsCount);
                        else if (needAddFinish) AddRoom(RoomType.Finish, false, currentRoom, doorCRCoordinate, doorCRDirection, ref rooms, ref queue, ref roomsCount);
                        else AddRoom(type, true, currentRoom, doorCRCoordinate, doorCRDirection, ref rooms, ref queue, ref roomsCount);
                    }                    
                }
            }
            return ShiftAllRoomsIfNegative(rooms);
        }

        private Room[] ShiftAllRoomsIfNegative(List<Room> rooms)
        {
            int minX = rooms.Min(room => room.Coordinates.X);
            int minY = rooms.Min(room => room.Coordinates.Y);
            if (minX < 0 || minY < 0)
            {
                int shiftX = minX < 0 ? -minX + 1 : 0;
                int shiftY = minY < 0 ? -minY + 1 : 0;
                foreach (var room in rooms)
                    room.Coordinates = new Coordinates(room.Coordinates.X + shiftX, room.Coordinates.Y + shiftY);
            }
            return rooms.ToArray();
        }

        public Map GenerateMap(int maxRoomsCount)
        {
            var rooms = GenerateFinalRooms(maxRoomsCount);
            int totalHeight = 0;
            int totalWidth = 0;
            foreach (var room in rooms)
            {
                totalHeight = Math.Max(totalHeight, room.Bottom + 3);
                totalWidth = Math.Max(totalWidth, room.Right + 3);
            }
            char[,] map = new char[totalHeight, totalWidth];
            for (int i = 0; i < totalHeight; i++)
            {
                for (int j = 0; j < totalWidth; j++)
                    map[i, j] = '.';
            }
            foreach (var room in rooms)
            {
                for (int i = 0; i < room.Height; i++)
                {
                    for (int j = 0; j < room.Width; j++)
                        map[room.Coordinates.Y + i, room.Coordinates.X + j] = room.Map[i, j];
                }
            }
            var finalizeMap = FinalizeMap(map);
            return new Map(finalizeMap, totalWidth, totalHeight, rooms.Length);
        }

        private Room[] GenerateFinalRooms(int maxRoomsCount)
        {
            Room[] rooms = GenerateRooms(maxRoomsCount);
            if (maxRoomsCount > 6 && rooms.Length < maxRoomsCount - 6) rooms = GenerateRooms(rooms, maxRoomsCount);
            bool hasShop = rooms.Any(room => room is ShopRoom);
            bool hasFinish = rooms.Any(room => room is FinishRoom);
            while (!hasShop || !hasFinish)
            {
                rooms = GenerateRooms(maxRoomsCount);
                if (maxRoomsCount > 6 && rooms.Length < maxRoomsCount - 6) rooms = GenerateRooms(rooms, maxRoomsCount);
                hasShop = rooms.Any(room => room is ShopRoom);
                hasFinish = rooms.Any(room => room is FinishRoom);
            }
            foreach (var room in rooms)
            {
                for (int y = 0; y < room.Height; y++)
                {
                    for (int x = 0; x < room.Width; x++)
                    {
                        if (room.Map[y, x] != '.') continue;
                        if (x == 0 || x == room.Height - 1 || y == 0 || y == room.Height - 1)
                            room.Map[y, x] = '#';
                    }
                }
            }
            return rooms;
        }

        private static char[,] FinalizeMap(char[,] map)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (x == 0 || y == 0 || y == map.GetLength(0) - 1 || x == map.GetLength(1) - 1)
                        map[y, x] = '#';
                    if (map[y, x] == '!' || map[y, x] == 'Q') map[y, x] = '#';
                }
            }
            return map;
        }
    }
}