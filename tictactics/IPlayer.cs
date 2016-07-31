using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactics
{
    public abstract class Player
    {
        public Game game;

        public int playerid;
        abstract public Move getMove(Move lastMove);
        abstract public int[] getSetup();

    }
}
