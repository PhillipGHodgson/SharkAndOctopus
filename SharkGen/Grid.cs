using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SharkGen
{
    public class Grid
    {
        const int cellHeight = 20;
        const int cellWidth = 30;

        public LevelData data;
        Point loc;
        public Grid(LevelData d, Point location)
        {
            data = d;
            loc = location;
        }

        public void drawGrid(Graphics g)
        {
            Pen pen = new Pen(Color.Black);
            Brush brush = new SolidBrush(Color.Blue);
            int totalHeight = data.height * cellHeight;
            int totalWidth = data.width * cellWidth;
            g.FillRectangle(new SolidBrush(Color.White), loc.X, loc.Y, totalWidth, totalHeight);
            for (int i = 0; i <= data.width; i++)
            {
                g.DrawLine(pen, loc.X + i * cellWidth, loc.Y, loc.X + i * cellWidth, loc.Y + totalHeight);
            }
            for (int i = 0; i <= data.height; i++)
            {
                g.DrawLine(pen, loc.X, loc.Y + i * cellHeight, loc.X + totalWidth, loc.Y + i * cellHeight);
            }
            foreach (Point p in data.blocked)
            {
                if(p.X < data.width && p.Y < data.height)
                g.FillRectangle(brush, p.X * cellWidth + 1, p.Y * cellHeight + 1, cellWidth -1, cellHeight -1);
            }
        }
        public void toggleBlocks(Point p)
        {
            for (int i = 0; i < data.width; i++)
                for (int j = 0; j < data.height; j++)
                {
                    Rectangle r = new Rectangle(loc.X + cellWidth * i, loc.Y + cellHeight * j, cellWidth, cellHeight);
                    if (r.Contains(p))
                    {
                        for (int c = 0; c < data.blocked.Count; c++)
                        {
                            if (data.blocked[c].X == i && data.blocked[c].Y == j)
                            {
                                data.blocked.RemoveAt(c);
                                return;
                            }

                        }
                        data.blocked.Add(new Point(i, j));
                    }
                }
        }
    }
}
