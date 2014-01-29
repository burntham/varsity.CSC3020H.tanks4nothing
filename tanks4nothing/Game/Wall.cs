using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace tanks4nothing
{
    class Wall : GameObject
    {
        enum Collidable
        {
            //normal wall object
            stdWall = 1,

            //destroyed wall. - do not draw
            deadWall = 0,
        }

        /// <summary>
        /// Relative position on tilefrid (allows for easy updating of surrounding tile textures)
        /// </summary>
        int tileGridx=-1;
        int tileGridy=-1;
        
        //Usefull Variables
        Collidable Type;
        public int type
        {
            get { return Type == Collidable.stdWall ? 1 : 0; }
            
        }

        public int tex =0;
  
        
        public Texture2D wallTex
        {
            get { return wallTex; }
            set { wallTex = value; }
        }

        public Wall(int posX, int posY, int width, int height,int bbSideLength_)
        {
            bbSideLength = bbSideLength_;            
            position = new Vector2(posX, posY);
            tileGridy=posY/bbSideLength;
            tileGridx = posX / bbSideLength;
            Type = LevelMan.blocks[tileGridy, tileGridx] == 0 ? Collidable.deadWall : Collidable.stdWall;

            updateT();
        }

        public bool isColliadble()
        {
            return Type == Collidable.stdWall ? true : false;
        }

        //return a bounding box, will be used for collision detection
        public Rectangle getBoundBox()
        {
            return new Rectangle((int)position.X, (int)position.Y, wallTex.Width, wallTex.Height);
        }

        //work out which texture should be drawn
        /// <summary>
        /// Update texture thingy - dynamically changes outline when needed
        /// </summary>
        public void updateT()
        {
            
            //create arrays and compare with grid storing tile type and decide which texture will be used
            if (Type == Collidable.deadWall)
            { }
            else
            {
                if ((int)position.X == 0 || (int)position.X >= LevelMan.blocks.GetLength(1)*bbSideLength)
                {
                    //do nothing
                }
                else if ((int)position.Y == 0 || (int)position.Y >=LevelMan.blocks.GetLength(0)*bbSideLength)
                {
                    //do nothing
                }
                else
                {
                    int testT=0;

                    int posY = (int)position.Y / bbSideLength; int posX = (int)position.X / bbSideLength;
                    
                    int xL = (posX - 1); int xR = (posX + 1); int yU = (posY - 1); int yD = (posY + 1);

                    testT = LevelMan.blocks[posY, xL] == 1 ? testT + 8 : testT;
                    testT = LevelMan.blocks[posY, xR] == 1 ? testT + 2 : testT;
                    testT = LevelMan.blocks[yU, posX] == 1 ? testT + 1 : testT;
                    testT = LevelMan.blocks[yD, posX] == 1 ? testT + 4 : testT;

                    this.tex = testT;
                }                
            }
        }

        /// <summary>
        /// Remove block and update surrounding
        /// </summary>
        public void Destroy()
        {
            this.Type = Collidable.deadWall;
            int blockPosX = (int)(this.position.X / bbSideLength);
            int blockPosy = (int)(this.position.Y / bbSideLength);
            LevelMan.blocks[blockPosy, blockPosX] = 0;
            Collider.TileGrid[blockPosy - 1, blockPosX].updateT();
            Collider.TileGrid[blockPosy + 1, blockPosX].updateT();
            Collider.TileGrid[blockPosy , blockPosX+1].updateT();
            Collider.TileGrid[blockPosy, blockPosX - 1].updateT();
        }

    }
}
