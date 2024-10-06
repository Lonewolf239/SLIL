using LiteNetLib.Utils;

namespace GameServer
{
    public enum Directions { STOP, FORWARD, BACK, LEFT, RIGHT, WALK, RUN };

    public class Player : Entity
    {
        public int DeathSound { get; set; }
        public string? Name { get; set; }
        public double Look { get; set; }
        public double HP { get; set; }
        public double TRANSPORT_HP { get; set; }
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
        public double MAX_RUN_SPEED { get; set; }
        public double RUN_SPEED { get; set; }
        public double DEPTH { get; set; }
        public int SelectedItem { get; set; }
        public bool Fast { get; set; }
        public bool NoClip { get; set; }
        public bool InTransport { get; set; }
        public bool InSelectingMode { get; set; }
        public bool BlockInput { get; set; }
        public bool BlockCamera { get; set; }
        public bool BlockMouse { get; set; }
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
            new RPG(), new Petition()
        ];
        public List<Gun> Guns = [];
        public List<DisposableItem> DisposableItems = [];
        public DisposableItem? DISPOSABLE_ITEM = null;
        public int ItemFrame { get; set; }
        public Pet? PET = null;
        public Transport? TRANSPORT = null;
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
            writer.Put(InTransport);
            writer.Put(BlockCamera);
            writer.Put(BlockInput);
            writer.Put(SelectedItem);
            writer.Put(GUNS.Length);
            foreach (Gun gun in GUNS)
                writer.Put(gun.HasIt);
            writer.Put(Guns.Count);
            foreach (Gun gun in Guns)
            {
                writer.Put(gun.ItemID);
                gun.Serialize(writer);
            }
            writer.Put(DisposableItems.Count);
            foreach (DisposableItem item in DisposableItems)
            {
                writer.Put(item.ItemID);
                item.Serialize(writer);
            }
            writer.Put(Effects.Count);
            foreach (Effect effect in Effects)
            {
                writer.Put(effect.ID);
                effect.Serialize(writer);
            }
        }

        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
            HP = reader.GetDouble();
            Dead = reader.GetBool();
            Money = reader.GetInt();
            CurrentGun = reader.GetInt();
            A = reader.GetDouble();
            Look = reader.GetDouble();
            InParkour = reader.GetBool();
            InTransport = reader.GetBool();
            BlockCamera = reader.GetBool();
            BlockInput = reader.GetBool();
            SelectedItem = reader.GetInt();
            int GUNSLength = reader.GetInt();
            for (int i = 0; i < GUNSLength; i++)
                GUNS[i].HasIt = reader.GetBool();
            int GunsCount = reader.GetInt();
            List<Gun> tempGuns = [];
            for (int i = 0; i < GunsCount; i++)
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
            for (int i = 0; i < disposableItemsCount; i++)
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
            int effectsCount = reader.GetInt();
            List<Effect> tempEffects = [];
            for (int i = 0; i < effectsCount; i++)
            {
                switch (reader.GetInt())
                {
                    case 0:
                        Regeneration regeneration = new();
                        regeneration.Deserialize(reader);
                        tempEffects.Add(regeneration);
                        break;
                    case 1:
                        Adrenaline adrenaline = new();
                        adrenaline.Deserialize(reader);
                        tempEffects.Add(adrenaline);
                        break;
                    case 2:
                        Protection protection = new();
                        protection.Deserialize(reader);
                        tempEffects.Add(protection);
                        break;
                    case 3:
                        Fatigue fatigue = new();
                        fatigue.Deserialize(reader);
                        tempEffects.Add(fatigue);
                        break;
                    case 4:
                        Rider rider = new();
                        rider.Deserialize(reader);
                        tempEffects.Add(rider);
                        break;
                    case 5:
                        Bleeding bleeding = new();
                        bleeding.Deserialize(reader);
                        tempEffects.Add(bleeding);
                        break;
                    case 6:
                        Blindness blindness = new();
                        blindness.Deserialize(reader);
                        tempEffects.Add(blindness);
                        break;
                    default:
                        break;
                }
            }
            Guns = tempGuns;
            DisposableItems = tempDisposableItems;
            Effects = tempEffects;
        }

        public void Deserialize(NetDataReader reader, bool updateCoordinates)
        {
            if (!updateCoordinates)
            {
                reader.GetDouble(); reader.GetDouble();
                reader.GetDouble();
                HP = reader.GetDouble();
                Dead = reader.GetBool();
                Money = reader.GetInt();
                CurrentGun = reader.GetInt();
                reader.GetDouble(); reader.GetDouble();
                InParkour = reader.GetBool();
                InTransport = reader.GetBool();
                BlockCamera = reader.GetBool();
                BlockInput = reader.GetBool();
                SelectedItem = reader.GetInt();
                int GUNSLength = reader.GetInt();
                for (int i = 0; i < GUNSLength; i++)
                    GUNS[i].HasIt = reader.GetBool();
                int GunsCount = reader.GetInt();
                List<Gun> tempGuns = [];
                for (int i = 0; i < GunsCount; i++)
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
                for (int i = 0; i < disposableItemsCount; i++)
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
                int effectsCount = reader.GetInt();
                List<Effect> tempEffects = [];
                for (int i = 0; i < effectsCount; i++)
                {
                    switch (reader.GetInt())
                    {
                        case 0:
                            Regeneration regeneration = new();
                            regeneration.Deserialize(reader);
                            tempEffects.Add(regeneration);
                            break;
                        case 1:
                            Adrenaline adrenaline = new();
                            adrenaline.Deserialize(reader);
                            tempEffects.Add(adrenaline);
                            break;
                        case 2:
                            Protection protection = new();
                            protection.Deserialize(reader);
                            tempEffects.Add(protection);
                            break;
                        case 3:
                            Fatigue fatigue = new();
                            fatigue.Deserialize(reader);
                            tempEffects.Add(fatigue);
                            break;
                        case 4:
                            Rider rider = new();
                            rider.Deserialize(reader);
                            tempEffects.Add(rider);
                            break;
                        case 5:
                            Bleeding bleeding = new();
                            bleeding.Deserialize(reader);
                            tempEffects.Add(bleeding);
                            break;
                        case 6:
                            Blindness blindness = new();
                            blindness.Deserialize(reader);
                            tempEffects.Add(blindness);
                            break;
                        default:
                            break;
                    }
                }
                Guns = tempGuns;
                DisposableItems = tempDisposableItems;
                Effects = tempEffects;
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
            Texture = 27;
            SetAnimations(1, 0);
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
                Guns.Add(GUNS[0]);
                Guns.Add(GUNS[1]);
                Guns.Add(GUNS[2]);
                for (int i = 0; i < DisposableItems.Count; i++)
                    DisposableItems[i].SetDefault();
                Money = 15;
                CurseCureChance = 0.08;
                Stage = 0;
                MAX_MOVE_SPEED = 1.8;
                MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 2;
                MAX_RUN_SPEED = 2.25;
                DEPTH = 8;
                SelectedItem = 0;
                DISPOSABLE_ITEM = DisposableItems[SelectedItem];
                PET = null;
                CuteMode = false;
                Fast = false;
                NoClip = false;
                PlayerDirection = Directions.STOP;
                StrafeDirection = Directions.STOP;
                PlayerMoveStyle = Directions.WALK;
                if (InTransport) StopEffect(4);
            }
            ItemFrame = 0;
            EnemiesKilled = 0;
            Look = 0;
            GunState = 0;
            MOVE_SPEED = 0;
            STRAFE_SPEED = 0;
            RUN_SPEED = 0;
            Dead = false;
            Invulnerable = false;
            TimeoutInvulnerable = 2;
            LevelUpdated = false;
            Aiming = false;
            UseItem = false;
            LevelUpdated = false;
            IsPetting = false;
            InParkour = false;
            InSelectingMode = false;
            BlockInput = false;
            if (!InTransport)
            {
                CanShoot = true;
                BlockCamera = false;
                BlockMouse = false;
                CanUnblockCamera = true;
            }
            ParkourState = 0;
            PreviousGun = CurrentGun = 2;
            STAMINE = MAX_STAMINE;
            if (Guns.Count == 0)
            {
                Guns.Add(GUNS[0]);
                Guns.Add(GUNS[1]);
                Guns.Add(GUNS[2]);
            }
        }

        public double GetDrawDistance()
        {
            if (EffectCheck(6)) return 4;
            if (InTransport) return DEPTH + 2;
            if (InParkour) return DEPTH;
            return DEPTH + (Aiming || GetCurrentGun() is Flashlight ? GetCurrentGun().AimingFactor : 0);
        }

        public void ReducesStamine()
        {
            int x = 3;
            if (GetWeight() >= 0.95) x = 2;
            STAMINE -= x;
        }

        public void RestoreStamine()
        {
            int x = 2;
            if (EffectCheck(3)) x = 1;
            STAMINE += x;
        }

        public double GetWeight()
        {
            if (InTransport) return 1;
            return GetCurrentGun().Weight;
        }

        public double GetMoveSpeed(double elapsed_time) => MOVE_SPEED * GetWeight() * RUN_SPEED * elapsed_time;

        public double GetStrafeSpeed(double elapsed_time) => STRAFE_SPEED * GetWeight() * RUN_SPEED * elapsed_time;

        public void ChangeItem(int index)
        {
            SelectedItem = index;
            DISPOSABLE_ITEM = DisposableItems[SelectedItem];
        }

        public void ChangeSpeed()
        {
            int speedFactor;
            double walk = 0.075, transport = 0.05;
            if (!InTransport)
            {
                if (STRAFE_SPEED > MAX_STRAFE_SPEED)
                    STRAFE_SPEED -= walk;
                if (MOVE_SPEED > MAX_MOVE_SPEED)
                    MOVE_SPEED -= walk;
            }
            else
            {
                if (STRAFE_SPEED > MAX_STRAFE_SPEED)
                    STRAFE_SPEED -= transport * 1.75;
                if (MOVE_SPEED > MAX_MOVE_SPEED)
                    MOVE_SPEED -= transport * 1.75;
            }
            switch (StrafeDirection)
            {
                case Directions.LEFT:
                    if (STRAFE_SPEED < 0) speedFactor = 4;
                    else speedFactor = 1;
                    if (!InTransport)
                    {
                        if (STRAFE_SPEED + walk * speedFactor <= MAX_STRAFE_SPEED + 0.01)
                            STRAFE_SPEED += walk * speedFactor;
                    }
                    else
                    {
                        if (MOVE_SPEED < 0.25 && PlayerDirection == Directions.FORWARD ||
                            MOVE_SPEED > -0.25 && PlayerDirection == Directions.BACK)
                            STRAFE_SPEED = 0;
                        if (STRAFE_SPEED + transport * 1.75 * speedFactor <= MAX_STRAFE_SPEED + 0.01)
                            STRAFE_SPEED += transport * 1.75 * speedFactor;
                    }
                    break;
                case Directions.RIGHT:
                    if (STRAFE_SPEED > 0) speedFactor = 4;
                    else speedFactor = 1;
                    if (!InTransport)
                    {
                        if (STRAFE_SPEED - walk * speedFactor >= -MAX_STRAFE_SPEED - 0.01)
                            STRAFE_SPEED -= walk * speedFactor;
                    }
                    else
                    {
                        if (MOVE_SPEED < 0.25 && PlayerDirection == Directions.FORWARD ||
                            MOVE_SPEED > -0.25 && PlayerDirection == Directions.BACK)
                            STRAFE_SPEED = 0;
                        if (STRAFE_SPEED - transport * 1.75 * speedFactor >= -MAX_STRAFE_SPEED - 0.01)
                            STRAFE_SPEED -= transport * 1.75 * speedFactor;
                    }
                    break;
                case Directions.STOP:
                    if (!InTransport)
                    {
                        if (STRAFE_SPEED + walk * 2 <= 0)
                            STRAFE_SPEED += walk * 2;
                        else if (STRAFE_SPEED - walk * 2 >= 0)
                            STRAFE_SPEED -= walk * 2;
                        else
                            STRAFE_SPEED = 0;
                    }
                    else
                    {
                        if (MOVE_SPEED < 0.25 && PlayerDirection == Directions.FORWARD ||
                            MOVE_SPEED > -0.25 && PlayerDirection == Directions.BACK)
                            STRAFE_SPEED = 0;
                        if (STRAFE_SPEED + transport * 1.75 * 2 <= 0)
                            STRAFE_SPEED += transport * 1.75 * 2;
                        else if (STRAFE_SPEED - transport * 1.75 * 2 >= 0)
                            STRAFE_SPEED -= transport * 1.75 * 2;
                        else
                            STRAFE_SPEED = 0;
                    }
                    break;
            }
            switch (PlayerDirection)
            {
                case Directions.FORWARD:
                    if (MOVE_SPEED < 0) speedFactor = 2;
                    else speedFactor = 1;
                    if (!InTransport)
                    {
                        if (MOVE_SPEED + walk * speedFactor <= MAX_MOVE_SPEED + 0.01)
                            MOVE_SPEED += walk * speedFactor;
                    }
                    else
                    {
                        if (MOVE_SPEED + transport * speedFactor <= MAX_MOVE_SPEED + 0.01)
                            MOVE_SPEED += transport * speedFactor;
                    }
                    break;
                case Directions.BACK:
                    if (MOVE_SPEED > 0) speedFactor = 2;
                    else speedFactor = 1;
                    if (!InTransport)
                    {
                        if (MOVE_SPEED - walk * speedFactor >= -MAX_MOVE_SPEED - 0.01)
                            MOVE_SPEED -= walk * speedFactor;
                    }
                    else
                    {
                        if (MOVE_SPEED - transport * speedFactor >= -MAX_MOVE_SPEED - 0.01)
                            MOVE_SPEED -= transport * speedFactor;
                    }
                    break;
                case Directions.STOP:
                    if (!InTransport)
                    {
                        if (MOVE_SPEED + walk * 2 <= 0)
                            MOVE_SPEED += walk * 2;
                        else if (MOVE_SPEED - walk * 2 >= 0)
                            MOVE_SPEED -= walk * 2;
                        else
                            MOVE_SPEED = 0;
                    }
                    else
                    {
                        if (MOVE_SPEED + transport * 2 <= 0)
                            MOVE_SPEED += transport * 2;
                        else if (MOVE_SPEED - transport * 2 >= 0)
                            MOVE_SPEED -= transport * 2;
                        else
                            MOVE_SPEED = 0;
                    }
                    break;
            }
            switch (PlayerMoveStyle)
            {
                case Directions.RUN:
                    if (RUN_SPEED + walk <= MAX_RUN_SPEED + 0.01)
                        RUN_SPEED += walk;
                    break;
                default:
                    if (RUN_SPEED > 1) RUN_SPEED -= walk * 2;
                    else RUN_SPEED = 1;
                    break;
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
                if (Effects[i].ID == 5)
                {
                    int damage = rand.Next(2);
                    if (HP - damage >= 1)
                        DealDamage(damage, false);
                }
                if (Effects[i].ReducingTimeRemaining())
                {
                    if (Effects[i].ID == 1)
                    {
                        Fast = false;
                        MAX_MOVE_SPEED -= 1.5;
                        MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 2;
                    }
                    else if (Effects[i].ID == 3)
                    {
                        MAX_STAMINE += 450;
                        STAMINE = MAX_STAMINE;
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
                if (EffectCheck(5)) StopEffect(5);
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
                MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 2;
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
                MAX_STAMINE -= 450;
                if (STAMINE > MAX_STAMINE)
                    STAMINE = MAX_STAMINE;
            }
            else if (index == 4)
            {
                if (EffectCheck(4) || TRANSPORT == null) return;
                StopEffect(0);
                StopEffect(1);
                Rider effect = new();
                effect.UpdateTimeRemaining();
                Effects.Add(effect);
                CanUnblockCamera = false;
                BlockMouse = true;
                CanShoot = false;
                Look = 0;
                InTransport = true;
                Fast = true;
                MAX_MOVE_SPEED += TRANSPORT.Speed;
                MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 2;
                MOVE_SPEED = 0;
                STRAFE_SPEED = 0;
            }
            else if (index == 5)
            {
                if (EffectCheck(5) || EffectCheck(4) || EffectCheck(0)) return;
                Bleeding effect = new();
                if (!standart_time)
                    effect.SetTotalTime(time);
                effect.Infinity = infinity;
                effect.UpdateTimeRemaining();
                Effects.Add(effect);
            }
            else if (index == 6)
            {
                if (EffectCheck(6)) return;
                Blindness effect = new();
                if (!standart_time)
                    effect.SetTotalTime(time);
                effect.Infinity = infinity;
                effect.UpdateTimeRemaining();
                Effects.Add(effect);
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
            ItemFrame = 0;
            if (SelectedItem == 0)
            {
                if (EffectCheck(0)) return;
                if (EffectCheck(5)) StopEffect(5);
                Effects.Add(new Regeneration());
            }
            else if (SelectedItem == 1)
            {
                if (EffectCheck(1) || EffectCheck(4)) return;
                Effects.Add(new Adrenaline());
                Fast = true;
                MAX_MOVE_SPEED += 1.5;
                MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 2;
            }
            else if (SelectedItem == 2)
            {
                if (EffectCheck(2)) return;
                Effects.Add(new Protection());
            }
        }

        public void ResetEffectTime(int id)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                if (Effects[i].ID == id)
                {
                    Effects[i].UpdateTimeRemaining();
                    break;
                }
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
                        MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 2;
                    }
                    else if (Effects[i].ID == 3)
                    {
                        MAX_STAMINE += 450;
                        STAMINE = MAX_STAMINE;
                    }
                    else if (Effects[i].ID == 4)
                    {
                        if (TRANSPORT != null)
                        {
                            BlockMouse = false;
                            CanShoot = true;
                            InTransport = false;
                            Fast = false;
                            MAX_MOVE_SPEED -= TRANSPORT.Speed;
                            MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 2;
                            MOVE_SPEED = 0;
                            STRAFE_SPEED = 0;
                            TRANSPORT = null;
                        }
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
                    MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 2;
                }
                else if (Effects[i].ID == 3)
                {
                    MAX_STAMINE += 450;
                    STAMINE = MAX_STAMINE;
                }
                else if (Effects[i].ID == 4)
                {
                    if (TRANSPORT != null)
                    {
                        BlockMouse = false;
                        CanShoot = true;
                        InTransport = false;
                        Fast = false;
                        MAX_MOVE_SPEED -= TRANSPORT.Speed;
                        MAX_STRAFE_SPEED = MAX_MOVE_SPEED / 2;
                        MOVE_SPEED = 0;
                        STRAFE_SPEED = 0;
                        TRANSPORT = null;
                    }
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

        public bool DealDamage(double damage, bool give_invulnerable)
        {
            if (EffectCheck(2) || EffectCheck(4)) damage *= 0.8;
            if (InTransport) TRANSPORT_HP -= damage;
            else HP -= damage;
            if (give_invulnerable)
            {
                TimeoutInvulnerable = 2;
                Invulnerable = true;
            }
            if (HP <= 0) Dead = true;
            if (TRANSPORT_HP <= 0) StopEffect(4);
            return Dead;
        }
    }
}