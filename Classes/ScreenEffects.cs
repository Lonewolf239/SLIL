namespace SLIL.Classes
{
    internal class ScreenEffects
    {
        internal int TotalTime { get; set; }
        internal int TimeRemaining { get; set; }

        internal void ReducingTimeRemaining() => TimeRemaining--;

        internal void UpdateTimeRemaining() => TimeRemaining = TotalTime;

        internal void SetTotalTime(int value) => TotalTime = value;
    }

    internal class BloodEffects : ScreenEffects
    {
        internal BloodEffects() : base()
        {
            TotalTime = 15; // 1.5 sec
            UpdateTimeRemaining();
        }
    }

    internal class BloodEffect1 : BloodEffects { }
    internal class BloodEffect2 : BloodEffects { }
    internal class BloodEffect3 : BloodEffects { }
    internal class BloodEffect4 : BloodEffects { }
}