using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactics
{
    class Match
    {
        public Game game { get; private set; }

        Player[] players;

        public Match(Player player1, Player player2)
        {
            game = new Game();
            players = new Player[] {player1,player2};
        }

        public void Setup()
        {

        }

        public void Run()
        {

        }

    }
}
