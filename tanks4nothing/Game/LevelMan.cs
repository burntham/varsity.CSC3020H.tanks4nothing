using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tanks4nothing
{
    static class LevelMan
    {
        public static int[,] Level1 ;

        public static int[,] blocks;

        //used for drawing static objects?
        public static List<Wall> wallObjects;

        /// <summary>
        /// Create the Level!
        /// </summary>
        /// <param name="tilesX">How many Tiles long</param>
        /// <param name="tilesY">How Many Tiles High</param>
        /// <param name="wallSize">The Pixel width/length of the tiles</param>
        public static void initialize(int tilesX, int tilesY,int wallSize)
        {
            wallObjects = new List<Wall>();
            Level1 = new int[tilesY,tilesX];

            for (int y = 0; y < tilesY; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    Level1[y, x] = (x == 0 || y == 0) ? 0 : (x == tilesX - 1 || y == tilesY - 1) ? 0 : 1;
                }
            }


            blocks = (int[,])Level1.Clone();
            for (int j = 0; j < Level1.GetLength(0); j++)
            {
                for (int i = 0; i < Level1.GetLength(1); i++)
                {
                    wallObjects.Add(new Wall(i * wallSize, j * wallSize, wallSize, wallSize,wallSize));
                }
            }
        }
    }
}
