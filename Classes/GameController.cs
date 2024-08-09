using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLIL.Classes
{
    internal class GameController
    {
        private GameModel Game;
        public int playerID;
        public GameController()
        {
            Game = new GameModel();
        }

        internal void MovePlayer(double dX, double dY)
        {
            Game.MovePlayer(dX, dY, playerID);
        }
    }
}
