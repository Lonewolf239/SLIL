using System;
using System.Linq;
using LiteNetLib.Utils;

namespace SLIL.Classes
{
    internal abstract class Entity : INetSerializable
    {
        internal int ID { get; set; }
        internal int EntityID { get; set; }
        internal bool HasAI { get; set; }
        protected double Angle { get; set; }
        internal double A { get => ML.NormalizeAngle(Angle); set => Angle = value; }
        internal double X { get; set; }
        internal double Y { get; set; }
        internal double EntityWidth;
        internal int IntX { get; set; }
        internal int IntY { get; set; }
        internal double VMove { get; set; }
        internal int Texture { get; set; }
        internal int[] Animations { get; set; }
        internal bool RespondsToFlashlight { get; set; }
        internal int Frames { get; set; }
        internal bool HasStaticAnimation { get; set; }
        internal bool HasSpriteRotation { get; set; }
        protected readonly Random Rand;

        protected abstract int GetEntityID();

        protected abstract int GetTexture();
        protected abstract double GetEntityWidth();
        internal virtual int Interaction() => 0;

        internal Entity(double x, double y, int mapWidth, ref int maxEntityID)
        {
            ID = maxEntityID;
            maxEntityID++;
            EntityID = this.GetEntityID();
            Rand = new Random(Guid.NewGuid().GetHashCode());
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
        internal Entity(double x, double y, int mapWidth, int maxEntityID)
        {
            ID = maxEntityID;
            EntityID = this.GetEntityID();
            Rand = new Random(Guid.NewGuid().GetHashCode());
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

    internal abstract class Creature : Entity
    {
        protected double HP { get; set; }
        protected char[] ImpassibleCells;
        protected int MovesInARow;
        protected int NumberOfMovesLeft;
        internal bool CanHit { get; set; }
        internal bool Dead { get; set; }
        internal int Respawn { get; set; }
        internal int MaxMoney { get; set; }
        internal int MinMoney { get; set; }
        internal int MaxDamage { get; set; }
        internal int MinDamage { get; set; }
        protected int MapWidth { get; set; }
        protected int MaxHP { get; set; }
        internal int DeathSound { get; set; }
        protected const int RespawnTime = 60;

        protected abstract int GetMaxHP();
        protected abstract int GetMAX_MONEY();
        protected abstract int GetMIN_MONEY();
        protected abstract int GetMAX_DAMAGE();
        protected abstract int GetMIN_DAMAGE();
        protected abstract char[] GetImpassibleCells();
        protected abstract int GetMovesInARow();
        internal abstract double GetMove();

        internal Creature(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init(mapWidth);
        internal Creature(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init(mapWidth);

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
            A = Rand.NextDouble();
            MapWidth = mapWidth;
            DeathSound = -1;
        }

        internal virtual bool DealDamage(double damage)
        {
            HP -= damage;
            if (HP <= 0) Kill();
            return Dead;
        }

        internal void Kill()
        {
            Respawn = RespawnTime;
            Dead = true;
        }

        internal void DoRespawn()
        {
            HP = MaxHP;
            Dead = false;
        }

        internal virtual void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
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
                A = Rand.NextDouble() * (Math.PI * 2);
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
                A = Rand.NextDouble() * (Math.PI * 2);
                NumberOfMovesLeft = MovesInARow;
            }
            if (ImpassibleCells.Contains(map[(int)tempY * MapWidth + (int)(tempX - EntityWidth / 2)]))
            {
                tempX += EntityWidth / 2 - (tempX % 1);
                A = Rand.NextDouble() * (Math.PI * 2);
                NumberOfMovesLeft = MovesInARow;
            }
            if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MapWidth + (int)tempX]))
            {
                tempY -= EntityWidth / 2 - (1 - tempY % 1);
                A = Rand.NextDouble() * (Math.PI * 2);
                NumberOfMovesLeft = MovesInARow;
            }
            if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MapWidth + (int)tempX]))
            {
                tempY += EntityWidth / 2 - (tempY % 1);
                A = Rand.NextDouble() * (Math.PI * 2);
                NumberOfMovesLeft = MovesInARow;
            }
            X = tempX;
            Y = tempY;
        }
    }

    internal abstract class Friend : Creature
    {
        internal Friend(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => CanHit = false;
        internal Friend(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => CanHit = false;
    }

    internal abstract class NPC : Friend
    {
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 0;
        protected override int GetMaxHP() => 0;
        protected override int GetTexture() => Texture;
        internal override double GetMove() => 0;
        protected override int GetMAX_MONEY() => 0;
        protected override int GetMIN_MONEY() => 0;
        protected override int GetMAX_DAMAGE() => 0;
        protected override int GetMIN_DAMAGE() => 0;


        internal NPC(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => RespondsToFlashlight = false;
        internal NPC(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => RespondsToFlashlight = false;

        internal override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0) { }
    }

    internal abstract class Pet : Friend
    {
        protected double detectionRange;
        internal bool Stoped { get; set; }
        internal bool HasStopAnimation { get; set; }
        internal string[] Name { get; set; }
        internal string[] Description { get; set; }
        internal int Cost { get; set; }
        internal int Index { get; set; }
        internal bool PetAbilityReloading { get; set; }
        internal int IsInstantAbility { get; set; }
        internal int AbilityReloadTime { get; set; }
        internal int AbilityTimer { get; set; }
        protected int PetAbility { get; set; }
        protected override double GetEntityWidth() => 0.1;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'S' };
        protected override int GetMovesInARow() => 0;
        protected override int GetMaxHP() => 0;
        protected override int GetTexture() => Texture;
        internal override double GetMove() => 0.2;
        protected override int GetMAX_MONEY() => 0;
        protected override int GetMIN_MONEY() => 0;
        protected override int GetMAX_DAMAGE() => 0;
        protected override int GetMIN_DAMAGE() => 0;

        internal Pet(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Pet(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            IsInstantAbility = 0;
            HasStopAnimation = false;
            Stoped = false;
            detectionRange = 8.0;
        }

        internal void SetNewParametrs(double x, double y, int mapWidth)
        {
            X = x;
            Y = y;
            MapWidth = mapWidth;
        }

        internal int GetPetAbility() => PetAbility;

        internal override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
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

    internal abstract class GameObject : Entity
    {
        internal bool Temporarily { get; set; }
        internal int TotalLifeTime { get; set; }
        internal int LifeTime { get; set; }
        internal bool Animated { get; set; }
        internal int CurrentFrame { get; set; }
        protected override double GetEntityWidth() => 0.4;
        protected override int GetTexture() => Texture;

        internal GameObject(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal GameObject(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Temporarily = false;
            RespondsToFlashlight = false;
            TotalLifeTime = Frames;
            Animated = false;
            CurrentFrame = 0;
        }
    }

    internal abstract class Decoration : GameObject
    {
        internal Decoration(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) { }
        internal Decoration(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) { }
    }

    internal abstract class Enemy : Creature
    {
        internal enum Stages { Roaming, Chasing, Escaping };
        internal Stages Stage;
        protected double DetectionRange;
        internal bool Fast { get; set; }

        internal Enemy(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Enemy(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Stage = Stages.Roaming;
            CanHit = true;
            Fast = false;
        }

        internal void SetDamage(double offset)
        {
            MaxDamage = (int)(MaxDamage * offset);
            MinDamage = (int)(MinDamage * offset);
        }
    }

    internal abstract class RangeEnemy : Enemy
    {
        protected double SafeDistance { get; set; }
        internal double ShotDistance { get; set; }
        internal int TotalShotPause { get; set; }
        protected int TotalTimeAfterShot { get; set; }
        internal int ShotPause = 0, TimeAfterShot = 0;
        internal double ShotA = 0;
        internal bool ReadyToShot = false, DidShot = false;

        internal RangeEnemy(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal RangeEnemy(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init() => ShotPause = TotalShotPause;

        protected abstract bool ShotLogic(bool isPlayerVisible, double distanceToPlayer, double angleToPlayer);
    }

    internal abstract class Rockets : NPC
    {
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', 'S' };
        internal abstract char[] GetInpassibleRocketCells();
        internal bool CanHitOnlyPlayer = false;
        internal int ExplosionID { get; set; }

        internal Rockets(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => CanHit = false;
        internal Rockets(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => CanHit = false;

        internal void SetA(double value) => A = value;
        internal override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
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

    internal abstract class Boxes : NPC
    {
        internal bool BoxWithMoney { get; set; }
        internal double MoneyChance { get; set; }

        internal Boxes(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Boxes(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            CanHit = true;
            HP = 2.5;
        }

        internal void SetMoneyChance()
        {
            if (Rand.NextDouble() <= MoneyChance)
                BoxWithMoney = true;
        }

        internal override bool DealDamage(double damage)
        {
            if (!CanHit) return false;
            HP -= damage;
            if (HP <= 0)
                Dead = true;
            return Dead;
        }
    }

    internal abstract class Transport : GameObject
    {
        internal string[] Name { get; set; }
        internal int Index { get; set; }
        internal int Cost { get; set; }
        internal bool CanJump { get; set; }
        internal bool AddToShop { get; set; }
        internal double TransportHP { get; set; } //max: 500
        internal double Speed { get; set; } //max: 7.5
        internal int Controllability { get; set; } //90-175

        internal Transport(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Transport(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init() => A = Rand.NextDouble();

        internal bool DealDamage(double damage)
        {
            TransportHP -= damage;
            if (TransportHP <= 0) return true;
            return false;
        }
    }

    internal abstract class Explosions : GameObject
    {
        internal bool CanBrakeDoors = false;
        internal bool CanHitOnlyPlayer = false;
        internal int MinDamage { get; set; }
        internal int MaxDamage { get; set; }
        internal double HitDistance { get; set; }

        internal Explosions(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Explosions(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            LifeTime = 0;
            TotalLifeTime = 4;
            Temporarily = true;
            Animated = true;
        }
    }

    internal class Covering : GameObject
    {
        internal float HP { get; set; }
        internal bool Broken { get; set; }
        protected override int GetEntityID() => 22;

        internal Covering(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Covering(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 33;
            HP = 100;
            Animated = false;
            Broken = false;
            base.AnimationsToStatic();
        }

        internal void FullRepair()
        {
            HP = 100;
            Broken = false;
        }
        internal void Repair(float value)
        {
            HP += value;
            Broken = false;
            if (HP > 100) HP = 100;
        }
        internal bool DealDamage(float value)
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

    internal class Bike : Transport
    {
        internal Bike(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Bike(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

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
        internal override int Interaction() => 4;
    }

    internal class RpgRocket : Rockets
    {
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'S' };
        internal override char[] GetInpassibleRocketCells() => GetImpassibleCells();
        protected override int GetEntityID() => 16;
        protected override double GetEntityWidth() => 0.25;
        internal override double GetMove() => 0.6;

        internal RpgRocket(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal RpgRocket(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 26;
            ExplosionID = 0;
            base.SetAnimations(1, 0);
        }
    }

    internal class SoulClot : Rockets
    {
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', 'S' };
        internal override char[] GetInpassibleRocketCells() => GetImpassibleCells();
        protected override int GetEntityID() => 28;
        protected override double GetEntityWidth() => 0.25;
        internal override double GetMove() => 0.4;

        internal SoulClot(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal SoulClot(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 42;
            ExplosionID = 1;
            CanHitOnlyPlayer = true;
            base.SetAnimations(1, 0);
        }
    }

    internal class RpgExplosion : Explosions
    {
        protected override int GetEntityID() => 17;
        protected override double GetEntityWidth() => 0.4;

        internal RpgExplosion(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal RpgExplosion(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 27;
            MinDamage = 25;
            MaxDamage = 50;
            HitDistance = 2.66;
            base.SetAnimations(2, 2);
        }
    }

    internal class SoulExplosion : Explosions
    {
        protected override int GetEntityID() => 29;
        protected override double GetEntityWidth() => 0.4;

        internal SoulExplosion(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal SoulExplosion(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

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

    internal class BarrelExplosion : Explosions
    {
        protected override int GetEntityID() => 30;
        protected override double GetEntityWidth() => 0.4;

        internal BarrelExplosion(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal BarrelExplosion(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 27;
            MinDamage = 35;
            MaxDamage = 50;
            HitDistance = 3.15;
            CanBrakeDoors = true;
            base.SetAnimations(2, 2);
        }
    }

    internal class AmmoBox : GameObject
    {
        internal Type WeaponType;

        protected override int GetEntityID() => 31;
        protected override double GetEntityWidth() => 0.4;

        internal AmmoBox(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal AmmoBox(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 44;
            base.AnimationsToStatic();
        }
    }

    internal class PlayerDeadBody : NPC
    {
        internal PlayerDeadBody(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal PlayerDeadBody(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Dead = true;
            Texture = 28;
            base.AnimationsToStatic();
        }
        protected override int GetEntityID() => 13;
    }

    internal class SillyCat : Pet
    {
        internal SillyCat(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal SillyCat(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

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
        internal override int Interaction() => 1;
    }

    internal class GreenGnome : Pet
    {
        internal GreenGnome(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal GreenGnome(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

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
        internal override int Interaction() => 2;
    }

    internal class EnergyDrink : Pet
    {
        internal EnergyDrink(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal EnergyDrink(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

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
        internal override int Interaction() => 3;
    }

    internal class Pyro : Pet
    {
        protected override int GetEntityID() => 8;
        protected override char[] GetImpassibleCells()
        {
            return new char[] { '#', 'D', 'd', 'S' };
        }

        internal Pyro(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Pyro(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

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
        internal override int Interaction() => 2;
    }

    internal class VoidTeleport : GameObject
    {
        protected override int GetEntityID() => 23;

        internal VoidTeleport(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal VoidTeleport(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 34;
            Animated = true;
            base.SetAnimations(1, 0);
        }
    }

    internal class BackroomsTeleport : GameObject
    {
        protected override int GetEntityID() => 21;

        internal BackroomsTeleport(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal BackroomsTeleport(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 32;
            Animated = true;
            base.SetAnimations(1, 0);
        }
    }

    internal class Teleport : GameObject
    {
        protected override int GetEntityID() => 9;

        internal Teleport(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Teleport(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 13;
            Animated = true;
            base.SetAnimations(1, 0);
        }
    }

    internal class Box : Boxes
    {
        protected override int GetEntityID() => 14;

        internal Box(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Box(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 16;
            DeathSound = 4;
            MoneyChance = 0.25;
            SetMoneyChance();
            base.AnimationsToStatic();
        }
    }

    internal class Barrel : Boxes
    {
        protected override int GetEntityID() => 15;

        internal Barrel(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Barrel(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 17;
            DeathSound = 4;
            MoneyChance = 0.75;
            SetMoneyChance();
            base.AnimationsToStatic();
        }
    }

    internal class ExplodingBarrel : Boxes
    {
        protected override int GetEntityID() => 29;

        internal ExplodingBarrel(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal ExplodingBarrel(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 43;
            DeathSound = -1;
            base.SetAnimations(1, 0);
        }
    }

    internal class HittingTheWall : GameObject
    {
        protected override int GetEntityID() => 10;
        internal HittingTheWall(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal HittingTheWall(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

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

    internal class ShopDoor : GameObject
    {
        protected override int GetEntityID() => 11;
        internal ShopDoor(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal ShopDoor(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 6;
            base.AnimationsToStatic();
        }

        internal override int Interaction() => 5;
    }

    internal class ShopMan : NPC
    {
        protected override int GetEntityID() => 12;

        internal ShopMan(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal ShopMan(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 14;
            RespondsToFlashlight = true;
            base.AnimationsToStatic();
        }

        internal override int Interaction() => 5;
    }

    internal class Zombie : Enemy
    {
        protected override int GetEntityID() => 1;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 10;
        protected override int GetMaxHP() => 10;
        protected override int GetTexture() => Texture;
        internal override double GetMove() => 0.16;
        protected override int GetMAX_MONEY() => 10;
        protected override int GetMIN_MONEY() => 5;
        protected override int GetMAX_DAMAGE() => 35;
        protected override int GetMIN_DAMAGE() => 15;

        internal Zombie(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Zombie(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 0;
            Texture = 9;
            DetectionRange = 5;
            //HasSpriteRotation = true;
            base.SetAnimations(1, 0);
        }
        internal override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
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

    internal class Dog : Enemy
    {
        protected override int GetEntityID() => 2;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 10;
        protected override int GetMaxHP() => 5;
        protected override int GetTexture() => Texture;
        internal override double GetMove() => 0.125;
        protected override int GetMAX_MONEY() => 15;
        protected override int GetMIN_MONEY() => 10;
        protected override int GetMAX_DAMAGE() => 15;
        protected override int GetMIN_DAMAGE() => 10;

        internal Dog(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Dog(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 1;
            Texture = 10;
            DetectionRange = 7;
            Fast = true;
            base.SetAnimations(1, 0);
        }
        internal override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
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

    internal class Ogr : Enemy
    {
        protected override int GetEntityID() => 3;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 40;
        protected override int GetMaxHP() => 20;
        protected override int GetTexture() => Texture;
        internal override double GetMove() => 0.125;
        protected override int GetMAX_MONEY() => 18;
        protected override int GetMIN_MONEY() => 12;
        protected override int GetMAX_DAMAGE() => 35;
        protected override int GetMIN_DAMAGE() => 25;

        internal Ogr(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Ogr(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 2;
            Texture = 11;
            DetectionRange = 8;
            base.SetAnimations(2, 0);
        }
        internal override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
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

    internal class Bat : Enemy
    {
        protected override int GetEntityID() => 4;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', 'W', 'S' };
        protected override int GetMovesInARow() => 10;
        protected override int GetMaxHP() => 2;
        protected override int GetTexture() => Texture;
        internal override double GetMove() => 0.13;
        protected override int GetMAX_MONEY() => 18;
        protected override int GetMIN_MONEY() => 13;
        protected override int GetMAX_DAMAGE() => 8;
        protected override int GetMIN_DAMAGE() => 3;

        internal Bat(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Bat(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 3;
            Texture = 12;
            DetectionRange = 6;
            Fast = true;
            base.SetAnimations(1, 0);
        }
        internal override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
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

    internal class Stalker : Enemy
    {
        protected override int GetEntityID() => 25;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 15;
        protected override int GetMaxHP() => 1;
        protected override int GetTexture() => Texture;
        internal override double GetMove() => Stage == Stages.Chasing ? 0.2 : 0.35;
        protected override int GetMAX_MONEY() => 1;
        protected override int GetMIN_MONEY() => 0;
        protected override int GetMAX_DAMAGE() => 1000;
        protected override int GetMIN_DAMAGE() => 999;
        private const int TotalLifeTime = 180; //18 sec
        private int RoamingTime = TotalLifeTime * 2, LifeTime = TotalLifeTime;

        internal Stalker(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Stalker(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 36;
            MovesInARow = 6;
            DetectionRange = 16;
            base.SetAnimations(2, 0);
        }

        internal override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
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

    internal class VoidStalker : Enemy
    {
        protected override int GetEntityID() => 24;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 10;
        protected override int GetMaxHP() => 1;
        protected override int GetTexture() => Texture;
        internal override double GetMove() => 0.13;
        protected override int GetMAX_MONEY() => 1;
        protected override int GetMIN_MONEY() => 0;
        protected override int GetMAX_DAMAGE() => 1000;
        protected override int GetMIN_DAMAGE() => 999;
        private const int TotalPauseTime = 12; //1.2 sec
        private int PauseTime = 1, MovePauseTime = TotalPauseTime;
        private bool IsFirstTime = true;
        internal bool PlayerSees = false, DidTP = false;

        internal VoidStalker(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal VoidStalker(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 35;
            DetectionRange = 10;
            base.AnimationsToStatic();
        }

        internal bool ISeeU()
        {
            PlayerSees = true;
            PauseTime = 1;
            if (!IsFirstTime) return false;
            IsFirstTime = false;
            return true;
        }

        internal override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
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

    internal class Shooter : RangeEnemy
    {
        protected override int GetEntityID() => 26;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 10;
        protected override int GetMaxHP() => 10;
        protected override int GetTexture() => Texture;
        internal override double GetMove() => Stage == Stages.Escaping ? 0.5 : 0.16;
        protected override int GetMAX_MONEY() => 10;
        protected override int GetMIN_MONEY() => 5;
        protected override int GetMAX_DAMAGE() => 15;
        protected override int GetMIN_DAMAGE() => 8;

        internal Shooter(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Shooter(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

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

        internal override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
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

    internal class LostSoul : RangeEnemy
    {
        protected override int GetEntityID() => 27;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', 'W', 'S' };
        protected override int GetMovesInARow() => 10;
        protected override int GetMaxHP() => 3;
        protected override int GetTexture() => Texture;
        internal override double GetMove() => Stage == Stages.Escaping ? 0.45 : 0.375;
        protected override int GetMAX_MONEY() => 18;
        protected override int GetMIN_MONEY() => 13;
        protected override int GetMAX_DAMAGE() => 8;
        protected override int GetMIN_DAMAGE() => 3;

        internal LostSoul(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal LostSoul(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

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
        internal override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
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

    internal class Vine : Decoration
    {
        internal Vine(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Vine(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 18;
            base.AnimationsToStatic();
        }

        protected override int GetEntityID() => 19;
    }

    internal class Lamp : Decoration
    {
        internal Lamp(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        internal Lamp(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 19;
            base.AnimationsToStatic();
        }

        protected override int GetEntityID() => 20;
    }
}