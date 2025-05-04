using System.Linq;
using System.Collections.Generic;

namespace MaseGenerator.Rooms
{
    public enum DoorDirections { None, Up, Down, Left, Right }
    public enum RoomType { Start, Secret, Simple, Hallway, Shop, Finish };

    public class Doors
    {
        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }

        public Doors(string code)
        {
            Up = code.Contains('u');
            Down = code.Contains('d');
            Left = code.Contains('l');
            Right = code.Contains('r');
        }

        public Doors(char code)
        {
            Up = code == 'u';
            Down = code == 'd';
            Left = code == 'l';
            Right = code == 'r';
        }

        public Doors() => (Up, Down, Left, Right) = (true, true, true, true);
    }

    public abstract class Room
    {
        public RoomType Type;
        public RoomType[] PossibleConnections;
        public char[,] Map;
        public Doors RoomDoors;
        public int Height => Map.GetLength(0);
        public int Width => Map.GetLength(1);
        public int Top => Coordinates.Y;
        public int Bottom => Coordinates.Y + Height;
        public int Left => Coordinates.X;
        public int Right => Coordinates.X + Width;
        public Coordinates Coordinates;
        public double RoomGenerationChance;
        public double RoomVariationSpawnChance;
        public double ChanceOfContinuedGeneration;
        public int DoorsCount => Map.Cast<char>().Count(c => c == '!');
        public Coordinates[] DoorCoordinates => GetDoorCoordinates();
        public bool IsFinalRoom;

        public Room(int x, int y) => Coordinates = new Coordinates(x, y);

        public bool ThereIsMatchingDoor(DoorDirections doorDirections) =>
            doorDirections == DoorDirections.Up && RoomDoors.Down ||
            doorDirections == DoorDirections.Down && RoomDoors.Up ||
            doorDirections == DoorDirections.Left && RoomDoors.Right ||
            doorDirections == DoorDirections.Right && RoomDoors.Left;

        public Coordinates GetDoorCoordinates(DoorDirections doorDirections)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Map[y, x] == '!' && GetDoorDirection(new Coordinates(x, y)) == doorDirections)
                        return new Coordinates(x, y);
                }
            }
            return new Coordinates(-1, -1);
        }

        public Coordinates[] GetDoorCoordinates()
        {
            var result = new List<Coordinates>();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Map[y, x] == '!') result.Add(new Coordinates(x, y));
                }
            }
            return result.ToArray();
        }

        public DoorDirections GetDoorDirection(Coordinates doorCoordinates)
        {
            if (doorCoordinates.Y == 0) return DoorDirections.Up;
            if (doorCoordinates.Y == Height - 1) return DoorDirections.Down;
            if (doorCoordinates.X == 0) return DoorDirections.Left;
            return DoorDirections.Right;
        }

        public void ChangeMap(char c, Coordinates coordinates) { if (coordinates.ValidValue()) Map[coordinates.Y, coordinates.X] = c; }

        public void RemoveDoor(char c, Coordinates coordinates)
        {
            if (!coordinates.ValidValue()) return;
            ChangeMap(c, coordinates);
            var doorDirection = GetDoorDirection(coordinates);
            if (doorDirection == DoorDirections.Up) RoomDoors.Up = false;
            else if (doorDirection == DoorDirections.Down) RoomDoors.Down = false;
            else if (doorDirection == DoorDirections.Left) RoomDoors.Left = false;
            else RoomDoors.Right = false;
        }

        public void Restore()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Map[y, x] == 'Q')
                    {
                        ChangeMap('!', new Coordinates(x, y));
                        var doorDirection = GetDoorDirection(new Coordinates(x, y));
                        if (doorDirection == DoorDirections.Up) RoomDoors.Up = true;
                        else if (doorDirection == DoorDirections.Down) RoomDoors.Down = true;
                        else if (doorDirection == DoorDirections.Left) RoomDoors.Left = true;
                        else RoomDoors.Right = true;
                    }
                }
            }
        }

        public static Room GetRoomFromType(RoomType type)
        {
            switch (type)
            {
                case RoomType.Start: return new Start_0(0, 0);
                case RoomType.Hallway: return new Hallway_0(0, 0);
                case RoomType.Secret: return new Secret_0(0, 0);
                case RoomType.Simple: return new Simple_0(0, 0);
                case RoomType.Shop: return new Shop_0(0, 0);
                default: return new Finish_0(0, 0);
            }
        }
    }

    public abstract class StartRoom : Room
    {
        public StartRoom(int x, int y) : base(x, y)
        {
            Type = RoomType.Start;
            PossibleConnections = new RoomType[] { RoomType.Hallway, RoomType.Simple };
            ChanceOfContinuedGeneration = 1;
            RoomGenerationChance = 1;
        }
    }

    public abstract class HallwayRoom : Room
    {
        public HallwayRoom(int x, int y) : base(x, y)
        {
            Type = RoomType.Hallway;
            PossibleConnections = new RoomType[] { RoomType.Simple, RoomType.Shop, RoomType.Finish };
            ChanceOfContinuedGeneration = 0.9;
            RoomGenerationChance = 0.6;
        }
    }

    public abstract class SecretRoom : Room
    {
        public SecretRoom(int x, int y) : base(x, y)
        {
            Type = RoomType.Secret;
            PossibleConnections = new RoomType[] { RoomType.Simple, RoomType.Hallway };
            ChanceOfContinuedGeneration = 0.1;
            RoomGenerationChance = 0.01;
        }
    }

    public abstract class SimpleRoom : Room
    {
        public SimpleRoom(int x, int y) : base(x, y)
        {
            Type = RoomType.Simple;
            PossibleConnections = new RoomType[] { RoomType.Hallway, RoomType.Shop, RoomType.Finish, RoomType.Secret };
            ChanceOfContinuedGeneration = 0.6;
            RoomGenerationChance = 0.7;
        }
    }

    public abstract class ShopRoom : Room
    {
        public ShopRoom(int x, int y) : base(x, y)
        {
            Type = RoomType.Shop;
            PossibleConnections = new RoomType[] { RoomType.Hallway, RoomType.Simple, RoomType.Finish };
            ChanceOfContinuedGeneration = 0.3;
            RoomGenerationChance = 0.4;
        }
    }

    public abstract class FinishRoom : Room
    {
        public FinishRoom(int x, int y) : base(x, y)
        {
            Type = RoomType.Finish;
            PossibleConnections = new RoomType[] { };
            ChanceOfContinuedGeneration = 0;
            RoomGenerationChance = 0.5;
        }
    }

    public class Start_0 : StartRoom
    {
        public Start_0(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors('u'); 
            RoomVariationSpawnChance = 0.6;
            Map = new char[,]
            {
                { '#', '#', '#', '#', '#', '#', '#', '!', '#', '#' },
                { '#', 'P', '.', 'b', '#', 'l', '.', '.', 'l', '#' },
                { '#', '.', 'L', 'l', '#', 'L', '.', '.', '.', '#' },
                { '#', 'd', '#', '#', '#', 'E', '.', '.', 'E', '#' },
                { '#', '.', '.', 'L', '#', '.', '.', '.', '.', '#' },
                { '#', '.', '.', '.', '=', 'L', '.', '.', 'b', '#' },
                { '#', '.', 'E', '.', '=', '.', '.', '.', 'L', '#' },
                { '#', '.', '.', 'l', '#', '.', '.', '.', '.', '#' },
                { '#', 'L', '.', '.', 'd', '.', '.', '.', 'b', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
            };
        }
    }

    public class Start_1 : StartRoom
    {
        public Start_1(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors('r');
            RoomVariationSpawnChance = 0.5;
            Map = new char[,]
            {
                { '#', '#', '#', '#', '.', '.', '.', '.', '.', '.' },
                { '#', 'P', 'L', '#', '.', '.', '.', '.', '.', '.' },
                { '#', '.', '.', '#', '#', '#', '.', '.', '.', '.' },
                { '#', '.', '.', '.', 'b', '#', '.', '.', '.', '.' },
                { '#', 'L', '.', '.', 'B', '#', '#', '#', '#', '#' },
                { '#', '.', '.', '.', 'L', '=', '.', 'L', '.', '#' },
                { '#', '.', 'E', '.', '.', '#', '.', '.', 'E', '#' },
                { '#', '.', '.', '.', '.', '#', '.', '.', '.', '!' },
                { '#', 'l', '.', 'L', '.', 'd', '.', 'X', '.', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
            };
        }
    }

    public class Hallway_0 : HallwayRoom
    {
        public Hallway_0(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors("ulr");
            RoomVariationSpawnChance = 0.6;
            Map = new char[,]
            {
                { '.', '.', '.', '#', '#', '!', '#', '#', '.', '.' },
                { '.', '.', '.', '#', '.', '.', 'L', '#', '.', '.' },
                { '.', '.', '.', '#', 'L', '.', '.', '#', '.', '.' },
                { '.', '.', '.', '#', '.', '.', '.', '#', '.', '.' },
                { '.', '.', '.', '#', '.', '.', 'l', '#', '.', '.' },
                { '#', '#', '#', '#', '#', '=', '#', '#', '#', '#' },
                { '#', 'B', '.', '.', '.', '.', '.', '.', 'b', '#' },
                { '!', '.', '.', '.', 'E', '.', '.', '.', '.', '!' },
                { '#', 'l', 'L', '.', '.', '.', '.', 'L', 'l', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
            };
        }
    }

    public class Hallway_1 : HallwayRoom
    {
        public Hallway_1(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors("ul");
            RoomVariationSpawnChance = 0.5;
            Map = new char[,]
            {
                { '.', '.', '.', '.', '.', '#', '#', '!', '#', '#' },
                { '.', '.', '.', '.', '.', '#', 'b', '.', 'l', '#' },
                { '.', '.', '.', '.', '.', '#', '.', '.', '.', '#' },
                { '.', '.', '.', '.', '.', '#', '.', '.', 'L', '#' },
                { '.', '.', '.', '.', '.', '#', '.', 'E', '.', '#' },
                { '#', '#', '#', '#', '#', '#', 'L', '.', '.', '#' },
                { '#', 'l', '.', '.', 'L', '.', '.', '.', '.', '#' },
                { '!', '.', '.', '.', 'E', '.', '.', '.', '.', '#' },
                { '#', 'b', 'L', '.', '.', '.', 'B', '.', 'l', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
            };
        }
    }

    public class Hallway_2 : HallwayRoom
    {
        public Hallway_2(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors("ur");
            RoomVariationSpawnChance = 0.5;
            Map = new char[,]
            {
                { '#', '#', '!', '#', '#', '.', '.', '.', '.', '.' },
                { '#', '.', '.', 'L', '#', '.', '.', '.', '.', '.' },
                { '#', 'l', '.', '.', '#', '.', '.', '.', '.', '.' },
                { '#', '.', '.', '.', '#', '.', '.', '.', '.', '.' },
                { '#', 'L', '.', '.', '#', '#', '.', '.', '.', '.' },
                { '#', '.', '.', '.', 'X', '#', '#', '#', '#', '#' },
                { '#', '.', '.', '.', '.', '.', '.', '.', 'L', '#' },
                { '#', 'B', '.', '.', '.', '.', '.', '.', '.', '!' },
                { '#', 'b', 'B', '.', 'L', 'E', '.', '.', 'l', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
            };
        }
    }

    public class Hallway_3 : HallwayRoom
    {
        public Hallway_3(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors("dr");
            RoomVariationSpawnChance = 0.5;
            Map = new char[,]
            {
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', 'L', 'X', '.', '.', '.', '.', '.', 'L', '#' },
                { '#', '.', 'E', '.', '.', '.', '.', '.', '.', '!' },
                { '#', '.', '.', '.', '.', 'L', '.', '.', 'l', '#' },
                { '#', '.', '.', '.', 'b', '#', '#', '#', '#', '#' },
                { '#', 'B', '.', '.', '#', '#', '.', '.', '.', '.' },
                { '#', '.', '.', '.', '#', '.', '.', '.', '.', '.' },
                { '#', '.', '.', '.', '#', '.', '.', '.', '.', '.' },
                { '#', 'L', '.', 'B', '#', '.', '.', '.', '.', '.' },
                { '#', '#', '!', '#', '#', '.', '.', '.', '.', '.' },
            };
        }
    }

    public class Hallway_4 : HallwayRoom
    {
        public Hallway_4(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors("ld");
            RoomVariationSpawnChance = 0.6;
            Map = new char[,]
            {
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', 'L', '.', '.', '.', '.', '.', 'B', 'L', '#' },
                { '!', '.', '.', '.', '.', 'E', '.', '.', '.', '#' },
                { '#', 'L', '.', 'b', '.', '.', '.', '.', '.', '#' },
                { '#', '#', '#', '#', '#', 'l', '.', '.', 'E', '#' },
                { '.', '.', '.', '.', '#', '#', '.', '.', '.', '#' },
                { '.', '.', '.', '.', '.', '#', '.', '.', '.', '#' },
                { '.', '.', '.', '.', '.', '#', '.', '.', '.', '#' },
                { '.', '.', '.', '.', '.', '#', 'L', '.', 'b', '#' },
                { '.', '.', '.', '.', '.', '#', '#', '!', '#', '#' },
            };
        }
    }

    public class Hallway_5 : HallwayRoom
    {
        public Hallway_5(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors("lur");
            RoomVariationSpawnChance = 0.6;
            Map = new char[,]
            {
                { '.', '.', '.', '.', '.', '#', '#', '!', '#', '#' },
                { '.', '.', '.', '.', '.', '#', 'b', '.', 'l', '#' },
                { '.', '.', '.', '.', '.', '#', '.', '.', '.', '#' },
                { '.', '.', '.', '.', '.', '#', '.', '.', 'L', '#' },
                { '.', '.', '.', '.', '.', '#', '.', 'E', '.', '#' },
                { '#', '#', '#', '#', '#', '#', 'L', '.', '.', '#' },
                { '#', 'l', '.', '.', 'L', '.', '.', '.', '.', '#' },
                { '!', '.', '.', '.', 'E', '.', '.', '.', '.', '!' },
                { '#', 'b', 'L', '.', '.', '.', 'B', '.', 'l', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
            };
        }
    }

    public class Secret_0 : SecretRoom
    {
        public Secret_0(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors();
            RoomVariationSpawnChance = 1;
            Map = new char[,]
            {
                { '#', '#', '!', '#', '#' },
                { '#', '.', '.', '.', '#' },
                { '!', '.', 'G', '.', '!' },
                { '#', '.', '.', '.', '#' },
                { '#', '#', '!', '#', '#' }
            };
        }
    }

    public class Simple_0 : SimpleRoom
    {
        public Simple_0(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors("dl");
            RoomVariationSpawnChance = 0.7;
            Map = new char[,]
            {
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', 'b', 'B', '#', 'E', '.', '.', 'E', 'L', '#' },
                { '#', 'L', '.', '#', 'L', '.', '.', '.', 'l', '#' },
                { '#', 'l', '.', '#', '.', '.', '.', '.', 'L', '#' },
                { '!', '.', '.', 'd', '.', '.', '.', '.', '.', '#' },
                { '#', '=', '=', '#', '#', '#', '.', 'E', '.', '#' },
                { '#', '.', '.', '.', 'L', '#', '.', '.', '.', '#' },
                { '#', '.', 'E', '.', '.', '#', '.', '.', '.', '#' },
                { '#', 'L', '.', 'E', '.', 'd', '.', 'L', 'l', '#' },
                { '#', '#', '!', '#', '#', '#', '#', '#', '#', '#' },
            };
        }
    }

    public class Simple_1 : SimpleRoom
    {
        public Simple_1(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors("ul");
            RoomVariationSpawnChance = 0.5;
            Map = new char[,]
            {
                { '#', '!', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', '.', '.', '.', '.', '.', 'L', '#', '#', '#' },
                { '#', 'L', '.', '.', 'E', '.', '.', 'l', '#', '#' },
                { '#', '.', '.', '.', '.', '.', '.', '.', 'L', '#' },
                { '#', 'B', '.', 'E', '.', 'E', '.', '.', '.', '#' },
                { '#', 'b', 'B', 'L', '.', '.', '.', '.', '.', '#' },
                { '#', '#', '#', '#', '=', '#', '#', '.', '.', '#' },
                { '#', '.', '.', '.', '.', '.', '.', '.', '.', '#' },
                { '!', '.', 'L', '.', '.', '.', '.', 'L', 'l', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
            };
        }
    }

    public class Simple_2 : SimpleRoom
    {
        public Simple_2(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors("udl");
            RoomVariationSpawnChance = 0.5;
            Map = new char[,]
            {
                { '.', '.', '.', '.', '.', '.', '#', '#', '!', '#' },
                { '#', '#', '#', '#', '#', '#', '#', 'l', '.', '#' },
                { '#', 'b', '.', '.', 'L', 'l', '#', 'L', '.', '#' },
                { '#', 'L', '.', '.', '.', '.', '#', '.', '.', '#' },
                { '!', '.', '.', 'E', '.', '.', 'd', '.', '.', '#' },
                { '#', '.', '.', '.', '.', '.', '#', '.', 'E', '#' },
                { '#', 'b', 'L', '.', 'L', 'l', '#', '.', 'L', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '.', '.', '#' },
                { '.', '.', '.', '.', '.', '.', '#', 'B', '.', '#' },
                { '.', '.', '.', '.', '.', '.', '#', '#', '!', '#' },
            };
        }
    }

    public class Simple_3 : SimpleRoom
    {
        public Simple_3(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors();
            RoomVariationSpawnChance = 0.5;
            Map = new char[,]
            {
                { '.', '.', '.', '#', '#', '!', '#', '#', '.', '.' },
                { '.', '.', '.', '#', 'b', '.', 'l', '#', '.', '.' },
                { '#', '#', '#', '#', 'L', '.', '.', '#', '#', '#' },
                { '#', 'l', '.', 'E', '.', '.', '.', 'L', 'l', '#' },
                { '!', '.', '.', '.', '.', '.', '.', '.', '.', '!' },
                { '#', 'L', '.', '.', '.', '.', '.', 'b', 'L', '#' },
                { '#', '#', '=', '#', '.', '.', 'E', '#', '#', '#' },
                { '#', 'X', '.', '#', '.', '.', '.', '#', '.', '.' },
                { '#', 'b', 'B', '#', 'l', '.', 'L', '#', '.', '.' },
                { '#', '#', '#', '#', '#', '!', '#', '#', '.', '.' },
            };
        }
    }

    public class Simple_4 : SimpleRoom
    {
        public Simple_4(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors();
            RoomVariationSpawnChance = 0.6;
            Map = new char[,]
            {
                { '#', '#', '#', '!', '#', '#', '#', '#', '#', '#' },
                { '#', 'L', 'b', '.', 'l', '.', '.', 'L', 'b', '#' },
                { '!', 'l', '.', '.', '.', '.', '.', 'L', '.', '#' },
                { '#', '.', '.', '.', '.', '.', '.', '.', '.', '#' },
                { '#', 'L', '.', 'E', '.', '.', '.', '.', 'L', '#' },
                { '#', '.', '.', '.', '.', '.', '.', 'E', '.', '#' },
                { '#', 'l', '.', '.', 'b', '.', '.', '.', 'l', '#' },
                { '#', 'b', 'E', '.', '.', '.', '.', '.', 'L', '#' },
                { '#', 'L', '.', 'L', '.', '.', '.', 'l', 'b', '!' },
                { '#', '#', '#', '#', '#', '#', '!', '#', '#', '#' },
            };
        }
    }

    public class Shop_0 : ShopRoom
    {
        public Shop_0(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors("udr");
            RoomVariationSpawnChance = 0.7;
            Map = new char[,]
            {
                { '#', '!', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', '.', 'L', '#', 'b', 'B', 'b', '#', '$', '#' },
                { '#', '.', '.', '#', 'l', 'L', 'X', '#', 'D', '#' },
                { '#', '.', '.', 'd', '.', '.', '.', '.', '.', '#' },
                { '#', '.', '.', '#', '.', '.', '.', '.', '.', '#' },
                { '#', '.', 'L', '#', 'b', '.', '.', '.', '.', '!' },
                { '#', '.', '.', '#', '#', '#', '#', '#', '#', '#' },
                { '#', 'L', '.', '.', '.', '.', 'E', '.', '.', '#' },
                { '#', 'l', '.', 'E', '.', '.', 'L', '.', 'l', '#' },
                { '#', '#', '#', '#', '#', '!', '#', '#', '#', '#' },
            };
        }
    }

    public class Shop_1 : ShopRoom
    {
        public Shop_1(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors("dl");
            RoomVariationSpawnChance = 0.6;
            Map = new char[,]
            {
                { '#', '#', '#', '#', '#', '#', '#', '#', '.', '.' },
                { '!', 'L', '.', 'L', '.', 'D', '$', '#', '.', '.' },
                { '#', '.', '.', '.', '.', '#', '#', '#', '.', '.' },
                { '#', 'b', 'l', '.', '.', '#', '.', '.', '.', '.' },
                { '#', '#', '#', '#', 'd', '#', '.', '.', '.', '.' },
                { '#', 'B', 'X', 'l', '.', '#', '.', '.', '.', '.' },
                { '#', 'E', '.', '.', '.', '#', '.', '.', '.', '.' },
                { '#', '.', 'L', '.', '.', '#', '.', '.', '.', '.' },
                { '#', 'b', '.', '.', 'L', '#', '.', '.', '.', '.' },
                { '#', '#', '#', '#', '!', '#', '.', '.', '.', '.' },
            };
        }
    }

    public class Shop_2 : ShopRoom
    {
        public Shop_2(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors("lr");
            RoomVariationSpawnChance = 0.5;
            Map = new char[,]
            {
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', 'b', 'L', '.', '.', 'l', '.', 'b', 'L', '#' },
                { '!', '.', '.', '.', '.', '.', '.', '.', '.', '!' },
                { '#', 'L', 'l', '.', '.', '.', 'b', '.', 'L', '#' },
                { '#', '#', '#', '#', 'D', '#', '#', '#', '#', '#' },
                { '.', '.', '.', '#', '$', '#', '.', '.', '.', '.' },
                { '.', '.', '.', '#', '#', '#', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.', '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.', '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.', '.', '.', '.', '.', '.' },
            };
        }
    }

    public class Finish_0 : FinishRoom
    {
        public Finish_0(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors('r');
            RoomVariationSpawnChance = 0.6;
            Map = new char[,]
            {
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', 'L', 'E', 'l', '.', 'E', '.', '.', 'F', '#' },
                { '#', '.', '.', '.', '.', '.', '.', '.', 'b', '#' },
                { '#', '.', '.', 'L', '.', 'E', '.', '.', 'l', '#' },
                { '#', 'L', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', '.', '#', 'l', '.', '.', '.', '.', 'B', '#' },
                { '#', '.', '#', 'b', '.', 'E', '.', '.', 'L', '#' },
                { '#', '.', '#', '#', '#', '#', '#', 'd', '#', '#' },
                { '#', '.', '.', '.', '.', 'L', '.', '.', '.', '!' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
            };
        }
    }

    public class Finish_1 : FinishRoom
    {
        public Finish_1(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors('d');
            RoomVariationSpawnChance = 0.5;
            Map = new char[,]
            {
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', 'b', '.', 'L', '.', '.', 'E', '.', 'l', '#' },
                { '#', '.', '.', '.', '.', '.', '.', '.', 'E', '#' },
                { '#', 'L', '.', '#', '#', 'd', '#', '#', '#', '#' },
                { '#', '.', '.', '#', 'b', '.', '.', '.', 'L', '#' },
                { '#', '.', 'B', '#', '.', 'E', '.', '.', '.', '#' },
                { '#', '.', '.', '#', '.', '.', '.', '.', '.', '#' },
                { '#', '.', '.', '#', 'L', 'l', 'L', '.', 'F', '#' },
                { '#', '.', 'L', '#', '#', '#', '#', '#', '#', '#' },
                { '#', '!', '#', '#', '.', '.', '.', '.', '.', '.' },
            };
        }
    }

    public class Finish_2 : FinishRoom
    {
        public Finish_2(int x, int y) : base(x, y)
        {
            IsFinalRoom = true;
            RoomDoors = new Doors("ud");
            RoomVariationSpawnChance = 0.5;
            Map = new char[,]
            {
                { '#', '#', '#', '#', '#', '#', '#', '#', '!', '#' },
                { '#', 'L', '.', '.', '#', 'l', '.', 'L', '.', '#' },
                { '#', '.', '.', '.', '=', '.', '.', '.', '.', '#' },
                { '#', 'E', '.', '.', '=', '.', '.', '.', 'L', '#' },
                { '#', '.', '.', 'l', '#', 'E', '.', '.', '.', '#' },
                { '#', '#', '#', 'd', '#', '.', '.', '.', 'E', '#' },
                { '#', 'L', 'B', '.', '#', 'L', '.', '.', 'L', '#' },
                { '#', '.', '.', '.', '#', '.', '.', '.', '.', '#' },
                { '#', 'F', 'L', 'b', '#', 'l', '.', '.', '.', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '!', '#' },
            };
        }
    }
}