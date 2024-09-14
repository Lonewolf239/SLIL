namespace SLIL.Classes
{
    public class Effect
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int EffectTotalTime { get; set; }
        public int EffectTimeRemaining { get; set; }
        public bool Debaf { get; set; }
        public bool Infinity { get; set; }

        public Effect()
        {
            Debaf = false;
            Infinity = false;
        }

        public bool ReducingTimeRemaining()
        {
            if (Infinity) return false;
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
            EffectTotalTime = 120;
            Name = "Protection";
            Description = "Reduces damage taken";
            UpdateTimeRemaining();
        }
    }

    public class Fatigue : Effect
    {
        public Fatigue() : base()
        {
            ID = 3;
            Debaf = true;
            EffectTotalTime = 15;
            Name = "Fatigue";
            Description = "Prevents window entry";
            UpdateTimeRemaining();
        }
    }

    public class Biker : Effect
    {
        public Biker() : base()
        {
            ID = 4;
            Infinity = true;
            EffectTotalTime = 1;
            Name = "Biker";
            Description = "I'm a Biker bitch!";
            UpdateTimeRemaining();
        }
    }
}