using LiteNetLib;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer
{
    internal class GameServerConsole
    {
        internal const string version = "|1.3|";
        private const int GWL_STYLE = -16;
        private const int WS_SIZEBOX = 0x00040000;
        private const int WS_MAXIMIZEBOX = 0x00010000;
        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;
        const int STD_INPUT_HANDLE = -10;
        const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        const uint ENABLE_INSERT_MODE = 0x0020;
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        private static void SetupConsoleSettings()
        {
            int width = 80;
            int height = 34;
            Console.Title = "GameServer for SLIL";
            Console.SetWindowSize(width, height);
            Console.OutputEncoding = Encoding.UTF8;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.SetBufferSize(width, height);
                Console.CursorSize = 100;
                int screenWidth = GetSystemMetrics(SM_CXSCREEN);
                int screenHeight = GetSystemMetrics(SM_CYSCREEN);
                IntPtr consoleWindow = GetConsoleWindow();
                GetWindowRect(consoleWindow, out RECT rect);
                int consoleWidthPixels = rect.Right - rect.Left;
                int consoleHeightPixels = rect.Bottom - rect.Top;
                int posX = (screenWidth - consoleWidthPixels) / 2;
                int posY = (screenHeight - consoleHeightPixels) / 2;
                MoveWindow(consoleWindow, posX, posY, consoleWidthPixels, consoleHeightPixels, true);
                int style = GetWindowLong(consoleWindow, GWL_STYLE);
                style &= ~WS_SIZEBOX;
                style &= ~WS_MAXIMIZEBOX;
                _ = SetWindowLong(consoleWindow, GWL_STYLE, style);
                IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);
                GetConsoleMode(consoleHandle, out uint consoleMode);
                SetConsoleMode(consoleHandle, consoleMode & ~(ENABLE_QUICK_EDIT_MODE | ENABLE_EXTENDED_FLAGS | ENABLE_INSERT_MODE));
            }
        }

        internal static void Main()
        {
            SetupConsoleSettings();
            GameServerProgram program = new();
            Mutex mutex = new(true, "SLIL_GameServer_Unique_Mutex");
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                program.MainMenu();
                mutex.ReleaseMutex();
            }
        }
    }

    internal class Clipboard
    {
        const uint GMEM_MOVEABLE = 0x0002;
        const uint CF_UNICODETEXT = 13;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalFree(IntPtr hMem);

        internal static void SetText(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (!OpenClipboard(IntPtr.Zero))
                throw new InvalidOperationException("Unable to open clipboard.");
            try
            {
                EmptyClipboard();
                byte[] bytes = Encoding.Unicode.GetBytes(text);
                int dataSize = bytes.Length + 2;
                IntPtr hGlobal = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)dataSize);
                if (hGlobal == IntPtr.Zero)
                    throw new OutOfMemoryException("Unable to allocate memory for clipboard data.");
                try
                {
                    IntPtr locked = GlobalLock(hGlobal);
                    if (locked == IntPtr.Zero)
                        throw new InvalidOperationException("Unable to lock memory for clipboard data.");
                    try
                    {
                        Marshal.Copy(bytes, 0, locked, bytes.Length);
                        Marshal.WriteInt16(locked, bytes.Length, 0);
                    }
                    finally { GlobalUnlock(locked); }
                    if (SetClipboardData(CF_UNICODETEXT, hGlobal) == IntPtr.Zero)
                        throw new InvalidOperationException("Unable to set clipboard data.");
                    hGlobal = IntPtr.Zero;
                }
                finally
                {
                    if (hGlobal != IntPtr.Zero)
                        GlobalFree(hGlobal);
                }
            }
            finally { CloseClipboard(); }
        }
    }

    internal class GameServerProgram
    {
        //private readonly NetPacketProcessor processor = new();
        private readonly EventBasedNetListener Listener;
        private NetManager Server;
        private readonly Dispatcher Dispatcher;
        private readonly SendOutcomingMessageDelegate? SendOutcomingMessageHandle;
        private string ServerPassword = "None";
        private bool Exit = false, ServerStarted = false, StopedThread = true;
        private readonly int[] DefaultSettingsValue = [0, 1, 3, 1], SettingsValue = [0, 1, 3, 1], SettingsMaxValue = [3, 3, 7, 1];
        private int[] ServerSettingsValue = [0, 1, 3, 1];
        private readonly string[] SettingsItems, ServerSettingsItem;
        private int SelectedCategory = 0, Selected = 0, SelectedDifficult = 1;
        private int MaxCommandIndex = 6;
        private readonly List<string> BannedPlayersList;
        private GameModes GameMode = GameModes.Classic;
        private readonly string[] Difficulties = ["Easy", "Normal", "Hard", "Very hard"];
        private readonly ConsoleColor[] DifficultyColors = { ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Red, ConsoleColor.DarkRed };
        private readonly ConsoleColor[] GameModeColors = { ConsoleColor.White, ConsoleColor.Red, ConsoleColor.DarkGray, ConsoleColor.DarkGray };
        private readonly string[] MenuItems =
        [
            "How to play together",
            "Start the server",
            "Stop the server",
            "Players list",
            "Banned players list",
            "Set difficulty",
            "Select game mode",
            "Start the game",
            "Stop the game",
            "Settings",
            "Exit the program"
        ];
        private readonly Dictionary<string, List<string>> CategorizedMenu;
        private int CurrentCategory = -1;
        private string StatusMessage = "The status text will be displayed here";
        private int MAX_CONNECTIONS = 4;
        //server.UnsyncedEvents = true;
        //server.UpdateTime = 1;

        internal GameServerProgram()
        {
            BannedPlayersList = [];
            SettingsItems =
            [
                "Default game mode",
                "Default difficulty",
                "Max Players",
                "PVP"
            ];
            ServerSettingsItem =
            [
                "Game mode",
                "Difficulty",
                "Max Players",
                "PVP"
            ];
            Listener = new();
            Server = new(Listener);
            Dispatcher = new();
            CategorizedMenu = new Dictionary<string, List<string>>
            {
                ["Server Management"] = [],
                ["Player Management"] = [],
                ["Game Control"] = [],
                ["Game Settings"] = [],
                ["Information"] = [],
                ["System"] = []
            };
            CategorizedMenu["Information"].Add(MenuItems[0]);
            CategorizedMenu["Server Management"].Add(MenuItems[1]);
            CategorizedMenu["Server Management"].Add(MenuItems[2]);
            CategorizedMenu["Player Management"].Add(MenuItems[3]);
            CategorizedMenu["Player Management"].Add(MenuItems[4]);
            CategorizedMenu["Game Settings"].Add(MenuItems[5]);
            CategorizedMenu["Game Settings"].Add(MenuItems[6]);
            CategorizedMenu["Game Control"].Add(MenuItems[7]);
            CategorizedMenu["Game Control"].Add(MenuItems[8]);
            CategorizedMenu["System"].Add(MenuItems[9]);
            CategorizedMenu["System"].Add(MenuItems[10]);
            SendOutcomingMessageHandle = SendOutcomingMessageInvoker;
            Dispatcher.sendMessageDelegate = SendOutcomingMessageHandle;
            Listener.ConnectionRequestEvent += request =>
            {
                string data = request.Data.GetString();
                if (Server.ConnectedPeersCount < MAX_CONNECTIONS && data.StartsWith("SomeKey:"))
                {
                    string[] request_data = data.Replace("SomeKey:", "").Split('|');
                    string name = request_data[0];
                    if (BannedPlayersList.Contains(name))
                    {
                        request.Reject();
                        return;
                    }
                    else
                    {
                        string password = request_data[1];
                        if (ServerPassword != "None" && password != ServerPassword)
                            request.Reject();
                        else
                            Dispatcher.AppendPlayerPeerDictionary(request.Accept().Id, name);
                    }
                }
                else request.Reject();
            };
            Listener.PeerConnectedEvent += peer =>
            {
                StatusMessage = $"<{DateTime.Now:hh:mm}> We got connection: {peer}";
                Dispatcher.SendOutcomingMessage(100, ref peer);
            };
            Listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
            {
                Packet pack = new();
                pack.Deserialize(dataReader);
                int playerIDFromPeer = Dispatcher.PeerPlayerIDs[fromPeer.Id];
                Dispatcher.DispatchIncomingMessage(pack.PacketID, pack.Data, ref Server, playerIDFromPeer);
            };
            Listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
            {
                Dispatcher.RemovePlayer(Dispatcher.PeerPlayerIDs[peer.Id]);
                Dispatcher.PeerPlayerIDs.Remove(peer.Id);
                StatusMessage = $"<{DateTime.Now:hh:mm}> Closed connection: {peer}";
                Dispatcher.PeerPlayerNames.Remove(peer.Id);
            };
            try
            {
                if (System.IO.File.Exists("data.txt"))
                {
                    string[] data = System.IO.File.ReadAllText("data.txt").Split(';');
                    SettingsValue = [Convert.ToInt32(data[0]), Convert.ToInt32(data[1]), Convert.ToInt32(data[2]), Convert.ToInt32(data[3])];
                }
                else SettingsToDefault();
            }
            catch { SettingsToDefault(); }
            for (int i = 0; i < SettingsValue.Length; i++)
            {
                if (SettingsValue[i] < 0 || SettingsValue[i] > SettingsMaxValue[i])
                    SettingsValue[i] = DefaultSettingsValue[i];
            }
            GameMode = (GameModes)SettingsValue[0];
            Dispatcher.ChangeGameMode(GameMode);
            SelectedDifficult = SettingsValue[1];
            Dispatcher.ChangeDifficulty(SettingsValue[1]);
            MAX_CONNECTIONS = SettingsValue[2] + 1;
        }

        private void SettingsToDefault()
        {
            for (int i = 0; i < DefaultSettingsValue.Length; i++)
                SettingsValue[i] = DefaultSettingsValue[i];
        }

        private async static Task DownloadFileAsync(string url, string outputPath)
        {
            using var httpClient = new HttpClient();
            try
            {
                using var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                using var stream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
                await stream.CopyToAsync(fileStream);
            }
            catch { }
        }

        private async static Task<bool> CheckUpdate()
        {
            return true;
            try
            {
                using HttpClient httpClient = new();
                string content = await httpClient.GetStringAsync("https://base-escape.ru/version_SLIL_GameServer.txt");
                string line = content.Split(["\r\n", "\r", "\n"], StringSplitOptions.None)[0];
                if (!line.Contains(GameServerConsole.version))
                {
                    if (!File.Exists("UpdateDownloader.exe"))
                        await DownloadFileAsync("https://base-escape.ru/downloads/UpdateDownloader.exe", "UpdateDownloader.exe");
                    Console.Write("\nThe game server version is out of date!");
                    Console.Write("\nPress any button to download the update...");
                    Console.ReadKey();
                    Process.Start(new ProcessStartInfo("UpdateDownloader.exe", "https://base-escape.ru/downloads/GameServer.zip GameServer true"));
                    return false;
                }
                else return true;
            }
            catch
            {
                Console.Write("\nError checking for updates!");
                Console.Write("\nPress any key to exit...");
                Console.ReadKey();
                return false;
            }
        }

        internal void MainMenu()
        {
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Cyan;
            DrawBorder('╔', '╗', '═', 52);
            WriteColoredCenteredText("Welcome to GameServer for SLIL", ConsoleColor.Yellow, 52);
            WriteColoredCenteredText($"Version {GameServerConsole.version.Trim('|')}", ConsoleColor.Yellow, 52);
            DrawBorder('╟', '╢', '─', 52);
            WriteColoredCenteredText("Checking for updates...", ConsoleColor.Green, 52);
            DrawBorder('╚', '╝', '═', 52);
            Console.ResetColor();
            Thread.Sleep(1000);
            while (Console.KeyAvailable) Console.ReadKey(true);
            if (!CheckUpdate().Result) return;
            bool only_buttons = false;
            while (!Exit)
            {
                DisplayMainMenu(only_buttons);
                only_buttons = false;
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        if (CurrentCategory == -1)
                        {
                            SelectedCategory--;
                            if (SelectedCategory < 0)
                                SelectedCategory = MaxCommandIndex;
                        }
                        else
                        {
                            Selected--;
                            if (Selected < 0)
                                Selected = MaxCommandIndex;
                        }
                        only_buttons = true;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        if (CurrentCategory == -1)
                        {
                            SelectedCategory++;
                            if (SelectedCategory > MaxCommandIndex)
                                SelectedCategory = 0;
                        }
                        else
                        {
                            Selected++;
                            if (Selected > MaxCommandIndex)
                                Selected = 0;
                        }
                        only_buttons = true;
                        break;
                    case ConsoleKey.I:
                        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) break;
                        if (ServerStarted)
                        {
                            try
                            {
                                Clipboard.SetText($"{GetLocalIPAddress()}:{Server.LocalPort}");
                                StatusMessage = "IP successfully copied to clipboard";
                            }
                            catch { StatusMessage = "An error occurred while copying IP"; }
                        }
                        break;
                    case ConsoleKey.P:
                        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) break;
                        if (ServerStarted)
                        {
                            if (ServerPassword != "None")
                            {
                                try
                                {
                                    Clipboard.SetText(ServerPassword);
                                    StatusMessage = "Password successfully copied to clipboard";
                                }
                                catch { StatusMessage = "An error occurred while copying the password"; }
                            }
                            else StatusMessage = "Password not set...";
                        }
                        break;
                    case ConsoleKey.D1:
                    case ConsoleKey.D2:
                    case ConsoleKey.D3:
                    case ConsoleKey.D4:
                    case ConsoleKey.D5:
                    case ConsoleKey.D6:
                    case ConsoleKey.D7:
                    case ConsoleKey.D8:
                    case ConsoleKey.D9:
                        if (CurrentCategory == -1)
                        {
                            SelectedCategory = int.Parse(key.ToString().Trim('D')) - 1;
                            if (SelectedCategory > MaxCommandIndex)
                                SelectedCategory = MaxCommandIndex;
                        }
                        else
                        {
                            Selected = int.Parse(key.ToString().Trim('D')) - 1;
                            if (Selected > MaxCommandIndex)
                                Selected = MaxCommandIndex;
                        }
                        break;
                    case ConsoleKey.D0:
                        if (CurrentCategory == -1)
                        {
                            SelectedCategory = 9;
                            if (SelectedCategory > MaxCommandIndex)
                                SelectedCategory = MaxCommandIndex;
                        }
                        else
                        {
                            Selected = 9;
                            if (Selected > MaxCommandIndex)
                                Selected = MaxCommandIndex;
                        }
                        break;
                    case ConsoleKey.Enter:
                        if (CurrentCategory == -1)
                            ProcessingCommands(SelectedCategory);
                        else
                            ProcessingCommands(Selected);
                        break;
                    case ConsoleKey.Escape:
                        if (CurrentCategory != -1)
                            ProcessingCommands(999);
                        else
                        {
                            CurrentCategory = 5;
                            ProcessingCommands(1);
                        }
                        break;
                    default:
                        only_buttons = true;
                        break;
                }
            }
        }

        private void DisplayMainMenu(bool only_buttons)
        {
            Console.CursorVisible = false;
            const int windowWidth = 52;
            if (only_buttons)
            {
                if (ServerStarted) Console.SetCursorPosition(0, 14);
                else Console.SetCursorPosition(0, 8);
                if (CurrentCategory == -1)
                {
                    MaxCommandIndex = CategorizedMenu.Count - 1;
                    for (int i = 0; i < CategorizedMenu.Count; i++)
                        DrawButton($"{i + 1}. " + CategorizedMenu.Keys.ElementAt(i), i == SelectedCategory, windowWidth);
                }
                else
                {
                    string key = CategorizedMenu.Keys.ElementAt(CurrentCategory);
                    MaxCommandIndex = CategorizedMenu[key].Count - 1;
                    for (int i = 0; i < CategorizedMenu[key].Count; i++)
                        DrawButton($"{i + 1}. " + CategorizedMenu[key][i], i == Selected, windowWidth);
                }
                return;
            }
            Console.Clear();
            DrawBorder('╔', '╗', '═', windowWidth);
            WriteColoredCenteredText("Server Info", ConsoleColor.Yellow, windowWidth);
            if (ServerStarted)
            {
                WriteColoredCenteredText("┌────────────────────────────┐", ConsoleColor.Cyan, windowWidth);
                WriteServerInfoLine("Status", "Online", ConsoleColor.Green, windowWidth);
                WriteServerInfoLine("Game", Dispatcher.GameStarted() ? "Started" : "Stoped",
                    Dispatcher.GameStarted() ? ConsoleColor.Green : ConsoleColor.Red, windowWidth);
                WriteServerInfoLine("PVP", Dispatcher.GetPVP() ? "On" : "Off",
                    Dispatcher.GetPVP() ? ConsoleColor.DarkRed : ConsoleColor.Green, windowWidth);
                WriteServerInfoLine("IP", $"{GetLocalIPAddress()}:{Server.LocalPort}", ConsoleColor.White, windowWidth);
                WriteServerInfoLine("Password", ServerPassword == "None" ? "Not set" : ServerPassword,
                                    ServerPassword == "None" ? ConsoleColor.DarkGray : ConsoleColor.White, windowWidth);
                WriteServerInfoLine("Game mode", GameMode.ToString(), GameModeColors[(int)GameMode], windowWidth);
                WriteServerInfoLine("Difficult", Difficulties[SelectedDifficult], DifficultyColors[SelectedDifficult], windowWidth);
                WriteServerInfoLine("Max Players", MAX_CONNECTIONS.ToString(), ConsoleColor.White, windowWidth);
                WriteColoredCenteredText("└────────────────────────────┘", ConsoleColor.Cyan, windowWidth);
            }
            else
            {
                WriteColoredCenteredText("┌────────────────────────────┐", ConsoleColor.Cyan, windowWidth);
                WriteServerInfoLine("Status", "Offline", ConsoleColor.Red, windowWidth);
                WriteColoredCenteredText("└────────────────────────────┘", ConsoleColor.Cyan, windowWidth);
            }
            DrawBorder('║', '║', ' ', windowWidth);
            WriteColoredCenteredText(StatusMessage, ConsoleColor.Magenta, windowWidth);
            DrawBorder('╟', '╢', '─', windowWidth);
            if (CurrentCategory == -1)
            {
                MaxCommandIndex = CategorizedMenu.Count - 1;
                for (int i = 0; i < CategorizedMenu.Count; i++)
                    DrawButton($"{i + 1}. " + CategorizedMenu.Keys.ElementAt(i), i == SelectedCategory, windowWidth);
            }
            else
            {
                string key = CategorizedMenu.Keys.ElementAt(CurrentCategory);
                MaxCommandIndex = CategorizedMenu[key].Count - 1;
                for (int i = 0; i < CategorizedMenu[key].Count; i++)
                    DrawButton($"{i + 1}. " + CategorizedMenu[key][i], i == Selected, windowWidth);
            }
            DrawBorder('╟', '╢', '─', windowWidth);
            WriteColoredCenteredText("↑↓: Move    ESC: Exit    Enter: Confirm", ConsoleColor.Green, windowWidth);
            if (ServerStarted && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                WriteColoredCenteredText("I: Copy IP    P: Copy Password", ConsoleColor.Green, windowWidth);
            DrawBorder('╟', '╢', '─', windowWidth);
            WriteColoredCenteredText("Developed by: Fatalan & Lonewolf239", ConsoleColor.Yellow, windowWidth);
            WriteColoredCenteredText("GUI designed by: Lonewolf239", ConsoleColor.Yellow, windowWidth);
            DrawBorder('╚', '╝', '═', windowWidth);
            WriteColoredCenteredText($"v{GameServerConsole.version.Trim('|')}", ConsoleColor.DarkGray, windowWidth, false);
        }

        private static void DrawBorder(char left, char right, char fill, int width)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{left}{new string(fill, width - 2)}{right}");
            Console.ResetColor();
        }

        private static void WriteServerInfoLine(string label, string value, ConsoleColor valueColor, int windowWidth)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write('║' + " ".PadRight((windowWidth - 4 - label.Length - value.Length) / 2));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{label}: ");
            Console.ForegroundColor = valueColor;
            Console.Write(value);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" ".PadLeft((windowWidth - 4 - label.Length - value.Length + 1) / 2) + '║');
            Console.ResetColor();
        }

        private static void WriteColoredCenteredText(string text, ConsoleColor color, int width, bool drawBorder = true)
        {
            Console.ResetColor();
            if (drawBorder)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write('║');
            }
            Console.ForegroundColor = color;
            Console.Write(CenterText(text, drawBorder ? width - 2 : width));
            if (drawBorder)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine('║');
            }
            Console.ResetColor();
        }

        private static void DrawButton(string text, bool isSelected, int width)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write('║');
            Console.ResetColor();
            string leftPadding = "   ";
            if (isSelected)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                leftPadding = " ➤ ";
            }
            else
                Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(leftPadding + text.PadRight(width - 5));
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine('║');
            Console.ResetColor();
        }

        private static string GetInput(int maxLength, bool onlyDigit)
        {
            string input = "";
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                    Console.Write("\b \b");
                }
                else if (input.Length < maxLength)
                {
                    if (onlyDigit && !char.IsDigit(key.KeyChar)) continue;
                    if (char.IsLetterOrDigit(key.KeyChar) || key.KeyChar == '_')
                    {
                        input += key.KeyChar;
                        Console.Write(key.KeyChar);
                    }
                }
            }
            return input;
        }

        private void ProcessingCommands(int command)
        {
            if (CurrentCategory == -1)
            {
                CurrentCategory = command;
                Selected = 0;
                return;
            }
            if (command == 999)
            {
                CurrentCategory = -1;
                Selected = 0;
                return;
            }
            string keyCategorizedMenu = CategorizedMenu.Keys.ElementAt(CurrentCategory);
            string commandInCategory = CategorizedMenu[keyCategorizedMenu][command];
            switch (commandInCategory)
            {
                case "How to play together":
                    string[] howPlayText =
                    [
                        "╔════════════════════════════════════════════════════════════════╗",
                        "║                                                                ║",
                        "║   ATTENTION THIS IS NOT THE FINAL VERSION OF THE MULTIPLAYER   ║",
                        "║         IF YOU FIND ANY BUGS, PLEASE REPORT THEM TO US         ║",
                        "║                                                                ║",
                        "╟────────────────────────────────────────────────────────────────╢",
                        "║  How to play together:                                         ║",
                        "║                                                                ║",
                        "║  1. The host must start the server.                            ║",
                        "║  2. All players must be on the same local network as the host. ║",
                        "║  3. After creating the server, the host should copy the IP     ║",
                        "║     and port from the menu and send it to friends.             ║",
                        "║  4. Friends should enter the IP:port in the game and click     ║",
                        "║     'Connect'. Enter the password if required.                 ║",
                        "║  5. The host should also connect using the same IP:port.       ║",
                        "║  6. Once everyone is connected, the host can start the game    ║",
                        "║     on the server.                                             ║",
                        "║  7. Now you're ready to play together!                         ║",
                        "║                                                                ║",
                        "║  Enjoy your multiplayer experience!                            ║",
                        "║                                                                ║",
                        "║  (I'm not sure if this will work through Hamachi)              ║"
                    ];
                    DisplayTextMessage(howPlayText);
                    break;
                case "Start the server":
                    const int windowWidth = 54;
                    ServerSettingsValue = (int[])SettingsValue.Clone();
                    Console.Clear();
                    Console.CursorVisible = true;
                    DrawBorder('╔', '╗', '═', windowWidth);
                    WriteColoredCenteredText("Server Configuration", ConsoleColor.Yellow, windowWidth);
                    DrawBorder('╟', '╢', '─', windowWidth);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write('║');
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(" Enter port (1000-9999, empty to set default): ".PadRight(windowWidth - 2));
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine('║');
                    Console.Write('║');
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(" Enter password (empty for no password): ".PadRight(windowWidth - 2));
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine('║');
                    DrawBorder('╟', '╢', '─', windowWidth);
                    for (int i = 0; i < ServerSettingsItem.Length; i++)
                        DrawSettingsParametr(i, ServerSettingsItem[i], false, windowWidth, ServerSettingsValue[i], SettingsMaxValue[i], ConsoleColor.DarkGray);
                    DrawBorder('╚', '╝', '═', windowWidth);
                    Console.SetCursorPosition(48, 3);
                    Console.ForegroundColor = ConsoleColor.Green;
                    string input = GetInput(4, true);
                    if (input.Length == 0) input = "0";
                    int port = Convert.ToInt32(input);
                    if (port < 1000) port = 9999;
                    Console.SetCursorPosition(48, 3);
                    Console.Write(port);
                    Console.SetCursorPosition(42, 4);
                    ServerPassword = GetInput(10, false);
                    Console.CursorVisible = false;
                    if (ServerPassword.Length < 4)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        ServerPassword = "None";
                    }
                    Console.SetCursorPosition(42, 4);
                    Console.Write(ServerPassword);
                    int selectedServerIndex = 0;
                    ConsoleKey key;
                    while (true)
                    {
                        Console.SetCursorPosition(0, 5);
                        DrawBorder('╟', '╢', '─', windowWidth);
                        for (int i = 0; i < ServerSettingsItem.Length; i++)
                        {
                            if ((GameModes)ServerSettingsValue[0] == GameModes.Deathmatch && i == 3)
                                DrawSettingsParametr(i, ServerSettingsItem[i], i == selectedServerIndex, windowWidth, ServerSettingsValue[i], SettingsMaxValue[i], ConsoleColor.DarkGray);
                            else
                                DrawSettingsParametr(i, ServerSettingsItem[i], i == selectedServerIndex, windowWidth, ServerSettingsValue[i], SettingsMaxValue[i]);
                        }
                        DrawBorder('╟', '╢', '─', windowWidth);
                        WriteColoredCenteredText("↑↓: Move    ←→: Change value", ConsoleColor.Green, windowWidth);
                        WriteColoredCenteredText("ESC: Cancel    Enter: Confirm", ConsoleColor.Green, windowWidth);
                        DrawBorder('╚', '╝', '═', windowWidth);
                        key = Console.ReadKey(true).Key;
                        if (key == ConsoleKey.UpArrow || key == ConsoleKey.W)
                        {
                            selectedServerIndex--;
                            if (selectedServerIndex < 0)
                                selectedServerIndex = ServerSettingsItem.Length - 1;
                        }
                        else if (key == ConsoleKey.DownArrow || key == ConsoleKey.S)
                        {
                            selectedServerIndex++;
                            if (selectedServerIndex >= ServerSettingsItem.Length)
                                selectedServerIndex = 0;
                        }
                        if (key == ConsoleKey.LeftArrow || key == ConsoleKey.A)
                        {
                            if (ServerSettingsValue[selectedServerIndex] > 0)
                                ServerSettingsValue[selectedServerIndex]--;
                            ChangeSettingsValue(selectedServerIndex, ServerSettingsValue[selectedServerIndex]);
                        }
                        else if (key == ConsoleKey.RightArrow || key == ConsoleKey.D)
                        {
                            if (ServerSettingsValue[selectedServerIndex] < SettingsMaxValue[selectedServerIndex])
                                ServerSettingsValue[selectedServerIndex]++;
                            ChangeSettingsValue(selectedServerIndex, ServerSettingsValue[selectedServerIndex]);
                        }
                        if ((GameModes)ServerSettingsValue[0] == GameModes.Deathmatch) ServerSettingsValue[3] = 1;
                        if (key == ConsoleKey.Escape)
                        {
                            StatusMessage = "Server startup cancelled...";
                            return;
                        }
                        if (key == ConsoleKey.Enter)
                        {
                            try
                            {
                                MAX_CONNECTIONS = ServerSettingsValue[2] + 1;
                                Server.Start(port);
                                ServerStarted = true;
                                StopedThread = false;
                                PacketsThread();
                                GameMode = (GameModes)ServerSettingsValue[0];
                                Dispatcher.ChangeGameMode(GameMode);
                                SelectedDifficult = ServerSettingsValue[1];
                                Dispatcher.ChangeDifficulty(ServerSettingsValue[1]);
                                Dispatcher.SetPVP(ServerSettingsValue[3] == 1);
                                StatusMessage = $"Server started successfully";
                            }
                            catch (Exception e)
                            {
                                ServerStarted = false;
                                StopedThread = true;
                                StatusMessage = $"Error when starting server: {e.Message}";
                            }
                            return;
                        }
                    }
                case "Stop the server":
                    Server.Stop();
                    ServerStarted = false;
                    StopedThread = true;
                    StatusMessage = "Server stopped successfully";
                    break;
                case "Players list":
                    if (!ServerStarted)
                        StatusMessage = "Please start the server first";
                    else
                    {
                        Dictionary<int, string> players = Dispatcher.GetPlayers();
                        int[] playersID = players.Select(p => p.Key).ToArray();
                        (int, bool) selected_player = SelectPlayers(players);
                        int playerId = selected_player.Item1;
                        bool ban = selected_player.Item2;
                        if (playerId != -1)
                        {
                            if (YesNoMessage())
                            {
                                try
                                {
                                    if (ban)
                                    {
                                        BannedPlayersList.Add(players.Values.ElementAt(playerId));
                                        string name = players.Values.ElementAt(playerId);
                                        playerId = players.Keys.ElementAt(playerId);
                                        Dispatcher.KickPlayer(playerId, ref Server);
                                        StatusMessage = $"Player {name} has been baned";
                                    }
                                    else
                                    {
                                        string name = players.Values.ElementAt(playerId);
                                        playerId = players.Keys.ElementAt(playerId);
                                        Dispatcher.KickPlayer(playerId, ref Server);
                                        StatusMessage = $"Player {name} has been kicked from the server";
                                    }
                                }
                                catch { StatusMessage = "An unknown error occurred..."; }
                            }
                        }
                    }
                    break;
                case "Banned players list":
                    if (!ServerStarted)
                        StatusMessage = "Please start the server first";
                    else
                    {
                        int selectedPlayer = SelectOption("Banned players list", [.. BannedPlayersList], "No banned players...", "Exit", "Unban");
                        if (selectedPlayer == -1)
                            return;
                        string bannedPlayerName = BannedPlayersList[selectedPlayer];
                        BannedPlayersList.RemoveAt(selectedPlayer);
                        StatusMessage = $"Player {bannedPlayerName} was unbanned";
                    }
                    break;
                case "Set difficulty":
                    if (!ServerStarted)
                        StatusMessage = "Please start the server first";
                    else
                    {
                        try
                        {
                            int selectedIndex = SelectOption("Select difficulty:", Difficulties);
                            if (selectedIndex == -1)
                            {
                                StatusMessage = "The difficulty change has been cancelled.";
                                return;
                            }
                            SelectedDifficult = selectedIndex;
                            Dispatcher.ChangeDifficulty(selectedIndex);
                            StatusMessage = $"Difficulty set to {Difficulties[selectedIndex]}";
                        }
                        catch (Exception e) { StatusMessage = $"Error setting difficulty: {e.Message}"; }
                    }
                    break;
                case "Select game mode":
                    if (!ServerStarted)
                        StatusMessage = "Please start the server first";
                    else
                    {
                        try
                        {
                            string[] gameModes = Enum.GetNames(typeof(GameModes));
                            int selectedIndex = SelectOption("Select game mode:", gameModes);
                            if (selectedIndex == -1)
                            {
                                StatusMessage = "The game mode change has been cancelled.";
                                return;
                            }
                            GameMode = (GameModes)selectedIndex;
                            Dispatcher.ChangeGameMode(GameMode);
                            StatusMessage = $"Game mode set to {GameMode}";
                        }
                        catch (Exception e) { StatusMessage = $"Error setting game mode: {e.Message}"; }
                    }
                    break;
                case "Start the game":
                    if (!ServerStarted)
                        StatusMessage = "Please start the server first";
                    else
                    {
                        Dispatcher.StartGame();
                        StatusMessage = "New game started on the server";
                    }
                    break;
                case "Stop the game":
                    if (!ServerStarted)
                        StatusMessage = "Please start the server first";
                    else
                    {
                        Dispatcher.StopGame();
                        StatusMessage = "Game stopped on the server";
                    }
                    break;
                case "Settings":
                    if (!ServerStarted) Settings();
                    else StatusMessage = "Please stop the server first";
                    break;
                case "Exit the program":
                    if (YesNoMessage())
                    {
                        Server.Stop();
                        Exit = true;
                    }
                    break;

            }
        }

        private void Settings()
        {
            const int windowWidth = 52;
            int selectedIndex = 0;
            ConsoleKey key;
            Console.Clear();
            DrawBorder('╔', '╗', '═', windowWidth);
            WriteColoredCenteredText("Settings", ConsoleColor.Yellow, windowWidth);
            DrawBorder('╟', '╢', '─', windowWidth);
            for (int i = 0; i < SettingsItems.Length; i++) Console.WriteLine("");
            DrawBorder('╟', '╢', '─', windowWidth);
            WriteColoredCenteredText("↑↓: Move    ←→: Change value", ConsoleColor.Green, windowWidth);
            WriteColoredCenteredText("R: Reset    ESC: Exit", ConsoleColor.Green, windowWidth);
            DrawBorder('╚', '╝', '═', windowWidth);
            while (true)
            {
                Console.SetCursorPosition(0, 3);
                for (int i = 0; i < SettingsItems.Length; i++)
                {
                    if ((GameModes)SettingsValue[0] == GameModes.Deathmatch && i == 3)
                        DrawSettingsParametr(i, SettingsItems[i], i == selectedIndex, windowWidth, SettingsValue[i], SettingsMaxValue[i], ConsoleColor.DarkGray);
                    else
                        DrawSettingsParametr(i, SettingsItems[i], i == selectedIndex, windowWidth, SettingsValue[i], SettingsMaxValue[i]);
                }
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow || key == ConsoleKey.W)
                {
                    selectedIndex--;
                    if (selectedIndex < 0)
                        selectedIndex = SettingsItems.Length - 1;
                }
                else if (key == ConsoleKey.DownArrow || key == ConsoleKey.S)
                {
                    selectedIndex++;
                    if (selectedIndex >= SettingsItems.Length)
                        selectedIndex = 0;
                }
                if (key == ConsoleKey.LeftArrow || key == ConsoleKey.A)
                {
                    if (SettingsValue[selectedIndex] > 0)
                        SettingsValue[selectedIndex]--;
                    ChangeSettingsValue(selectedIndex, SettingsValue[selectedIndex]);
                }
                else if (key == ConsoleKey.RightArrow || key == ConsoleKey.D)
                {
                    if (SettingsValue[selectedIndex] < SettingsMaxValue[selectedIndex])
                        SettingsValue[selectedIndex]++;
                    ChangeSettingsValue(selectedIndex, SettingsValue[selectedIndex]);
                }
                if ((GameModes)SettingsValue[0] == GameModes.Deathmatch) SettingsValue[3] = 1;
                if (key == ConsoleKey.R) SettingsToDefault();
                if (key == ConsoleKey.Escape)
                {
                    System.IO.File.WriteAllText("data.txt", $"{SettingsValue[0]};{SettingsValue[1]};{SettingsValue[2]}");
                    ServerSettingsValue = (int[])SettingsValue.Clone();
                    break;
                }
            }
        }

        private void DrawSettingsParametr(int index, string label, bool isSelected, int width, int value, int maxValue, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write('║');
            Console.ResetColor();
            string leftPadding = "   ";
            if (isSelected)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                leftPadding = " ➤ ";
            }
            else
                Console.ForegroundColor = color;
            int barWidth = maxValue + 1;
            string bar = new('═', barWidth);
            int pointerPosition = Math.Min(value, maxValue);
            bar = bar.Remove(pointerPosition, 1).Insert(pointerPosition, "█");
            string text = $"{label + ": " + GetParametrName(index, value),-30} [{bar}]";
            Console.Write(leftPadding + text.PadRight(width - 5));
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine('║');
            Console.ResetColor();
        }

        private void ChangeSettingsValue(int index, int value)
        {
            if (index == 2)
                MAX_CONNECTIONS = value + 1;
        }

        private string GetParametrName(int index, int value)
        {
            return index switch
            {
                0 => ((GameModes)value).ToString(),
                1 => Difficulties[value],
                2 => (value + 1).ToString(),
                3 => value == 1 ? "On" : "Off",
                _ => "null",
            };
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void SendOutcomingMessageInvoker(int packetID, byte[]? data = null)
        {
            Dispatcher.SendOutcomingMessage(packetID, ref Server, data);
        }

        private void PacketsThread()
        {
            new Thread(() =>
            {
                while (!StopedThread && !Exit)
                {
                    Server.PollEvents();
                    Dispatcher.SendOutcomingMessage(0, ref Server);
                    Thread.Sleep(10);
                }
            }).Start();
        }

        private static bool YesNoMessage()
        {
            const int windowWidth = 31;
            int selectedIndex = 0;
            string[] options = ["Yes", "No"];
            Console.Clear();
            ConsoleKey key;
            DrawBorder('╔', '╗', '═', windowWidth);
            WriteColoredCenteredText("Are you sure?", ConsoleColor.Yellow, windowWidth);
            DrawBorder('╟', '╢', '─', windowWidth);
            for (int i = 0; i < options.Length; i++) Console.WriteLine("");
            DrawBorder('╟', '╢', '─', windowWidth);
            WriteColoredCenteredText("↑↓: Move    Enter: Select", ConsoleColor.Green, windowWidth);
            DrawBorder('╚', '╝', '═', windowWidth);
            do
            {
                Console.SetCursorPosition(0, 3);
                for (int i = 0; i < options.Length; i++)
                    DrawButton(options[i], i == selectedIndex, windowWidth);
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow || key == ConsoleKey.W)
                {
                    selectedIndex--;
                    if (selectedIndex < 0)
                        selectedIndex = options.Length - 1;
                }
                else if (key == ConsoleKey.DownArrow || key == ConsoleKey.S)
                {
                    selectedIndex++;
                    if (selectedIndex >= options.Length)
                        selectedIndex = 0;
                }
                if (key == ConsoleKey.Escape) return false;
            } while (key != ConsoleKey.Enter);
            return selectedIndex == 0;
        }

        private static (int, bool) SelectPlayers(Dictionary<int, string> players)
        {
            const int windowWidth = 52;
            int selectedIndex = 0;
            ConsoleKey key;
            Console.Clear();
            DrawBorder('╔', '╗', '═', windowWidth);
            WriteColoredCenteredText("Lobby:", ConsoleColor.Yellow, windowWidth);
            DrawBorder('╟', '╢', '─', windowWidth);
            if (players.Count > 0)
            {
                for (int i = 0; i < players.Count; i++) Console.WriteLine("");
            }
            else
                WriteColoredCenteredText("Lobby is empty...", ConsoleColor.Red, windowWidth);
            DrawBorder('╟', '╢', '─', windowWidth);
            if (players.Count > 0)
                WriteColoredCenteredText("↑↓: Move    ESC: Exit    K: Kick    B: Ban", ConsoleColor.Green, windowWidth);
            else
                WriteColoredCenteredText("↑↓: Move    ESC: Exit", ConsoleColor.Green, windowWidth);
            DrawBorder('╚', '╝', '═', windowWidth);
            do
            {
                Console.SetCursorPosition(0, 3);
                if (players.Count > 0)
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        int id = players.Keys.ElementAt(i);
                        DrawButton($"ID: {id}, Name: {players[id]}", i == selectedIndex, windowWidth);
                    }
                }
                key = Console.ReadKey(true).Key;
                if (players.Count > 0)
                {
                    if (key == ConsoleKey.UpArrow || key == ConsoleKey.W)
                    {
                        selectedIndex--;
                        if (selectedIndex < 0)
                            selectedIndex = players.Count - 1;
                    }
                    else if (key == ConsoleKey.DownArrow || key == ConsoleKey.S)
                    {
                        selectedIndex++;
                        if (selectedIndex >= players.Count)
                            selectedIndex = 0;
                    }
                }
                if (key == ConsoleKey.Escape) return (-1, false);
            } while (key != ConsoleKey.K && key != ConsoleKey.B);
            if (players.Count > 0) return (selectedIndex, key == ConsoleKey.B);
            return (-1, false);
        }

        private static int SelectOption(string prompt, string[] options, string empty_string = "List is empty...", string button1 = "Cancel", string button2 = "Select")
        {
            const int windowWidth = 52;
            int selectedIndex = 0;
            ConsoleKey key;
            Console.Clear();
            DrawBorder('╔', '╗', '═', windowWidth);
            WriteColoredCenteredText(prompt, ConsoleColor.Yellow, windowWidth);
            DrawBorder('╟', '╢', '─', windowWidth);
            if (options.Length > 0)
            {
                for (int i = 0; i < options.Length; i++) Console.WriteLine("");
            }
            else
                WriteColoredCenteredText(empty_string, ConsoleColor.Red, windowWidth);
            DrawBorder('╟', '╢', '─', windowWidth);
            if (options.Length > 0)
                WriteColoredCenteredText($"↑↓: Move    ESC: {button1}    Enter: {button2}", ConsoleColor.Green, windowWidth);
            else
                WriteColoredCenteredText($"↑↓: Move  ESC: {button1}", ConsoleColor.Green, windowWidth);
            DrawBorder('╚', '╝', '═', windowWidth);
            do
            {
                Console.SetCursorPosition(0, 3);
                if (options.Length > 0)
                {
                    for (int i = 0; i < options.Length; i++)
                        DrawButton(options[i], i == selectedIndex, windowWidth);
                }
                key = Console.ReadKey(true).Key;
                if (options.Length == 0 && key == ConsoleKey.Enter) key = ConsoleKey.None;
                if (options.Length > 0)
                {
                    if (key == ConsoleKey.UpArrow || key == ConsoleKey.W)
                    {
                        selectedIndex--;
                        if (selectedIndex < 0)
                            selectedIndex = options.Length - 1;
                    }
                    else if (key == ConsoleKey.DownArrow || key == ConsoleKey.S)
                    {
                        selectedIndex++;
                        if (selectedIndex >= options.Length)
                            selectedIndex = 0;
                    }
                }
                if (key == ConsoleKey.Escape) return -1;
            } while (key != ConsoleKey.Enter);
            if (options.Length > 0) return selectedIndex;
            return -1;
        }

        private static string CenterText(string text, int width) => text.PadLeft((width - text.Length) / 2 + text.Length).PadRight(width);

        private static void DisplayTextMessage(string[] message)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (string line in message)
            {
                if (line.StartsWith('║'))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write('║');
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(line[1..^1]);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine('║');
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(line);
                }
            }
            Console.ResetColor();
            DrawBorder('╟', '╢', '─', message[0].Length);
            WriteColoredCenteredText("Press any key to return to the main menu...", ConsoleColor.DarkGray, message[0].Length);
            DrawBorder('╚', '╝', '═', message[0].Length);
            Console.ReadKey();
        }
    }
}