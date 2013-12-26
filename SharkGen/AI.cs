using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;

namespace SharkGen
{
    [Serializable]
    public class AI
    {
        bool shark;
        public bool processing = false;
        Point[][] possibleScores;
        char[,] spots;
        int scoringNumber;
        char[] scoreChars;
        gameMove m_bestMove;
        int difficulty;
        Random r = new Random();
        public AI(bool squid, LevelData data, int difficulty)
        {
            shark = !squid;
            this.difficulty = difficulty;
            processing = false;
            scoringNumber = data.rowLength;
            scoreChars = new char[data.chars];
            for (int i = 0; i < scoreChars.Length; i++)
            {
                scoreChars[i] = ChooseyBox.letters[i];
            }
        }
        public void startSearch(GameGrid grid)
        {
            List<Point[]> places = new List<Point[]>();
            possibleScores = grid.getPossibleScorings();
            spots = grid.getGrid();
            Thread processingThread = new Thread(new ThreadStart(continueSearch));
            processingThread.Start();
        }
        public gameMove getMove()
        {
            if (!processing)
                return m_bestMove;
            else
                return null;
        }
        private void continueSearch()
        {
            processing = true;
           // int bestScore = 0;
       //     if (shark)
       //         bestScore = Int32.MaxValue;
      //      else
       //         bestScore = Int32.MinValue;
           //gameMove[] bestMoves = new gameMove[5];
            List<gameMove> moves = new List<gameMove>();
           // gameMove bestMove = new gameMove(new Point(-1,-1),'x');
            for(int x = 0; x < spots.GetLength(0); x++)
                for (int y = 0; y < spots.GetLength(1); y++)
            {
                    if(spots[x,y] == ' ')
                        for (int i = 0; i < scoreChars.Length; i++)
                        {
                            gameMove move = new gameMove(new Point(x,y), scoreChars[i]);
                            move.getScore(spots,possibleScores);

                            moves.Add(move);
                          //  if((shark && score < bestScore) || (!shark && score > bestScore))
                          //  {
                         //       bestScore = score;
                        //        bestMove = move;
                       //     }
                }
            }
            int numMoves = moves.Count;
            moves.Sort();
            int pick = r.Next(10 - difficulty);
            if (10 - difficulty > moves.Count)
                pick = r.Next(moves.Count);
            
            
            if (shark)
                m_bestMove = moves[pick];
            else
                m_bestMove = moves[moves.Count - pick - 1];
          //  m_bestMove = bestMove;
            processing = false;
            
        }
        

    }
}
