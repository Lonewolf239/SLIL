using LiteNetLib.Utils;

namespace GameServer
{
    public enum Directions { STOP, FORWARD, BACK, LEFT, RIGHT, WALK, RUN };

    public class Player : Entity
    {
        public int DeathSound { get; set; }
        public string? Name { get; set; }
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
        public bool InParkour { get; set; }
        public int ParkourState { get; set; }
        public int Stage { get; set; }
        public bool CuteMode { get; set; }
        public int EnemiesKilled { get; set; }
        public double MAX_MOVE_SPEED { get; set; }
        public double MAX_STRAFE_SPEED { get; set; }
        public double MOVE_SPEED { get; set; }
        public double STRAFE_SPEED { get; set; }
        public double RUN_SPEED { get; set; }
        public double DEPTH { get; set; }
        public int SelectedItem { get; set; }
        public bool Fast { get; set; }
        public bool NoClip { get; set; }
        public bool OnBike { get; set; }
        public bool InSelectingMode { get; set; }
        public bool BlockInput { get; set; }
        public bool BlockCamera { get; set; }
        public bool CanUnblockCamera { get; set; }
        public Directions PlayerDirection { get; set; }
        public Directions StrafeDirection { get; set; }
        public Directions PlayerMoveStyle { get; set; }
        public List<Effect> Effects = [];
        public readonly Gun[] GUNS =
        [
            new Flashlight(), new Knife(), new Pistol(),
            new Shotgun(), new SubmachineGun(), new AssaultRifle(),
            new SniperRifle(), new Fingershot(), new TSPitW(),
            new Gnome(), new FirstAidKit(), new Candy(),
            new Rainblower(), new Adrenalin(), new Helmet(),
            new RPG()
        ];
        public List<Gun> Guns = [];
        public List<DisposableItem> DisposableItems = [];
        public Pet? PET = null;
        public double MAX_HP { get; set; }
        public double MAX_STAMINE { get; set; }

