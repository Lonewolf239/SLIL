namespace MaseGenerator.Rooms
{
    public class Coordinates
    {
        public int X;
        public int Y;

        public Coordinates(Coordinates coordinates) { X = coordinates.X; Y = coordinates.Y; }
        public Coordinates(int x, int y) { X = x; Y = y; }
        public Coordinates() { X = 0; Y = 0; }

        public static Coordinates operator +(Coordinates c1, Coordinates c2) => new Coordinates(c1.X + c2.X, c1.Y + c2.Y);

        public override string ToString() => $"X: {X} Y: {Y}";

        public bool ValidValue() => X >= 0 && Y >= 0;
    }

    public class Rectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle(Coordinates topLeft, int width, int height)
        {
            X = topLeft.X;
            Y = topLeft.Y;
            Width = width;
            Height = height;
        }

        public bool Overlaps(Rectangle other)=>
            X < other.X + other.Width - 1 &&
            X + Width - 1 > other.X &&
            Y < other.Y + other.Height - 1 &&
            Y + Height - 1 > other.Y;

        public override string ToString() => $"X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
    }
}