using MazeGenerator;
using LiteNetLib.Utils;
using System.Text;
using GameServer;

namespace SLIL.Classes
{
    internal class GameModel : INetSerializable
    {
        private StringBuilder MAP = new();
        private const string bossMap = @"#########################...............##F###.................####..##...........##..###...=...........=...###...=.....E.....=...###...................###...................###.........#.........###...##.........##...###....#.........#....###...................###..#...##.#.##...#..####.....#.....#.....######...............##############d####################...#################E=...=E#################...#################$D.P.D$#################...################################",
            debugMap = @"####################.................##..WWWW...........##..W..W..B..b..#..##..WE.W...........##..W..W...........##..W..W........d..##..W.EW...........##..WWWW...........##........P.....=..##..#b.............##..###............##..#B..........F..##.................##..WWW.B=.#D#..#..##..WEW====#$#.#d=.##..WWW.=b.###..=..##.................####################";
        private int inDebug = 0;
        private readonly Pet[] PETS;
        public List<Entity> Entities = [];
        private readonly char[] impassibleCells = { '#', 'D', '=', 'd' };
        private const double playerWidth = 0.4;
        private bool GameStarted = false;
        private readonly Random rand;
        private int difficulty;
        private int seconds, minutes;
        private int MAP_WIDTH, MAP_HEIGHT;
        private bool CUSTOM = false;
        private int CustomMazeHeight, CustomMazeWidth;
        private StringBuilder CUSTOM_MAP = new();
        private int CUSTOM_X, CUSTOM_Y;
        private static System.Timers.Timer RespawnTimer;
        private static System.Timers.Timer EnemyTimer;
        private static System.Timers.Timer TimeRemain;
        public int MaxEntityID;

        SendMessageFromGameCallback sendMessageFromGameCallback;

