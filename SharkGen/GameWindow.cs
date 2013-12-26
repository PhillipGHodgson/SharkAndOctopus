using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharkGen
{
    public interface GameWindow
    {
        void refreshGraphics();
        void reportMove(gameMove move1);
        gameMove getMove();
        int getDifficulty();
    }
}
