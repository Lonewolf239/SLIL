﻿using MazeGenerator;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLIL.Classes
{
    internal class GameModel : INetSerializable
    {
        private StringBuilder MAP = new StringBuilder();
        private const string bossMap = @"#########################...............##F###.................####..##...........##..###...=...........=...###...=.....E.....=...###...................###...................###.........#.........###...##.........##...###....#.........#....###...................###..#...##.#.##...#..####.....#.....#.....######...............##############d####################...#################E=...=E#################...#################$D.P.D$#################...################################",
            debugMap = @"####################.................##.................##..1..2..3..4..#..##.................##..b..............##..............d..##..B..............##.................##........P.....=..##..#b.............##..###............##..#B..........F..##.................##..WWW.B=5#D#..#..##..WEW====#$#.#d=.##..WWW.=b.###..=..##.................####################",
            bikeMap = @"############################......######..555..#####........####.........###.......................##.......................##....####......####.....##...######....######....##...######====#dd###....##...##$###....#dd###....##...##D###....######....##...##.b##.....####.....##WWW##..##..............##EEE#F...d..............##WWW##..##..............##...##.B##.....####.....##...##D###....######....##...##$###....###dd#====##...######....###dd#....##...######....######....##....####......####.....##.......................##.......................###........####.......P.#####......######..555..############################";
        private int inDebug = 0;
        private readonly Pet[] PETS;
        public List<Entity> Entities = new List<Entity>();
        private double EnemyDamageOffset = 1;
        private const double playerWidth = 0.4;
        private bool GameStarted = false;
        private readonly Random rand;
        private int difficulty;
        private int MAP_WIDTH, MAP_HEIGHT;
        private bool CUSTOM = false;
        private int CustomMazeHeight, CustomMazeWidth;
        private StringBuilder CUSTOM_MAP = new StringBuilder();
        private double CUSTOM_X, CUSTOM_Y;
        private readonly StopGameDelegate StopGameHandle;
        private readonly PlaySoundDelegate PlaySoundHandle;
        private readonly SetPlayerIDDelegate SetPlayerID;
        public bool IsMultiplayer;
        private static System.Windows.Forms.Timer RespawnTimer;
        private static System.Windows.Forms.Timer EnemyTimer;
        private static System.Windows.Forms.Timer TimeRemain;
        public int MaxEntityID;
        
        public GameModel(StopGameDelegate stopGame, SetPlayerIDDelegate setPlayerID, PlaySoundDelegate playSound)
        {
            StopGameHandle = stopGame;
            SetPlayerID = setPlayerID;
            Pet[] pets = { new SillyCat(0, 0, 0, 0), new GreenGnome(0, 0, 0, 0), new EnergyDrink(0, 0, 0, 0), new Pyro(0, 0, 0, 0) };
            PETS = pets;
            MAP_WIDTH = 16;
            MAP_HEIGHT = 16;
            rand = new Random();
            difficulty = MainMenu.difficulty;
            RespawnTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            RespawnTimer.Tick += RespawnTimer_Tick;
            EnemyTimer = new System.Windows.Forms.Timer
            {
                Interval = 100
            };
            EnemyTimer.Tick += EnemyTimer_Tick;
            TimeRemain = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            TimeRemain.Tick += TimeRemain_Tick;
            PlaySoundHandle = playSound;
        }

        public void Pause(bool paused)
        {
            if (paused)
            {
                TimeRemain.Stop();
                RespawnTimer.Stop();
                EnemyTimer.Stop();
            }
            else
            {
                TimeRemain.Start();
                RespawnTimer.Start();
                EnemyTimer.Start();
            }
        }

        public void StartGame(bool startTimers)
        {
            if (inDebug == 0) difficulty = SLIL.difficulty;
            else difficulty = 5;
            if (Entities.Count == 0) SetPlayerID(AddPlayer());
            if (MAP.Length == 0) InitMap();
            GameStarted = true;
            if (startTimers && !IsMultiplayer)
            {
                RespawnTimer.Start();
                EnemyTimer.Start();
                TimeRemain.Start();
            }
        }

        public bool IsGameStarted() => GameStarted;

        public int AddPlayer()
        {
            Entities.Add(new Player(3, 3, MAP_WIDTH, ref MaxEntityID));
            return MaxEntityID - 1;
        }

        private void EnemyTimer_Tick(object sender, EventArgs e)
        {
            List<Player> playersList = new List<Player>();
            foreach (Entity ent in Entities)
            {
                if (ent is Player) playersList.Add(ent as Player);
            }
            if (playersList.Count == 0) return;
            for (int i = 0; i < Entities.Count; i++)
            {
                if (GameStarted)
                {
                    if (Entities[i] is Player) continue;
                    if (!Entities[i].HasAI) continue;
                    var entity = Entities[i] as dynamic;
                    var playerListOrdered = playersList.OrderBy((playerI) => Math.Pow(entity.X - playerI.X, 2) + Math.Pow(entity.Y - playerI.Y, 2));
                    Player player = playerListOrdered.First();
                    double distance = Math.Sqrt(Math.Pow(entity.X - player.X, 2) + Math.Pow(entity.Y - player.Y, 2));
                    if (entity is GameObject && entity.Temporarily)
                    {
                        entity.LifeTime++;
                        entity.CurrentFrame++;
                        if (entity.LifeTime >= entity.TotalLifeTime)
                        {
                            if (entity is Explosion explosion)
                            {
                                foreach (Entity ent in Entities)
                                {
                                    if (ent is NPC || ent is Player || ent is Enemy)
                                    {
                                        if (ent is Creature creature && (creature.DEAD || !creature.CanHit || !creature.HasAI)) continue;
                                        double distanceSquared = (explosion.X - ent.X) * (explosion.X - ent.X) + (explosion.Y - ent.Y) * (explosion.Y - ent.Y);
                                        if (distanceSquared > 3) continue;
                                        double damage = rand.Next(25, 50);
                                        if (ent is Player playerTarget)
                                            playerTarget.DealDamage(damage);
                                        if (ent is NPC npc)
                                            npc.DealDamage(damage);
                                        if (ent is Enemy enemy)
                                            enemy.DealDamage(damage);
                                    }
                                }
                            }
                            Entities.Remove(entity);
                            continue;
                        }
                    }
                    if (entity is Enemy)
                    {
                        if (distance <= 22)
                        {
                            if (!entity.DEAD && !player.Dead)
                            {
                                entity.UpdateCoordinates(MAP.ToString(), player.X, player.Y);
                                if (entity.Fast)
                                    entity.UpdateCoordinates(MAP.ToString(), player.X, player.Y);
                                if (Math.Abs(entity.X - player.X) <= 0.5 && Math.Abs(entity.Y - player.Y) <= 0.5)
                                {
                                    if (!player.Invulnerable)
                                    {
                                        player.DealDamage(rand.Next(entity.MIN_DAMAGE, entity.MAX_DAMAGE));
                                        if (player.CuteMode)
                                            PlaySoundHandle(SLIL.hungry, player.X, player.Y);
                                        else
                                        {
                                            if (player.OnBike)
                                                PlaySoundHandle(SLIL.hit[1], player.X, player.Y);
                                            else
                                                PlaySoundHandle(SLIL.hit[0], player.X, player.Y);
                                        }
                                        if (player.HP <= 0)
                                        {
                                            Entities.Add(new PlayerDeadBody(player.X, player.Y, MAP_WIDTH, ref MaxEntityID));
                                            GameOver(0);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (entity is Pet)
                    {
                        Player owner = null;
                        foreach (Entity ent in Entities)
                        {
                            if (ent is Player player1)
                            {
                                if ((ent as Player).PET == entity)
                                {
                                    owner = player1;
                                    distance = Math.Sqrt(Math.Pow(entity.X - player.X, 2) + Math.Pow(entity.Y - player.Y, 2));
                                }
                            }
                        }
                        if (distance > 1 && !(owner.GetCurrentGun() is Flashlight && entity.RespondsToFlashlight))
                            entity.UpdateCoordinates(MAP.ToString(), owner.X, owner.Y);
                        else
                            entity.Stoped = true;
                    }
                    else if (entity is Rockets rocket)
                    {
                        if (!entity.DEAD)
                            entity.UpdateCoordinates(MAP.ToString(), player.X, player.Y);
                        char[] ImpassibleCells = { '#', 'D', 'd', '=' };
                        double newX = entity.X + entity.GetMove() * Math.Sin(entity.A);
                        double newY = entity.Y + entity.GetMove() * Math.Cos(entity.A);
                        if (ImpassibleCells.Contains(MAP[(int)newY * MAP_WIDTH + (int)(newX + entity.EntityWidth / 2)])
                        || ImpassibleCells.Contains(MAP[(int)newY * MAP_WIDTH + (int)(newX - entity.EntityWidth / 2)])
                        || ImpassibleCells.Contains(MAP[(int)(newY + entity.EntityWidth / 2) * MAP_WIDTH + (int)newX])
                        || ImpassibleCells.Contains(MAP[(int)(newY - entity.EntityWidth / 2) * MAP_WIDTH + (int)newX]))
                        {
                            Entities.Remove(entity);
                            Entities.Add(new Explosion(entity.X, entity.Y, MAP_WIDTH, ref MaxEntityID));
                        }
                        if (!Entities.Contains(entity)) continue;
                        foreach (Entity ent in Entities)
                        {
                            if (ent == entity) continue;
                            if (ent is Creature creature && (creature.DEAD || !creature.CanHit || !creature.HasAI)) continue;
                            if (Math.Sqrt((entity.X - ent.X) * (entity.X - ent.X) + (entity.Y - ent.Y) * (entity.Y - ent.Y)) < (entity.EntityWidth + ent.EntityWidth) * (entity.EntityWidth + ent.EntityWidth))
                            {
                                Entities.Remove(entity);
                                Entities.Add(new Explosion(entity.X, entity.Y, MAP_WIDTH, ref MaxEntityID));
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void RespawnTimer_Tick(object sender, EventArgs e)
        {
            List<Player> playersList = new List<Player>();
            foreach (Entity ent in Entities)
            {
                if (ent is Player) playersList.Add(ent as Player);
            }
            if (playersList.Count == 0) return;
            Parallel.For(0, Entities.Count, i =>
            {
                if (GameStarted)
                {
                    if (!(Entities[i] is Enemy)) return;
                    if (!Entities[i].HasAI) return;
                    var enemy = Entities[i] as dynamic;
                    playersList.OrderBy((playerI) => Math.Pow(enemy.X - playerI.X, 2) + Math.Pow(enemy.Y - playerI.Y, 2));
                    Player player = playersList[0];
                    double distance = Math.Sqrt(Math.Pow(enemy.X - player.X, 2) + Math.Pow(enemy.Y - player.Y, 2));
                    if (distance <= 30)
                    {
                        if (difficulty <= 1)
                        {
                            if (enemy.DEAD && enemy.RESPAWN > 0)
                                enemy.RESPAWN--;
                            else if (enemy.DEAD && enemy.RESPAWN <= 0)
                            {
                                if (Math.Abs(enemy.X - player.X) > 1 && Math.Abs(enemy.Y - player.Y) > 1)
                                    enemy.Respawn();
                            }
                        }
                    }
                }
            });
        }

        public void SetCustom(bool custom, int CustomWidth, int CustomHeight, string CustomMap, double customX, double customY)
        {
            CUSTOM = custom;
            CustomMazeWidth = CustomWidth;
            CustomMazeHeight = CustomHeight;
            CUSTOM_MAP = new StringBuilder(CustomMap);
            CUSTOM_X = customX;
            CUSTOM_Y = customY;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(MAP.ToString());
            writer.Put(MAP_WIDTH);
            writer.Put(MAP_HEIGHT);
            writer.Put(Entities.Count);
            foreach (var entity in Entities)
            {
                writer.Put(entity.EntityID);
                writer.Put(entity.ID);
                entity.Serialize(writer);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            MAP = new StringBuilder(reader.GetString());
            MAP_WIDTH = reader.GetInt();
            MAP_HEIGHT = reader.GetInt();
            int entCount = reader.GetInt();
            List<Entity> tempEntities = new List<Entity>();
            for (int i = 0; i < entCount; i++)
            {
                int entityID = reader.GetInt();
                int ID = reader.GetInt();
                switch (entityID)
                {
                    case 0:
                        Player p = new Player(0, 0, MAP_WIDTH, ID);
                        p.Deserialize(reader);
                        tempEntities.Add(p);
                        break;
                    case 1:
                        Man man = new Man(0, 0, MAP_WIDTH, ID);
                        man.SetDamage(EnemyDamageOffset);
                        man.Deserialize(reader);
                        tempEntities.Add(man);
                        break;
                    case 2:
                        Dog dog = new Dog(0, 0, MAP_WIDTH, ID);
                        dog.SetDamage(EnemyDamageOffset);
                        dog.Deserialize(reader);
                        tempEntities.Add(dog);
                        break;
                    case 3:
                        Abomination abomination = new Abomination(0, 0, MAP_WIDTH, ID);
                        abomination.SetDamage(EnemyDamageOffset);
                        abomination.Deserialize(reader);
                        tempEntities.Add(abomination);
                        break;
                    case 4:
                        Bat bat = new Bat(0, 0, MAP_WIDTH, ID);
                        bat.SetDamage(EnemyDamageOffset);
                        bat.Deserialize(reader);
                        tempEntities.Add(bat);
                        break;
                    case 5:
                        SillyCat sillyCat = new SillyCat(0, 0, MAP_WIDTH, ID);
                        sillyCat.Deserialize(reader);
                        tempEntities.Add(sillyCat);
                        break;
                    case 6:
                        GreenGnome greenGnome = new GreenGnome(0, 0, MAP_WIDTH, ID);
                        greenGnome.Deserialize(reader);
                        tempEntities.Add(greenGnome);
                        break;
                    case 7:
                        EnergyDrink energyDrink = new EnergyDrink(0, 0, MAP_WIDTH, ID);
                        energyDrink.Deserialize(reader);
                        tempEntities.Add(energyDrink);
                        break;
                    case 8:
                        Pyro pyro = new Pyro(0, 0, MAP_WIDTH, ID);
                        pyro.Deserialize(reader);
                        tempEntities.Add(pyro);
                        break;
                    case 9:
                        Teleport teleport = new Teleport(0, 0, MAP_WIDTH, ID);
                        teleport.Deserialize(reader);
                        tempEntities.Add(teleport);
                        break;
                    case 10:
                        HittingTheWall hittingTheWall = new HittingTheWall(0, 0, MAP_WIDTH, ID);
                        hittingTheWall.Deserialize(reader);
                        tempEntities.Add(hittingTheWall);
                        break;
                    case 11:
                        ShopDoor shopDoor = new ShopDoor(0, 0, MAP_WIDTH, ID);
                        shopDoor.Deserialize(reader);
                        tempEntities.Add(shopDoor);
                        break;
                    case 12:
                        ShopMan shopMan = new ShopMan(0, 0, MAP_WIDTH, ID);
                        shopMan.Deserialize(reader);
                        tempEntities.Add(shopMan);
                        break;
                    case 13:
                        PlayerDeadBody playerDeadBody = new PlayerDeadBody(0, 0, MAP_WIDTH, ID);
                        playerDeadBody.Deserialize(reader);
                        tempEntities.Add(playerDeadBody);
                        break;
                    case 14:
                        Box box = new Box(0, 0, MAP_WIDTH, ID);
                        box.Deserialize(reader);
                        tempEntities.Add(box);
                        break;
                    case 15:
                        Barrel barrel = new Barrel(0, 0, MAP_WIDTH, ID);
                        barrel.Deserialize(reader);
                        tempEntities.Add(barrel);
                        break;
                    case 16:
                        RpgRocket rpgRocket = new RpgRocket(0, 0, MAP_WIDTH, ID);
                        rpgRocket.Deserialize(reader);
                        tempEntities.Add(rpgRocket);
                        break;
                    case 17:
                        Explosion explosion = new Explosion(0, 0, MAP_WIDTH, ID);
                        explosion.Deserialize(reader);
                        tempEntities.Add(explosion);
                        break;
                    case 18:
                        Bike bike = new Bike(0, 0, MAP_WIDTH, ID);
                        bike.Deserialize(reader);
                        tempEntities.Add(bike);
                        break;
                    default:
                        break;
                }
            }
            Entities = new List<Entity>(tempEntities);
        }

        public void Deserialize(NetDataReader reader, int playerID)
        {
            MAP = new StringBuilder(reader.GetString());
            MAP_WIDTH = reader.GetInt();
            MAP_HEIGHT = reader.GetInt();
            int entCount = reader.GetInt();
            List<Entity> tempEntities = new List<Entity>();
            for (int i = 0; i < entCount; i++)
            {
                int entityID = reader.GetInt();
                int ID = reader.GetInt();
                if (ID == playerID)
                {
                    foreach (Entity ent in Entities)
                    {
                        if (ent.ID == playerID)
                        {
                            Player player = ent as Player;
                            player.Deserialize(reader, false);
                            tempEntities.Add(player);
                            break;
                        }
                    }
                    continue;
                }
                switch (entityID)
                {
                    case 0:
                        Player p = new Player(0, 0, MAP_WIDTH, ID);
                        p.Deserialize(reader);
                        tempEntities.Add(p);
                        break;
                    case 1:
                        Man man = new Man(0, 0, MAP_WIDTH, ID);
                        man.SetDamage(EnemyDamageOffset);
                        man.Deserialize(reader);
                        tempEntities.Add(man);
                        break;
                    case 2:
                        Dog dog = new Dog(0, 0, MAP_WIDTH, ID);
                        dog.SetDamage(EnemyDamageOffset);
                        dog.Deserialize(reader);
                        tempEntities.Add(dog);
                        break;
                    case 3:
                        Abomination abomination = new Abomination(0, 0, MAP_WIDTH, ID);
                        abomination.SetDamage(EnemyDamageOffset);
                        abomination.Deserialize(reader);
                        tempEntities.Add(abomination);
                        break;
                    case 4:
                        Bat bat = new Bat(0, 0, MAP_WIDTH, ID);
                        bat.SetDamage(EnemyDamageOffset);
                        bat.Deserialize(reader);
                        tempEntities.Add(bat);
                        break;
                    case 5:
                        SillyCat sillyCat = new SillyCat(0, 0, MAP_WIDTH, ID);
                        sillyCat.Deserialize(reader);
                        tempEntities.Add(sillyCat);
                        break;
                    case 6:
                        GreenGnome greenGnome = new GreenGnome(0, 0, MAP_WIDTH, ID);
                        greenGnome.Deserialize(reader);
                        tempEntities.Add(greenGnome);
                        break;
                    case 7:
                        EnergyDrink energyDrink = new EnergyDrink(0, 0, MAP_WIDTH, ID);
                        energyDrink.Deserialize(reader);
                        tempEntities.Add(energyDrink);
                        break;
                    case 8:
                        Pyro pyro = new Pyro(0, 0, MAP_WIDTH, ID);
                        pyro.Deserialize(reader);
                        tempEntities.Add(pyro);
                        break;
                    case 9:
                        Teleport teleport = new Teleport(0, 0, MAP_WIDTH, ID);
                        teleport.Deserialize(reader);
                        tempEntities.Add(teleport);
                        break;
                    case 10:
                        HittingTheWall hittingTheWall = new HittingTheWall(0, 0, MAP_WIDTH, ID);
                        hittingTheWall.Deserialize(reader);
                        tempEntities.Add(hittingTheWall);
                        break;
                    case 11:
                        ShopDoor shopDoor = new ShopDoor(0, 0, MAP_WIDTH, ID);
                        shopDoor.Deserialize(reader);
                        tempEntities.Add(shopDoor);
                        break;
                    case 12:
                        ShopMan shopMan = new ShopMan(0, 0, MAP_WIDTH, ID);
                        shopMan.Deserialize(reader);
                        tempEntities.Add(shopMan);
                        break;
                    case 13:
                        PlayerDeadBody playerDeadBody = new PlayerDeadBody(0, 0, MAP_WIDTH, ID);
                        playerDeadBody.Deserialize(reader);
                        tempEntities.Add(playerDeadBody);
                        break;
                    case 14:
                        Box box = new Box(0, 0, MAP_WIDTH, ID);
                        box.Deserialize(reader);
                        tempEntities.Add(box);
                        break;
                    case 15:
                        Barrel barrel = new Barrel(0, 0, MAP_WIDTH, ID);
                        barrel.Deserialize(reader);
                        tempEntities.Add(barrel);
                        break;
                    case 16:
                        RpgRocket rpgRocket = new RpgRocket(0, 0, MAP_WIDTH, ID);
                        rpgRocket.Deserialize(reader);
                        tempEntities.Add(rpgRocket);
                        break;
                    case 17:
                        Explosion explosion = new Explosion(0, 0, MAP_WIDTH, ID);
                        explosion.Deserialize(reader);
                        tempEntities.Add(explosion);
                        break;
                    case 18:
                        Bike bike = new Bike(0, 0, MAP_WIDTH, ID);
                        bike.Deserialize(reader);
                        tempEntities.Add(bike);
                        break;
                    default:
                        break;
                }
            }
            Entities = new List<Entity>(tempEntities);
        }

        public void AddPet(int playerID, int index)
        {
            Player player = null;
            foreach (Entity ent in Entities)
            {
                if (ent is Player player1)
                {
                    if (ent.ID == playerID)
                        player = player1;
                }
            }
            Pet pet = PETS[index];
            pet.ID = MaxEntityID;
            MaxEntityID++;
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] is Pet)
                {
                    if ((Entities[i] as Pet).IsInstantAbility != 0)
                    {
                        switch ((Entities[i] as Pet).GetPetAbility())
                        {
                            case 1: //GreenGnome
                                player.MAX_HP -= 25;
                                player.HealHP(125);
                                break;
                            case 2: //Energy Drink
                                player.MAX_STAMINE -= 150;
                                player.MOVE_SPEED -= 0.15;
                                player.RUN_SPEED -= 0.15;
                                player.STAMINE = player.MAX_STAMINE;
                                break;
                            case 3: //Pyro
                                player.CuteMode = false;
                                CuteMode(player);
                                break;
                        }
                    }
                    Entities.RemoveAt(i);
                }
            }
            if (pet.IsInstantAbility != 0)
            {
                switch (pet.GetPetAbility())
                {
                    case 1: //GreenGnome
                        player.MAX_HP += 25;
                        player.HealHP(125);
                        break;
                    case 2: //Energy Drink
                        player.MAX_STAMINE += 150;
                        player.MOVE_SPEED += 0.15;
                        player.RUN_SPEED += 0.15;
                        player.STAMINE = player.MAX_STAMINE;
                        break;
                    case 3: //Pyro
                        player.CuteMode = true;
                        CuteMode(player);
                        break;
                }
            }
            player.PET = pet;
            UpdatePet(player);
            player.Money -= pet.Cost;
        }

        private void CuteMode(Player player)
        {
            player.Guns.Clear();
            player.CurrentGun = 1;
            if (player.CuteMode)
            {
                player.GUNS[11].HasIt = true;
                player.GUNS[12].HasIt = true;
                player.Guns.Add(player.GUNS[11]);
                player.Guns.Add(player.GUNS[12]);
            }
            else
            {
                player.GUNS[11].HasIt = false;
                player.GUNS[12].HasIt = false;
                for (int i = 0; i < 11; i++)
                {
                    if (player.GUNS[i].HasIt)
                        player.Guns.Add(player.GUNS[i]);
                }
            }
        }

        private void UpdatePet(Player player)
        {
            bool doesPlayerExist = false;
            foreach (Entity entity in Entities)
            {
                if ((player as Entity) == entity)
                    doesPlayerExist = true;
            }
            if (!doesPlayerExist)
                return;
            if (player.PET == null)
                return;
            player.PET.SetNewParametrs(player.X + 0.1, player.Y + 0.1, MAP_WIDTH);
            Entities.Add(player.PET);
        }

        private void GameOver(int win)
        {
            EnemyTimer.Stop();
            RespawnTimer.Stop();
            TimeRemain.Stop();
            GameStarted = false;
            MAP.Clear();
            if (win == 1)
            {
                foreach (Entity ent in Entities.ToArray())
                {
                    if (!(ent is Player player))
                    {
                        Entities.Remove(ent);
                        continue;
                    }
                    if (difficulty != 4 && difficulty != 6)
                        player.Stage++;
                    if (!player.CuteMode)
                    {
                        for (int i = 0; i < player.Guns.Count; i++)
                        {
                            if (player.Guns[i].AmmoInStock == 0)
                                player.Guns[i].AmmoInStock = player.Guns[i].CartridgesClip;
                        }
                    }
                    player.ChangeMoney(50 + (5 * player.EnemiesKilled));
                    StartGame(true);
                    UpdatePet(player);
                }
            }
            else if (win == 0)
            {
                EnemyTimer.Stop();
                RespawnTimer.Stop();
                TimeRemain.Stop();
                Entities.Clear();
                MaxEntityID = 0;
            }
            else
            {
                EnemyTimer.Stop();
                RespawnTimer.Stop();
                TimeRemain.Stop();
                Entities.Clear();
                MaxEntityID = 0;
            }
            StopGameHandle(win);
        }

        private void GetFirstAidKit(Player player) => player.DisposableItems[0].AddItem();

        public void MovePlayer(double dX, double dY, int playerID)
        {
            if (!GameStarted) return;
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] is Player player)
                {
                    if ((Entities[i] as Player).ID == playerID)
                    {
                        if (player.BlockInput) return;
                        Entities[i].X += dX;
                        Entities[i].Y += dY;
                        if (player.EffectCheck(4))
                        {
                            double extendedX = Entities[i].X + Math.Sin(player.A) * 0.3;
                            double extendedY = Entities[i].Y + Math.Cos(player.A) * 0.3;
                            if (MAP[(int)extendedY * MAP_WIDTH + (int)extendedX] == '=')
                            {
                                double distance = 1;
                                while (distance <= 2)
                                {
                                    distance += 0.1d;
                                    int x1 = (int)(Entities[i].X + Math.Sin(player.A) * distance);
                                    int y1 = (int)(Entities[i].Y + Math.Cos(player.A) * distance);
                                    if (!HasImpassibleCells(playerID, y1 * MAP_WIDTH + x1))
                                    {
                                        DoParkour(player.ID, (int)extendedY, (int)extendedX);
                                        break;
                                    }
                                }
                            }
                        }
                        if (MAP[(int)Entities[i].Y * MAP_WIDTH + (int)Entities[i].X] == 'F')
                        {
                            if (!IsMultiplayer)
                                GameOver(1);
                            return;
                        }
                        return;
                    }
                }
            }
        }

        public void GoDebug(int debug)
        {
            inDebug = debug;
            difficulty = 5;
        }

        private Entity GetEntityForInitMap(char c, int x, int y)
        {
            Entity entity = null;
            switch (c)
            {
                case 'F':
                    Teleport teleport = new Teleport(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    entity = teleport;
                    break;
                case 'D':
                    ShopDoor shopDoor = new ShopDoor(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    entity = shopDoor;
                    break;
                case '$':
                    ShopMan shopMan = new ShopMan(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    entity = shopMan;
                    break;
                case 'b':
                    Box box = new Box(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    entity = box;
                    break;
                case 'B':
                    Barrel barrel = new Barrel(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    entity = barrel;
                    break;
                case 'E':
                    SpawnEnemis(x, y, MAP_WIDTH);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
                case 'e':
                    SpawnEnemis(x +0.5, y + 0.5, MAP_WIDTH, false);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
                case '1':
                    SpawnEnemis(x + 0.5, y + 0.5, MAP_WIDTH, false, 0);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
                case '2':
                    SpawnEnemis(x + 0.5, y + 0.5, MAP_WIDTH, false, 1);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
                case '3':
                    SpawnEnemis(x + 0.5, y + 0.5, MAP_WIDTH, false, 2);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
                case '4':
                    SpawnEnemis(x + 0.5, y + 0.5, MAP_WIDTH, false, 3);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
                case '5':
                    SpawnEnemis(x + 0.5, y + 0.5, MAP_WIDTH, false, 4);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
            }
            return entity;
        }

        public void InitMap()
        {
            double enemy_count = 0;
            int MazeWidth = 0, MazeHeight = 0, MAX_SHOP_COUNT = 1;
            if (difficulty == 0)
                enemy_count = 0.07;
            else if (difficulty == 1)
                enemy_count = 0.065;
            else if (difficulty == 2)
                enemy_count = 0.055;
            else if (difficulty == 3)
                enemy_count = 0.045;
            else if (difficulty == 4)
            {
                MazeHeight = CustomMazeHeight;
                MazeWidth = CustomMazeWidth;
                enemy_count = 0.06;
                MAX_SHOP_COUNT = 5;
            }
            else
            {
                if (inDebug == 1)
                {
                    MazeHeight = 6;
                    MazeWidth = 6;
                }
                else if (inDebug == 2)
                {
                    MazeHeight = 7;
                    MazeWidth = 7;
                }
                else if (inDebug == 3)
                {
                    MazeHeight = 8;
                    MazeWidth = 8;
                }
            }
            if (difficulty < 4)
            {
                MazeHeight = MazeWidth = 10;
                MAX_SHOP_COUNT = 2;
            }
            foreach (Entity ent in Entities)
            {
                if (!(ent is Player player)) continue;
                if (difficulty == 0)
                    enemy_count = 0.07;
                else if (difficulty == 1)
                    enemy_count = 0.065;
                else if (difficulty == 2)
                {
                    enemy_count = 0.055;
                    if(player.Stage == 0)
                        player.Guns[1].LevelUpdate();
                }
                else if (difficulty == 3)
                {
                    enemy_count = 0.045;
                    if(player.Stage == 0)
                        player.Guns[1].LevelUpdate();
                }
                else if (difficulty == 4)
                {
                    MazeHeight = CustomMazeHeight;
                    MazeWidth = CustomMazeWidth;
                    enemy_count = 0.06;
                    MAX_SHOP_COUNT = 5;
                }
                else
                {
                    if (inDebug == 1)
                    {
                        player.X = 9;
                        player.Y = 9;
                        MazeHeight = 6;
                        MazeWidth = 6;
                    }
                    else if (inDebug == 2)
                    {
                        player.X = 10.5;
                        player.Y = 19.5;
                        MazeHeight = 7;
                        MazeWidth = 7;
                    }
                    else if (inDebug == 3)
                    {
                        player.X = 21.5;
                        player.Y = 22.5;
                        MazeHeight = 8;
                        MazeWidth = 8;
                    }
                }
                if (difficulty < 4)
                {
                    if (player.Stage == 0)
                    {
                        MazeHeight = MazeWidth = 10;
                        MAX_SHOP_COUNT = 2;
                    }
                    else if (player.Stage == 1)
                    {
                        MazeHeight = MazeWidth = 15;
                        MAX_SHOP_COUNT = 4;
                    }
                    else if (player.Stage == 2)
                    {
                        MazeHeight = MazeWidth = 20;
                        MAX_SHOP_COUNT = 6;
                    }
                    else if (player.Stage == 3)
                    {
                        MazeHeight = MazeWidth = 25;
                        MAX_SHOP_COUNT = 8;
                    }
                    else
                    {
                        MazeHeight = MazeWidth = 25;
                        MAX_SHOP_COUNT = 8;
                    }
                }
            }
            MAP_WIDTH = MazeWidth * 3 + 1;
            MAP_HEIGHT = MazeHeight * 3 + 1;
            MAP.Clear();
            if (difficulty == 5)
            {
                if (inDebug == 1)
                    MAP.AppendLine(debugMap);
                else if (inDebug == 2)
                    MAP.AppendLine(bossMap);
                else if (inDebug == 3)
                    MAP.AppendLine(bikeMap);
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    for (int y = 0; y < MAP_HEIGHT; y++)
                    {
                        Entity entity = GetEntityForInitMap(MAP[y * MAP_WIDTH + x], x, y);
                        if (entity != null)
                            Entities.Add(entity);
                    }
                }
            }
            else
            {
                if (!CUSTOM)
                {
                    Random random = new Random();
                    StringBuilder sb = new StringBuilder();
                    char[,] map = (new Maze()).GenerateCharMap(MazeWidth, MazeHeight, '#', '=', 'd', '.', 'F', MAX_SHOP_COUNT);
                    map[1, 1] = 'P';
                    List<int[]> shops = new List<int[]>();
                    for (int y = 0; y < map.GetLength(1); y++)
                    {
                        for (int x = 0; x < map.GetLength(0); x++)
                        {
                            try
                            {
                                if ((map[x, y] == '.' || map[x, y] == '=' || map[x, y] == 'D') &&
                                    (map[x + 1, y] == '#' || map[x + 1, y] == '=' || map[x + 1, y] == 'D') &&
                                    (map[x, y + 1] == '#' || map[x, y + 1] == '=' || map[x, y + 1] == 'D') &&
                                    ((map[x + 2, y] == '#' || map[x + 2, y] == '=' || map[x + 2, y] == 'D') ||
                                    (map[x, y + 2] == '#' || map[x, y + 2] == '=' || map[x, y + 2] == 'D')))
                                    map[x, y] = '#';
                            }
                            catch { }
                            if (map[x, y] == '$')
                                shops.Add(new int[] { x, y });
                        }
                    }
                    if (shops.Count == 0)
                    {
                        if (map[3, 1] == '#')
                        {
                            map[3, 1] = '$';
                            shops.Add(new int[] { 3, 1 });
                        }
                        else if (map[1, 3] == '#')
                        {
                            map[1, 3] = '$';
                            shops.Add(new int[] { 1, 3 });
                        }
                    }
                    for (int i = 0; i < shops.Count; i++)
                    {
                        int[] shop = shops[i];
                        int shop_x = shop[0];
                        int shop_y = shop[1];
                        for (int x = shop_x - 1; x <= shop_x + 1; x++)
                        {
                            for (int y = shop_y - 1; y <= shop_y + 1; y++)
                            {
                                if (y >= 0 && y >= map.GetLength(0) && x >= 0 && x >= map.GetLength(1))
                                    continue;
                                if (x == shop_x && y == shop_y)
                                    continue;
                                if (map[x, y] != 'F')
                                    map[x, y] = '#';
                            }
                        }
                        bool spawned = false;
                        if (shop_y >= 2 && shop_y < map.GetLength(0) - 2 && shop_x >= 0 && shop_x < map.GetLength(1) && map[shop_x, shop_y - 2] == '.')
                        {
                            try
                            {
                                if (!spawned)
                                {
                                    map[shop_x, shop_y - 1] = 'D';
                                    spawned = true;
                                }
                            }
                            catch { }
                        }
                        if (shop_y >= 0 && shop_y < map.GetLength(0) - 2 && shop_x >= 0 && shop_x < map.GetLength(1) && map[shop_x, shop_y + 2] == '.')
                        {
                            try
                            {
                                if (!spawned)
                                {
                                    map[shop_x, shop_y + 1] = 'D';
                                    spawned = true;
                                }
                            }
                            catch { }
                        }
                        if (shop_y >= 0 && shop_y < map.GetLength(0) && shop_x >= 2 && shop_x < map.GetLength(1) - 2 && map[shop_x - 2, shop_y] == '.')
                        {
                            try
                            {
                                if (!spawned)
                                {
                                    map[shop_x - 1, shop_y] = 'D';
                                    spawned = true;
                                }
                            }
                            catch { }
                        }
                        if (shop_y >= 0 && shop_y < map.GetLength(0) && shop_x >= 0 && shop_x < map.GetLength(1) - 2 && map[shop_x + 2, shop_y] == '.')
                        {
                            try
                            {
                                if (!spawned)
                                {
                                    map[shop_x + 1, shop_y] = 'D';
                                    spawned = true;
                                }
                            }
                            catch { }
                        }
                        if (!spawned)
                        {
                            if (shop_y - 1 > 0)
                                map[shop_x, shop_y - 1] = 'D';
                            if (shop_y + 1 < map.GetLength(0) - 1)
                                map[shop_x, shop_y + 1] = 'D';
                            if (shop_x - 1 > 0)
                                map[shop_x - 1, shop_y] = 'D';
                            if (shop_x + 1 < map.GetLength(1) - 1)
                                map[shop_x + 1, shop_y] = 'D';
                        }
                    }
                    for (int y = 0; y < map.GetLength(1); y++)
                    {
                        for (int x = 0; x < map.GetLength(0); x++)
                        {
                            if (map[x, y] == '.' && random.NextDouble() <= enemy_count && x > 5 && y > 5)
                                SpawnEnemis(x, y, MAP_WIDTH);
                            else
                            {
                                Entity entity = GetEntityForInitMap(map[x, y], x, y);
                                if (entity != null)
                                    Entities.Add(entity);
                            }
                            sb.Append(map[x, y]);
                        }
                    }
                    MAP = sb;
                }
                else
                {
                    MAP.Append(CUSTOM_MAP);
                    for (int x = 0; x < CustomMazeWidth * 3 + 1; x++)
                    {
                        for (int y = 0; y < CustomMazeHeight * 3 + 1; y++)
                        {
                            Entity entity = GetEntityForInitMap(MAP[y * (CustomMazeWidth * 3 + 1) + x], x, y);
                            if (entity != null) Entities.Add(entity);
                        }
                    }
                }
                for (int i = 0; i < MAP.Length; i++)
                {
                    if (MAP[i] == 'o')
                        MAP[i] = 'd';
                }
            }
            foreach (Entity ent in Entities)
            {
                if (!(ent is Player player)) continue;
                if (CUSTOM)
                {
                    player.X = CUSTOM_X;
                    player.Y = CUSTOM_Y;
                    continue;
                }
                if (difficulty != 5)
                {
                    player.X = 1.5;
                    player.Y = 1.5;
                }
            };
        }

        public void SpawnRockets(double x, double y, int id, double a)
        {
            Rockets rocket = null;
            if (id == 0)
                rocket = new RpgRocket(x, y, MAP_WIDTH, ref MaxEntityID);
            rocket.X += Math.Sin(a);// * rocket.GetMove();
            rocket.Y += Math.Cos(a);// * rocket.GetMove();
            if (rocket != null)
            {
                rocket.SetA(a);
                Entities.Add(rocket);
            }
        }

        private void SpawnEnemis(double x, double y, int size, bool ai = true, int type = -1)
        {
            if (type == -1)
            {
                double dice = rand.NextDouble();
                if (dice <= 0.4) // 40%
                {
                    Man enemy = new Man(x, y, size, ref MaxEntityID);
                    enemy.SetDamage(EnemyDamageOffset);
                    enemy.HasAI = ai;
                    Entities.Add(enemy);
                }
                else if (dice > 0.4 && dice <= 0.65) // 25%
                {
                    Dog enemy = new Dog(x, y, size, ref MaxEntityID);
                    enemy.SetDamage(EnemyDamageOffset);
                    enemy.HasAI = ai;
                    Entities.Add(enemy);
                }
                else if (dice > 0.65 && dice <= 0.85) // 20%
                {
                    Bat enemy = new Bat(x, y, size, ref MaxEntityID);
                    enemy.SetDamage(EnemyDamageOffset);
                    enemy.HasAI = ai;
                    Entities.Add(enemy);
                }
                else // 15%
                {
                    Abomination enemy = new Abomination(x, y, size, ref MaxEntityID);
                    enemy.SetDamage(EnemyDamageOffset);
                    enemy.HasAI = ai;
                    Entities.Add(enemy);
                }
            }
            else if (type == 0)
            {
                Man enemy = new Man(x, y, size, ref MaxEntityID);
                enemy.SetDamage(EnemyDamageOffset);
                enemy.HasAI = ai;
                Entities.Add(enemy);
            }
            else if (type == 1)
            {
                Dog enemy = new Dog(x, y, size, ref MaxEntityID);
                enemy.SetDamage(EnemyDamageOffset);
                enemy.HasAI = ai;
                Entities.Add(enemy);
            }
            else if (type == 2)
            {
                Abomination enemy = new Abomination(x, y, size, ref MaxEntityID);
                enemy.SetDamage(EnemyDamageOffset);
                enemy.HasAI = ai;
                Entities.Add(enemy);
            }
            else if (type == 3)
            {
                Bat enemy = new Bat(x, y, size, ref MaxEntityID);
                enemy.SetDamage(EnemyDamageOffset);
                enemy.HasAI = ai;
                Entities.Add(enemy);
            }
            else if (type == 4)
            {
                Bike bike = new Bike(x, y, size, ref MaxEntityID);
                Entities.Add(bike);
            }
        }

        internal void RemoveEntity(int id)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i].ID == id)
                {
                    Entities.RemoveAt(i);
                    break;
                }
            }
        }

        internal Entity GetEntity(int id)
        {
            Entity entity = null;
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i].ID == id)
                {
                    entity = Entities[i];
                    break;
                }
            }
            return entity;
        }

        public StringBuilder GetMap() => MAP;

        public Pet[] GetPets() => PETS;

        public int GetMapWidth() => MAP_WIDTH;

        public int GetMapHeight() => MAP_HEIGHT;

        public List<Entity> GetEntities() => Entities;

        public bool DealDamage(int ID, double damage, int attackerID)
        {
            Entity target = null;
            Entity attacker = null;
            foreach (Entity entity in Entities)
            {
                if (entity.ID == ID) target = entity;
                if (entity.ID == attackerID) attacker = entity;
            }
            if (target is Player p)
            {
                if (!p.HasAI) return false;
                if (attacker is Player attackerPlayer && p.DealDamage(damage))
                {
                    double multiplier = 1;
                    if (difficulty == 3)
                        multiplier = 1.5;
                    attackerPlayer.ChangeMoney(rand.Next((int)(50 * multiplier), (int)(100 * multiplier)));
                    attackerPlayer.EnemiesKilled++;
                    return true;
                }
                return false;
            }
            if (target is Creature c)
            {
                if (!c.HasAI) return false;
                if (c.DealDamage(damage))
                {
                    if (attacker is Player attackerPlayer)
                    {
                        double multiplier = 1;
                        if (difficulty == 3)
                            multiplier = 1.5;
                        attackerPlayer.ChangeMoney(rand.Next((int)(c.MIN_MONEY * multiplier), (int)(c.MAX_MONEY * multiplier)));
                        attackerPlayer.EnemiesKilled++;
                        if (target is Boxes box && !attackerPlayer.CuteMode)
                        {
                            if (box.BoxWithMoney)
                                attackerPlayer.ChangeMoney(rand.Next(5, 11));
                            else
                            {
                                int type = rand.Next(1, attackerPlayer.Guns.Count);
                                int max = attackerPlayer.Guns[type].MaxAmmo;
                                int ammo = attackerPlayer.Guns[type].CartridgesClip + attackerPlayer.Guns[type].AmmoInStock;
                                if (ammo > max) ammo = max;
                                attackerPlayer.Guns[type].AmmoInStock = ammo;
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public void AddEntity(Entity entity)
        {
            entity.ID = MaxEntityID;
            MaxEntityID++;
            Entities.Add(entity);
        }

        private void TimeRemain_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                if (!(Entities[i] is Player player)) continue;
                if (player.Invulnerable)
                    player.InvulnerableEnd();
                player.UpdateEffectsTime();
                Pet playerPet = player.PET;
                if (playerPet != null && playerPet.IsInstantAbility != 1)
                {
                    if (playerPet.PetAbilityReloading)
                    {
                        if (playerPet.AbilityTimer >= playerPet.AbilityReloadTime)
                            playerPet.PetAbilityReloading = false;
                        else
                            playerPet.AbilityTimer++;
                    }
                    if (!playerPet.PetAbilityReloading)
                    {
                        switch (playerPet.GetPetAbility())
                        {
                            case 0: //Silly Cat
                                player.HealHP(2);
                                playerPet.AbilityTimer = 0;
                                playerPet.PetAbilityReloading = true;
                                break;
                            case 3: //Pyro
                                if (player.GUNS[12].AmmoCount + 10 <= player.GUNS[12].MaxAmmo)
                                    player.GUNS[12].AmmoCount += 10;
                                else
                                    player.GUNS[12].AmmoCount = player.GUNS[12].MaxAmmo;
                                playerPet.AbilityTimer = 0;
                                playerPet.PetAbilityReloading = true;
                                break;
                        }
                    }
                }
            }
        }

        private void ResetDefault()
        {
            /*
            map = null;
            display.SCREEN = null;
            scope[scope_type] = GetScope(scope[scope_type]);
            scope_shotgun[scope_type] = GetScope(scope_shotgun[scope_type]);
            display.Refresh();
            int x = display.PointToScreen(Point.Empty).X + (display.Width / 2);
            int y = display.PointToScreen(Point.Empty).Y + (display.Height / 2);
            Cursor.Position = new Point(x, y);
            seconds = 0;
            if (!CUSTOM)
                player.X = player.Y = 1.5d;
            else
            {
                player.X = CUSTOM_X;
                player.Y = CUSTOM_Y;
            }
            if (player.Guns.Count == 0)
            {
                player.Guns.Add(player.GUNS[1]);
                player.Guns.Add(player.GUNS[2]);
            }
            player.SetDefault();
            player.LevelUpdated = false;
            open_shop = false;
            Entities.Clear();
            strafeDirection = playerDirection = Direction.STOP;
            playerMoveStyle = Direction.WALK;
            if (difficulty == 0)
                enemy_count = 0.07;
            else if (difficulty == 1)
                enemy_count = 0.065;
            else if (difficulty == 2)
            {
                enemy_count = 0.055;
                if (player.Guns[1].Level == Levels.LV1)
                    player.Guns[1].LevelUpdate();
            }
            else if (difficulty == 3)
            {
                enemy_count = 0.045;
                if (player.Guns[1].Level == Levels.LV1)
                    player.Guns[1].LevelUpdate();
            }
            else if (difficulty == 4)
            {
                minutes = 9999;
                MazeHeight = CustomMazeHeight;
                MazeWidth = CustomMazeWidth;
                enemy_count = 0.06;
                MAX_SHOP_COUNT = 5;
            }
            else
            {
                if (inDebug == 1)
                {
                    player.X = 9;
                    player.Y = 9;
                    MazeHeight = 6;
                    MazeWidth = 6;
                }
                else if (inDebug == 2)
                {
                    player.X = 10.5;
                    player.Y = 19.5;
                    MazeHeight = 7;
                    MazeWidth = 7;
                }
                minutes = 9999;
            }
            if (difficulty != 4 && difficulty != 5)
            {
                if (player.Stage == 0)
                {
                    minutes = START_EASY;
                    MazeHeight = MazeWidth = 10;
                    MAX_SHOP_COUNT = 2;
                }
                else if (player.Stage == 1)
                {
                    minutes = START_NORMAL;
                    MazeHeight = MazeWidth = 15;
                    MAX_SHOP_COUNT = 4;
                }
                else if (player.Stage == 2)
                {
                    minutes = START_HARD;
                    MazeHeight = MazeWidth = 20;
                    MAX_SHOP_COUNT = 6;
                }
                else if (player.Stage == 3)
                {
                    minutes = START_VERY_HARD;
                    MazeHeight = MazeWidth = 25;
                    MAX_SHOP_COUNT = 8;
                }
                else
                {
                    minutes = START_VERY_HARD;
                    MazeHeight = MazeWidth = 25;
                    MAX_SHOP_COUNT = 8;
                }
            }
            MAP_WIDTH = MazeWidth * 3 + 1;
            MAP_HEIGHT = MazeHeight * 3 + 1;
            //map = new Bitmap(Controller.GetMapWidth(), Controller.GetMapHeight());
            map = new Bitmap(MAP_WIDTH, MAP_HEIGHT);
            if (MainMenu.sounds)
            {
                prev_ost = rand.Next(ost.Length - 2);
                ChangeOst(prev_ost);
            }*/
        }

        public void AddHittingTheWall(double X, double Y, double vMove)
        {
            HittingTheWall hittingTheWall = new HittingTheWall(X, Y, MAP_WIDTH, ref MaxEntityID)
            {
                VMove = vMove
            };
            Entities.Add(hittingTheWall);
        }

        internal void ChangePlayerA(double v, int playerID)
        {
            foreach (Entity ent in Entities)
            {
                if (ent is Player p && ent.ID == playerID)
                {
                    p.A += v;
                    return;
                }
            }
        }

        internal void ChangePlayerLook(double lookDif, int playerID)
        {
            foreach (Entity ent in Entities)
            {
                if (ent is Player p && ent.ID == playerID)
                {
                    if (p.BlockCamera) return;
                    p.Look += lookDif;
                    if (p.Look < -360)
                        p.Look = -360;
                    else if (p.Look > 360)
                        p.Look = 360;
                    return;
                }
            }
        }

        internal void StopGame(int win) => GameOver(win);

        internal void AmmoCountDecrease(int playerID)
        {
            foreach (Entity entity in Entities)
            {
                if (entity.ID == playerID)
                {
                    Gun gun = (entity as Player).GetCurrentGun();
                    int ammo = gun is SubmachineGun && gun.Level == Levels.LV3 ? 2 : 1;
                    gun.AmmoCount -= ammo;
                    return;
                }
            }
        }

        internal void ReloadClip(int playerID)
        {
            foreach(Entity entity in Entities)
            {
                if(entity.ID == playerID)
                {
                    (entity as Player).GetCurrentGun().ReloadClip();
                    return;
                }
            }
        }

        internal bool OnOffNoClip(int playerID)
        {
            foreach (Entity ent in Entities)
            {
                if (ent.ID == playerID)
                {
                    Player p = (Player)ent;
                    p.NoClip = !p.NoClip;
                    return p.NoClip;
                }
            }
            return false;
        }

        internal bool HasNoClip(int playerID)
        {
            foreach (Entity ent in Entities)
            {
                if (ent.ID == playerID)
                {
                    Player p = (Player)ent;
                    return p.NoClip;
                }
            }
            return false;
        }

        internal void GettingOffTheBike(int playerID)
        {
            Player p = GetPlayer(playerID);
            if (p == null) return;
            Bike bike = new Bike(p.X, p.Y, MAP_WIDTH, MaxEntityID)
            {
                BikeHP = p.BIKE_HP
            };
            p.StopEffect(4);
            AddEntity(bike);
        }

        internal bool HasImpassibleCells(int index, int playerID)
        {
            Player p = GetPlayer(playerID);
            if (p == null) return false;
            char[] impassibleCells = { '#', 'D', '=', 'd', 'S' };
            if (HasNoClip(playerID) || p.InParkour) return false;
            return impassibleCells.Contains(GetMap()[index]);
        }

        internal Player GetPlayer(int playerID)
        {
            foreach (Entity ent in Entities)
            {
                if (ent.ID == playerID)                   
                    return (Player)ent;
            }
            return null;
        }

        internal bool DoParkour(int playerID, int y, int x)
        {
            Player p = GetPlayer(playerID);
            if (p == null) return false;
            if (p.EffectCheck(3)) return false;
            p.InParkour = true;
            p.ParkourState = 0;
            p.X = x + 0.5;
            p.Y = y + 0.5;
            p.Look = 0;
            p.CanUnblockCamera = false;
            p.BlockCamera = p.BlockInput = true;
            p.PlayerMoveStyle = Directions.WALK;
            p.StrafeDirection = Directions.STOP;
            p.PlayerMoveStyle = Directions.STOP;
            new Thread(() => {
                Thread.Sleep(500);
                StopParkour(playerID);
                p.ParkourState++;
            }).Start();
            return true;
        }

        internal void StopParkour(int playerID)
        {
            Player p = GetPlayer(playerID);
            if (p == null) return;
            double new_x = p.X + Math.Sin(p.A);
            double new_y = p.Y + Math.Cos(p.A);
            p.InParkour = false;
            while (HasImpassibleCells((int)new_y * MAP_WIDTH + (int)(new_x), playerID))
            {
                new_x += Math.Sin(p.A) / 4;
                new_y += Math.Cos(p.A) / 4;
            }
            p.X = new_x;
            p.Y = new_y;
            p.GiveEffect(3, true);
            p.BlockInput = false;
            if (!p.OnBike) p.BlockCamera = false;
        }

        internal void ChangeWeapon(int playerID, int new_gun)
        {
            foreach (Entity entity in Entities) 
            {
                if (entity.ID == playerID)
                {
                    (entity as Player).CurrentGun = new_gun;
                    return;
                }
            }
        }

        internal void BuyAmmo(int playerID, int weaponID)
        {
            foreach (Entity ent in Entities)
            {
                if (ent.ID == playerID)
                {
                    Player p = (Player)ent;
                    Gun weapon = p.Guns[weaponID];
                    if (p.Money >= weapon.AmmoCost && weapon.AmmoInStock + weapon.AmmoCount <= weapon.MaxAmmo)
                    {
                        p.ChangeMoney(-weapon.AmmoCost);
                        weapon.AmmoInStock += weapon.CartridgesClip;
                    }
                    return;
                }
            }
        }

        internal void BuyWeapon(int playerID, int weaponID)
        {
            foreach (Entity ent in Entities)
            {
                if (ent.ID == playerID)
                {
                    Player p = (Player)ent;
                    Gun weapon = p.GUNS[weaponID];
                    if (p.Money >= weapon.GunCost)
                    {
                        p.ChangeMoney(-weapon.GunCost);
                        weapon.SetDefault();
                        weapon.HasIt = true;
                        p.Guns.Add(weapon);
                    }
                    return;
                }
            }
        }

        internal void UpdateWeapon(int playerID, int weaponID)
        {
            foreach (Entity ent in Entities)
            {
                if (ent.ID == playerID)
                {
                    Player p = (Player)ent;
                    Gun weapon = p.Guns[weaponID];
                    if (p.Money >= weapon.UpdateCost)
                    {
                        p.ChangeMoney(-weapon.UpdateCost);
                        weapon.LevelUpdate();
                        p.LevelUpdated = true;
                    }
                    return;
                }
            }
        }

        internal void BuyConsumable(int playerID, int itemID)
        {
            foreach (Entity ent in Entities)
            {
                if (ent.ID == playerID)
                {
                    Player p = (Player)ent;
                    DisposableItem item = p.GUNS[itemID] as DisposableItem;
                    if (p.Money >= item.GunCost && !item.HasIt)
                    {
                        p.ChangeMoney(-item.GunCost);
                        item.AddItem();
                    }
                    return;
                }
            }
        }

        internal void InteractingWithDoors(int coordinate)
        {
            double X = coordinate % MAP_HEIGHT;
            double Y = (coordinate - X) / MAP_WIDTH;
            if (MAP[coordinate] == 'o')
            {
                MAP[coordinate] = 'd';
                PlaySoundHandle(SLIL.door[1], X, Y);
            }
            else
            {
                MAP[coordinate] = 'o';
                PlaySoundHandle(SLIL.door[0], X, Y);
            }
        }

        internal void SetEnemyDamageOffset(double value) => EnemyDamageOffset = value;

        internal void GetOnABike(int ID, int playerID)
        {
            Player p = GetPlayer(playerID);
            if (p == null) return;
            Entity entity = GetEntity(ID);
            if (entity != null)
            {
                double hp = ((Bike)entity).BikeHP;
                p.BIKE_HP = hp;
            }
            RemoveEntity(ID);
            p.GiveEffect(4, true);
        }

        internal void DeserializePlayer(int playerID, byte[] rawData)
        {
            foreach(Entity ent in Entities)
            {
                if(ent.ID == playerID)
                {
                    Player p = (Player)ent;
                    NetDataReader reader = new NetDataReader(rawData);
                    p.Deserialize(reader);
                    return;
                }
            }
        }
    }
}