        public GameModel(SendMessageFromGameCallback sendMessage)
        {
            Pet[] pets = { new SillyCat(0, 0, 0, 0), new GreenGnome(0, 0, 0, 0), new EnergyDrink(0, 0, 0, 0), new Pyro(0, 0, 0, 0) };
            PETS = pets;
            MAP_WIDTH = 16;
            MAP_HEIGHT = 16;
            rand = new Random();
            difficulty = 3;
            this.sendMessageFromGameCallback = sendMessage;
            RespawnTimer = new System.Timers.Timer(1000);
            RespawnTimer.Elapsed += RespawnTimer_Tick;
            EnemyTimer = new System.Timers.Timer(100);
            EnemyTimer.Elapsed += EnemyTimer_Tick;
            TimeRemain = new System.Timers.Timer(1000);
            TimeRemain.Elapsed += TimeRemain_Tick;
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

        public void StartGame()
        {
            if (inDebug == 0) difficulty = 3;
            else difficulty = 5;
            //if (Entities.Count == 0) SetPlayerID(AddPlayer());
            if (MAP.Length == 0) InitMap();
            GameStarted = true;
            RespawnTimer.Start();
            EnemyTimer.Start();
            TimeRemain.Start();
        }

        public bool IsGameStarted() => GameStarted;

        public int AddPlayer()
        {
            if (MAP.Length > 0)
            {
                double X = 3, Y = 3;
                bool OK = false;
                while (!OK)
                {
                    X = rand.Next(1, MAP_WIDTH - 1);
                    Y = rand.Next(1, MAP_HEIGHT - 1);
                    if (MAP[(int)Y * MAP_WIDTH + (int)X] == '.')
                    {
                        OK = true;
                    }
                }
                Player p = new Player(X + 0.5, Y + 0.5, MAP_WIDTH, ref MaxEntityID);
                if (difficulty == 3 || difficulty == 2)
                {
                    p.Guns[1].LevelUpdate();
                }
                Entities.Add(p);
            }
            else
            {
                Player p = new Player(1.5, 1.5, MAP_WIDTH, ref MaxEntityID);
                if (difficulty == 3 || difficulty == 2)
                {
                    p.Guns[1].LevelUpdate();
                }
                Entities.Add(p);
            }
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
                    var entity = Entities[i] as dynamic;
                    playersList.OrderBy((playerI) => Math.Pow(entity.X - playerI.X, 2) + Math.Pow(entity.Y - playerI.Y, 2));
                    Player player = playersList[0];
                    double distance = Math.Sqrt(Math.Pow(entity.X - player.X, 2) + Math.Pow(entity.Y - player.Y, 2));
                    if (entity is GameObject && entity.Temporarily)
                    {
                        entity.LifeTime++;
                        entity.CurrentFrame++;
                        if (entity.LifeTime >= entity.TotalLifeTime)
                        {
                            Entities.Remove(entity);
                            continue;
                        }
                    }
                    if (entity is Enemy)
                    {
                        if (distance <= 22)
                        {
                            if (!entity.DEAD)
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
                                        {
                                            //PlaySoundHandle(SLIL.hungry);
                                        }
                                        else
                                        {
                                            //PlaySoundHandle(SLIL.hit);
                                        }
                                        if (player.HP <= 0)
                                        {
                                            Entities.Add(new PlayerDeadBody(player.X, player.Y, MAP_WIDTH, ref MaxEntityID));
                                            //GameOver(0);
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
                            if (ent is Player)
                            {
                                if ((ent as Player).PET == entity)
                                {
                                    owner = (Player)ent;
                                    distance = Math.Sqrt(Math.Pow(entity.X - player.X, 2) + Math.Pow(entity.Y - player.Y, 2));
                                }
                            }
                        }
                        if (distance > 1 && !(owner.GetCurrentGun() is Flashlight && entity.RespondsToFlashlight))
                            entity.UpdateCoordinates(MAP.ToString(), owner.X, owner.Y);
                        else
                            entity.Stoped = true;
                    }
                }
            }
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
                        man.Deserialize(reader);
                        tempEntities.Add(man);
                        break;
                    case 2:
                        Dog dog = new Dog(0, 0, MAP_WIDTH, ID);
                        dog.Deserialize(reader);
                        tempEntities.Add(dog);
                        break;
                    case 3:
                        Abomination abomination = new Abomination(0, 0, MAP_WIDTH, ID);
                        abomination.Deserialize(reader);
                        tempEntities.Add(abomination);
                        break;
                    case 4:
                        Bat bat = new Bat(0, 0, MAP_WIDTH, ID);
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
                        tempEntities.Add(new Player(entityX, entityY, MAP_WIDTH, ID));
                        break;
                    case 1:
                        tempEntities.Add(new Man(entityX, entityY, MAP_WIDTH, ID));
                        break;
                    case 2:
                        tempEntities.Add(new Dog(entityX, entityY, MAP_WIDTH, ID));
                        break;
                    case 3:
                        tempEntities.Add(new Abomination(entityX, entityY, MAP_WIDTH, ID));
                        break;
                    case 4:
                        tempEntities.Add(new Bat(entityX, entityY, MAP_WIDTH, ID));
                        break;
                    case 5:
                        tempEntities.Add(new SillyCat(entityX, entityY, MAP_WIDTH, ID));
                        break;
                    case 6:
                        tempEntities.Add(new GreenGnome(entityX, entityY, MAP_WIDTH, ID));
                        break;
                    case 7:
                        tempEntities.Add(new EnergyDrink(entityX, entityY, MAP_WIDTH, ID));
                        break;
                    case 8:
                        tempEntities.Add(new Pyro(entityX, entityY, MAP_WIDTH, ID));
                        break;
                    case 9:
                        tempEntities.Add(new Teleport(entityX, entityY, MAP_WIDTH, ID));
                        break;
                    case 10:
                        tempEntities.Add(new HittingTheWall(entityX, entityY, MAP_WIDTH, ID));
                        break;
                    case 11:
                        tempEntities.Add(new ShopDoor(entityX, entityY, MAP_WIDTH, ID));
                        break;
                    case 12:
                        tempEntities.Add(new ShopMan(entityX, entityY, MAP_WIDTH, ID));
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
            //foreach (SLIL_PetShopInterface control in pet_shop_page.Controls.Find("SLIL_PetShopInterface", true))
            //control.buy_button.Text = MainMenu.Language ? $"Купить ${control.pet.Cost}" : $"Buy ${control.pet.Cost}";
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

        private void CuteMode(Player player)
        {
            player.Guns.Clear();
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
            //TakeFlashlight(false);
            //ChangeWeapon(1);
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
                    if (difficulty != 4)
                        player.Stage++;
                    if (!player.CuteMode)
                    {
                        for (int i = 0; i < player.Guns.Count; i++)
                        {
                            if (player.Guns[i].MaxAmmoCount == 0)
                                player.Guns[i].MaxAmmoCount = player.Guns[i].CartridgesClip;
                        }
                    }
                    player.ChangeMoney(50 + (5 * player.EnemiesKilled));
                    StartGame();
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
            //StopGameHandle(win);
        }

        private void GetFirstAidKit(Player player) => player.DisposableItems[0].AddItem();

        public void MovePlayer(double dX, double dY, int playerID)
        {
            if (!GameStarted) return;
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] is Player)
                {
                    if ((Entities[i] as Player).ID == playerID)
                    {
                        Entities[i].X = dX;
                        Entities[i].Y = dY;
                        if (MAP[(int)Entities[i].Y * MAP_WIDTH + (int)Entities[i].X] == 'F')
                        {
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

        public void InitMap()
        {
            seconds = 0;
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
                    MazeHeight = 6;
                    MazeWidth = 6;
                }
                else if (inDebug == 2)
                {
                    MazeHeight = 7;
                    MazeWidth = 7;
                }
            }
            minutes = 9999;
            if (difficulty != 4 && difficulty != 5)
            {
                minutes = 5;
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
                    if (player.Stage == 0)
                        player.Guns[1].LevelUpdate();
                }
                else if (difficulty == 3)
                {
                    enemy_count = 0.045;
                    if (player.Stage == 0)
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
                }
                minutes = 9999;
                if (difficulty != 4 && difficulty != 5)
                {
                    if (player.Stage == 0)
                    {
                        minutes = 5;
                        MazeHeight = MazeWidth = 10;
                        MAX_SHOP_COUNT = 2;
                    }
                    else if (player.Stage == 1)
                    {
                        minutes = 10;
                        MazeHeight = MazeWidth = 15;
                        MAX_SHOP_COUNT = 4;
                    }
                    else if (player.Stage == 2)
                    {
                        minutes = 15;
                        MazeHeight = MazeWidth = 20;
                        MAX_SHOP_COUNT = 6;
                    }
                    else if (player.Stage == 3)
                    {
                        minutes = 20;
                        MazeHeight = MazeWidth = 25;
                        MAX_SHOP_COUNT = 8;
                    }
                    else
                    {
                        minutes = 20;
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
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    for (int y = 0; y < MAP_HEIGHT; y++)
                    {
                        if (MAP[y * MAP_WIDTH + x] == 'F')
                        {
                            Teleport teleport = new Teleport(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                            Entities.Add(teleport);
                        }
                        if (MAP[y * MAP_WIDTH + x] == 'D')
                        {
                            ShopDoor shopDoor = new ShopDoor(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                            Entities.Add(shopDoor);
                        }
                        if (MAP[y * MAP_WIDTH + x] == '$')
                        {
                            ShopMan shopMan = new ShopMan(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                            Entities.Add(shopMan);
                        }
                        if (MAP[y * MAP_WIDTH + x] == 'E')
                        {
                            SpawnEnemis(x, y, MAP_WIDTH);
                            MAP[y * MAP_WIDTH + x] = '.';
                        }
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
                        try
                        {
                            if (shop_x == 3 && shop_y == 1 && map[shop_x - 1, shop_y] == '.')
                                map[shop_x - 1, shop_y] = 'D';
                            else if (shop_x == 1 && shop_y == 3 && map[shop_x, shop_y - 1] == '.')
                                map[shop_x, shop_y - 1] = 'D';
                            else if (shop_y >= 2 && shop_y < map.GetLength(0) - 1 && shop_x >= 0 && shop_x < map.GetLength(1) && map[shop_x, shop_y - 2] == '.')
                                map[shop_x, shop_y - 1] = 'D';
                            else if (shop_y >= 0 && shop_y < map.GetLength(0) - 1 && shop_x >= 0 && shop_x < map.GetLength(1) && map[shop_x, shop_y + 2] == '.')
                                map[shop_x, shop_y + 1] = 'D';
                            else if (shop_y >= 0 && shop_y < map.GetLength(0) && shop_x >= 2 && shop_x < map.GetLength(1) - 1 && map[shop_x - 2, shop_y] == '.')
                                map[shop_x - 1, shop_y] = 'D';
                            else if (shop_y >= 0 && shop_y < map.GetLength(0) && shop_x >= 0 && shop_x < map.GetLength(1) - 1 && map[shop_x + 2, shop_y] == '.')
                                map[shop_x + 1, shop_y] = 'D';
                        }
                        catch { }
                    }
                    for (int y = 0; y < map.GetLength(1); y++)
                    {
                        for (int x = 0; x < map.GetLength(0); x++)
                        {
                            if (map[x, y] == 'F')
                            {
                                Teleport teleport = new Teleport(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                                Entities.Add(teleport);
                            }
                            if (map[x, y] == 'D')
                            {
                                ShopDoor shopDoor = new ShopDoor(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                                Entities.Add(shopDoor);
                            }
                            if (map[x, y] == '$')
                            {
                                ShopMan shopMan = new ShopMan(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                                Entities.Add(shopMan);
                            }
                            if (map[x, y] == '.' && random.NextDouble() <= enemy_count && x > 5 && y > 5)
                                SpawnEnemis(x, y, MAP_WIDTH);
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
                            if (MAP[y * (CustomMazeWidth * 3 + 1) + x] == 'F')
                            {
                                Teleport teleport = new Teleport(x + 0.5, y + 0.5, CustomMazeWidth * 3 + 1, ref MaxEntityID);
                                Entities.Add(teleport);
                            }
                            if (MAP[y * (CustomMazeWidth * 3 + 1) + x] == 'D')
                            {
                                ShopDoor shopDoor = new ShopDoor(x + 0.5, y + 0.5, CustomMazeWidth * 3 + 1, ref MaxEntityID);
                                Entities.Add(shopDoor);
                            }
                            if (MAP[y * (CustomMazeWidth * 3 + 1) + x] == '$')
                            {
                                ShopMan shopMan = new ShopMan(x + 0.5, y + 0.5, CustomMazeWidth * 3 + 1, ref MaxEntityID);
                                Entities.Add(shopMan);
                            }
                            if (MAP[y * (CustomMazeWidth * 3 + 1) + x] == 'E')
                            {
                                SpawnEnemis(x, y, CustomMazeWidth * 3 + 1);
                                MAP[y * (CustomMazeWidth * 3 + 1) + x] = '.';
                            }
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

        private void SpawnEnemis(int x, int y, int size)
        {
            double dice = rand.NextDouble();
            if (dice <= 0.4) // 40%
            {
                Man enemy = new Man(x, y, size, ref MaxEntityID);
                Entities.Add(enemy);
            }
            else if (dice > 0.4 && dice <= 0.65) // 25%
            {
                Dog enemy = new Dog(x, y, size, ref MaxEntityID);
                Entities.Add(enemy);
            }
            else if (dice > 0.65 && dice <= 0.85) // 20%
            {
                Bat enemy = new Bat(x, y, size, ref MaxEntityID);
                Entities.Add(enemy);
            }
            else // 15%
            {
                Abomination enemy = new Abomination(x, y, size, ref MaxEntityID);
                Entities.Add(enemy);
            }
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
                if (c.DealDamage(damage))
                {
                    if (attacker is Player attackerPlayer)
                    {
                        double multiplier = 1;
                        if (difficulty == 3)
                            multiplier = 1.5;
                        attackerPlayer.ChangeMoney(rand.Next((int)(c.MIN_MONEY * multiplier), (int)(c.MAX_MONEY * multiplier)));
                        attackerPlayer.EnemiesKilled++;
                        return true;
                    }
                }
                else if (attacker is Player attackerPlayer)
                    if (difficulty == 0 && attackerPlayer.GetCurrentGun().FireType == FireTypes.Single && !(attackerPlayer.GetCurrentGun() is Knife))
                        c.UpdateCoordinates(MAP.ToString(), attackerPlayer.X, attackerPlayer.Y);
                return false;
            }
            return false;
        }

        private void TimeRemain_Tick(object sender, EventArgs e)
        {
            seconds--;
            if (seconds < 0)
            {
                if (minutes > 0)
                {
                    minutes--;
                    seconds = 59;
                }
                else
                {
                    seconds = 0;
                    GameOver(0);
                }
            }
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

        public void AddHittingTheWall(double X, double Y) => Entities.Add(new HittingTheWall(X, Y, MAP_WIDTH, ref MaxEntityID));

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
                    p.Look += lookDif;
                    if (p.Look < -360)
                        p.Look = -360;
                    else if (p.Look > 360)
                        p.Look = 360;
                    return;
                }
            }
        }

        internal (int, int) GetSecondsAndMinutes() => (this.minutes, this.seconds);

        internal void StopGame(int win) => GameOver(win);

        internal void AmmoCountDecrease(int playerID)
        {
            foreach(Entity entity in Entities)
            {
                if(entity.ID == playerID)
                {
                    Player player = entity as Player;
                    player.GetCurrentGun().AmmoCount--;
                    return;
                }
            }
        }

        internal void Reload(int playerID)
        {
            foreach(Entity entity in Entities)
            {
                if(entity.ID == playerID)
                {
                    Player player = entity as Player;
                    player.GetCurrentGun().ReloadClip();
                    return;
                }
            }
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
    }
}