using System.Drawing;
using Play_Sound;

namespace SLIL.Classes
{
    public enum FireTypes { Single, SemiAutomatic }
    public enum Levels { LV1 = 0, LV2 = 1, LV3 = 2, LV4 = 3 }

    public class Gun
    {
        public string[] Name { get; set; }
        public int RechargeTime { get; set; }
        public int MaxAmmoCount { get; set; }
        public int CartridgesClip { get; set; }
        public int AmmoCount { get; set; }
        public double FiringRange { get; set; }
        public int FiringRate { get; set; }
        public int BurstShots { get; set; }
        public double MaxDamage { get; set; }
        public double MinDamage { get; set; }
        public int Recoil { get; set; }
        public bool HasIt { get; set; }
        public int GunCost { get; set; }
        public int AmmoCost { get; set; }
        public int UpdateCost { get; set; }
        public int MaxAmmo { get; set; }
        public int RadiusSound { get; set; }
        public int ReloadFrames { get; set; }
        public bool AddToShop { get; set; }
        public bool HaveLV4 { get; set; }
        public bool IsMagic { get; set; }
        public bool CanShoot { get; set; }
        public bool CanAiming { get; set; }
        public bool ShowAmmo { get; set; }
        public bool ShowScope { get; set; }
        public FireTypes FireType { get; set; }
        public Levels Level { get; set; }
        public Image[] Icon { get; set; }
        public Image[,] Images { get; set; }
        public PlaySound[,] Sounds { get; set; }

        public Gun()
        {
            Level = Levels.LV1;
            IsMagic = false;
            CanAiming = false;
            CanShoot = true;
            ShowAmmo = true;
            ShowScope = true;
        }

        public virtual void SetDefault()
        {
            Level = Levels.LV1;
            ApplyUpdate();
            HasIt = false;
        }

