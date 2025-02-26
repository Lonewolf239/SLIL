namespace SLIL.Classes
{
    public class Pixel
    {
        private int _Blackout;
        public int X { get; set; }
        public int Y { get; set; }
        public int Blackout
        {
            get
            {
                if (_Blackout > 100) return 100;
                return _Blackout;
            }
            set => _Blackout = value;
        }
        public double Distance { get; set; }
        public double WallHeight { get; set; }
        public int TextureId { get; set; }
        public double TextureX { get; set; }
        public double TextureY { get; set; }
        public int Side { get; set; }
        public SpriteStates SpriteState { get; set; }

        public Pixel(int x, int y, int blackout, double distance, double wallHeight, int textureId, SpriteStates spriteState)
        {
            X = x;
            Y = y;
            Blackout = blackout;
            Distance = distance;
            WallHeight = wallHeight;
            TextureId = textureId;
            SpriteState = spriteState;
        }
    }
}