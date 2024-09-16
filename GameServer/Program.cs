using LiteNetLib;
using LiteNetLib.Utils;
using GameServer;

NetPacketProcessor processor = new();
EventBasedNetListener listener = new();
NetManager server = new(listener);
Dispatcher dispatcher = new();
SendOutcomingMessageDelegate sendOutcomingMessageHandle;
sendOutcomingMessageHandle = SendOutcomingMessageInvoker;
dispatcher.sendMessageDelegate = sendOutcomingMessageHandle;
//server.UnsyncedEvents = true;
//server.UpdateTime = 1;
server.Start(9999 /* port */);
    
listener.ConnectionRequestEvent += request =>
{
    if (server.ConnectedPeersCount < 4 /* max connections */)
        request.AcceptIfKey("SomeKey");
    else
        request.Reject();
};

listener.PeerConnectedEvent += peer =>
{
    Console.WriteLine($"We got connection: {peer}");
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
    Console.WriteLine($"Closed connection: {peer}");
};

void SendOutcomingMessageInvoker(int packetID, byte[] data = null)
{
   dispatcher.SendOutcomingMessage(packetID, ref server, data);
}

bool exit = false;

new Thread(() =>
{
    while (!exit)
    {
        server.PollEvents();
        dispatcher.SendOutcomingMessage(0, ref server);
        Thread.Sleep(10);
    }
}).Start();

while(!exit)
{
    Console.Write("Enter the command: ");
    string? command = Console.ReadLine();
    switch (command)
    {
        case "help":
            Console.WriteLine(
                             "┌──────────────────┬─────────────────────────────────────────────┐\n" +
                             "│ Command          │ Description                                 │\n" +
                             "├──────────────────┼─────────────────────────────────────────────┤\n" +
                             "│ stop             │ Stopping the server                         │\n" +
                             "│ stop_game        │ Stopping the game on the server             │\n" +
                             "│ start_game       │ The start of a new game on the server       │\n" +
                             "│ kick_{ID}        │ They kick the player with ID from server    │\n" +
                             "│ set_difficulty_  │ Setting the difficulty on the server        │\n" +
                             "│ set_game_mode_   │ Setting the game mode on the server         │\n" +
                             "└──────────────────┴─────────────────────────────────────────────┘\n");
            break;
        case "stop":
            server.Stop();
            exit = true;
            break;
        case "stop_game":
            dispatcher.StopGame();
            break;
        case "start_game":
            dispatcher.StartGame();
            break;
        case { } when command.StartsWith("kick_"):
            try { dispatcher.KickPlayer(int.Parse(command.Split('_')[1]), ref server); }
            catch(Exception e) { Console.WriteLine(e); }
            break;
        case { } when command.StartsWith("set_difficulty_"):
            try { dispatcher.ChangeDifficulty(int.Parse(command.Split('_')[2])); }
            catch(Exception e) { Console.WriteLine(e); }
            break;
        case { } when command.StartsWith("set_game_mode_"):
            try { dispatcher.ChangeGameMode((GameMode)int.Parse(command.Split('_')[3])); }
            catch(Exception e) { Console.WriteLine(e); }
            break;
        default:
            break;
    }
}