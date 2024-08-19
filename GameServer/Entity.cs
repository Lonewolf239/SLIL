using LiteNetLib.Utils;
using System;
using System.Drawing;
using System.Linq;

namespace SLIL.Classes
{
    public abstract class Entity : INetSerializable
    {
        public int ID { get; set; }
        public int EntityID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        protected double EntityWidth;
        public int IntX { get; set; }
        public int IntY { get; set; }
        public int Texture { get; set; }
        public int[][] Animations { get; set; }
        public bool RespondsToFlashlight { get; set; }
        public int Frames { get; set; }
        protected readonly Random rand;

        protected abstract int GetEntityID();

        protected abstract int GetTexture();
        protected abstract double GetEntityWidth();
        public virtual int Interaction() => 0;

        public Entity(double x, double y, int map_width, ref int maxEntityID)
        {
            ID = maxEntityID;
            maxEntityID++;
            EntityID = this.GetEntityID();
            rand = new Random();
            Frames = 24;
            Texture = this.GetTexture();
            EntityWidth = this.GetEntityWidth();
            RespondsToFlashlight = false;
            Texture = this.GetTexture();
            X = x;
            Y = y;
            IntX = (int)x;
            IntY = (int)y;
        }
        public Entity(double x, double y, int map_width, int maxEntityID)
        {
            ID = maxEntityID;
            EntityID = this.GetEntityID();
            rand = new Random();
            Frames = 24;
            Texture = this.GetTexture();
            EntityWidth = this.GetEntityWidth();
            RespondsToFlashlight = false;
            Texture = this.GetTexture();
            X = x;
            Y = y;
            IntX = (int)x;
            IntY = (int)y;
        }

        public virtual void Serialize(NetDataWriter writer)
        {
            writer.Put(this.X);
            writer.Put(this.Y);
        }

        public virtual void Deserialize(NetDataReader reader)
        {
            this.X = reader.GetDouble();
            this.Y = reader.GetDouble();
        }

        protected void AnimationsToStatic()
        {
            Animations = new int[1][];
            Animations[0] = new int[Frames];
            for (int item = 0; item < Frames; item++)
                Animations[0][item] = Texture;
        }

        protected void SetAnimations(int pause, int mode)
        {
            Animations = new int[1][];
            Animations[0] = new int[Frames];
            int state = 0;
            for (int item = 0; item < Frames; item++)
            {
                if (mode == 1)
                {
                    if (item % pause == 0)
                        Animations[0][item] = Texture + 1;
                    else
                        Animations[0][item] = Texture;
                }
                else if (mode == 2)
                {
                    if (item >= pause)
                        Animations[0][item] = Texture + 1;
                    else
                        Animations[0][item] = Texture;
                }
                else
                {
                    if (item % pause == 0) state = state == 1 ? 0 : 1;
                    Animations[0][item] = Texture + state;
                }
            }
        }
    }

    public abstract class Creature : Entity
    {
        protected double HP { get; set; }
        public double A { get; set; }
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
        protected abstract double GetMove();

