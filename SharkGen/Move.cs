using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SharkGen
{
    [Serializable]
    public class gameMove : IComparable
    {
        public Point location;
        public char character;
        public int points;
        public gameMove(Point p, char c)
        {
            character = c;
            location = p;
        }
        public gameMove clone()
        {
            return new gameMove(this.location, this.character);
        }
        public int getScore(char[,] spots, Point[][] possibleScores)
        {
            char[,] spots2 = (char[,])spots.Clone();
            spots2[location.X, location.Y] = character;
            int cumlativeScore = 0;
            foreach (Point[] points in possibleScores)
                cumlativeScore += judgePossibleScore(spots2, points);

            this.points = cumlativeScore;
            return cumlativeScore;
        }
        private int judgePossibleScore(char[,] spots, Point[] possibleScores)
        {
            int score = 1;
            char c = spots[possibleScores[0].X, possibleScores[0].Y];
            for (int i = 0; i < possibleScores.Length; i++)
            {
                char cNext = spots[possibleScores[i].X, possibleScores[i].Y];
                if (cNext == ' ')
                    continue;
                else if (cNext == c)
                {
                    score++;
                }
                else if (cNext != c)
                {
                    if (c == ' ')
                    {
                        c = cNext;
                        score++;
                    }
                    else
                        return 0;
                }
            }
            return score;
        }
        public override string ToString()
        {
            return character + "" + location.X + "`" + location.Y;
        }
        public static gameMove parseMove(string dat)
        {
            char c = dat[0];
            string[] coord = dat.Substring(1).Split('`');
            Point p = new Point(Convert.ToInt32(coord[0]), Convert.ToInt32(coord[1]));
            return new gameMove(p, c);
        }

        public int CompareTo(object obj)
        {
            gameMove gm = (gameMove)obj;
            return this.points - gm.points;
        }
    }
}
