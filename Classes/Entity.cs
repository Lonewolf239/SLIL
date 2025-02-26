using System;
using System.Linq;
using LiteNetLib.Utils;

namespace SLIL.Classes
{
    public abstract class Entity : INetSerializable
    {
        public int ID { get; set; }
        public int EntityID { get; set; }
        public bool HasAI { get; set; }
        protected double Angle { get; set; }
        public double A { get => ML.NormalizeAngle(Angle); set => Angle = value; }
        public double X { get; set; }
        public double Y { get; set; }
        public double EntityWidth;
        public int IntX { get; set; }
        public int IntY { get; set; }
        public double VMove { get; set; }
        public int Texture { get; set; }
        public int[] Animations { get; set; }
        public bool RespondsToFlashlight { get; set; }
        public int Frames { get; set; }
        public bool HasStaticAnimation { get; set; }
        public bool HasSpriteRotation { get; set; }
        protected readonly Random rand;

        protected abstract int GetEntityID();

        protected abstract int GetTexture();
        protected abstract double GetEntityWidth();
        public virtual int Interaction() => 0;

        public Entity(double x, double y, int mapWidth, ref int maxEntityID)
        {
            ID = maxEntityID;
            maxEntityID++;
            EntityID = this.GetEntityID();
            rand = new Random();
            VMove = this.GetVMove();
            Frames = 24;
            EntityWidth = this.GetEntityWidth();
            RespondsToFlashlight = false;
            Texture = this.GetTexture();
            HasAI = true;
            HasStaticAnimation = false;
            HasSpriteRotation = false;
            X = x;
            Y = y;
            IntX = (int)x;
            IntY = (int)y;
        }
        public Entity(double x, double y, int mapWidth, int maxEntityID)
        {
            ID = maxEntityID;
            EntityID = this.GetEntityID();
            rand = new Random();
            VMove = this.GetVMove();
            Frames = 24;
            Texture = this.GetTexture();
            EntityWidth = this.GetEntityWidth();
            RespondsToFlashlight = false;
            Texture = this.GetTexture();
            HasAI = true;
            HasStaticAnimation = false;
            HasSpriteRotation = false;
            X = x;
            Y = y;
            IntX = (int)x;
            IntY = (int)y;
        }

        public virtual void Serialize(NetDataWriter writer)
        {
            writer.Put(this.X);
            writer.Put(this.Y);
            writer.Put(this.A);
        }

        public virtual void Deserialize(NetDataReader reader)
        {
            this.X = reader.GetDouble();
            this.Y = reader.GetDouble();
            this.A = reader.GetDouble();
        }

        protected void AnimationsToStatic() => HasStaticAnimation = true;

        protected void SetAnimations(int pause, int mode)
        {
            Animations = new int[Frames];
            int state = 0;
            for (int item = 0; item < Frames; item++)
            {
                if (mode == 1)
                {
                    if (item % pause == 0)
                        Animations[item] = 1;
                    else
                        Animations[item] = 0;
                }
                else if (mode == 2)
                {
                    if (item >= pause)
                        Animations[item] = 1;
                    else
                        Animations[item] = 0;
                }
                else
                {
                    if (item % pause == 0) state = state == 1 ? 0 : 1;
                    Animations[item] = state;
                }
            }
        }

        protected virtual double GetVMove() => 0;
    }

    public abstract class Creature : Entity
    {
        protected double HP { get; set; }
        protected char[] ImpassibleCells;
        protected int MovesInARow;
        protected int NumberOfMovesLeft;
        public bool CanHit { get; set; }
        public bool Dead { get; set; }
        public int Respawn { get; set; }
        public int MaxMoney { get; set; }
        public int MinMoney { get; set; }
        public int MaxDamage { get; set; }
        public int MinDamage { get; set; }
        protected int MapWidth { get; set; }
        protected int MaxHP { get; set; }
        public int DeathSound { get; set; }
        protected const int RespawnTime = 60;

        protected abstract int GetMaxHP();
        protected abstract int GetMAX_MONEY();
        protected abstract int GetMIN_MONEY();
        protected abstract int GetMAX_DAMAGE();
        protected abstract int GetMIN_DAMAGE();
        protected abstract char[] GetImpassibleCells();
        protected abstract int GetMovesInARow();
        public abstract double GetMove();

        public Creature(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init(mapWidth);
        public Creature(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init(mapWidth);

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.Put(HP);
            writer.Put(Dead);
        }

        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
            this.HP = reader.GetDouble();
            this.Dead = reader.GetBool();
        }

        private void Init(int mapWidth)
        {
            MaxHP = this.GetMaxHP();
            MaxMoney = this.GetMAX_MONEY();
            MinMoney = this.GetMIN_MONEY();
            MaxDamage = this.GetMAX_DAMAGE();
            MinDamage = this.GetMIN_DAMAGE();
            ImpassibleCells = this.GetImpassibleCells();
            MovesInARow = this.GetMovesInARow();
            NumberOfMovesLeft = MovesInARow;
            HP = MaxHP;
            A = rand.NextDouble();
            MapWidth = mapWidth;
            DeathSound = -1;
        }

        public virtual bool DealDamage(double damage)
        {
            HP -= damage;
            if (HP <= 0) Kill();
            return Dead;
        }

        public void Kill()
        {
            Respawn = RespawnTime;
            Dead = true;
        }

        public void DoRespawn()
        {
            HP = MaxHP;
            Dead = false;
        }