        public Player(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => InitPlayer();
        public Player(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => InitPlayer();

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.Put(HP);
            writer.Put(Dead);
            writer.Put(Money);
            writer.Put(CurrentGun);
            writer.Put(A);
            writer.Put(Look);
            writer.Put(InParkour);
            writer.Put(OnBike);
            writer.Put(BlockCamera);
            writer.Put(BlockInput);
            writer.Put(this.GUNS.Length);
            foreach(Gun gun in this.GUNS)
                writer.Put(gun.HasIt);
            writer.Put(Guns.Count);
            foreach(Gun gun in this.Guns)
            {
                writer.Put(gun.ItemID);
                gun.Serialize(writer);
            }
            writer.Put(DisposableItems.Count);
            foreach(DisposableItem item in this.DisposableItems)
            {
                writer.Put(item.ItemID);
                item.Serialize(writer);
            }
        }

        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
            this.HP = reader.GetDouble();
            this.Dead = reader.GetBool();
            this.Money = reader.GetInt();
            this.CurrentGun = reader.GetInt();
            this.A = reader.GetDouble();
            this.Look = reader.GetDouble();
            this.InParkour = reader.GetBool();
            this.OnBike = reader.GetBool();
            this.BlockCamera = reader.GetBool();
            this.BlockInput = reader.GetBool();
            int GUNSLength = reader.GetInt();
            for(int i = 0; i < GUNSLength; i++)
                this.GUNS[i].HasIt = reader.GetBool();
            int GunsCount = reader.GetInt();
            List<Gun> tempGuns = [];
            for(int i = 0; i< GunsCount; i++)
            {
                int gunID = reader.GetInt();
                switch (gunID)
                {
                    case 0:
                        Flashlight flashlight = new();
                        flashlight.Deserialize(reader);
                        tempGuns.Add(flashlight);
                        break;
                    case 1:
                        Knife knife = new();
                        knife.Deserialize(reader);
                        tempGuns.Add(knife);
                        break;
                    case 2:
                        Candy candy = new();
                        candy.Deserialize(reader);
                        tempGuns.Add(candy);
                        break;
                    case 3:
                        Rainblower rainblower = new();
                        rainblower.Deserialize(reader);
                        tempGuns.Add(rainblower);
                        break;
                    case 4:
                        Pistol pistol = new();
                        pistol.Deserialize(reader);
                        tempGuns.Add(pistol);
                        break;
                    case 5:
                        Shotgun shotgun = new();
                        shotgun.Deserialize(reader);
                        tempGuns.Add(shotgun);
                        break;
                    case 6:
                        SubmachineGun submachineGun = new();
                        submachineGun.Deserialize(reader);
                        tempGuns.Add(submachineGun);
                        break;
                    case 7:
                        AssaultRifle assaultRifle = new();
                        assaultRifle.Deserialize(reader);
                        tempGuns.Add(assaultRifle);
                        break;
                    case 8:
                        SniperRifle sniperRifle = new();
                        sniperRifle.Deserialize(reader);
                        tempGuns.Add(sniperRifle);
                        break;
                    case 9:
                        Fingershot fingershot = new();
                        fingershot.Deserialize(reader);
                        tempGuns.Add(fingershot);
                        break;
                    case 10:
                        TSPitW tSPitW = new();
                        tSPitW.Deserialize(reader);
                        tempGuns.Add(tSPitW);
                        break;
                    case 11:
                        Gnome gnome = new();
                        gnome.Deserialize(reader);
                        tempGuns.Add(gnome);
                        break;
                    case 12:
                        FirstAidKit firstAidKit = new();
                        firstAidKit.Deserialize(reader);
                        tempGuns.Add(firstAidKit);
                        break;
                    case 13:
                        Adrenalin adrenalin = new();
                        adrenalin.Deserialize(reader);
                        tempGuns.Add(adrenalin);
                        break;
                    case 14:
                        Helmet helmet = new();
                        helmet.Deserialize(reader);
                        tempGuns.Add(helmet);
                        break;
                    case 15:
                        RPG rpg = new();
                        rpg.Deserialize(reader);
                        tempGuns.Add(rpg);
                        break;
                    default:
                        break;
                }
            }
            int disposableItemsCount = reader.GetInt();
            List<DisposableItem> tempDisposableItems = [];
            for(int i = 0; i < disposableItemsCount; i++)
            {
                int itemID = reader.GetInt();
                switch (itemID)
                {
                    case 12:
                        FirstAidKit firstAidKit = new();
                        firstAidKit.Deserialize(reader);
                        tempDisposableItems.Add(firstAidKit);
                        break;
                    case 13:
                        Adrenalin adrenalin = new();
                        adrenalin.Deserialize(reader);
                        tempDisposableItems.Add(adrenalin);
                        break;
                    case 14:
                        Helmet helmet = new();
                        helmet.Deserialize(reader);
                        tempDisposableItems.Add(helmet);
                        break;
                    default:
                        break;
                }
            }
            Guns = tempGuns;
            DisposableItems = tempDisposableItems;
        }

