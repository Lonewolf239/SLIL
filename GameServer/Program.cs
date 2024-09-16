using LiteNetLib;
using LiteNetLib.Utils;
using System.Runtime.InteropServices;

namespace GameServer
{
    class GameServerProgramm
    {
        private const int GWL_STYLE = -16;
        private const int WS_SIZEBOX = 0x00040000;
        private const int WS_MAXIMIZEBOX = 0x00010000;
        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;
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

        private static readonly NetPacketProcessor processor = new();
        private static readonly EventBasedNetListener listener = new();
        private static NetManager server = new(listener);
        private static readonly Dispatcher dispatcher = new();
        private static SendOutcomingMessageDelegate? sendOutcomingMessageHandle;
        private const string version = "1.2.2.2";
        private static bool exit = false, stoped_thread = true;
        private const int MAX_CONNECTIONS = 4;
        //server.UnsyncedEvents = true;
        //server.UpdateTime = 1;

        public static void Main()
        {
            sendOutcomingMessageHandle = SendOutcomingMessageInvoker;
            dispatcher.sendMessageDelegate = sendOutcomingMessageHandle;
            listener.ConnectionRequestEvent += request =>
            {
                if (server.ConnectedPeersCount < MAX_CONNECTIONS)
                    request.AcceptIfKey("SomeKey");
                else
                    request.Reject();
            };
            listener.PeerConnectedEvent += peer =>
            {
                DisplayWelcomeText($"We got connection: {peer}");
                dispatcher.SendOutcomingMessage(100, ref peer);
            };
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
            {
                Packet pack = new();
                pack.Deserialize(dataReader);
                int playerIDFromPeer = dispatcher.PeerPlayerIDs[fromPeer.Id];
                dispatcher.DispatchIncomingMessage(pack.PacketID, pack.Data, ref server, playerIDFromPeer);
            };
            listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
            {
                dispatcher.RemovePlayer(dispatcher.PeerPlayerIDs[peer.Id]);
                dispatcher.PeerPlayerIDs.Remove(peer.Id);
                DisplayWelcomeText($"Closed connection: {peer}");
            };
            SetupConsoleSettings();
            DisplayWelcomeText();
            while (!exit)
            {
                string? command = Console.ReadLine()?.Replace(" ", null).ToLower();
                switch (command)
                {
                    case "0":
                    case "help":
                        string[] helpText =
                        [
                            "╔═════╦══════════════════╦═════════════════════════════════════════════╗",
                            "║ ID  ║ Command          ║ Description                                 ║",
                            "╠═════╬══════════════════╬═════════════════════════════════════════════╣",
                            "║ 0   ║ help             ║ Display this help menu                      ║",
                            "║ 1   ║ how_play         ║ Explanation of how to play together         ║",
                            "╠═════╬══════════════════╬═════════════════════════════════════════════╣",
                            "║ 2   ║ start            ║ Start the server                            ║",
                            "║ 3   ║ stop             ║ Stop the server                             ║",
                            "║ 4   ║ exit             ║ Exit the program                            ║",
                            "╠═════╬══════════════════╬═════════════════════════════════════════════╣",
                            "║ 5   ║ ip               ║ Display server IP address                   ║",
                            "║ 6   ║ kick             ║ Kick the player from server                 ║",
                            "╠═════╬══════════════════╬═════════════════════════════════════════════╣",
                            "║ 7   ║ set_difficulty   ║ Set the difficulty on the server            ║",
                            "║ 8   ║ set_game_mode    ║ Set the game mode on the server             ║",
                            "╠═════╬══════════════════╬═════════════════════════════════════════════╣",
                            "║ 9   ║ start_game       ║ Start a new game on the server              ║",
                            "║ 10  ║ stop_game        ║ Stop the game on the server                 ║",
                            "╚═════╩══════════════════╩═════════════════════════════════════════════╝"
                        ];
                        DisplayTextMessage(helpText, true);
                        break;
                    case "1":
                    case "how_play":
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
                    case "2":
                    case "start":
                        Console.Write("\nEnter port (1000-9999, 0 to set default): ");
                        int port;
                        try { port = Convert.ToInt32(Console.ReadLine()); }
                        catch { port = 9999; }
                        if (port < 1000 || port > 9999) port = 9999;
                        try
                        {
                            server.Start(port);
                            stoped_thread = false;
                            PacketsThread();
                            DisplayWelcomeText($"Server started successfully on port: {port}");
                        }
                        catch (Exception e)
                        {
                            DisplayWelcomeText($"Error when starting server: {e.Message}");
                        }
                        break;
                    case "3":
                    case "stop":
                        server.Stop();
                        stoped_thread = true;
                        DisplayWelcomeText("Server stopped successfully");
                        break;
                    case "4":
                    case "exit":
                        server.Stop();
                        exit = true;
                        break;
                    case "5":
                    case "ip":
                        DisplayWelcomeText($"Server IP address: {"127.0.0.1:9999"}");
                        break;
                    case "6":
                    case "kick":
                        try
                        {
                            string[] playerStrings = dispatcher.GetPlayers().Select(p => p.Value.ToString()).ToArray();
                            int playerId = SelectOption("Select player to kick from server:", playerStrings);
                            if(playerId == -1)
                            {
                                DisplayWelcomeText("The operation was rejected");
                                continue;
                            }
                            dispatcher.KickPlayer(playerId, ref server);
                            DisplayWelcomeText($"Player with ID {playerId} has been kicked from the server");
                        }
                        catch (Exception e) { DisplayWelcomeText($"Error kicking player: {e.Message}"); }
                        break;
                    case "7":
                    case "set_difficulty":
                        try
                        {
                            string[] difficulties = ["Easy", "Normal", "Hard", "Very hard"];
                            int selectedIndex = SelectOption("Select difficulty:", difficulties);
                            if(selectedIndex == -1)
                            {
                                DisplayWelcomeText("The difficulty change has been cancelled.");
                                continue;
                            }
                            dispatcher.ChangeDifficulty(selectedIndex);
                            DisplayWelcomeText($"Difficulty set to {difficulties[selectedIndex]}");
                        }
                        catch (Exception e) { DisplayWelcomeText($"Error setting difficulty: {e.Message}"); }
                        break;
                    case "8":
                    case "set_game_mode":
                        try
                        {
                            string[] gameModes = Enum.GetNames(typeof(GameMode));
                            int selectedIndex = SelectOption("Select game mode:", gameModes);
                            if (selectedIndex == -1)
                            {
                                DisplayWelcomeText("The game mode change has been cancelled.");
                                continue;
                            }
                            GameMode selectedMode = (GameMode)selectedIndex;
                            dispatcher.ChangeGameMode(selectedMode);
                            DisplayWelcomeText($"Game mode set to {selectedMode}");
                        }
                        catch (Exception e) { DisplayWelcomeText($"Error setting game mode: {e.Message}"); }
                        break;
                    case "9":
                    case "start_game":
                        dispatcher.StartGame();
                        DisplayWelcomeText("New game started on the server");
                        break;
                    case "10":
                    case "stop_game":
                        dispatcher.StopGame();
                        DisplayWelcomeText("Game stopped on the server");
                        break;
                    default:
                        DisplayWelcomeText("Unknown command. Type \"help\" for a list of available commands");
                        break;
                }
            }
        }

