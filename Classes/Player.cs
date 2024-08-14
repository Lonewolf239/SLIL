using SharpDX.Direct2D1;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace SLIL.Classes
{
    public class Player : Entity
    {
        //public double X { get; set; }
        //public double Y { get; set; }
        //public int ID { get; set; }
        public string Name { get; set; }
        public double A { get; set; }
        public double Look { get; set; }
        public double HP { get; set; }
        public bool Invulnerable { get; set; }
        public int TimeoutInvulnerable { get; set; }
        public double STAMINE { get; set; }
        public bool CanShoot { get; set; }
        public bool Aiming { get; set; }
        public bool UseItem { get; set; }
        public bool Dead { get; set; }
        public int Money { get; set; }
        public int CurrentGun { get; set; }
        public int PreviousGun { get; set; }
        public int GunState { get; set; }
        public int MoveStyle { get; set; }
        public bool LevelUpdated { get; set; }
        public double CurseCureChance { get; set; }
        public bool IsPetting { get; set; }
        public int Stage { get; set; }
        public bool CuteMode { get; set; }
        public int EnemiesKilled { get; set; }
        public double MOVE_SPEED { get; set; }
        public double RUN_SPEED { get; set; }
        public double DEPTH { get; set; }
        public int SelectedItem { get; set; }
        public bool Fast { get; set; }
        public List<Effect> Effects = new List<Effect>();
        public readonly Gun[] GUNS =
        {
            new Flashlight(), new Knife(), new Pistol(),
            new Shotgun(), new SubmachineGun(), new AssaultRifle(),
            new SniperRifle(), new Fingershot(), new TSPitW(),
            new Gnome(), new FirstAidKit(), new Candy(),
            new Rainblower(), new Adrenalin()
        };
        public readonly List<Gun> Guns = new List<Gun>();
        public readonly List<DisposableItem> DisposableItems = new List<DisposableItem>();
        public Pet PET = null;
        public double MAX_HP { get; set; }
        public double MAX_STAMINE { get; set; }

        public Player(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID)
        {
            Texture = 17;
            base.SetAnimations(1, 0);
            Dead = true;
            SetDefault();
        }
        public Player(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID)
        {
            Texture = 17;
            base.SetAnimations(1, 0);
            Dead = true;
            SetDefault();
        }

        public void SetDefault()
        {
            if (Dead)
            {
                MAX_HP = 100;
                MAX_STAMINE = 650;
                HP = MAX_HP;
                Effects.Clear();
                Guns.Clear();
                for (int i = 0; i < DisposableItems.Count; i++)
                    DisposableItems[i].SetDefault();
                Money = 15;
                CurseCureChance = 0.08;
                Stage = 0;
                MOVE_SPEED = 1.75;
                RUN_SPEED = 2.25;
                DEPTH = 8;
                SelectedItem = 0;
                PET = null;
                CuteMode = false;
                Fast = false;
            }
            EnemiesKilled = 0;
            Look = 0;
            GunState = 0;
            CanShoot = true;
            Dead = false;
            Invulnerable = false;
            TimeoutInvulnerable = 2;
            Aiming = false;
            UseItem = false;
            LevelUpdated = false;
            IsPetting = false;
            PreviousGun = CurrentGun = 1;
            STAMINE = MAX_STAMINE;
            if (Guns.Count == 0)
            {
                Guns.Add(GUNS[1]);
                Guns.Add(GUNS[2]);
            }
        }

        public Gun GetCurrentGun() => Guns[CurrentGun];

        public void InvulnerableEnd()
        {
            TimeoutInvulnerable--;
            if (TimeoutInvulnerable <= 0)
                Invulnerable = false;
        }

        public void UpdateEffectsTime()
        {
            if (Effects.Count == 0) return;
            for (int i = Effects.Count - 1; i >= 0; i--)
            {
                if (Effects[i].ID == 0) HealHP(rand.Next(2, 6));
                if (Effects[i].ReducingTimeRemaining())
                {
                    if (Effects[i].ID == 1)
                    {
                        Fast = false;
                        MOVE_SPEED -= 1.5;
                    }
                    Effects.RemoveAt(i);
                }
            }
        }

        public void GiveEffect(int index, bool standart_time, int time = 0)
        {
            UseItem = false;
            if (index == 0)
            {
                if (EffectCheck(0)) return;
                Regeneration effect = new Regeneration();
                if (!standart_time)
                    effect.SetTotalTime(time);
                effect.UpdateTimeRemaining();
                Effects.Add(effect);
            }
            else if (index == 1)
            {
                if (EffectCheck(1)) return;
                Adrenaline effect = new Adrenaline();
                if (!standart_time)
                    effect.SetTotalTime(time);
                effect.UpdateTimeRemaining();
                Effects.Add(effect);
                Fast = true;
                MOVE_SPEED += 1.5;
            }
            else if (index == 2)
            {
                if (EffectCheck(2)) return;
                Protection effect = new Protection();
                if (!standart_time)
                    effect.SetTotalTime(time);
                effect.UpdateTimeRemaining();
                Effects.Add(effect);
            }
        }

        public bool EffectCheck(int id)
        {
            if (Effects.Count >= 4) return true;
            for (int i = 0; i < Effects.Count; i++)
                if (Effects[i].ID == id) return true;
            return false;
        }

        public void SetEffect()
        {
            UseItem = false;
            if (SelectedItem == 0)
            {
                if (EffectCheck(0)) return;
                Effects.Add(new Regeneration());
            }
            else if (SelectedItem == 1)
            {
                if (EffectCheck(1)) return;
                Effects.Add(new Adrenaline());
                Fast = true;
                MOVE_SPEED += 1.5;
            }
            else if (SelectedItem == 2)
            {
                if (EffectCheck(2)) return;
                Effects.Add(new Protection());
            }
        }

        public void StopEffects()
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                if (Effects[i].ID == 1)
                {
                    Fast = false;
                    MOVE_SPEED -= 1.5;
                }
            }
            Effects.Clear();
        }

        public void HealHP(int value)
        {
            HP += value;
            if (HP > MAX_HP)
                HP = MAX_HP;
        }

        public void ChangeMoney(int value)
        {
            Money += value;
            if (Money > 9999)
                Money = 9999;
            else if (Money < 0)
                Money = 0;
        }

        protected override int GetEntityID() => 0;

        protected override int GetTexture() => Texture;

        protected override double GetEntityWidth() => 0.4;
        public bool DealDamage(double damage)
        {
            if (EffectCheck(2)) damage *= 0.8;
            HP -= damage;
            TimeoutInvulnerable = 2;
            Invulnerable = true;
            if (HP <= 0)
                this.Dead = true;
            return Dead;
        }
    }
}