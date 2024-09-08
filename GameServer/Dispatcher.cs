using LiteNetLib.Utils;
using LiteNetLib;

namespace GameServer
{
    public delegate void SendOutcomingMessageDelegate(int packetID, byte[] data = null);
    public delegate void SendMessageFromGameCallback(int packetID, byte[] data = null);

    internal class Dispatcher
    {
        private readonly GameModel Game;
        public Dictionary<int, int> PeerPlayerIDs = [];
        public SendOutcomingMessageDelegate? sendMessageDelegate;
        public SendMessageFromGameCallback sendMessageFromGameCallback;

        public Dispatcher()
        {
            sendMessageFromGameCallback = SendMessageFromGameHandle;
            Game = new(sendMessageFromGameCallback);
            Game.StartGame();
        }

        public void SendMessageFromGameHandle(int packetID, byte[] data = null) => sendMessageDelegate?.Invoke(packetID, data);

        public void DispatchIncomingMessage(int packetID, byte[] data, ref NetManager server, int playerIDfromPeer)
        {
            NetDataReader dreader = new(data);
            switch (packetID)
            {
                case 0:
                    //Game.Deserialize(new NetDataReader(data));
                    break;
                case 1:
                    double newX = dreader.GetDouble();
                    double newY = dreader.GetDouble();
                    int playerID = dreader.GetInt();
                    //Console.WriteLine("Player ID = " + playerID.ToString() + "; X = " + newX.ToString() + "; Y = " + newY.ToString());
                    Game.MovePlayer(newX, newY, playerIDfromPeer);
                    //Console.WriteLine(Game.GetEntities().ToString());
                    //SendOutcomingMessage(0, ref server);
                    break;
                case 5:
                    int EntityID = dreader.GetInt();
                    double damage = dreader.GetDouble();
                    Game.DealDamage(EntityID, damage, playerIDfromPeer);
                    break;
                case 11:
                    Game.AddPet(playerIDfromPeer, dreader.GetInt());
                    break;
                case 33:
                    Game.AmmoCountDecrease(playerIDfromPeer);
                    break;
                case 34:
                    Game.Reload(playerIDfromPeer);
                    break;
                case 35:
                    Game.ChangeWeapon(playerIDfromPeer, dreader.GetInt());
                    break;
                case 36:
                    Game.BuyAmmo(playerIDfromPeer, dreader.GetInt());
                    break;
                case 37:
                    Game.BuyWeapon(playerIDfromPeer, dreader.GetInt());
                    break;
                case 38:
                    Game.UpdateWeapon(playerIDfromPeer, dreader.GetInt());
                    break;
                case 39:
                    Game.BuyConsumable(playerIDfromPeer, dreader.GetInt());
                    break;
                case 40:
                    Game.InteractingWithDoors(dreader.GetInt());
                    break;
                case 77:
                    Game.AddHittingTheWall(dreader.GetDouble(), dreader.GetDouble(), dreader.GetDouble());
                    break;
                case 89:
                    Game.SpawnRockets(dreader.GetDouble(), dreader.GetDouble(), dreader.GetInt(), dreader.GetDouble());
                    break;
                default:
                    break;
            }
        }
        public void SendOutcomingMessage(int packetID, ref NetManager server, byte[] data = null) 
        {
            NetDataWriter writer = new();
            writer.Put(packetID);
            switch (packetID)
            {
                case 0:
                    Game.Serialize(writer);
                    break;
                case 101:
                    break;
                case 102:
                    foreach(KeyValuePair<int, int> entry in PeerPlayerIDs)
                    {
                        NetDataWriter peerWriter = new();
                        peerWriter.Put(packetID);
                        SerializeGame(peerWriter);
                        server.GetPeerById(entry.Key).Send(peerWriter, DeliveryMethod.ReliableOrdered);
                    }
                    return;
                case 666:
                    List<int> deadPlayers = new List<int>();
                    foreach(Entity ent in Game.Entities)
                    {
                        if (ent is Player p) 
                        {
                            if(p.Dead)
                                deadPlayers.Add(p.ID);
                        }
                    }
                    foreach(KeyValuePair<int, int> entry in PeerPlayerIDs)
                    {
                        if (deadPlayers.Contains(entry.Value))
                        {
                            NetDataWriter peerWriter = new();
                            peerWriter.Put(packetID);
                            server.GetPeerById(entry.Key).Send(peerWriter, DeliveryMethod.ReliableOrdered);
                        }
                    }
                    return;
                case 1000:
                    NetDataReader reader = new NetDataReader(data);
                    int playerID = reader.GetInt();
                    int soundID = reader.GetInt();
                    foreach (KeyValuePair<int, int> entry in PeerPlayerIDs)
                    {
                        if (entry.Value == playerID)
                        {
                            NetDataWriter peerWriter = new();
                            peerWriter.Put(packetID);
                            peerWriter.Put(soundID);
                            server.GetPeerById(entry.Key).Send(peerWriter, DeliveryMethod.ReliableOrdered);
                            return;
                        }
                    }
                    break;
                default:
                    break;
            }
            server.SendToAll(writer, DeliveryMethod.ReliableOrdered);
        }
        public void SendOutcomingMessage(int packetID, ref NetPeer peer)
        {
            NetDataWriter writer = new();
            writer.Put(packetID);
            switch (packetID)
            {
                case 100:
                    int newPlayerId = AddPlayer();
                    PeerPlayerIDs.Add(peer.Id, newPlayerId);
                    writer.Put(newPlayerId);
                    SerializeGame(writer);
                    peer.Send(writer, DeliveryMethod.ReliableOrdered);
                    break;
                default:
                    break;
            }
        }

        public int AddPlayer() => Game.AddPlayer();

        public void RemovePlayer(int playerID) => Game.RemovePlayer(playerID);

        public void SerializeGame(NetDataWriter writer) => Game.Serialize(writer);
    }
}