        public void ReloadClip()
        {
            MaxAmmoCount += AmmoCount;
            AmmoCount = 0;
            if (MaxAmmoCount >= CartridgesClip)
            {
                AmmoCount = CartridgesClip;
                MaxAmmoCount -= CartridgesClip;
            }
            else
            {
                AmmoCount = MaxAmmoCount;
                MaxAmmoCount = 0;
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

        public virtual bool CanUpdate() => (!HaveLV4 && Level != Levels.LV3) || (HaveLV4 && Level != Levels.LV4);
    }

    public class Magic : Gun
    {
        public Magic() : base()
        {
            AddToShop = false;
            HasIt = false;
            IsMagic = true;
            FireType = FireTypes.Single;
            CartridgesClip = 1;
            AmmoCount = CartridgesClip;
            FiringRange = 10;
            Recoil = 50;
            RadiusSound = 0;
            BurstShots = 1;
            Icon = new[] { Properties.Resources.missing };
        }

        public override bool CanUpdate() => false;
    }

    public class Item : Gun
    {
        public string[] Description { get; set; }
        public bool HasCuteDescription { get; set; }

        public Item() : base()
        {
            FireType = FireTypes.Single;
            HasCuteDescription = false;
            ShowScope = false;
            ShowAmmo = false;
            AddToShop = false;
            CanShoot = false;
            CartridgesClip = 1;
            MaxAmmoCount = 1;
            RechargeTime = 1;
            FiringRate = 1;
            MaxAmmo = 1;
            BurstShots = 1;
            Description = null;
            AmmoCount = CartridgesClip;
            Icon = new[] { Properties.Resources.missing };
        }

        public override bool CanUpdate() => false;
    }

    public class DisposableItem : Item
    {
        public bool HasLVMechanics { get; set; }
        public Image CuteItemIcon { get; set; }
        public Image ItemIcon { get; set; }

        public DisposableItem() : base()
        {
            HasIt = false;
            HaveLV4 = false;
            HasCuteDescription = false;
            HasLVMechanics = false;
            RechargeTime = 980;
            AmmoCount = 0;
            MaxAmmoCount = 0;
            MaxAmmo = 2;
            FiringRate = 150;
            ReloadFrames = 3;
        }

        public void AddItem()
        {
            AmmoCount = CartridgesClip;
            MaxAmmoCount = CartridgesClip;
            HasIt = true;
        }

        public override void SetDefault()
        {
            AmmoCount = 0;
            MaxAmmoCount = 0;
            HasIt = false;
        }

        public override bool CanUpdate() => false;
    }

    public class Flashlight : Item
    {
        public Flashlight() : base()
        {
            HasIt = true;
            Name = new[] { "Фонарик", "Flashlight" };
            Images = new[,] { { Properties.Resources.flashlight, Properties.Resources.flashlight_run } };
            Sounds = new[,] { { new PlaySound(null, false), } };
        }

        public override void SetDefault()
        {
            Level = Levels.LV1;
            ApplyUpdate();
        }
    }

    public class Knife : Gun
    {
        public Knife() : base()
        {
            ShowAmmo = false;
            AddToShop = false;
            HasIt = true;
            Name = new[] { "Нож", "Knife" };
            FireType = FireTypes.Single;
            RechargeTime = 600;
            CartridgesClip = 1;
            MaxAmmoCount = CartridgesClip;
            MaxAmmo = CartridgesClip;
            FiringRange = 1.5;
            MaxDamage = 2;
            MinDamage = 1.5;
            Recoil = 0;
            FiringRate = 175;
            BurstShots = 1;
            RadiusSound = 0;
            ReloadFrames = 1;
            Icon = new[] { Properties.Resources.missing };
            Images = new[,] { { Properties.Resources.knife, Properties.Resources.knife_hit, Properties.Resources.knife_run } };
            Sounds = new[,] { { new PlaySound(MainMenu.CGFReader.GetFile("knife.wav"), false) } };
            AmmoCount = CartridgesClip;
        }

        public override void SetDefault()
        {
            Level = Levels.LV1;
            ApplyUpdate();
        }

        public override bool CanUpdate() => false;
    }

    public class Candy : Knife
    {
        public Candy() : base()
        {
            FiringRate = 400;
            ShowScope = false;
            HasIt = false;
            Name = new[] { "Конфета", "Candy" };
            Images = new[,] { { Properties.Resources.gun_candy, Properties.Resources.gun_candy_shooted, Properties.Resources.gun_candy_run } };
            Sounds = new[,] { { new PlaySound(MainMenu.CGFReader.GetFile("candy.wav"), false) } };
        }
    }

    public class Rainblower : Gun
    {
        public Rainblower() : base()
        {
            ShowScope = false;
            AddToShop = false;
            HasIt = false;
            Name = new[] { "Радужигатель", "Rainblower" };
            FireType = FireTypes.SemiAutomatic;
            RechargeTime = 600;
            CartridgesClip = 100;
            MaxAmmoCount = 0;
            MaxAmmo = 100;
            FiringRange = 2.5;
            MaxDamage = 3.75;
            MinDamage = 3.5;
            Recoil = 5;
            FiringRate = 125;
            BurstShots = 5;
            RadiusSound = 6;
            ReloadFrames = 1;
            Icon = new[] { Properties.Resources.missing };
            Images = new[,]
            {                    
                   { Properties.Resources.gun_rainblower, Properties.Resources.gun_rainblower_shooted, Properties.Resources.gun_rainblower_run },
            };
            Sounds = new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("gun_rainblower.wav"), false), new PlaySound(null, false), new PlaySound(null, false) },
            };
            AmmoCount = CartridgesClip;
        }

        public override void SetDefault()
        {
            HasIt = false;
            Level = Levels.LV1;
            ApplyUpdate();
        }

