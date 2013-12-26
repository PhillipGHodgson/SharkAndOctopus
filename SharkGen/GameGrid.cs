using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SharkGen
{
    [Serializable]
    public class GameGrid
    {
        const int cellHeight = 20;
        const int cellWidth = 30;

        int height = 3;
        int width = 3;

        char[,] spots;

        int scoringNumber;
        Font font;

        Point loc;
        gameMove lastMove;
        List<Point[]> scorings;
        List<Point[]> possibleScorings;
        public bool ready = true;
        public LevelData levelData;
        public bool playerControlled;
        public AI myAI;

        public GameGrid(LevelData d, Point location, Font f)
        {
            d.cleanup();
            playerControlled = true;
            font = f;
            height = d.height;
            width = d.width;
            scoringNumber = d.rowLength;
            spots = new char[width, height];
            foreach (Point p in d.blocked)
            {
                spots[p.X, p.Y] = '!';
            }
            reset();
            levelData = new LevelData(d.width, d.height, d.chars, d.rowLength, d.blocked.ToList());
            loc = location;
        }
        public char[,] getGrid()
        {
            return (char[,])spots.Clone();
        }
        public Point[][] getPossibleScorings()
        {
            Point[][] points = new Point[possibleScorings.Count][];
            possibleScorings.CopyTo(points);
            return points;
        }
        public Point Location
        {
            get { return loc; }
            set { loc = value; }
        }
        public int LengthToScore
        {
            get { return scoringNumber; }
        }
        public Point getLowerRight()
        {
            return new Point(width * cellWidth + loc.X, height * cellHeight + loc.Y);
        }
        public Point getUpperight()
        {
            return new Point(width * cellWidth + loc.X, loc.Y);
        }
        public PointF getTextLoc(bool above)
        {
            if(above)
                return new Point(loc.X + 20, loc.Y - 24);
            return new Point(loc.X + 20, height * cellHeight + loc.Y);
        }
        public void drawGrid(Graphics g)
        {
                Pen pen = new Pen(Color.Black);
            if(!ready || !playerControlled)
                pen = new Pen(Color.Gray);
            Pen linePen = new Pen(Color.Red);
            Pen linePen2 = new Pen(Color.Purple);
            Brush brush = new SolidBrush(Color.Blue);
            Brush newBrush = new SolidBrush(Color.Black);
            //Brush redBrush = new SolidBrush(Color.Red);
            int totalHeight = height * cellHeight;
            int totalWidth = width * cellWidth;
            g.FillRectangle(new SolidBrush(Color.White), loc.X, loc.Y, totalWidth, totalHeight);
            for (int i = 0; i <= width; i++)
            {
                g.DrawLine(pen, loc.X + i * cellWidth, loc.Y, loc.X + i * cellWidth, loc.Y + totalHeight);
            }
            for (int i = 0; i <= height; i++)
            {
                g.DrawLine(pen, loc.X, loc.Y + i * cellHeight, loc.X + totalWidth, loc.Y + i * cellHeight);
            }
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if (spots[i, j] == '!')
                        g.FillRectangle(brush, loc.X + i * cellWidth + 1, loc.Y + j * cellHeight + 1, cellWidth - 1, cellHeight - 1);
                    else
                    {
                        if(spots[i,j] != ' ')
                        {
                            if (i == lastMove.location.X && j == lastMove.location.Y)
                                g.DrawString(spots[i, j] + "", font, newBrush, new PointF(loc.X + i * cellWidth + 6, loc.Y + j * cellHeight - 3));
                            else
                                g.DrawString(spots[i,j] + "",font,brush,new PointF(loc.X +i*cellWidth + 6, loc.Y + j * cellHeight -3 ));
                        }
                    }
                }
            foreach (Point[] points in scorings)
            {
                g.DrawLine(linePen, loc.X + points[0].X * cellWidth + cellWidth / 2, loc.Y + points[0].Y * cellHeight + cellHeight / 2, loc.X + points[points.Length - 1].X * cellWidth + cellWidth / 2, loc.Y + points[points.Length - 1].Y * cellHeight + cellHeight / 2);
            }
            /*
            foreach (Point[] points in possibleScorings)
            {
                g.DrawLine(linePen2, loc.X + points[0].X * cellWidth + cellWidth / 2, loc.Y + points[0].Y * cellHeight + cellHeight / 2, loc.X + points[points.Length - 1].X * cellWidth + cellWidth / 2, loc.Y + points[points.Length - 1].Y * cellHeight + cellHeight / 2);
            }
             * */
        }
        void checkScoring()
        {
            scorings = new List<Point[]>();
            possibleScorings = new List<Point[]>();
            //checkScoringHorizontal();
            //checkScoringVertical();
            //checkScoringDiagonalDown();
            //checkScoringDiagonalUp();
            checkScoringInDir(1, 0);
            checkScoringInDir(0, 1);
            checkScoringInDir(1, 1);
            checkScoringInDir(1, -1);

        }
        void checkScoringInDir(int deltaX, int deltaY)
        {
            int yStart = 0;
            int xEnd = width - deltaX * (scoringNumber) + 1 * deltaX;
            int yEnd = height - deltaY * (scoringNumber) + 1 * deltaY;
            if (deltaY == -1)
            {
                yStart = scoringNumber - 1;
                yEnd = height;
            }
            Point[] points = new Point[scoringNumber];
            for (int i = 0; i < xEnd; i++)//horizontal
                for (int j = yStart; j < yEnd; j++)
                {
                    bool scoring = true;
                    char c = spots[i, j];
                    if (c == '!')
                        continue;
                    if (c == ' ')
                        scoring = false;

                    bool possibleScoring = true;
                    for (int x = 0; x < scoringNumber; x++)
                    {
                        int spotX = i + deltaX * x;
                        int spotY = j + deltaY * x;
                        if (spots[spotX, spotY] == c)
                        {
                            points[x] = new Point(spotX, spotY);
                        }
                        else
                        {
                            scoring = false;
                            if (c == ' ')
                            {
                                c = spots[spotX, spotY];
                                if (c == '!')
                                {
                                    possibleScoring = false;
                                    break;
                                }
                                points[x] = new Point(spotX, spotY);
                            }
                            else if (spots[spotX, spotY] == ' ')
                            {
                                points[x] = new Point(spotX, spotY);
                            }
                            else
                            {
                                possibleScoring = false;
                                break;
                            }
                        }
                    }
                    if (scoring)
                    {
                        scorings.Add((Point[])points.Clone());
                    }
                    else if (possibleScoring)
                        possibleScorings.Add((Point[])points.Clone());
                }
        }
        public int getScore()
        {
            return scorings.Count;
        }
        public void reset()
        {
            scorings = new List<Point[]>();
            possibleScorings = new List<Point[]>();

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if(spots[i, j] != '!')
                        spots[i, j] = ' ';
                }
            checkScoring();
        }

        public bool clickIt(Point click, char val)
        {
            if (!ready || !playerControlled)
                return false;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    Rectangle r = new Rectangle(loc.X + cellWidth * i, loc.Y + cellHeight * j, cellWidth, cellHeight);
                    if (r.Contains(click))
                    {
                        if (spots[i, j] == ' ')
                        {
                            spots[i, j] = val;
                            lastMove = new gameMove(new Point(i, j), val);
                            checkScoring();
                            return true;
                        }
                        return false;
                    }
                }
            return false;
        }
        public void makeMove(gameMove move)
        {
            spots[move.location.X, move.location.Y] = move.character;
            lastMove = move.clone();
            checkScoring(); 
        }
        public gameMove getLastMove()
        {
            return lastMove.clone();
        }
        public bool undo()
        {
            if (lastMove == null || spots[lastMove.location.X, lastMove.location.Y] == ' ')
                return false;
            spots[lastMove.location.X, lastMove.location.Y] = ' ';
            checkScoring();
            return true;
        }
        public bool filled()
        {
            foreach (char c in spots)
                if (c == ' ')
                    return false;
            return true;
        }
    }
}