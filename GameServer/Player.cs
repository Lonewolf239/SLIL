﻿namespace SLIL.Classes
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
        public bool UseFirstMedKit { get; set; }
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
        public readonly Gun[] GUNS = { new Flashlight(), new Knife(), new Pistol(), new Shotgun(), new SubmachineGun(), new AssaultRifle(), new SniperRifle(), new Fingershot(), new TSPitW(), new Gnome(), new FirstAidKit(), new Candy(), new Rainblower() };
        public readonly List<Gun> Guns = new List<Gun>();
        public readonly List<FirstAidKit> FirstAidKits = new List<FirstAidKit>();
        public Pet PET = null;
        public double MAX_HP { get; set; }
        public double MAX_STAMINE { get; set; }

        public Player(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID)
        {
            ID = maxEntityID;
            maxEntityID++;
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
                Guns.Clear();
                FirstAidKits.Clear();
                Money = 15;
                CurseCureChance = 0.08;
                Stage = 0;
                MOVE_SPEED = 1.75;
                RUN_SPEED = 2.25;
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
            UseFirstMedKit = false;
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

        public void HealHP(int value)
        {
            UseFirstMedKit = false;
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
            HP -= damage;
            TimeoutInvulnerable = 2;
            Invulnerable = true;
            if (HP <= 0)
                this.Dead = true;
            return Dead;
        }
    }
}