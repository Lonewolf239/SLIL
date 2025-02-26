using System.Drawing;

namespace SLIL.Classes
{
    internal class ScreenEffects
    {
        internal int TotalTime { get; set; }
        internal int TimeRemaining { get; set; }

        internal void ReducingTimeRemaining() => TimeRemaining--;
    }

    internal class BloodEffects : ScreenEffects
    {
        internal BloodEffects() : base()
        {
            TotalTime = 35; // 3.5 sec
            TimeRemaining = TotalTime;
        }
    }

    internal class BloodEffect1 : BloodEffects { }
    internal class BloodEffect2 : BloodEffects { }
    internal class BloodEffect3 : BloodEffects { }
    internal class BloodEffect4 : BloodEffects { }

    internal class ScreenShot : ScreenEffects
    {
        internal Image SSImage { get; }

        internal ScreenShot(Image ssImage) : base()
        {
            TotalTime = 7; // 0.7 sec
            TimeRemaining = TotalTime;
            SSImage = ssImage;
        }
    }
}