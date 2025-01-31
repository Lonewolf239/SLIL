using LiteNetLib.Utils;

namespace SLIL.Classes
{
    public class Effect
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int EffectTotalTime { get; set; }
        public int EffectTimeRemaining { get; set; }
        public bool Infinity { get; set; }

        public Effect() => Infinity = false;

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

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(EffectTimeRemaining);
        }

        public void Deserialize(NetDataReader reader)
        {
            this.EffectTimeRemaining = reader.GetInt();
        }
    }

    public class Debaf : Effect
    {
        public Debaf() : base() { }
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

    public class Rider : Effect
    {
        public Rider() : base()
        {
            ID = 4;
            Infinity = true;
            EffectTotalTime = 1;
            Name = "Biker";
            Description = "I'm a Biker bitch!";
            UpdateTimeRemaining();
        }
    }

    public class Fatigue : Debaf
    {
        public Fatigue() : base()
        {
            ID = 3;
            EffectTotalTime = 15;
            Name = "Fatigue";
            Description = "Prevents window entry";
            UpdateTimeRemaining();
        }
    }

    public class Bleeding : Debaf
    {
        public Bleeding() : base()
        {
            ID = 5;
            EffectTotalTime = 15;
            Name = "Bleeding";
            Description = "Gradually reduces health";
            UpdateTimeRemaining();
        }
    }

    public class Blindness : Debaf
    {
        public Blindness() : base()
        {
            ID = 6;
            EffectTotalTime = 10;
            Name = "Blindness";
            Description = "Reduces visibility range";
            UpdateTimeRemaining();
        }
    }

    public class Stunned : Debaf
    {
        public Stunned() : base()
        {
            ID = 7;
            EffectTotalTime = 7;
            Name = "Stunned";
            Description = "Reduces movement speed and invulnerability duration";
            UpdateTimeRemaining();
        }
    }

    public class Void : Debaf
    {
        public Void() : base()
        {
            ID = 8;
            EffectTotalTime = 1;
            Infinity = true;
            Name = "Void";
            Description = "Void";
            UpdateTimeRemaining();
        }
    }
}