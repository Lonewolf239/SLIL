using LiteNetLib;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer
{
    internal class GameServerConsole
    {
        internal const string version = "|1.2.2.2|";
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
            int height = 32;
            Console.Title = $"GameServer for SLIL v{version.Trim('|')}";
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
        private readonly int[] SettingsValue, SettingsMaxValue;
        private int[] ServerSettingsValue;
        private readonly string[] SettingsItems, ServerSettingsItem;
        private int Selected = 0, SelectedDifficult = 1;
        private const int MaxCommandIndex = 10;
        private readonly List<string> BannedPlayersList;
        private GameModes GameMode = GameModes.Classic;
        private readonly string[] Difficulties = ["Easy", "Normal", "Hard", "Very hard"];
        private readonly ConsoleColor[] DifficultyColors = { ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Red, ConsoleColor.DarkRed };
        private readonly ConsoleColor[] GameModeColors = { ConsoleColor.White, ConsoleColor.Red, ConsoleColor.DarkGray, ConsoleColor.DarkGray };
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
                "Max Players"
            ];
            ServerSettingsItem =
            [
                "Game mode",
                "Difficulty",
                "Max Players"
            ];
            Listener = new();
            Server = new(Listener);
            Dispatcher = new();
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
                    SettingsValue = [Convert.ToInt32(data[0]), Convert.ToInt32(data[1]), Convert.ToInt32(data[2])];
                }
                else SettingsValue = [0, 1, 3];
            }
            catch { SettingsValue = [0, 1, 3]; }
            SettingsMaxValue = [3, 3, 7];
            if (SettingsValue[0] < 0 || SettingsValue[0] > SettingsMaxValue[0])
                SettingsValue[0] = 0;
            if (SettingsValue[1] < 0 || SettingsValue[1] > SettingsMaxValue[1])
                SettingsValue[1] = 1;
            if (SettingsValue[2] < 0 || SettingsValue[2] > SettingsMaxValue[2])
                SettingsValue[2] = 0;
            ServerSettingsValue = (int[])SettingsValue.Clone();
            GameMode = (GameModes)SettingsValue[0];
            Dispatcher.ChangeGameMode(GameMode);
            SelectedDifficult = SettingsValue[1];
            Dispatcher.ChangeDifficulty(SettingsValue[1]);
            MAX_CONNECTIONS = SettingsValue[2] + 1;
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
            DrawBorder('╠', '╣', '═', 52);
            WriteColoredCenteredText("Checking for updates...", ConsoleColor.Green, 52);
            DrawBorder('╚', '╝', '═', 52);
            Console.ResetColor();
            Thread.Sleep(1000);
            while (Console.KeyAvailable) Console.ReadKey(true);
            if (!CheckUpdate().Result) return;
            while (!Exit)
            {
                DisplayMainMenu();
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        Selected--;
                        if (Selected < 0)
                            Selected = MaxCommandIndex;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        Selected++;
                        if (Selected > MaxCommandIndex)
                            Selected = 0;
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
                        Selected = int.Parse(key.ToString().Trim('D')) - 1;
                        break;
                    case ConsoleKey.D0:
                        Selected = 9;
                        break;
                    case ConsoleKey.Enter:
                        ProcessingCommands(Selected);
                        break;
                    case ConsoleKey.Escape:
                        ProcessingCommands(10);
                        break;
                }
            }
        }

        private void DisplayMainMenu()
        {
            Console.CursorVisible = false;
            const int windowWidth = 52;
            string[] menuItems =
            [
                "1. Guide to connecting to the game",
                "2. Start the server",
                "3. Stop the server",
                "4. Players list",
                "5. Banned players list",
                "6. Set difficulty",
                "7. Select game mode",
                "8. Start the game",
                "9. Stop the game",
                "0. Settings",
                "ESC. Exit the program"
            ];
            Console.Clear();
            DrawBorder('╔', '╗', '═', windowWidth);
            WriteColoredCenteredText("Server Info", ConsoleColor.White, windowWidth);
            if (ServerStarted)
            {
                WriteColoredCenteredText("┌────────────────────────────┐", ConsoleColor.Cyan, windowWidth);
                WriteServerInfoLine("Status", "Online", ConsoleColor.Green, windowWidth);
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
            DrawBorder('╠', '╣', '═', windowWidth);
            for (int i = 0; i < menuItems.Length; i++)
                WriteMenuItem(menuItems[i], i == Selected, windowWidth);
            DrawBorder('╠', '╣', '═', windowWidth);
            WriteColoredCenteredText("↑↓: Move    ESC: Exit    Enter: Confirm", ConsoleColor.Green, windowWidth);
                if (ServerStarted && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                WriteColoredCenteredText("I: Copy IP    P: Copy Password", ConsoleColor.Green, windowWidth);
            DrawBorder('╠', '╣', '═', 52);
            WriteColoredCenteredText("Developed by: Fatalan & Lonewolf239", ConsoleColor.Yellow, 52);
            WriteColoredCenteredText("GUI designed by: Lonewolf239", ConsoleColor.Yellow, 52);
            DrawBorder('╚', '╝', '═', windowWidth);
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

        private static void WriteColoredCenteredText(string text, ConsoleColor color, int width)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write('║');
            Console.ForegroundColor = color;
            Console.Write(CenterText(text, width - 2));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine('║');
            Console.ResetColor();
        }

        private static void WriteMenuItem(string text, bool isSelected, int width)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write('║');
            Console.ResetColor();
            string leftPadding = "   ", rightPadding = "   ";
            if (isSelected)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                leftPadding = " → ";
                rightPadding = " ← ";
            }
            else
                Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(leftPadding + text.PadRight(width - 8) + rightPadding);
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
            switch (command)
            {
                case 0:
                    string[] howPlayText =
                    [
                        "╔════════════════════════════════════════════════════════════════╗",
                            "║                                                                ║",
                            "║   ATTENTION THIS IS NOT THE FINAL VERSION OF THE MULTIPLAYER   ║",
                            "║         IF YOU FIND ANY BUGS, PLEASE REPORT THEM TO US         ║",
                            "║                                                                ║",
                            "╠════════════════════════════════════════════════════════════════╣",
                            "║  How to play together:                                         ║",
                            "║                                                                ║",
                            "║   I do it later :)                                             ║",
                            "║                                                                ║",
                            "║                                                                ║",
                            "║                                                                ║",
                            "║                                                                ║",
                            "║                                                                ║",
                            "║                                                                ║",
                            "║                                                                ║",
                            "║                                                                ║",
                            "║                                                                ║",
                            "║                                                                ║",
                            "║                                                                ║",
                            "║                                                                ║",
                            "╚════════════════════════════════════════════════════════════════╝"
                    ];
                    DisplayTextMessage(howPlayText);
                    break;
                case 1:
                    const int windowWidth = 54;
                    Console.Clear();
                    Console.CursorVisible = true;
                    DrawBorder('╔', '╗', '═', windowWidth);
                    WriteColoredCenteredText("Server Configuration", ConsoleColor.Yellow, windowWidth);
                    DrawBorder('╠', '╣', '═', windowWidth);
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
                    DrawBorder('╠', '╣', '═', windowWidth);
                    for (int i = 0; i < ServerSettingsItem.Length; i++)
                        DrawSettingsParametr(i, ServerSettingsItem[i], false, windowWidth, ServerSettingsValue[i], SettingsMaxValue[i]);
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
                    if (ServerPassword.Length == 0) ServerPassword = "None";
                    Console.SetCursorPosition(42, 4);
                    Console.Write(ServerPassword);
                    int selectedServerIndex = 0;
                    ConsoleKey key;
                    while (true)
                    {
                        Console.SetCursorPosition(0, 5);
                        DrawBorder('╠', '╣', '═', windowWidth);
                        for (int i = 0; i < ServerSettingsItem.Length; i++)
                            DrawSettingsParametr(i, ServerSettingsItem[i], i == selectedServerIndex, windowWidth, ServerSettingsValue[i], SettingsMaxValue[i]);
                        DrawBorder('╠', '╣', '═', windowWidth);
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
                                StatusMessage = $"Server started successfully on port: {port}";
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
                case 2:
                    Server.Stop();
                    ServerStarted = false;
                    StopedThread = true;
                    StatusMessage = "Server stopped successfully";
                    break;
                case 3:
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
                            else DisplayMainMenu();
                        }
                        else DisplayMainMenu();
                    }
                    break;
                case 4:
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
                case 5:
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
                case 6:
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
                case 7:
                    if (!ServerStarted)
                        StatusMessage = "Please start the server first";
                    else
                    {
                        Dispatcher.StartGame();
                        StatusMessage = "New game started on the server";
                    }
                    break;
                case 8:
                    if (!ServerStarted)
                        StatusMessage = "Please start the server first";
                    else
                    {
                        Dispatcher.StopGame();
                        StatusMessage = "Game stopped on the server";
                    }
                    break;
                case 9:
                    if (!ServerStarted) Settings();
                    else StatusMessage = "Please stop the server first";
                    break;
                case 10:
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
            while (true)
            {
                Console.Clear();
                DrawBorder('╔', '╗', '═', windowWidth);
                WriteColoredCenteredText("Settings", ConsoleColor.Yellow, windowWidth);
                DrawBorder('╠', '╣', '═', windowWidth);
                for (int i = 0; i < SettingsItems.Length; i++)
                    DrawSettingsParametr(i, SettingsItems[i], selectedIndex == i, windowWidth, SettingsValue[i], SettingsMaxValue[i]);
                DrawBorder('╠', '╣', '═', windowWidth);
                WriteColoredCenteredText($"↑↓: Move    ←→: Change value    ESC: Exit", ConsoleColor.Green, windowWidth);
                DrawBorder('╚', '╝', '═', windowWidth);
                Console.ResetColor();
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
                if (key == ConsoleKey.Escape)
                {
                    System.IO.File.WriteAllText("data.txt", $"{SettingsValue[0]};{SettingsValue[1]};{SettingsValue[2]}");
                    ServerSettingsValue = (int[])SettingsValue.Clone();
                    break;
                }
            }
        }

        private void DrawSettingsParametr(int index, string label, bool isSelected, int width, int value, int maxValue)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write('║');
            Console.ResetColor();
            string leftPadding = "   ", rightPadding = "   ";
            if (isSelected)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                leftPadding = " → ";
                rightPadding = " ← ";
            }
            else
                Console.ForegroundColor = ConsoleColor.Gray;
            int barWidth = maxValue + 1;
            string bar = new('─', barWidth);
            int pointerPosition = Math.Min(value, maxValue);
            bar = bar.Remove(pointerPosition, 1).Insert(pointerPosition, "■");
            string text = $"{label + ": " + GetParametrName(index, value),-30} [{bar}]";
            Console.Write(leftPadding + text.PadRight(width - 8) + rightPadding);
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
            ConsoleKey key;
            do
            {
                Console.Clear();
                DrawBorder('╔', '╗', '═', windowWidth);
                WriteColoredCenteredText("Are you sure?", ConsoleColor.Yellow, windowWidth);
                DrawBorder('╠', '╣', '═', windowWidth);
                for (int i = 0; i < options.Length; i++)
                    WriteMenuItem(options[i], i == selectedIndex, windowWidth);
                DrawBorder('╠', '╣', '═', windowWidth);
                WriteColoredCenteredText("↑↓: Move    Enter: Select", ConsoleColor.Green, windowWidth);
                DrawBorder('╚', '╝', '═', windowWidth);
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
            do
            {
                Console.Clear();
                DrawBorder('╔', '╗', '═', windowWidth);
                WriteColoredCenteredText("Lobby:", ConsoleColor.Yellow, windowWidth);
                DrawBorder('╠', '╣', '═', windowWidth);
                if (players.Count > 0)
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        int id = players.Keys.ElementAt(i);
                        WriteMenuItem($"ID: {id}, Name: {players[id]}", i == selectedIndex, windowWidth);
                    }
                }
                else
                    WriteColoredCenteredText("Lobby is empty...", ConsoleColor.Red, windowWidth);
                DrawBorder('╠', '╣', '═', windowWidth);
                if (players.Count > 0)
                    WriteColoredCenteredText("↑↓: Move    ESC: Exit    K: Kick    B: Ban", ConsoleColor.Green, windowWidth);
                else
                    WriteColoredCenteredText("↑↓: Move    ESC: Exit", ConsoleColor.Green, windowWidth);
                DrawBorder('╚', '╝', '═', windowWidth);
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
            do
            {
                Console.Clear();
                DrawBorder('╔', '╗', '═', windowWidth);
                WriteColoredCenteredText(prompt, ConsoleColor.Yellow, windowWidth);
                DrawBorder('╠', '╣', '═', windowWidth);
                if (options.Length > 0)
                {
                    for (int i = 0; i < options.Length; i++)
                        WriteMenuItem(options[i], i == selectedIndex, windowWidth);
                }
                else
                    WriteColoredCenteredText(empty_string, ConsoleColor.Red, windowWidth);
                DrawBorder('╠', '╣', '═', windowWidth);
                if (options.Length > 0)
                    WriteColoredCenteredText($"↑↓: Move    ESC: {button1}    Enter: {button2}", ConsoleColor.Green, windowWidth);
                else
                    WriteColoredCenteredText($"↑↓: Move  ESC: {button1}", ConsoleColor.Green, windowWidth);
                DrawBorder('╚', '╝', '═', windowWidth);
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

        private void DisplayTextMessage(string[] message, bool do_split = false)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (string line in message)
            {
                if (line.StartsWith('║'))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write('║');
                    if (do_split)
                    {
                        int firstDividerIndex = line.IndexOf('║', 1);
                        int secondDividerIndex = line.IndexOf('║', firstDividerIndex + 1);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(line[1..firstDividerIndex]);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write('║');
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(line.Substring(firstDividerIndex + 1, secondDividerIndex - firstDividerIndex - 1));
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write('║');
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(line.Substring(secondDividerIndex + 1, line.Length - secondDividerIndex - 2));
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine('║');
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(line[1..^1]);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine('║');
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(line);
                }
            }
            Console.ResetColor();
            Console.Write("\nPress any key to return to the main menu...");
            Console.ReadKey();
            DisplayMainMenu();
        }
    }
}