using MaseGenerator.Rooms;

namespace MazeGenerator
{
    internal class Map
    {
        internal char[,] MapArray { get; private set; }
        internal bool Empty { get; private set; }
        internal int RoomsNumber { get; private set; }
        internal int Width { get; private set; }
        internal int Height { get; private set; }

        internal Map(char[,] map, int width, int height, int roomsNumber)
        {
            Empty = false;
            MapArray = map;
            Width = width;
            Height = height;
            RoomsNumber = roomsNumber;
        }
        internal Map(string map, int width, int height, int roomsNumber)
        {
            Empty = false;
            MapArray = new char[height, width];
            Width = width;
            Height = height;
            int index = 0;
            for(int y = 0;y< Height; y++)
            {
                for (int x = 0;x < Width; x++)
                {
                    MapArray[y, x] = map[index];
                    index++;
                }
            }
            RoomsNumber = roomsNumber;
        }
        internal Map() => Empty = true;

        internal Coordinates FindChar(char c)
        {
            if (Empty) throw new System.Exception("map is empty");
            for (int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height; y++)
                {
                    if (GetChar(x, y) == c) return new Coordinates(x,y);
                }
            }
            throw new System.Exception($"char {c} not found");
        }

        internal Coordinates FindCharAndReplace(char c, char c1)
        {
            if (Empty) throw new System.Exception("map is empty");
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (GetChar(x, y) == c)
                    {
                        ChangeChar(c1, x, y);
                        return new Coordinates(x, y);
                    }
                }
            }
            throw new System.Exception($"char {c} not found");
        }

        internal void FindAndReplace(char c, char c1)
        {
            if (Empty) throw new System.Exception("map is empty");
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (GetChar(x, y) == c) ChangeChar(c1, x, y);
                }
            }
        }

        internal bool OutOfBounds(int x, int y) => x < 0 || x >= Width || y < 0 || y >= Height;

        internal void Clear() => Empty = true;

        internal void ClearChar(int x, int y)
        {
            if (Empty) throw new System.Exception("map is empty");
            MapArray[y, x] = '.';
        }

        internal void ClearChar(double x, double y) => ClearChar((int)y, (int)x);

        internal void ChangeChar(char c, int x, int y)
        {
            if (Empty) throw new System.Exception("map is empty");
            MapArray[y, x] = c;
        }

        internal void ChangeChar(char c, double x, double y) => ChangeChar(c, (int)y, (int)x);

        internal char GetChar(int x, int y)
        {
            if (Empty) throw new System.Exception("map is empty");
            if (OutOfBounds(x, y)) throw new System.Exception($"out of bounds x: {x} y: {y} h: {Height} w: {Width}");
            return MapArray[y, x];
        }

        internal char GetChar(double x, double y) => GetChar((int)y, (int)x);
    }
}