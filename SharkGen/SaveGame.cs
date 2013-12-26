using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharkGen
{
    [Serializable]
   public  class SaveGame
    {
        public SaveGame(Game g)
        {
            game = g;
        }
        public Game game;
    }
}
