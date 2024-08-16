using LiteNetLib.Utils;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
using Play_Sound;

namespace SLIL.Classes
{
    public delegate void SetPlayerIDDelegate(int id);
    internal class GameController
    {
        private GameModel Game;
        public int playerID;
        private EventBasedNetListener listener;
        private NetManager client;
        private NetPacketProcessor processor;
        private NetPeer peer;
        StartGameDelegate StartGameHandle;
        InitPlayerDelegate InitPlayerHandle;
        StopGameDelegate StopGameHandle;
        PlaySoundDelegate PlaySoundHandle;
        SetPlayerIDDelegate SetPlayerID;
        public GameController(StartGameDelegate startGame, InitPlayerDelegate initPlayer, StopGameDelegate stopGame, PlaySoundDelegate playSound)
        {
            InitPlayerHandle = initPlayer;
            StartGameHandle = startGame;
            StopGameHandle = stopGame;
            PlaySoundHandle = playSound;
            SetPlayerID = SetPlayerIDInvoker;
            Game = new GameModel(StopGameHandle, SetPlayerID, PlaySoundHandle);
        }
        public GameController(string adress, int port, StartGameDelegate startGame, InitPlayerDelegate initPlayer, StopGameDelegate stopGame, PlaySoundDelegate playSound)
        {
            playerID = -1;
            StopGameHandle = stopGame;
            PlaySoundHandle = playSound;
            SetPlayerID = SetPlayerIDInvoker;
            Game = new GameModel(StopGameHandle, SetPlayerID, PlaySoundHandle);
            listener = new EventBasedNetListener();
            client = new NetManager(listener);
            //client.UnsyncedEvents = true;
            //client.UpdateTime = 1;
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
                    //initPlayer();
                    Game.StartGame();
                    startGame();
                }
                if (packetType == 0)
                {
                    if(playerID!=-1)
                        Game.Deserialize(dataReader, playerID);
                    else Game.Deserialize(dataReader);
                }
                dataReader.Recycle();
            };
            new Thread(() =>
            {
                while (true)
                {
                    if (GetPlayer() != null)
                    {
                        NetDataWriter writer = new NetDataWriter();
                        writer.Put(1);
                        writer.Put(GetPlayer().X);
                        writer.Put(GetPlayer().Y);
                        writer.Put(playerID);
                        peer.Send(writer, DeliveryMethod.Unreliable);
                    }
                    client.PollEvents();
                    Thread.Sleep(10);
                }
            }).Start();
        }

        public void CloseConnection()
        {
            if(client!=null) client.Stop();
        }

        ~GameController()
        {
            if (client != null) client.Stop();
        }
        internal void MovePlayer(double dX, double dY)
        {
            Game.MovePlayer(dX, dY, playerID);
            //if (peer != null)
            //{
            //    NetDataWriter writer = new NetDataWriter();
            //    writer.Put(1);
            //    writer.Put(dX);
            //    writer.Put(dY);
            //    writer.Put(playerID);
            //    peer.Send(writer, DeliveryMethod.Unreliable);
            //}
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

        public void SetPlayerIDInvoker(int id)
        {
            this.playerID = id;
        }

        public void AddPet(int index)
        {
            Game.AddPet(playerID, index);
        }
        public void AddPlayer()
        {
            playerID = Game.AddPlayer();
        }

        public (int, int) GetSecondsAndMinutes()
        {
            return Game.GetSecondsAndMinutes();
        }

        public Player GetPlayer()
        {
            Player player = null;
            List<Entity> Entities = Game.GetEntities();

            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] is Player)
                {
                    if ((Entities[i] as Player).ID == playerID)
                    {
                        player = (Player)Entities[i];
                    }
                }
            }
            return player;
        }
        public void StartGame()
        {
            if (GetPlayer() == null) playerID = Game.AddPlayer();
            Game.StartGame();
            InitPlayerHandle();
            StartGameHandle();
        }
        public void RestartGame()
        {
            if (!Game.IsGameStarted())
            {
                Game.StartGame();
                //playerID = Game.AddPlayer();
            }
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

        internal void ChangePlayerA(double v)
        {
            Game.ChangePlayerA(v, playerID);
        }

        internal void ChangePlayerLook(double lookDif)
        {
            Game.ChangePlayerLook(lookDif, playerID);
        }

        internal void StopGame(int win)
        {
            Game.StopGame(win);
        }
        public void SetCustom(bool custom, int CustomWidth, int CustomHeight, string CustomMap, int customX, int customY)
        {
            Game.SetCustom(custom, CustomWidth, CustomHeight, CustomMap, customX, customY);
        }
    }
}
