using SLIL.Classes;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;
using System.Collections.Immutable;

namespace GameServer
{
    internal class Dispatcher
    {
        private GameModel Game;
        public Dispatcher()
        {
            Game = new GameModel();
        }
        public void DispatchIncomingMessage(int packetID, byte[] data, ref NetManager server)
        {
            switch (packetID)
            {
                case 0:
                    Game.Deserialize(new NetDataReader(data));
                    break;
                case 1:
                    NetDataReader dreader = new NetDataReader(data);
                    Game.MovePlayer(dreader.GetDouble(), dreader.GetDouble(), dreader.GetInt());
                    SendOutcomingMessage(0, ref server);
                    break;
                default:
                    break;
            }
        }
        public void SendOutcomingMessage(int packetID, ref NetManager server) 
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(packetID);
            switch (packetID)
            {
                case 0:
                    Game.Serialize(writer);
                    break;
                default:
                    break;
            }
            server.SendToAll(writer, DeliveryMethod.ReliableOrdered);
        }
        public int AddPlayer()
        {
            return Game.AddPlayer();
        }
        public void RemovePlayer(int playerID)
        {
            Game.RemovePlayer(playerID);
        }
        public void SerializeGame(NetDataWriter writer)
        {
            Game.Serialize(writer);
        }
    }
}
