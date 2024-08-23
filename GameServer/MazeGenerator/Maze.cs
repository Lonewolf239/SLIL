using MazeGenerator.Enum;

namespace MazeGenerator
{
    public class Maze
    {
        private static readonly Random rand = new();
        Room finishRoom = null;

        private List<Room> Generate(int width, int height)
        {
            List<Room> rooms = [];
            Stack<Room> roomStack = new();
            var initialRoom = new Room { X = 0, Y = 0 };
            roomStack.Push(initialRoom);
            rooms.Add(initialRoom);
            int finishIndex = rand.Next(width * height / 2, width * height);
            while (rooms.Count < height * width)
            {
                Room currentRoom = roomStack.Peek();
                List<Direction> availableDirections = [];
                if (currentRoom.X != 0 && !rooms.Any(r => r.X == currentRoom.X - 1 && r.Y == currentRoom.Y))
                    availableDirections.Add(Direction.Left);
                if (currentRoom.X != width - 1 && !rooms.Any(r => r.X == currentRoom.X + 1 && r.Y == currentRoom.Y))
                    availableDirections.Add(Direction.Right);
                if (currentRoom.Y != 0 && !rooms.Any(r => r.X == currentRoom.X && r.Y == currentRoom.Y - 1))
                    availableDirections.Add(Direction.Up);
                if (currentRoom.Y != height - 1 && !rooms.Any(r => r.X == currentRoom.X && r.Y == currentRoom.Y + 1))
                    availableDirections.Add(Direction.Down);
                if (availableDirections.Count == 0)
                {
                    roomStack.Pop();
                    continue;
                }
                Direction newDirection = availableDirections[rand.Next(0, availableDirections.Count)];
                if (availableDirections.Contains(Direction.Right))
                {
                    if (rand.Next(0, 11) < 3)
                        newDirection = Direction.Right;
                }
                var newRoom = new Room();
                switch (newDirection)
                {
                    case Direction.Right:
                        newRoom.X = currentRoom.X + 1;
                        newRoom.Y = currentRoom.Y;
                        newRoom.Links.Add(Direction.Left);
                        currentRoom.Links.Add(Direction.Right);
                        break;
                    case Direction.Left:
                        newRoom.X = currentRoom.X - 1;
                        newRoom.Y = currentRoom.Y;
                        newRoom.Links.Add(Direction.Right);
                        currentRoom.Links.Add(Direction.Left);
                        break;
                    case Direction.Up:
                        newRoom.X = currentRoom.X;
                        newRoom.Y = currentRoom.Y - 1;
                        newRoom.Links.Add(Direction.Down);
                        currentRoom.Links.Add(Direction.Up);
                        break;
                    case Direction.Down:
                        newRoom.X = currentRoom.X;
                        newRoom.Y = currentRoom.Y + 1;
                        newRoom.Links.Add(Direction.Up);
                        currentRoom.Links.Add(Direction.Down);
                        break;
                }
                if (rooms.Count == finishIndex)
                    finishRoom = newRoom;
                roomStack.Push(newRoom);
                rooms.Add(newRoom);
            }
            return rooms;
        }

        public char[,] GenerateCharMap(int width, int height, char wallChar, char windowChar, char doorChar, char emptyChar, char finishChar, int MAX_SHOP_COUNT = 1)
        {
            int shop_count = 0;
            char[,] result = new char[width * 3 + 1, height * 3 + 1];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = emptyChar;
            }
            for (int i = 0; i < result.GetLength(1); i++)
                result[0, i] = wallChar;
            for (int i = 0; i < result.GetLength(0); i++)
                result[i, 0] = wallChar;
            List<Room> roomMap = Generate(width, height);
            foreach (Room room in roomMap)
            {
                if (!room.Links.Contains(Direction.Right))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        result[room.X * 3 + 3, room.Y * 3 + 1 + i] = wallChar;
                        if (rand.NextDouble() <= 0.06)
                            result[room.X * 3 + 3, room.Y * 3 + 1 + i] = windowChar;
                        if (rand.NextDouble() <= 0.01)
                            result[room.X * 3 + 3, room.Y * 3 + 1 + i] = doorChar;
                        if (rand.NextDouble() <= 0.015 && shop_count < MAX_SHOP_COUNT)
                        {
                            result[room.X * 3 + 3, room.Y * 3 + 1 + i] = '$';
                            shop_count++;
                        }
                    }
                }
                if (!room.Links.Contains(Direction.Down))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        result[room.X * 3 + 1 + i, room.Y * 3 + 3] = wallChar;
                        if (rand.NextDouble() <= 0.06)
                            result[room.X * 3 + 1 + i, room.Y * 3 + 3] = windowChar;
                        if (rand.NextDouble() <= 0.01)
                            result[room.X * 3 + 1 + i, room.Y * 3 + 3] = doorChar;
                        if (rand.NextDouble() <= 0.015 && shop_count < MAX_SHOP_COUNT)
                        {
                            result[room.X * 3 + 1 + i, room.Y * 3 + 3] = '$';
                            shop_count++;
                        }
                    }
                }
                if (room.X == finishRoom.X && room.Y == finishRoom.Y)
                    result[room.X * 3 + 2, room.Y * 3 + 2] = finishChar;
            }
            for (int i = 0; i < result.GetLength(1); i++)
                result[result.GetLength(0) - 1, i] = wallChar;
            for (int i = 0; i < result.GetLength(0); i++)
                result[i, result.GetLength(1) - 1] = wallChar;
            return result;
        }
    }
}