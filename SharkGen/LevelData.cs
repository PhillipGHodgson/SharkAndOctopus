using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SharkGen
{
    [Serializable]
    public class LevelData 
    {
        public LevelData(int w, int h, int ch, int scoreLength, List<Point> b)
        {
            width = w;
            height = h;
            blocked = b;
            chars = ch;
            rowLength = scoreLength;
        }
        public void cleanup()
        {
            for (int i = 0; i < blocked.Count; i++)
            {
                if (blocked[i].X >= width || blocked[i].Y > height)
                {
                    blocked.RemoveAt(i);
                    i--;
                }
            }
        }
        public static LevelData randomLevel()
        {
            Random r = new Random();
            int width = r.Next(11, 20) / 3;
            int height = r.Next(11, 20) / 3;
            List<Point> blocks = new List<Point>();
            int blockNum = r.Next(3, width * height / 2) / 2;
            while (blockNum > 0)
            {
                blockNum--;
                blocks.Add(new Point(r.Next(width), r.Next(height)));
            }
            return new LevelData(width, height, 2, 3, blocks);
        }
        public override string ToString()
        {
            string blocks = "";
            for(int i = 0; i < blocked.Count; i++)
            {
                blocks += blocked[i].X + "`" + blocked[i].Y;
                if(i != blocked.Count - 1)
                    blocks += "~";
            }
            return width + "," + height + "," + chars + "," + rowLength + "," + blocks;
        }
        public static LevelData parseData(string input)
        {
            string[] parts = input.Split(',');
            int width = Convert.ToInt32(parts[0]);
            int height = Convert.ToInt32(parts[1]);
            int chars = Convert.ToInt32(parts[2]);
            int rowLength = Convert.ToInt32(parts[3]);
            List<Point> blocks = new List<Point>();
            string[] blockStrings = parts[4].Split('~');
            foreach (string s in blockStrings)
            {
                string[] coords = s.Split('`');
                blocks.Add(new Point(Convert.ToInt32(coords[0]), Convert.ToInt32(coords[1])));
            }
            return new LevelData(width,height,chars,rowLength,blocks);
        }
        public int width;
        public int height;
        public int chars;
        public int rowLength;
        public List<Point> blocked;
    }
}
