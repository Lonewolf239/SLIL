using LiteNetLib.Utils;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace SLIL.Classes
{
    internal class GameController
    {
        private GameModel Game;
        public int playerID;
        private EventBasedNetListener listener;
        private NetManager client;
        private NetPacketProcessor processor;
        private NetPeer peer;
        public GameController()
        {
            Game = new GameModel();
        }
        public GameController(string adress, int port, StartGameDelegate startGame)
        {
            Game = new GameModel();
            listener = new EventBasedNetListener();
            client = new NetManager(listener);
            processor = new NetPacketProcessor();
            client.Start();
            client.Connect(adress, port, "SomeKey");
            Application.ApplicationExit += (sender, e) => client.Stop();
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
            {
                peer = fromPeer;
                int packetType = dataReader.GetInt();
                if (packetType == 100)
                {
                    playerID = dataReader.GetInt();
                    Game.Deserialize(dataReader);
                }
                if (packetType == 0)
                {
                    Game.Deserialize(dataReader);
                }
                dataReader.Recycle();
            };
            new Thread(() =>
            {
                while (true)
                {
                    client.PollEvents();
                    Thread.Sleep(15);
                }
            }).Start();
        }

        internal void MovePlayer(double dX, double dY)
        {
            Game.MovePlayer(dX, dY, playerID);
            NetDataWriter writer = new NetDataWriter();
            writer.Put(1);
            writer.Put(dX);
            writer.Put(dY);
            writer.Put(playerID);
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }
        internal void InitMap()
        {
            //Game.InitMap();
        }
        internal StringBuilder GetMap()
        {
            return Game.GetMap();
        }
        internal int GetMapWidth()
        {
            return Game.GetMapWidth();
        }
        internal int GetMapHeight()
        {
            return Game.GetMapHeight();
        }
        public List<Entity> GetEntities()
        {
            return Game.GetEntities();
        }
        public void AddPet(Player player, int index)
        {
            Game.AddPet(player, index);
        }
        public void AddPlayer()
        {
            playerID = Game.AddPlayer();
        }
        public Player GetPlayer()
        {
            List<Entity> Entities = Game.GetEntities();
            Player player = null;
            foreach (Entity ent in Entities)
            {
                if (ent is Player)
                {
                    if ((ent as Player).ID == playerID)
                    {
                        player = (Player)ent;
                    }
                }
            }
            return player;
        }
        public void StartGame()
        {
            Game.StartGame();
        }
        public bool DealDamage(Entity ent, double damage)
        {
            return Game.DealDamage(ent.ID, damage, playerID);
        }
        public Pet[] GetPets()
        {
            return Game.GetPets();
        }

        public void AddHittingTheWall(double X, double Y)
        {
            Game.AddHittingTheWall(X, Y);
        }
    }
}
