using System.Collections.Generic;

namespace SLIL.Classes
{
    public class Player : Entity
    {
        public int ID { get; set; }
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
        public bool HasEffect { get; set; }
        public int EffectTotalTime { get; set; }
        public int EffectTimeRemaining { get; set; }
        public bool Adrenaline { get; set; }
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

        public Player(double x, double y, int map_width) : base(x, y, map_width)
        {
            Dead = true;
            SetDefault();
        }

        public void SetDefault()
        {
            if (Dead)
            {
                HasEffect = false;
                Adrenaline = false;
                MAX_HP = 100;
                MAX_STAMINE = 650;
                HP = MAX_HP;
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
        }

        public Gun GetCurrentGun() => Guns[CurrentGun];

        public void InvulnerableEnd()
        {
            TimeoutInvulnerable--;
            if (TimeoutInvulnerable <= 0)
                Invulnerable = false;
        }

        public void GiveEffect(int index, bool standart_time, int time = 0)
        {
            StopEffect();
            UseItem = false;
            SelectedItem = index + 1;
            if (SelectedItem == 1)
            {
                Adrenaline = true;
                HasEffect = true;
                if (standart_time)
                    EffectTotalTime = 30;
                else
                    EffectTotalTime = time;
                EffectTimeRemaining = EffectTotalTime;
                MOVE_SPEED += 2.25;
            }
        }

        public void SetEffect()
        {
            UseItem = false;
            if (SelectedItem == 0) HealHP(rand.Next(40, 60));
            else if (SelectedItem == 1)
            {
                Adrenaline = true;
                HasEffect = true;
                EffectTotalTime = 30;
                EffectTimeRemaining = EffectTotalTime;
                MOVE_SPEED += 2.25;
            }
        }

        public void StopEffect()
        {
            HasEffect = false;
            if (Adrenaline)
            {
                Adrenaline = false;
                MOVE_SPEED -= 2.25;
            }
        }

        public void HealHP(int value)
        {
            HP += value;
            if (HP > MAX_HP)
                HP = MAX_HP;
        }

        public void DealDamage(double value)
        {
            HP -= value;
            TimeoutInvulnerable = 2;
            Invulnerable = true;
        }

        public void ChangeMoney(int value)
        {
            Money += value;
            if (Money > 9999)
                Money = 9999;
            else if (Money < 0)
                Money = 0;
        }

        protected override int GetTexture() => Texture;

        protected override double GetEntityWidth() => 0.4;
    }
}