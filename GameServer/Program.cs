using SLIL.Classes;
using LiteNetLib;
using LiteNetLib.Utils;
using GameServer;
using System.Reflection;

NetPacketProcessor processor = new NetPacketProcessor();
//processor.RegisterNestedType<Player>(() => { return new Player(0,0,0,0); });
//processor.RegisterNestedType<GameModel>(() => { return new GameModel(null); });
EventBasedNetListener listener = new EventBasedNetListener();
NetManager server = new NetManager(listener);
Dictionary<int, int> peerPlayerIDs = new Dictionary<int, int>();
Dispatcher dispatcher = new Dispatcher();
//SendOutcomingMessageDelegate sendOutcomingMessageHandle;
//sendOutcomingMessageHandle = SendOutcomingMessageInvoker;
//dispatcher.sendMessageDelegate = sendOutcomingMessageHandle;
//server.UnsyncedEvents = true;
//server.UpdateTime = 1;
server.Start(9999 /* port */);

listener.ConnectionRequestEvent += request =>
{
    if (server.ConnectedPeersCount < 10 /* max connections */)
        request.AcceptIfKey("SomeKey");
    else
        request.Reject();
};

listener.PeerConnectedEvent += peer =>
{
    Console.WriteLine("We got connection: {0}", peer);
    NetDataWriter writer = new NetDataWriter();         
    writer.Put(100);
    int newPlayerId = dispatcher.AddPlayer();
    peerPlayerIDs.Add(peer.Id, newPlayerId);
    writer.Put(newPlayerId);
    dispatcher.SerializeGame(writer);
    peer.Send(writer, DeliveryMethod.ReliableOrdered);
};
listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
{
    Packet pack = new Packet();
    pack.Deserialize(dataReader);
    int playerIDFromPeer = peerPlayerIDs[fromPeer.Id];
    dispatcher.DispatchIncomingMessage(pack.PacketID, pack.Data, ref server, playerIDFromPeer);
};
listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
{
    dispatcher.RemovePlayer(peerPlayerIDs[peer.Id]);
    peerPlayerIDs.Remove(peer.Id);
    Console.WriteLine("Closed connection: {0}", peer);
};

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
    string command = Console.ReadLine();
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