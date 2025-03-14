using System;
using Play_Sound;
using System.Text;
using System.Linq;
using MazeGenerator;
using LiteNetLib.Utils;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SLIL.Classes
{
    internal class GameModel : INetSerializable
    {
        private StringBuilder MAP = new StringBuilder();
        private const string bossMap = @"#########################...............##F###.................####..##...........##..###...=...........=...###...=.....E.....=...###...................###...................###.........#.........###...##.........##...###....#.........#....###...................###..#...##.#.##...#..####.....#.....#.....######...............##############d####################...#################E=...=E#################...#################$D.P.D$#################...################################",
            debugMap = @"##########################.......................##.......................##..WWWW..7.6.4.3.2.1.#..##..WE.W.................##..W.EW.................##..WWWW...........b..d..##.......................##.......................##.................B..=..##.......................##..#....................##..#b.......P.....L..F..##..####X................##..#B...................##..#..............l..b..##..####.................##..X....................##....................B..##.......................##......b=5#D#...##d#=#####...======#$#L###X.#....##.....=B..###.#B#....#..##...............L..l##.f##########################",
            bikeMap = @"############################......######..555..#####........####.........###.......................##.......................##....####......####....=##...######....######...=##...######====#dd###...=##...##$###....#dd###...=##...##D###....######...=##...##.b##.....####....=##WWW##..##..............##EEE#F...d..............##WWW##..##..............##...##.B##.....####.....##...##D###....######....##...##$###....###dd#====##...######....###dd#....##...######....######....##....####......####.....##.......................##.......................###........####.......P.#####......######..555..############################",
            backroomsMap = @"##########################....#.#..##.#..........##..#...##.#......#...#..###...###.......#.####.####.#.........#..#...#..#.##..V###....#.#..##......##..##.#..#......#....#..##...#...#..###.####...####.#.#...#.......#.......##.#...#....#..###.####..##.##.#....##..#.#..#.#..##....##.........##.#.##.##..#..#.##......#....#..##........#......######..##.###....##...#.........##...###.......#.#.......##.#..#..#...#....##..#.#####..#.##...#....#...#####.......##.##.#....#.#..##.#.#....#..#......#.#..##...##.#....#.....#.....##....#....#.....#.....#.###.#.##...#..#.##..##.#.##..#....#.#..#..0..#....##########################",
            emptyMap = @"##########################....#.#..##.#..........##..#...##.#......#...#..###...###.......#.####.####.#.........#..#...#..#.##..F###....#.#..##......##..##.#..#......#....#..##...#...#..###.####...####.#.#...#.......#.......##.#...#....#..###.####..##.##.#....##..#.#..#.#..##....##.........##.#.##.##..#..#.##......#....#..##........#......######..##.###....##...#.........##...###.......#.#.......##.#..#..#...#....##..#.#####..#.##...#....#...#####.......##.##.#....#.#..##.#.#....#..#......#.#..##...##.#....#.....#.....##....#....#.....#.....#.###.#.##...#..#.##..##.#.##..#....#.#..#..0..#....##########################";
        private int inDebug = 0;
        private readonly Pet[] PETS;
        private readonly Transport[] TRANSPORTS;
        private readonly DisposableItem[] ITEMS;
        internal List<Entity> Entities = new List<Entity>();
        private int TotalTime = 0, DeathCause = -1, Stage = 0, TotalKilled = 0;
        private double EnemyDamageOffset = 1;
        private const double playerWidth = 0.4;
        private bool GameStarted = false;
        private readonly Random Rand;
        private int difficulty;
        private int MAP_WIDTH, MAP_HEIGHT;
        private bool CUSTOM = false;
        private int BackroomsStage = 0;
        private bool inBackrooms = false, BackroomsCompleted = false, isTutorial = false;
        private int CustomMazeHeight, CustomMazeWidth;
        private StringBuilder CUSTOM_MAP = new StringBuilder();
        private double CUSTOM_X, CUSTOM_Y;
        private readonly StopGameDelegate StopGameHandle;
        private readonly PlaySoundDelegate PlaySoundHandle;
        private readonly SetPlayerIDDelegate SetPlayerID;
        internal const bool IsMultiplayer = false;
        private static System.Windows.Forms.Timer RespawnTimer;
        private static System.Windows.Forms.Timer EntityTimer;
        private static System.Windows.Forms.Timer TimeRemain;
        internal int MaxEntityID;
        
        internal GameModel(StopGameDelegate stopGame, SetPlayerIDDelegate setPlayerID, PlaySoundDelegate playSound)
        {
            StopGameHandle = stopGame;
            SetPlayerID = setPlayerID;
            Pet[] pets = { new SillyCat(0, 0, 0, 0), new GreenGnome(0, 0, 0, 0), new EnergyDrink(0, 0, 0, 0), new Pyro(0, 0, 0, 0) };
            DisposableItem[] items = { new FirstAidKit(), new Adrenalin(), new Helmet() };
            Transport[] transports = { new Bike(0, 0, 0, 0) };
            PETS = pets;
            ITEMS = items;
            TRANSPORTS = transports;
            MAP_WIDTH = 16;
            MAP_HEIGHT = 16;
            Rand = new Random(Guid.NewGuid().GetHashCode());
            difficulty = MainMenu.difficulty;
            RespawnTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            RespawnTimer.Tick += RespawnTimer_Tick;
            EntityTimer = new System.Windows.Forms.Timer
            {
                Interval = 100
            };
            EntityTimer.Tick += EntityTimer_Tick;
            TimeRemain = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            TimeRemain.Tick += TimeRemain_Tick;
            PlaySoundHandle = playSound;
        }

        internal void Pause(bool paused)
        {
            if (paused)
            {
                TimeRemain.Stop();
                RespawnTimer.Stop();
                EntityTimer.Stop();
            }
            else
            {
                TimeRemain.Start();
                RespawnTimer.Start();
                EntityTimer.Start();
            }
        }

        internal void StartGame(bool startTimers)
        {
            if (inDebug == 0) difficulty = SLIL.difficulty;
            else difficulty = 5;
            if (Entities.Count == 0) SetPlayerID(AddPlayer());
            if (MAP.Length == 0) InitMap();
            GameStarted = true;
            DeathCause = -1;
            if (startTimers && !IsMultiplayer)
            {
                RespawnTimer.Start();
                EntityTimer.Start();
                TimeRemain.Start();
            }
        }

        internal void ToTutorial() => isTutorial = true;

        internal bool IsGameStarted() => GameStarted;

        internal int AddPlayer()
        {
            TotalTime = Stage = TotalKilled = 0;
            Player player = new Player(3, 3, MAP_WIDTH, ref MaxEntityID);
            Entities.Add(player);
            if (difficulty == 0) player.Money = 15;
            else if (difficulty == 1) player.Money = 15;
            else if (difficulty == 2) player.Money = 75;
            else if (difficulty == 3) player.Money = 150;
            if (isTutorial) player.Money = 500;
            if (inDebug != 0) player.Money = 1234;
            return MaxEntityID - 1;
        }

        private void EntityTimer_Tick(object sender, EventArgs e)
        {
            List<Entity> targetsList = new List<Entity>();
            foreach (Entity ent in Entities)
            {
                if (ent is Player) targetsList.Add(ent);
            }
            if (targetsList.Count == 0) return;
            for (int i = 0; i < Entities.Count; i++)
            {
                if (GameStarted)
                {
                    if (Entities[i] is Player || !Entities[i].HasAI) continue;
                    var entity = Entities[i] as dynamic;
                    var targetListOrdered = targetsList.OrderBy((playerI) => Math.Pow(entity.X - playerI.X, 2) + Math.Pow(entity.Y - playerI.Y, 2));
                    Entity target = targetListOrdered.First();
                    double distance = ML.GetDistance(new TPoint(target.X, target.Y), new TPoint(entity.X, entity.Y));
                    if (entity is Creature c && c.CanHitByTransport && !(!c.HasAI || c.Dead || !c.CanHit))
                    {
                        if (target is Player playerOnTransport)
                        {
                            bool playerCanHitc = true;
                            if (!playerOnTransport.InTransport || playerOnTransport.MoveSpeed < 2.5) playerCanHitc = false;
                            if (Math.Abs(c.X - playerOnTransport.X) > 0.5 || Math.Abs(c.Y - playerOnTransport.Y) > 0.5) playerCanHitc = false;
                            if (playerCanHitc)
                            {
                                if (DealDamageFromPlayerToCreature(c, 5, playerOnTransport))
                                {
                                    if (c.DeathSound != -1)
                                    {
                                        if (playerOnTransport.CuteMode)
                                            PlayGameSound(SLIL.CuteDeathSounds[c.DeathSound, Rand.Next(0, SLIL.DeathSounds.GetLength(1))], c.X, c.Y);
                                        else
                                            PlayGameSound(SLIL.DeathSounds[c.DeathSound, Rand.Next(0, SLIL.DeathSounds.GetLength(1))], c.X, c.Y);
                                    }
                                }
                                KickCreature(c, playerOnTransport, 1.2, 1.75);
                                playerOnTransport.MoveSpeed = 0.5;
                            }
                        }
                    }
                    if (entity is GameObject && entity.Temporarily)
                    {
                        entity.LifeTime++;
                        entity.CurrentFrame++;
                        if (entity.LifeTime >= entity.TotalLifeTime)
                        {
                            Entities.Remove(entity);
                            i--;
                            continue;
                        }
                        else if (entity.LifeTime >= entity.TotalLifeTime / 2)
                        {
                            if (entity is Explosions explosion && !explosion.ExplosionOccurred)
                            {
                                if (explosion.CanBrakeDoors)
                                {
                                    int explosinDistance = (int)explosion.HitDistance + 3;
                                    for (int k = (int)explosion.X - explosinDistance; k <= (int)explosion.X + explosinDistance; k++)
                                    {
                                        for (int l = (int)explosion.Y - explosinDistance; l <= (int)explosion.Y + explosinDistance; l++)
                                        {
                                            if (k < 0 || k > MAP_WIDTH || l < 0 || l > MAP_HEIGHT) continue;
                                            double dis = ML.GetDistance(new TPoint(explosion.X, explosion.Y), new TPoint(k, l));
                                            if (dis > explosion.HitDistance) continue;
                                            if (MAP[GetCoordinate(k, l)] == 'd') BrokeDoor(k, l);
                                        }
                                    }
                                }
                                for (int j = 0; j < Entities.Count; j++)
                                {
                                    var ent = Entities[j];
                                    if (ent is NPC || ent is Player || ent is Enemy || ent is Transport)
                                    {
                                        if (ent is Creature creature && (creature.Dead || !creature.CanHit || !creature.HasAI)) continue;
                                        if (explosion.CanHitOnlyPlayer && !(ent is Player)) continue;
                                        double distanceSquared = ML.GetDistance(new TPoint(ent.X, ent.Y), new TPoint(explosion.X, explosion.Y));
                                        if (distanceSquared > explosion.HitDistance) continue;
                                        double damage = Rand.Next(explosion.MinDamage, explosion.MaxDamage);
                                        if (ent is Player playerTarget)
                                        {
                                            if (playerTarget.InTransport) PlayGameSound(SLIL.HitTransport);
                                            else if (playerTarget.CuteMode) PlayGameSound(SLIL.Hungry);
                                            else PlayGameSound(SLIL.Hit[Rand.Next(SLIL.Hit.Length)]);
                                            playerTarget.DealDamage(damage, true);
                                            if (playerTarget.HP <= 0)
                                            {
                                                Entities.Add(new PlayerDeadBody(playerTarget.X, playerTarget.Y, MAP_WIDTH, ref MaxEntityID));
                                                TotalKilled = playerTarget.TotalEnemiesKilled;
                                                DeathCause = SetDeathCause(entity);
                                                GameOver(0);
                                                return;
                                            }
                                        }
                                        if (ent is NPC npc)
                                        {
                                            if (npc.DealDamage(damage) && npc is ExplodingBarrel)
                                            {
                                                PlayGameSound(SLIL.BarrelExplosion, npc.X, npc.Y);
                                                SpawnExplotion(npc);
                                            }
                                        }
                                        if (ent is Enemy enemy) enemy.DealDamage(damage);
                                        if (ent is Transport transport) DealDamage(transport.ID, damage * 1.5, explosion.ID);
                                    }
                                }
                                explosion.ExplosionOccurred = true;
                                continue;
                            }
                        }
                    }
                    if (entity is Creature creature1 && creature1.Stunned)
                    {
                        creature1.UpdateCoordinates(MAP.ToString(), target.X, target.Y);
                        continue;
                    }
                    else if (entity is Enemy)
                    {
                        if (distance <= 22)
                        {
                            if (target is Player playerTarget)
                            {
                                if (!entity.Dead && !playerTarget.Dead)
                                {
                                    bool extraVision = IsExtraVision(playerTarget);
                                    entity.UpdateCoordinates(MAP.ToString(), target.X, target.Y, target.A, extraVision);
                                    if (entity.Fast) entity.UpdateCoordinates(MAP.ToString(), target.X, target.Y, target.A, extraVision);
                                    if (Math.Abs(entity.X - target.X) <= 0.5 && Math.Abs(entity.Y - target.Y) <= 0.5)
                                    {
                                        if (!playerTarget.Invulnerable)
                                        {
                                            if (playerTarget.InTransport) PlayGameSound(SLIL.HitTransport);
                                            else if (playerTarget.CuteMode) PlayGameSound(SLIL.Hungry);
                                            else PlayGameSound(SLIL.Hit[SLIL.Hit.Length]);
                                            GiveDebaf(playerTarget, entity);
                                            playerTarget.DealDamage(Rand.Next(entity.MinDamage, entity.MaxDamage), true);
                                            if (playerTarget.HP <= 0)
                                            {
                                                Entities.Add(new PlayerDeadBody(target.X, target.Y, MAP_WIDTH, ref MaxEntityID));
                                                TotalKilled = playerTarget.TotalEnemiesKilled;
                                                DeathCause = SetDeathCause(entity);
                                                GameOver(0);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (target is Covering coveringTarget)
                            {
                                if (!entity.Dead && !coveringTarget.Broken)
                                {
                                    entity.UpdateCoordinates(MAP.ToString(), target.X, target.Y);
                                    if (entity.Fast) entity.UpdateCoordinates(MAP.ToString(), target.X, target.Y);
                                    if (Math.Abs(entity.X - target.X) <= 0.5 && Math.Abs(entity.Y - target.Y) <= 0.5)
                                        coveringTarget.DealDamage(Rand.Next(entity.MinDamage, entity.MaxDamage));
                                }
                            }
                            if (entity is Stalker stalker && stalker.Dead)
                            {
                                double spawnDistance = 0;
                                int iteration = 0;
                                while (true)
                                {
                                    int x = Rand.Next(0, MAP_WIDTH);
                                    int y = Rand.Next(0, MAP_HEIGHT);
                                    spawnDistance = ML.GetDistance(new TPoint(target.X, target.Y), new TPoint(x, y));
                                    if (MAP[GetCoordinate(x, y)] == '.')
                                    {
                                        if (spawnDistance >= 14 || (iteration > 10000 && spawnDistance > 3))
                                        {
                                            stalker.X = x;
                                            stalker.Y = y;
                                            stalker.DoRespawn();
                                            break;
                                        }
                                    }
                                    if (iteration > 20000) break;
                                    iteration++;
                                }
                            }
                            if (entity is RangeEnemy range && range.DidShot)
                            {
                                double shotAX = Math.Sin(range.ShotA);
                                double shotAY = Math.Cos(range.ShotA);
                                if (range is Shooter shooter)
                                {
                                    PlayGameSound(SLIL.SoundsofShotsEnemies[0, ((Player)target).CuteMode ? 1 : 0], shooter.X, shooter.Y);
                                    HashSet<char> impassibleCells = new HashSet<char> { '#', 'D', 'd', 'W', 'S', 'R' };
                                    double shotDistance = 0;
                                    const double shotStep = 0.01d;
                                    bool hit = false;
                                    List<(int, int)> points = new List<(int, int)>();
                                    while (!hit && shotDistance <= shooter.ShotDistance + 6)
                                    {
                                        int pX = (int)(shooter.X + shotAX * shotDistance);
                                        int pY = (int)(shooter.Y + shotAY * shotDistance);
                                        points.Add((pX, pY));
                                        if (impassibleCells.Contains(MAP[pY * MAP_WIDTH + pX]))
                                        {
                                            AddHittingTheWall(shooter.X + shotAX * (shotDistance - 0.5), shooter.Y + shotAY * (shotDistance - 0.5), 1);
                                            hit = true;
                                            break;
                                        }
                                        shotDistance += shotStep;
                                    }
                                    hit = false;
                                    foreach (var point in points)
                                    {
                                        foreach (var ent in targetListOrdered)
                                        {
                                            if (ent is Player player)
                                            {
                                                if (Math.Abs(point.Item1 - player.X) <= 0.5 && Math.Abs(point.Item2 - player.Y) <= 0.5 && !player.Invulnerable)
                                                {
                                                    if (player.InTransport) PlayGameSound(SLIL.HitTransport);
                                                    else if (player.CuteMode) PlayGameSound(SLIL.Hungry);
                                                    else PlayGameSound(SLIL.Hit[Rand.Next(SLIL.Hit.Length)]);
                                                    player.DealDamage(Rand.Next(shooter.MinDamage, shooter.MaxDamage), true);
                                                    if (player.HP <= 0)
                                                    {
                                                        Entities.Add(new PlayerDeadBody(player.X, player.Y, MAP_WIDTH, ref MaxEntityID));
                                                        TotalKilled = player.TotalEnemiesKilled;
                                                        DeathCause = SetDeathCause(entity);
                                                        GameOver(0);
                                                        return;
                                                    }
                                                    hit = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (hit) break;
                                    }
                                }
                                if (range is LostSoul soul)
                                {
                                    PlayGameSound(SLIL.SoundsofShotsEnemies[1, ((Player)target).CuteMode ? 1 : 0], soul.X, soul.Y);
                                    SpawnRockets(soul.X, soul.Y, 1, soul.ShotA);
                                }
                                range.DidShot = false;
                                range.ShotPause = range.TotalShotPause;
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
                                    distance = ML.GetDistance(new TPoint(target.X, target.Y), new TPoint(entity.X, entity.Y));
                                }
                            }
                        }
                        if (distance > 1 && !(owner.GetCurrentGun() is Flashlight && entity.RespondsToFlashlight))
                            entity.UpdateCoordinates(MAP.ToString(), owner.X, owner.Y);
                        else entity.Stoped = true;
                    }
                    else if (entity is Rockets rocket)
                    {
                        if (!entity.Dead) entity.UpdateCoordinates(MAP.ToString(), target.X, target.Y);
                        char[] ImpassibleCells = rocket.GetInpassibleRocketCells();
                        double newX = entity.X + entity.GetMove() * Math.Sin(entity.A);
                        double newY = entity.Y + entity.GetMove() * Math.Cos(entity.A);
                        if (ImpassibleCells.Contains(MAP[(int)newY * MAP_WIDTH + (int)(newX + entity.EntityWidth / 2)])
                        || ImpassibleCells.Contains(MAP[(int)newY * MAP_WIDTH + (int)(newX - entity.EntityWidth / 2)])
                        || ImpassibleCells.Contains(MAP[(int)(newY + entity.EntityWidth / 2) * MAP_WIDTH + (int)newX])
                        || ImpassibleCells.Contains(MAP[(int)(newY - entity.EntityWidth / 2) * MAP_WIDTH + (int)newX]))
                        {
                            Entities.Remove(entity);
                            i--;
                            SpawnExplotion(rocket);
                            PlayGameSound(SLIL.RPGExplosion, entity.X, entity.Y);
                        }
                        if (!Entities.Contains(entity)) continue;
                        foreach (Entity ent in Entities)
                        {
                            if (ent == entity) continue;
                            if (rocket.CanHitOnlyPlayer && !(ent is Player)) continue;
                            if (ent is Creature creature && (creature.Dead || !creature.CanHit || !creature.HasAI)) continue;
                            if (ML.GetDistance(new TPoint(ent.X, ent.Y), new TPoint(entity.X, entity.Y)) < (entity.EntityWidth + ent.EntityWidth) * (entity.EntityWidth + ent.EntityWidth))
                            {
                                Entities.Remove(entity);
                                i--;
                                SpawnExplotion(rocket);
                                PlayGameSound(SLIL.RPGExplosion, entity.X, entity.Y);
                                return;
                            }
                        }
                    }
                    else if (entity is Bonus bonus)
                    {
                        if (target is Player p)
                        {
                            if (ML.GetDistance(new TPoint(bonus.X, bonus.Y), new TPoint(p.X, p.Y)) < (p.EntityWidth + bonus.EntityWidth) * (p.EntityWidth + bonus.EntityWidth))
                            {
                                if (bonus is AmmoBox ammoBox)
                                {
                                    if (ammoBox.WeaponType == null) continue;
                                    int weaponIndex = -1;
                                    for (int j = 0; j < p.Guns.Count; j++)
                                    {
                                        if (p.Guns[j].GetType() == ammoBox.WeaponType)
                                        {
                                            weaponIndex = j;
                                            break;
                                        }
                                    }
                                    if (weaponIndex == -1) continue;
                                    int ammo = p.Guns[weaponIndex].CartridgesClip + p.Guns[weaponIndex].AmmoInStock;
                                    if (ammo > p.Guns[weaponIndex].MaxAmmo) continue;
                                    PlayGameSound(SLIL.LiftingAmmoBox);
                                    p.Guns[weaponIndex].AmmoInStock = ammo;
                                }
                                else if (bonus is MoneyPile pile)
                                {
                                    if (p.Money == 9999) continue;
                                    p.ChangeMoney(pile.MoneyCount);
                                    PlayGameSound(SLIL.LiftingMoneyPile);
                                }
                                Entities.Remove(bonus);
                                i--;
                            }
                        }
                    }
                }
            }
        }

        private static bool IsExtraVision(Player p)
        {
            if (p.GetCurrentGun() is Flashlight) return true;
            return false;
        }

        private void BrokeDoor(double x, double y)
        {
            MAP[GetCoordinate(x, y)] = 'R';
            PlayGameSound(SLIL.BreakdownDoors, x, y);
            Entities.Add(new BrokenDoor(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID));
        }

        private void SpawnExplotion(Entity entity)
        {
            if (entity is Rockets rocket)
            {
                if (rocket.ExplosionID == 0) Entities.Add(new RpgExplosion(entity.X, entity.Y, MAP_WIDTH, ref MaxEntityID));
                if (rocket.ExplosionID == 1) Entities.Add(new SoulExplosion(entity.X, entity.Y, MAP_WIDTH, ref MaxEntityID));
            }
            else
            {
                if (entity is ExplodingBarrel) Entities.Add(new BarrelExplosion(entity.X, entity.Y, MAP_WIDTH, ref MaxEntityID));
                else Entities.Add(new RpgExplosion(entity.X, entity.Y, MAP_WIDTH, ref MaxEntityID));
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
            else if (entity is RpgExplosion) return 4;
            else if (entity is Stalker) return -1;
            else if (entity is VoidStalker) return -1;
            else if (entity is Shooter) return -1;
            else if (entity is LostSoul) return -1;
            else if (entity is SoulExplosion) return -1;
            else return -1;
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
                    double distance = ML.GetDistance(new TPoint(player.X, player.Y), new TPoint(enemy.X, enemy.Y));
                    if (distance <= 30)
                    {
                        if (difficulty <= 1)
                        {
                            if (enemy.Dead && enemy.Respawn > 0)
                                enemy.Respawn--;
                            else if (enemy.Dead && enemy.Respawn <= 0)
                            {
                                if (Math.Abs(enemy.X - player.X) > 1 && Math.Abs(enemy.Y - player.Y) > 1)
                                    enemy.Respawn();
                            }
                        }
                    }
                }
            });
        }

        internal void SetCustom(bool custom, int CustomWidth, int CustomHeight, string CustomMap, double customX, double customY)
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
                        Zombie man = new Zombie(0, 0, MAP_WIDTH, ID);
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
                        Ogr abomination = new Ogr(0, 0, MAP_WIDTH, ID);
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
                        RpgExplosion explosion = new RpgExplosion(0, 0, MAP_WIDTH, ID);
                        explosion.Deserialize(reader);
                        tempEntities.Add(explosion);
                        break;
                    case 18:
                        Bike bike = new Bike(0, 0, MAP_WIDTH, ID);
                        bike.Deserialize(reader);
                        tempEntities.Add(bike);
                        break;
                    case 19:
                        Vine vine = new Vine(0, 0, MAP_WIDTH, ID);
                        vine.Deserialize(reader);
                        tempEntities.Add(vine);
                        break;
                    case 20:
                        Lamp lamp = new Lamp(0, 0, MAP_WIDTH, ID);
                        lamp.Deserialize(reader);
                        tempEntities.Add(lamp);
                        break;
                    case 21:
                        BackroomsTeleport backroomsTeleport = new BackroomsTeleport(0, 0, MAP_WIDTH, ID);
                        backroomsTeleport.Deserialize(reader);
                        tempEntities.Add(backroomsTeleport);
                        break;
                    case 22:
                        Covering covering = new Covering(0, 0, MAP_WIDTH, ID);
                        covering.Deserialize(reader);
                        tempEntities.Add(covering);
                        break;
                    case 23:
                        VoidTeleport emptyTeleport = new VoidTeleport(0, 0, MAP_WIDTH, ID);
                        emptyTeleport.Deserialize(reader);
                        tempEntities.Add(emptyTeleport);
                        break;
                    case 24:
                        VoidStalker voidStalker = new VoidStalker(0, 0, MAP_WIDTH, ID);
                        voidStalker.Deserialize(reader);
                        tempEntities.Add(voidStalker);
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
                        Zombie man = new Zombie(0, 0, MAP_WIDTH, ID);
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
                        Ogr abomination = new Ogr(0, 0, MAP_WIDTH, ID);
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
                        RpgExplosion explosion = new RpgExplosion(0, 0, MAP_WIDTH, ID);
                        explosion.Deserialize(reader);
                        tempEntities.Add(explosion);
                        break;
                    case 18:
                        Bike bike = new Bike(0, 0, MAP_WIDTH, ID);
                        bike.Deserialize(reader);
                        tempEntities.Add(bike);
                        break;
                    case 19:
                        Vine vine = new Vine(0, 0, MAP_WIDTH, ID);
                        vine.Deserialize(reader);
                        tempEntities.Add(vine);
                        break;
                    case 20:
                        Lamp lamp = new Lamp(0, 0, MAP_WIDTH, ID);
                        lamp.Deserialize(reader);
                        tempEntities.Add(lamp);
                        break;
                    case 21:
                        BackroomsTeleport backroomsTeleport = new BackroomsTeleport(0, 0, MAP_WIDTH, ID);
                        backroomsTeleport.Deserialize(reader);
                        tempEntities.Add(backroomsTeleport);
                        break;
                    case 22:
                        Covering covering = new Covering(0, 0, MAP_WIDTH, ID);
                        covering.Deserialize(reader);
                        tempEntities.Add(covering);
                        break;
                    case 23:
                        VoidTeleport emptyTeleport = new VoidTeleport(0, 0, MAP_WIDTH, ID);
                        emptyTeleport.Deserialize(reader);
                        tempEntities.Add(emptyTeleport);
                        break;
                    case 24:
                        VoidStalker voidStalker = new VoidStalker(0, 0, MAP_WIDTH, ID);
                        voidStalker.Deserialize(reader);
                        tempEntities.Add(voidStalker);
                        break;
                    default:
                        break;
                }
            }
            Entities = new List<Entity>(tempEntities);
        }

        internal void AddTransport(int playerID, int index)
        {
            Player p = GetPlayer(playerID);
            if (p == null) return;
            Transport transport = null;
            if (index == 0) transport = new Bike(p.X, p.Y, MAP_WIDTH, MaxEntityID);
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

        internal void AddPet(int playerID, int index)
        {
            Player player = GetPlayer(playerID);
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
                                player.MaxHP -= 25;
                                player.HealHP(125);
                                break;
                            case 2: //Energy Drink
                                player.MaxStamine -= 150;
                                player.MaxMoveSpeed -= 0.15;
                                player.MaxRunSpeed -= 0.1;
                                player.MaxStrafeSpeed = player.MaxMoveSpeed / 1.4;
                                player.Stamine = player.MaxStamine;
                                break;
                            case 3: //Pyro
                                player.CuteMode = false;
                                CuteMode(player);
                                break;
                        }
                    }
                    Entities.RemoveAt(i);
                    i--;
                }
            }
            if (pet.IsInstantAbility != 0)
            {
                switch (pet.GetPetAbility())
                {
                    case 1: //GreenGnome
                        player.MaxHP += 25;
                        player.HealHP(125);
                        break;
                    case 2: //Energy Drink
                        player.MaxStamine += 150;
                        player.MaxMoveSpeed += 0.15;
                        player.MaxRunSpeed += 0.1;
                        player.MaxStrafeSpeed = player.MaxMoveSpeed / 1.4;
                        player.Stamine = player.MaxStamine;
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
            EntityTimer.Stop();
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
                    if (difficulty != 4 && difficulty != 6) Stage++;
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
                    if (!inBackrooms) UpdatePet(player);
                }
            }
            else
            {
                PlayGameSound(SLIL.PlayerDeathSound);
                EntityTimer.Stop();
                RespawnTimer.Stop();
                TimeRemain.Stop();
                Entities.Clear();
                MaxEntityID = 0;
                inBackrooms = BackroomsCompleted = false;
            }
            StopGameHandle(win);
        }

        private void GetFirstAidKit(Player player) => player.DisposableItems[0].AddItem();

        internal void MovePlayer(double dX, double dY, int playerID)
        {
            if (!GameStarted) return;
            Player p = GetPlayer(playerID);
            if (p == null || p.BlockInput) return;
            p.X += dX;
            p.Y += dY;
            if (p.TRANSPORT != null && p.EffectCheck(4))
            {
                double extendedX = p.X + Math.Sin(p.A) * 0.3;
                double extendedY = p.Y + Math.Cos(p.A) * 0.3;
                if (p.TRANSPORT.CanJump && !p.EffectCheck(3))
                {
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
                if (p.MoveSpeed >= 2.5)
                {
                    char[] c = { '#', 'd', 'D', 'S', 'R' };
                    if (c.Contains(MAP[(int)extendedY * MAP_WIDTH + (int)extendedX]) ||
                        (MAP[(int)extendedY * MAP_WIDTH + (int)extendedX] == '=' && (!p.TRANSPORT.CanJump || p.EffectCheck(3))))
                    {
                        p.MoveSpeed = 0.5;
                        PlayGameSound(SLIL.HitTransport);
                        p.DealDamage(5, false);
                    }
                }
            }
            if (MAP[(int)p.Y * MAP_WIDTH + (int)p.X] == 'F')
            {
                if (!IsMultiplayer)
                {
                    if (inBackrooms)
                    {
                        BackroomsCompleted = true;
                        p.StopEffect(8);
                    }
                    inBackrooms = false;
                    GameOver(1);
                }
            }
            if (MAP[(int)p.Y * MAP_WIDTH + (int)p.X] == 'f')
            {
                if (!IsMultiplayer)
                {
                    inBackrooms = true;
                    BackroomsStage = 0;
                    if (p.InTransport)
                        GettingOffTheTransport(playerID, false);
                    GameOver(1);
                }
            }
            if (MAP[(int)p.Y * MAP_WIDTH + (int)p.X] == 'V')
            {
                if (!IsMultiplayer)
                {
                    BackroomsStage = 1;
                    GameOver(1);
                }
            }
        }

        internal void GoDebug(int debug)
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
                    entity = new Teleport(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    break;
                case 'f':
                    entity = new BackroomsTeleport(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    break;
                case 'V':
                    entity = new VoidTeleport(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    break;
                case 'D':
                    entity = new ShopDoor(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    break;
                case '$':
                    entity = new ShopMan(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    break;
                case 'b':
                    entity = new Box(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    break;
                case 'B':
                    entity = new Barrel(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    break;
                case 'X':
                    entity = new ExplodingBarrel(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    break;
                case 'L':
                    entity = new Vine(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    break;
                case 'l':
                    entity = new Lamp(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID); ;
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
                case '5':
                    entity = new Bike(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                    break;
                case '6':
                    SpawnEnemis(x, y, MAP_WIDTH, false, 4);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
                case '7':
                    SpawnEnemis(x, y, MAP_WIDTH, false, 5);
                    MAP[y * MAP_WIDTH + x] = '.';
                    break;
                case '0':
                    if (inBackrooms)
                    {
                        if (BackroomsStage == 0) entity = new Stalker(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                        else entity = new VoidStalker(x + 0.5, y + 0.5, MAP_WIDTH, ref MaxEntityID);
                        MAP[y * MAP_WIDTH + x] = '.';
                    }
                    break;
            }
            return entity;
        }

        internal void InitMap()
        {
            double enemy_count = 0;
            int MazeWidth = 0, MazeHeight = 0, MAX_SHOP_COUNT = 1;
            if (inBackrooms)
            {
                if (BackroomsStage == 0)
                    MazeHeight = MazeWidth = 25;
                else
                    MazeHeight = MazeWidth = 25;
            }
            else
            {
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
                        MazeHeight = 25;
                        MazeWidth = 25;
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
                if (difficulty < 4)
                {
                    MazeHeight = MazeWidth = 10;
                    MAX_SHOP_COUNT = 2;
                }
            }
            foreach (Entity ent in Entities)
            {
                if (!(ent is Player player)) continue;
                if (inBackrooms)
                {
                    if (BackroomsStage == 0)
                    {
                        player.X = player.Y = 12.5;
                        MazeHeight = MazeWidth = 25;
                    }
                    else
                    {
                        player.X = player.Y = 12.5;
                        MazeHeight = MazeWidth = 25;
                    }
                }
                else
                {
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
                            player.X = 12.5;
                            player.Y = 12.5;
                            MazeHeight = 25;
                            MazeWidth = 25;
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
                    if (difficulty < 4)
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
            }
            if (difficulty == 5 || inBackrooms)
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
            if (difficulty == 5 || inBackrooms)
            {
                if (inBackrooms)
                {
                    if (BackroomsStage == 0)
                        MAP.Append(backroomsMap);
                    else
                        MAP.Append(emptyMap);
                }
                else if (inDebug == 1)
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
                    Random random = new Random(Guid.NewGuid().GetHashCode());
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
                            if (BackroomsCompleted && map[x, y] == 'f')
                                map[x, y] = '.';
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
                                if (entity != null) Entities.Add(entity);
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
                    if (MAP[i] == 'o') MAP[i] = 'd';
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
                if (difficulty != 5 && !inBackrooms)
                {
                    player.X = 1.5;
                    player.Y = 1.5;
                }
            };
        }

        internal void SpawnRockets(double x, double y, int id, double a)
        {
            Rockets rocket = null;
            if (id == 0) rocket = new RpgRocket(x, y, MAP_WIDTH, ref MaxEntityID);
            else if (id == 1) rocket = new SoulClot(x, y, MAP_WIDTH, ref MaxEntityID);
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
            x += 0.5; y += 0.5;
            if (type == -1)
            {
                double dice = Rand.NextDouble();
                Entity entity;
                if (dice <= 0.25) // 25%
                {
                    entity = new Zombie(x, y, size, ref MaxEntityID);
                    ((Enemy)entity).SetDamage(EnemyDamageOffset);
                    entity.HasAI = ai;
                }
                else if (dice <= 0.40) // 15%
                {
                    entity = new Dog(x, y, size, ref MaxEntityID);
                    ((Enemy)entity).SetDamage(EnemyDamageOffset);
                    entity.HasAI = ai;
                }
                else if (dice <= 0.55) // 15%
                {
                    entity = new Bat(x, y, size, ref MaxEntityID);
                    ((Enemy)entity).SetDamage(EnemyDamageOffset);
                    entity.HasAI = ai;
                }
                else if (dice <= 0.65) // 10%
                {
                    entity = new Ogr(x, y, size, ref MaxEntityID);
                    ((Enemy)entity).SetDamage(EnemyDamageOffset);
                    entity.HasAI = ai;
                }
                else if (dice <= 0.85) // 20%
                {
                    entity = new Shooter(x, y, size, ref MaxEntityID);
                    ((Enemy)entity).SetDamage(EnemyDamageOffset);
                    entity.HasAI = ai;
                }
                else if (dice <= 0.90) // 5%
                {
                    entity = new LostSoul(x, y, size, ref MaxEntityID);
                    ((Enemy)entity).SetDamage(EnemyDamageOffset);
                    entity.HasAI = ai;
                }
                else // 10%
                {
                    if (Rand.NextDouble() <= 0.2)
                        entity = new Vine(x, y, size, ref MaxEntityID);
                    else if (Rand.NextDouble() <= 0.4)
                        entity = new Lamp(x, y, size, ref MaxEntityID);
                    else if (Rand.NextDouble() <= 0.6)
                        entity = new Box(x, y, size, ref MaxEntityID);
                    else if(Rand.NextDouble() <= 0.8)
                        entity = new Barrel(x, y, size, ref MaxEntityID);
                    else 
                        entity = new ExplodingBarrel(x, y, size, ref MaxEntityID);
                }
                Entities.Add(entity);
            }
            else if (type == 0)
            {
                Zombie enemy = new Zombie(x, y, size, ref MaxEntityID);
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
                Ogr enemy = new Ogr(x, y, size, ref MaxEntityID);
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
                Shooter enemy = new Shooter(x, y, size, ref MaxEntityID);
                enemy.SetDamage(EnemyDamageOffset);
                enemy.HasAI = ai;
                Entities.Add(enemy);
            }
            else if (type == 5)
            {
                LostSoul enemy = new LostSoul(x, y, size, ref MaxEntityID);
                enemy.SetDamage(EnemyDamageOffset);
                enemy.HasAI = ai;
                Entities.Add(enemy);
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

        internal int GetBackroomsStage() => BackroomsStage;

        internal bool InBackrooms() => inBackrooms;

        internal StringBuilder GetMap() => MAP;

        internal void ChangeMapChar(int coordinate, char c) => MAP[coordinate] = c;

        internal Pet[] GetPets() => PETS;

        internal Transport[] GetTransports() => TRANSPORTS;

        internal int GetMapWidth() => MAP_WIDTH;

        internal int GetMapHeight() => MAP_HEIGHT;

        internal List<Entity> GetEntities() => Entities;

        internal bool DealDamage(int ID, double damage, int attackerID)
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
                if (!p.HasAI || p.Dead) return false;
                if (attacker is Player attackerPlayer && p.DealDamage(damage, true))
                {
                    double multiplier = 1;
                    if (difficulty == 3)
                        multiplier = 1.5;
                    attackerPlayer.ChangeMoney(Rand.Next((int)(20 * multiplier), (int)(30 * multiplier)));
                    attackerPlayer.EnemiesKilled++;
                    attackerPlayer.TotalEnemiesKilled++;
                    return true;
                }
                return false;
            }
            if (target is Creature c) return DealDamageFromPlayerToCreature(c, damage, attacker);
            if (target is Transport t)
            {
                if (t.TransportHP <= 0) return false;
                PlayGameSound(SLIL.HitTransport, t.X, t.Y);
                if (t.DealDamage(damage))
                {
                    int coordinate = GetCoordinate(t.X, t.Y);
                    RemoveEntity(t.ID);
                    ChangeMapChar(coordinate, '.');
                    return true;
                }
                return false;
            }
            return false;
        }

        private bool DealDamageFromPlayerToCreature(Creature c, double damage, Entity attacker)
        {
            if (!c.HasAI || c.Dead || !c.CanHit) return false;
            if (c.DealDamage(damage))
            {
                if (attacker is Player attackerPlayer)
                {
                    if (c is Boxes box)
                    {
                        BoxBreakage(box, attackerPlayer);
                        return true;
                    }
                    double multiplier = difficulty == 3 ? 1.5 : 1;
                    attackerPlayer.ChangeMoney(Rand.Next((int)(c.MinMoney * multiplier), (int)(c.MaxMoney * multiplier)));
                    if (c is Enemy)
                    {
                        attackerPlayer.EnemiesKilled++;
                        attackerPlayer.TotalEnemiesKilled++;
                    }
                    return true;
                }
            }
            return false;
        }

        private void BoxBreakage(Boxes box, Player p)
        {
            if (box is ExplodingBarrel barrel)
            {
                PlayGameSound(SLIL.BarrelExplosion, barrel.X, barrel.Y);
                SpawnExplotion(barrel);
                return;
            }
            if (box.BoxWithMoney)
                AddEntity(new MoneyPile(box.X, box.Y, MAP_WIDTH, ref MaxEntityID) { MoneyCount = Rand.Next(5, 15) });
            else if (!p.CuteMode)
            {
                Type type = null;
                Gun gun;
                if (Rand.Next(100) > 15)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        gun = p.Guns[Rand.Next(p.Guns.Count)];
                        if (gun.CanGetAmmoFromBox)
                        {
                            type = gun.GetType();
                            break;
                        }
                    }
                }
                AmmoBox ammoBox = new AmmoBox(box.X, box.Y, MAP_WIDTH, ref MaxEntityID) { WeaponType = type };
                if (type == null) ammoBox.Texture = 45;
                AddEntity(ammoBox);
            }
        }

        internal void AddEntity(Entity entity)
        {
            entity.ID = MaxEntityID;
            MaxEntityID++;
            Entities.Add(entity);
        }

        private void TimeRemain_Tick(object sender, EventArgs e)
        {
            TotalTime++;
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] is Player player)
                {
                    if (player.Invulnerable) player.InvulnerableEnd();
                    player.UpdateEffectsTime();
                    if (player.IncreasingFear()) PlayGameSound(SLIL.ScarySounds[Rand.Next(SLIL.ScarySounds.Length)]);
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
        }

        internal void AddHittingTheWall(double X, double Y, double vMove)
        {
            HittingTheWall hittingTheWall = new HittingTheWall(X, Y, MAP_WIDTH, ref MaxEntityID)
            {
                VMove = vMove
            };
            Entities.Add(hittingTheWall);
        }

        internal void ChangePlayerA(double v, int playerID)
        {
            Player p = GetPlayer(playerID);
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
            p.A = ML.NormalizeAngle(p.A + v);
        }

        internal void ChangePlayerLook(double lookDif, int playerID)
        {
            Player p = GetPlayer(playerID);
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
            Player p = GetPlayer(playerID);
            if (p == null) return;
            Gun gun = p.GetCurrentGun();
            int ammo = gun is SubmachineGun && gun.Level == Levels.LV3 ? 2 : 1;
            gun.AmmoCount -= ammo;
        }

        internal void ReloadClip(int playerID)
        {
            Player p = GetPlayer(playerID);
            p?.GetCurrentGun().ReloadClip();
        }

        internal bool OnOffNoClip(int playerID)
        {
            Player p = GetPlayer(playerID);
            if (p == null) return false;
            p.NoClip = !p.NoClip;
            return p.NoClip;
        }

        internal bool HasNoClip(int playerID)
        {
            Player p = GetPlayer(playerID);
            if (p == null) return false;
            return p.NoClip;
        }

        internal void GettingOffTheTransport(int playerID, bool add_transport = true)
        {
            Player p = GetPlayer(playerID);
            if (p == null || p.BlockInput) return;
            Transport transport = null;
            if (p.TRANSPORT is Bike)
                transport = new Bike(p.X, p.Y, MAP_WIDTH, MaxEntityID)
                {
                    TransportHP = p.TransportHP,
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
            Player p = GetPlayer(playerID);
            if (p == null) return false;
            char[] impassibleCells = { '#', 'D', '=', 'd', 'S', '$', 'R' };
            if (HasNoClip(playerID) || p.InParkour) return false;
            return impassibleCells.Contains(GetMap()[index]);
        }

        internal int GetDeathCause() => DeathCause;

        internal int GetStage() => Stage;

        internal int GetTotalKilled() => TotalKilled;

        internal string GetTotalTime()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(TotalTime);
            return timeSpan.ToString(@"hh\:mm\:ss");
        }

        internal void DidKick(int playerID)
        {
            Player p = GetPlayer(playerID);
            if (p == null || p.Stamine < 200 || p.DoesKick) return;
            if (p.EffectCheck(3)) return;
            if (p.InTransport) return;
            bool damnKick = Rand.NextDouble() <= p.CurseKickChance;
            if (damnKick) PlayGameSound(SLIL.DamnKick);
            else PlayGameSound(SLIL.Kick);
            if (damnKick) p.ReducesStamine(600);
            else p.ReducesStamine(150);
            p.DoesKick = true;
            p.Look = 0;
            p.KickType = 0;
            p.CanUnblockCamera = false;
            p.BlockCamera = p.BlockInput = true;
            p.PlayerMoveStyle = Directions.WALK;
            p.CanShoot = false;
            p.KickType = damnKick ? 1 : 0;
            new Thread(() =>
            {
                if (damnKick) Thread.Sleep(325);
                FindEntityForKick(p, damnKick);
                Thread.Sleep(150);
                StopKick(playerID);
            }).Start();
        }

        private void FindEntityForKick(Entity ent, bool damnKick = false)
        {
            double maxDistance = damnKick ? 2.5 : 0.6;
            int maxCountKickEntity = damnKick ? 6 : 2;
            int kickCount = 0;
            double distance = 0;
            double kickX = Math.Sin(ent.A);
            double kickY = Math.Cos(ent.A);
            while (distance <= maxDistance)
            {
                distance += 0.1;
                double testX = ent.X + kickX * distance;
                double testY = ent.Y + kickY * distance;
                for (int i = 0; i < Entities.Count; i++)
                {
                    if (Entities[i] is Player || !Entities[i].HasAI) continue;
                    var entity = Entities[i];
                    if (entity is Creature creature)
                    {
                        if (!creature.MightBeKicked) continue;
                        if (ML.GetDistance(new TPoint(creature.X, creature.Y), new TPoint(ent.X, ent.Y)) > maxDistance + 1) continue;
                        if (Math.Abs(creature.X - testX) > 0.5 || Math.Abs(creature.Y - testY) > 0.5) continue;
                        KickCreature(creature, ent, damnKick ? 2 : 0.8, damnKick ? 15 : 1);
                        kickCount++;
                        if (kickCount == maxCountKickEntity) return;
                    }
                }
            }
        }

        private void KickCreature(Creature creature, Entity attacker, double power, double damage)
        {
            creature.KnockbackBlow(attacker.A, power);
            if (creature.CanHitByKick) DealDamageFromPlayerToCreature(creature, damage, attacker);
        }

        private void StopKick(int playerID)
        {
            Player p = GetPlayer(playerID);
            if (p == null) return;
            p.DoesKick = false;
            p.BlockInput = false;
            p.BlockCamera = false;
            if (!p.InTransport) p.CanShoot = true;
        }

        internal void DrawItem(int playerID)
        {
            Player p = GetPlayer(playerID);
            if (p == null || p.DisposableItem == null) return;
            if (p.DisposableItem.HasLVMechanics)
            {
                if (p.CuteMode)
                    p.DisposableItem.Level = Levels.LV4;
                else
                {
                    if (Rand.NextDouble() <= p.CurseCureChance)
                    {
                        if (Rand.NextDouble() <= 0.5)
                            p.DisposableItem.Level = Levels.LV2;
                        else
                            p.DisposableItem.Level = Levels.LV3;
                    }
                    else
                        p.DisposableItem.Level = Levels.LV1;
                }
            }
            else
                p.DisposableItem.Level = Levels.LV1;
            p.ItemFrame = 0;
            p.GunState = 1;
            p.Aiming = p.CanShoot = false;
            p.UseItem = true;
        }

        internal void UseItem(int playerID)
        {
            Player p = GetPlayer(playerID);
            if (p == null || p.DisposableItem == null) return;
            new Thread(() =>
            {
                while (p.UseItem)
                {
                    if (p.ItemFrame > p.DisposableItem.ReloadFrames)
                    {
                        p.SetItemEffect();
                        p.DisposableItem.RemoveItem();
                        p.GunState = p.MoveStyle;
                        p.CanShoot = true;
                    }
                    Thread.Sleep(p.DisposableItem.RechargeTime);
                    p.ItemFrame++;
                }
            }).Start();
        }

        internal void ChangeItem(int playerID, int index)
        {
            Player p = GetPlayer(playerID);
            p?.ChangeItem(index);
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

        internal void DoParkour(int playerID, int y, int x)
        {
            Player p = GetPlayer(playerID);
            if (p == null || p.Stamine < 200) return;
            if (p.EffectCheck(3)) return;
            if (p.InTransport) PlayGameSound(SLIL.Climb[1]);
            else PlayGameSound(SLIL.Climb[0]);
            p.InParkour = true;
            p.ParkourState = 0;
            p.X = x + 0.5;
            p.Y = y + 0.5;
            p.Look = 0;
            p.CanUnblockCamera = false;
            p.BlockCamera = p.BlockInput = true;
            p.PlayerMoveStyle = Directions.WALK;
            p.CanShoot = false;
            new Thread(() =>
            {
                Thread.Sleep(p.InTransport ? 250 : 375);
                StopParkour(playerID);
                p.ParkourState++;
            }).Start();
        }

        internal void StopParkour(int playerID)
        {
            Player p = GetPlayer(playerID);
            if (p == null) return;
            double new_x = p.X + Math.Sin(p.A);
            double new_y = p.Y + Math.Cos(p.A);
            p.InParkour = false;
            p.ReducesStamine(150);
            while (HasImpassibleCells((int)new_y * MAP_WIDTH + (int)(new_x), playerID))
            {
                new_x += Math.Sin(p.A) / 4;
                new_y += Math.Cos(p.A) / 4;
            }
            p.X = new_x;
            p.Y = new_y;
            p.BlockInput = false;
            p.BlockCamera = false;
            if (!p.InTransport) p.CanShoot = true;
        }

        internal void DoDodge(double dodgeX, double dodgeY, int playerID)
        {
            Player p = GetPlayer(playerID);
            if (p == null || p.Stamine < 200) return;
            if (p.EffectCheck(3)) return;
            double distance = 0;
            char[] impassibleCells = { '#', 'D', '=', 'd', 'S', '$', 'T', 't', 'R' };
            PlayGameSound(SLIL.Climb[0]);
            p.ReducesStamine(150);
            new Thread(() =>
            {
                while (distance <= 0.09)
                {
                    distance += 0.01d;
                    double x = p.X + dodgeX * distance;
                    double y = p.Y + dodgeY * distance;
                    char test_wall = GetMap()[GetCoordinate(x, y)];
                    if (impassibleCells.Contains(test_wall)) return;
                    p.X = x;
                    p.Y = y;
                    Thread.Sleep(10);
                }
            }).Start();
        }

        internal void ChangeWeapon(int playerID, int new_gun)
        {
            Player p = GetPlayer(playerID);
            if (p != null) p.CurrentGun = new_gun;
        }

        internal void SetWeaponSlot(int playerID, int slot, int index)
        {
            Player p = GetPlayer(playerID);
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
            Player p = GetPlayer(playerID);
            if (p == null) return;
            Gun weapon = p.Guns[weaponID];
            if (p.Money >= weapon.AmmoCost && weapon.AmmoInStock + weapon.AmmoCount <= weapon.MaxAmmo)
            {
                p.ChangeMoney(-weapon.AmmoCost);
                weapon.AmmoInStock += weapon.CartridgesClip;
            }
        }

        private static int GetGunIndex(Player player, Gun weapon)
        {
            for (int i = 0; i < player.Guns.Count; i++)
            {
                if (player.Guns[i].GetType() == weapon.GetType())
                    return i;
            }
            return -1;
        }

        internal void BuyWeapon(int playerID, int weaponID)
        {
            Player p = GetPlayer(playerID);
            if (p == null) return;
            Gun weapon = p.GUNS[weaponID];
            if (p.Money >= weapon.GunCost)
            {
                p.ChangeMoney(-weapon.GunCost);
                weapon.SetDefault();
                weapon.HasIt = true;
                p.Guns.Add(weapon);
                if (p.WeaponSlot_1 == -1) SetWeaponSlot(playerID, 1, GetGunIndex(p, weapon));
                if (p.WeaponSlot_0 == -1) SetWeaponSlot(playerID, 0, GetGunIndex(p, weapon));
            }
        }

        internal void UpdateWeapon(int playerID, int weaponID)
        {
            Player p = GetPlayer(playerID);
            if (p == null) return;
            Gun weapon = p.Guns[weaponID];
            if (p.Money >= weapon.UpdateCost)
            {
                p.ChangeMoney(-weapon.UpdateCost);
                weapon.LevelUpdate();
                p.LevelUpdated = true;
            }
        }

        internal void BuyConsumable(int playerID, int itemID)
        {
            Player p = GetPlayer(playerID);
            if (p == null) return;
            DisposableItem item = (DisposableItem)p.GUNS[itemID];
            if (p.Money >= item.GunCost && item.CanBuy())
            {
                p.ChangeMoney(-item.GunCost);
                item.AddItem();
            }
        }

        internal void PlayGameSound(PlaySound sound, double X, double Y)
        {
            if (!MainMenu.sounds) return;
            PlaySoundHandle(sound, X, Y, true);
        }

        internal void PlayGameSound(PlaySound sound)
        {
            if (!MainMenu.sounds) return;
            PlaySoundHandle(sound, 0, 0, false);
        }

        internal void InteractingWithDoors(int coordinate)
        {
            double X = coordinate % MAP_HEIGHT;
            double Y = (coordinate - X) / MAP_WIDTH;
            if (MAP[coordinate] == 'o')
            {
                MAP[coordinate] = 'd';
                PlayGameSound(SLIL.Door[1], X, Y);
            }
            else
            {
                MAP[coordinate] = 'o';
                PlayGameSound(SLIL.Door[0], X, Y);
            }
        }

        internal void SetEnemyDamageOffset(double value) => EnemyDamageOffset = value;

        internal void GetOnATransport(int ID, int playerID)
        {
            Player p = GetPlayer(playerID);
            if (p == null) return;
            Entity entity = GetEntity(ID);
            if (entity != null)
            {
                if (MAP[(int)entity.Y * MAP_WIDTH + (int)entity.X] == '5')
                    MAP[(int)entity.Y * MAP_WIDTH + (int)entity.X] = '.';
                p.TRANSPORT = (Transport)entity;
                p.TransportHP = ((Transport)entity).TransportHP;
                p.A = ((Transport)entity).A;
                p.X = entity.X;
                p.Y = entity.Y;
            }
            RemoveEntity(ID);
            p.GiveEffect(4, true);
        }

        internal void DeserializePlayer(int playerID, byte[] rawData)
        {
            foreach (Entity ent in Entities)
            {
                if (ent.ID == playerID)
                {
                    Player p = (Player)ent;
                    NetDataReader reader = new NetDataReader(rawData);
                    p.Deserialize(reader);
                    return;
                }
            }
        }

        internal int GetCoordinate(double x, double y)
        {
            if (y < 0 || x < 0 || y > GetMapHeight() || x > GetMapWidth())
                return 0;
            return (int)y * GetMapWidth() + (int)x;
        }
    }
}