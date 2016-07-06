using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactics
{
    class Match
    {
        Game game;

        IPlayer[] players;

        public Match(IPlayer player1, IPlayer player2)
        {
            game = new Game();
            players = new IPlayer[] {player1,player2};
        }

    }
}