        public virtual void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
        {
            double move = this.GetMove();
            double newX = X;
            double newY = Y;
            double tempX = X;
            double tempY = Y;
            newX += Math.Sin(A) * move;
            newY += Math.Cos(A) * move;
            if (NumberOfMovesLeft > 0)
                NumberOfMovesLeft--;
            else
            {
                A = rand.NextDouble() * (Math.PI * 2);
                NumberOfMovesLeft = MovesInARow;
            }
            IntX = (int)X;
            IntY = (int)Y;
            if (!(ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX + EntityWidth / 2)])
                || ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX - EntityWidth / 2)])))
                tempX = newX;
            if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MapWidth + (int)newX])
                || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MapWidth + (int)newX])))
                tempY = newY;
            if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX + EntityWidth / 2)]))
            {
                tempX -= EntityWidth / 2 - (1 - tempX % 1);
                A = rand.NextDouble() * (Math.PI * 2);
                NumberOfMovesLeft = MovesInARow;
            }
            if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX - EntityWidth / 2)]))
            {
                tempX += EntityWidth / 2 - (tempX % 1);
                A = rand.NextDouble() * (Math.PI * 2);
                NumberOfMovesLeft = MovesInARow;
            }
            if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MapWidth + (int)tempX]))
            {
                tempY -= EntityWidth / 2 - (1 - tempY % 1);
                A = rand.NextDouble() * (Math.PI * 2);
                NumberOfMovesLeft = MovesInARow;
            }
            if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MapWidth + (int)tempX]))
            {
                tempY += EntityWidth / 2 - (tempY % 1);
                A = rand.NextDouble() * (Math.PI * 2);
                NumberOfMovesLeft = MovesInARow;
            }
            X = tempX;
            Y = tempY;
        }
    }

    public abstract class Friend : Creature
    {
        public Friend(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => CanHit = false;
        public Friend(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => CanHit = false;
    }

    public abstract class NPC : Friend
    {
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 0;
        protected override int GetMaxHP() => 0;
        protected override int GetTexture() => Texture;
        public override double GetMove() => 0;
        protected override int GetMAX_MONEY() => 0;
        protected override int GetMIN_MONEY() => 0;
        protected override int GetMAX_DAMAGE() => 0;
        protected override int GetMIN_DAMAGE() => 0;


        public NPC(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => RespondsToFlashlight = false;
        public NPC(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => RespondsToFlashlight = false;

        public override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0) { }
    }

    public abstract class Pet : Friend
    {
        protected double detectionRange;
        public bool Stoped { get; set; }
        public bool HasStopAnimation { get; set; }
        public string[] Name { get; set; }
        public string[] Description { get; set; }
        public int Cost { get; set; }
        public int Index { get; set; }
        public bool PetAbilityReloading { get; set; }
        public int IsInstantAbility { get; set; }
        public int AbilityReloadTime { get; set; }
        public int AbilityTimer { get; set; }
        protected int PetAbility { get; set; }
        protected override double GetEntityWidth() => 0.1;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'S' };
        protected override int GetMovesInARow() => 0;
        protected override int GetMaxHP() => 0;
        protected override int GetTexture() => Texture;
        public override double GetMove() => 0.2;
        protected override int GetMAX_MONEY() => 0;
        protected override int GetMIN_MONEY() => 0;
        protected override int GetMAX_DAMAGE() => 0;
        protected override int GetMIN_DAMAGE() => 0;

        public Pet(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Pet(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            IsInstantAbility = 0;
            HasStopAnimation = false;
            Stoped = false;
            detectionRange = 8.0;
        }

        public void SetNewParametrs(double x, double y, int mapWidth)
        {
            X = x;
            Y = y;
            MapWidth = mapWidth;
        }

        public int GetPetAbility() => PetAbility;

        public override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
        {
            Stoped = false;
            bool isPlayerVisible = true;
            double distanceToPlayer = ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y));
            if (distanceToPlayer > detectionRange) isPlayerVisible = false;
            double angleToPlayer = Math.Atan2(X - playerX, Y - playerY) - Math.PI;
            if (isPlayerVisible)
            {
                double distance = 0;
                double step = 0.01;
                double rayAngleX = Math.Sin(angleToPlayer);
                double rayAngleY = Math.Cos(angleToPlayer);
                while (distance <= distanceToPlayer)
                {
                    int test_x = (int)(X + rayAngleX * distance);
                    int test_y = (int)(Y + rayAngleY * distance);
                    if (test_x == (int)playerX && test_y == (int)playerY)
                        break;
                    if (ImpassibleCells.Contains(map[test_y * MapWidth + test_x]))
                    {
                        isPlayerVisible = false;
                        break;
                    }
                    distance += step;
                }
            }
            double move = this.GetMove();
            double newX = X;
            double newY = Y;
            double tempX = X;
            double tempY = Y;
            A = angleToPlayer;
            if (ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y)) <= EntityWidth) return;
            newX += Math.Sin(A) * move;
            newY += Math.Cos(A) * move;
            IntX = (int)X;
            IntY = (int)Y;
            if (!(ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX + EntityWidth / 2)])
                || ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX - EntityWidth / 2)])))
                tempX = newX;
            if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MapWidth + (int)newX])
                || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MapWidth + (int)newX])))
                tempY = newY;
            if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX + EntityWidth / 2)]))
                tempX -= EntityWidth / 2 - (1 - tempX % 1);
            if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX - EntityWidth / 2)]))
                tempX += EntityWidth / 2 - (tempX % 1);
            if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MapWidth + (int)tempX]))
                tempY -= EntityWidth / 2 - (1 - tempY % 1);
            if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MapWidth + (int)tempX]))
                tempY += EntityWidth / 2 - (tempY % 1);
            if (isPlayerVisible)
            {
                if (ML.GetDistance(new TPoint(playerX, playerY), new TPoint(tempX, tempY)) >= 0.75)
                {
                    X = tempX;
                    Y = tempY;
                }
            }
            else
            {
                X = playerX + 0.1;
                Y = playerY + 0.1;
            }
        }
    }

    public abstract class GameObject : Entity
    {
        public bool Temporarily { get; set; }
        public int TotalLifeTime { get; set; }
        public int LifeTime { get; set; }
        public bool Animated { get; set; }
        public int CurrentFrame { get; set; }
        protected override double GetEntityWidth() => 0.4;
        protected override int GetTexture() => Texture;

        public GameObject(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public GameObject(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Temporarily = false;
            RespondsToFlashlight = false;
            TotalLifeTime = Frames;
            Animated = false;
            CurrentFrame = 0;
        }
    }

    public abstract class Decoration : GameObject
    {
        public Decoration(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) { }
        public Decoration(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) { }
    }

    public abstract class Enemy : Creature
    {
        public enum Stages { Roaming, Chasing, Escaping };
        public Stages Stage;
        protected double DetectionRange;
        public bool Fast { get; set; }

        public Enemy(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Enemy(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Stage = Stages.Roaming;
            CanHit = true;
            Fast = false;
        }

        public void SetDamage(double offset)
        {
            MaxDamage = (int)(MaxDamage * offset);
            MinDamage = (int)(MinDamage * offset);
        }
    }

    public abstract class RangeEnemy : Enemy
    {
        protected double SafeDistance { get; set; }
        public double ShotDistance { get; set; }
        public int TotalShotPause { get; set; }
        protected int TotalTimeAfterShot { get; set; }
        public int ShotPause = 0, TimeAfterShot = 0;
        public double ShotA = 0;
        public bool ReadyToShot = false, DidShot = false;

        public RangeEnemy(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public RangeEnemy(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init() => ShotPause = TotalShotPause;

        protected abstract bool ShotLogic(bool isPlayerVisible, double distanceToPlayer, double angleToPlayer);
    }

    public abstract class Rockets : NPC
    {
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', 'S' };
        public abstract char[] GetInpassibleRocketCells();
        public bool CanHitOnlyPlayer = false;
        public int ExplosionID { get; set; }

        public Rockets(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => CanHit = false;
        public Rockets(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => CanHit = false;

        public void SetA(double value) => A = value;
        public override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
        {
            double move = this.GetMove();
            double newX = X;
            double newY = Y;
            double tempX = X;
            double tempY = Y;
            newX += Math.Sin(A) * move;
            newY += Math.Cos(A) * move;
            if (!(ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX + EntityWidth / 2)])
                || ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX - EntityWidth / 2)])))
                tempX = newX;
            if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MapWidth + (int)newX])
                || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MapWidth + (int)newX])))
                tempY = newY;
            if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX + EntityWidth / 2)]))
                tempX -= EntityWidth / 2 - (1 - tempX % 1);
            if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX - EntityWidth / 2)]))
                tempX += EntityWidth / 2 - (tempX % 1);
            if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MapWidth + (int)tempX]))
                tempY -= EntityWidth / 2 - (1 - tempY % 1);
            if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MapWidth + (int)tempX]))
                tempY += EntityWidth / 2 - (tempY % 1);
            X = tempX;
            Y = tempY;
        }
    }

    public abstract class Boxes : NPC
    {
        public bool BoxWithMoney { get; set; }
        public double MoneyChance { get; set; }

        public Boxes(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Boxes(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            CanHit = true;
            HP = 2.5;
        }

        public void SetMoneyChance(Random r)
        {
            if (r.NextDouble() <= MoneyChance)
                BoxWithMoney = true;
        }

        public override bool DealDamage(double damage)
        {
            if (!CanHit) return false;
            HP -= damage;
            if (HP <= 0)
                Dead = true;
            return Dead;
        }
    }

    public abstract class Transport : GameObject
    {
        public string[] Name { get; set; }
        public int Index { get; set; }
        public int Cost { get; set; }
        public bool CanJump { get; set; }
        public bool AddToShop { get; set; }
        public double TransportHP { get; set; } //max: 500
        public double Speed { get; set; } //max: 7.5
        public int Controllability { get; set; } //90-175

        public Transport(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Transport(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init() => A = rand.NextDouble();

        public bool DealDamage(double damage)
        {
            TransportHP -= damage;
            if (TransportHP <= 0) return true;
            return false;
        }
    }

    public abstract class Explosions : GameObject
    {
        public int ShooterID;
        public bool CanHitOnlyPlayer = false;
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public double HitDistance { get; set; }

        public Explosions(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Explosions(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            LifeTime = 0;
            TotalLifeTime = 4;
            Temporarily = true;
            Animated = true;
        }
    }

    public class Covering : GameObject
    {
        public float HP { get; set; }
        public bool Broken { get; set; }
        protected override int GetEntityID() => 22;

        public Covering(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Covering(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 33;
            HP = 100;
            Animated = false;
            Broken = false;
            base.AnimationsToStatic();
        }

        public void FullRepair()
        {
            HP = 100;
            Broken = false;
        }
        public void Repair(float value)
        {
            HP += value;
            Broken = false;
            if (HP > 100) HP = 100;
        }
        public bool DealDamage(float value)
        {
            HP -= value;
            if (HP <= 0)
            {
                HP = 0;
                Broken = true;
            }
            return Broken;
        }
    }

    public class Bike : Transport
    {
        public Bike(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Bike(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 21;
            Index = 0;
            CanJump = true;
            AddToShop = true;
            //HasSpriteRotation = true;
            Cost = 150;
            TransportHP = 150;
            Speed = 2.35;
            Controllability = 95;
            Name = new string[] { "7-0", "Motorbike" };
            base.AnimationsToStatic();
        }
        protected override int GetEntityID() => 18;
        public override int Interaction() => 4;
    }

    public class RpgRocket : Rockets
    {
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'S' };
        public override char[] GetInpassibleRocketCells() => GetImpassibleCells();
        protected override int GetEntityID() => 16;
        protected override double GetEntityWidth() => 0.25;
        public override double GetMove() => 0.6;

        public RpgRocket(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public RpgRocket(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 26;
            ExplosionID = 0;
            base.SetAnimations(1, 0);
        }
    }

    public class SoulClot : Rockets
    {
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', 'S' };
        public override char[] GetInpassibleRocketCells() => GetImpassibleCells();
        protected override int GetEntityID() => 28;
        protected override double GetEntityWidth() => 0.25;
        public override double GetMove() => 0.4;

        public SoulClot(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public SoulClot(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 42;
            ExplosionID = 1;
            CanHitOnlyPlayer = true;
            base.SetAnimations(1, 0);
        }
    }

    public class RpgExplosion : Explosions
    {
        protected override int GetEntityID() => 17;
        protected override double GetEntityWidth() => 0.4;

        public RpgExplosion(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public RpgExplosion(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 27;
            MinDamage = 25;
            MaxDamage = 50;
            HitDistance = 2.66;
            base.SetAnimations(2, 2);
        }
    }

    public class SoulExplosion : Explosions
    {
        protected override int GetEntityID() => 29;
        protected override double GetEntityWidth() => 0.4;

        public SoulExplosion(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public SoulExplosion(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 27;
            MinDamage = 10;
            MaxDamage = 25;
            HitDistance = 1.75;
            CanHitOnlyPlayer = true;
            base.SetAnimations(2, 2);
        }
    }

    public class PlayerDeadBody : NPC
    {
        public PlayerDeadBody(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public PlayerDeadBody(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Dead = true;
            Texture = 28;
            base.AnimationsToStatic();
        }
        protected override int GetEntityID() => 13;
    }

    public class SillyCat : Pet
    {
        public SillyCat(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public SillyCat(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Index = 0;
            Cost = 150;
            Name = new[] { "5-0", "Silly Cat" };
            Description = new[] { "5-1", "Restores 2 HP every 8 seconds" };
            Texture = 22;
            PetAbility = 0;
            AbilityReloadTime = 8;
            HasStopAnimation = true;
            RespondsToFlashlight = true;
            base.SetAnimations(1, 0);
        }
        protected override int GetEntityID() => 5;
        public override int Interaction() => 1;
    }

    public class GreenGnome : Pet
    {
        public GreenGnome(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public GreenGnome(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Index = 1;
            Cost = 60;
            Name = new[] { "5-2", "Green Gnome" };
            Description = new[] { "5-3", "Increases maximum health by 25" };
            Texture = 23;
            PetAbility = 1;
            IsInstantAbility = 1;
            RespondsToFlashlight = true;
            base.SetAnimations(6, 1);
        }
        protected override int GetEntityID() => 6;
        public override int Interaction() => 2;
    }

    public class EnergyDrink : Pet
    {
        public EnergyDrink(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public EnergyDrink(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Index = 2;
            Cost = 60;
            Name = new[] { "5-4", "Energy Drink" };
            Description = new[] { "5-5", "Increases endurance and speed" };
            Texture = 24;
            PetAbility = 2;
            IsInstantAbility = 1;
            RespondsToFlashlight = false;
            base.AnimationsToStatic();
        }
        protected override int GetEntityID() => 7;
        public override int Interaction() => 3;
    }

    public class Pyro : Pet
    {
        protected override int GetEntityID() => 8;
        protected override char[] GetImpassibleCells()
        {
            return new char[] { '#', 'D', 'd', 'S' };
        }

        public Pyro(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Pyro(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Index = 3;
            Cost = 666;
            Name = new[] { "5-6", "Podseratel" };
            Description = new[] { "5-7", "The world is a fairy tale..." };
            Texture = 25;
            PetAbility = 3;
            IsInstantAbility = 2;
            AbilityReloadTime = 15;
            RespondsToFlashlight = true;
            base.SetAnimations(1, 0);
        }
        public override int Interaction() => 2;
    }

    public class VoidTeleport : GameObject
    {
        protected override int GetEntityID() => 23;

        public VoidTeleport(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public VoidTeleport(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 34;
            Animated = true;
            base.SetAnimations(1, 0);
        }
    }

    public class BackroomsTeleport : GameObject
    {
        protected override int GetEntityID() => 21;

        public BackroomsTeleport(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public BackroomsTeleport(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 32;
            Animated = true;
            base.SetAnimations(1, 0);
        }
    }

    public class Teleport : GameObject
    {
        protected override int GetEntityID() => 9;

        public Teleport(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Teleport(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 13;
            Animated = true;
            base.SetAnimations(1, 0);
        }
    }

    public class Box : Boxes
    {
        protected override int GetEntityID() => 14;

        public Box(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Box(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 16;
            DeathSound = 4;
            MoneyChance = 0.25;
            SetMoneyChance(new Random());
            base.AnimationsToStatic();
        }
    }

    public class Barrel : Boxes
    {
        protected override int GetEntityID() => 15;

        public Barrel(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Barrel(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 17;
            DeathSound = 4;
            MoneyChance = 0.75;
            SetMoneyChance(new Random());
            base.AnimationsToStatic();
        }
    }

    public class HittingTheWall : GameObject
    {
        protected override int GetEntityID() => 10;
        public HittingTheWall(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public HittingTheWall(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
            this.VMove = reader.GetDouble();
        }

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.Put(this.VMove);
        }

        private void Init()
        {
            Texture = 15;
            LifeTime = 0;
            TotalLifeTime = 4;
            Temporarily = true;
            Animated = true;
            base.SetAnimations(2, 2);
        }
    }

    public class ShopDoor : GameObject
    {
        protected override int GetEntityID() => 11;
        public ShopDoor(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public ShopDoor(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 6;
            base.AnimationsToStatic();
        }

        public override int Interaction() => 5;
    }

    public class ShopMan : NPC
    {
        protected override int GetEntityID() => 12;

        public ShopMan(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public ShopMan(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 14;
            RespondsToFlashlight = true;
            base.AnimationsToStatic();
        }

        public override int Interaction() => 5;
    }

    public class Zombie : Enemy
    {
        protected override int GetEntityID() => 1;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 10;
        protected override int GetMaxHP() => 10;
        protected override int GetTexture() => Texture;
        public override double GetMove() => 0.16;
        protected override int GetMAX_MONEY() => 10;
        protected override int GetMIN_MONEY() => 5;
        protected override int GetMAX_DAMAGE() => 35;
        protected override int GetMIN_DAMAGE() => 15;

        public Zombie(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Zombie(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 0;
            Texture = 9;
            DetectionRange = 5;
            //HasSpriteRotation = true;
            base.SetAnimations(1, 0);
        }
        public override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
        {
            bool isPlayerVisible = true;
            double distanceToPlayer = ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y));
            if (distanceToPlayer > DetectionRange) isPlayerVisible = false;
            double angleToPlayer = Math.Atan2(X - playerX, Y - playerY) - Math.PI;
            if (isPlayerVisible)
            {
                double distance = 0;
                double step = 0.01;
                double rayAngleX = Math.Sin(angleToPlayer);
                double rayAngleY = Math.Cos(angleToPlayer);
                while (distance <= distanceToPlayer)
                {
                    int test_x = (int)(X + rayAngleX * distance);
                    int test_y = (int)(Y + rayAngleY * distance);
                    if (ImpassibleCells.Contains(map[test_y * MapWidth + test_x]))
                    {
                        isPlayerVisible = false;
                        break;
                    }
                    distance += step;
                }
            }
            if (Stage == Stages.Roaming)
            {
                base.UpdateCoordinates(map, playerX, playerY);
                if (isPlayerVisible)
                    Stage = Stages.Chasing;
                return;
            }
            if (Stage == Stages.Chasing)
            {
                if (!isPlayerVisible)
                {
                    Stage = Stages.Roaming;
                    NumberOfMovesLeft = MovesInARow;
                    return;
                }
                double move = this.GetMove();
                double newX = X;
                double newY = Y;
                double tempX = X;
                double tempY = Y;
                A = angleToPlayer;
                if (ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y)) <= EntityWidth) return;
                newX += Math.Sin(A) * move;
                newY += Math.Cos(A) * move;
                IntX = (int)X;
                IntY = (int)Y;
                if (!(ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX + EntityWidth / 2)])
                    || ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX - EntityWidth / 2)])))
                    tempX = newX;
                if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MapWidth + (int)newX])
                    || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MapWidth + (int)newX])))
                    tempY = newY;
                if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX + EntityWidth / 2)]))
                    tempX -= EntityWidth / 2 - (1 - tempX % 1);
                if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX - EntityWidth / 2)]))
                    tempX += EntityWidth / 2 - (tempX % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MapWidth + (int)tempX]))
                    tempY -= EntityWidth / 2 - (1 - tempY % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MapWidth + (int)tempX]))
                    tempY += EntityWidth / 2 - (tempY % 1);
                X = tempX;
                Y = tempY;
            }
        }
    }

    public class Dog : Enemy
    {
        protected override int GetEntityID() => 2;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 10;
        protected override int GetMaxHP() => 5;
        protected override int GetTexture() => Texture;
        public override double GetMove() => 0.125;
        protected override int GetMAX_MONEY() => 15;
        protected override int GetMIN_MONEY() => 10;
        protected override int GetMAX_DAMAGE() => 15;
        protected override int GetMIN_DAMAGE() => 10;

        public Dog(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Dog(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 1;
            Texture = 10;
            DetectionRange = 7;
            Fast = true;
            base.SetAnimations(1, 0);
        }
        public override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
        {
            bool isPlayerVisible = true;
            double distanceToPlayer = ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y));
            if (distanceToPlayer > DetectionRange) isPlayerVisible = false;
            double angleToPlayer = Math.Atan2(X - playerX, Y - playerY) - Math.PI;
            if (isPlayerVisible)
            {
                double distance = 0;
                double step = 0.01;
                double rayAngleX = Math.Sin(angleToPlayer);
                double rayAngleY = Math.Cos(angleToPlayer);
                while (distance <= distanceToPlayer)
                {
                    int test_x = (int)(X + rayAngleX * distance);
                    int test_y = (int)(Y + rayAngleY * distance);
                    if (ImpassibleCells.Contains(map[test_y * MapWidth + test_x]))
                    {
                        isPlayerVisible = false;
                        break;
                    }
                    distance += step;
                }
            }
            if (Stage == Stages.Roaming)
            {
                base.UpdateCoordinates(map, playerX, playerY);
                if (isPlayerVisible)
                    Stage = Stages.Chasing;
                return;
            }
            if (Stage == Stages.Chasing)
            {
                if (!isPlayerVisible)
                {
                    Stage = Stages.Roaming;
                    NumberOfMovesLeft = MovesInARow;
                    return;
                }
                double move = this.GetMove();
                double newX = X;
                double newY = Y;
                double tempX = X;
                double tempY = Y;
                A = angleToPlayer;
                if (ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y)) <= EntityWidth) return;
                newX += Math.Sin(A) * move;
                newY += Math.Cos(A) * move;
                IntX = (int)X;
                IntY = (int)Y;
                if (!(ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX + EntityWidth / 2)])
                    || ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX - EntityWidth / 2)])))
                    tempX = newX;
                if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MapWidth + (int)newX])
                    || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MapWidth + (int)newX])))
                    tempY = newY;
                if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX + EntityWidth / 2)]))
                    tempX -= EntityWidth / 2 - (1 - tempX % 1);
                if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX - EntityWidth / 2)]))
                    tempX += EntityWidth / 2 - (tempX % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MapWidth + (int)tempX]))
                    tempY -= EntityWidth / 2 - (1 - tempY % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MapWidth + (int)tempX]))
                    tempY += EntityWidth / 2 - (tempY % 1);
                X = tempX;
                Y = tempY;
            }
        }
    }

    public class Ogr : Enemy
    {
        protected override int GetEntityID() => 3;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 40;
        protected override int GetMaxHP() => 20;
        protected override int GetTexture() => Texture;
        public override double GetMove() => 0.125;
        protected override int GetMAX_MONEY() => 18;
        protected override int GetMIN_MONEY() => 12;
        protected override int GetMAX_DAMAGE() => 35;
        protected override int GetMIN_DAMAGE() => 25;

        public Ogr(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Ogr(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 2;
            Texture = 11;
            DetectionRange = 8;
            base.SetAnimations(2, 0);
        }
        public override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
        {
            bool isPlayerVisible = true;
            double distanceToPlayer = ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y));
            if (distanceToPlayer > DetectionRange) isPlayerVisible = false;
            double angleToPlayer = Math.Atan2(X - playerX, Y - playerY) - Math.PI;
            if (isPlayerVisible)
            {
                double distance = 0;
                double step = 0.01;
                double rayAngleX = Math.Sin(angleToPlayer);
                double rayAngleY = Math.Cos(angleToPlayer);
                while (distance <= distanceToPlayer)
                {
                    int test_x = (int)(X + rayAngleX * distance);
                    int test_y = (int)(Y + rayAngleY * distance);
                    if (ImpassibleCells.Contains(map[test_y * MapWidth + test_x]))
                    {
                        isPlayerVisible = false;
                        break;
                    }
                    distance += step;
                }
            }
            if (Stage == Stages.Roaming)
            {
                base.UpdateCoordinates(map, playerX, playerY);
                if (isPlayerVisible)
                    Stage = Stages.Chasing;
                return;
            }
            if (Stage == Stages.Chasing)
            {
                if (!isPlayerVisible)
                {
                    Stage = Stages.Roaming;
                    NumberOfMovesLeft = MovesInARow;
                    return;
                }
                double move = this.GetMove();
                double newX = X;
                double newY = Y;
                double tempX = X;
                double tempY = Y;
                A = angleToPlayer;
                if (ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y)) <= EntityWidth) return;
                newX += Math.Sin(A) * move;
                newY += Math.Cos(A) * move;
                IntX = (int)X;
                IntY = (int)Y;
                if (!(ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX + EntityWidth / 2)])
                    || ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX - EntityWidth / 2)])))
                    tempX = newX;
                if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MapWidth + (int)newX])
                    || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MapWidth + (int)newX])))
                    tempY = newY;
                if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX + EntityWidth / 2)]))
                    tempX -= EntityWidth / 2 - (1 - tempX % 1);
                if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX - EntityWidth / 2)]))
                    tempX += EntityWidth / 2 - (tempX % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MapWidth + (int)tempX]))
                    tempY -= EntityWidth / 2 - (1 - tempY % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MapWidth + (int)tempX]))
                    tempY += EntityWidth / 2 - (tempY % 1);
                X = tempX;
                Y = tempY;
            }
        }
    }

    public class Bat : Enemy
    {
        protected override int GetEntityID() => 4;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', 'W', 'S' };
        protected override int GetMovesInARow() => 10;
        protected override int GetMaxHP() => 2;
        protected override int GetTexture() => Texture;
        public override double GetMove() => 0.13;
        protected override int GetMAX_MONEY() => 18;
        protected override int GetMIN_MONEY() => 13;
        protected override int GetMAX_DAMAGE() => 8;
        protected override int GetMIN_DAMAGE() => 3;

        public Bat(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Bat(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 3;
            Texture = 12;
            DetectionRange = 6;
            Fast = true;
            base.SetAnimations(1, 0);
        }
        public override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
        {
            bool isPlayerVisible = true;
            double distanceToPlayer = ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y));
            if (distanceToPlayer > DetectionRange) isPlayerVisible = false;
            double angleToPlayer = Math.Atan2(X - playerX, Y - playerY) - Math.PI;
            if (isPlayerVisible)
            {
                double distance = 0;
                double step = 0.01;
                double rayAngleX = Math.Sin(angleToPlayer);
                double rayAngleY = Math.Cos(angleToPlayer);
                while (distance <= distanceToPlayer)
                {
                    int test_x = (int)(X + rayAngleX * distance);
                    int test_y = (int)(Y + rayAngleY * distance);
                    if (ImpassibleCells.Contains(map[test_y * MapWidth + test_x]))
                    {
                        isPlayerVisible = false;
                        break;
                    }
                    distance += step;
                }
            }
            if (Stage == Stages.Roaming)
            {
                base.UpdateCoordinates(map, playerX, playerY);
                if (isPlayerVisible)
                    Stage = Stages.Chasing;
                return;
            }
            if (Stage == Stages.Chasing)
            {
                if (!isPlayerVisible)
                {
                    Stage = Stages.Roaming;
                    NumberOfMovesLeft = MovesInARow;
                    return;
                }
                double move = this.GetMove();
                double newX = X;
                double newY = Y;
                double tempX = X;
                double tempY = Y;
                A = angleToPlayer;

                if (ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y)) <= EntityWidth) return;
                newX += Math.Sin(A) * move;
                newY += Math.Cos(A) * move;
                IntX = (int)X;
                IntY = (int)Y;
                if (!(ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX + EntityWidth / 2)])
                    || ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX - EntityWidth / 2)])))
                    tempX = newX;
                if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MapWidth + (int)newX])
                    || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MapWidth + (int)newX])))
                    tempY = newY;
                if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX + EntityWidth / 2)]))
                    tempX -= EntityWidth / 2 - (1 - tempX % 1);
                if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX - EntityWidth / 2)]))
                    tempX += EntityWidth / 2 - (tempX % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MapWidth + (int)tempX]))
                    tempY -= EntityWidth / 2 - (1 - tempY % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MapWidth + (int)tempX]))
                    tempY += EntityWidth / 2 - (tempY % 1);
                X = tempX;
                Y = tempY;
            }
        }
    }

    public class Stalker : Enemy
    {
        protected override int GetEntityID() => 25;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 15;
        protected override int GetMaxHP() => 1;
        protected override int GetTexture() => Texture;
        public override double GetMove() => Stage == Stages.Chasing ? 0.2 : 0.35;
        protected override int GetMAX_MONEY() => 1;
        protected override int GetMIN_MONEY() => 0;
        protected override int GetMAX_DAMAGE() => 1000;
        protected override int GetMIN_DAMAGE() => 999;
        private const int TotalLifeTime = 180; //18 sec
        private int RoamingTime = TotalLifeTime * 2, LifeTime = TotalLifeTime;

        public Stalker(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Stalker(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 36;
            MovesInARow = 6;
            DetectionRange = 16;
            base.SetAnimations(2, 0);
        }

        public override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
        {
            bool isPlayerVisible = true;            
            double distanceToPlayer = ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y));
            if (distanceToPlayer > DetectionRange) isPlayerVisible = false;
            double angleToPlayer = Math.Atan2(X - playerX, Y - playerY) - Math.PI;
            if (isPlayerVisible)
            {
                double distance = 0;
                double step = 0.01;
                double rayAngleX = Math.Sin(angleToPlayer);
                double rayAngleY = Math.Cos(angleToPlayer);
                while (distance <= distanceToPlayer)
                {
                    int test_x = (int)(X + rayAngleX * distance);
                    int test_y = (int)(Y + rayAngleY * distance);
                    if (ImpassibleCells.Contains(map[test_y * MapWidth + test_x]))
                    {
                        isPlayerVisible = false;
                        break;
                    }
                    distance += step;
                }
            }
            if (Stage == Stages.Roaming)
            {
                base.UpdateCoordinates(map, playerX, playerY);
                RoamingTime--;
                if (RoamingTime < 0)
                {
                    Dead = true;
                    LifeTime = TotalLifeTime;
                    RoamingTime = TotalLifeTime * 2;
                }
                if (isPlayerVisible) Stage = Stages.Chasing;
                return;
            }
            if (Stage == Stages.Chasing)
            {
                if (!isPlayerVisible)
                {
                    Stage = Stages.Roaming;
                    NumberOfMovesLeft = MovesInARow;
                    return;
                }
                double move = this.GetMove();
                double newX = X;
                double newY = Y;
                double tempX = X;
                double tempY = Y;
                A = angleToPlayer;
                if (ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y)) <= EntityWidth) return;
                newX += Math.Sin(A) * move;
                newY += Math.Cos(A) * move;
                IntX = (int)X;
                IntY = (int)Y;
                if (!(ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX + EntityWidth / 2)])
                    || ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX - EntityWidth / 2)])))
                    tempX = newX;
                if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MapWidth + (int)newX])
                    || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MapWidth + (int)newX])))
                    tempY = newY;
                if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX + EntityWidth / 2)]))
                    tempX -= EntityWidth / 2 - (1 - tempX % 1);
                if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX - EntityWidth / 2)]))
                    tempX += EntityWidth / 2 - (tempX % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MapWidth + (int)tempX]))
                    tempY -= EntityWidth / 2 - (1 - tempY % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MapWidth + (int)tempX]))
                    tempY += EntityWidth / 2 - (tempY % 1);
                X = tempX;
                Y = tempY;
                LifeTime--;
                if (LifeTime < 0)
                {
                    Dead = true;
                    LifeTime = TotalLifeTime;
                    RoamingTime = TotalLifeTime * 2;
                }
            }
        }
    }

    public class VoidStalker : Enemy
    {
        protected override int GetEntityID() => 24;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 10;
        protected override int GetMaxHP() => 1;
        protected override int GetTexture() => Texture;
        public override double GetMove() => 0.13;
        protected override int GetMAX_MONEY() => 1;
        protected override int GetMIN_MONEY() => 0;
        protected override int GetMAX_DAMAGE() => 1000;
        protected override int GetMIN_DAMAGE() => 999;
        private const int TotalPauseTime = 12; //1.2 sec
        private int PauseTime = 1, MovePauseTime = TotalPauseTime;
        private bool IsFirstTime = true;
        public bool PlayerSees = false, DidTP = false;

        public VoidStalker(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public VoidStalker(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 35;
            DetectionRange = 10;
            base.AnimationsToStatic();
        }

        public bool ISeeU()
        {
            PlayerSees = true;
            PauseTime = 1;
            if (!IsFirstTime) return false;
            IsFirstTime = false;
            return true;
        }

        public override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
        {
            if (PlayerSees)
            {
                DidTP = false;
                MovePauseTime = TotalPauseTime;
                PauseTime--;
                if (PauseTime < 0)
                {
                    PlayerSees = false;
                    PauseTime = 1;
                }
                return;
            }
            if (DidTP)
            {
                MovePauseTime--;
                if (MovePauseTime < 0)
                {
                    DidTP = false;
                    MovePauseTime = TotalPauseTime;
                }
                return;
            }
            A = Math.Atan2(playerY - Y, playerX - X);
            double newX = X + (playerX - X) / 2;
            double newY = Y + (playerY - Y) / 2;
            if (!CheckCollision(map, newX, newY))
            {
                X = newX;
                Y = newY;
                DidTP = true;
            }
        }

        private bool CheckCollision(string map, double x, double y)
        {
            return ImpassibleCells.Contains(map[(int)y * MapWidth + (int)(x + EntityWidth / 2)]) ||
                   ImpassibleCells.Contains(map[(int)y * MapWidth + (int)(x - EntityWidth / 2)]) ||
                   ImpassibleCells.Contains(map[(int)(y + EntityWidth / 2) * MapWidth + (int)x]) ||
                   ImpassibleCells.Contains(map[(int)(y - EntityWidth / 2) * MapWidth + (int)x]);
        }
    }

    public class Shooter : RangeEnemy
    {
        protected override int GetEntityID() => 26;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 10;
        protected override int GetMaxHP() => 10;
        protected override int GetTexture() => Texture;
        public override double GetMove() => Stage == Stages.Escaping ? 0.5 : 0.16;
        protected override int GetMAX_MONEY() => 10;
        protected override int GetMIN_MONEY() => 5;
        protected override int GetMAX_DAMAGE() => 15;
        protected override int GetMIN_DAMAGE() => 8;

        public Shooter(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Shooter(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 5;
            Texture = 37;
            DetectionRange = 8;
            SafeDistance = 4.25;
            ShotDistance = 5;
            TotalShotPause = 32; // 3.2 sec
            TotalTimeAfterShot = 2; //0.5 sec
            base.SetAnimations(1, 0);
        }

        public override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
        {
            if (DidShot) return;
            bool isPlayerVisible = true;
            double distanceToPlayer = ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y));
            if (distanceToPlayer > DetectionRange) isPlayerVisible = false;
            double angleToPlayer = Math.Atan2(X - playerX, Y - playerY) - Math.PI;
            if (isPlayerVisible)
            {
                double distance = 0;
                double step = 0.01;
                double rayAngleX = Math.Sin(angleToPlayer);
                double rayAngleY = Math.Cos(angleToPlayer);
                while (distance <= distanceToPlayer)
                {
                    int test_x = (int)(X + rayAngleX * distance);
                    int test_y = (int)(Y + rayAngleY * distance);
                    if (ImpassibleCells.Contains(map[test_y * MapWidth + test_x]))
                    {
                        isPlayerVisible = false;
                        break;
                    }
                    distance += step;
                }
            }
            if (!ShotLogic(isPlayerVisible, distanceToPlayer, angleToPlayer)) return;
            if (Stage == Stages.Roaming)
            {
                base.UpdateCoordinates(map, playerX, playerY);
                if (isPlayerVisible) Stage = Stages.Chasing;
                return;
            }
            if (!isPlayerVisible)
            {
                Stage = Stages.Roaming;
                NumberOfMovesLeft = MovesInARow;
                return;
            }
            if (ReadyToShot) return;
            double move = this.GetMove();
            double newX = X;
            double newY = Y;
            double tempX = X;
            double tempY = Y;
            A = Stage == Stages.Escaping ? ML.NormalizeAngle(angleToPlayer + Math.PI) : angleToPlayer;
            if (ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y)) <= EntityWidth) return;
            newX += Math.Sin(A) * move;
            newY += Math.Cos(A) * move;
            IntX = (int)X;
            IntY = (int)Y;
            if (!(ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX + EntityWidth / 2)])
                || ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX - EntityWidth / 2)])))
                tempX = newX;
            if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MapWidth + (int)newX])
                || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MapWidth + (int)newX])))
                tempY = newY;
            if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX + EntityWidth / 2)]))
                tempX -= EntityWidth / 2 - (1 - tempX % 1);
            if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX - EntityWidth / 2)]))
                tempX += EntityWidth / 2 - (tempX % 1);
            if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MapWidth + (int)tempX]))
                tempY -= EntityWidth / 2 - (1 - tempY % 1);
            if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MapWidth + (int)tempX]))
                tempY += EntityWidth / 2 - (tempY % 1);
            X = tempX;
            Y = tempY;
        }

        protected override bool ShotLogic(bool isPlayerVisible, double distanceToPlayer, double angleToPlayer)
        {
            if (TimeAfterShot > 0) TimeAfterShot--;
            if (isPlayerVisible)
            {
                ShotPause--;
                if (distanceToPlayer <= SafeDistance && !ReadyToShot && ShotPause > 0) Stage = Stages.Escaping;
                else if (distanceToPlayer <= ShotDistance)
                {
                    if (ShotPause <= 0)
                    {
                        if (ReadyToShot)
                        {
                            DidShot = true;
                            ReadyToShot = false;
                            ShotPause = TotalShotPause;
                            TimeAfterShot = TotalTimeAfterShot;
                        }
                        else
                        {
                            ShotA = angleToPlayer;
                            ReadyToShot = true;
                            ShotPause = TotalTimeAfterShot;
                        }
                    }
                    Stage = Stages.Chasing;
                    return false;
                }
                else
                {
                    if (ReadyToShot) ReadyToShot = false;
                    Stage = Stages.Chasing;
                }
            }
            return true;
        }
    }

    public class LostSoul : RangeEnemy
    {
        protected override int GetEntityID() => 27;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', 'W', 'S' };
        protected override int GetMovesInARow() => 10;
        protected override int GetMaxHP() => 3;
        protected override int GetTexture() => Texture;
        public override double GetMove() => Stage == Stages.Escaping ? 0.45 : 0.375;
        protected override int GetMAX_MONEY() => 18;
        protected override int GetMIN_MONEY() => 13;
        protected override int GetMAX_DAMAGE() => 8;
        protected override int GetMIN_DAMAGE() => 3;

        public LostSoul(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public LostSoul(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 6;
            Texture = 41;
            DetectionRange = 11;
            SafeDistance = 10;
            ShotDistance = 5;
            TotalShotPause = 25; // 2.5 sec
            TotalTimeAfterShot = 3; //1.5 sec
            ShotPause = TotalShotPause;
            base.SetAnimations(1, 0);
        }
        public override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
        {
            bool isPlayerVisible = true;
            double distanceToPlayer = ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y));
            if (distanceToPlayer > DetectionRange) isPlayerVisible = false;
            double angleToPlayer = Math.Atan2(X - playerX, Y - playerY) - Math.PI;
            if (isPlayerVisible)
            {
                double distance = 0;
                double step = 0.01;
                double rayAngleX = Math.Sin(angleToPlayer);
                double rayAngleY = Math.Cos(angleToPlayer);
                while (distance <= distanceToPlayer)
                {
                    int test_x = (int)(X + rayAngleX * distance);
                    int test_y = (int)(Y + rayAngleY * distance);
                    if (ImpassibleCells.Contains(map[test_y * MapWidth + test_x]))
                    {
                        isPlayerVisible = false;
                        break;
                    }
                    distance += step;
                }
            }
            if (!ShotLogic(isPlayerVisible, distanceToPlayer, angleToPlayer)) return;
            if (Stage == Stages.Roaming)
            {
                base.UpdateCoordinates(map, playerX, playerY);
                if (isPlayerVisible) Stage = Stages.Chasing;
                return;
            }
            if (!isPlayerVisible)
            {
                Stage = Stages.Roaming;
                NumberOfMovesLeft = MovesInARow;
                return;
            }
            double move = this.GetMove();
            double newX = X;
            double newY = Y;
            double tempX = X;
            double tempY = Y;
            A = Stage == Stages.Escaping ? ML.NormalizeAngle(angleToPlayer + Math.PI) : angleToPlayer;
            if (ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y)) <= EntityWidth) return;
            newX += Math.Sin(A) * move;
            newY += Math.Cos(A) * move;
            IntX = (int)X;
            IntY = (int)Y;
            if (!(ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX + EntityWidth / 2)])
                || ImpassibleCells.Contains(map[(int)newY * MapWidth + (int)(newX - EntityWidth / 2)])))
                tempX = newX;
            if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MapWidth + (int)newX])
                || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MapWidth + (int)newX])))
                tempY = newY;
            if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX + EntityWidth / 2)]))
                tempX -= EntityWidth / 2 - (1 - tempX % 1);
            if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX - EntityWidth / 2)]))
                tempX += EntityWidth / 2 - (tempX % 1);
            if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MapWidth + (int)tempX]))
                tempY -= EntityWidth / 2 - (1 - tempY % 1);
            if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MapWidth + (int)tempX]))
                tempY += EntityWidth / 2 - (tempY % 1);
            X = tempX;
            Y = tempY;
        }

        protected override bool ShotLogic(bool isPlayerVisible, double distanceToPlayer, double angleToPlayer)
        {
            if (TimeAfterShot > 0) TimeAfterShot--;
            ShotPause--;
            if (isPlayerVisible)
            {
                if (ShotPause <= 0)
                {
                    if (distanceToPlayer <= ShotDistance)
                    {
                        ShotA = angleToPlayer;
                        DidShot = true;
                        ShotPause = TotalShotPause;
                        TimeAfterShot = TotalTimeAfterShot;
                        Stage = Stages.Chasing;
                        return false;
                    }
                }
                else if (distanceToPlayer <= SafeDistance)
                {
                    Stage = Stages.Escaping;
                    return true;
                }
            }
            Stage = Stages.Chasing;
            return true;
        }
    }

    public class Vine : Decoration
    {
        public Vine(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Vine(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 18;
            base.AnimationsToStatic();
        }

        protected override int GetEntityID() => 19;
    }

    public class Lamp : Decoration
    {
        public Lamp(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Lamp(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 19;
            base.AnimationsToStatic();
        }

        protected override int GetEntityID() => 20;
    }
}