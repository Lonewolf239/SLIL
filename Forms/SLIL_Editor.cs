using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MazeGenerator;

namespace SLIL
{
    public partial class SLIL_Editor : Form
    {
        public SLIL_Editor()
        {
            InitializeComponent();
        }

        public int MazeHeight, MazeWidth;
        private int old_MazeHeight;
        private int old_MazeWidth;
        public static int x, y;
        private Panel[,] panels;
        private bool playerExist = false;
        private int finishCount = 0;
        public StringBuilder MAP;
        public bool OK = false;
        private readonly Random rand = new Random();

        private void Import_btn_Click(object sender, EventArgs e)
        {
            editor_interface.Focus();
            int maze_height, maze_width;
            string map = Clipboard.GetText();
            try
            {
                string[] MAP = map.Split(':');
                maze_height = Convert.ToInt32(MAP[0]);
                maze_width = Convert.ToInt32(MAP[1]);
                if (MAP[2].Any(c => c != '.' && c != '#' && c != '=' && c != 'D' && c != 'd' && c != 'F' && c != 'P' && c != 'E' && c != '$' && c != 'W'))
                {
                    if (MainMenu.Language)
                        MessageBox.Show("Строка содержит недопустимые символы.", "Ошибка импорта карты", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show("The string contains invalid characters.", "Error importing map", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (map.Length == 0)
                {
                    if (MainMenu.Language)
                        MessageBox.Show("Буфер обмена пуст.", "Ошибка импорта карты", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show("The clipboard is empty.", "Error importing map", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (maze_height < 2 * 3 + 1 || maze_height > 20 * 3 + 1 || maze_width < 2 * 3 + 1 || maze_width > 20 * 3 + 1)
                {
                    if (MainMenu.Language)
                        MessageBox.Show("Неверный формат строки.", "Ошибка импорта карты", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show("Invalid string format.", "Error importing map", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                map = MAP[2];
                MazeWidth = maze_width;
                MazeHeight = maze_height;
                GenerateField(map);
            }
            catch
            {
                MazeHeight = old_MazeHeight;
                MazeWidth = old_MazeWidth;
                editor_interface.Controls.Clear();
                GenerateField();
                if (MainMenu.Language)
                    MessageBox.Show("Неверный формат строки.", "Ошибка импорта карты", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Invalid string format.", "Error importing map", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void Export_btn_Click(object sender, EventArgs e)
        {
            editor_interface.Focus();
            try
            {
                string map = $"{MazeHeight}:{MazeWidth}:{GenerateMap()}";
                Clipboard.SetText(map);
                if (MainMenu.Language)
                    MessageBox.Show("Карта успешно скопирована в буфер обмена.", "Карта скопирована", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("The map was successfully copied to the clipboard.", "The map was copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (MainMenu.Language)
                    MessageBox.Show($"Не удалось скопировать карту в буфер обмена.\n{ex.Message}", "Ошибка копирования", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show($"Could not copy the map to the clipboard.\n{ex.Message}", "Copy error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SLIL_Editor_Load(object sender, EventArgs e)
        {
            if (!MainMenu.Language)
            {
                Text = "Editor";
                about.Text = "Elements:";
                elements.Items.Clear();
                string[] div = { "Player", "Wall", "Door", "Window", "Finish", "Shop", "Enemy", "Invisible wall" };
                elements.Items.AddRange(div);
            }
            old_MazeHeight = MazeHeight;
            old_MazeWidth = MazeWidth;
            if (MazeHeight != MazeWidth)
            {
                random_btn.BackColor = Color.LightGray;
                random_btn.Enabled = false;
            }
            elements.SelectedIndex = 0;
            GenerateField();
        }

        private void GenerateField(string map = "empty")
        {
            if (map == "empty")
            {
                for (int i = 0; i < MazeWidth * MazeHeight + 1; i++)
                    map += ".";
            }
            panels = null;
            editor_interface.Controls.Clear();
            int size = 15;
            if (MazeHeight < 11 && MazeWidth < 11)
                size = 30;
            else if (MazeHeight < 19 && MazeWidth < 19)
                size = 20;
            int min = Math.Min(MazeHeight, MazeWidth), max = Math.Max(MazeHeight, MazeWidth);
            panels = new Panel[min, max];
            for (int i = 0; i < min; i++)
            {
                for (int j = 0; j < max; j++)
                {
                    Color color = Color.Black;
                    char c = map[i * min + j];
                    if (i != 0 && i != min - 1 && j != 0 && j != max - 1)
                    {
                        if (c == '.')
                            color = Color.White;
                        else if (c == '=')
                            color = Color.Blue;
                        else if (c == 'D')
                            color = Color.DarkOrange;
                        else if (c == 'd')
                            color = Color.Orange;
                        else if (c == 'F')
                        {
                            color = Color.Lime;
                            finishCount = 1;
                        }
                        else if (c == 'P')
                        {
                            color = Color.Red;
                            playerExist = true;
                        }
                        else if (c == '$')
                            color = Color.Pink;
                        else if (c == 'E')
                            color = Color.Navy;
                    }
                    Panel panel = new Panel
                    {
                        Height = size,
                        Width = size,
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = color,
                        Left = size * j,
                        Top = size * i,
                        Name = $"panel_{i}_{j}"
                    };
                    panel.MouseEnter += new EventHandler(Panels_MouseEnter);
                    editor_interface.Controls.Add(panel);
                    panels[i, j] = panel;
                }
            }
            accept_button.Left = reset_btn.Left = random_btn.Left = import_btn.Left = export_btn.Left = editor_interface.Right + 6;
            size_label.Top = editor_interface.Bottom + 3;
            height.Top = size_label.Bottom + 3;
            width.Top = height.Top;
            separator.Top = height.Top + 2;
            accept_size_btn.Top = width.Bottom - accept_size_btn.Height;
            about.Top = height.Bottom + 3;
            elements.Top = about.Bottom + 3;
            question.Top = elements.Top - 8;
            Width = accept_button.Right + 21;
            Height = elements.Bottom + 43;
            int centerX = Owner.Left + (Owner.Width - Width) / 2;
            int centerY = Owner.Top + (Owner.Height - Height) / 2;
            Location = new Point(centerX, centerY);
        }

        private StringBuilder GenerateMap()
        {
            StringBuilder MAP = new StringBuilder();
            for (int i = 0; i < panels.GetLength(0); i++)
            {
                for (int j = 0; j < panels.GetLength(1); j++)
                {
                    if (panels[i, j].BackColor == Color.Black)
                        MAP.Append("#");
                    else if (panels[i, j].BackColor == Color.Blue)
                        MAP.Append("=");
                    else if (panels[i, j].BackColor == Color.DarkOrange)
                        MAP.Append("D");
                    else if (panels[i, j].BackColor == Color.Orange)
                        MAP.Append("d");
                    else if (panels[i, j].BackColor == Color.White)
                        MAP.Append(".");
                    else if (panels[i, j].BackColor == Color.Lime)
                        MAP.Append("F");
                    else if (panels[i, j].BackColor == Color.Pink)
                        MAP.Append("$");
                    else if (panels[i, j].BackColor == Color.Purple)
                        MAP.Append("W");
                    else if (panels[i, j].BackColor == Color.Red)
                    {
                        MAP.Append("P");
                        x = j;
                        y = i;
                    }
                    else if (panels[i, j].BackColor == Color.Navy)
                        MAP.Append("E");
                }
            }
            return MAP;
        }

        private void Panels_MouseEnter(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            panel.Focus();
        }
        
        private void Reset_btn_Click(object sender, EventArgs e)
        {
            editor_interface.Focus();
            playerExist = false;
            finishCount = 0;
            GenerateField();
        }

        private void Random_btn_Click(object sender, EventArgs e)
        {
            editor_interface.Focus();
            finishCount = 0;
            playerExist = true;
            StringBuilder sb = new StringBuilder();
            Maze MazeGenerator = new Maze();
            char[,] map = MazeGenerator.GenerateCharMap((MazeWidth - 1) / 3, (MazeHeight - 1) / 3, '#', '=', 'd', '.', 'F');
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
                        if (map[x, y] == '$')
                            shops.Add(new int[] { x, y });
                        if (map[x, y] == '.' && rand.NextDouble() <= 0.015 && x > 5 && y > 5)
                            map[x, y] = 'E';
                    }
                    catch { }
                    if (map[x, y] == 'F')
                        finishCount++;
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
                        if (y < 0 && y >= map.GetLength(0) && x < 0 && x >= map.GetLength(1))
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
                    else if (shop_y >= 2 && shop_y < map.GetLength(0) - 2 && shop_x >= 0 && shop_x < map.GetLength(1) && map[shop_x, shop_y - 2] == '.')
                        map[shop_x, shop_y - 1] = 'D';
                    else if (shop_y >= 0 && shop_y < map.GetLength(0) - 2 && shop_x >= 0 && shop_x < map.GetLength(1) && map[shop_x, shop_y + 2] == '.')
                        map[shop_x, shop_y + 1] = 'D';
                    else if (shop_y >= 0 && shop_y < map.GetLength(0) && shop_x >= 2 && shop_x < map.GetLength(1) - 2 && map[shop_x - 2, shop_y] == '.')
                        map[shop_x - 1, shop_y] = 'D';
                    else if (shop_y >= 0 && shop_y < map.GetLength(0) && shop_x >= 0 && shop_x < map.GetLength(1) - 2 && map[shop_x + 2, shop_y] == '.')
                        map[shop_x + 1, shop_y] = 'D';
                }
                catch { }
            }
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                    sb.Append(map[x, y]);
            }
            GenerateField(sb.ToString());
        }

        private void Accept_button_Click(object sender, EventArgs e)
        {
            editor_interface.Focus();
            if (!playerExist)
            {
                if (MainMenu.Language)
                    MessageBox.Show("Отсутствует игрок", "Карта не завершена", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Missing player", "The map is not completed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (finishCount == 0)
            {
                if (MainMenu.Language)
                    MessageBox.Show("Отсутствует финиш", "Карта не завершена", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Missing finish", "The map is not completed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MAP = GenerateMap();
            OK = true;
            Close();
        }

        private void Question_Click(object sender, EventArgs e)
        {
            editor_interface.Focus();
            if (MainMenu.Language)
                MessageBox.Show("Управление редактором:\nРазмещение и удаление элементов происходит в той ячейке, на которую наведен курсор мыши.\nРазместить выбранный элемент: Space или Enter\nУдалить элемент: Backspace или Del", "Подсказка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Editor control:\nPlacement and removal of elements occur in the cell where the mouse cursor is hovered.\nPlace selected element: Space or Enter\nDelete element: Backspace or Del", "Hint", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Accept_size_btn_Click(object sender, EventArgs e)
        {
            MazeHeight = (int)(height.Value * 3 + 1);
            MazeWidth = (int)(width.Value * 3 + 1);
            if (MazeHeight != MazeWidth)
            {
                random_btn.BackColor = Color.LightGray;
                random_btn.Enabled = false;
            }
            else
            {
                random_btn.BackColor = SystemColors.Control;
                random_btn.Enabled = true;
            }
            elements.SelectedIndex = 0;
            GenerateField();
        }

        private void SLIL_Editor_KeyDown(object sender, KeyEventArgs e)
        {
            Panel panel = null;
            int x = 0, y = 0;
            for (int i = 1; i < panels.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < panels.GetLength(1) - 1; j++)
                {
                    if (panels[i, j].Focused)
                    {
                        panel = panels[i, j];
                        x = i;
                        y = j;
                    }
                }
            }
            if (panel == null)
                return;
            int index = elements.SelectedIndex;
            if (panel.BackColor == Color.White)
            {
                if(e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
                {
                    if (index == 0 && !playerExist)
                    {
                        panel.BackColor = Color.Red;
                        playerExist = true;
                    }
                    else if (index == 1)
                        panel.BackColor = Color.Black;
                    else if (index == 2)
                        panel.BackColor = Color.Orange;
                    else if (index == 3)
                        panel.BackColor = Color.Blue;
                    else if (index == 4)
                    {
                        panel.BackColor = Color.Lime;
                        finishCount++;
                    }
                    else if (index == 5)
                    {
                        panel.BackColor = Color.Pink;
                        for (int i = x - 1; i <= x + 1; i++)
                        {
                            for (int j = y - 1; j <= y + 1; j++)
                            {
                                if (j <= 0 && j >= panels.GetLength(1) && i <= 0 && i >= panels.GetLength(0))
                                    continue;
                                if (i == x && j == y)
                                    continue;
                                panels[i, j].BackColor = Color.Black;
                            }
                        }
                        if (y >= 2 && y < panels.GetLength(0) - 2 && x >= 0 && x < panels.GetLength(1) && panels[x, y - 2].BackColor == Color.White)
                            panels[x, y - 1].BackColor = Color.DarkOrange;
                        else if (y >= 0 && y < panels.GetLength(0) - 2 && x >= 0 && x < panels.GetLength(1) && panels[x, y + 2].BackColor == Color.White)
                            panels[x, y + 1].BackColor = Color.DarkOrange;
                        else if (y >= 0 && y < panels.GetLength(0) && x >= 2 && x < panels.GetLength(1) - 2 && panels[x - 2, y].BackColor == Color.White)
                            panels[x - 1, y].BackColor = Color.DarkOrange;
                        else if (y >= 0 && y < panels.GetLength(0) && x >= 0 && x < panels.GetLength(1) - 2 && panels[x + 2, y].BackColor == Color.White)
                            panels[x + 1, y].BackColor = Color.DarkOrange;
                    }
                    else if (index == 6)
                        panel.BackColor = Color.Navy;
                    else if (index == 7)
                        panel.BackColor = Color.Purple;
                }
            }
            else
            {
                if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
                {
                    if (panel.BackColor == Color.Lime)
                        finishCount--;
                    else if (panel.BackColor == Color.Red)
                        playerExist = false;
                    panel.BackColor = Color.White;
                }
            }
        }
    }
}