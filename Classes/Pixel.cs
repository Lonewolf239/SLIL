namespace SLIL.Classes
{
    internal class Pixel
    {
        private int _Blackout;
        internal int X { get; set; }
        internal int Y { get; set; }
        internal int Blackout
        {
            get
            {
                if (_Blackout > 100) return 100;
                if (_Blackout < 0) return 0;
                return _Blackout;
            }
            set => _Blackout = value;
        }
        internal double Distance { get; set; }
        internal double WallHeight { get; set; }
        internal int TextureId { get; set; }
        internal double TextureX { get; set; }
        internal double TextureY { get; set; }
        internal int Side { get; set; }
        internal SpriteStates SpriteState { get; set; }

        internal Pixel(int x, int y, int blackout, double distance, double wallHeight, int textureId, SpriteStates spriteState)
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