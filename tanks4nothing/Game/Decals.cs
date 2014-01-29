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
    class Decals: GameObject
    {
        //initial scale of drawn thing
        float initScale = 0.5f;
        //how many frames before decal self destruct?
        int frames = 3;

        public bool marked
        {
            get { return (frames == 0); }
        }

        public Decals(Vector2 position_, int decal_, List<Decals> decalList_, int bbSize, Vector2 velocity_)
        {
            position = position_;

            decalType = decal_;
            decalList = decalList_;
            bbSideLength = bbSize;
        }

        /// <summary>
        /// Scale/Animate the Decal expanding/contracting
        /// </summary>
        /// <returns>Returns the scale of the image</returns>
        public float scalaMate()
        {
            if (frames > 0)
            {
                --frames;
                position.X += 1f*initScale;
                position.Y -= 1f * initScale;

                if (decalType == 0)
                {
                    initScale += 0.4f;
                    return initScale;

                }
            }
            return 1f;
        }
    }
}
