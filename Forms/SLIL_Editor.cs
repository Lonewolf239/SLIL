using System;
using System.Linq;
using System.Text;
using MazeGenerator;
using System.Drawing;
using SLIL.UserControls;
using System.Windows.Forms;

namespace SLIL
{
    internal partial class SLIL_Editor : Form
    {
        internal int MazeHeight, MazeWidth;
        private int old_MazeHeight;
        private int old_MazeWidth;
        private int x_panel, y_panel;
        private Panel[,] panels;
        private bool playerExist = false;
        private int finishCount = 0;
        internal Map MAP;
        internal bool OK = false;
        private Panel panel;
        private int Element = -1;
        private const int ElementsCount = 14;
        private readonly Color[] ElementsColor =
        {
            //игрок
             Color.Red,
            //враг
             Color.Navy,
            //мотоцикл
             Color.Yellow,
            //стена
             Color.Gray,
            //дверь
             Color.Orange,
            //окно
             Color.Blue,
            //финиш
             Color.Lime,
            //магазин
             Color.Pink,
            //ящик
             Color.Brown,
            //бочка
             Color.RosyBrown,
            //лоза
             Color.OliveDrab,
            //светильник
             Color.DarkGoldenrod,
            //взрывающаяся бочка
             Color.OrangeRed,
            //невидимая стена
             Color.Purple,
        };

        internal SLIL_Editor()
        {
            InitializeComponent();
            Cursor = Program.SLILCursorDefault;
        }

        private void SLIL_Editor_MouseEnter(object sender, EventArgs e) => panel = null;

        private bool IsValidMapCharacter(char c)
        {
            return c == '.' || c == '#' || c == '=' ||
                c == 'D' || c == 'd' || c == 'b' ||
                c == 'B' || c == 'F' || c == 'P' ||
                c == 'E' || c == '$' || c == 'W' ||
                c == 'L' || c == 'l' || c == '5' ||
                c == 'X';
        }

