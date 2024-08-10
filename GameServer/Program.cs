using SLIL.Classes;
using LiteNetLib;
using LiteNetLib.Utils;
using GameServer;
int sus = 0;
NetPacketProcessor processor = new NetPacketProcessor();
processor.RegisterNestedType<Player>(() => { return new Player(0,0,0,ref sus); });
processor.RegisterNestedType<GameModel>(() => { return new GameModel(); });
EventBasedNetListener listener = new EventBasedNetListener();
NetManager server = new NetManager(listener);
Dictionary<int, int> peerPlayerIDs = new Dictionary<int, int>();
Dispatcher dispatcher = new Dispatcher();
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
    dispatcher.DispatchIncomingMessage(pack.PacketID, pack.Data, ref server);
    int packetType = dataReader.GetInt();
    NetDataWriter writer = new NetDataWriter();
};
listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
{
    dispatcher.RemovePlayer(peerPlayerIDs[peer.Id]);
    peerPlayerIDs.Remove(peer.Id);
};
while (true)
{
    server.PollEvents();
    Thread.Sleep(15);
}
server.Stop();