        public void Deserialize(NetDataReader reader, bool updateCoordinates)
        {
            if (!updateCoordinates)
            {
                reader.GetDouble(); reader.GetDouble();
                this.HP = reader.GetDouble();
                this.Dead = reader.GetBool();
                this.Money = reader.GetInt();
                this.CurrentGun = reader.GetInt();
                reader.GetDouble(); reader.GetDouble();
                this.InParkour = reader.GetBool();
                this.OnBike = reader.GetBool();
                this.BlockCamera = reader.GetBool();
                this.BlockInput = reader.GetBool();
                int GUNSLength = reader.GetInt();
                for(int i = 0; i < GUNSLength; i++)
                    this.GUNS[i].HasIt = reader.GetBool();
                int GunsCount = reader.GetInt();
                List<Gun> tempGuns = [];
                for(int i = 0; i< GunsCount; i++)
                {
                    int gunID = reader.GetInt();
                    switch (gunID)
                    {
                        case 0:
                            Flashlight flashlight = new();
                            flashlight.Deserialize(reader);
                            tempGuns.Add(flashlight);
                            break;
                        case 1:
                            Knife knife = new();
                            knife.Deserialize(reader);
                            tempGuns.Add(knife);
                            break;
                        case 2:
                            Candy candy = new();
                            candy.Deserialize(reader);
                            tempGuns.Add(candy);
                            break;
                        case 3:
                            Rainblower rainblower = new();
                            rainblower.Deserialize(reader);
                            tempGuns.Add(rainblower);
                            break;
                        case 4:
                            Pistol pistol = new();
                            pistol.Deserialize(reader);
                            tempGuns.Add(pistol);
                            break;
                        case 5:
                            Shotgun shotgun = new();
                            shotgun.Deserialize(reader);
                            tempGuns.Add(shotgun);
                            break;
                        case 6:
                            SubmachineGun submachineGun = new();
                            submachineGun.Deserialize(reader);
                            tempGuns.Add(submachineGun);
                            break;
                        case 7:
                            AssaultRifle assaultRifle = new();
                            assaultRifle.Deserialize(reader);
                            tempGuns.Add(assaultRifle);
                            break;
                        case 8:
                            SniperRifle sniperRifle = new();
                            sniperRifle.Deserialize(reader);
                            tempGuns.Add(sniperRifle);
                            break;
                        case 9:
                            Fingershot fingershot = new();
                            fingershot.Deserialize(reader);
                            tempGuns.Add(fingershot);
                            break;
                        case 10:
                            TSPitW tSPitW = new();
                            tSPitW.Deserialize(reader);
                            tempGuns.Add(tSPitW);
                            break;
                        case 11:
                            Gnome gnome = new();
                            gnome.Deserialize(reader);
                            tempGuns.Add(gnome);
                            break;
                        case 12:
                            FirstAidKit firstAidKit = new();
                            firstAidKit.Deserialize(reader);
                            tempGuns.Add(firstAidKit);
                            break;
                        case 13:
                            Adrenalin adrenalin = new();
                            adrenalin.Deserialize(reader);
                            tempGuns.Add(adrenalin);
                            break;
                        case 14:
                            Helmet helmet = new();
                            helmet.Deserialize(reader);
                            tempGuns.Add(helmet);
                            break;
                        case 15:
                            RPG rpg = new();
                            rpg.Deserialize(reader);
                            tempGuns.Add(rpg);
                            break;
                        default:
                            break;
                    }
                }
                int disposableItemsCount = reader.GetInt();
                List<DisposableItem> tempDisposableItems = [];
                for(int i = 0; i < disposableItemsCount; i++)
                {
                    int itemID = reader.GetInt();
                    switch (itemID)
                    {
                        case 12:
                            FirstAidKit firstAidKit = new();
                            firstAidKit.Deserialize(reader);
                            tempDisposableItems.Add(firstAidKit);
                            break;
                        case 13:
                            Adrenalin adrenalin = new();
                            adrenalin.Deserialize(reader);
                            tempDisposableItems.Add(adrenalin);
                            break;
                        case 14:
                            Helmet helmet = new();
                            helmet.Deserialize(reader);
                            tempDisposableItems.Add(helmet);
                            break;
                        default:
                            break;
                    }
                }
                Guns = tempGuns;
                DisposableItems = tempDisposableItems;
            }
            else
                base.Deserialize(reader);
        }

        private void InitPlayer()
        {
            DeathSound = 5;
            DisposableItems.Add((FirstAidKit)GUNS[10]);
            DisposableItems.Add((Adrenalin)GUNS[13]);
            DisposableItems.Add((Helmet)GUNS[14]);
            Texture = 38;
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
                MAX_MOVE_SPEED = 1.8;
                MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 1.4;
                RUN_SPEED = 2.25;
                DEPTH = 8;
                SelectedItem = 0;
                PET = null;
                CuteMode = false;
                Fast = false;
                NoClip = false;
                if (OnBike) StopEffect(4);
            }
            EnemiesKilled = 0;
            Look = 0;
            GunState = 0;
            MOVE_SPEED = 0;
            STRAFE_SPEED = 0;
            CanShoot = true;
            Dead = false;
            Invulnerable = false;
            TimeoutInvulnerable = 2;
            Aiming = false;
            UseItem = false;
            LevelUpdated = false;
            IsPetting = false;
            InParkour = false;
            InSelectingMode = false;
            BlockInput = false;
            if (!OnBike)
            {
                BlockCamera = false;
                CanUnblockCamera = true;
            }
            PlayerDirection = Directions.STOP;
            StrafeDirection = Directions.STOP;
            PlayerMoveStyle = Directions.WALK;
            ParkourState = 0;
            PreviousGun = CurrentGun = 1;
            STAMINE = MAX_STAMINE;
            if (Guns.Count == 0)
            {
                Guns.Add(GUNS[1]);
                Guns.Add(GUNS[2]);
            }
        }

