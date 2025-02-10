using LiteNetLib.Utils;
using System.Collections.Generic;

namespace SLIL.Classes
{
    public enum Directions { STOP, FORWARD, BACK, LEFT, RIGHT, WALK, RUN };

    public class Player : Entity
    {
        public int DeathSound { get; set; }
        public string Name { get; set; }
        public double Look { get; set; }
        public double HP { get; set; }
        public double TransportHP { get; set; }
        public bool Invulnerable { get; set; }
        public int TimeoutInvulnerable { get; set; }
        public double Stamine { get; set; }
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
        public bool CuteMode { get; set; }
        public int TotalEnemiesKilled { get; set; }
        public int EnemiesKilled { get; set; }
        public double MaxMoveSpeed { get; set; }
        public double MaxStrafeSpeed { get; set; }
        public double MoveSpeed { get; set; }
        public double StrafeSpeed { get; set; }
        public double MaxRunSpeed { get; set; }
        public double RunSpeed { get; set; }
        public double Depth { get; set; }
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
        public List<Effect> Effects = new List<Effect>();
        public readonly Gun[] GUNS =
        {
            new Flashlight(), new Knife(), new Pistol(),
            new Shotgun(), new SubmachineGun(), new AssaultRifle(),
            new SniperRifle(), new Fingershot(), new TSPitW(),
            new Gnome(), new FirstAidKit(), new Candy(),
            new Rainblower(), new Adrenalin(), new Helmet(),
            new RPG(), new Petition(), new MedicalKit()
        };
        public List<Gun> Guns = new List<Gun>();
        public List<DisposableItem> DisposableItems = new List<DisposableItem>();
        public DisposableItem DisposableItem = null;
        public int WeaponSlot_0 { get; set; }
        public int WeaponSlot_1 { get; set; }
        public int ItemFrame { get; set; }
        public Pet PET = null;
        public Transport TRANSPORT = null;
        public double MaxHP { get; set; }
        public double MaxStamine { get; set; }

