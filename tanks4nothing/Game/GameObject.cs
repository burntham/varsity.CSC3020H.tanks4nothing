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
    
    class GameObject
    {
        protected bool destroyed = false;

        //Keep track of what I'm stored in:D
        protected List<CollideBlock> containedInList = new List<CollideBlock>();

        protected List<Decals> decalList = new List<Decals>();

        /// <summary>
        /// Contains list of starting positions that object may spawn at/reset to
        /// </summary>
        public List<Vector2> spawningPositions = new List<Vector2>();

        /// <summary>
        /// Allow collision block to add itself to list of blocks containing player position
        /// </summary>
        /// <param name="container"></param>
        public void track(CollideBlock container)
        {
            containedInList.Add(container);
        }

        /// <summary>
        /// What should be drawn when this object is destroyed?
        /// </summary>
        public int decalType = -1;

        //Corner Pos
        protected Vector2 position;
        protected Vector2 prevPosition;
        protected Vector2 velocity;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 PrevPosition
        {
            get { return prevPosition; }
        }

        public Vector2 centerPosition
        {
            get { return position + (new Vector2(position.X + bbSideLength/2,position.Y + bbSideLength/2)); }
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)position.X, (int)position.Y, bbSideLength, bbSideLength); }
        }

        //used to rotate sprite
        public int orientation = 0;

        //bounding box width/height
        public int bbSideLength;

        public void collideUpdate(){
            //Collider.updatePos(this);
        }
    }
}