        public double GetDrawDistance()
        {
            if (OnBike) return DEPTH + 2;
            return DEPTH + (Aiming || GetCurrentGun() is Flashlight ? GetCurrentGun().AimingFactor : 0);
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
                        MAX_MOVE_SPEED -= 1.5;
                        MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 1.4;
                        MOVE_SPEED = 0;
                        STRAFE_SPEED = 0;
                    }
                    Effects.RemoveAt(i);
                }
            }
        }

        public void GiveEffect(int index, bool standart_time, int time = 0, bool infinity = false)
        {
            UseItem = false;
            if (index == 0)
            {
                if (EffectCheck(0)) return;
                Regeneration effect = new();
                if (!standart_time)
                    effect.SetTotalTime(time);
                effect.Infinity = infinity;
                effect.UpdateTimeRemaining();
                Effects.Add(effect);
            }
            else if (index == 1)
            {
                if (EffectCheck(1) || EffectCheck(4)) return;
                Adrenaline effect = new();
                if (!standart_time)
                    effect.SetTotalTime(time);
                effect.Infinity = infinity;
                effect.UpdateTimeRemaining();
                Effects.Add(effect);
                Fast = true;
                MAX_MOVE_SPEED += 1.5;
                MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 1.4;
                MOVE_SPEED = 0;
                STRAFE_SPEED = 0;
            }
            else if (index == 2)
            {
                if (EffectCheck(2)) return;
                Protection effect = new();
                if (!standart_time)
                    effect.SetTotalTime(time);
                effect.Infinity = infinity;
                effect.UpdateTimeRemaining();
                Effects.Add(effect);
            }
            else if (index == 3)
            {
                if (EffectCheck(3)) return;
                Fatigue effect = new();
                if (!standart_time)
                    effect.SetTotalTime(time);
                effect.Infinity = infinity;
                effect.UpdateTimeRemaining();
                Effects.Add(effect);
            }
            else if (index == 4)
            {
                if (EffectCheck(4)) return;
                StopEffect(0);
                StopEffect(1);
                Biker effect = new();
                effect.UpdateTimeRemaining();
                Effects.Add(effect);
                CanUnblockCamera = false;
                BlockCamera = true;
                CanShoot = false;
                Look = 0;
                OnBike = true;
                Fast = true;
                MAX_MOVE_SPEED += 2.15;
                MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 1.4;
                MOVE_SPEED = 0;
                STRAFE_SPEED = 0;
            }
        }

        public int GetEffectID() => DisposableItems[SelectedItem].EffectID;

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
                if (EffectCheck(1) || EffectCheck(4)) return;
                Effects.Add(new Adrenaline());
                Fast = true;
                MAX_MOVE_SPEED += 1.5;
                MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 1.4;
                MOVE_SPEED = 0;
                STRAFE_SPEED = 0;
            }
            else if (SelectedItem == 2)
            {
                if (EffectCheck(2)) return;
                Effects.Add(new Protection());
            }
        }

        public void StopEffect(int id)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                if (Effects[i].ID == id)
                {
                    if (Effects[i].ID == 1)
                    {
                        Fast = false;
                        MAX_MOVE_SPEED -= 1.5;
                        MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 1.4;
                        MOVE_SPEED = 0;
                        STRAFE_SPEED = 0;
                    }
                    else if (Effects[i].ID == 4)
                    {
                        BlockCamera = false;
                        CanShoot = true;
                        OnBike = false;
                        Fast = false;
                        MAX_MOVE_SPEED -= 2.15;
                        MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 1.4;
                        MOVE_SPEED = 0;
                        STRAFE_SPEED = 0;
                        HP = MAX_HP;
                    }
                    Effects.RemoveAt(i);
                    break;
                }
            }
        }

        public void StopEffects()
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                if (Effects[i].ID == 1)
                {
                    Fast = false;
                    MAX_MOVE_SPEED -= 1.5;
                    MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 1.4;
                    MOVE_SPEED = 0;
                    STRAFE_SPEED = 0;
                }
                else if (Effects[i].ID == 4)
                {
                    BlockCamera = false;
                    CanShoot = true;
                    OnBike = false;
                    Fast = false;
                    MAX_MOVE_SPEED -= 2.15;
                    MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 1.4;
                    MOVE_SPEED = 0;
                    STRAFE_SPEED = 0;
                    HP = MAX_HP;
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
            if (EffectCheck(2) || EffectCheck(4)) damage *= 0.8;
            HP -= damage;
            TimeoutInvulnerable = 2;
            Invulnerable = true;
            if (HP <= 0)
            {
                if (OnBike) StopEffect(4);
                else this.Dead = true;
            }
            return Dead;
        }
    }
}