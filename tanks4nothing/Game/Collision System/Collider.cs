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
    static class Collider
    {
        public static CollideBlock[,] collisionGrid;
        public static Wall[,] TileGrid;

        public static int fieldHeight, fieldLength, blockDegreeX, blockDegreeY;
        static int gridCellSize;
        public static int tileSize;

        static int ticksLeft;

        static int subdivsX;
        static int subdivsY;

        static int cellsX;
        static int cellsY;

        public static List<GameObject> markforDest;

        /// <summary>
        /// Initialize variables etc
        /// </summary>
        /// <param name="field_height"></param>
        /// <param name="field_length"></param>
        /// <param name="block_degreeX">how many collision grid boxes on x axis</param>
        /// <param name="block_degreeY">how manyy collision grid boxes on y axis</param>
        public static void initialize(int field_height, int field_length, int block_degreeX, int block_degreeY, int boxSize, int cellsperCollideBlockX,int cellsperCollideBlockY)
        {
            markforDest = new List<GameObject>();
            fieldHeight = field_height; fieldLength = field_length; blockDegreeX = block_degreeX; blockDegreeY = block_degreeY;
            gridCellSize = boxSize;
            tileSize = boxSize;
            cellsX =cellsperCollideBlockX;
            cellsY = cellsperCollideBlockY;

            //how many blocks will there be in the collision grid (each block contains walls etc - basically how many blocks do you want managed for each 'quadrant'
            subdivsX = fieldLength / (boxSize * cellsperCollideBlockX);
            subdivsY = fieldHeight / (boxSize * cellsperCollideBlockY);

            //create a collision Grid
            collisionGrid = new CollideBlock[subdivsY, subdivsX];
            TileGrid = new Wall[fieldHeight / boxSize, fieldLength / boxSize];

            for (int y = 0; y < subdivsY; y++)
            {
                for (int x = 0; x < subdivsX; x++)
                {
                    //initialize the collision grid
                    collisionGrid[y, x] = new CollideBlock(fieldLength, fieldHeight, blockDegreeX, blockDegreeY, gridCellSize, x, y, cellsX, cellsY);

                    //initialize tickets left to 0
                    ticksLeft = 0;
                }
            }
        }

        public static void populate(List<Wall> walls)
        {
            foreach (Wall wall in walls)
            {
                //Console.WriteLine(((int)(wall.Position.Y / (gridCellSize * cellsY))) + " "+(int)(wall.Position.X /(cellsX*gridCellSize)) );
                //Console.WriteLine(wall.Position);
                //Console.WriteLine((int)(wall.Position.Y / (gridCellSize * cellsY)) + " " + (int)(wall.Position.X / (cellsX * gridCellSize)));
                collisionGrid[(int)(wall.Position.Y / (gridCellSize*cellsY)), (int)(wall.Position.X /(cellsX*gridCellSize))].addWall(wall, blockDegreeX, blockDegreeY);
                TileGrid[(int)(wall.Position.Y / wall.bbSideLength), (int)(wall.Position.X / wall.bbSideLength)] = wall;        
            }


        }

        /// <summary>
        /// Update player position in collision grid
        /// </summary>
        /// <param name="Player">Player Object</param>
        /// <param name="bbSize">Player Bounding Box size</param>
        public static void updatePos(GameObject Player, int bbSize)
        {
            int xBlock = (int)Player.Position.X / (gridCellSize * cellsX);
            int yBlock = (int)Player.Position.Y / (gridCellSize * cellsY);
            int xFar = (int)Math.Ceiling((double)((Player.Position.X + bbSize) / (gridCellSize * cellsX)));
            int yFar = (int)Math.Ceiling((double)((Player.Position.Y + bbSize) / (gridCellSize * cellsY)));

            for (int y = yBlock; y < yFar; y++)
            {
                for (int x = xBlock; x < xFar; x++)
                {
                    collisionGrid[y, x].add(Player);
                }
            }  
        }

        /// <summary>
        /// Collision checker
        /// </summary>
        /// <param name="ticks">Ticks TBimplemented</param>
        public static void CollisionChecker(int ticks)
        {
            if (false)//ticksLeft != 0)
            {
                ticksLeft = (int)MathHelper.Clamp(ticksLeft - 1, 0, ticks);
            }
            else
            {
                ticksLeft = ticks;

                for (int y = 0; y < collisionGrid.GetLength(0); y++)
                {
                    for (int x = 0; x < collisionGrid.GetLength(1); x++)
                    {
                        collisionGrid[y, x].checkCollision();
                    }
                }                
            }


            foreach (GameObject doomedObject in markforDest)
            {
                if (doomedObject is Wall)
                {
                    Wall doomed = (Wall)doomedObject;
                    doomed.Destroy();
                }
                else if (doomedObject is Projectile)
                {
                    Projectile doomed = (Projectile)doomedObject;
                    doomed.destroy();
                }
            }
        }
    }
}
