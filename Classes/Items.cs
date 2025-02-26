﻿using LiteNetLib.Utils;
using System;
using System.Linq;

namespace SLIL.Classes
{
    public enum AmmoTypes { Magic, Bubbles, Bullet, Shell, Rifle, Rocket, C4, None }
    public enum FireTypes { Single, SemiAutomatic }
    public enum Levels { LV1 = 0, LV2 = 1, LV3 = 2, LV4 = 3 }

    public abstract class Gun : INetSerializable
    {
        public int ItemID { get; set; }
        public string[] Name { get; set; }
        public bool Upgradeable { get; set; }
        public bool InMultiplayer { get; set; }
        public bool CanRun { get; set; }
        public bool ShowAmmoAsNumber { get; set; }
        public bool HasIt { get; set; }
        public bool AddToShop { get; set; }
        public bool AddToStorage { get; set; }
        public bool HaveLV4 { get; set; }
        public bool IsMagic { get; set; }
        public bool CanShoot { get; set; }
        public bool CanAiming { get; set; }
        public bool ShowAmmo { get; set; }
        public bool ShowScope { get; set; }
        public bool ShowHitScope { get; set; }
        public bool InfinityAmmo { get; set; }
        public int BulletCount { get; set; }
        public int AimingState { get; set; }
        public int AimingFactor { get; set; }
        public int RechargeTime { get; set; }
        public int AmmoInStock { get; set; }
        public int CartridgesClip { get; set; }
        public int AmmoCount { get; set; }
        public int PauseBetweenShooting { get; set; }
        public int FiringRate { get; set; }
        public int BurstShots { get; set; }
        public int RecoilY { get; set; }
        public int GunCost { get; set; }
        public int AmmoCost { get; set; }
        public int UpdateCost { get; set; }
        public int MaxAmmo { get; set; }
        public int RadiusSound { get; set; }
        public int ReloadFrames { get; set; }
        public double Weight { get; set; }
        public double Accuracy { get; set; }
        public double FiringRange { get; set; }
        public double MaxDamage { get; set; }
        public double MinDamage { get; set; }
        public double RecoilX { get; set; }
        public AmmoTypes AmmoType { get; set; }
        public FireTypes FireType { get; set; }
        public Levels Level { get; set; }

        public Gun()
        {
            ItemID = GetItemID();
            Level = Levels.LV1;
            Accuracy = 1;
            BulletCount = 1;
            PauseBetweenShooting = 500;
            AddToStorage = true;
            Upgradeable = true;
            CanRun = true;
            Weight = 1;
            InMultiplayer = true;
            ShowAmmoAsNumber = false;
            IsMagic = false;
            CanAiming = false;
            CanShoot = true;
            ShowAmmo = true;
            ShowScope = true;
            ShowHitScope = true;
            InfinityAmmo = false;
        }

        public abstract int GetItemID();

        public double GetRecoilX(double rand) => (rand <= 0.5 ? RecoilX : -RecoilX) / 100;

        public virtual void SetDefault()
        {
            Level = Levels.LV1;
            ApplyUpdate();
            HasIt = false;
        }

        public virtual void ReloadClip()
        {
            AmmoInStock += AmmoCount;
            AmmoCount = 0;
            if (AmmoInStock >= CartridgesClip)
            {
                AmmoCount = CartridgesClip;
                AmmoInStock -= CartridgesClip;
            }
            else
            {
                AmmoCount = AmmoInStock;
                AmmoInStock = 0;
            }
        }

        public int GetLevel()
        {
            switch (Level)
            {
                case Levels.LV1:
                    return 0;
                case Levels.LV2:
                    return 1;
                case Levels.LV3:
                    return 2;
                default:
                    return 3;
            }
        }

        protected virtual void ApplyUpdate() => AmmoCount = CartridgesClip;

        public virtual void LevelUpdate()
        {
            if (CanUpdate())
            {
                Level++;
                UpdateCost += 20;
                ApplyUpdate();
            }
        }

        public virtual void LevelDowngrade()
        {
            if (CanDowngrade())
            {
                Level--;
                UpdateCost -= 20;
                ApplyUpdate();
            }
        }

        public virtual bool CanUpdate() => !HaveLV4 && Level != Levels.LV3 || HaveLV4 && Level != Levels.LV4;

