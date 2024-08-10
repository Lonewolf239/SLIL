using MazeGenerator;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SLIL.Classes
{
    internal class GameModel : INetSerializable
    {
        private static StringBuilder MAP = new StringBuilder();
        private const string bossMap = "#########################...............##F###.................####..##...........##..###...=...........=...###...=.....E.....=...###...................###...................###.........#.........###...##.........##...###....#.........#....###...................###..#...##.#.##...#..####.....#.....#.....######...............##############d####################...#################E=...=E#################...#################$D.P.D$#################...################################",
            debugMap = @"####################.................##..=======........##..=.....=.....#..##..=....E=........##..=..====........##..=..=........d..##..=.E=...........##..====...........##........P.....=..##.................##.................##..............F..##.................##..===....#D#..#..##..=E=====#$#.#d=.##..===....###..=..##.................####################";
        private Pet[] PETS;
        public static readonly List<Entity> Entities = new List<Entity>();
        private readonly char[] impassibleCells = { '#', 'D', '=', 'd' };
        private const double playerWidth = 0.4;
        private bool GameStarted = false, CorrectExit = false;
        private readonly Random rand;
        private int difficulty;
        private int MAP_WIDTH, MAP_HEIGHT;
        public StringBuilder CUSTOM_MAP = new StringBuilder();
        public int CUSTOM_X, CUSTOM_Y;

        private static System.Timers.Timer RespawnTimer;
        private static System.Timers.Timer EnemyTimer;

        public int MaxEntityID;

        public GameModel()
        {
            Pet[] pets = { new SillyCat(0, 0, 0, ref MaxEntityID), new GreenGnome(0, 0, 0, ref MaxEntityID), new EnergyDrink(0, 0, 0, ref MaxEntityID), new Pyro(0, 0, 0, ref MaxEntityID) };
            PETS = pets;
            //MAP.Append(@"####################.................##..=======........##..=.....=.....#..##..=....E=........##..=..====........##..=..=........d..##..=.E=...........##..====...........##........P.....=..##.................##.................##..............F..##.................##..===....#D#..#..##..=E=====#$#.#d=.##..===....###..=..##.................####################");
            MAP_WIDTH = 16;
            MAP_HEIGHT = 16;
            rand = new Random();
            difficulty = 2;
            InitMap();
            RespawnTimer = new System.Timers.Timer(1000);
            RespawnTimer.Interval = 1000;
            RespawnTimer.Elapsed += RespawnTimer_Tick;
            EnemyTimer = new System.Timers.Timer(100);
            EnemyTimer.Interval = 100;
            EnemyTimer.Elapsed += EnemyTimer_Tick;
        }

        public void StartGame()
        {
            GameStarted = true;
            RespawnTimer.Start();
            EnemyTimer.Start();
        }

        public int AddPlayer()
        {
            bool OK = false;
            double X = 3, Y = 3;
            while (!OK)
            {
                X = rand.Next(1, MAP_WIDTH);
                Y = rand.Next(1, MAP_HEIGHT);
                if (MAP[(int)Y * MAP_WIDTH + (int)X] == '.') OK = true;
            }
            X += 0.5; Y += 0.5;
            Entities.Add(new Player(X, Y, MAP_WIDTH, ref MaxEntityID));
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
                                        if (player.HP <= 0)
                                        {
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

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(MAP.ToString());
            writer.Put(MAP_WIDTH);
            writer.Put(MAP_HEIGHT);
            writer.Put(Entities.Count);
            foreach (var entity in Entities)
            {
                entity.Serialize(writer);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            MAP = new StringBuilder(reader.GetString());
            MAP_WIDTH = reader.GetInt();
            MAP_HEIGHT = reader.GetInt();
            int entCount = reader.GetInt();
            Entities.Clear();
            for (int i = 0; i < entCount; i++)
            {
                int entityID = reader.GetInt();
                int pID = -1;
                double entityX = reader.GetDouble();
                double entityY = reader.GetDouble();
                if (entityID == 0) pID = reader.GetInt();
                switch (entityID)
                {
                    case 0:
                        Entities.Add(new Player(entityX, entityY, MAP_WIDTH, ref MaxEntityID));
                        break;
                    case 1:
                        Entities.Add(new Man(entityX, entityY, MAP_WIDTH, ref MaxEntityID));
                        break;
                    case 2:
                        Entities.Add(new Dog(entityX, entityY, MAP_WIDTH, ref MaxEntityID));
                        break;
                    case 3:
                        Entities.Add(new Abomination(entityX, entityY, MAP_WIDTH, ref MaxEntityID));
                        break;
                    case 4:
                        Entities.Add(new Bat(entityX, entityY, MAP_WIDTH, ref MaxEntityID));
                        break;
                    case 5:
                        Entities.Add(new SillyCat(entityX, entityY, MAP_WIDTH, ref MaxEntityID));
                        break;
                    case 6:
                        Entities.Add(new GreenGnome(entityX, entityY, MAP_WIDTH, ref MaxEntityID));
                        break;
                    case 7:
                        Entities.Add(new EnergyDrink(entityX, entityY, MAP_WIDTH, ref MaxEntityID));
                        break;
                    case 8:
                        Entities.Add(new Pyro(entityX, entityY, MAP_WIDTH, ref MaxEntityID));
                        break;
                    case 9:
                        Entities.Add(new Teleport(entityX, entityY, MAP_WIDTH, ref MaxEntityID));
                        break;
                    case 10:
                        Entities.Add(new HittingTheWall(entityX, entityY, MAP_WIDTH, ref MaxEntityID));
                        break;
                    case 11:
                        Entities.Add(new ShopDoor(entityX, entityY, MAP_WIDTH, ref MaxEntityID));
                        break;
                    case 12:
                        Entities.Add(new ShopMan(entityX, entityY, MAP_WIDTH, ref MaxEntityID));
                        break;
                    default:
                        break;
                }
            }
        }
        public void AddPet(Player player, int index)
        {
            Pet pet = PETS[index];
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
                {
                    doesPlayerExist = true;
                }
            }
            if (!doesPlayerExist)
            {
                return;
            }
            if (player.PET == null)
                return;
            player.PET.SetNewParametrs(player.X + 0.1, player.Y + 0.1, MAP_WIDTH);
            Entities.Add(player.PET);
        }

        private void GameOver(int win)
        {
            //TODO: realization of enemy timer
            //enemy_timer.Stop();
            //GameStarted = false;
            //if (win == 1)
            //{
            //    if (MainMenu.sounds)
            //        tp.Play(Volume);
            //    if (difficulty != 4)
            //        player.Stage++;
            //    if (!player.CuteMode)
            //    {
            //        for (int i = 0; i < player.Guns.Count; i++)
            //        {
            //            if (player.Guns[i].MaxAmmoCount == 0)
            //                player.Guns[i].MaxAmmoCount = player.Guns[i].CartridgesClip;
            //        }
            //    }
            //    player.ChangeMoney(50 + (5 * player.EnemiesKilled));
            //    GetFirstAidKit();
            //    StartGame();
            //    UpdatePet();
            //}
            //else if (win == 0)
            //{
            //    ToDefault();
            //    game_over_panel.Visible = true;
            //    game_over_panel.BringToFront();
            //    if (MainMenu.sounds)
            //        game_over.Play(Volume);
            //}
            //else
            //    ToDefault();
        }

        public void MovePlayer(double dX, double dY, int playerID)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] is Player)
                {
                    if ((Entities[i] as Player).ID == playerID)
                    {
                        Entities[i].X += dX;
                        Entities[i].Y += dY;
                        return;
                    }
                }
            }
        }

        public void InitMap()
        {
            MAP.Clear();
            if (difficulty == 5)
            {
                //if (inDebug == 1)
                //{
                //    MAP.AppendLine(debugMap);
                //}
                //else if (inDebug == 2)
                //{
                //    MAP.AppendLine(bossMap);
                //}
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    for (int y = 0; y < MAP_HEIGHT; y++)
                    {
                        if (MAP[y * MAP_WIDTH + x] == 'F')
                        {
                            Teleport teleport = new Teleport(x, y, MAP_WIDTH, ref MaxEntityID);
                            Entities.Add(teleport);
                        }
                        if (MAP[y * MAP_WIDTH + x] == 'D')
                        {
                            ShopDoor shopDoor = new ShopDoor(x, y, MAP_WIDTH, ref MaxEntityID);
                            Entities.Add(shopDoor);
                        }
                        if (MAP[y * MAP_WIDTH + x] == '$')
                        {
                            ShopMan shopMan = new ShopMan(x, y, MAP_WIDTH, ref MaxEntityID);
                            Entities.Add(shopMan);
                        }
                        if (MAP[y * MAP_WIDTH + x] == 'E')
                        {
                            SpawnEnemis(x, y, MAP_WIDTH);
                            MAP[y * MAP_WIDTH + x] = '.';
                        }
                    }
                }
                return;
            }
            //if (!CUSTOM)
            if (true)
            {
                Random random = new Random();
                StringBuilder sb = new StringBuilder();
                char[,] map = (new Maze()).GenerateCharMap((MAP_WIDTH - 1) / 3, (MAP_HEIGHT - 1) / 3, '#', '=', 'd', '.', 'F', 5);//MAX_SHOP_COUNT);
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
                            Teleport teleport = new Teleport(x, y, MAP_WIDTH, ref MaxEntityID);
                            Entities.Add(teleport);
                        }
                        if (map[x, y] == 'D')
                        {
                            ShopDoor shopDoor = new ShopDoor(x, y, MAP_WIDTH, ref MaxEntityID);
                            Entities.Add(shopDoor);
                        }
                        if (map[x, y] == '$')
                        {
                            ShopMan shopMan = new ShopMan(x, y, MAP_WIDTH, ref MaxEntityID);
                            Entities.Add(shopMan);
                        }
                        double enemy_count = 0.07;
                        if (difficulty == 0)
                            enemy_count = 0.07;
                        else if (difficulty == 1)
                            enemy_count = 0.065;
                        else if (difficulty == 2)
                        {
                            enemy_count = 0.055;
                        }
                        else if (difficulty == 3)
                        {
                            enemy_count = 0.045;
                        }
                        else if (difficulty == 4)
                        {
                            enemy_count = 0.06;
                        }
                        if (map[x, y] == '.' && random.NextDouble() <= enemy_count && x > 5 && y > 5)
                            SpawnEnemis(x, y, MAP_WIDTH);
                        sb.Append(map[x, y]);
                    }
                }
                MAP = sb;
            }
            else
            {/*
                MAP.Append(CUSTOM_MAP);
                for (int x = 0; x < CustomMazeWidth * 3 + 1; x++)
                {
                    for (int y = 0; y < CustomMazeHeight * 3 + 1; y++)
                    {
                        if (MAP[y * (CustomMazeWidth * 3 + 1) + x] == 'F')
                        {
                            Teleport teleport = new Teleport(x, y, CustomMazeWidth * 3 + 1);
                            Entities.Add(teleport);
                        }
                        if (MAP[y * (CustomMazeWidth * 3 + 1) + x] == 'D')
                        {
                            ShopDoor shopDoor = new ShopDoor(x, y, CustomMazeWidth * 3 + 1);
                            Entities.Add(shopDoor);
                        }
                        if (MAP[y * (CustomMazeWidth * 3 + 1) + x] == '$')
                        {
                            ShopMan shopMan = new ShopMan(x, y, CustomMazeWidth * 3 + 1);
                            Entities.Add(shopMan);
                        }
                        if (MAP[y * (CustomMazeWidth * 3 + 1) + x] == 'E')
                        {
                            SpawnEnemis(x, y, CustomMazeWidth * 3 + 1);
                            MAP[y * (CustomMazeWidth * 3 + 1) + x] = '.';
                        }
                    }
                }*/
            }
            for (int i = 0; i < MAP.Length; i++)
            {
                if (MAP[i] == 'o')
                    MAP[i] = 'd';
            }
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
            else if (dice > 0.65 && dice <= 0.85 && difficulty != 5) // 20%
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
        public StringBuilder GetMap()
        {
            return MAP;
        }
        public int GetMapWidth()
        {
            return MAP_WIDTH;
        }
        public int GetMapHeight()
        {
            return MAP_HEIGHT;
        }
        public List<Entity> GetEntities()
        {
            return Entities;
        }

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
                    }
                }
                else if (attacker is Player attackerPlayer)
                    if (difficulty == 0 && attackerPlayer.GetCurrentGun().FireType == FireTypes.Single && !(attackerPlayer.GetCurrentGun() is Knife))
                        c.UpdateCoordinates(MAP.ToString(), attackerPlayer.X, attackerPlayer.Y);
                return false;
            }
            return false;
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
    }
}