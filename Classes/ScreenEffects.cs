namespace SLIL.Classes
{
    internal class ScreenEffects
    {
        internal int ID { get; set; }
        internal int TotalTime { get; set; }
        internal int TimeRemaining { get; set; }

        internal bool ReducingTimeRemaining()
        {
            TimeRemaining--;
            if (TimeRemaining < 0) return true;
            return false;
        }

        internal void UpdateTimeRemaining() => TimeRemaining = TotalTime;

        internal void SetTotalTime(int value) => TotalTime = value;
    }

    internal class BloodEffect : ScreenEffects
    {
        internal BloodEffect() : base()
        {
            ID = 0;
            TotalTime = 20;
            UpdateTimeRemaining();
        }
    }
}