        private string GetElementsName(int index)
        {
            switch (index)
            {
                case 0:
                    if (MainMenu.DownloadedLocalizationList)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, "1-16");
                    return "Player";
                case 1:
                    if (MainMenu.DownloadedLocalizationList)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, "1-17");
                    return "Enemy";
                case 2:
                    if (MainMenu.DownloadedLocalizationList)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, "1-26");
                    return "Motorbike";
                case 3:
                    if (MainMenu.DownloadedLocalizationList)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, "1-18");
                    return "Wall";
                case 4:
                    if (MainMenu.DownloadedLocalizationList)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, "1-19");
                    return "Door";
                case 5:
                    if (MainMenu.DownloadedLocalizationList)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, "1-20");
                    return "Window";
                case 6:
                    if (MainMenu.DownloadedLocalizationList)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, "1-21");
                    return "Finish";
                case 7:
                    if (MainMenu.DownloadedLocalizationList)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, "1-22");
                    return "Shop";
                case 8:
                    if (MainMenu.DownloadedLocalizationList)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, "1-23");
                    return "Box";
                case 9:
                    if (MainMenu.DownloadedLocalizationList)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, "1-24");
                    return "Barrel";
                case 10:
                    if (MainMenu.DownloadedLocalizationList)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, "1-27");
                    return "Vine";
                case 11:
                    if (MainMenu.DownloadedLocalizationList)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, "1-28");
                    return "Lamp";
                case 12:
                    if (MainMenu.DownloadedLocalizationList)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, "1-28");
                    return "Exploding Barrel";
                default:
                    if (MainMenu.DownloadedLocalizationList)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, "1-25");
                    return "Invisible Wall";
            }
        }

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
                if (MAP[2].Any(c => !IsValidMapCharacter(c)))
                {
                    if (MainMenu.DownloadedLocalizationList)
                        MessageBox.Show(MainMenu.Localizations.GetLString(MainMenu.Language, "1-0"), MainMenu.Localizations.GetLString(MainMenu.Language, "1-1"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show("The string contains invalid characters.", "Error importing map", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (map.Length == 0)
                {
                    if (MainMenu.DownloadedLocalizationList)
                        MessageBox.Show(MainMenu.Localizations.GetLString(MainMenu.Language, "1-3"), MainMenu.Localizations.GetLString(MainMenu.Language, "1-1"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show("The clipboard is empty.", "Error importing map", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (maze_height < 2 * 3 + 1 || maze_height > 20 * 3 + 1 || maze_width < 2 * 3 + 1 || maze_width > 20 * 3 + 1)
                {
                    if (MainMenu.DownloadedLocalizationList)
                        MessageBox.Show(MainMenu.Localizations.GetLString(MainMenu.Language, "1-2"), MainMenu.Localizations.GetLString(MainMenu.Language, "1-1"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (MainMenu.DownloadedLocalizationList)
                    MessageBox.Show(MainMenu.Localizations.GetLString(MainMenu.Language, "1-2"), MainMenu.Localizations.GetLString(MainMenu.Language, "1-1"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (MainMenu.DownloadedLocalizationList)
                    MessageBox.Show(MainMenu.Localizations.GetLString(MainMenu.Language, "1-12"), MainMenu.Localizations.GetLString(MainMenu.Language, "1-13"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("The map was successfully copied to the clipboard.", "The map was copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (MainMenu.DownloadedLocalizationList)
                    MessageBox.Show($"{MainMenu.Localizations.GetLString(MainMenu.Language, "1-14")}\n{ex.Message}", MainMenu.Localizations.GetLString(MainMenu.Language, "1-15"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show($"Could not copy the map to the clipboard.\n{ex.Message}", "Copy error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SLIL_Editor_Load(object sender, EventArgs e)
        {
            if (!MainMenu.DownloadedLocalizationList)
            {
                Text = "Editor";
                about.Text = "Elements:";
                size_label.Text = "Field size:";
                accept_size_btn.Text = "Accept";
            }
            else
            {
                Text = MainMenu.Localizations.GetLString(MainMenu.Language, "0-15");
                about.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "1-4");
                size_label.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "1-5");
                accept_size_btn.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "1-6");
            }
            UserControl separator = new UserControl()
            {
                Height = 2,
                Cursor = Program.SLILCursorDefault,
                BackColor = Color.Black,
                Dock = DockStyle.Top
            };
            elements_panel.Controls.Add(separator);
            separator.BringToFront();
            for (int i = 0; i < ElementsCount; i++)
            {
                EditorElementSelector selector = new EditorElementSelector()
                {
                    Index = i,
                    ElementName = GetElementsName(i),
                    ElementColor = ElementsColor[i],
                    Dock = DockStyle.Top
                };
                selector.element_color.Click += Select_btn_Click;
                selector.element_name.Click += Select_btn_Click;
                selector.select_btn.Click += Select_btn_Click;
                elements_panel.Controls.Add(selector);
                selector.BringToFront();
                separator = new UserControl()
                {
                    Height = 2,
                    Cursor = Program.SLILCursorDefault,
                    BackColor = Color.Black,
                    Dock = DockStyle.Top
                };
                elements_panel.Controls.Add(separator);
                separator.BringToFront();
            }
            old_MazeHeight = MazeHeight;
            old_MazeWidth = MazeWidth;
            if (MazeHeight != MazeWidth)
            {
                random_btn.BackColor = Color.LightGray;
                random_btn.Enabled = false;
            }
            GenerateField();
        }

        private void ClearSelector()
        {
            Element = -1;
            foreach (EditorElementSelector selector in elements_panel.Controls.Find("EditorElementSelector", false).Cast<EditorElementSelector>())
                selector.select_btn.Image = null;
        }

        private void Select_btn_Click(object sender, EventArgs e)
        {
            editor_interface.Focus();
            ClearSelector();
            EditorElementSelector parent = (sender as Control).Parent as EditorElementSelector;
            Element = Convert.ToInt32(parent.Index);
            parent.select_btn.Image = Properties.Resources.selected;
        }

        private void GenerateField(string map = "empty")
        {
            if (map == "empty")
            {
                playerExist = false;
                finishCount = 0;
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
                    Color color = Color.Gray;
                    char c = map[i * min + j];
                    if (i != 0 && i != min - 1 && j != 0 && j != max - 1)
                    {
                        if (c == '.')
                            color = Color.White;
                        else if (c == '=')
                            color = Color.Blue;
                        else if (c == 'D')
                            color = Color.DarkOrange;
                        else if (c == 'W')
                            color = Color.Purple;
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
                        else if (c == 'b')
                            color = Color.Brown;
                        else if (c == 'B')
                            color = Color.RosyBrown;
                        else if (c == 'E')
                            color = Color.Navy;
                        else if (c == '5')
                            color = Color.Yellow;
                        else if (c == 'L')
                            color = Color.OliveDrab;
                        else if (c == 'l')
                            color = Color.DarkGoldenrod;
                        else if (c == 'X')
                            color = Color.OrangeRed;
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
                    if (i != 0 && i != min - 1 && j != 0 && j != max - 1)
                        panel.MouseEnter += new EventHandler(Panels_MouseEnter);
                    editor_interface.Controls.Add(panel);
                    panels[i, j] = panel;
                }
            }
            accept_button.Top = reset_btn.Top = random_btn.Top = import_btn.Top = export_btn.Top = question.Top = editor_interface.Bottom + 6;
            size_label.Top = accept_button.Bottom + 3;
            height.Top = size_label.Bottom + 3;
            width.Top = height.Top;
            separator.Top = height.Top + 2;
            accept_size_btn.Top = width.Bottom - accept_size_btn.Height;
            about.Top = height.Bottom + 3;
            height.Value = (MazeHeight - 1) / 3;
            width.Value = (MazeWidth - 1) / 3;
            Width = editor_interface.Right + 274;
            Height = accept_size_btn.Bottom + 43;
            int centerX = Owner.Left + (Owner.Width - Width) / 2;
            int centerY = Owner.Top + (Owner.Height - Height) / 2;
            Location = new Point(centerX, centerY);
        }

        private Map GenerateMap()
        {
            StringBuilder MAP = new StringBuilder();
            for (int i = 0; i < panels.GetLength(0); i++)
            {
                for (int j = 0; j < panels.GetLength(1); j++)
                {
                    if (panels[i, j].BackColor == Color.Gray)
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
                    else if (panels[i, j].BackColor == Color.Brown)
                        MAP.Append("b");
                    else if (panels[i, j].BackColor == Color.RosyBrown)
                        MAP.Append("B");
                    else if (panels[i, j].BackColor == Color.Red)
                        MAP.Append("P");
                    else if (panels[i, j].BackColor == Color.Navy)
                        MAP.Append("E");
                    else if (panels[i, j].BackColor == Color.Yellow)
                        MAP.Append("5");
                    else if (panels[i, j].BackColor == Color.OliveDrab)
                        MAP.Append("L");
                    else if (panels[i, j].BackColor == Color.DarkGoldenrod)
                        MAP.Append("l");
                    else if (panels[i, j].BackColor == Color.OrangeRed)
                        MAP.Append("X");
                }
            }
                return new Map(MAP.ToString(), panels.GetLength(1), panels.GetLength(0), 0);
        }

        private void Panels_MouseEnter(object sender, EventArgs e)
        {
            panel = sender as Panel;
            x_panel = Convert.ToInt32(panel.Name.Split('_')[1]);
            y_panel = Convert.ToInt32(panel.Name.Split('_')[2]);
            panel.Focus();
        }

        private void Reset_btn_Click(object sender, EventArgs e)
        {
            editor_interface.Focus();
            playerExist = false;
            finishCount = 0;
            ClearSelector();
            GenerateField();
        }

        private void Random_btn_Click(object sender, EventArgs e)
        {
            editor_interface.Focus();
            finishCount = 0;
            playerExist = true;
            StringBuilder sb = new StringBuilder();
            var map = new Generator().GenerateMap(12);
            MazeWidth = map.Width;
            MazeHeight = map.Height;
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    if (map.GetChar(x, y) == 'F') finishCount++;
                    sb.Append(map.GetChar(x, y));
                }
            }
            GenerateField(sb.ToString());
        }

        private void Accept_button_Click(object sender, EventArgs e)
        {
            editor_interface.Focus();
            if (!playerExist)
            {
                if (MainMenu.DownloadedLocalizationList)
                    MessageBox.Show(MainMenu.Localizations.GetLString(MainMenu.Language, "1-9"), MainMenu.Localizations.GetLString(MainMenu.Language, "1-10"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Missing player", "The map is not completed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (finishCount == 0)
            {
                if (MainMenu.DownloadedLocalizationList)
                    MessageBox.Show(MainMenu.Localizations.GetLString(MainMenu.Language, "1-11"), MainMenu.Localizations.GetLString(MainMenu.Language, "1-10"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (MainMenu.DownloadedLocalizationList)
                MessageBox.Show(MainMenu.Localizations.GetLString(MainMenu.Language, "1-7"), MainMenu.Localizations.GetLString(MainMenu.Language, "1-8"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            ClearSelector();
            GenerateField();
        }

        private void SLIL_Editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (panel == null) return;
            int index = Element;
            if (panel.BackColor == Color.White)
            {
                if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
                {
                    if (index == -1) return;
                    if (index == 0 && !playerExist)
                    {
                        panel.BackColor = Color.Red;
                        playerExist = true;
                    }
                    else if (index == 1)
                        panel.BackColor = Color.Navy;
                    else if (index == 2)
                        panel.BackColor = Color.Yellow;
                    else if (index == 3)
                        panel.BackColor = Color.Gray;
                    else if (index == 4)
                        panel.BackColor = Color.Orange;
                    else if (index == 5)
                        panel.BackColor = Color.Blue;
                    else if (index == 6)
                    {
                        panel.BackColor = Color.Lime;
                        finishCount++;
                    }
                    else if (index == 7)
                    {
                        panel.BackColor = Color.Pink;
                        for (int i = x_panel - 1; i <= x_panel + 1; i++)
                        {
                            for (int j = y_panel - 1; j <= y_panel + 1; j++)
                            {
                                if (j <= 0 && j >= panels.GetLength(1) && i <= 0 && i >= panels.GetLength(0))
                                    continue;
                                if (i == x_panel && j == y_panel)
                                    continue;
                                panels[i, j].BackColor = Color.Gray;
                            }
                        }
                        bool spawned = false;
                        if (y_panel >= 2 && y_panel < panels.GetLength(0) - 2 && x_panel >= 0 && x_panel < panels.GetLength(1) && panels[x_panel, y_panel - 2].BackColor == Color.White)
                        {
                            try
                            {
                                if (!spawned)
                                {
                                    panels[x_panel, y_panel - 1].BackColor = Color.DarkOrange;
                                    spawned = true;
                                }
                            }
                            catch { }
                        }
                        if (y_panel >= 0 && y_panel < panels.GetLength(0) - 2 && x_panel >= 0 && x_panel < panels.GetLength(1) && panels[x_panel, y_panel + 2].BackColor == Color.White)
                        {
                            try
                            {
                                if (!spawned)
                                {
                                    panels[x_panel, y_panel + 1].BackColor = Color.DarkOrange;
                                    spawned = true;
                                }
                            }
                            catch { }
                        }
                        if (y_panel >= 0 && y_panel < panels.GetLength(0) && x_panel >= 2 && x_panel < panels.GetLength(1) - 2 && panels[x_panel - 2, y_panel].BackColor == Color.White)
                        {
                            try
                            {
                                if (!spawned)
                                {
                                    panels[x_panel - 1, y_panel].BackColor = Color.DarkOrange;
                                    spawned = true;
                                }
                            }
                            catch { }
                        }
                        if (y_panel >= 0 && y_panel < panels.GetLength(0) && x_panel >= 0 && x_panel < panels.GetLength(1) - 2 && panels[x_panel + 2, y_panel].BackColor == Color.White)
                        {
                            try
                            {
                                if (!spawned)
                                {
                                    panels[x_panel + 1, y_panel].BackColor = Color.DarkOrange;
                                    spawned = true;
                                }
                            }
                            catch { }
                        }
                        if (!spawned)
                        {
                            if (y_panel - 1 > 0)
                                panels[x_panel, y_panel - 1].BackColor = Color.DarkOrange;
                            if (y_panel + 1 < panels.GetLength(0) - 1)
                                panels[x_panel, y_panel + 1].BackColor = Color.DarkOrange;
                            if (x_panel - 1 > 0)
                                panels[x_panel - 1, y_panel].BackColor = Color.DarkOrange;
                            if (x_panel + 1 < panels.GetLength(1) - 1)
                                panels[x_panel + 1, y_panel].BackColor = Color.DarkOrange;
                        }
                    }
                    else if (index == 8)
                        panel.BackColor = Color.Brown;
                    else if (index == 9)
                        panel.BackColor = Color.RosyBrown;
                    else if (index == 10)
                        panel.BackColor = Color.OliveDrab;
                    else if (index == 11)
                        panel.BackColor = Color.DarkGoldenrod;
                    else if (index == 12)
                        panel.BackColor = Color.OrangeRed;
                    else if (index == 13)
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