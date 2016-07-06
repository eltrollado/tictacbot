using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactics

{
    public class Move
    {
       public Move(int grid, int field, int player) { g = grid; f = field; p = player; }
       public int g {get;set;}
       public int f {get;set;}

       public int p;

       public int prevSelect;
       public int prevBlocked;
       public bool wasFreeMove;

       public float value;
    }

    class Game
    {

        int playerTurn;

        public bool isFinished = false;
        public bool isFreeMove = true;

        public int[,] board {get; private set;}
        public int[] gridCounters { get; private set; }
        public int[] takenGrids { get; private set; }
        public int[] p1grids = new int[9];
        public int[] p2grids = new int[9];

        public int selectedGrid = -1;
        public int blocedField = -1;

        int moves = 0;


        public Game()
        {
            playerTurn = 1;
            board = new int[9,9];
            gridCounters = new int[9];
            takenGrids = new int[9];            
            
            ClearBoard();
        }


        public bool setField(int grid, int field, int player)
        {
            if (board[grid, field] == 0)
            {
                gridCounters[grid]++;
                if (player == 1)
                {
                    p1grids[grid]++;
                }
                else
                {
                    p2grids[grid]++;
                }
                
            }

            if (board[grid, field] == player)
            {
                gridCounters[grid]--;
                moves--;
                board[grid, field] = 0;
                if (player == 1)
                {
                    p1grids[grid]--;
                }
                else
                {
                    p2grids[grid]--;
                }
                return false;
            }

            board[grid, field] = player;

            takenGrids[grid] = CheckSmallBoard(grid);

            return true;
        }


        int[,] pw = { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };
        public int CheckSmallBoard(int grid)
        {
            int [] sb = new int[9];
            for (int i = 0; i < 9; i++)
			{
                sb[i] = board[grid, i] << 1;
                if (sb[i] == 0) sb[i] = 1;
			}

            bool possibleWin = false;

            for (int i = 0; i < 8; i++)
            {
                int outcome = sb[pw[i, 0]] | sb[pw[i, 2]] | sb[pw[i, 1]];

                if (outcome == 2 || outcome == 4)
                    return outcome >> 1;
                if (outcome == 5 || outcome == 3)
                    possibleWin = true;
            }

            if (possibleWin)
                return 0;
            else return 4;
        }

        int FastBoardCheck(int g, int f)
        {
            switch (f)
            {
                case 0: return (board[g, 0] & board[g, 1] & board[g, 2]) |
                    (board[g, 0] & board[g, 3] & board[g, 6]) |
                    (board[g, 0] & board[g, 4] & board[g, 8]);

                case 1: return (board[g, 0] & board[g, 1] & board[g, 2]) |
                    (board[g, 1] & board[g, 4] & board[g, 7]);

                case 2: return (board[g, 0] & board[g, 1] & board[g, 2]) |
                    (board[g, 2] & board[g, 5] & board[g, 8]) |
                    (board[g, 2] & board[g, 4] & board[g, 6]);

                case 3: return (board[g, 3] & board[g, 4] & board[g, 5]) |
                    (board[g, 0] & board[g, 3] & board[g, 6]);

                case 4: return (board[g, 3] & board[g, 4] & board[g, 5]) |
                    (board[g, 0] & board[g, 3] & board[g, 6]) |
                    (board[g, 1] & board[g, 4] & board[g, 7]) |
                    (board[g, 0] & board[g, 4] & board[g, 8]) |
                    (board[g, 2] & board[g, 4] & board[g, 6]);

                case 5: return (board[g, 3] & board[g, 4] & board[g, 5]) |
                    (board[g, 2] & board[g, 5] & board[g, 8]);

                case 6: return (board[g, 6] & board[g, 7] & board[g, 8]) |
                    (board[g, 0] & board[g, 3] & board[g, 6]) |
                    (board[g, 2] & board[g, 4] & board[g, 6]);

                case 7: return (board[g, 6] & board[g, 7] & board[g, 8]) |
                    (board[g, 1] & board[g, 4] & board[g, 7]);

                case 8: return (board[g, 6] & board[g, 7] & board[g, 8]) |
                    (board[g, 2] & board[g, 5] & board[g, 8]) |
                    (board[g, 0] & board[g, 4] & board[g, 8]);

                default: return 0;

            }
        }

        bool IsWinPossible(int g)
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[g, i] == 0)
                {

                }
            }

            return false;
        }


        public int CheckBigBoard()
        {
            int[] sb = new int[9];
            for (int i = 0; i < 9; i++)
            {
                sb[i] = takenGrids[i];
                if (sb[i] == 0) sb[i] = 8;
            }

            bool possibleWin = false;

            for (int i = 0; i < 8; i++)
            {
                int outcome = sb[pw[i, 0]] | sb[pw[i, 2]] | sb[pw[i, 1]];

                if (outcome == 2 || outcome == 6)
                    return 2;
                if (outcome == 1 || outcome == 5)
                    return 1;
                if (outcome == 9 || outcome == 10)
                    possibleWin = true;
            }

            if (possibleWin)
                return 0;
            else return 4;
        }

        float possibleLines(int player)
        {
            int[] possibleWin = {0,0};

            for (int i = 0; i < 8; i++)
            {
                int outcome = takenGrids[pw[i, 0]] | takenGrids[pw[i, 2]] | takenGrids[pw[i, 1]];


                if (outcome == 1)
                    possibleWin[0]++;
                    
                if(outcome == 2)
                    possibleWin[1]++;
            }

            if (player == 2)
                return possibleWin[1] * 0.03f - possibleWin[0] * 0.02f;
            else
                return possibleWin[0] * 0.03f - possibleWin[1] * 0.02f;

        }


        public void ClearBoard()
        {
            Array.Clear(board, 0, 81);
            Array.Clear(gridCounters, 0, 9);
            selectedGrid = -1;
            blocedField = -1;
        }



        public bool IsMoveLegal(int grid, int field, int player)
        {
            if (playerTurn != player)
                return false;
            if (grid != selectedGrid && !isFreeMove)
                return false;
            if (board[grid, field] != 0)
                return false;
            if (field == blocedField && gridCounters[grid] < 8)
                return false;

            return true;
        }


        public List<Move> GetLegalMoves(int player)
        {
            List<Move> list = new List<Move>();

            if (isFreeMove)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (IsMoveLegal(i, j, player))
                            list.Add(new Move(i, j, player));
                    }
                }
            }
            else
            {
                for (int i = 0; i < 9; i++)
                {
                    if (board[selectedGrid, i] != 0)
                        continue;
                    if (i == blocedField && gridCounters[selectedGrid] < 8)
                        continue;
                    list.Add(new Move(selectedGrid, i, player));
                }
            }


            return list;
        }


        public bool makeMove(int grid, int field, int player)
        {
            if (!IsMoveLegal(grid, field, player))
                return false;

            selectedGrid = field;
            blocedField = grid;
            board[grid, field] = player;

            gridCounters[grid]++;
            if (gridCounters[selectedGrid] > 8)
                isFreeMove = true;
            else isFreeMove = false;

            if (player == 1)
            {
                p1grids[grid]++;
                playerTurn = 2;
            }
            else
            {
                p2grids[grid]++;
                playerTurn = 1;
            }

            if (takenGrids[grid] == 0)
            {
                takenGrids[grid] = CheckSmallBoard(grid);
                int p = CheckBigBoard();
                FinishGame(p);
            }

            moves++;
                
            return true;
        }

        public bool tryMove(Move m)
        {
            int f = m.f;
            int g = m.g;

            m.prevBlocked = blocedField;
            m.prevSelect = selectedGrid;
            m.wasFreeMove = isFreeMove;

            selectedGrid = f;
            blocedField = g;
            board[g, f] = m.p;

            gridCounters[g]++;
            if (gridCounters[selectedGrid] > 8)
                isFreeMove = true;
            else isFreeMove = false;

            if (m.p == 1)
            {
                p1grids[g]++;
                playerTurn = 2;
            }
            else
            {
                p2grids[g]++;
                playerTurn = 1;
            }

            return true;
        }


        void UnmakeMove(Move m)
        {
            int g = m.g;

            gridCounters[g]--;
            board[g, m.f] = 0;

            selectedGrid = m.prevSelect;
            blocedField = m.prevBlocked;
            isFreeMove = m.wasFreeMove;

            if (m.p == 1)
            {
                p1grids[g]--;
                playerTurn = 1;
            }
            else
            {
                p2grids[g]--;
                playerTurn = 2;
            }

            if (takenGrids[g] != 0)
            {
                takenGrids[g] = CheckSmallBoard(g);
            }

        }


        private void FinishGame(int p)
        {
            isFinished = true;
        }


        public bool makeMove(Move m)
        {
            return makeMove(m.g, m.f, m.p);
        }


        Move FindBestMove(int player)
        {
            levels = 9;
            List<Move> possible = GetLegalMoves(player);
            Move best = possible[0];

            foreach (Move mov in possible)
            {
                Min(mov,1);
                if (mov.value > best.value)
                    best = mov;
            }

            float max = possible.Max(m => m.value);

            var choices = possible.Where(m => m.value == max);
            Random rnd = new Random(levels + possible.Count + player);

            best = choices.ElementAt(rnd.Next(0, choices.Count()));

            return best;
        }


        float Min(Move m, int level)
        {
            mins++;
            int g = m.g;
            tryMove(m);

            if (takenGrids[g] == 0 && gridCounters[g] > 2)
            {
                int newState;
                if (gridCounters[g] < 7)
                    newState = FastBoardCheck(g, m.f);
                else newState = CheckSmallBoard(g);
                takenGrids[g] = newState;

                if (newState != 0)
                {
                    int winner = CheckBigBoard();
                    if (winner == 2)
                    {
                        UnmakeMove(m);
                        m.value = 1;
                        wins++;
                        return 1;
                    }
                    else if (winner == 1)
                    {
                        UnmakeMove(m);
                        m.value = -1;
                        wins++;
                        return -1;
                    }
                    else if (winner == 4)
                    {
                        UnmakeMove(m);
                        m.value = 0;
                        wins++;
                        return 0;

                    }

                }
            }

            if (level < levels)
            {
                List<Move> possible = GetLegalMoves(playerTurn);

                if (possible.Count == 1)
                    level--;

                foreach (Move mov in possible)
                {
                    Max(mov, level + 1);
                }

                m.value = possible.Min(move => move.value);
            }
            else
                m.value = ScoreCurrentState();

            UnmakeMove(m);
            return 0;
        }

        float Max(Move m, int level)
        {
            maxes++;
            int g = m.g;
            tryMove(m);

            if (takenGrids[g] == 0 && gridCounters[g] > 2)
            {
                int newState;
                if (gridCounters[g] < 7)
                    newState = FastBoardCheck(g, m.f);
                else newState = CheckSmallBoard(g);
                takenGrids[g] = newState;

                if (newState != 0)
                {
                    int winner = CheckBigBoard();
                    if (winner == 2)
                    {
                        UnmakeMove(m);
                        m.value = 1;
                        wins++;
                        return 1;
                    }
                    else if (winner == 1)
                    {
                        UnmakeMove(m);
                        m.value = -1;
                        wins++;
                        return -1;
                    }
                    else if (winner == 4)
                    {
                        UnmakeMove(m);
                        m.value = 0;
                        wins++;
                        return 0;

                    }

                }
            }

            if (level < levels)
            {
                List<Move> possible = GetLegalMoves(playerTurn);

                if (possible.Count == 1)
                    level--;

                foreach (Move mov in possible)
                {
                    Min(mov, level + 1);
                }

                m.value = possible.Max(move => move.value);
            }
            else
                m.value = ScoreCurrentState();


            UnmakeMove(m);
            return 0;
        }

        float ScoreCurrentState()
        {
            float score = 0;
            for (int i = 0; i < 9; i++)
            {
                if (takenGrids[i] == 1)
                    score -= 0.1f;
                if (takenGrids[i] == 2)
                    score += 0.1f;
            }
            counter++;

            int balance = 0;

            for (int i = 0; i < 9; i++)
            {
                if(takenGrids[i] == 0)
                {
                    balance += p2grids[i];
                    balance -= p1grids[i];
                }
            }

            return score + balance*0.025f + possibleLines(2);
        }


        float AnalyzeMoves(Move m, int level)
        {            
            counter++;

            if (m != null)
            {
                int g = m.g;
                tryMove(m);

                if (takenGrids[g] == 0 && gridCounters[g] > 2)
                {
                    int newState = FastBoardCheck(g,m.f);
                    takenGrids[g] = newState;

                    if (newState != 0)
                    {
                        int winner = CheckBigBoard();
                        if (winner != 0)
                        {
                            UnmakeMove(m);
                            reverses++;
                            wins++;
                            return winner;
                        }
                            
                    }
                        

                }

            }

            if (level < 10)
            {
                List<Move> possible = GetLegalMoves(playerTurn);

                foreach(Move mov in possible)
                {
                    AnalyzeMoves(mov, level + 1);
                }
            }
          

            if (m != null)
            {
                UnmakeMove(m);
                reverses++;
            }

            return 0;
        }


        int fails = 0;
        int counter = 0;
        int reverses = 0;
        int wins = 0;
        int mins = 0;
        int maxes = 0;
        int levels = 7;
        int baseLevels = 7;

        public Move MakeAIMove()
        {
            counter = 0;
            Stopwatch stopwatch = Stopwatch.StartNew(); //creates and start the instance of Stopwatch
            //your sample code

            Move m = FindBestMove(2);

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);



            if (makeMove(m))
                return m;
            else return null;

        }

    }
}
