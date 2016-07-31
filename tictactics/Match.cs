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

        public delegate void DrawCallback(Game game);
        public DrawCallback draw;

        int[][] setup = new int[2][];

        public Match(Player player1, Player player2)
        {
            players = new Player[] {player1,player2};
            players[0].playerid = 1;
            players[1].playerid = 2;

        }

        public void Setup()
        {
            game = new Game();

            setup[0] = players[0].getSetup();

            for (int i = 0; i < 9; i++)
            {
                game.setField(setup[0][i], i, 1);
            }

            System.Threading.Thread.Sleep(100);

            setup[1] = players[1].getSetup();

            for (int i = 0; i < 9; i++)
            {
                game.setField(setup[1][i], i, 2);
            }

            players[0].game = new Game(game);
            players[1].game = new Game(game);
        }


        public void Run()
        {
            Move m = null;

            while(!game.isFinished)
            {

                m = players[game.playerTurn - 1].getMove(m);

                game.makeMove(m);

                if (draw != null)
                    draw(game);

            }

        }

        public void Rematch()
        {
            game = new Game();

            for (int i = 0; i < 9; i++)
            {
                game.setField(setup[0][i], i, 2);
            }

            for (int i = 0; i < 9; i++)
            {
                game.setField(setup[1][i], i, 1);
            }

            game.playerTurn = 2;

            players[0].game = new Game(game);
            players[1].game = new Game(game);

            if (draw != null)
                draw(game);
            System.Threading.Thread.Sleep(200);

            Run();

        }

    }
}
