using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SharkGen
{
    [Serializable]
    public class ChooseyBox
    {
        public static char[] letters = new char[] {'x','o','z','y','a','b','c','d','e','f','g','h','i','j','k','l','m','n','p','q','r','s','t','u','v','w'};
        const int cellHeight = 20;
        const int cellWidth = 30;
        Rectangle r;

        int height = cellHeight;
        int width = cellWidth;
        public int selected = 0;
        int size = 2;
        Point loc;
        Font font;

        public ChooseyBox(Point location, int siz, Font f)
        {
            size = siz;
            width = cellWidth * size;
            font = f;
            r = new Rectangle(loc.X, loc.Y, width, height);
        }
        public char getLetter()
        {
            return letters[selected];
        }


        public void draw(Graphics g)
        {
            Pen p = new Pen(Color.Black);
            Brush b = new SolidBrush(Color.Red);
            Brush b2 = new SolidBrush(Color.Blue);
            g.DrawRectangle(p, loc.X, loc.Y, width, height);
            g.FillRectangle(b2, loc.X + selected * cellWidth + 1, loc.Y + 1, cellWidth -1, cellHeight -1);
            for (int i = 0; i < size; i++)
            {
                if( i > 0)
                    g.DrawLine(p, loc.X + i * cellWidth, loc.Y,loc.X + i * cellWidth, loc.Y + cellHeight);
                g.DrawString(letters[i] + "", font, b, new PointF(loc.X + i * cellWidth + 6, loc.Y -3));
            }
            
        }
        public void clicked(Point p)
        {
            if (r.Contains(p))
            {
                selected = (int)((p.X - loc.X) / cellWidth);
            }
        }
        public int Length
        {
            get { return size; }
        }
    }
}
