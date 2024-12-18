﻿using MazeGenerator;
using LiteNetLib.Utils;
using System.Text;

namespace GameServer
{
    public enum GameModes
    {
        Classic,
        Deathmatch,
        Survival,
        DebugBikes,
        Debug
    }

    internal class GameModel : INetSerializable
    {
        private StringBuilder MAP = new();
        private const string bossMap = @"#########################...............##F###.................####..##...........##..###...=...........=...###...=.....E.....=...###...................###...................###.........#.........###...##.........##...###....#.........#....###...................###..#...##.#.##...#..####.....#.....#.....######...............##############d####################...#################E=...=E#################...#################$D.P.D$#################...################################",
            debugMap = @"######################...................##...................##..WWWW.1.2.3.4..#..##..W.EW.............##..WE.W..........d..##..WWWW.............##................=..##..L................##................S..##..l......P.........##................F..##.#b................##.###............#..##.#B............#d=.##................=..##...B=5#D#..........##..====#$#L####d##=###...=b.###.#.L..l#..##............#...L..######################",
            bikeMap = @"############################......######..555..#####........####.........###.......................##.......................##....####......####....=##...######....######...=##...######====#dd###...=##...##$###....#dd###...=##...##D###....######...=##...##.b##.....####....=##WWW##..##..............##EEE#F...d..............##WWW##..##..............##...##.B##.....####.....##...##D###....######....##...##$###....###dd#====##...######....###dd#....##...######....######....##....####......####.....##.......................##.......................###........####.......P.#####......######..555..############################",
            deathmatchMap = @"################$D..#..=.....##D#..L..#.....##....#....#...##B..b#....#.#=###==##...b=...##......##=#...##......=......##...#=##......##...=b...##==###=#.#....#b..B##...#....#....##.....#..L..#D##.....=..#..D$################",
            zombieModeMap = @"#####################....#@#...........##....#.#...........##..###T####.###....#####......#.#@#....##@.T......#.#.#....#####...######T##...##..#...T.@#....######..#...#T##....T.@###..##d##.......#########...#......##...##@.T...####d############....#...#......#####....d...####...##@.T....#...T.@#...#######T##...####...##....#.T....####...##....#@#....T.@#...##....###....####...#####################";
        private int inDebug = 0;
        private readonly Pet[] PETS;
        private readonly DisposableItem[] ITEMS;
        public List<Entity> Entities = [];
        private int Stage = 0;
        private int TotalTime = 0;
        private double EnemyDamageOffset = 1;
        private const double playerWidth = 0.4;
        private bool GameStarted = false;
        private readonly Random rand;
        private GameModes GameMode;
        private bool PVP = true;
        private int Difficulty;
        private int MAP_WIDTH, MAP_HEIGHT;
        private bool CUSTOM = false;
        private int CustomMazeHeight, CustomMazeWidth;
        private StringBuilder CUSTOM_MAP = new();
        private double CUSTOM_X, CUSTOM_Y;
        private static System.Timers.Timer? RespawnTimer;
        private static System.Timers.Timer? EntityTimer;
        private static System.Timers.Timer? TimeRemain;
        public int MaxEntityID;
        private readonly SendMessageFromGameCallback sendMessageFromGameCallback;

        public GameModel(SendMessageFromGameCallback sendMessage)
        {
            Pet[] pets = [new SillyCat(0, 0, 0, 0), new GreenGnome(0, 0, 0, 0), new EnergyDrink(0, 0, 0, 0), new Pyro(0, 0, 0, 0)];
            DisposableItem[] items = [new FirstAidKit(), new Adrenalin(), new Helmet()];
            PETS = pets;
            ITEMS = items;
            MAP_WIDTH = 16;
            MAP_HEIGHT = 16;
            rand = new();
            Difficulty = 3;
            this.sendMessageFromGameCallback = sendMessage;
            RespawnTimer = new System.Timers.Timer(1000);
            RespawnTimer.Elapsed += RespawnTimer_Tick;
            EntityTimer = new System.Timers.Timer(100);
            EntityTimer.Elapsed += EntityTimer_Tick;
            TimeRemain = new System.Timers.Timer(1000);
            TimeRemain.Elapsed += TimeRemain_Tick;
        }

        public void StartGame()
        {
            if (MAP.Length == 0) InitMap();
            MAP.Replace('P', '.');
            GameStarted = true;
            RespawnTimer?.Start();
            EntityTimer?.Start();
            TimeRemain?.Start();
            sendMessageFromGameCallback(102);
        }

        public bool IsGameStarted() => GameStarted;

        public int AddPlayer(string name)
        {
            if (MAP.Length > 0)
            {
                double X = 3, Y = 3;
                bool OK = false;
                if(GameMode == GameModes.Survival)
                {
                    X = Y = 5.5;
                    OK = true;
                }
                while (!OK)
                {
                    X = rand.Next(1, MAP_WIDTH - 1);
                    Y = rand.Next(1, MAP_HEIGHT - 1);
                    if (MAP[(int)Y * MAP_WIDTH + (int)X] == '.')
                        OK = true;
                }
                Player p = new(X + 0.5, Y + 0.5, MAP_WIDTH, ref MaxEntityID)
                {
                    Name = name,
                    PVP = PVP
                };
                if (Difficulty == 3 || Difficulty == 2)
                {
                    p.Guns[2].LevelUpdate();
                    ((DisposableItem)p.GUNS[10]).AddItem(2);
                }
                if (GameMode == GameModes.Debug || GameMode == GameModes.DebugBikes)
                    p.Money = 1234;
                Entities.Add(p);
            }
            else
            {
                Player p = new(1.5, 1.5, MAP_WIDTH, ref MaxEntityID)
                {
                    Name = name,
                    PVP = PVP
                };
                if (Difficulty == 3 || Difficulty == 2)
                {
                    p.Guns[2].LevelUpdate();
                    ((DisposableItem)p.GUNS[10]).AddItem(2);
                }
                if (GameMode == GameModes.Debug || GameMode == GameModes.DebugBikes)
                    p.Money = 1234;
                Entities.Add(p);
            }
            return MaxEntityID - 1;
        }

