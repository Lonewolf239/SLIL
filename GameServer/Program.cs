using LiteNetLib;
using LiteNetLib.Utils;
using GameServer;

NetPacketProcessor processor = new();
//processor.RegisterNestedType<Player>(() => { return new Player(0,0,0,0); });
//processor.RegisterNestedType<GameModel>(() => { return new GameModel(null); });
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
    Console.WriteLine("We got connection: {0}", peer);
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
    Console.WriteLine("Closed connection: {0}", peer);
};

void SendOutcomingMessageInvoker(int packetID, byte[] data = null)
{
    if(packetID == 102)
    {
       dispatcher.SendOutcomingMessage(packetID, ref server, data);
    }
    else dispatcher.SendOutcomingMessage(packetID, ref server, data);
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
    string? command = Console.ReadLine();
    switch (command)
    {
        case "stop":
            server.Stop();
            exit = true;
            break;
        default:
            break;
    }
}