        public Player(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => InitPlayer();
        public Player(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => InitPlayer();

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.Put(HP);
            writer.Put(Dead);
            writer.Put(Money);
            writer.Put(CurrentGun);
            writer.Put(WeaponSlot_0);
            writer.Put(WeaponSlot_1);
            writer.Put(TotalEnemiesKilled);
            writer.Put(A);
            writer.Put(Look);
            writer.Put(InParkour);
            writer.Put(InTransport);
            writer.Put(BlockCamera);
            writer.Put(BlockInput);
            writer.Put(SelectedItem);
            writer.Put(GUNS.Length);
            foreach (Gun gun in GUNS) writer.Put(gun.HasIt);
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
            WeaponSlot_0 = reader.GetInt();
            WeaponSlot_1 = reader.GetInt();
            TotalEnemiesKilled = reader.GetInt();
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
            List<Gun> tempGuns = new List<Gun>();
            for (int i = 0; i < GunsCount; i++)
            {
                int gunID = reader.GetInt();
                switch (gunID)
                {
                    case 0:
                        Flashlight flashlight = new Flashlight();
                        flashlight.Deserialize(reader);
                        tempGuns.Add(flashlight);
                        break;
                    case 1:
                        Knife knife = new Knife();
                        knife.Deserialize(reader);
                        tempGuns.Add(knife);
                        break;
                    case 2:
                        Candy candy = new Candy();
                        candy.Deserialize(reader);
                        tempGuns.Add(candy);
                        break;
                    case 3:
                        Rainblower rainblower = new Rainblower();
                        rainblower.Deserialize(reader);
                        tempGuns.Add(rainblower);
                        break;
                    case 4:
                        Pistol pistol = new Pistol();
                        pistol.Deserialize(reader);
                        tempGuns.Add(pistol);
                        break;
                    case 5:
                        Shotgun shotgun = new Shotgun();
                        shotgun.Deserialize(reader);
                        tempGuns.Add(shotgun);
                        break;
                    case 6:
                        SubmachineGun submachineGun = new SubmachineGun();
                        submachineGun.Deserialize(reader);
                        tempGuns.Add(submachineGun);
                        break;
                    case 7:
                        AssaultRifle assaultRifle = new AssaultRifle();
                        assaultRifle.Deserialize(reader);
                        tempGuns.Add(assaultRifle);
                        break;
                    case 8:
                        SniperRifle sniperRifle = new SniperRifle();
                        sniperRifle.Deserialize(reader);
                        tempGuns.Add(sniperRifle);
                        break;
                    case 9:
                        Fingershot fingershot = new Fingershot();
                        fingershot.Deserialize(reader);
                        tempGuns.Add(fingershot);
                        break;
                    case 10:
                        TSPitW tSPitW = new TSPitW();
                        tSPitW.Deserialize(reader);
                        tempGuns.Add(tSPitW);
                        break;
                    case 11:
                        Gnome gnome = new Gnome();
                        gnome.Deserialize(reader);
                        tempGuns.Add(gnome);
                        break;
                    case 12:
                        FirstAidKit firstAidKit = new FirstAidKit();
                        firstAidKit.Deserialize(reader);
                        tempGuns.Add(firstAidKit);
                        break;
                    case 13:
                        Adrenalin adrenalin = new Adrenalin();
                        adrenalin.Deserialize(reader);
                        tempGuns.Add(adrenalin);
                        break;
                    case 14:
                        Helmet helmet = new Helmet();
                        helmet.Deserialize(reader);
                        tempGuns.Add(helmet);
                        break;
                    case 15:
                        RPG rpg = new RPG();
                        rpg.Deserialize(reader);
                        tempGuns.Add(rpg);
                        break;
                    case 16:
                        C4 c4 = new C4();
                        c4.Deserialize(reader);
                        tempGuns.Add(c4);
                        break;
                    case 17:
                        Petition petition = new Petition();
                        petition.Deserialize(reader);
                        tempGuns.Add(petition);
                        break;
                    case 18:
                        MedicalKit medicalKit = new MedicalKit();
                        medicalKit.Deserialize(reader);
                        tempGuns.Add(medicalKit);
                        break;
                    default:
                        break;
                }
            }
            int disposableItemsCount = reader.GetInt();
            List<DisposableItem> tempDisposableItems = new List<DisposableItem>();
            for (int i = 0; i < disposableItemsCount; i++)
            {
                int itemID = reader.GetInt();
                switch (itemID)
                {
                    case 12:
                        FirstAidKit firstAidKit = new FirstAidKit();
                        firstAidKit.Deserialize(reader);
                        tempDisposableItems.Add(firstAidKit);
                        break;
                    case 13:
                        Adrenalin adrenalin = new Adrenalin();
                        adrenalin.Deserialize(reader);
                        tempDisposableItems.Add(adrenalin);
                        break;
                    case 14:
                        Helmet helmet = new Helmet();
                        helmet.Deserialize(reader);
                        tempDisposableItems.Add(helmet);
                        break;
                    case 18:
                        MedicalKit medicalKit = new MedicalKit();
                        medicalKit.Deserialize(reader);
                        tempDisposableItems.Add(medicalKit);
                        break;
                    default:
                        break;
                }
            }
            int effectsCount = reader.GetInt();
            List<Effect> tempEffects = new List<Effect>();
            for (int i = 0; i < effectsCount; i++)
            {
                switch (reader.GetInt())
                {
                    case 0:
                        Regeneration regeneration = new Regeneration();
                        regeneration.Deserialize(reader);
                        tempEffects.Add(regeneration);
                        break;
                    case 1:
                        Adrenaline adrenaline = new Adrenaline();
                        adrenaline.Deserialize(reader);
                        tempEffects.Add(adrenaline);
                        break;
                    case 2:
                        Protection protection = new Protection();
                        protection.Deserialize(reader);
                        tempEffects.Add(protection);
                        break;
                    case 3:
                        Fatigue fatigue = new Fatigue();
                        fatigue.Deserialize(reader);
                        tempEffects.Add(fatigue);
                        break;
                    case 4:
                        Rider rider = new Rider();
                        rider.Deserialize(reader);
                        tempEffects.Add(rider);
                        break;
                    case 5:
                        Bleeding bleeding = new Bleeding();
                        bleeding.Deserialize(reader);
                        tempEffects.Add(bleeding);
                        break;
                    case 6:
                        Blindness blindness = new Blindness();
                        blindness.Deserialize(reader);
                        tempEffects.Add(blindness);
                        break;
                    case 7:
                        Stunned stunned = new Stunned();
                        stunned.Deserialize(reader);
                        tempEffects.Add(stunned);
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
                WeaponSlot_0 = reader.GetInt();
                WeaponSlot_1 = reader.GetInt();
                TotalEnemiesKilled = reader.GetInt();
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
                List<Gun> tempGuns = new List<Gun>();
                for (int i = 0; i < GunsCount; i++)
                {
                    int gunID = reader.GetInt();
                    switch (gunID)
                    {
                        case 0:
                            Flashlight flashlight = new Flashlight();
                            flashlight.Deserialize(reader);
                            tempGuns.Add(flashlight);
                            break;
                        case 1:
                            Knife knife = new Knife();
                            knife.Deserialize(reader);
                            tempGuns.Add(knife);
                            break;
                        case 2:
                            Candy candy = new Candy();
                            candy.Deserialize(reader);
                            tempGuns.Add(candy);
                            break;
                        case 3:
                            Rainblower rainblower = new Rainblower();
                            rainblower.Deserialize(reader);
                            tempGuns.Add(rainblower);
                            break;
                        case 4:
                            Pistol pistol = new Pistol();
                            pistol.Deserialize(reader);
                            tempGuns.Add(pistol);
                            break;
                        case 5:
                            Shotgun shotgun = new Shotgun();
                            shotgun.Deserialize(reader);
                            tempGuns.Add(shotgun);
                            break;
                        case 6:
                            SubmachineGun submachineGun = new SubmachineGun();
                            submachineGun.Deserialize(reader);
                            tempGuns.Add(submachineGun);
                            break;
                        case 7:
                            AssaultRifle assaultRifle = new AssaultRifle();
                            assaultRifle.Deserialize(reader);
                            tempGuns.Add(assaultRifle);
                            break;
                        case 8:
                            SniperRifle sniperRifle = new SniperRifle();
                            sniperRifle.Deserialize(reader);
                            tempGuns.Add(sniperRifle);
                            break;
                        case 9:
                            Fingershot fingershot = new Fingershot();
                            fingershot.Deserialize(reader);
                            tempGuns.Add(fingershot);
                            break;
                        case 10:
                            TSPitW tSPitW = new TSPitW();
                            tSPitW.Deserialize(reader);
                            tempGuns.Add(tSPitW);
                            break;
                        case 11:
                            Gnome gnome = new Gnome();
                            gnome.Deserialize(reader);
                            tempGuns.Add(gnome);
                            break;
                        case 12:
                            FirstAidKit firstAidKit = new FirstAidKit();
                            firstAidKit.Deserialize(reader);
                            tempGuns.Add(firstAidKit);
                            break;
                        case 13:
                            Adrenalin adrenalin = new Adrenalin();
                            adrenalin.Deserialize(reader);
                            tempGuns.Add(adrenalin);
                            break;
                        case 14:
                            Helmet helmet = new Helmet();
                            helmet.Deserialize(reader);
                            tempGuns.Add(helmet);
                            break;
                        case 15:
                            RPG rpg = new RPG();
                            rpg.Deserialize(reader);
                            tempGuns.Add(rpg);
                            break;
                        case 16:
                            C4 c4 = new C4();
                            c4.Deserialize(reader);
                            tempGuns.Add(c4);
                            break;
                        case 17:
                            Petition petition = new Petition();
                            petition.Deserialize(reader);
                            tempGuns.Add(petition);
                            break;
                        case 18:
                            MedicalKit medicalKit = new MedicalKit();
                            medicalKit.Deserialize(reader);
                            tempGuns.Add(medicalKit);
                            break;
                        default:
                            break;
                    }
                }
                int disposableItemsCount = reader.GetInt();
                List<DisposableItem> tempDisposableItems = new List<DisposableItem>();
                for (int i = 0; i < disposableItemsCount; i++)
                {
                    int itemID = reader.GetInt();
                    switch (itemID)
                    {
                        case 12:
                            FirstAidKit firstAidKit = new FirstAidKit();
                            firstAidKit.Deserialize(reader);
                            tempDisposableItems.Add(firstAidKit);
                            break;
                        case 13:
                            Adrenalin adrenalin = new Adrenalin();
                            adrenalin.Deserialize(reader);
                            tempDisposableItems.Add(adrenalin);
                            break;
                        case 14:
                            Helmet helmet = new Helmet();
                            helmet.Deserialize(reader);
                            tempDisposableItems.Add(helmet);
                            break;
                        case 18:
                            MedicalKit medicalKit = new MedicalKit();
                            medicalKit.Deserialize(reader);
                            tempDisposableItems.Add(medicalKit);
                            break;
                        default:
                            break;
                    }
                }
                int effectsCount = reader.GetInt();
                List<Effect> tempEffects = new List<Effect>();
                for (int i = 0; i < effectsCount; i++)
                {
                    switch (reader.GetInt())
                    {
                        case 0:
                            Regeneration regeneration = new Regeneration();
                            regeneration.Deserialize(reader);
                            tempEffects.Add(regeneration);
                            break;
                        case 1:
                            Adrenaline adrenaline = new Adrenaline();
                            adrenaline.Deserialize(reader);
                            tempEffects.Add(adrenaline);
                            break;
                        case 2:
                            Protection protection = new Protection();
                            protection.Deserialize(reader);
                            tempEffects.Add(protection);
                            break;
                        case 3:
                            Fatigue fatigue = new Fatigue();
                            fatigue.Deserialize(reader);
                            tempEffects.Add(fatigue);
                            break;
                        case 4:
                            Rider rider = new Rider();
                            rider.Deserialize(reader);
                            tempEffects.Add(rider);
                            break;
                        case 5:
                            Bleeding bleeding = new Bleeding();
                            bleeding.Deserialize(reader);
                            tempEffects.Add(bleeding);
                            break;
                        case 6:
                            Blindness blindness = new Blindness();
                            blindness.Deserialize(reader);
                            tempEffects.Add(blindness);
                            break;
                        case 7:
                            Stunned stunned = new Stunned();
                            stunned.Deserialize(reader);
                            tempEffects.Add(stunned);
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
            DeathSound = 0;
            DisposableItems.Add((FirstAidKit)GUNS[10]);
            DisposableItems.Add((Adrenalin)GUNS[13]);
            DisposableItems.Add((Helmet)GUNS[14]);
            DisposableItems.Add((MedicalKit)GUNS[17]);
            Texture = 27;
            SetAnimations(1, 0);
            Dead = true;
            SetDefault();
        }

        public void SetDefault()
        {
            if (Dead)
            {
                MaxHP = 100;
                MaxStamine = 650;
                HP = MaxHP;
                Effects.Clear();
                Guns.Clear();
                Guns.Add(GUNS[0]);
                Guns.Add(GUNS[1]);
                Guns.Add(GUNS[2]);
                for (int i = 0; i < DisposableItems.Count; i++)
                    DisposableItems[i].SetDefault();
                Money = 15;
                CurseCureChance = 0.08;
                TotalEnemiesKilled = 0;
                WeaponSlot_0 = WeaponSlot_1 = -1;
                MaxMoveSpeed = 1.8;
                MaxStrafeSpeed = MaxMoveSpeed / 2;
                MaxRunSpeed = 2.25;
                Depth = 8;
                SelectedItem = 0;
                DisposableItem = DisposableItems[SelectedItem];
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
            MoveSpeed = 0;
            StrafeSpeed = 0;
            RunSpeed = 0;
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
            Stamine = MaxStamine;
            PreviousGun = CurrentGun = 2;
            if (Guns.Count == 0)
            {
                Guns.Add(GUNS[0]);
                Guns.Add(GUNS[1]);
                Guns.Add(GUNS[2]);
            }
        }

        public double GetDrawDistance()
        {
            if (EffectCheck(6)) return 3.25;
            if (EffectCheck(8)) return 5;
            if (InTransport) return Depth + 2;
            if (InParkour) return Depth;
            return Depth + (Aiming || GetCurrentGun() is Flashlight ? GetCurrentGun().AimingFactor : 0);
        }

        public void ReducesStamine()
        {
            int x = 3;
            if (GetWeight() >= 0.95) x = 2;
            Stamine -= x;
        }

        public void RestoreStamine()
        {
            int x = 2;
            if (EffectCheck(3)) x = 1;
            Stamine += x;
        }

        public double GetWeight()
        {
            if (InTransport) return 1;
            return GetCurrentGun().Weight;
        }

        public void ChangeSpeed()
        {
            int speedFactor;
            double walk = 0.075, transport = 0.05;
            if (!InTransport)
            {
                if (StrafeSpeed > MaxStrafeSpeed)
                    StrafeSpeed -= walk;
                if (MoveSpeed > MaxMoveSpeed)
                    MoveSpeed -= walk;
            }
            else
            {
                if (StrafeSpeed > MaxStrafeSpeed)
                    StrafeSpeed -= transport * 1.75;
                if (MoveSpeed > MaxMoveSpeed)
                    MoveSpeed -= transport * 1.75;
            }
            switch (StrafeDirection)
            {
                case Directions.LEFT:
                    if (StrafeSpeed < 0) speedFactor = 4;
                    else speedFactor = 1;
                    if (!InTransport)
                    {
                        if (StrafeSpeed + walk * speedFactor <= MaxStrafeSpeed + 0.01)
                            StrafeSpeed += walk * speedFactor;
                    }
                    else
                    {
                        if (MoveSpeed < 0.25 && PlayerDirection == Directions.FORWARD ||
                            MoveSpeed > -0.25 && PlayerDirection == Directions.BACK)
                            StrafeSpeed = 0;
                        if (StrafeSpeed + transport * 1.75 * speedFactor <= MaxStrafeSpeed + 0.01)
                            StrafeSpeed += transport * 1.75 * speedFactor;
                    }
                    break;
                case Directions.RIGHT:
                    if (StrafeSpeed > 0) speedFactor = 4;
                    else speedFactor = 1;
                    if (!InTransport)
                    {
                        if (StrafeSpeed - walk * speedFactor >= -MaxStrafeSpeed - 0.01)
                            StrafeSpeed -= walk * speedFactor;
                    }
                    else
                    {
                        if (MoveSpeed < 0.25 && PlayerDirection == Directions.FORWARD ||
                            MoveSpeed > -0.25 && PlayerDirection == Directions.BACK)
                            StrafeSpeed = 0;
                        if (StrafeSpeed - transport * 1.75 * speedFactor >= -MaxStrafeSpeed - 0.01)
                            StrafeSpeed -= transport * 1.75 * speedFactor;
                    }
                    break;
                case Directions.STOP:
                    if (!InTransport)
                    {
                        if (StrafeSpeed + walk * 2 <= 0)
                            StrafeSpeed += walk * 2;
                        else if (StrafeSpeed - walk * 2 >= 0)
                            StrafeSpeed -= walk * 2;
                        else
                            StrafeSpeed = 0;
                    }
                    else
                    {
                        if (MoveSpeed < 0.25 && PlayerDirection == Directions.FORWARD ||
                            MoveSpeed > -0.25 && PlayerDirection == Directions.BACK)
                            StrafeSpeed = 0;
                        if (StrafeSpeed + transport * 1.75 * 2 <= 0)
                            StrafeSpeed += transport * 1.75 * 2;
                        else if (StrafeSpeed - transport * 1.75 * 2 >= 0)
                            StrafeSpeed -= transport * 1.75 * 2;
                        else
                            StrafeSpeed = 0;
                    }
                    break;
            }
            switch (PlayerDirection)
            {
                case Directions.FORWARD:
                    if (MoveSpeed < 0) speedFactor = 2;
                    else speedFactor = 1;
                    if (!InTransport)
                    {
                        if (MoveSpeed + walk * speedFactor <= MaxMoveSpeed + 0.01)
                            MoveSpeed += walk * speedFactor;
                    }
                    else
                    {
                        if (MoveSpeed + transport * speedFactor <= MaxMoveSpeed + 0.01)
                            MoveSpeed += transport * speedFactor;
                    }
                    break;
                case Directions.BACK:
                    if (MoveSpeed > 0) speedFactor = 2;
                    else speedFactor = 1;
                    if (!InTransport)
                    {
                        if (MoveSpeed - walk * speedFactor >= -MaxMoveSpeed - 0.01)
                            MoveSpeed -= walk * speedFactor;
                    }
                    else
                    {
                        if (MoveSpeed - transport * speedFactor >= -MaxMoveSpeed - 0.01)
                            MoveSpeed -= transport * speedFactor;
                    }
                    break;
                case Directions.STOP:
                    if (!InTransport)
                    {
                        if (MoveSpeed + walk * 2 <= 0)
                            MoveSpeed += walk * 2;
                        else if (MoveSpeed - walk * 2 >= 0)
                            MoveSpeed -= walk * 2;
                        else
                            MoveSpeed = 0;
                    }
                    else
                    {
                        if (MoveSpeed + transport * 2 <= 0)
                            MoveSpeed += transport * 2;
                        else if (MoveSpeed - transport * 2 >= 0)
                            MoveSpeed -= transport * 2;
                        else
                            MoveSpeed = 0;
                    }
                    break;
            }
            switch (PlayerMoveStyle)
            {
                case Directions.RUN:
                    if (RunSpeed + walk <= MaxRunSpeed + 0.01)
                        RunSpeed += walk;
                    break;
                default:
                    if (RunSpeed > 1) RunSpeed -= walk * 2;
                    else RunSpeed = 1;
                    break;
            }
        }

        public double GetMoveSpeed(double elapsed_time) => MoveSpeed * GetWeight() * RunSpeed * elapsed_time;

        public double GetStrafeSpeed(double elapsed_time) => StrafeSpeed * GetWeight() * RunSpeed * elapsed_time;

        public void ChangeItem(int index)
        {
            SelectedItem = index;
            DisposableItem = DisposableItems[SelectedItem];
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
                    int damage = rand.Next(3);
                    if (HP - damage >= 1) DealDamage(damage, false);
                }
                if (Effects[i].ReducingTimeRemaining())
                {
                    if (Effects[i].ID == 1)
                    {
                        Fast = false;
                        MaxMoveSpeed -= 1.5;
                        MaxStrafeSpeed = MaxMoveSpeed / 2;
                    }
                    else if (Effects[i].ID == 3)
                    {
                        MaxStamine += 450;
                        Stamine = MaxStamine;
                    }
                    else if (Effects[i].ID == 7)
                    {
                        MaxMoveSpeed += 0.8;
                        MaxStrafeSpeed = MaxMoveSpeed / 2;
                    }
                    Effects.RemoveAt(i);
                }
            }
        }

        public void GiveEffect(int index, bool standart_time, int time = 0, bool infinity = false)
        {
            UseItem = false;
            Effect effect = null;
            switch (index)
            {
                case 0:
                    if (EffectCheck(0)) return;
                    if (EffectCheck(5)) StopEffect(5);
                    effect = new Regeneration();
                    break;
                case 1:
                    if (EffectCheck(1) || EffectCheck(4)) return;
                    effect = new Adrenaline();
                    Fast = true;
                    MaxMoveSpeed += 1.5;
                    MaxStrafeSpeed = MaxMoveSpeed / 2;
                    break;
                case 2:
                    if (EffectCheck(2)) return;
                    effect = new Protection();
                    break;
                case 3:
                    if (EffectCheck(3) || EffectCheck(9)) return;
                    effect = new Fatigue();
                    MaxStamine -= 450;
                    if (Stamine > MaxStamine) Stamine = MaxStamine;
                    break;
                case 4:
                    if (EffectCheck(4) || TRANSPORT == null) return;
                    StopEffect(0);
                    StopEffect(1);
                    effect = new Rider();
                    CanUnblockCamera = false;
                    BlockMouse = true;
                    CanShoot = false;
                    Look = 0;
                    InTransport = true;
                    Fast = true;
                    MaxMoveSpeed += TRANSPORT.Speed;
                    MaxStrafeSpeed = MaxMoveSpeed / 2;
                    MoveSpeed = 0;
                    StrafeSpeed = 0;
                    break;
                case 5:
                    if (EffectCheck(5) || EffectCheck(4) || EffectCheck(0)) return;
                    if (EffectCheck(9)) return;
                    effect = new Bleeding();
                    break;
                case 6:
                    if (EffectCheck(6) || EffectCheck(9)) return;
                    effect = new Blindness();
                    break;
                case 7:
                    if (EffectCheck(7) || EffectCheck(9)) return;
                    effect = new Stunned();
                    MaxMoveSpeed -= 0.8;
                    MaxStrafeSpeed = MaxMoveSpeed / 2;
                    break;
                case 8:
                    if (EffectCheck(8)) return;
                    effect = new VoidE();
                    break;
                case 9:
                    if (EffectCheck(9)) return;
                    effect = new God();
                    break;
            }
            if (effect != null)
            {
                if (!standart_time)
                {
                    effect.SetTotalTime(time);
                    effect.Infinity = infinity;
                }
                effect.UpdateTimeRemaining();
                Effects.Add(effect);
            }
        }

        public int GetEffectID() => DisposableItems[SelectedItem].EffectID;

        public bool EffectCheck(int id)
        {
            if (Effects.Count >= 12) return true;
            for (int i = 0; i < Effects.Count; i++)
                if (Effects[i].ID == id) return true;
            return false;
        }

        public void SetItemEffect()
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
                MaxMoveSpeed += 1.5;
                MaxStrafeSpeed = MaxMoveSpeed / 2;
            }
            else if (SelectedItem == 2)
            {
                if (EffectCheck(2)) return;
                Effects.Add(new Protection());
            }
            else if (SelectedItem == 3)
            {
                if (EffectCheck(5)) StopEffect(5);
                HealHP((int)((MaxHP - HP) * 0.8));
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
                        MaxMoveSpeed -= 1.5;
                        MaxStrafeSpeed = MaxMoveSpeed / 2;
                    }
                    else if (Effects[i].ID == 3)
                    {
                        MaxStamine += 450;
                        Stamine = MaxStamine;
                    }
                    else if (Effects[i].ID == 4)
                    {
                        if (TRANSPORT != null)
                        {
                            BlockMouse = false;
                            CanShoot = true;
                            InTransport = false;
                            Fast = false;
                            MaxMoveSpeed -= TRANSPORT.Speed;
                            MaxStrafeSpeed = MaxMoveSpeed / 2;
                            MoveSpeed = 0;
                            StrafeSpeed = 0;
                            TRANSPORT = null;
                        }
                    }
                    else if (Effects[i].ID == 7)
                    {
                        MaxMoveSpeed += 0.8;
                        MaxStrafeSpeed = MaxMoveSpeed / 2;
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
                    MaxMoveSpeed -= 1.5;
                    MaxStrafeSpeed = MaxMoveSpeed / 2;
                }
                else if (Effects[i].ID == 3)
                {
                    MaxStamine += 450;
                    Stamine = MaxStamine;
                }
                else if (Effects[i].ID == 4)
                {
                    if (TRANSPORT != null)
                    {
                        BlockMouse = false;
                        CanShoot = true;
                        InTransport = false;
                        Fast = false;
                        MaxMoveSpeed -= TRANSPORT.Speed;
                        MaxStrafeSpeed = MaxMoveSpeed / 2;
                        MoveSpeed = 0;
                        StrafeSpeed = 0;
                        TRANSPORT = null;
                    }
                }
                else if (Effects[i].ID == 7)
                {
                    MaxMoveSpeed += 0.8;
                    MaxStrafeSpeed = MaxMoveSpeed / 2;
                }
            }
            Effects.Clear();
        }

        public void HealHP(int value)
        {
            HP += value;
            if (HP > MaxHP) HP = MaxHP;
        }

        public void ChangeMoney(int value)
        {
            Money += value;
            if (Money > 9999) Money = 9999;
            else if (Money < 0) Money = 0;
        }

        protected override int GetEntityID() => 0;

        protected override int GetTexture() => Texture;

        protected override double GetEntityWidth() => 0.4;

        public bool DealDamage(double damage, bool give_invulnerable)
        {
            if (Invulnerable || EffectCheck(9)) return Dead;
            if (EffectCheck(2) || EffectCheck(4)) damage *= 0.8;
            if (InTransport) TransportHP -= damage;
            else HP -= damage;
            if (give_invulnerable)
            {
                TimeoutInvulnerable = 2;
                Invulnerable = true;
            }
            if (HP <= 0) Dead = true;
            if (TransportHP <= 0) StopEffect(4);
            return Dead;
        }
    }
}