        public override bool CanUpdate() => false;
    }

    public class Pistol : Gun
    {
        public Pistol() : base()
        {
            AddToShop = true;
            HasIt = true;
            HaveLV4 = true;
            Name = new[] { "Пистолет", "Pistol" };
            FireType = FireTypes.Single;
            UpdateCost = 20;
            AmmoCost = 5;
            RechargeTime = 600;
            CartridgesClip = 8;
            MaxAmmoCount = CartridgesClip * 3;
            MaxAmmo = CartridgesClip * 6;
            FiringRange = 7;
            MaxDamage = 1.75;
            MinDamage = 1.25;
            Recoil = 10;
            FiringRate = 175;
            BurstShots = 1;
            RadiusSound = 6;
            ReloadFrames = 1;
            Icon = new[]
            {
                    /*LV1:*/ Properties.Resources.gun_1_0_icon,
                    /*LV2:*/ Properties.Resources.gun_1_1_icon,
                    /*LV3:*/ Properties.Resources.gun_1_2_icon,
                    /*LV4:*/ Properties.Resources.gun_1_3_icon
            };
            Images = new[,]
            {                    
                   /*LV1:*/ { Properties.Resources.gun_1_0, Properties.Resources.gun_1_0_shooted, Properties.Resources.gun_1_0_reload, Properties.Resources.gun_1_0_reload, Properties.Resources.gun_1_0, Properties.Resources.gun_1_0_run, Properties.Resources.gun_1_0_run },
                   /*LV2:*/ { Properties.Resources.gun_1_1, Properties.Resources.gun_1_1_shooted, Properties.Resources.gun_1_1_reload, Properties.Resources.gun_1_1_reload_empty, Properties.Resources.gun_1_1_empty, Properties.Resources.gun_1_1_run, Properties.Resources.gun_1_1_run_empty },
                   /*LV3:*/ { Properties.Resources.gun_1_2, Properties.Resources.gun_1_2_shooted, Properties.Resources.gun_1_2_reload, Properties.Resources.gun_1_2_reload_empty, Properties.Resources.gun_1_2_empty, Properties.Resources.gun_1_2_run, Properties.Resources.gun_1_2_run_empty },
                   /*LV4:*/ { Properties.Resources.gun_1_3, Properties.Resources.gun_1_3_shooted, Properties.Resources.gun_1_3_1_reload, Properties.Resources.gun_1_3_2_reload, Properties.Resources.gun_1_3, Properties.Resources.gun_1_3_run, Properties.Resources.gun_1_3_run },
            };
            Sounds = new[,]
            {
                   /*LV1:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_0_1.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_0_2_reloading.wav"), false), new PlaySound(null, false) },
                   /*LV2:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_0_2.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_0_2_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_0_empty.wav"), false) },
                   /*LV3:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_0_4.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_0_2_reloading.wav"), false), new PlaySound(null, false) },
                   /*LV4:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_0_3.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_0_3_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_0_3_empty.wav"), false) },
            };
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
                RechargeTime = 600;
                CartridgesClip = 8;
                MaxAmmoCount = CartridgesClip * 3;
                MaxAmmo = CartridgesClip * 6;
                FiringRange = 7;
                MaxDamage = 1.75;
                MinDamage = 1.25;
                Recoil = 10;
                FiringRate = 175;
                BurstShots = 1;
                RadiusSound = 6;
                ReloadFrames = 1;
            }
            else if (Level == Levels.LV2)
            {
                RechargeTime = 350;
                CartridgesClip = 12;
                MaxAmmoCount = CartridgesClip * 3;
                MaxAmmo = CartridgesClip * 7;
                FiringRange = 8;
                MaxDamage = 2.75;
                MinDamage = 2.5;
                Recoil = 20;
                FiringRate = 175;
                BurstShots = 1;
                RadiusSound = 7;
                ReloadFrames = 1;
            }
            else if (Level == Levels.LV3)
            {
                RechargeTime = 400;
                CartridgesClip = 7;
                MaxAmmoCount = CartridgesClip * 2;
                MaxAmmo = CartridgesClip * 6;
                FiringRange = 9;
                MaxDamage = 3.45;
                MinDamage = 3;
                Recoil = 25;
                FiringRate = 175;
                BurstShots = 1;
                RadiusSound = 10;
                ReloadFrames = 1;
            }
            else
            {
                RechargeTime = 400;
                CartridgesClip = 6;
                MaxAmmoCount = CartridgesClip * 2;
                MaxAmmo = CartridgesClip * 6;
                FiringRange = 9;
                MaxDamage = 10.5;
                MinDamage = 5;
                Recoil = 35;
                FiringRate = 225;
                BurstShots = 1;
                RadiusSound = 10;
                ReloadFrames = 2;
            }
            MaxAmmoCount = CartridgesClip * 3;
            base.ApplyUpdate();
        }
    }

    public class Shotgun : Gun
    {
        public Shotgun() : base()
        {
            AddToShop = true;
            HasIt = false;
            HaveLV4 = false;
            Name = new[] { "Дробовик", "Shotgun" };
            FireType = FireTypes.Single;
            UpdateCost = 30;
            GunCost = 35;
            AmmoCost = 12;
            RechargeTime = 425;
            CartridgesClip = 2;
            MaxAmmoCount = CartridgesClip;
            MaxAmmo = CartridgesClip * 8;
            FiringRange = 4;
            MaxDamage = 3.5;
            MinDamage = 2.75;
            Recoil = 120;
            FiringRate = 200;
            BurstShots = 1;
            RadiusSound = 10;
            ReloadFrames = 2;
            Icon = new[]
            {
                    /*LV1:*/ Properties.Resources.gun_0_0_icon,
                    /*LV2:*/ Properties.Resources.gun_0_1_icon,
                    /*LV3:*/ Properties.Resources.gun_0_2_icon,
            };
            Images = new[,]
            {
                    /*LV1:*/ { Properties.Resources.gun_0_0, Properties.Resources.gun_0_0_shooted, Properties.Resources.gun_0_0_1_reload, Properties.Resources.gun_0_0_2_reload, Properties.Resources.gun_0_0_2_reload, Properties.Resources.gun_0_0_run },
                    /*LV2:*/ { Properties.Resources.gun_0_1, Properties.Resources.gun_0_1_shooted, Properties.Resources.gun_0_1_pump, Properties.Resources.gun_0_1_1_reload, Properties.Resources.gun_0_1_2_reload, Properties.Resources.gun_0_1_run },
                    /*LV3:*/ { Properties.Resources.gun_0_2, Properties.Resources.gun_0_2_shooted, Properties.Resources.gun_0_2_pump, Properties.Resources.gun_0_2_1_reload, Properties.Resources.gun_0_2_2_reload, Properties.Resources.gun_0_2_run },
            };
            Sounds = new[,]
            {
                    /*LV1:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_1.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_1_0_reloading.wav"), false), new PlaySound(null, false), new PlaySound(null, false) },
                    /*LV2:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_1.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_1_1_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_1_empty.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_1_shell.wav"), false) },
                    /*LV3:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_1.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_1_2_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_1_empty.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_1_shell.wav"), false) },
            };
            AmmoCount = CartridgesClip;
        }

        protected override void ApplyUpdate()
        {
            if (Level == Levels.LV1)
            {
                UpdateCost = 30;
                RechargeTime = 425;
                CartridgesClip = 2;
                MaxAmmoCount = CartridgesClip;
                MaxAmmo = CartridgesClip * 8;
                FiringRange = 7;
                MaxDamage = 3.5;
                MinDamage = 2.75;
                Recoil = 120;
                FiringRate = 200;
                BurstShots = 1;
                RadiusSound = 10;
                ReloadFrames = 2;
            }
            else if (Level == Levels.LV2)
            {
                RechargeTime = 425;
                CartridgesClip = 6;
                MaxAmmoCount = CartridgesClip * 2;
                MaxAmmo = CartridgesClip * 7;
                FiringRange = 6;
                MaxDamage = 4.75;
                MinDamage = 3.25;
                Recoil = 80;
                FiringRate = 200;
                BurstShots = 1;
                RadiusSound = 10;
                ReloadFrames = 3;
            }
            else
            {
                RechargeTime = 425;
                CartridgesClip = 14;
                MaxAmmoCount = CartridgesClip * 2;
                MaxAmmo = CartridgesClip * 4;
                FiringRange = 5;
                MaxDamage = 6.25;
                MinDamage = 5.25;
                Recoil = 135;
                FiringRate = 200;
                BurstShots = 1;
                RadiusSound = 10;
                ReloadFrames = 3;
            }
            MaxAmmoCount = CartridgesClip * 2;
            base.ApplyUpdate();
        }
    }

    public class SubmachineGun : Gun
    {
        public SubmachineGun() : base()
        {
            AddToShop = true;
            HasIt = false;
            HaveLV4 = false;
            Name = new[] { "Пистолет-пулемет", "Submachine gun" };
            FireType = FireTypes.SemiAutomatic;
            UpdateCost = 40;
            GunCost = 30;
            AmmoCost = 18;
            RechargeTime = 375;
            CartridgesClip = 18;
            MaxAmmoCount = CartridgesClip * 3;
            MaxAmmo = CartridgesClip * 3;
            FiringRange = 8;
            MaxDamage = 2;
            MinDamage = 1.5;
            Recoil = 35;
            FiringRate = 50;
            BurstShots = 6;
            RadiusSound = 6;
            ReloadFrames = 2;
            Icon = new[]
            {
                    /*LV1:*/ Properties.Resources.gun_3_1_icon,
                    /*LV2:*/ Properties.Resources.gun_3_2_icon,
                    /*LV3:*/ Properties.Resources.gun_3_3_icon,
            };
            Images = new[,]
            {
                    /*LV1:*/ { Properties.Resources.gun_3_1, Properties.Resources.gun_3_1_shooted, Properties.Resources.gun_3_1_1_reload, Properties.Resources.gun_3_1_2_reload, Properties.Resources.gun_3_1_run},
                    /*LV2:*/ { Properties.Resources.gun_3_2, Properties.Resources.gun_3_2_shooted, Properties.Resources.gun_3_2_1_reload, Properties.Resources.gun_3_2_2_reload, Properties.Resources.gun_3_2_run },
                    /*LV3:*/ { Properties.Resources.gun_3_3, Properties.Resources.gun_3_3_shooted, Properties.Resources.gun_3_3_1_reload, Properties.Resources.gun_3_3_2_reload, Properties.Resources.gun_3_3_run }
            };
            Sounds = new[,]
            {
                    /*LV1:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_3_1.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_3_1_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_3_empty.wav"), false) },
                    /*LV2:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_3_2.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_3_2_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_3_empty.wav"), false) },
                    /*LV3:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_3_3.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_3_3_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_3_empty.wav"), false) }
            };
            AmmoCount = CartridgesClip;
        }

        protected override void ApplyUpdate()
        {
            if (Level == Levels.LV1)
            {
                UpdateCost = 40;
                RechargeTime = 375;
                CartridgesClip = 18;
                MaxAmmoCount = CartridgesClip * 3;
                MaxAmmo = CartridgesClip * 6;
                FiringRange = 8;
                MaxDamage = 2;
                MinDamage = 1.5;
                Recoil = 35;
                FiringRate = 50;
                BurstShots = 6;
                RadiusSound = 6;
                ReloadFrames = 2;
            }
            else if (Level == Levels.LV2)
            {
                RechargeTime = 350;
                CartridgesClip = 30;
                MaxAmmoCount = CartridgesClip * 3;
                MaxAmmo = CartridgesClip * 6;
                FiringRange = 8;
                MaxDamage = 2.5;
                MinDamage = 1.75;
                Recoil = 15;
                FiringRate = 50;
                BurstShots = 3;
                RadiusSound = 6;
                ReloadFrames = 2;
            }
            else
            {
                RechargeTime = 350;
                CartridgesClip = 30;
                MaxAmmoCount = CartridgesClip * 3;
                MaxAmmo = CartridgesClip * 6;
                FiringRange = 8;
                MaxDamage = 2.7;
                MinDamage = 1.95;
                Recoil = 25;
                FiringRate = 25;
                BurstShots = 6;
                RadiusSound = 6;
                ReloadFrames = 2;
            }
            MaxAmmoCount = CartridgesClip * 1;
            base.ApplyUpdate();
        }
    }

    public class AssaultRifle : Gun
    {
        public AssaultRifle() : base()
        {
            AddToShop = true;
            HasIt = false;
            HaveLV4 = false;
            Name = new[] { "Автомат", "Assault rifle" };
            FireType = FireTypes.SemiAutomatic;
            UpdateCost = 50;
            GunCost = 45;
            AmmoCost = 25;
            RechargeTime = 700;
            CartridgesClip = 30;
            MaxAmmoCount = CartridgesClip * 2;
            MaxAmmo = CartridgesClip * 4;
            FiringRange = 8;
            MaxDamage = 3.25;
            MinDamage = 2.75;
            Recoil = 30;
            FiringRate = 100;
            BurstShots = 3;
            RadiusSound = 13;
            ReloadFrames = 2;
            Icon = new[]
            {
                    /*LV1:*/ Properties.Resources.gun_4_1_icon,
                    /*LV2:*/ Properties.Resources.gun_4_2_icon,
                    /*LV3:*/ Properties.Resources.gun_4_3_icon,
            };
            Images = new[,]
            {
                    /*LV1:*/ { Properties.Resources.gun_4_1, Properties.Resources.gun_4_1_shooted, Properties.Resources.gun_4_1_1_reload, Properties.Resources.gun_4_1_2_reload, Properties.Resources.gun_4_1_run },
                    /*LV2:*/ { Properties.Resources.gun_4_2, Properties.Resources.gun_4_2_shooted, Properties.Resources.gun_4_2_1_reload, Properties.Resources.gun_4_2_2_reload, Properties.Resources.gun_4_2_run },
                    /*LV3:*/ { Properties.Resources.gun_4_3, Properties.Resources.gun_4_3_shooted, Properties.Resources.gun_4_3_1_reload, Properties.Resources.gun_4_3_2_reload, Properties.Resources.gun_4_3_run }
            };
            Sounds = new[,]
            {
                    /*LV1:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_4.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_4_1_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_4_empty.wav"), false) },
                    /*LV2:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_4.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_4_1_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_4_empty.wav"), false) },
                    /*LV3:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_4_3.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_4_3_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_4_empty.wav"), false) }
            };
            AmmoCount = CartridgesClip;
        }

        protected override void ApplyUpdate()
        {
            if (Level == Levels.LV1)
            {
                UpdateCost = 50;
                FireType = FireTypes.SemiAutomatic;
                RechargeTime = 700;
                CartridgesClip = 30;
                MaxAmmoCount = CartridgesClip * 3;
                MaxAmmo = CartridgesClip * 5;
                FiringRange = 8;
                MaxDamage = 2.5;
                MinDamage = 2;
                Recoil = 30;
                FiringRate = 100;
                BurstShots = 3;
                RadiusSound = 13;
                ReloadFrames = 2;
            }
            else if (Level == Levels.LV2)
            {
                FireType = FireTypes.SemiAutomatic;
                RechargeTime = 450;
                CartridgesClip = 30;
                MaxAmmoCount = CartridgesClip * 3;
                MaxAmmo = CartridgesClip * 5;
                FiringRange = 8;
                MaxDamage = 3.25;
                MinDamage = 2.75;
                Recoil = 25;
                FiringRate = 100;
                BurstShots = 3;
                RadiusSound = 13;
                ReloadFrames = 2;
            }
            else
            {
                FireType = FireTypes.Single;
                RechargeTime = 400;
                CartridgesClip = 20;
                MaxAmmoCount = CartridgesClip * 2;
                MaxAmmo = CartridgesClip * 4;
                FiringRange = 8;
                MaxDamage = 5.25;
                MinDamage = 4.5;
                Recoil = 40;
                FiringRate = 175;
                BurstShots = 1;
                RadiusSound = 13;
                ReloadFrames = 2;
            }
            base.ApplyUpdate();
        }
    }

    public class SniperRifle : Gun
    {
        public SniperRifle() : base()
        {
            ShowScope = false;
            AddToShop = true;
            HasIt = false;
            CanAiming = true;
            Name = new[] { "Снайперка", "Sniper rifle" };
            FireType = FireTypes.Single;
            GunCost = 55;
            AmmoCost = 30;
            RechargeTime = 850;
            CartridgesClip = 2;
            MaxAmmoCount = 2;
            MaxAmmo = CartridgesClip * 4;
            FiringRange = 21;
            MaxDamage = 23;
            MinDamage = 17;
            Recoil = 150;
            FiringRate = 200;
            BurstShots = 1;
            RadiusSound = 20;
            ReloadFrames = 1;
            Icon = new[]
            {
                    Properties.Resources.gun_2_icon
            };
            Images = new[,]
            {
                    { Properties.Resources.gun_2, Properties.Resources.gun_2_shooted, Properties.Resources.gun_2_reload, Properties.Resources.gun_2_aiming, Properties.Resources.gun_2_run }
            };
            Sounds = new[,]
            {
                    { new PlaySound(MainMenu.CGFReader.GetFile("gun_2.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_2_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_2_empty.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_2_aiming.wav"), false) }
            };
            AmmoCount = CartridgesClip;
        }

        public override bool CanUpdate() => false;
    }

    public class Fingershot : Gun
    {
        public Fingershot() : base()
        {
            AddToShop = false;
            HasIt = false;
            Name = new[] { "Пальцестрел", "Fingershot" };
            FireType = FireTypes.Single;
            RechargeTime = 600;
            CartridgesClip = 1;
            MaxAmmoCount = 99;
            MaxAmmo = 99;
            FiringRange = 10;
            MaxDamage = 25;
            MinDamage = 20;
            Recoil = 350;
            FiringRate = 175;
            BurstShots = 1;
            RadiusSound = 0;
            ReloadFrames = 2;
            Icon = new[] { Properties.Resources.missing };
            Images = new[,]
            {
                   { Properties.Resources.gun_5, Properties.Resources.gun_5_shooted, Properties.Resources.gun_5_1_reload, Properties.Resources.gun_5_2_reload, Properties.Resources.gun_5_run, Properties.Resources.gun_5_run }
            };
            Sounds = new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("gun_5.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_5_reloading.wav"), false), new PlaySound(null, false) }
            };
            AmmoCount = CartridgesClip;
        }

        public override bool CanUpdate() => false;
    }

    public class TSPitW : Gun
    {
        public TSPitW() : base()
        {
            AddToShop = false;
            HasIt = false;
            Name = new[] { "СМПвМ", "TSPitW" };
            FireType = FireTypes.Single;
            RechargeTime = 750;
            CartridgesClip = 7;
            MaxAmmoCount = CartridgesClip * 4;
            MaxAmmo = CartridgesClip * 4;
            FiringRange = 16;
            MaxDamage = 55;
            MinDamage = 50;
            Recoil = 350;
            FiringRate = 175;
            BurstShots = 1;
            RadiusSound = 0;
            ReloadFrames = 3;
            Icon = new[] { Properties.Resources.missing };
            Images = new[,]
            {
                   { Properties.Resources.gun_6, Properties.Resources.gun_6_shooted, Properties.Resources.gun_6_1_reload, Properties.Resources.gun_6_2_reload, Properties.Resources.gun_6_3_reload, Properties.Resources.gun_6 }
            };
            Sounds = new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("gun_6.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_6_reloading.wav"), false), new PlaySound(null, false) }
            };
            AmmoCount = CartridgesClip;
        }

        public override bool CanUpdate() => false;
    }

    public class Gnome : Magic
    {
        public Gnome() : base()
        {
            Name = new[] { "Гном-волшебник", "Wizard Gnome" };
            RechargeTime = 300;
            MaxAmmoCount = 99;
            MaxAmmo = 99;
            MaxDamage = 25;
            MinDamage = 20;
            FiringRate = 650;
            ReloadFrames = 4;
            Images = new[,]
            {
                   { Properties.Resources.gun_gnome, Properties.Resources.gun_gnome_shooted,
                    Properties.Resources.gun_gnome_reloading_0, Properties.Resources.gun_gnome_reloading_1,
                    Properties.Resources.gun_gnome_reloading_2, Properties.Resources.gun_gnome_reloading_3,
                    Properties.Resources.gun_gnome_run }
            };
            Sounds = new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("fireball.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gnome_reloading.wav"), false), new PlaySound(null, false) }
            };
        }
    }

    public class FirstAidKit : DisposableItem
    {
        public FirstAidKit() : base()
        {
            HasLVMechanics = true;
            HasCuteDescription = true;
            GunCost = 50;
            ItemIcon = Properties.Resources.first_aid;
            CuteItemIcon = Properties.Resources.food_count;
            Name = new[]
            { 
                "Аптечка", "First Aid Kit",
                "Бобы", "Beans"
            };
            Description = new[]
            { 
                "Восстанавливает здоровье",
                "Restores health",
                "Вкусный перекус",
                "A tasty snack"
            };
            Icon = new[]
            {
                Properties.Resources.medkit_icon,
                Properties.Resources.food_icon
            };
            Images = new[,]
            {
                   { Properties.Resources.medkit, Properties.Resources.medkit, Properties.Resources.medkit_using_0, Properties.Resources.medkit_using_1, Properties.Resources.medkit_using_2, Properties.Resources.medkit_run },
                   { Properties.Resources.syringe, Properties.Resources.syringe, Properties.Resources.syringe_using_0, Properties.Resources.syringe_using_1, Properties.Resources.syringe_using_2, Properties.Resources.medkit_run },
                   { Properties.Resources.hand, Properties.Resources.hand, Properties.Resources.hand_using_0, Properties.Resources.hand_using_1, Properties.Resources.hand_using_2, Properties.Resources.medkit_run },
                   { Properties.Resources.food, Properties.Resources.food, Properties.Resources.food_using_0, Properties.Resources.food_using_1, Properties.Resources.food_using_2, Properties.Resources.medkit_run },
            };
            Sounds = new[,]
            {
                   { new PlaySound(null, false), new PlaySound(MainMenu.CGFReader.GetFile("medkit_using.wav"), false), new PlaySound(null, false) },
                   { new PlaySound(null, false), new PlaySound(MainMenu.CGFReader.GetFile("syringe_using.wav"), false), new PlaySound(null, false) },
                   { new PlaySound(null, false), new PlaySound(MainMenu.CGFReader.GetFile("hand_using.wav"), false), new PlaySound(null, false) },
                   { new PlaySound(null, false), new PlaySound(MainMenu.CGFReader.GetFile("food_using.wav"), false), new PlaySound(null, false) }
            };
        }
    }

    public class Adrenalin : DisposableItem
    {
        public Adrenalin() : base()
        {
            RechargeTime = 530;
            GunCost = 50;
            ItemIcon = Properties.Resources.adrenalin_count_icon;
            CuteItemIcon = Properties.Resources.adrenalin_count_icon;
            Name = new[]
            {
                "Адреналин", "Adrenalin"
            };
            Description = new[]
            {
                "Увеличивает скорость передвижения на 30 сек",
                "Increases movement speed for 30 sec",
            };
            Icon = new[]
            {
                Properties.Resources.adrenalin_icon,
                Properties.Resources.adrenalin_icon
            };
            Images = new[,]
            {
                   { Properties.Resources.adrenalin, Properties.Resources.adrenalin, Properties.Resources.adrenalin_using_0, Properties.Resources.adrenalin_using_1, Properties.Resources.adrenalin_using_2, Properties.Resources.medkit_run },
            };
            Sounds = new[,]
            {
                   { new PlaySound(null, false), new PlaySound(MainMenu.CGFReader.GetFile("adrenalin_using.wav"), false), new PlaySound(null, false) }
            };
        }
    }
}