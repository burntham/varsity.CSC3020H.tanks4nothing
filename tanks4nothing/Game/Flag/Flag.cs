using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace tanks4nothing
{
    class Flag :GameObject
    {
        public Player owner;
        public Flag(Vector2 position_, Player player)
        {
            position = position_;
            spawningPositions.Add(position);
            owner = player;
        }


        public void reset()
        {
            
        }
    }
}
