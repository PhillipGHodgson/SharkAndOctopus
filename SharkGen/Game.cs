using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace SharkGen
{
    public enum gameType { Local, VsAI, Online }
    [Serializable]
    public class Game
    {
        GameGrid grid1, grid2;
        ChooseyBox choose;
        Font font1;
        bool grid1SquidTurn = true;
        public LevelData levelData;
        public gameType theGameType;
        bool gameOver = false;
        GameWindow window;
        bool moveMade = false;
        public Game(LevelData data, Font font1, gameType thisGameType, GameWindow wind)
        {
            window = wind;
            theGameType = thisGameType;
            startGame(data, font1);
            levelData = data;
            gameOver = false;
            
        }
        public void reset()
        {
            
            startGame(levelData, font1);
        }
        public void draw(Graphics g)
        {
            grid1.drawGrid(g);
            grid2.drawGrid(g);
            choose.draw(g);
            if (gameOver)
            {
                int p1Score = grid1.getScore();
                int p2Score = grid2.getScore();
                g.DrawString(p1Score + " pts", font1, new SolidBrush(Color.LightSeaGreen), grid1.getTextLoc(false));
                g.DrawString(p2Score + " pts", font1, new SolidBrush(Color.LightSalmon), grid2.getTextLoc(true));
                if (theGameType == gameType.Local)
                {
                    if (p1Score > p2Score)
                        g.DrawString("Player 1 wins!", font1, new SolidBrush(Color.Blue), grid1.getUpperight());
                    else if (p2Score > p1Score)
                        g.DrawString("Player 2 wins!", font1, new SolidBrush(Color.Blue), grid1.getUpperight());
                    else
                        g.DrawString("It's a draw!", font1, new SolidBrush(Color.Blue), grid1.getUpperight());
                }
                else
                {
                    if (p1Score > p2Score)
                        g.DrawString("You have won!", font1, new SolidBrush(Color.Blue), grid1.getUpperight());
                    else if (p2Score > p1Score)
                        g.DrawString("You have lost!", font1, new SolidBrush(Color.Blue), grid1.getUpperight());
                    else
                        g.DrawString("It's a draw!", font1, new SolidBrush(Color.Blue), grid1.getUpperight());
                }
            }
            else
            {
                if (!grid1SquidTurn)
                {
                    g.DrawString("Shark Turn", font1, new SolidBrush(Color.Blue), grid1.getUpperight());
                    g.DrawString("p2", font1, new SolidBrush(Color.LightSeaGreen), grid1.getTextLoc(false));
                    g.DrawString("p1", font1, new SolidBrush(Color.LightSalmon), grid2.getTextLoc(true));
                }
                else
                {
                    g.DrawString("Octopus Turn", font1, new SolidBrush(Color.Red), grid1.getUpperight());
                    g.DrawString("p2", font1, new SolidBrush(Color.LightSeaGreen), grid2.getTextLoc(true));
                    g.DrawString("p1", font1, new SolidBrush(Color.LightSalmon), grid1.getTextLoc(false));
                }
            }
        }
        void switchTurn()
        {
            if (!grid2.ready && !grid1.ready)
            {
                if (grid1.filled())
                {
                    gameOver = true;
                    return;
                }
                grid1SquidTurn = !grid1SquidTurn;
                grid1.ready = true;
                grid2.ready = true;
                if (theGameType == gameType.VsAI || theGameType == gameType.Online)
                {
                    if (grid1SquidTurn)
                    {
                        grid1.playerControlled = true;
                        grid2.playerControlled = false;
                    }
                    else
                    {
                        grid1.playerControlled = false;
                        grid2.playerControlled = true;
                    }
                    if (theGameType == gameType.VsAI)
                    {
                        if (grid1SquidTurn)
                        {
                            grid2.myAI.startSearch(grid2);
                        }
                        else
                        {
                            grid1.myAI.startSearch(grid1);
                        }
                        //window.startTimer();
                    }
                }
                
            }
            
            /*
            if (sharkTurn)
                label1.Text = "Shark's turn";
            else
                label1.Text = "Squid's turn";
             * */
        }
        void startGame(LevelData data, Font font)
        {
            gameOver = false;
            font1 = font;
            data.cleanup();
            grid1 = new GameGrid(data, new Point(0, 50),font1);
            grid2 = new GameGrid(data, grid1.getLowerRight(), font1);//new Font(Font.FontFamily, 14));
            grid1SquidTurn = true;
            if (theGameType == gameType.VsAI)
            {
                grid1.myAI = new AI(false, data,0);
                grid2.myAI = new AI(true, data,0);
            }
            choose = new ChooseyBox(new Point(0, 0), data.chars, font1);
            if (theGameType == gameType.Online || theGameType == gameType.VsAI)
            {
                grid2.playerControlled = false;
                if (theGameType == gameType.VsAI)
                {
                    grid2.myAI.startSearch(grid2);
                    //window.startTimer();
                }
            }
        }
        public void periodicCall()
        {
            if (theGameType == gameType.VsAI)
            {
                if(grid1SquidTurn)
                {
                    if (!grid2.myAI.processing && moveMade)
                    {
                        makeMove(grid2.myAI.getMove());
                        window.refreshGraphics();
                        moveMade = false;
                    }
                }
                else
                {
                    if (!grid1.myAI.processing && moveMade)
                    {
                        makeMove(grid1.myAI.getMove());
                        window.refreshGraphics();
                        moveMade = false;
                    }
                }
            }
            else if (theGameType == gameType.Online)
            {
                if (!moveMade)
                    return;
                gameMove m = window.getMove();
                if (m != null)
                {
                    makeMove(m);
                    window.refreshGraphics();
                    moveMade = false;
                }
            }
        }
        private void makeMove(gameMove theMove)
        {
            if (grid1SquidTurn)
            {
                grid2.makeMove(theMove);
                grid2.ready = false;
            }
            else
            {
                grid1.makeMove(theMove);
                grid1.ready = false;
            }
            switchTurn();
        }
        public void clickInput(MouseEventArgs e)
        {
            if (grid1.clickIt(e.Location, choose.getLetter()))
            {
                grid1.ready = false;
                moveMade = true;
                window.reportMove(grid1.getLastMove());
                switchTurn();
            }
            else if (grid2.clickIt(e.Location, choose.getLetter()))
            {
                grid2.ready = false;
                moveMade = true;
                window.reportMove(grid2.getLastMove());
                switchTurn();
            }
            else
            {
                choose.clicked(e.Location);
            }
        }
    }
}
