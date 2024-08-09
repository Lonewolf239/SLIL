using SharpDX.Direct2D1;
using SLIL.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SLIL.Classes
{
    internal class GameModel
    {
        private static StringBuilder MAP = new StringBuilder();
        private const string bossMap = "#########################...............##F###.................####..##...........##..###...=...........=...###...=.....E.....=...###...................###...................###.........#.........###...##.........##...###....#.........#....###...................###..#...##.#.##...#..####.....#.....#.....######...............##############d####################...#################E=...=E#################...#################$D.P.D$#################...################################",
            debugMap = @"####################.................##..=======........##..=.....=.....#..##..=....E=........##..=..====........##..=..=........d..##..=.E=...........##..====...........##........P.....=..##.................##.................##..............F..##.................##..===....#D#..#..##..=E=====#$#.#d=.##..===....###..=..##.................####################";
        private readonly Gun[] GUNS = { new Flashlight(), new Knife(), new Pistol(), new Shotgun(), new SubmachineGun(), new AssaultRifle(), new SniperRifle(), new Fingershot(), new TSPitW(), new Gnome(), new FirstAidKit(), new Candy(), new Rainblower() };
        private readonly Pet[] PETS = { new SillyCat(0, 0, 0), new GreenGnome(0, 0, 0), new EnergyDrink(0, 0, 0), new Pyro(0, 0, 0) };
        public static readonly List<Entity> Entities = new List<Entity>();
        private readonly char[] impassibleCells = { '#', 'D', '=', 'd' };
        private const double playerWidth = 0.4;
        private bool GameStarted = false, CorrectExit = false;
        private readonly Random rand;
        private int difficulty;
        private int MAP_WIDTH, MAP_HEIGHT;
        public StringBuilder CUSTOM_MAP = new StringBuilder();
        public int CUSTOM_X, CUSTOM_Y;
        public GameModel()
        {
            rand = new Random();
            difficulty = MainMenu.difficulty;
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
                GUNS[11].HasIt = true;
                GUNS[12].HasIt = true;
                player.Guns.Add(GUNS[11]);
                player.Guns.Add(GUNS[12]);
            }
            else
            {
                GUNS[11].HasIt = false;
                GUNS[12].HasIt = false;
                for (int i = 0; i < 11; i++)
                {
                    if (GUNS[i].HasIt)
                        player.Guns.Add(GUNS[i]);
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
            GameStarted = false;
            if (win == 1)
            {
                if (MainMenu.sounds)
                    tp.Play(Volume);
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
                GetFirstAidKit();
                StartGame();
                UpdatePet();
            }
            else if (win == 0)
            {
                ToDefault();
                game_over_panel.Visible = true;
                game_over_panel.BringToFront();
                if (MainMenu.sounds)
                    game_over.Play(Volume);
            }
            else
                ToDefault();
        }

        public void MovePlayer(double dX, double dY, int playerID)
        {
            for(int i = 0; i < Entities.Count; i++)
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
    }
}
