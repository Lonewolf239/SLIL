using SLIL.Classes;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;
using System.Collections.Immutable;
using System.Net.WebSockets;

namespace GameServer
{
    public delegate void SendOutcomingMessageDelegate(int packetID);
    public delegate void SendMessageFromGameCallback(int packetID);
    internal class Dispatcher
    {
        private GameModel Game;
        public SendOutcomingMessageDelegate sendMessageDelegate;
        public SendMessageFromGameCallback sendMessageFromGameCallback;
        public Dispatcher()
        {
            sendMessageFromGameCallback = SendMessageFromGameHandle;
            Game = new GameModel(sendMessageFromGameCallback);
            Game.StartGame();
        }
        public void SendMessageFromGameHandle(int packetID)
        {
            sendMessageDelegate(packetID);
        }
        public void DispatchIncomingMessage(int packetID, byte[] data, ref NetManager server, int playerIDfromPeer)
        {
            NetDataReader dreader = new NetDataReader(data);
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
