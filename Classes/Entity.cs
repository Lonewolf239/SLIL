﻿using System;
using System.Linq;
using LiteNetLib.Utils;

namespace SLIL.Classes
{
    public abstract class Entity : INetSerializable
    {
        public int ID { get; set; }
        public int EntityID { get; set; }
        public bool HasAI { get; set; }
        public double A { get; set; }
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
        public bool DEAD { get; set; }
        public int RESPAWN { get; set; }
        public int MAX_MONEY;
        public int MIN_MONEY;
        public int MAX_DAMAGE;
        public int MIN_DAMAGE;
        protected int MAP_WIDTH { get; set; }
        protected int MAX_HP;
        public int DeathSound { get; set; }
        protected const int RESPAWN_TIME = 60;

        protected abstract int GetMAX_HP();
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
            writer.Put(DEAD);
        }

        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
            this.HP = reader.GetDouble();
            this.DEAD = reader.GetBool();
        }

        private void Init(int mapWidth)
        {
            MAX_HP = this.GetMAX_HP();
            MAX_MONEY = this.GetMAX_MONEY();
            MIN_MONEY = this.GetMIN_MONEY();
            MAX_DAMAGE = this.GetMAX_DAMAGE();
            MIN_DAMAGE = this.GetMIN_DAMAGE();
            ImpassibleCells = this.GetImpassibleCells();
            MovesInARow = this.GetMovesInARow();
            NumberOfMovesLeft = MovesInARow;
            HP = MAX_HP;
            A = rand.NextDouble();
            MAP_WIDTH = mapWidth;
            DeathSound = -1;
        }

        public virtual bool DealDamage(double damage)
        {
            HP -= damage;
            if (HP <= 0) Kill();
            return DEAD;
        }

        public void Kill()
        {
            RESPAWN = RESPAWN_TIME;
            DEAD = true;
        }

        public void Respawn()
        {
            HP = MAX_HP;
            DEAD = false;
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
            if (!(ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX + EntityWidth / 2)])
                || ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX - EntityWidth / 2)])))
                tempX = newX;
            if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MAP_WIDTH + (int)newX])
                || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MAP_WIDTH + (int)newX])))
                tempY = newY;
            if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX + EntityWidth / 2)]))
            {
                tempX -= EntityWidth / 2 - (1 - tempX % 1);
                A = rand.NextDouble() * (Math.PI * 2);
                NumberOfMovesLeft = MovesInARow;
            }
            if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX - EntityWidth / 2)]))
            {
                tempX += EntityWidth / 2 - (tempX % 1);
                A = rand.NextDouble() * (Math.PI * 2);
                NumberOfMovesLeft = MovesInARow;
            }
            if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
            {
                tempY -= EntityWidth / 2 - (1 - tempY % 1);
                A = rand.NextDouble() * (Math.PI * 2);
                NumberOfMovesLeft = MovesInARow;
            }
            if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
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
        protected override char[] GetImpassibleCells()
        {
            return new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        }
        protected override int GetMovesInARow() => 0;
        protected override int GetMAX_HP() => 0;
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
        protected override char[] GetImpassibleCells()
        {
            return new char[] { '#', 'D', 'd', '=', 'S' };
        }
        protected override int GetMovesInARow() => 0;
        protected override int GetMAX_HP() => 0;
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
            MAP_WIDTH = mapWidth;
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
                    if (ImpassibleCells.Contains(map[test_y * MAP_WIDTH + test_x]))
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
            if (!(ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX + EntityWidth / 2)])
                || ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX - EntityWidth / 2)])))
                tempX = newX;
            if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MAP_WIDTH + (int)newX])
                || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MAP_WIDTH + (int)newX])))
                tempY = newY;
            if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX + EntityWidth / 2)]))
                tempX -= EntityWidth / 2 - (1 - tempX % 1);
            if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX - EntityWidth / 2)]))
                tempX += EntityWidth / 2 - (tempX % 1);
            if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
                tempY -= EntityWidth / 2 - (1 - tempY % 1);
            if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
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
            MAX_DAMAGE = (int)(MAX_DAMAGE * offset);
            MIN_DAMAGE = (int)(MIN_DAMAGE * offset);
        }
    }

    public abstract class Rockets : NPC
    {
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', 'S' };

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
            if (!(ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX + EntityWidth / 2)])
                || ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX - EntityWidth / 2)])))
                tempX = newX;
            if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MAP_WIDTH + (int)newX])
                || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MAP_WIDTH + (int)newX])))
                tempY = newY;
            if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX + EntityWidth / 2)]))
                tempX -= EntityWidth / 2 - (1 - tempX % 1);
            if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX - EntityWidth / 2)]))
                tempX += EntityWidth / 2 - (tempX % 1);
            if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
                tempY -= EntityWidth / 2 - (1 - tempY % 1);
            if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
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
                DEAD = true;
            return DEAD;
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
        protected override int GetEntityID() => 16;
        protected override double GetEntityWidth() => 0.4;
        public override double GetMove() => 0.6;

        public RpgRocket(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public RpgRocket(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 26;
            base.SetAnimations(1, 0);
        }
    }

    public class Explosion : GameObject
    {
        protected override int GetEntityID() => 17;
        protected override double GetEntityWidth() => 0.4;

        public Explosion(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Explosion(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        public int ShooterID;

        private void Init()
        {
            Texture = 27;
            LifeTime = 0;
            TotalLifeTime = 4;
            Temporarily = true;
            Animated = true;
            base.SetAnimations(2, 2);
        }
    }

    public class PlayerDeadBody : NPC
    {
        public PlayerDeadBody(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public PlayerDeadBody(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            DEAD = true;
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
        protected override int GetMAX_HP() => 10;
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
                    if (ImpassibleCells.Contains(map[test_y * MAP_WIDTH + test_x]))
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
                if (!(ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX + EntityWidth / 2)])
                    || ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX - EntityWidth / 2)])))
                    tempX = newX;
                if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MAP_WIDTH + (int)newX])
                    || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MAP_WIDTH + (int)newX])))
                    tempY = newY;
                if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX + EntityWidth / 2)]))
                    tempX -= EntityWidth / 2 - (1 - tempX % 1);
                if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX - EntityWidth / 2)]))
                    tempX += EntityWidth / 2 - (tempX % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
                    tempY -= EntityWidth / 2 - (1 - tempY % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
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
        protected override int GetMAX_HP() => 5;
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
                    if (ImpassibleCells.Contains(map[test_y * MAP_WIDTH + test_x]))
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
                if (!(ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX + EntityWidth / 2)])
                    || ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX - EntityWidth / 2)])))
                    tempX = newX;
                if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MAP_WIDTH + (int)newX])
                    || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MAP_WIDTH + (int)newX])))
                    tempY = newY;
                if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX + EntityWidth / 2)]))
                    tempX -= EntityWidth / 2 - (1 - tempX % 1);
                if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX - EntityWidth / 2)]))
                    tempX += EntityWidth / 2 - (tempX % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
                    tempY -= EntityWidth / 2 - (1 - tempY % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
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
        protected override int GetMAX_HP() => 20;
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
                    if (ImpassibleCells.Contains(map[test_y * MAP_WIDTH + test_x]))
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
                if (!(ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX + EntityWidth / 2)])
                    || ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX - EntityWidth / 2)])))
                    tempX = newX;
                if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MAP_WIDTH + (int)newX])
                    || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MAP_WIDTH + (int)newX])))
                    tempY = newY;
                if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX + EntityWidth / 2)]))
                    tempX -= EntityWidth / 2 - (1 - tempX % 1);
                if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX - EntityWidth / 2)]))
                    tempX += EntityWidth / 2 - (tempX % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
                    tempY -= EntityWidth / 2 - (1 - tempY % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
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
        protected override int GetMAX_HP() => 2;
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
                    if (ImpassibleCells.Contains(map[test_y * MAP_WIDTH + test_x]))
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
                if (!(ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX + EntityWidth / 2)])
                    || ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX - EntityWidth / 2)])))
                    tempX = newX;
                if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MAP_WIDTH + (int)newX])
                    || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MAP_WIDTH + (int)newX])))
                    tempY = newY;
                if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX + EntityWidth / 2)]))
                    tempX -= EntityWidth / 2 - (1 - tempX % 1);
                if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX - EntityWidth / 2)]))
                    tempX += EntityWidth / 2 - (tempX % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
                    tempY -= EntityWidth / 2 - (1 - tempY % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
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
        protected override int GetMAX_HP() => 1;
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
                    if (ImpassibleCells.Contains(map[test_y * MAP_WIDTH + test_x]))
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
                    DEAD = true;
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
                if (!(ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX + EntityWidth / 2)])
                    || ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX - EntityWidth / 2)])))
                    tempX = newX;
                if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MAP_WIDTH + (int)newX])
                    || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MAP_WIDTH + (int)newX])))
                    tempY = newY;
                if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX + EntityWidth / 2)]))
                    tempX -= EntityWidth / 2 - (1 - tempX % 1);
                if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX - EntityWidth / 2)]))
                    tempX += EntityWidth / 2 - (tempX % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
                    tempY -= EntityWidth / 2 - (1 - tempY % 1);
                if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
                    tempY += EntityWidth / 2 - (tempY % 1);
                X = tempX;
                Y = tempY;
                LifeTime--;
                if (LifeTime < 0)
                {
                    DEAD = true;
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
        protected override int GetMAX_HP() => 1;
        protected override int GetTexture() => Texture;
        public override double GetMove() => 0.13;
        protected override int GetMAX_MONEY() => 1;
        protected override int GetMIN_MONEY() => 0;
        protected override int GetMAX_DAMAGE() => 1000;
        protected override int GetMIN_DAMAGE() => 999;
        private const int TotalPauseTime = 12; //1.2 sec
        private int PauseTime = 1, MovePauseTime = TotalPauseTime;
        public bool PlayerSees = false, DidTP = false;

        public VoidStalker(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public VoidStalker(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            Texture = 35;
            DetectionRange = 10;
            base.AnimationsToStatic();
        }

        public void ISeeU()
        {
            PlayerSees = true;
            PauseTime = 1;
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
            return ImpassibleCells.Contains(map[(int)y * MAP_WIDTH + (int)(x + EntityWidth / 2)]) ||
                   ImpassibleCells.Contains(map[(int)y * MAP_WIDTH + (int)(x - EntityWidth / 2)]) ||
                   ImpassibleCells.Contains(map[(int)(y + EntityWidth / 2) * MAP_WIDTH + (int)x]) ||
                   ImpassibleCells.Contains(map[(int)(y - EntityWidth / 2) * MAP_WIDTH + (int)x]);
        }
    }

    public class Shooter : Enemy
    {
        protected override int GetEntityID() => 25;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells() => new char[] { '#', 'D', 'd', '=', 'W', 'S' };
        protected override int GetMovesInARow() => 10;
        protected override int GetMAX_HP() => 10;
        protected override int GetTexture() => Texture;
        public override double GetMove() => Stage == Stages.Escaping ? 0.5 : 0.16;
        protected override int GetMAX_MONEY() => 10;
        protected override int GetMIN_MONEY() => 5;
        protected override int GetMAX_DAMAGE() => 35;
        protected override int GetMIN_DAMAGE() => 15;
        private const int SafeDistance = 4;
        public const int ShotDistance = 6;
        public const int TotalShotPause = 32; // 3.2 sec
        public int ShotPause = TotalShotPause, TimeAfterShot = 0;
        public double ShotA = 0;
        public bool ReadyToShot = false, DidShot = false;

        public Shooter(double x, double y, int mapWidth, ref int maxEntityID) : base(x, y, mapWidth, ref maxEntityID) => Init();
        public Shooter(double x, double y, int mapWidth, int maxEntityID) : base(x, y, mapWidth, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 0;
            Texture = 37;
            DetectionRange = 7;
            //HasSpriteRotation = true;
            base.SetAnimations(1, 0);
        }

        public override void UpdateCoordinates(string map, double playerX, double playerY, double playerA = 0)
        {
            if (DidShot) return;
            bool isPlayerVisible = true;
            double distanceToPlayer = ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y));
            if (distanceToPlayer > DetectionRange) isPlayerVisible = false;
            A = Math.Atan2(X - playerX, Y - playerY) - Math.PI;
            if (isPlayerVisible)
            {
                double distance = 0;
                double step = 0.01;
                double rayAngleX = Math.Sin(A);
                double rayAngleY = Math.Cos(A);
                while (distance <= distanceToPlayer)
                {
                    int test_x = (int)(X + rayAngleX * distance);
                    int test_y = (int)(Y + rayAngleY * distance);
                    if (ImpassibleCells.Contains(map[test_y * MAP_WIDTH + test_x]))
                    {
                        isPlayerVisible = false;
                        break;
                    }
                    distance += step;
                }
            }
            if (TimeAfterShot > 0) TimeAfterShot--;
            if (isPlayerVisible)
            {
                if (distanceToPlayer <= SafeDistance && !ReadyToShot) Stage = Stages.Escaping;
                else if (distanceToPlayer <= ShotDistance)
                {
                    ShotPause--;
                    if (ShotPause < 0)
                    {
                        if (ReadyToShot)
                        {
                            DidShot = true;
                            ReadyToShot = false;
                            ShotPause = TotalShotPause;
                            TimeAfterShot = 5;
                        }
                        else
                        {
                            ShotA = A;
                            ReadyToShot = true;
                            ShotPause = 5;
                        }
                    }
                    Stage = Stages.Chasing;
                    return;
                }
                else Stage = Stages.Chasing;
            }
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
            A = Stage == Stages.Escaping ? ML.NormalizeAngle(A + Math.PI) : A;
            if (ML.GetDistance(new TPoint(playerX, playerY), new TPoint(X, Y)) <= EntityWidth) return;
            newX += Math.Sin(A) * move;
            newY += Math.Cos(A) * move;
            IntX = (int)X;
            IntY = (int)Y;
            if (!(ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX + EntityWidth / 2)])
                || ImpassibleCells.Contains(map[(int)newY * MAP_WIDTH + (int)(newX - EntityWidth / 2)])))
                tempX = newX;
            if (!(ImpassibleCells.Contains(map[(int)(newY + EntityWidth / 2) * MAP_WIDTH + (int)newX])
                || ImpassibleCells.Contains(map[(int)(newY - EntityWidth / 2) * MAP_WIDTH + (int)newX])))
                tempY = newY;
            if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX + EntityWidth / 2)]))
                tempX -= EntityWidth / 2 - (1 - tempX % 1);
            if (ImpassibleCells.Contains(map[(int)tempY * MAP_WIDTH + (int)(tempX - EntityWidth / 2)]))
                tempX += EntityWidth / 2 - (tempX % 1);
            if (ImpassibleCells.Contains(map[(int)(tempY + EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
                tempY -= EntityWidth / 2 - (1 - tempY % 1);
            if (ImpassibleCells.Contains(map[(int)(tempY - EntityWidth / 2) * MAP_WIDTH + (int)tempX]))
                tempY += EntityWidth / 2 - (tempY % 1);
            X = tempX;
            Y = tempY;
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