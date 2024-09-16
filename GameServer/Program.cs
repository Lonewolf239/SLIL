using LiteNetLib;
using LiteNetLib.Utils;
using System.Runtime.InteropServices;

namespace GameServer
{
    class GameServerProgramm
    {
        private static NetPacketProcessor processor = new();
        private static EventBasedNetListener listener = new();
        private static NetManager server = new(listener);
        private static Dispatcher dispatcher = new();
        private static SendOutcomingMessageDelegate? sendOutcomingMessageHandle;
        private static bool exit = false;
        private const int MAX_CONNECTIONS = 4;
        //server.UnsyncedEvents = true;
        //server.UpdateTime = 1;
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

        private static void SetupConsoleSettings()
        {
            int width = 80;
            int height = 25;
            Console.SetWindowSize(width, height);
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
            ConsoleMenu();
        }

        private static void SendOutcomingMessageInvoker(int packetID, byte[]? data = null)
        {
            if (data != null)
                dispatcher.SendOutcomingMessage(packetID, ref server, data);
        }

        private static void StartThread()
        {
            new Thread(() =>
            {
                while (!exit)
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
            Console.WriteLine('┌' + new string('─', windowWidth - 2) + '┐');
            Console.Write('│');
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(CenterText("Welcome to GameServer for SLIL", windowWidth - 2));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine('│');
            Console.Write('│');
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(CenterText("Version 1.2.2.2", windowWidth - 2));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine('│');
            Console.WriteLine('├' + new string('─', windowWidth - 2) + '┤');
            Console.Write('│');
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(CenterText("Developed by: Fatalan", windowWidth - 2));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine('│');
            Console.Write('│');
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(CenterText("GUI designed by: Lonewolf239", windowWidth - 2));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine('│');
            Console.WriteLine('├' + new string('─', windowWidth - 2) + '┤');
            if (message != "none")
            {
                Console.Write('│');
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(CenterText(message, windowWidth - 2));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine('│');
                Console.WriteLine('├' + new string('─', windowWidth - 2) + '┤');
            }
            Console.Write('│');
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(CenterText("Type \"help\" for a list of available commands", windowWidth - 2));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine('│');
            Console.WriteLine('└' + new string('─', windowWidth - 2) + '┘');
            Console.ResetColor();
            Console.Write("\nEnter the command: ");
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
                Console.WriteLine('║' + CenterText("↑↓: Move  Enter: Select", windowWidth - 2) + '║');
                Console.WriteLine('╚' + new string('═', windowWidth - 2) + '╝');
                Console.ResetColor();
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.UpArrow)
                    selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                    selectedIndex = (selectedIndex + 1) % options.Length;
            } while (keyInfo.Key != ConsoleKey.Enter);

            Console.Clear();
            return selectedIndex;
        }

        private static string CenterText(string text, int width) => text.PadLeft((width - text.Length) / 2 + text.Length).PadRight(width);

        private static void ConsoleMenu()
        {
            while (!exit)
            {
                string? command = Console.ReadLine()?.Replace(" ", null).ToLower();
                switch (command)
                {
                    case "help":
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        string[] helpText =
                        [
                            "┌──────────────────┬─────────────────────────────────────────────┐",
                            "│ Command          │ Description                                 │",
                            "├──────────────────┼─────────────────────────────────────────────┤",
                            "│ exit             │ Exit the program                            │",
                            "│ help             │ Display this help menu                      │",
                            "│ start            │ Start the server                            │",
                            "│ stop             │ Stop the server                             │",
                            "│ ip               │ Display server IP address                   │",
                            "│ kick_{ID}        │ Kick the player with ID from server         │",
                            "│ set_difficulty   │ Set the difficulty on the server            │",
                            "│ set_game_mode    │ Set the game mode on the server             │",
                            "│ start_game       │ Start a new game on the server              │",
                            "│ stop_game        │ Stop the game on the server                 │",
                            "└──────────────────┴─────────────────────────────────────────────┘"
                        ];
                        foreach (string line in helpText)
                        {
                            if (line.StartsWith('│'))
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.Write('│');
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(line.Substring(1, line.IndexOf('│', 1) - 1));
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.Write('│');
                                int rightStartIndex = line.IndexOf('│', 1) + 1;
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write(line.Substring(rightStartIndex, line.Length - rightStartIndex - 1));
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine('│');
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine(line);
                            }
                        }
                        Console.ResetColor();
                        Console.WriteLine("\nPress any key to return to the main menu...");
                        Console.ReadKey();
                        DisplayWelcomeText();
                        break;
                    case "start":
                        Console.Write("\nEnter port (0 for default): ");
                        int port;
                        try { port = Convert.ToInt32(Console.ReadLine()); }
                        catch { port = 9999; }
                        if (port < 1000 || port > 9999) port = 9999;
                        try
                        {
                            server.Start(port);
                            StartThread();
                            DisplayWelcomeText($"Server started successfully on port: {port}");
                        }
                        catch (Exception e)
                        {
                            DisplayWelcomeText($"Error when starting server: {e.Message}");
                        }
                        break;
                    case "stop":
                        server.Stop();
                        DisplayWelcomeText("Server stopped successfully");
                        break;
                    case "exit":
                        server.Stop();
                        exit = true;
                        break;
                    case "stop_game":
                        dispatcher.StopGame();
                        DisplayWelcomeText("Game stopped on the server");
                        break;
                    case "start_game":
                        dispatcher.StartGame();
                        DisplayWelcomeText("New game started on the server");
                        break;
                    case "ip":
                        DisplayWelcomeText($"Server IP address: {":)"}");
                        break;
                    case { } when command.StartsWith("kick_"):
                        try
                        {
                            int playerId = int.Parse(command.Split('_')[1]);
                            dispatcher.KickPlayer(playerId, ref server);
                            DisplayWelcomeText($"Player with ID {playerId} has been kicked from the server");
                        }
                        catch (Exception e) { DisplayWelcomeText($"Error kicking player: {e.Message}"); }
                        break;
                    case { } when command.StartsWith("set_difficulty"):
                        try
                        {
                            string[] difficulties = { "Easy", "Normal", "Hard", "Very hard" };
                            int selectedIndex = SelectOption("Select difficulty:", difficulties);
                            dispatcher.ChangeDifficulty(selectedIndex);
                            DisplayWelcomeText($"Difficulty set to {difficulties[selectedIndex]}");
                        }
                        catch (Exception e) { DisplayWelcomeText($"Error setting difficulty: {e.Message}"); }
                        break;
                    case { } when command.StartsWith("set_game_mode"):
                        try
                        {
                            string[] gameModes = Enum.GetNames(typeof(GameMode));
                            int selectedIndex = SelectOption("Select game mode:", gameModes);
                            GameMode selectedMode = (GameMode)selectedIndex;
                            dispatcher.ChangeGameMode(selectedMode);
                            DisplayWelcomeText($"Game mode set to {selectedMode}");
                        }
                        catch (Exception e) { DisplayWelcomeText($"Error setting game mode: {e.Message}"); }
                        break;
                    default:
                        DisplayWelcomeText("Unknown command. Type \"help\" for a list of available commands");
                        break;
                }
            }
        }
    }
}