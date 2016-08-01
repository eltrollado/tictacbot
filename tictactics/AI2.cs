using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactics
{
    class AI2 : Player
    {
        float LineVal;
        int horizon;
        float gridVal;
        float markVal;

        public AI2(int levels = 8, float lineValue = 0.015f, float gridValue = 0.1f, float markValue = 0.03f )
        {
            this.horizon = levels;
            LineVal = lineValue;
            gridVal = gridValue;
            markVal = markValue;
        }

        override public Move getMove(Move lastMove)
        {
            if (lastMove != null)
                game.makeMove(lastMove);

            Move m = FindBestMove(playerid);
            game.makeMove(m);

            return m;
        }



        override public int[] getSetup()
        {
            int[] setup = new int[9];

            Random rnd  = new Random();

            int[] vector = new int[9];
            Array.Clear(vector, 0, 9);


            for (int i = 0; i < 9; i++)
            {
                int grid;
                do
                {
                    grid = (rnd.Next(0, 99) + rnd.Next(101)) % 9;
                } while (vector[grid] >= 2 || (i == 4 && grid == 4));
                 
                setup[i] = grid;
                vector[grid]++;
            }

            return setup;
        }
        int[,] pw = { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };

        float possibleLines(int player)
        {
            int[] possibleWin = { 0, 0 };            

            for (int i = 0; i < 8; i++)
            {
                int outcome = game.takenGrids[pw[i, 0]] | game.takenGrids[pw[i, 2]] | game.takenGrids[pw[i, 1]];


                if (outcome == 1)
                    possibleWin[0]++;

                if (outcome == 2)
                    possibleWin[1]++;
            }

            if (player == 2)
                return possibleWin[1] * LineVal - possibleWin[0] * LineVal;
            else
                return possibleWin[0] * LineVal - possibleWin[1] * LineVal;

        }

        public Move FindBestMove(int player)
        {
            int levels = horizon; // Math.Max(12, 12 + (moves - 30) / 4);
            //Output(String.Format("Scanning {0} levels", levels));

            List<Move> possible = game.GetLegalMoves(player);
            Move best;

            int i = 1;

            foreach (Move mov in possible)
            {
                //Output(String.Format("move {0} out of {1}", i, possible.Count));
                Console.WriteLine("move {0} out of {1}", i, possible.Count);
                float score = AlphaBetaMax(mov, levels, -100, 100, 1, player);

                ++i;
            }

            float max = possible.Max(m => m.value);
            var choices = possible.Where(m => m.value == max);

            if (choices.Count() > 1 && max < 1)
            {
                possible = choices.ToList();

                foreach (Move m in choices)
                {
                    EvaluatePosition(m, player);
                }

                max = possible.Max(m => m.value);
                choices = possible.Where(m => m.value == max);
            }

            Random rnd = new Random(levels + possible.Count + player);

            best = choices.ElementAt(rnd.Next(0, choices.Count()));

            //Output(String.Format("Best val: {0}", best.value));
            Console.WriteLine("Best val: {0}", best.value);
            return best;
        }

        void EvaluatePosition(Move m, int perspective)
        {
            game.tryMove(m);

            const float closeFactor = 0.1f;
            const float lineFactor = 0.01f;

            int[] closeWin = { 0, 0 };
            int[] blockedWin = { 0, 0 };
            int[] possibleLines = { 0, 0 };

            int[] sb = new int[9];
            for (int i = 0; i < 9; i++)
            {
                sb[i] = game.board[m.g, i];
            }

            for (int i = 0; i < 8; i++)
            {
                int[] c = new int[3];

                int outcome = sb[pw[i, 0]] | sb[pw[i, 2]] | sb[pw[i, 1]];

                if (outcome == 1 )
                {
                    possibleLines[0]++;
                    int sum = sb[pw[i, 0]] + sb[pw[i, 2]] + sb[pw[i, 1]];
                    if (sum == 2)
                        closeWin[0]++;
                }
                if (outcome == 2)
                {
                    possibleLines[1]++;
                    int sum = sb[pw[i, 0]] + sb[pw[i, 2]] + sb[pw[i, 1]];
                    if (sum == 4)
                        closeWin[1]++;
                }
            }

            float val = (closeWin[0] - closeWin[1]) * closeFactor + (possibleLines[0] - possibleLines[1]) * lineFactor;

            m.value = val;

            game.UnmakeMove(m);
        }



        float ScoreCurrentState(int player)
        {
            float score = 0;

            float side = player == 2 ? 1.0f : -1.0f;

            for (int i = 0; i < 9; i++)
            {
                if (game.takenGrids[i] == 1)
                    score -= gridVal;
                if (game.takenGrids[i] == 2)
                    score += gridVal;
            }

            int balance = 0;

            for (int i = 0; i < 9; i++)
            {
                if (game.takenGrids[i] == 0)
                {
                    balance += game.p2grids[i];
                    balance -= game.p1grids[i];
                }
            }

            return (score + balance * markVal) * side + possibleLines(player);
        }

        float AlphaBetaMin(Move m, int levels, float alpha, float beta, int depth, int perspective)
        {
            int g = m.g;
            game.tryMove(m);

            if (game.takenGrids[g] == 0 && game.gridCounters[g] > 2)
            {
                int newState;
                if (game.gridCounters[g] < 7)
                    newState = game.FastBoardCheck(g, m.f);
                else newState = game.CheckSmallBoard(g);
                game.takenGrids[g] = newState;

                if (newState != 0)
                {
                    int winner = game.CheckBigBoard();

                    if (winner == perspective)
                    {
                        game.UnmakeMove(m);
                        m.value = 1.0f + (1.0f / depth);
                        return m.value;
                    }
                    else if (winner != perspective && winner != 4 && winner != 0)
                    {
                        game.UnmakeMove(m);
                        m.value = -1.0f - (1.0f / depth);
                        return m.value;
                    }
                    else if (winner == 4)
                    {
                        game.UnmakeMove(m);
                        m.value = 0;
                        return 0;

                    }

                }
            }

            if (levels > 0)
            {
                List<Move> possible = game.GetLegalMoves(game.playerTurn);

                if (possible.Count == 1)
                    levels++;
                if (game.isFreeMove)
                    levels -= 2;

                foreach (Move mov in possible)
                {
                    float score = AlphaBetaMax(mov, levels - 1, alpha, beta, depth + 1, perspective);

                    if (score >= beta)
                    {
                        m.value = beta;
                        game.UnmakeMove(m);
                        return beta;
                    }

                    if (score > alpha)
                        alpha = score;

                }

                m.value = possible.Min(move => move.value);
            }
            else
            {
                m.value = ScoreCurrentState(perspective);
                game.UnmakeMove(m);
                return m.value;
            }

            game.UnmakeMove(m);
            m.value = alpha;
            return alpha;
        }

        float AlphaBetaMax(Move m, int levels, float alpha, float beta, int depth, int perspective)
        {
            int g = m.g;
            game.tryMove(m);

            if (game.takenGrids[g] == 0 && game.gridCounters[g] > 2)
            {
                int newState;
                if (game.gridCounters[g] < 7)
                    newState = game.FastBoardCheck(g, m.f);
                else newState = game.CheckSmallBoard(g);
                game.takenGrids[g] = newState;

                if (newState != 0)
                {
                    int winner = game.CheckBigBoard();
                    if (winner == perspective)
                    {
                        game.UnmakeMove(m);
                        m.value = 1.0f + (1.0f / depth);
                        return m.value;
                    }
                    else if (winner != perspective && winner != 4 && winner != 0)
                    {
                        game.UnmakeMove(m);
                        m.value = -1.0f - (1.0f / depth);
                        return m.value;
                    }
                    else if (winner == 4)
                    {
                        game.UnmakeMove(m);
                        m.value = 0;
                        return 0;

                    }

                }
            }

            if (levels > 0)
            {
                List<Move> possible = game.GetLegalMoves(game.playerTurn);

                if (possible.Count == 1)
                    levels++;
                if (game.isFreeMove)
                    levels -= 2;

                foreach (Move mov in possible)
                {
                    float score = AlphaBetaMin(mov, levels - 1, alpha, beta, depth + 1, perspective);

                    if (score <= alpha)
                    {
                        m.value = alpha;
                        game.UnmakeMove(m);
                        return alpha;
                    }

                    if (score < beta)
                        beta = score;
                }

                m.value = possible.Max(move => move.value);
            }
            else
            {
                m.value = ScoreCurrentState(perspective);
                game.UnmakeMove(m);
                return m.value;
            }



            game.UnmakeMove(m);
            m.value = beta;
            return beta;
        }
    }
}
