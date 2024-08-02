namespace SLIL.Classes
{
    public class Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Blackout { get; set; }
        public double Distance { get; set; }
        public double WallHeight { get; set; }
        public int TextureId { get; set; }
        public double TextureX { get; set; }
        public double TextureY { get; set; }
        public int Side { get; set; }

        public Pixel(int x, int y, int blackout, double distance, double wallHeight, int textureId)
        {
            X = x;
            Y = y;
            Blackout = blackout;
            Distance = distance;
            WallHeight = wallHeight;
            TextureId = textureId;
        }
    }
}