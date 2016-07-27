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
            int[] moves = players[0].getSetup();

            for (int i = 0; i < 9; i++)
            {
                game.setField(moves[i], i, 1);
            }

            System.Threading.Thread.Sleep(100);

            moves = players[1].getSetup();

            for (int i = 0; i < 9; i++)
            {
                game.setField(moves[i], i, 2);
            }
        }

        public delegate void DrawCallback();

        public void Run(DrawCallback draw)
        {
            Game g = new Game();
            g.Copy(game);

            while(!game.isFinished)
            {
                if (game.playerTurn == 1)
                    game.makeMove(game.GetRandomMove(game.playerTurn));
                else game.MakeAIMove();

                if (draw != null)
                    draw();

                System.Threading.Thread.Sleep(200);
            }

        }

    }
}