        public Creature(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init(map_width);
        public Creature(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init(map_width);

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

        private void Init(int map_width)
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
            MAP_WIDTH = map_width;
            DeathSound = -1;
        }

        public virtual bool DealDamage(double damage)
        {
            HP -= damage;
            if (HP <= 0)
                Kill();
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

        public virtual void UpdateCoordinates(string map, double playerX, double playerY)
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
        public Friend(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => CanHit = false;
        public Friend(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => CanHit = false;
    }

    public abstract class NPC : Friend
    {
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells()
        {
            return new char[] { '#', 'D', 'd', '=', 'W' };
        }
        protected override int GetMovesInARow() => 0;
        protected override int GetMAX_HP() => 0;
        protected override int GetTexture() => Texture;
        protected override double GetMove() => 0;
        protected override int GetMAX_MONEY() => 0;
        protected override int GetMIN_MONEY() => 0;
        protected override int GetMAX_DAMAGE() => 0;
        protected override int GetMIN_DAMAGE() => 0;


        public NPC(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => RespondsToFlashlight = false;
        public NPC(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => RespondsToFlashlight = false;

        public override void UpdateCoordinates(string map, double playerX, double playerY) { }
    }

    public abstract class Pet : Friend
    {
        protected double detectionRange;
        public bool Stoped { get; set; }
        public bool HasStopAnimation { get; set; }
        public string[] Name { get; set; }
        public string[] Descryption { get; set; }
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
            return new char[] { '#', 'D', 'd', '=' };
        }
        protected override int GetMovesInARow() => 0;
        protected override int GetMAX_HP() => 0;
        protected override int GetTexture() => Texture;
        protected override double GetMove() => 0.2;
        protected override int GetMAX_MONEY() => 0;
        protected override int GetMIN_MONEY() => 0;
        protected override int GetMAX_DAMAGE() => 0;
        protected override int GetMIN_DAMAGE() => 0;

        public Pet(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public Pet(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            IsInstantAbility = 0;
            HasStopAnimation = false;
            Stoped = false;
            detectionRange = 8.0;
        }

        public void SetNewParametrs(double x, double y, int map_width)
        {
            X = x;
            Y = y;
            MAP_WIDTH = map_width;
        }

        public int GetPetAbility() => PetAbility;

        public override void UpdateCoordinates(string map, double playerX, double playerY)
        {
            Stoped = false;
            bool isPlayerVisible = true;
            double distanceToPlayer = Math.Sqrt(Math.Pow(X - playerX, 2) + Math.Pow(Y - playerY, 2));
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
            if (Math.Sqrt(Math.Pow(X - playerX, 2) + Math.Pow(Y - playerY, 2)) <= EntityWidth) return;
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
                if (Math.Sqrt(Math.Pow(tempX - playerX, 2) + Math.Pow(tempY - playerY, 2)) >= 0.75)
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

        public GameObject(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public GameObject(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            Temporarily = false;
            RespondsToFlashlight = false;
            TotalLifeTime = Frames;
            Animated = false;
            CurrentFrame = 0;
        }
    }

    public abstract class Enemy : Creature
    {
        protected enum Stages { Roaming, Chasing };
        protected Stages stage;
        protected double detectionRange;
        public bool Fast { get; set; }

        public Enemy(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public Enemy(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            stage = Stages.Roaming;
            CanHit = true;
            Fast = false;
        }
    }

    public class PlayerDeadBody : GameObject
    {
        public PlayerDeadBody(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public PlayerDeadBody(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            Texture = 43;
            base.AnimationsToStatic();
        }
        protected override int GetEntityID() => 13;
    }

    public class SillyCat : Pet
    {
        public SillyCat(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public SillyCat(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            Index = 0;
            Cost = 150;
            Name = new[] { "Глупый Кот", "Silly Cat" };
            Descryption = new[] { "Раз в 8 секунд восстанавливает 2 HP", "Restores 2 HP every 8 seconds" };
            Texture = 17;
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
        public GreenGnome(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public GreenGnome(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            Index = 1;
            Cost = 60;
            Name = new[] { "Зелёный Гном", "Green Gnome" };
            Descryption = new[] { "Увеличивает максимальное здоровье на 25", "Increases maximum health by 25" };
            Texture = 24;
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
        public EnergyDrink(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public EnergyDrink(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            Index = 2;
            Cost = 60;
            Name = new[] { "Энергетик", "Energy Drink" };
            Descryption = new[] { "Увеличивает выносливость и скорость", "Increases endurance and speed" };
            Texture = 27;
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
            return new char[] { '#', 'D', 'd' };
        }

        public Pyro(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public Pyro(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            Index = 3;
            Cost = 666;
            Name = new[] { "Подсератель", "Podseratel" };
            Descryption = new[] { "Мир — это сказка...", "The world is a fairy tale..." };
            Texture = 31;
            PetAbility = 3;
            IsInstantAbility = 2;
            AbilityReloadTime = 15;
            RespondsToFlashlight = true;
            base.SetAnimations(1, 0);
        }

        public override int Interaction() => 2;
    }

    public class Teleport : GameObject
    {
        protected override int GetEntityID() => 9;

        public Teleport(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public Teleport(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            Texture = 34;
            Animated = true;
            base.SetAnimations(1, 0);
        }
    }

    public abstract class Boxes : NPC
    {
        public bool BoxWithMoney { get; set; }

        public Boxes(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public Boxes(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            if (rand.NextDouble() <= 0.25) BoxWithMoney = true;
            CanHit = true;
            HP = 2.5;
        }

        public void BreakTheBox()
        {
            CanHit = false;
            Texture++;
            base.AnimationsToStatic();
        }

        public override bool DealDamage(double damage)
        {
            HP -= damage;
            if (HP <= 0)
            {
                BreakTheBox();
                return true;
            }
            return false;
        }
    }

    public class Box : Boxes
    {
        protected override int GetEntityID() => 14;

        public Box(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public Box(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            Texture = 44;
            DeathSound = 4;
            base.AnimationsToStatic();
        }
    }

    public class Barrel : Boxes
    {
        protected override int GetEntityID() => 15;

        public Barrel(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public Barrel(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            Texture = 46;
            DeathSound = 4;
            base.AnimationsToStatic();
        }
    }

    public class HittingTheWall : GameObject
    {
        protected override int GetEntityID() => 10;
        public HittingTheWall(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public HittingTheWall(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            Texture = 36;
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
        public ShopDoor(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public ShopDoor(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            Texture = 4;
            base.AnimationsToStatic();
        }
    }

    public class ShopMan : NPC
    {
        protected override int GetEntityID() => 12;

        public ShopMan(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public ShopMan(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            Texture = 21;
            RespondsToFlashlight = true;
            base.AnimationsToStatic();
        }
    }

    public class Man : Enemy
    {
        protected override int GetEntityID() => 1;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells()
        {
            return new char[] { '#', 'D', 'd', '=', 'W' };
        }
        protected override int GetMovesInARow() => 10;
        protected override int GetMAX_HP() => 10;
        protected override int GetTexture() => Texture;
        protected override double GetMove() => 0.16;
        protected override int GetMAX_MONEY() => 10;
        protected override int GetMIN_MONEY() => 5;
        protected override int GetMAX_DAMAGE() => 35;
        protected override int GetMIN_DAMAGE() => 15;

        public Man(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public Man(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 0;
            Texture = 8;
            detectionRange = 8;
            base.SetAnimations(1, 0);
        }
        public override void UpdateCoordinates(string map, double playerX, double playerY)
        {
            bool isPlayerVisible = true;
            double distanceToPlayer = Math.Sqrt(Math.Pow(X - playerX, 2) + Math.Pow(Y - playerY, 2));
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
                    if (ImpassibleCells.Contains(map[test_y * MAP_WIDTH + test_x]))
                    {
                        isPlayerVisible = false;
                        break;
                    }
                    distance += step;
                }
            }
            if (stage == Stages.Roaming)
            {
                base.UpdateCoordinates(map, playerX, playerY);
                if (isPlayerVisible)
                    stage = Stages.Chasing;
                return;
            }
            if (stage == Stages.Chasing)
            {
                if (!isPlayerVisible)
                {
                    stage = Stages.Roaming;
                    NumberOfMovesLeft = MovesInARow;
                    return;
                }
                double move = this.GetMove();
                double newX = X;
                double newY = Y;
                double tempX = X;
                double tempY = Y;
                A = angleToPlayer;
                if (Math.Sqrt(Math.Pow(X - playerX, 2) + Math.Pow(Y - playerY, 2)) <= EntityWidth) return;
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
        protected override char[] GetImpassibleCells()
        {
            return new char[] { '#', 'D', 'd', '=', 'W' };
        }
        protected override int GetMovesInARow() => 10;
        protected override int GetMAX_HP() => 5;
        protected override int GetTexture() => Texture;
        protected override double GetMove() => 0.125;
        protected override int GetMAX_MONEY() => 15;
        protected override int GetMIN_MONEY() => 10;
        protected override int GetMAX_DAMAGE() => 40;
        protected override int GetMIN_DAMAGE() => 25;

        public Dog(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public Dog(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 1;
            Texture = 11;
            detectionRange = 8;
            Fast = true;
            base.SetAnimations(1, 0);
        }
        public override void UpdateCoordinates(string map, double playerX, double playerY)
        {
            bool isPlayerVisible = true;
            double distanceToPlayer = Math.Sqrt(Math.Pow(X - playerX, 2) + Math.Pow(Y - playerY, 2));
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
                    if (ImpassibleCells.Contains(map[test_y * MAP_WIDTH + test_x]))
                    {
                        isPlayerVisible = false;
                        break;
                    }
                    distance += step;
                }
            }
            if (stage == Stages.Roaming)
            {
                base.UpdateCoordinates(map, playerX, playerY);
                if (isPlayerVisible)
                    stage = Stages.Chasing;
                return;
            }
            if (stage == Stages.Chasing)
            {
                if (!isPlayerVisible)
                {
                    stage = Stages.Roaming;
                    NumberOfMovesLeft = MovesInARow;
                    return;
                }
                double move = this.GetMove();
                double newX = X;
                double newY = Y;
                double tempX = X;
                double tempY = Y;
                A = angleToPlayer;
                if (Math.Sqrt(Math.Pow(X - playerX, 2) + Math.Pow(Y - playerY, 2)) <= EntityWidth) return;
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

    public class Abomination : Enemy
    {
        protected override int GetEntityID() => 3;
        protected override double GetEntityWidth() => 0.4;
        protected override char[] GetImpassibleCells()
        {
            return new char[] { '#', 'D', 'd', '=', 'W' };
        }
        protected override int GetMovesInARow() => 40;
        protected override int GetMAX_HP() => 20;
        protected override int GetTexture() => Texture;
        protected override double GetMove() => 0.125;
        protected override int GetMAX_MONEY() => 18;
        protected override int GetMIN_MONEY() => 12;
        protected override int GetMAX_DAMAGE() => 30;
        protected override int GetMIN_DAMAGE() => 20;

        public Abomination(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public Abomination(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 2;
            Texture = 14;
            detectionRange = 8;
            base.SetAnimations(2, 0);
        }
        public override void UpdateCoordinates(string map, double playerX, double playerY)
        {
            bool isPlayerVisible = true;
            double distanceToPlayer = Math.Sqrt(Math.Pow(X - playerX, 2) + Math.Pow(Y - playerY, 2));
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
                    if (ImpassibleCells.Contains(map[test_y * MAP_WIDTH + test_x]))
                    {
                        isPlayerVisible = false;
                        break;
                    }
                    distance += step;
                }
            }
            if (stage == Stages.Roaming)
            {
                base.UpdateCoordinates(map, playerX, playerY);
                if (isPlayerVisible)
                    stage = Stages.Chasing;
                return;
            }
            if (stage == Stages.Chasing)
            {
                if (!isPlayerVisible)
                {
                    stage = Stages.Roaming;
                    NumberOfMovesLeft = MovesInARow;
                    return;
                }
                double move = this.GetMove();
                double newX = X;
                double newY = Y;
                double tempX = X;
                double tempY = Y;
                A = angleToPlayer;
                if (Math.Sqrt(Math.Pow(X - playerX, 2) + Math.Pow(Y - playerY, 2)) <= EntityWidth) return;
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
        protected override char[] GetImpassibleCells()
        {
            return new char[] { '#', 'D', 'd', 'W' };
        }
        protected override int GetMovesInARow() => 10;
        protected override int GetMAX_HP() => 2;
        protected override int GetTexture() => Texture;
        protected override double GetMove() => 0.13;
        protected override int GetMAX_MONEY() => 18;
        protected override int GetMIN_MONEY() => 13;
        protected override int GetMAX_DAMAGE() => 30;
        protected override int GetMIN_DAMAGE() => 22;

        public Bat(double x, double y, int map_width, ref int maxEntityID) : base(x, y, map_width, ref maxEntityID) => Init();
        public Bat(double x, double y, int map_width, int maxEntityID) : base(x, y, map_width, maxEntityID) => Init();

        private void Init()
        {
            DeathSound = 3;
            Texture = 28;
            detectionRange = 8;
            Fast = true;
            base.SetAnimations(1, 0);
        }
        public override void UpdateCoordinates(string map, double playerX, double playerY)
        {
            bool isPlayerVisible = true;
            double distanceToPlayer = Math.Sqrt(Math.Pow(X - playerX, 2) + Math.Pow(Y - playerY, 2));
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
                    if (ImpassibleCells.Contains(map[test_y * MAP_WIDTH + test_x]))
                    {
                        isPlayerVisible = false;
                        break;
                    }
                    distance += step;
                }
            }
            if (stage == Stages.Roaming)
            {
                base.UpdateCoordinates(map, playerX, playerY);
                if (isPlayerVisible)
                    stage = Stages.Chasing;
                return;
            }
            if (stage == Stages.Chasing)
            {
                if (!isPlayerVisible)
                {
                    stage = Stages.Roaming;
                    NumberOfMovesLeft = MovesInARow;
                    return;
                }
                double move = this.GetMove();
                double newX = X;
                double newY = Y;
                double tempX = X;
                double tempY = Y;
                A = angleToPlayer;
                if (Math.Sqrt(Math.Pow(X - playerX, 2) + Math.Pow(Y - playerY, 2)) <= EntityWidth) return;
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
}