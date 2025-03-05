using LiteNetLib.Utils;

namespace SLIL.Classes
{
    internal class Effect
    {
        internal int ID { get; set; }
        internal string Name { get; set; }
        internal string Description { get; set; }
        internal int TotalTime { get; set; }
        internal int TimeRemaining { get; set; }
        internal bool Infinity { get; set; }
        internal bool CanIssuedByConsole { get; set; }

        internal Effect()
        {
            Infinity = false;
            CanIssuedByConsole = true;
        }

        internal bool ReducingTimeRemaining()
        {
            if (Infinity) return false;
            TimeRemaining--;
            if (TimeRemaining < 0)
                return true;
            return false;
        }

        internal void UpdateTimeRemaining() => TimeRemaining = TotalTime;

        internal void SetTotalTime(int value) => TotalTime = value;

        internal void Serialize(NetDataWriter writer)
        {
            writer.Put(TimeRemaining);
        }

        internal void Deserialize(NetDataReader reader)
        {
            this.TimeRemaining = reader.GetInt();
        }
    }

    internal class Debaf : Effect
    {
        internal Debaf() : base() { }
    }

    internal class Regeneration : Effect
    {
        internal Regeneration() : base()
        {
            ID = 0;
            TotalTime = 15;
            Name = "Regeneration";
            Description = "Gradually restores health";
            UpdateTimeRemaining();
        }
    }

    internal class Adrenaline : Effect
    {
        internal Adrenaline() : base()
        {
            ID = 1;
            TotalTime = 20;
            Name = "Adrenaline";
            Description = "Increases movement speed";
            UpdateTimeRemaining();
        }
    }

    internal class Protection : Effect
    {
        internal Protection() : base()
        {
            ID = 2;
            TotalTime = 120;
            Name = "Protection";
            Description = "Reduces damage taken";
            UpdateTimeRemaining();
        }
    }

    internal class Rider : Effect
    {
        internal Rider() : base()
        {
            ID = 4;
            Infinity = true;
            TotalTime = 1;
            Name = "Biker";
            Description = "I'm a Biker bitch!";
            UpdateTimeRemaining();
        }
    }

    internal class Fatigue : Debaf
    {
        internal Fatigue() : base()
        {
            ID = 3;
            TotalTime = 15;
            Name = "Fatigue";
            Description = "Prevents window entry";
            UpdateTimeRemaining();
        }
    }

    internal class Bleeding : Debaf
    {
        internal Bleeding() : base()
        {
            ID = 5;
            TotalTime = 15;
            Name = "Bleeding";
            Description = "Gradually reduces health";
            UpdateTimeRemaining();
        }
    }

    internal class Blindness : Debaf
    {
        internal Blindness() : base()
        {
            ID = 6;
            TotalTime = 10;
            Name = "Blindness";
            Description = "Reduces visibility range";
            UpdateTimeRemaining();
        }
    }

    internal class Stunned : Debaf
    {
        internal Stunned() : base()
        {
            ID = 7;
            TotalTime = 7;
            Name = "Stunned";
            Description = "Reduces speed and invulnerability";
            UpdateTimeRemaining();
        }
    }

    internal class VoidE : Debaf
    {
        internal VoidE() : base()
        {
            ID = 8;
            TotalTime = 1;
            Infinity = true;
            CanIssuedByConsole = false;
            Name = "Void";
            Description = "Void";
            UpdateTimeRemaining();
        }
    }

    internal class God : Effect
    {
        internal God() : base()
        {
            ID = 9;
            TotalTime = 1;
            Infinity = true;
            CanIssuedByConsole = false;
            Name = "God";
            Description = "Cheater!";
            UpdateTimeRemaining();
        }
    }
}