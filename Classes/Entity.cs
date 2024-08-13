using System;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace SLIL.Classes
{
    public abstract class Entity
    {
        public double X { get; set; }
        public double Y { get; set; }
        protected double EntityWidth;
        public int IntX { get; set; }
        public int IntY { get; set; }
        public int Texture { get; set; }
        public int[][] Animations { get; set; }
        public bool RespondsToFlashlight { get; set; }
        public int Frames { get; set; }
        protected readonly Random rand = new Random();

        protected abstract int GetTexture();
        protected abstract double GetEntityWidth();
        public virtual int Interaction() => 0;

        public Entity(double x, double y, int map_width)
        {
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

        public Creature(double x, double y, int map_width) : base(x, y, map_width)
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

        public bool DealDamage(double damage)
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
        public Friend(double x, double y, int map_width) : base(x, y, map_width)
        {
            CanHit = false;
        }
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


        public NPC(double x, double y, int map_width) : base(x, y, map_width)
        {
            RespondsToFlashlight = false;
            X += 0.5;
            Y += 0.5;
        }
    }

    public abstract class Pet : Friend
    {
        protected double detectionRange;
        public bool Stoped { get; set; }
        public bool HasStopAnimation { get; set; }
        public Image ShopIcon;
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

        public Pet(double x, double y, int map_width) : base(x, y, map_width)
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

        public GameObject(double x, double y, int map_width) : base(x, y, map_width)
        {
            Temporarily = false;
            RespondsToFlashlight = false;
            TotalLifeTime = Frames;
            Animated = false;
            CurrentFrame = 0;
            X += 0.5;
            Y += 0.5;
        }
    }

    public abstract class Enemy : Creature
    {
        protected enum Stages { Roaming, Chasing };
        protected Stages stage;
        protected double detectionRange;
        public bool Fast { get; set; }

        public Enemy(double x, double y, int map_width) : base(x, y, map_width)
        {
            stage = Stages.Roaming;
            CanHit = true;
            Fast = false;
        }
    }

    public class SillyCat : Pet
    {
        public SillyCat(double x, double y, int map_width) : base(x, y, map_width)
        {
            Index = 0;
            ShopIcon = Properties.Resources.pet_cat_icon;
            Cost = 150;
            Name = new[] { "Глупый Кот", "Silly Cat" };
            Descryption = new[] { "Раз в 5 секунд восстанавливает 2 HP", "Restores 2 HP every 5 seconds" };
            Texture = 17;
            PetAbility = 0;
            AbilityReloadTime = 5;
            HasStopAnimation = true;
            RespondsToFlashlight = true;
            base.SetAnimations(1, 0);
        }

        public override int Interaction() => 1;
    }

    public class GreenGnome : Pet
    {
        public GreenGnome(double x, double y, int map_width) : base(x, y, map_width)
        {
            Index = 1;
            ShopIcon = Properties.Resources.pet_gnome_icon;
            Cost = 60;
            Name = new[] { "Зелёный Гном", "Green Gnome" };
            Descryption = new[] { "Увеличивает максимальное здоровье на 25", "Increases maximum health by 25" };
            Texture = 24;
            PetAbility = 1;
            IsInstantAbility = 1;
            RespondsToFlashlight = true;
            base.SetAnimations(6, 1);
        }

        public override int Interaction() => 2;
    }

    public class EnergyDrink : Pet
    {
        public EnergyDrink(double x, double y, int map_width) : base(x, y, map_width)
        {
            Index = 2;
            ShopIcon = Properties.Resources.pet_energy_drink_icon;
            Cost = 60;
            Name = new[] { "Энергетик", "Energy Drink" };
            Descryption = new[] { "Увеличивает выносливость и скорость", "Increases endurance and speed" };
            Texture = 27;
            PetAbility = 2;
            IsInstantAbility = 1;
            RespondsToFlashlight = false;
            base.AnimationsToStatic();
        }

        public override int Interaction() => 3;
    }

    public class Pyro : Pet
    {
        protected override char[] GetImpassibleCells()
        {
            return new char[] { '#', 'D', 'd' };
        }

        public Pyro(double x, double y, int map_width) : base(x, y, map_width)
        {
            Index = 3;
            ShopIcon = Properties.Resources.pet_pyro_icon;
            Cost = 60;
            Name = new[] { "Подсератель", "Podseratel" };
            Descryption = new[] { "Мир — это сказка...", "The world is a fairy tale..." };
            Texture = 31;
            PetAbility = 3;
            IsInstantAbility = 2;
            AbilityReloadTime = 8;
            RespondsToFlashlight = true;
            base.SetAnimations(1, 0);
        }

        public override int Interaction() => 2;
    }

    public class Teleport : GameObject
    {
        public Teleport(double x, double y, int map_width) : base(x, y, map_width)
        {
            Texture = 34;
            Animated = true;
            base.SetAnimations(1, 0);
        }
    }

    public class HittingTheWall : GameObject
    {
        public HittingTheWall(double x, double y, int map_width) : base(x, y, map_width)
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
        public ShopDoor(double x, double y, int map_width) : base(x, y, map_width)
        {
            Texture = 4;
            base.AnimationsToStatic();
        }
    }

    public class ShopMan : NPC
    {
        public ShopMan(double x, double y, int map_width) : base(x, y, map_width)
        {
            Texture = 21;
            RespondsToFlashlight = true;
            base.AnimationsToStatic();
        }
    }

    public class Man : Enemy
    {
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

        public Man(double x, double y, int map_width) : base (x, y, map_width) 
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
            if(isPlayerVisible)
            {
                double distance = 0;
                double step = 0.01;
                double rayAngleX = Math.Sin(angleToPlayer);
                double rayAngleY = Math.Cos(angleToPlayer);
                while (distance <= distanceToPlayer)
                {
                    int test_x = (int)(X + rayAngleX * distance);
                    int test_y = (int)(Y + rayAngleY * distance);
                    if(ImpassibleCells.Contains(map[test_y * MAP_WIDTH + test_x]))
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
                if(!isPlayerVisible)
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

        public Dog(double x, double y, int map_width) : base(x, y, map_width)
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

        public Abomination(double x, double y, int map_width) : base(x, y, map_width)
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

        public Bat(double x, double y, int map_width) : base(x, y, map_width)
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