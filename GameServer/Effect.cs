using System.Drawing;

namespace SLIL.Classes
{
    public class Effect
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int EffectTotalTime { get; set; }
        public int EffectTimeRemaining { get; set; }

        public Effect() { }

        public bool ReducingTimeRemaining()
        {
            EffectTimeRemaining--;
            if (EffectTimeRemaining < 0)
                return true;
            return false;
        }

        public void UpdateTimeRemaining() => EffectTimeRemaining = EffectTotalTime;

        public void SetTotalTime(int value) => EffectTotalTime = value;
    }

    public class Regeneration : Effect
    {
        public Regeneration() : base()
        {
            ID = 0;
            EffectTotalTime = 15;
            Name = "Regeneration";
            Description = "Gradually restores health";
            UpdateTimeRemaining();
        }
    }

    public class Adrenaline : Effect
    {
        public Adrenaline() : base()
        {
            ID = 1;
            EffectTotalTime = 20;
            Name = "Adrenaline";
            Description = "Increases movement speed";
            UpdateTimeRemaining();
        }
    }

    public class Protection : Effect
    {
        public Protection() : base()
        {
            ID = 2;
            EffectTotalTime = 60;
            Name = "Protection";
            Description = "Reduces damage taken";
            UpdateTimeRemaining();
        }
    }
}