        private static void SetupConsoleSettings()
        {
            int width = 80;
            int height = 25;
            Console.Title = $"GameServer for SLIL v{version}";
            Console.SetWindowSize(width, height);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Console.SetBufferSize(width, height);
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
            }
        }

        private static void SendOutcomingMessageInvoker(int packetID, byte[]? data = null)
        {
            dispatcher.SendOutcomingMessage(packetID, ref server, data);
        }

        private static void PacketsThread()
        {
            new Thread(() =>
            {
                while (!stoped_thread && !exit)
                {
                    server.PollEvents();
                    dispatcher.SendOutcomingMessage(0, ref server);
                    Thread.Sleep(10);
                }
            }).Start();
        }

        private static void DisplayWelcomeText(string message = "none")
        {
            const int windowWidth = 70;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine('╔' + new string('═', windowWidth - 2) + '╗');
            Console.Write('║');
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(CenterText("Welcome to GameServer for SLIL", windowWidth - 2));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine('║');
            Console.Write('║');
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(CenterText($"Version {version}", windowWidth - 2));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine('║');
            Console.WriteLine('╠' + new string('═', windowWidth - 2) + '╣');
            Console.Write('║');
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(CenterText("Developed by: Fatalan", windowWidth - 2));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine('║');
            Console.Write('║');
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(CenterText("GUI designed by: Lonewolf239", windowWidth - 2));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine('║');
            Console.WriteLine('╠' + new string('═', windowWidth - 2) + '╣');
            if (message != "none")
            {
                Console.Write('║');
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(CenterText(message, windowWidth - 2));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine('║');
                Console.WriteLine('╠' + new string('═', windowWidth - 2) + '╣');
            }
            Console.Write('║');
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(CenterText("Type \"help\" for a list of available commands", windowWidth - 2));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine('║');
            Console.WriteLine('╚' + new string('═', windowWidth - 2) + '╝');
            Console.ResetColor();
            Console.Write("\nEnter the command or command ID: ");
        }

        private static int SelectOption(string prompt, string[] options)
        {
            const int windowWidth = 40;
            int selectedIndex = 0;
            ConsoleKeyInfo keyInfo;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine('╔' + new string('═', windowWidth - 2) + '╗');
                Console.WriteLine('║' + CenterText(prompt, windowWidth - 2) + '║');
                Console.WriteLine('╠' + new string('═', windowWidth - 2) + '╣');
                for (int i = 0; i < options.Length; i++)
                {
                    Console.Write("║ ");
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write("> " + options[i].PadRight(windowWidth - 6));
                        Console.ResetColor();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else
                        Console.Write("  " + options[i].PadRight(windowWidth - 6));
                    Console.WriteLine(" ║");
                }
                Console.WriteLine('╠' + new string('═', windowWidth - 2) + '╣');
                Console.WriteLine('║' + CenterText("↑↓: Move  ESC: Cancel  Enter: Select", windowWidth - 2) + '║');
                Console.WriteLine('╚' + new string('═', windowWidth - 2) + '╝');
                Console.ResetColor();
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.UpArrow)
                    selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                    selectedIndex = (selectedIndex + 1) % options.Length;
                if (keyInfo.Key == ConsoleKey.Escape) return -1;
            } while (keyInfo.Key != ConsoleKey.Enter);
            Console.Clear();
            return selectedIndex;
        }

        private static string CenterText(string text, int width) => text.PadLeft((width - text.Length) / 2 + text.Length).PadRight(width);

        private static void DisplayTextMessage(string[] message, bool do_split = false)
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
            DisplayWelcomeText();
        }
    }
}