        private void EntityTimer_Tick(object? sender, EventArgs e)
        {
            List<Entity> targetsList = [];
            foreach (Entity ent in Entities)
            {
                if (ent is Player pl && !pl.Dead)
                    targetsList.Add(pl);
                if (ent is Covering co && !co.Broken)
                    targetsList.Add(co);
            }
            if (targetsList.Count == 0) return;
            for (int i = 0; i < Entities.Count; i++)
            {
                if (GameStarted)
                {
                    if (Entities[i] is Player) continue;
                    if (!Entities[i].HasAI) continue;
                    var entity = Entities[i] as dynamic;
                    var targetrListOrdered = targetsList.OrderBy((playerI) => Math.Pow(entity.X - playerI.X, 2) + Math.Pow(entity.Y - playerI.Y, 2));
                    Entity target = targetrListOrdered.First();
                    double distance = Math.Sqrt(Math.Pow(entity.X - target.X, 2) + Math.Pow(entity.Y - target.Y, 2));
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
                                        {
                                            playerTarget.DealDamage(damage * 1.5, true);
                                            if (playerTarget.HP <= 0)
                                            {
                                                if (GameMode == GameModes.Deathmatch)
                                                    Entities.Add(new PlayerDeadBody(playerTarget.X, playerTarget.Y, MAP_WIDTH, ref MaxEntityID));
                                                //TODO:
                                                //Stage = playerTarget.Stage;
                                                //TotalKilled = playerTarget.TotalEnemiesKilled;
                                                //DeathCause = GetDeathCause(entity);
                                                sendMessageFromGameCallback(666);
                                                return;
                                            }
                                        }
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
                            if (target is Player playerTarget)
                            {
                                if (!entity.DEAD && !playerTarget.Dead)
                                {
                                    entity.UpdateCoordinates(MAP.ToString(), target.X, target.Y);
                                    if (entity.Fast)
                                        entity.UpdateCoordinates(MAP.ToString(), target.X, target.Y);
                                    if (Math.Abs(entity.X - target.X) <= 0.5 && Math.Abs(entity.Y - target.Y) <= 0.5)
                                    {
                                        if (!playerTarget.Invulnerable)
                                        {
                                            if (playerTarget.InTransport)
                                            {
                                                NetDataWriter writer = new();
                                                writer.Put(target.ID);
                                                writer.Put(1);
                                                byte[] data = writer.Data;
                                                sendMessageFromGameCallback(1000, data);
                                            }
                                            else if (playerTarget.CuteMode)
                                            {
                                                NetDataWriter writer = new();
                                                writer.Put(target.ID);
                                                writer.Put(2);
                                                byte[] data = writer.Data;
                                                sendMessageFromGameCallback(1000, data);
                                            }
                                            else
                                            {
                                                NetDataWriter writer = new();
                                                writer.Put(target.ID);
                                                writer.Put(0);
                                                byte[] data = writer.Data;
                                                sendMessageFromGameCallback(1000, data);
                                            }
                                            GiveDebaf(playerTarget, entity);
                                            playerTarget.DealDamage(rand.Next(entity.MIN_DAMAGE, entity.MAX_DAMAGE), true);
                                            if (playerTarget.HP <= 0)
                                            {
                                                if (GameMode == GameModes.Deathmatch)
                                                    Entities.Add(new PlayerDeadBody(target.X, target.Y, MAP_WIDTH, ref MaxEntityID));
                                                //TODO:
                                                //Stage = target.Stage;
                                                //TotalKilled = target.TotalEnemiesKilled;
                                                //DeathCause = GetDeathCause(entity);
                                                sendMessageFromGameCallback(666);
                                                //GameOver(0);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (target is Covering coveringTarget)
                            {
                                if (!entity.DEAD && !coveringTarget.Broken)
                                {
                                    entity.UpdateCoordinates(MAP.ToString(), target.X, target.Y);
                                    if (entity.Fast)
                                        entity.UpdateCoordinates(MAP.ToString(), target.X, target.Y);
                                    if (Math.Abs(entity.X - target.X) <= 0.5 && Math.Abs(entity.Y - target.Y) <= 0.5)
                                        coveringTarget.DealDamage(rand.Next(entity.MIN_DAMAGE, entity.MAX_DAMAGE));
                                }
                            }
                        }
                    }
                    else if (entity is Pet pet)
                    {
                        Player? owner = null;
                        foreach (Entity ent in Entities)
                        {
                            if (ent is Player player1)
                            {
                                if (((Player)ent).PET == pet)
                                {
                                    owner = player1;
                                    distance = Math.Sqrt(Math.Pow(pet.X - target.X, 2) + Math.Pow(pet.Y - target.Y, 2));
                                }
                            }
                        }
                        if (owner != null)
                        {
                            if (distance > 1 && !(owner.GetCurrentGun() is Flashlight && entity.RespondsToFlashlight))
                                pet.UpdateCoordinates(MAP.ToString(), owner.X, owner.Y);
                            else
                                pet.Stoped = true;
                        }
                    }
                    else if (entity is Rockets rocket)
                    {
                        if (!entity.DEAD)
                            entity.UpdateCoordinates(MAP.ToString(), target.X, target.Y);
                        char[] ImpassibleCells = ['#', 'D', 'd', '='];
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
                                PlayGameSound((int)entity.Y * MAP_WIDTH + (int)entity.X);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static void GiveDebaf(Player player, Entity entity)
        {
            int effect_id = -1;
            if (entity is Zombie) effect_id = 3;
            else if (entity is Ogr) effect_id = 7;
            else if (entity is Dog) effect_id = 5;
            else if (entity is Bat) effect_id = 6;
            if (effect_id != -1)
            {
                if (!player.EffectCheck(effect_id))
                    player.GiveEffect(effect_id, true);
                else
                    player.ResetEffectTime(effect_id);
            }
        }

        private static int SetDeathCause(Entity entity)
        {
            if (entity is Zombie) return 0;
            else if (entity is Dog) return 1;
            else if (entity is Ogr) return 2;
            else if (entity is Bat) return 3;
            else if (entity is Explosion) return 4;
            else return -1;
        }

        public void RemovePlayer(int playerID)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] is Player)
                {
                    if (Entities[i].ID == playerID) Entities.RemoveAt(i);
                    return;
                }
            }
        }

        private void RespawnTimer_Tick(object? sender, EventArgs e)
        {
            List<Player> playersList = [];
            foreach (Entity ent in Entities)
            {
                if (ent is Player pl) playersList.Add(pl);
            }
            if (playersList.Count == 0) return;
            Parallel.For(0, Entities.Count, i =>
            {
                if (GameStarted)
                {
                    if (Entities[i] is not Enemy) return;
                    if (!Entities[i].HasAI) return;
                    var enemy = Entities[i] as dynamic;
                    _ = playersList.OrderBy((playerI) => Math.Pow(enemy.X - playerI.X, 2) + Math.Pow(enemy.Y - playerI.Y, 2));
                    Player player = playersList[0];
                    double distance = Math.Sqrt(Math.Pow(enemy.X - player.X, 2) + Math.Pow(enemy.Y - player.Y, 2));
                    if (distance <= 30)
                    {
                        if (Difficulty <= 1)
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

        public void SetCustom(bool custom, int CustomWidth, int CustomHeight, string CustomMap, int customX, int customY)
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
            List<Entity> entities = new(Entities);
            foreach (var entity in entities)
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
            List<Entity> tempEntities = [];
            for (int i = 0; i < entCount; i++)
            {
                int entityID = reader.GetInt();
                int ID = reader.GetInt();
                switch (entityID)
                {
                    case 0:
                        Player p = new(0, 0, MAP_WIDTH, ID);
                        p.Deserialize(reader);
                        tempEntities.Add(p);
                        break;
                    case 1:
                        Zombie man = new(0, 0, MAP_WIDTH, ID);
                        man.SetDamage(EnemyDamageOffset);
                        man.Deserialize(reader);
                        tempEntities.Add(man);
                        break;
                    case 2:
                        Dog dog = new(0, 0, MAP_WIDTH, ID);
                        dog.SetDamage(EnemyDamageOffset);
                        dog.Deserialize(reader);
                        tempEntities.Add(dog);
                        break;
                    case 3:
                        Ogr abomination = new(0, 0, MAP_WIDTH, ID);
                        abomination.SetDamage(EnemyDamageOffset);
                        abomination.Deserialize(reader);
                        tempEntities.Add(abomination);
                        break;
                    case 4:
                        Bat bat = new(0, 0, MAP_WIDTH, ID);
                        bat.SetDamage(EnemyDamageOffset);
                        bat.Deserialize(reader);
                        tempEntities.Add(bat);
                        break;
                    case 5:
                        SillyCat sillyCat = new(0, 0, MAP_WIDTH, ID);
                        sillyCat.Deserialize(reader);
                        tempEntities.Add(sillyCat);
                        break;
                    case 6:
                        GreenGnome greenGnome = new(0, 0, MAP_WIDTH, ID);
                        greenGnome.Deserialize(reader);
                        tempEntities.Add(greenGnome);
                        break;
                    case 7:
                        EnergyDrink energyDrink = new(0, 0, MAP_WIDTH, ID);
                        energyDrink.Deserialize(reader);
                        tempEntities.Add(energyDrink);
                        break;
                    case 8:
                        Pyro pyro = new(0, 0, MAP_WIDTH, ID);
                        pyro.Deserialize(reader);
                        tempEntities.Add(pyro);
                        break;
                    case 9:
                        Teleport teleport = new(0, 0, MAP_WIDTH, ID);
                        teleport.Deserialize(reader);
                        tempEntities.Add(teleport);
                        break;
                    case 10:
                        HittingTheWall hittingTheWall = new(0, 0, MAP_WIDTH, ID);
                        hittingTheWall.Deserialize(reader);
                        tempEntities.Add(hittingTheWall);
                        break;
                    case 11:
                        ShopDoor shopDoor = new(0, 0, MAP_WIDTH, ID);
                        shopDoor.Deserialize(reader);
                        tempEntities.Add(shopDoor);
                        break;
                    case 12:
                        ShopMan shopMan = new(0, 0, MAP_WIDTH, ID);
                        shopMan.Deserialize(reader);
                        tempEntities.Add(shopMan);
                        break;
                    case 13:
                        PlayerDeadBody playerDeadBody = new(0, 0, MAP_WIDTH, ID);
                        playerDeadBody.Deserialize(reader);
                        tempEntities.Add(playerDeadBody);
                        break;
                    case 14:
                        Box box = new(0, 0, MAP_WIDTH, ID);
                        box.Deserialize(reader);
                        tempEntities.Add(box);
                        break;
                    case 15:
                        Barrel barrel = new(0, 0, MAP_WIDTH, ID);
                        barrel.Deserialize(reader);
                        tempEntities.Add(barrel);
                        break;
                    case 16:
                        RpgRocket rpgRocket = new(0, 0, MAP_WIDTH, ID);
                        rpgRocket.Deserialize(reader);
                        tempEntities.Add(rpgRocket);
                        break;
                    case 17:
                        Explosion explosion = new(0, 0, MAP_WIDTH, ID);
                        explosion.Deserialize(reader);
                        tempEntities.Add(explosion);
                        break;
                    case 18:
                        Bike bike = new(0, 0, MAP_WIDTH, ID);
                        bike.Deserialize(reader);
                        tempEntities.Add(bike);
                        break;
                    case 19:
                        Vine vine = new(0, 0, MAP_WIDTH, ID);
                        vine.Deserialize(reader);
                        tempEntities.Add(vine);
                        break;
                    case 20:
                        Lamp lamp = new(0, 0, MAP_WIDTH, ID);
                        lamp.Deserialize(reader);
                        tempEntities.Add(lamp);
                        break;
                    case 22:
                        Covering covering = new(0,0, MAP_WIDTH, ID);
                        covering.Deserialize(reader);
                        tempEntities.Add(covering);
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
            List<Entity> tempEntities = [];
            for (int i = 0; i < entCount; i++)
            {
                int entityID = reader.GetInt();
                double entityX = reader.GetDouble();
                double entityY = reader.GetDouble();
                int ID = reader.GetInt();
                if (ID == playerID)
                {
                    foreach (Entity ent in Entities)
                    {
                        if (ent.ID == playerID)
                        {
                            tempEntities.Add(ent);
                            break;
                        }
                    }
                    continue;
                }
                switch (entityID)
                {
                    case 0:
                        Player p = new(0, 0, MAP_WIDTH, ID);
                        p.Deserialize(reader);
                        tempEntities.Add(p);
                        break;
                    case 1:
                        Zombie man = new(0, 0, MAP_WIDTH, ID);
                        man.SetDamage(EnemyDamageOffset);
                        man.Deserialize(reader);
                        tempEntities.Add(man);
                        break;
                    case 2:
                        Dog dog = new(0, 0, MAP_WIDTH, ID);
                        dog.SetDamage(EnemyDamageOffset);
                        dog.Deserialize(reader);
                        tempEntities.Add(dog);
                        break;
                    case 3:
                        Ogr abomination = new(0, 0, MAP_WIDTH, ID);
                        abomination.SetDamage(EnemyDamageOffset);
                        abomination.Deserialize(reader);
                        tempEntities.Add(abomination);
                        break;
                    case 4:
                        Bat bat = new(0, 0, MAP_WIDTH, ID);
                        bat.SetDamage(EnemyDamageOffset);
                        bat.Deserialize(reader);
                        tempEntities.Add(bat);
                        break;
                    case 5:
                        SillyCat sillyCat = new(0, 0, MAP_WIDTH, ID);
                        sillyCat.Deserialize(reader);
                        tempEntities.Add(sillyCat);
                        break;
                    case 6:
                        GreenGnome greenGnome = new(0, 0, MAP_WIDTH, ID);
                        greenGnome.Deserialize(reader);
                        tempEntities.Add(greenGnome);
                        break;
                    case 7:
                        EnergyDrink energyDrink = new(0, 0, MAP_WIDTH, ID);
                        energyDrink.Deserialize(reader);
                        tempEntities.Add(energyDrink);
                        break;
                    case 8:
                        Pyro pyro = new(0, 0, MAP_WIDTH, ID);
                        pyro.Deserialize(reader);
                        tempEntities.Add(pyro);
                        break;
                    case 9:
                        Teleport teleport = new(0, 0, MAP_WIDTH, ID);
                        teleport.Deserialize(reader);
                        tempEntities.Add(teleport);
                        break;
                    case 10:
                        HittingTheWall hittingTheWall = new(0, 0, MAP_WIDTH, ID);
                        hittingTheWall.Deserialize(reader);
                        tempEntities.Add(hittingTheWall);
                        break;
                    case 11:
                        ShopDoor shopDoor = new(0, 0, MAP_WIDTH, ID);
                        shopDoor.Deserialize(reader);
                        tempEntities.Add(shopDoor);
                        break;
                    case 12:
                        ShopMan shopMan = new(0, 0, MAP_WIDTH, ID);
                        shopMan.Deserialize(reader);
                        tempEntities.Add(shopMan);
                        break;
                    case 13:
                        PlayerDeadBody playerDeadBody = new(0, 0, MAP_WIDTH, ID);
                        playerDeadBody.Deserialize(reader);
                        tempEntities.Add(playerDeadBody);
                        break;
                    case 14:
                        Box box = new(0, 0, MAP_WIDTH, ID);
                        box.Deserialize(reader);
                        tempEntities.Add(box);
                        break;
                    case 15:
                        Barrel barrel = new(0, 0, MAP_WIDTH, ID);
                        barrel.Deserialize(reader);
                        tempEntities.Add(barrel);
                        break;
                    case 16:
                        RpgRocket rpgRocket = new(0, 0, MAP_WIDTH, ID);
                        rpgRocket.Deserialize(reader);
                        tempEntities.Add(rpgRocket);
                        break;
                    case 17:
                        Explosion explosion = new(0, 0, MAP_WIDTH, ID);
                        explosion.Deserialize(reader);
                        tempEntities.Add(explosion);
                        break;
                    case 18:
                        Bike bike = new(0, 0, MAP_WIDTH, ID);
                        bike.Deserialize(reader);
                        tempEntities.Add(bike);
                        break;
                    case 19:
                        Vine vine = new(0, 0, MAP_WIDTH, ID);
                        vine.Deserialize(reader);
                        tempEntities.Add(vine);
                        break;
                    case 20:
                        Lamp lamp = new(0, 0, MAP_WIDTH, ID);
                        lamp.Deserialize(reader);
                        tempEntities.Add(lamp);
                        break;
                    case 22:
                        Covering covering = new(0, 0, MAP_WIDTH, ID);
                        covering.Deserialize(reader);
                        tempEntities.Add(covering);
                        break;
                    default:
                        break;
                }
            }
            Entities = new List<Entity>(tempEntities);
        }

        public void AddTransport(int playerID, int index)
        {
            Player? p = GetPlayer(playerID);
            if (p == null) return;
            Transport? transport = index switch
            {
                0 => new Bike(p.X, p.Y, MAP_WIDTH, MaxEntityID),
                _ => null
            };
            if (transport != null)
            {
                AddTransportOnMap(transport.Index, (int)p.Y * MAP_WIDTH + (int)p.X);
                p.ChangeMoney(-transport.Cost);
                AddEntity(transport);
            }
        }

        private void AddTransportOnMap(int index, int map_index)
        {
            if (MAP[map_index] != '.') return;
            if (index == 0) MAP[map_index] = '5';
        }

        public void AddPet(int playerID, int index)
        {
            Player? player = GetPlayer(playerID);
            if (player == null) return;
            Pet pet = PETS[index];
            pet.ID = MaxEntityID;
            MaxEntityID++;
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] is Pet pet1)
                {
                    if (pet1.IsInstantAbility != 0)
                    {
                        switch (pet1.GetPetAbility())
                        {
                            case 1: //GreenGnome
                                player.MAX_HP -= 25;
                                player.HealHP(125);
                                break;
                            case 2: //Energy Drink
                                player.MAX_STAMINE -= 150;
                                player.MAX_MOVE_SPEED -= 0.15;
                                player.MAX_RUN_SPEED -= 0.1;
                                player.MAX_STRAFE_SPEED = player.MAX_MOVE_SPEED / 1.4;
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
                        player.MAX_MOVE_SPEED += 0.15;
                        player.MAX_RUN_SPEED += 0.1;
                        player.MAX_STRAFE_SPEED = player.MAX_MOVE_SPEED / 1.4;
                        player.STAMINE = player.MAX_STAMINE;
                        break;
                    case 3: //Pyro
                        player.CuteMode = true;
                        CuteMode(player);
                        break;
                }
            }
            player.PET = pet;
            player.Money -= pet.Cost;
            UpdatePet(player);
        }

        private Entity? GetEntityForInitMap(char c, int x, int y)
        {
            Entity? entity = null;
            switch (c)
            {
                case 'F':
                    Teleport teleport = new(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    entity = teleport;
                    break;
                case 'D':
                    ShopDoor shopDoor = new(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    entity = shopDoor;
                    break;
                case '$':
                    ShopMan shopMan = new(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    entity = shopMan;
                    break;
                case 'b':
                    Box box = new(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    entity = box;
                    break;
                case 'B':
                    Barrel barrel = new(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    entity = barrel;
                    break;
                case 'L':
                    Vine vine = new(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    entity = vine;
                    break;
                case 'l':
                    Lamp lamp = new(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    entity = lamp;
                    break;
                case '5':
                    Bike bike = new(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    entity = bike;
                    break;
                case 'E':
                    SpawnEnemis(x, y, MAP_WIDTH);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
                case 'e':
                    SpawnEnemis(x, y, MAP_WIDTH, false);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
                case '1':
                    SpawnEnemis(x, y, MAP_WIDTH, false, 0);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
                case '2':
                    SpawnEnemis(x, y, MAP_WIDTH, false, 1);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
                case '3':
                    SpawnEnemis(x, y, MAP_WIDTH, false, 2);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
                case '4':
                    SpawnEnemis(x, y, MAP_WIDTH, false, 3);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
                case 'T':
                    Covering covering = new(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    entity = covering;
                    break;
            }
            return entity;
        }

        private static void CuteMode(Player player)
        {
            player.Guns.Clear();
            if (player.CuteMode)
            {
                player.GUNS[11].HasIt = true;
                player.GUNS[12].HasIt = true;
                player.Guns.Add(player.GUNS[0]);
                player.Guns.Add(player.GUNS[11]);
                player.Guns.Add(player.GUNS[12]);
            }
            else
            {
                player.GUNS[11].HasIt = false;
                player.GUNS[12].HasIt = false;
                for (int i = 0; i < player.GUNS.Length; i++)
                {
                    if (player.GUNS[i].HasIt)
                        player.Guns.Add(player.GUNS[i]);
                }
            }
            player.CurrentGun = 2;
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
            EntityTimer?.Stop();
            RespawnTimer?.Stop();
            TimeRemain?.Stop();
            GameStarted = false;
            MAP.Clear();
            if (win == 1)
            {
                sendMessageFromGameCallback(101);
                List<Player> players = [];
                foreach (Entity ent in Entities)
                {
                    if (ent is not Player player) continue;
                    if (Difficulty != 4 && Difficulty != 6) Stage++;
                    if (!player.CuteMode)
                    {
                        for (int i = 0; i < player.Guns.Count; i++)
                        {
                            if (player.Guns[i].AmmoInStock == 0)
                                player.Guns[i].AmmoInStock = player.Guns[i].CartridgesClip;
                        }
                    }
                    player.ChangeMoney(50 + (5 * player.EnemiesKilled));
                    //StartGame();
                    //UpdatePet(player);
                    players.Add(player);
                }
                Entities.Clear();
                foreach(Player p in players)
                    Entities.Add(p);
                StartGame();
                foreach (Player p in players)
                    UpdatePet(p);
                sendMessageFromGameCallback(102);
                
            }
            else if (win == 0)
            {
                EntityTimer?.Stop();
                RespawnTimer?.Stop();
                TimeRemain?.Stop();
                List<(int, string)> playerIDs = [];
                foreach(Entity ent in Entities)
                {
                    if (ent is Player p)
                    {
                        string? name = p.Name;
                        name ??= "NoName";
                        playerIDs.Add((p.ID, name));
                    }
                }
                Entities.Clear();
                foreach((int, string) pInfo in playerIDs)
                {
                    Player p = new(1.5, 1.5, MAP_WIDTH, pInfo.Item1) { Name = pInfo.Item2 };
                    Entities.Add(p);
                }
                TotalTime = Stage = 0;
                sendMessageFromGameCallback(101);
            }
            else if (win == 2)
            {
                sendMessageFromGameCallback(101);
                List<Player> players = [];
                foreach (Entity ent in Entities)
                {
                    if (ent is not Player player) continue;
                    players.Add(player);
                }
                Entities.Clear();
                foreach (Player p in players) Entities.Add(p);
                StartGame();
                sendMessageFromGameCallback(102);
            }
            else if (win == 3)
            {
                EntityTimer?.Stop();
                RespawnTimer?.Stop();
                TimeRemain?.Stop();
                List<Player> players = [];
                foreach(Entity ent in Entities)
                {
                    if (ent is Player p) players.Add(p);
                }
                Entities.Clear();
                foreach (Player p in players) Entities.Add(p);
                TotalTime = Stage = 0;
                sendMessageFromGameCallback(101);
            }
            else
            {
                EntityTimer?.Stop();
                RespawnTimer?.Stop();
                TimeRemain?.Stop();
                Entities.Clear();
                MaxEntityID = 0;
                TotalTime = Stage = 0;
            }
            //StopGameHandle(win);
        }

        private static void GetFirstAidKit(Player player) => player.DisposableItems[0].AddItem();

        public void MovePlayer(double dX, double dY, double newA, double newLook, int playerID)
        {
            if (!GameStarted) return;
            Player? p = GetPlayer(playerID);
            if (p == null || p.BlockInput) return;
            p.X = dX;
            p.Y = dY;
            p.A = newA;
            p.Look = newLook;
            if (p.TRANSPORT != null)
            {
                if (p.EffectCheck(4) && p.TRANSPORT.CanJump)
                {
                    double extendedX = p.X + Math.Sin(p.A) * 0.3;
                    double extendedY = p.Y + Math.Cos(p.A) * 0.3;
                    if (MAP[(int)extendedY * MAP_WIDTH + (int)extendedX] == '=')
                    {
                        double distance = 1;
                        while (distance <= 2)
                        {
                            distance += 0.1d;
                            int x1 = (int)(p.X + Math.Sin(p.A) * distance);
                            int y1 = (int)(p.Y + Math.Cos(p.A) * distance);
                            if (!HasImpassibleCells(playerID, y1 * MAP_WIDTH + x1))
                            {
                                DoParkour(playerID, (int)extendedY, (int)extendedX);
                                return;
                            }
                        }
                    }
                }
            }
            if (MAP[(int)p.Y * MAP_WIDTH + (int)p.X] == 'F') GameOver(1);
        }

        public void GoDebug(int debug)
        {
            inDebug = debug;
            Difficulty = 5;
        }

        public void InitMap()
        {
            double enemy_count = 0;
            int MazeWidth = 0, MazeHeight = 0, MAX_SHOP_COUNT = 1;
            if (GameMode == GameModes.Deathmatch)
            {
                MAP_HEIGHT = 15;
                MAP_WIDTH = 15;
                MAP.Clear();
                MAP.AppendLine(deathmatchMap);
                double[,] pos = { { 3.5, 2.5 }, { 2.5, 3.5 }, { 12.5, 11.5 }, { 11.5, 12.5 } };
                int i = 0;
                foreach (Entity ent in Entities)
                {
                    if (ent is not Player player) continue;
                    player.X = pos[i, 0];
                    player.Y = pos[i, 1];
                    i++;
                    if (i >= pos.GetLength(0)) i = 0;
                }
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    for (int y = 0; y < MAP_HEIGHT; y++)
                    {
                        Entity? entity = GetEntityForInitMap(MAP[y * MAP_WIDTH + x], x, y);
                        if (entity != null) Entities.Add(entity);
                    }
                }
                return;
            }
            if (GameMode == GameModes.Survival)
            {
                MAP_HEIGHT = 20;
                MAP_WIDTH = 20;
                MAP.Clear();
                MAP.AppendLine(zombieModeMap);
                foreach (Entity ent in Entities)
                {
                    if (ent is not Player player) continue;
                    player.X = player.Y = 5.5;
                }
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    for (int y = 0; y < MAP_HEIGHT; y++)
                    {
                        Entity? entity = GetEntityForInitMap(MAP[y * MAP_WIDTH + x], x, y);
                        if (entity != null) Entities.Add(entity);
                    }
                }
                return;
            }
            if (GameMode == GameModes.DebugBikes)
            {
                MAP_HEIGHT = 25;
                MAP_WIDTH = 25;
                MAP.Clear();
                MAP.AppendLine(bikeMap);
                foreach (Entity ent in Entities)
                {
                    if (ent is not Player player) continue;
                    player.X = 21.5;
                    player.Y = 22.5;
                }
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    for (int y = 0; y < MAP_HEIGHT; y++)
                    {
                        Entity? entity = GetEntityForInitMap(MAP[y * MAP_WIDTH + x], x, y);
                        if (entity != null) Entities.Add(entity);
                    }
                }
                return;
            }
            if (GameMode == GameModes.Debug)
            {
                MAP_HEIGHT = 21;
                MAP_WIDTH = 21;
                MAP.Clear();
                MAP.AppendLine(debugMap);
                foreach (Entity ent in Entities)
                {
                    if (ent is not Player player) continue;
                    player.X = 10.5;
                    player.Y = 10.5;
                }
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    for (int y = 0; y < MAP_HEIGHT; y++)
                    {
                        Entity? entity = GetEntityForInitMap(MAP[y * MAP_WIDTH + x], x, y);
                        if (entity != null) Entities.Add(entity);
                    }
                }
                return;
            }
            if (Difficulty == 0)
                enemy_count = 0.07;
            else if (Difficulty == 1)
                enemy_count = 0.065;
            else if (Difficulty == 2)
                enemy_count = 0.055;
            else if (Difficulty == 3)
                enemy_count = 0.045;
            else if (Difficulty == 4)
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
                    MazeHeight = 21;
                    MazeWidth = 21;
                }
                else if (inDebug == 2)
                {
                    MazeHeight = 22;
                    MazeWidth = 22;
                }
                else if (inDebug == 3)
                {
                    MazeHeight = 25;
                    MazeWidth = 25;
                }
            }
            if (Difficulty < 4)
            {
                MazeHeight = MazeWidth = 10;
                MAX_SHOP_COUNT = 2;
            }
            foreach (Entity ent in Entities)
            {
                if (ent is not Player player) continue;
                if (Difficulty == 0)
                    enemy_count = 0.07;
                else if (Difficulty == 1)
                    enemy_count = 0.065;
                else if (Difficulty == 2)
                {
                    enemy_count = 0.055;
                    if (Stage == 0 && player.Guns[2].Level==Levels.LV1)
                    {
                        player.Guns[2].LevelUpdate();
                        ((DisposableItem)player.GUNS[10]).AddItem(2);
                    }
                }
                else if (Difficulty == 3)
                {
                    enemy_count = 0.045;
                    if (Stage == 0 && player.Guns[2].Level==Levels.LV1)
                    {
                        player.Guns[2].LevelUpdate();
                        ((DisposableItem)player.GUNS[10]).AddItem(2);
                    }
                }
                else if (Difficulty == 4)
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
                        player.X = 10.5;
                        player.Y = 10.5;
                        MazeHeight = 21;
                        MazeWidth = 21;
                    }
                    else if (inDebug == 2)
                    {
                        player.X = 10.5;
                        player.Y = 19.5;
                        MazeHeight = 22;
                        MazeWidth = 22;
                    }
                    else if (inDebug == 3)
                    {
                        player.X = 21.5;
                        player.Y = 22.5;
                        MazeHeight = 25;
                        MazeWidth = 25;
                    }
                }
                if (Difficulty < 4)
                {
                    if (Stage == 0)
                    {
                        MazeHeight = MazeWidth = 10;
                        MAX_SHOP_COUNT = 2;
                    }
                    else if (Stage == 1)
                    {
                        MazeHeight = MazeWidth = 15;
                        MAX_SHOP_COUNT = 4;
                    }
                    else if (Stage == 2)
                    {
                        MazeHeight = MazeWidth = 20;
                        MAX_SHOP_COUNT = 6;
                    }
                    else if (Stage == 3)
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
            if (Difficulty == 5)
            {
                MAP_WIDTH = MazeWidth;
                MAP_HEIGHT = MazeHeight;
            }
            else
            {
                MAP_WIDTH = MazeWidth * 3 + 1;
                MAP_HEIGHT = MazeHeight * 3 + 1;
            }
            MAP.Clear();
            if (Difficulty == 5)
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
                        Entity? entity = GetEntityForInitMap(MAP[y * MAP_WIDTH + x], x, y);
                        if (entity != null)
                            Entities.Add(entity);
                    }
                }
            }
            else
            {
                if (!CUSTOM)
                {
                    Random random = new();
                    StringBuilder sb = new();
                    char[,] map = (new Maze()).GenerateCharMap(MazeWidth, MazeHeight, '#', '=', 'd', '.', 'F', MAX_SHOP_COUNT);
                    map[1, 1] = 'P';
                    List<int[]> shops = [];
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
                                shops.Add([x, y]);
                        }
                    }
                    if (shops.Count == 0)
                    {
                        if (map[3, 1] == '#')
                        {
                            map[3, 1] = '$';
                            shops.Add([3, 1]);
                        }
                        else if (map[1, 3] == '#')
                        {
                            map[1, 3] = '$';
                            shops.Add([1, 3]);
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
                                Entity? entity = GetEntityForInitMap(map[x, y], x, y);
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
                            Entity? entity = GetEntityForInitMap(MAP[y * (CustomMazeWidth * 3 + 1) + x], x, y);
                            if (entity != null)
                                Entities.Add(entity);
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
                if (ent is not Player player) continue;
                if (CUSTOM)
                {
                    player.X = CUSTOM_X;
                    player.Y = CUSTOM_Y;
                    continue;
                }
                if (Difficulty != 5)
                {
                    player.X = 1.5;
                    player.Y = 1.5;
                }
            };
        }

        public void SpawnRockets(double x, double y, int id, double a)
        {
            Rockets? rocket = null;
            if (id == 0)
                rocket = new RpgRocket(x, y, MAP_WIDTH, ref MaxEntityID);
            if (rocket == null) return;
            rocket.X += Math.Sin(a);
            rocket.Y += Math.Cos(a);
            if (rocket != null)
            {
                rocket.SetA(a);
                Entities.Add(rocket);
            }
        }

        private void SpawnEnemis(double x, double y, int size, bool ai = true, int type = -1)
        {
            x += 0.5;
            y += 0.5;
            if (type == -1)
            {
                double dice = rand.NextDouble();
                if (dice <= 0.35) // 35%
                {
                    Zombie enemy = new(x, y, size, ref MaxEntityID);
                    enemy.SetDamage(EnemyDamageOffset);
                    enemy.HasAI = ai;
                    Entities.Add(enemy);
                }
                else if (dice > 0.35 && dice <= 0.6) // 25%
                {
                    Dog enemy = new(x, y, size, ref MaxEntityID);
                    enemy.SetDamage(EnemyDamageOffset);
                    enemy.HasAI = ai;
                    Entities.Add(enemy);
                }
                else if (dice > 0.6 && dice <= 0.8) // 20%
                {
                    Bat enemy = new(x, y, size, ref MaxEntityID);
                    enemy.SetDamage(EnemyDamageOffset);
                    enemy.HasAI = ai;
                    Entities.Add(enemy);
                }
                else if (dice > 0.8 && dice <= 0.95) // 15%
                {
                    Ogr enemy = new(x, y, size, ref MaxEntityID);
                    enemy.SetDamage(EnemyDamageOffset);
                    enemy.HasAI = ai;
                    Entities.Add(enemy);
                }
                else
                {
                    if (rand.NextDouble() <= 0.5)
                    {
                        Vine vine = new(x, y, size, ref MaxEntityID);
                        Entities.Add(vine);
                    }
                    else
                    {
                        Lamp lamp = new(x, y, size, ref MaxEntityID);
                        Entities.Add(lamp);
                    }
                }
            }
            else if (type == 0)
            {
                Zombie enemy = new(x, y, size, ref MaxEntityID);
                enemy.SetDamage(EnemyDamageOffset);
                enemy.HasAI = ai;
                Entities.Add(enemy);
            }
            else if (type == 1)
            {
                Dog enemy = new(x, y, size, ref MaxEntityID);
                enemy.SetDamage(EnemyDamageOffset);
                enemy.HasAI = ai;
                Entities.Add(enemy);
            }
            else if (type == 2)
            {
                Ogr enemy = new(x, y, size, ref MaxEntityID);
                enemy.SetDamage(EnemyDamageOffset);
                enemy.HasAI = ai;
                Entities.Add(enemy);
            }
            else if (type == 3)
            {
                Bat enemy = new(x, y, size, ref MaxEntityID);
                enemy.SetDamage(EnemyDamageOffset);
                enemy.HasAI = ai;
                Entities.Add(enemy);
            }
            else if (type == 4)
            {
                Bike bike = new(x, y, size, ref MaxEntityID);
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

        internal Entity? GetEntity(int id)
        {
            Entity? entity = null;
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
            Entity? target = null;
            Entity? attacker = null;
            foreach (Entity entity in Entities)
            {
                if (entity.ID == ID) target = entity;
                if (entity.ID == attackerID) attacker = entity;
            }
            if (target is Player p)
            {
                if (!p.HasAI || p.Dead || !p.PVP) return false;
                if (attacker is Player attackerPlayer && p.DealDamage(damage, true))
                {
                    double multiplier = 1;
                    if (Difficulty == 3)
                        multiplier = 1.5;
                    attackerPlayer.ChangeMoney(rand.Next((int)(20 * multiplier), (int)(30 * multiplier)));
                    attackerPlayer.EnemiesKilled++;
                    attackerPlayer.TotalEnemiesKilled++;
                    Entities.Add(new PlayerDeadBody(p.X, p.Y, MAP_WIDTH, ref MaxEntityID));
                    NetDataWriter writer = new();
                    writer.Put(p.ID);
                    sendMessageFromGameCallback(103, writer.Data);
                    return true;
                }
                return false;
            }
            if (target is Creature c)
            {
                if (!c.HasAI || c.DEAD || !c.CanHit) return false;
                if (c.DealDamage(damage))
                {
                    if (attacker is Player attackerPlayer)
                    {
                        double multiplier = 1;
                        if (Difficulty == 3)
                            multiplier = 1.5;
                        attackerPlayer.ChangeMoney(rand.Next((int)(c.MIN_MONEY * multiplier), (int)(c.MAX_MONEY * multiplier)));
                        attackerPlayer.EnemiesKilled++;
                        attackerPlayer.TotalEnemiesKilled++;
                        if (target is Boxes boxes && !attackerPlayer.CuteMode)
                        {
                            if (boxes.BoxWithMoney)
                                attackerPlayer.ChangeMoney(rand.Next(5, 11));
                            else
                            {
                                int count = attackerPlayer.Guns.Count;
                                int type = rand.Next(1, count);
                                int max = attackerPlayer.Guns[type].MaxAmmo;
                                int ammo = attackerPlayer.Guns[type].CartridgesClip + attackerPlayer.Guns[type].AmmoInStock;
                                if (ammo > max)
                                {
                                    ammo = max;
                                    attackerPlayer.Money += rand.Next(5, 11);
                                }
                                attackerPlayer.Guns[type].AmmoInStock = ammo;
                            }
                        }
                        NetDataWriter writer = new();
                        writer.Put(c.X);
                        writer.Put(c.Y);
                        writer.Put(c.DeathSound);
                        sendMessageFromGameCallback(1001, writer.Data);
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

        private void TimeRemain_Tick(object? sender, EventArgs e)
        {
            bool areAllThePlayersDead = true;
            TotalTime++;
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] is not Player player) continue;
                if (!player.Dead) areAllThePlayersDead = false;
                else
                {
                    if(GameMode == GameModes.Deathmatch)
                    {
                        player.SetDefault();
                        double X = 1.5, Y = 1.5;
                        bool OK = false;
                        while (!OK)
                        {
                            X = rand.Next(1, MAP_WIDTH - 1);
                            Y = rand.Next(1, MAP_HEIGHT - 1);
                            if (MAP[(int)Y * MAP_WIDTH + (int)X] == '.')
                                OK = true;
                        }
                        player.X = X + 0.5; player.Y = Y + 0.5;
                        NetDataWriter writer = new();
                        writer.Put(player.ID);
                        sendMessageFromGameCallback(103, writer.Data);
                    }
                }
                if (player.Invulnerable)
                    player.InvulnerableEnd();
                player.UpdateEffectsTime();
                Pet? playerPet = player.PET;
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
            if (GameMode == GameModes.Classic)
            {
                if (areAllThePlayersDead)
                    GameOver(0);
            }
        }

        public void AddHittingTheWall(double X, double Y, double playerLook)
        {
            HittingTheWall hittingTheWall = new(X, Y, MAP_WIDTH, ref MaxEntityID)
            {
                VMove = playerLook
            };
            Entities.Add(hittingTheWall);
        }

        internal void ChangePlayerA(double v, int playerID)
        {
            Player? p = GetPlayer(playerID);
            if (p == null) return;
            if (p.BlockCamera)
            {
                if (p.CanUnblockCamera)
                {
                    p.BlockCamera = false;
                    p.CanUnblockCamera = false;
                }
                return;
            }
            p.A += v;
            if (p.A >= Math.PI * 2) p.A = 0;
            if (p.A <= -Math.PI * 2) p.A = 0;
        }

        internal void ChangePlayerLook(double lookDif, int playerID)
        {
            Player? p = GetPlayer(playerID);
            if (p == null) return;
            if (p.BlockCamera)
            {
                if (p.CanUnblockCamera)
                {
                    p.BlockCamera = false;
                    p.CanUnblockCamera = false;
                }
                return;
            }
            p.Look += lookDif;
            if (p.Look < -360) p.Look = -360;
            else if (p.Look > 360) p.Look = 360;
        }

        internal void StopGame(int win) => GameOver(win);

        internal void AmmoCountDecrease(int playerID)
        {
            foreach (Entity entity in Entities)
            {
                if (entity.ID == playerID)
                {
                    Gun gun = ((Player)entity).GetCurrentGun();
                    int ammo = gun is SubmachineGun && gun.Level == Levels.LV3 ? 2 : 1;
                    gun.AmmoCount -= ammo;
                    return;
                }
            }
        }

        internal void Reload(int playerID)
        {
            foreach (Entity entity in Entities)
            {
                if (entity.ID == playerID)
                {
                    Player? player = (Player)entity;
                    player.GetCurrentGun().ReloadClip();
                    return;
                }
            }
        }

        internal void OnOffNoClip(int playerID)
        {
            foreach (Entity ent in Entities)
            {
                if (ent.ID == playerID)
                    ((Player)ent).NoClip = !((Player)ent).NoClip;
            }
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

        internal void GettingOffTheTransport(int playerID, bool add_transport = true)
        {
            Player? p = GetPlayer(playerID);
            if (p == null || p.BlockInput) return;
            Transport? transport = null;
            if (p.TRANSPORT is Bike)
                transport = new Bike(p.X, p.Y, MAP_WIDTH, MaxEntityID)
                {
                    TransportHP = p.TRANSPORT_HP,
                    A = p.A
                };
            p.StopEffect(4);
            if (transport != null)
            {
                if (add_transport)
                {
                    AddTransportOnMap(transport.Index, (int)p.Y * MAP_WIDTH + (int)p.X);
                    AddEntity(transport);
                }
            }
        }

        internal bool HasImpassibleCells(int index, int playerID)
        {
            Player? p = GetPlayer(playerID);
            if (p == null) return false;
            char[] impassibleCells = ['#', 'D', '=', 'd', 'S', '$'];
            if (HasNoClip(playerID) || p.InParkour) return false;
            return impassibleCells.Contains(GetMap()[index]);
        }

        internal string GetTotalTime()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(TotalTime);
            return timeSpan.ToString(@"hh:mm:ss");
        }

        internal void DrawItem(int playerID)
        {
            Player? p = GetPlayer(playerID);
            if (p == null || p.DISPOSABLE_ITEM == null) return;
            if (p.DISPOSABLE_ITEM.HasLVMechanics)
            {
                if (p.CuteMode)
                    p.DISPOSABLE_ITEM.Level = Levels.LV4;
                else
                {
                    if (rand.NextDouble() <= p.CurseCureChance)
                    {
                        if (rand.NextDouble() <= 0.5)
                            p.DISPOSABLE_ITEM.Level = Levels.LV2;
                        else
                            p.DISPOSABLE_ITEM.Level = Levels.LV3;
                    }
                    else
                        p.DISPOSABLE_ITEM.Level = Levels.LV1;
                }
            }
            else
                p.DISPOSABLE_ITEM.Level = Levels.LV1;
            p.ItemFrame = 0;
            p.GunState = 1;
            p.Aiming = p.CanShoot = false;
            p.UseItem = true;
        }

        internal void UseItem(int playerID)
        {
            Player? p = GetPlayer(playerID);
            if (p == null || p.DISPOSABLE_ITEM == null) return;
            new Thread(() =>
            {
                while (p.UseItem)
                {
                    if (p.ItemFrame > p.DISPOSABLE_ITEM.ReloadFrames)
                    {
                        p.SetEffect();
                        p.DISPOSABLE_ITEM.Count--;
                        if (p.DISPOSABLE_ITEM.Count == 0)
                            p.DISPOSABLE_ITEM.SetDefault();
                        p.GunState = p.MoveStyle;
                        p.CanShoot = true;
                    }
                    Thread.Sleep(p.DISPOSABLE_ITEM.RechargeTime);
                    p.ItemFrame++;
                }
            }).Start();
        }

        internal void ChangeItem(int playerID, int index)
        {
            Player? p = GetPlayer(playerID);
            p?.ChangeItem(index);
        }

        internal Player? GetPlayer(int playerID)
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
            Player? p = GetPlayer(playerID);
            if (p == null) return false;
            if (p.EffectCheck(3)) return false;
            //TODO:
            //if (p.InTransport)
            //    PlayGameSound(playerID, SLIL.climb[1]);
            //else
            //    PlayGameSound(playerID, SLIL.climb[0]);
            p.InParkour = true;
            p.ParkourState = 0;
            p.X = x + 0.5;
            p.Y = y + 0.5;
            p.Look = 0;
            p.CanUnblockCamera = false;
            p.BlockCamera = p.BlockInput = true;
            p.PlayerMoveStyle = Directions.WALK;
            p.CanShoot = false;
            NetDataWriter writer = new();
            writer.Put(p.ID);
            p.Serialize(writer);
            sendMessageFromGameCallback(1334, writer.Data);
            new Thread(() => {
                Thread.Sleep(p.InTransport ? 250 : 375);
                StopParkour(playerID);
                p.ParkourState++;
                writer = new NetDataWriter();
                writer.Put(p.ID);
                p.Serialize(writer);
                sendMessageFromGameCallback(1334, writer.Data);
            }).Start();
            return true;
        }

        internal void StopParkour(int playerID)
        {
            Player? p = GetPlayer(playerID);
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
            p.BlockInput = false;
            p.BlockCamera = false;
            p.CanShoot = true;
        }

        internal void ChangeWeapon(int playerID, int new_gun)
        {
            Player? p = GetPlayer(playerID);
            if (p != null) p.CurrentGun = new_gun;
        }

        internal void SetWeaponSlot(int playerID, int slot, int index)
        {
            Player? p = GetPlayer(playerID);
            if (p == null) return;
            if (slot == 0)
            {
                p.WeaponSlot_0 = index;
                p.CurrentGun = p.PreviousGun = p.WeaponSlot_0;
                if (p.WeaponSlot_0 == p.WeaponSlot_1) p.WeaponSlot_1 = -1;
            }
            else
            {
                p.WeaponSlot_1 = index;
                p.CurrentGun = p.PreviousGun = p.WeaponSlot_1;
                if (p.WeaponSlot_0 == p.WeaponSlot_1) p.WeaponSlot_0 = -1;
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
                        p.Guns.Add(weapon);
                        weapon.HasIt = true;
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
            Player? p = GetPlayer(playerID);
            if (p == null) return;
            DisposableItem item = (DisposableItem)p.GUNS[itemID];
            if (p.Money >= item.GunCost && item.CanBuy())
            {
                p.ChangeMoney(-item.GunCost);
                item.AddItem();
            }
        }

        internal void PlayGameSound(int coordinate)
        {
            //TODO:
            double X = coordinate % MAP_HEIGHT;
            double Y = (coordinate - X) / MAP_WIDTH;
        }

        internal void RepairCovering(int id)
        {
            Entity? entity = GetEntity(id);
            if (entity == null) return;
            Covering covering = (Covering)entity;
            covering.Repair(25);
        }

        internal void InteractingWithDoors(int coordinate)
        {
            double X = coordinate % MAP_HEIGHT;
            double Y = (coordinate - X) / MAP_WIDTH;
            NetDataWriter writer = new();
            writer.Put(X); 
            writer.Put(Y);
            if (MAP[coordinate] == 'o')
            {
                MAP[coordinate] = 'd';
                sendMessageFromGameCallback(1, writer.Data);
            }
            else
            {
                MAP[coordinate] = 'o';
                sendMessageFromGameCallback(2, writer.Data);
            }
        }

        internal bool GetPVP() => PVP;

        internal void SetPVP(bool enabled) => PVP = enabled;

        internal void ChangeDifficulty(int difficulty)
        {
            this.Difficulty = difficulty;
            EnemyDamageOffset = difficulty switch
            {
                0 => 1.75,
                1 => 1.25,
                2 => 1,
                3 => 0.75,
                _ => 1,
            };
            if (GameStarted) StopGame(2);
        }

        internal void ChangeGameMode(GameModes gameMode)
        {
            GameMode = gameMode;
            if (GameStarted) StopGame(2);
        }

        internal void GetOnATransport(int ID, int playerID)
        {
            Player? p = GetPlayer(playerID);
            if (p == null) return;
            Entity? entity = GetEntity(ID);
            if (entity != null)
            {
                if (MAP[(int)entity.Y * MAP_WIDTH + (int)entity.X] == '5')
                    MAP[(int)entity.Y * MAP_WIDTH + (int)entity.X] = '.';
                p.TRANSPORT = (Transport)entity;
                p.TRANSPORT_HP = ((Transport)entity).TransportHP;
                p.A = ((Transport)entity).A;
                p.X = entity.X;
                p.Y = entity.Y;
            }
            RemoveEntity(ID);
            p.GiveEffect(4, true);
        }
    }
}