        public virtual bool CanDowngrade() => Level != Levels.LV1;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((int)Level);
            writer.Put(AmmoCount);
            writer.Put(AmmoInStock);
            writer.Put(HasIt);
        }

        public void Deserialize(NetDataReader reader)
        {
            Levels level = (Levels)reader.GetInt();
            if (level > Level && level <= Enum.GetValues(typeof(Levels)).Cast<Levels>().Max())
            {
                while (Level != level)
                    LevelUpdate();
            }
            AmmoCount = reader.GetInt();
            AmmoInStock = reader.GetInt();
            HasIt = reader.GetBool();
        }
    }

    public abstract class Magic : Gun
    {
        public Magic() : base()
        {
            AmmoType = AmmoTypes.Magic;
            FireType = FireTypes.Single;
            Upgradeable = false;
            AddToShop = false;
            HasIt = false;
            ShowAmmoAsNumber = true;
            IsMagic = true;
            Accuracy = 1;
            CartridgesClip = 1;
            AmmoCount = CartridgesClip;
            FiringRange = 10;
            RadiusSound = 0;
            BurstShots = 1;
        }

        public override bool CanUpdate() => false;
    }

    public abstract class Item : Gun
    {
        public string[] Description { get; set; }
        public bool HasCuteDescription { get; set; }

        public Item() : base()
        {
            FireType = FireTypes.Single;
            Upgradeable = false;
            HasCuteDescription = false;
            ShowScope = false;
            ShowAmmo = false;
            AddToStorage = false;
            AddToShop = false;
            CanShoot = false;
            Accuracy = 0;
            CartridgesClip = 1;
            AmmoInStock = 1;
            PauseBetweenShooting = 500;
            RechargeTime = 1;
            FiringRate = 1;
            MaxAmmo = 1;
            BurstShots = 1;
            Description = null;
            AmmoCount = CartridgesClip;
        }

        public override bool CanUpdate() => false;
    }

    public abstract class DisposableItem : Item
    {
        public int Count { get; set; }
        public int MaxCount { get; set; }
        public int EffectID { get; set; }
        public bool HasLVMechanics { get; set; }

        public DisposableItem() : base()
        {
            CanRun = false;
            HasIt = false;
            HaveLV4 = false;
            HasCuteDescription = false;
            HasLVMechanics = false;
            RechargeTime = 980;
            Count = 0;
            MaxAmmo = 2;
            FiringRate = 150;
            ReloadFrames = 3;
        }

        public void AddItem(int count = 1)
        {
            Count += count;
            if (Count >= MaxCount) Count = MaxCount;
            HasIt = true;
        }

        public void RemoveItem(int count = 1)
        {
            Count -= count;
            if (Count < 0) SetDefault();
        }

        public bool CanBuy() => Count < MaxCount;

        public override void SetDefault()
        {
            Count = 0;
            HasIt = false;
        }

        public override bool CanUpdate() => false;
    }

    public class Flashlight : Item
    {
        public Flashlight() : base()
        {
            AimingFactor = 4;
            Weight = 1;
            HasIt = true;
            Name = new[] { "3-0", "Flashlight" };
        }

        public override void SetDefault()
        {
            Level = Levels.LV1;
            ApplyUpdate();
        }
        public override int GetItemID() => 0;
    }

    public class Knife : Gun
    {
        public Knife() : base()
        {
            InfinityAmmo = true;
            Weight = 1;
            Upgradeable = false;
            ShowAmmo = false;
            AddToStorage = false;
            AddToShop = false;
            HasIt = true;
            Name = new[] { "3-1", "Knife" };
            FireType = FireTypes.Single;
            Accuracy = 1;
            PauseBetweenShooting = 500;
            RechargeTime = 600;
            FiringRate = 175;
            CartridgesClip = 1;
            AmmoInStock = CartridgesClip;
            MaxAmmo = CartridgesClip;
            FiringRange = 1.5;
            MaxDamage = 2;
            MinDamage = 1.5;
            RecoilY = 0;
            RecoilX = 0;
            BurstShots = 1;
            RadiusSound = 0;
            ReloadFrames = 1;
            AmmoCount = CartridgesClip;
        }

        public override void SetDefault()
        {
            Level = Levels.LV1;
            ApplyUpdate();
        }
        public override bool CanUpdate() => false;
        public override int GetItemID() => 1;
    }

    public class Candy : Knife
    {
        public Candy() : base()
        {
            FiringRate = 400;
            ShowScope = false;
            HasIt = false;
            Name = new[] { "3-2", "Candy" };
        }

        public override int GetItemID() => 2;
    }

    public class Petition : Knife
    {
        public Petition() : base()
        {
            FiringRate = 2900;
            PauseBetweenShooting = 3000;
            FiringRange = 2.5;
            MaxDamage = 12.5;
            MinDamage = 7.5;
            AddToStorage = true;
            ShowScope = false;
            ShowHitScope = false;
            HasIt = false;
            Name = new[] { "3-14", "Petition" };
        }

        public override int GetItemID() => 17;
    }

    public class Rainblower : Gun
    {
        public Rainblower() : base()
        {
            AmmoType = AmmoTypes.Bubbles;
            FireType = FireTypes.SemiAutomatic;
            Upgradeable = false;
            ShowScope = false;
            AddToStorage = false;
            AddToShop = false;
            HasIt = false;
            ShowAmmoAsNumber = true;
            Name = new[] { "3-3", "Rainblower" };
            Accuracy = 0.9;
            Weight = 0.8;
            BulletCount = 2;
            PauseBetweenShooting = 500;
            RechargeTime = 600;
            FiringRate = 125;
            CartridgesClip = 100;
            AmmoInStock = 0;
            MaxAmmo = 100;
            FiringRange = 2.5;
            MaxDamage = 2.25;
            MinDamage = 2;
            RecoilY = 3;
            RecoilX = 1.75;
            BurstShots = 5;
            RadiusSound = 6;
            ReloadFrames = 1;
            AmmoCount = CartridgesClip;
        }

        public override void SetDefault()
        {
            HasIt = false;
            Level = Levels.LV1;
            ApplyUpdate();
        }
        public override bool CanUpdate() => false;
        public override int GetItemID() => 3;
    }

    public class Pistol : Gun
    {
        public Pistol() : base()
        {
            AmmoType = AmmoTypes.Bullet;
            FireType = FireTypes.Single;
            AddToStorage = false;
            AddToShop = true;
            HasIt = true;
            HaveLV4 = true;
            Name = new[] { "3-4", "Pistol" };
            Accuracy = 0.85;
            Weight = 1;
            PauseBetweenShooting = 500;
            RechargeTime = 600;
            FiringRate = 175;
            UpdateCost = 20;
            AmmoCost = 5;
            CartridgesClip = 8;
            AmmoInStock = CartridgesClip * 2;
            MaxAmmo = CartridgesClip * 6;
            FiringRange = 7;
            MaxDamage = 1.75;
            MinDamage = 1.25;
            RecoilY = 10;
            RecoilX = 1.25;
            BurstShots = 1;
            RadiusSound = 6;
            ReloadFrames = 1;
            AmmoCount = CartridgesClip;
        }

        public override void SetDefault()
        {
            Level = Levels.LV1;
            ApplyUpdate();
        }
        protected override void ApplyUpdate()
        {
            if (Level == Levels.LV1)
            {
                UpdateCost = 20;
                Accuracy = 0.85;
                Weight = 1;
                RechargeTime = 600;
                CartridgesClip = 8;
                MaxAmmo = CartridgesClip * 6;
                FiringRange = 7;
                MaxDamage = 1.75;
                MinDamage = 1.25;
                RecoilY = 10;
                RecoilX = 1.25;
                FiringRate = 175;
                BurstShots = 1;
                RadiusSound = 6;
                ReloadFrames = 1;
            }
            else if (Level == Levels.LV2)
            {
                Accuracy = 0.95;
                Weight = 1;
                RechargeTime = 350;
                CartridgesClip = 12;
                MaxAmmo = CartridgesClip * 7;
                FiringRange = 8;
                MaxDamage = 2.75;
                MinDamage = 2.5;
                RecoilY = 20;
                RecoilX = 1.25;
                FiringRate = 175;
                BurstShots = 1;
                RadiusSound = 7;
                ReloadFrames = 1;
            }
            else if (Level == Levels.LV3)
            {
                Accuracy = 0.9;
                Weight = 1;
                RechargeTime = 400;
                CartridgesClip = 7;
                MaxAmmo = CartridgesClip * 6;
                FiringRange = 9;
                MaxDamage = 3.45;
                MinDamage = 3;
                RecoilY = 25;
                RecoilX = 1.5;
                FiringRate = 175;
                BurstShots = 1;
                RadiusSound = 10;
                ReloadFrames = 1;
            }
            else
            {
                Accuracy = 1;
                Weight = 0.98;
                RechargeTime = 400;
                CartridgesClip = 6;
                MaxAmmo = CartridgesClip * 6;
                FiringRange = 9;
                MaxDamage = 10.5;
                MinDamage = 5;
                RecoilY = 35;
                RecoilX = 2;
                FiringRate = 225;
                BurstShots = 1;
                RadiusSound = 10;
                ReloadFrames = 2;
            }
            AmmoInStock = CartridgesClip * 2;
            base.ApplyUpdate();
        }
        public override int GetItemID() => 4;
    }

    public class Shotgun : Gun
    {
        public int PullTime { get; set; }

        public Shotgun() : base()
        {
            AmmoType = AmmoTypes.Shell;
            FireType = FireTypes.Single;
            AddToShop = true;
            HasIt = false;
            HaveLV4 = false;
            Name = new[] { "3-5", "Shotgun" };
            Accuracy = 0.4;
            Weight = 0.85;
            BulletCount = 5;
            PauseBetweenShooting = 350;
            RechargeTime = 425;
            FiringRate = 200;
            PullTime = 1;
            UpdateCost = 30;
            GunCost = 35;
            AmmoCost = 12;
            CartridgesClip = 2;
            AmmoInStock = CartridgesClip * 2;
            MaxAmmo = CartridgesClip * 8;
            FiringRange = 7;
            MaxDamage = 2.25;
            MinDamage = 2;
            RecoilY = 120;
            RecoilX = 2;
            BurstShots = 1;
            RadiusSound = 10;
            ReloadFrames = 2;
            AmmoCount = CartridgesClip;
        }

        public override void ReloadClip()
        {
            if (Level == Levels.LV1)
            {
                AmmoInStock += AmmoCount;
                AmmoCount = 0;
                if (AmmoInStock >= CartridgesClip)
                {
                    AmmoCount = CartridgesClip;
                    AmmoInStock -= CartridgesClip;
                }
                else
                {
                    AmmoCount = AmmoInStock;
                    AmmoInStock = 0;
                }
            }
            else
            {
                int ammo = Level == Levels.LV3 ? 2 : 1;
                if (AmmoCount + ammo >= CartridgesClip)
                {
                    AmmoInStock += AmmoCount;
                    AmmoCount = CartridgesClip;
                    AmmoInStock -= CartridgesClip;
                }
                else
                {
                    if (AmmoInStock - ammo < 0)
                    {
                        if (AmmoInStock - ammo / 2 >= 0)
                        {
                            AmmoInStock -= ammo / 2;
                            AmmoCount += ammo / 2;
                        }
                    }
                    else
                    {
                        AmmoInStock -= ammo;
                        AmmoCount += ammo;
                    }
                }
            }
        }
        protected override void ApplyUpdate()
        {
            if (Level == Levels.LV1)
            {
                UpdateCost = 30;
                BulletCount = 5;
                Weight = 0.85;
                PauseBetweenShooting = 350;
                RechargeTime = 425;
                FiringRate = 200;
                PullTime = 1;
                CartridgesClip = 2;
                MaxAmmo = CartridgesClip * 8;
                FiringRange = 7;
                MaxDamage = 2.25;
                MinDamage = 2;
                RecoilY = 120;
                RecoilX = 2;
                BurstShots = 1;
                RadiusSound = 10;
                ReloadFrames = 2;
            }
            else if (Level == Levels.LV2)
            {
                BulletCount = 8;
                Weight = 0.85;
                PauseBetweenShooting = 500;
                RechargeTime = 325;
                FiringRate = 200;
                PullTime = 500;
                CartridgesClip = 6;
                MaxAmmo = CartridgesClip * 7;
                FiringRange = 6;
                MaxDamage = 3;
                MinDamage = 2.75;
                RecoilY = 80;
                RecoilX = 1.65;
                BurstShots = 1;
                RadiusSound = 10;
                ReloadFrames = 3;
            }
            else
            {
                BulletCount = 9;
                Weight = 0.78;
                PauseBetweenShooting = 500;
                RechargeTime = 325;
                FiringRate = 200;
                PullTime = 300;
                CartridgesClip = 14;
                MaxAmmo = CartridgesClip * 4;
                FiringRange = 6;
                MaxDamage = 3.75;
                MinDamage = 3.5;
                RecoilY = 135;
                RecoilX = 2;
                BurstShots = 1;
                RadiusSound = 10;
                ReloadFrames = 3;
            }
            AmmoInStock = CartridgesClip * 2;
            base.ApplyUpdate();
        }
        public override int GetItemID() => 5;
    }

    public class SubmachineGun : Gun
    {
        public SubmachineGun() : base()
        {
            AmmoType = AmmoTypes.Bullet;
            FireType = FireTypes.Single;
            AddToShop = true;
            HasIt = false;
            HaveLV4 = false;
            Name = new[] { "3-6", "Submachine gun" };
            Accuracy = 0.65;
            Weight = 0.85;
            RechargeTime = 375;
            BulletCount = 1;
            PauseBetweenShooting = 250;
            FiringRate = 125;
            UpdateCost = 40;
            GunCost = 30;
            AmmoCost = 18;
            CartridgesClip = 20;
            AmmoInStock = CartridgesClip * 2;
            MaxAmmo = CartridgesClip * 6;
            FiringRange = 8;
            MaxDamage = 2;
            MinDamage = 1.5;
            RecoilY = 18;
            RecoilX = 1.75;
            BurstShots = 1;
            RadiusSound = 6;
            ReloadFrames = 2;
            AmmoCount = CartridgesClip;
        }

        protected override void ApplyUpdate()
        {
            if (Level == Levels.LV1)
            {
                UpdateCost = 40;
                Weight = 0.85;
                Accuracy = 0.65;
                BulletCount = 1;
                PauseBetweenShooting = 250;
                FiringRate = 125;
                RechargeTime = 375;
                CartridgesClip = 20;
                MaxAmmo = CartridgesClip * 6;
                FiringRange = 8;
                MaxDamage = 2;
                MinDamage = 1.5;
                RecoilY = 18;
                RecoilX = 1.75;
                RadiusSound = 6;
                ReloadFrames = 2;
            }
            else if (Level == Levels.LV2)
            {
                Accuracy = 0.8;
                Weight = 0.9;
                RechargeTime = 350;
                BulletCount = 1;
                PauseBetweenShooting = 240;
                FiringRate = 120;
                CartridgesClip = 30;
                MaxAmmo = CartridgesClip * 6;
                FiringRange = 8;
                MaxDamage = 2.5;
                MinDamage = 1.75;
                RecoilY = 15;
                RecoilX = 1.15;
                RadiusSound = 6;
                ReloadFrames = 2;
            }
            else
            {
                Accuracy = 0.5;
                Weight = 0.88;
                RechargeTime = 350;
                BulletCount = 2;
                PauseBetweenShooting = 180;
                FiringRate = 90;
                CartridgesClip = 30;
                MaxAmmo = CartridgesClip * 6;
                FiringRange = 8;
                MaxDamage = 5.4;
                MinDamage = 3.9;
                RecoilY = 25;
                RecoilX = 2.25;
                RadiusSound = 6;
                ReloadFrames = 2;
            }
            AmmoInStock = CartridgesClip * 2;
            base.ApplyUpdate();
        }
        public override int GetItemID() => 6;
    }

    public class AssaultRifle : Gun
    {
        public AssaultRifle() : base()
        {
            AmmoType = AmmoTypes.Rifle;
            FireType = FireTypes.SemiAutomatic;
            AddToShop = true;
            HasIt = false;
            HaveLV4 = false;
            Name = new[] { "3-7", "Assault rifle" };
            Accuracy = 0.75;
            Weight = 0.75;
            PauseBetweenShooting = 750;
            RechargeTime = 700;
            FiringRate = 100;
            UpdateCost = 50;
            GunCost = 45;
            AmmoCost = 25;
            CartridgesClip = 30;
            AmmoInStock = CartridgesClip * 2;
            MaxAmmo = CartridgesClip * 5;
            FiringRange = 8;
            MaxDamage = 2.5;
            MinDamage = 2;
            RecoilY = 30;
            RecoilX = 1.75;
            BurstShots = 3;
            RadiusSound = 13;
            ReloadFrames = 2;
            AmmoCount = CartridgesClip;
        }

        protected override void ApplyUpdate()
        {
            if (Level == Levels.LV1)
            {
                UpdateCost = 50;
                FireType = FireTypes.SemiAutomatic;
                Accuracy = 0.75;
                Weight = 0.75;
                PauseBetweenShooting = 750;
                RechargeTime = 700;
                FiringRate = 100;
                CartridgesClip = 30;
                MaxAmmo = CartridgesClip * 5;
                FiringRange = 8;
                MaxDamage = 2.5;
                MinDamage = 2;
                RecoilY = 30;
                RecoilX = 1.75;
                BurstShots = 3;
                RadiusSound = 13;
                ReloadFrames = 2;
            }
            else if (Level == Levels.LV2)
            {
                FireType = FireTypes.SemiAutomatic;
                Accuracy = 0.85;
                Weight = 0.8;
                PauseBetweenShooting = 650;
                RechargeTime = 450;
                FiringRate = 100;
                CartridgesClip = 30;
                MaxAmmo = CartridgesClip * 5;
                FiringRange = 8;
                MaxDamage = 3.25;
                MinDamage = 2.75;
                RecoilY = 25;
                RecoilX = 1.5;
                BurstShots = 3;
                RadiusSound = 13;
                ReloadFrames = 2;
            }
            else
            {
                FireType = FireTypes.Single;
                Accuracy = 0.9;
                Weight = 0.85;
                PauseBetweenShooting = 500;
                RechargeTime = 400;
                CartridgesClip = 20;
                MaxAmmo = CartridgesClip * 4;
                FiringRange = 8;
                MaxDamage = 5.25;
                MinDamage = 4.5;
                RecoilY = 40;
                RecoilX = 2.5;
                FiringRate = 175;
                BurstShots = 1;
                RadiusSound = 13;
                ReloadFrames = 2;
            }
            AmmoInStock = CartridgesClip * 2;
            base.ApplyUpdate();
        }
        public override int GetItemID() => 7;
    }

    public class SniperRifle : Gun
    {
        public SniperRifle() : base()
        {
            AmmoType = AmmoTypes.Rifle;
            FireType = FireTypes.Single;
            CanRun = true;
            ShowScope = false;
            AddToShop = true;
            HasIt = false;
            CanAiming = true;
            Name = new[] { "3-8", "Sniper rifle" };
            RechargeTime = 650;
            Accuracy = 0.96;
            Weight = 0.75;
            FiringRate = 200;
            UpdateCost = 60;
            GunCost = 55;
            AmmoCost = 30;
            AimingState = 5;
            AimingFactor = 4;
            CartridgesClip = 1;
            MaxAmmo = CartridgesClip * 15;
            FiringRange = 14;
            MaxDamage = 11;
            MinDamage = 5;
            RecoilY = 35;
            RecoilX = 2.5;
            BurstShots = 1;
            RadiusSound = 20;
            ReloadFrames = 3;
            AmmoCount = CartridgesClip;
        }

        protected override void ApplyUpdate()
        {
            if (Level == Levels.LV1)
            {
                AimingFactor = 4;
                CanRun = true;
                Accuracy = 0.96;
                Weight = 0.75;
                RechargeTime = 650;
                CartridgesClip = 1;
                MaxAmmo = CartridgesClip * 15;
                FiringRange = 14;
                MaxDamage = 11;
                MinDamage = 5;
                RecoilY = 35;
                RecoilX = 2.5;
                BurstShots = 1;
                RadiusSound = 20;
                ReloadFrames = 3;
            }
            else if (Level == Levels.LV2)
            {
                AimingFactor = 5;
                CanRun = true;
                Accuracy = 0.98;
                Weight = 0.75;
                RechargeTime = 650;
                CartridgesClip = 10;
                MaxAmmo = CartridgesClip * 4;
                FiringRange = 15;
                MaxDamage = 20;
                MinDamage = 10;
                RecoilY = 55;
                RecoilX = 3.5;
                BurstShots = 1;
                RadiusSound = 25;
                ReloadFrames = 3;
            }
            else
            {
                AimingFactor = 12;
                CanRun = false;
                Weight = 0.55;
                Accuracy = 1;
                RechargeTime = 772;
                CartridgesClip = 10;
                MaxAmmo = CartridgesClip * 4;
                FiringRange = 22;
                MaxDamage = 29;
                MinDamage = 19;
                RecoilY = 150;
                RecoilX = 4.25;
                BurstShots = 1;
                RadiusSound = 20;
                ReloadFrames = 2;
            }
            AmmoInStock = CartridgesClip * 2;
            base.ApplyUpdate();
        }
        public override int GetItemID() => 8;
    }

    public class Fingershot : Gun
    {
        public Fingershot() : base()
        {
            AmmoType = AmmoTypes.Bullet;
            FireType = FireTypes.Single;
            Upgradeable = false;
            AddToShop = false;
            HasIt = false;
            InMultiplayer = false;
            Name = new[] { "3-9", "Fingershot" };
            Accuracy = 1;
            Weight = 0.9;
            RechargeTime = 600;
            FiringRate = 175;
            CartridgesClip = 1;
            AmmoInStock = 99;
            MaxAmmo = 99;
            FiringRange = 10;
            MaxDamage = 25;
            MinDamage = 20;
            RecoilY = 350;
            RecoilX = 10;
            BurstShots = 1;
            RadiusSound = 0;
            ReloadFrames = 2;
            AmmoCount = CartridgesClip;
        }

        public override bool CanUpdate() => false;
        public override int GetItemID() => 9;
    }

    public class TSPitW : Gun
    {
        public TSPitW() : base()
        {
            AmmoType = AmmoTypes.Bullet;
            FireType = FireTypes.Single;
            Upgradeable = false;
            CanRun = false;
            AddToShop = false;
            HasIt = false;
            InMultiplayer = false;
            Name = new[] { "3-10", "TSPitW" };
            Accuracy = 0.8;
            Weight = 0.5;
            RechargeTime = 750;
            FiringRate = 175;
            CartridgesClip = 7;
            AmmoInStock = CartridgesClip * 4;
            MaxAmmo = CartridgesClip * 4;
            FiringRange = 16;
            MaxDamage = 50;
            MinDamage = 45;
            RecoilY = 350;
            RecoilX = 10;
            BurstShots = 1;
            RadiusSound = 0;
            ReloadFrames = 3;
            AmmoCount = CartridgesClip;
        }

        public override bool CanUpdate() => false;
        public override int GetItemID() => 10;
    }

    public class Gnome : Magic
    {
        public Gnome() : base()
        {
            InMultiplayer = false;
            Name = new[] { "3-11", "Wizard Gnome" };
            RecoilY = 35;
            RecoilX = 2;
            Weight = 0.88;
            RechargeTime = 300;
            AmmoInStock = 99;
            MaxAmmo = 99;
            MaxDamage = 25;
            MinDamage = 20;
            FiringRate = 650;
            ReloadFrames = 4;
        }

        public override int GetItemID() => 11;
    }

    public class RPG : Gun
    {
        public RPG() : base()
        {
            AmmoType = AmmoTypes.Rocket;
            FireType = FireTypes.Single;
            Upgradeable = false;
            CanRun = false;
            ShowScope = false;
            AddToShop = true;
            HasIt = false;
            InMultiplayer = true;
            Name = new[] { "3-12", "RPG7" };
            Accuracy = 1;
            Weight = 0.65;
            GunCost = 150;
            AmmoCost = 50;
            RechargeTime = 440;
            CartridgesClip = 1;
            AmmoInStock = CartridgesClip * 2;
            MaxAmmo = CartridgesClip * 4;
            FiringRange = 30;
            MaxDamage = 40;
            MinDamage = 25;
            RecoilY = 0;
            RecoilX = 0;
            FiringRate = 200;
            BurstShots = 1;
            RadiusSound = 20;
            ReloadFrames = 3;
            AmmoCount = CartridgesClip;
        }

        public override void LevelUpdate() => ApplyUpdate();
        public override void LevelDowngrade() => ApplyUpdate();
        public override bool CanUpdate() => false;
        public override int GetItemID() => 15;
    }

    public class C4 : Gun
    {
        public C4() : base()
        {
            AmmoType = AmmoTypes.C4;
            FireType = FireTypes.Single;
            Upgradeable = false;
            ShowScope = false;
            AddToShop = true;
            HasIt = false;
            InMultiplayer = true;
            Name = new[] { "3-13", "C4" };
            Accuracy = 1;
            Weight = 0.9;
            GunCost = 100;
            AmmoCost = 50;
            RechargeTime = 350;
            CartridgesClip = 1;
            AmmoInStock = CartridgesClip * 2;
            MaxAmmo = CartridgesClip * 4;
            FiringRange = 0;
            MaxDamage = 40;
            MinDamage = 25;
            RecoilY = 0;
            RecoilX = 0;
            FiringRate = 0;
            BurstShots = 1;
            RadiusSound = 0;
            ReloadFrames = 1;
            AmmoCount = CartridgesClip;
        }

        public override void LevelUpdate() => ApplyUpdate();
        public override void LevelDowngrade() => ApplyUpdate();
        public override bool CanUpdate() => false;
        public override int GetItemID() => 16;
    }

    public class FirstAidKit : DisposableItem
    {
        public FirstAidKit() : base()
        {
            EffectID = 0;
            HasLVMechanics = true;
            HasCuteDescription = true;
            GunCost = 15;
            MaxCount = 4;
            Name = new[]
            {
                "4-0", "First Aid Kit",
                "4-1", "Beans"
            };
            Description = new[]
            {
                "4-2",
                "Restores health",
                "4-3",
                "A tasty snack"
            };
        }

        public override int GetItemID() => 12;
    }

    public class Adrenalin : DisposableItem
    {
        public Adrenalin() : base()
        {
            EffectID = 1;
            RechargeTime = 530;
            GunCost = 30;
            MaxCount = 2;
            Name = new[] { "4-4", "Adrenalin" };
            Description = new[]
            {
                "4-5",
                "Increases movement speed for 20 sec",
            };
        }

        public override int GetItemID() => 13;
    }

    public class Helmet : DisposableItem
    {
        public Helmet() : base()
        {
            EffectID = 2;
            ReloadFrames = 4;
            RechargeTime = 1000;
            GunCost = 40;
            MaxCount = 1;
            Name = new[] { "4-6", "Helmet" };
            Description = new[]
            {
                "4-7",
                "Reduces incoming damage by 20% for 2 minutes",
            };
        }

        public override int GetItemID() => 14;
    }

    public class MedicalKit : DisposableItem
    {
        public MedicalKit() : base()
        {
            EffectID = 4;
            RechargeTime = 666;
            GunCost = 60;
            MaxCount = 1;
            Name = new[] { "4-8", "Medical kit" };
            Description = new[]
            {
                "4-9",
                "Restores 80% of lost health",
            };
        }

        public override int GetItemID() => 18;
    }

    public class Minigun : Gun
    {
        public Minigun() : base()
        {
            AmmoType = AmmoTypes.Bullet;
            FireType = FireTypes.Single;
            Upgradeable = false;
            CanRun = false;
            ShowScope = false;
            AddToShop = false;
            HasIt = false;
            InMultiplayer = true;
            ShowAmmoAsNumber = true;
            Name = new[] { "3-15", "Minigun" };
            Accuracy = 0.75;
            Weight = 0.5;
            RechargeTime = 375;
            BulletCount = 1;
            PauseBetweenShooting = 80;
            FiringRate = 40;
            UpdateCost = 0;
            GunCost = 0;
            AmmoCost = 0;
            CartridgesClip = 1488;
            AmmoInStock = 0;
            MaxAmmo = 0;
            FiringRange = 8;
            MaxDamage = 4;
            MinDamage = 3.5;
            RecoilY = 22;
            RecoilX = 2.4;
            BurstShots = 1;
            RadiusSound = 6;
            ReloadFrames = 2;
            AmmoCount = CartridgesClip;
        }

        public override void LevelUpdate() => ApplyUpdate();
        public override void LevelDowngrade() => ApplyUpdate();
        public override bool CanUpdate() => false;
        public override int GetItemID() => 19;
    }
}