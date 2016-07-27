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

        public int playerTurn { get; private set; }
        public bool isFinished { get; private set; }
        public bool isFreeMove { get; private set; }

        public int[,] board {get; private set;}
        public int[] gridCounters { get; private set; }
        public int[] takenGrids { get; private set; }
        public int[] p1grids = new int[9];
        public int[] p2grids = new int[9];

        public int selectedGrid = -1;
        public int blocedField = -1;
        public int moves { get; private set; }

        public List<Move> history;
        int lastMoveId = -1;

        public delegate void OutputFunction(string output);
        public OutputFunction Output;

        


        public Game()
        {
            playerTurn = 1;
            board = new int[9,9];
            gridCounters = new int[9];
            takenGrids = new int[9];
            moves = 0;
            isFinished = false;
            isFreeMove = true;

            history = new List<Move>();
            
            ClearBoard();
        }

        public Game(Game template)
        {
            history = new List<Move>();
            lastMoveId = -1;

            Copy(template);
        }

        public void Copy(Game g)
        {
            Array.Copy(g.board, this.board, g.board.Length);
            Array.Copy(g.gridCounters, this.gridCounters, g.gridCounters.Length);
            Array.Copy(g.takenGrids, this.takenGrids, g.takenGrids.Length);
            Array.Copy(g.p1grids, this.p1grids, g.p1grids.Length);
            Array.Copy(g.p2grids, this.p2grids, g.p2grids.Length);

            this.isFinished = g.isFinished;
            this.isFreeMove = g.isFreeMove;
            this.playerTurn = g.playerTurn;

            this.selectedGrid = g.selectedGrid;
            this.blocedField = g.blocedField;
        }


        public bool setField(int grid, int field, int player)
        {
            if (board[grid, field] == 0)
            {
                moves++;
                gridCounters[grid]++;
                if (player == 1)
                {
                    p1grids[grid]++;
                }
                else
                {
                    p2grids[grid]++;
                }            
                
                board[grid, field] = player;

                if (takenGrids[grid] == 0)
                {
                    takenGrids[grid] = CheckSmallBoard(grid);
                }          

                return true;
                
            }

            else 
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
                if (takenGrids[grid] != 0)
                {
                    takenGrids[grid] = VerifyGrid(grid);
                }
                return false;
            }


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

        public int VerifyGrid(int grid)
        {
            int[] sb = new int[9];
            for (int i = 0; i < 9; i++)
            {
                sb[i] = board[grid, i];
                if (sb[i] == 0) sb[i] = 4;
            }

            bool possibleWin = false;

            int owner = takenGrids[grid];
            if (owner == 0)
                return 0;

            for (int i = 0; i < 8; i++)
            {
                int outcome = sb[pw[i, 0]] | sb[pw[i, 2]] | sb[pw[i, 1]];

                if (owner == outcome)
                    return owner;

                if (outcome == 5 || outcome == 6)
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
                if (outcome == 9 || outcome == 10 || outcome == 8)
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
                return possibleWin[1] * 0.015f - possibleWin[0] * 0.01f;
            else
                return possibleWin[0] * 0.015f - possibleWin[1] * 0.01f;

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

            Move m = new Move(grid, field, player);
            m.prevBlocked = blocedField;
            m.prevSelect = selectedGrid;
            m.wasFreeMove = isFreeMove;

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
                if (p != 0)
                    FinishGame(p);
            }

            moves++;
            lastMoveId++;

            if (lastMoveId != history.Count)
                history.RemoveRange(lastMoveId, history.Count - lastMoveId);
            history.Add(m);
                
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
                takenGrids[g] = VerifyGrid(g);
            }

        }

        public int Undo()
        {
            if(lastMoveId >= 0)
            {
                UnmakeMove(history.ElementAt(lastMoveId));
                lastMoveId--;
            }

            
            return selectedGrid;
            
        }

        public int Redo()
        {
            if (history.Count > lastMoveId +1)
            {
                Move m = history.ElementAt(lastMoveId + 1);
                tryMove(m);
                lastMoveId++;

                int grid = m.g;
                if (takenGrids[grid] == 0)
                {
                    takenGrids[grid] = CheckSmallBoard(grid);
                    int p = CheckBigBoard();
                    if (p != 0)
                        FinishGame(p);
                }
            }

            return selectedGrid;
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
            levels = 10; // Math.Max(12, 12 + (moves - 30) / 4);
            //Output(String.Format("Scanning {0} levels", levels));
                
            List<Move> possible = GetLegalMoves(player);
            Move best;

            int i = 1;

            foreach (Move mov in possible)
            {
                //Output(String.Format("move {0} out of {1}", i, possible.Count));
                Console.WriteLine("move {0} out of {1}",i, possible.Count);
                float score = AlphaBetaMax(mov,1,-100,100);

                ++i;
            }

            float max = possible.Max(m => m.value);

            var choices = possible.Where(m => m.value == max);
            Random rnd = new Random(levels + possible.Count + player);

            best = choices.ElementAt(rnd.Next(0, choices.Count()));

            //Output(String.Format("Best val: {0}", best.value));
            Console.WriteLine("Best val: {0}",best.value);
            return best;
        }

        public Move GetRandomMove(int player)
        {
            List<Move> possible = GetLegalMoves(player);
            Random rnd = new Random();

            return possible.ElementAt(rnd.Next(0, possible.Count()));

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

            return score + balance*0.03f + possibleLines(2);
        }

        float AlphaBetaMin(Move m, int level, float alpha, float beta)
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
                        m.value = 1.0f + (1.0f /level);
                        wins++;
                        return m.value;
                    }
                    else if (winner == 1)
                    {
                        UnmakeMove(m);
                        m.value = -1.0f -(1.0f /level);
                        wins++;
                        return m.value;
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
                if (isFreeMove)
                    level++;

                foreach (Move mov in possible)
                {
                    float score = AlphaBetaMax(mov, level + 1, alpha, beta);

                    if (score >= beta)
                    {
                        m.value = beta;
                        UnmakeMove(m);
                        return beta;
                    }

                    if (score > alpha)
                        alpha = score;

                }

                m.value = possible.Min(move => move.value);
            }
            else
            {
                m.value = ScoreCurrentState();
                UnmakeMove(m);
                return m.value;
            }

            UnmakeMove(m);
            m.value = alpha;
            return alpha;
        }

        float AlphaBetaMax(Move m, int level, float alpha, float beta)
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
                        m.value = 1.0f + (1.0f / level);
                        wins++;
                        return m.value;
                    }
                    else if (winner == 1)
                    {
                        UnmakeMove(m);
                        m.value = -1.0f - (1.0f / level);
                        wins++;
                        return m.value;
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
                if (isFreeMove)
                    level++;

                foreach (Move mov in possible)
                {
                    float score = AlphaBetaMin(mov, level + 1,alpha,beta);

                    if (score <= alpha)
                    {
                        m.value = alpha;
                        UnmakeMove(m);
                        return alpha;
                    }

                    if (score < beta)
                        beta = score;
                }

                m.value = possible.Max(move => move.value);
            }
            else
            {
                m.value = ScoreCurrentState();
                UnmakeMove(m);
                return m.value;
            }



            UnmakeMove(m);
            m.value = beta;
            return beta;
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
