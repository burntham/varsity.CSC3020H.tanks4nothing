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
    class CollideBlock
    {
        //relate stored grid to actual positions
        int blockoffSetX;
        int blockoffSetY;

        //relative coordinates in collision grid
        public int x_relative=0;
        public int y_relative=0;
        public int elements;

        int boxSize;

        int GridBlocksX;
        int GridBlocksY;

        int elementsY ;
        int elementsX ;

        int movingCount;
        int statColliadable;
        public bool containsMoving
        {
            get { return movingCount > 0 ? true : false; }
        }

        public bool constainsCollidable
        {
            get {return statColliadable > 0 ? true:false;}
        }


        
        //Block used to handle sections of collision stuffs
        //List of Fixed collidable Objects
        //List<GameObject> fixedObjects;
        Wall[,] fixedObjects;
        //List of collidable moving objects
        List<GameObject> movingObjects = new List<GameObject>();


        //public CollideBlock(int elementsX, int elementsY)
        /// <summary>
        /// Collide block constructs an object containing an array representing the orientation of 
        /// </summary>
        /// <param name="fieldLength"># of pixels wide the field is</param>
        /// <param name="fieldHeight"># of pixles high the field is</param>
        /// <param name="blockDegreeX"># of blocks field should be divided into (horizontally) for collision detection</param>
        /// <param name="blockDegreeY"># of blocks field should be divided into (vertically) for collision detection</param>
        /// <param name="x">relative block x</param>
        /// <param name="y">relative block y</param>
        public CollideBlock(int fieldLength, int fieldHeight, int blockDegreeX, int blockdegreeY, int bbSize, int x, int y, int GridCells_X, int GridCells_Y)
        {
            //how many cells should there be per block?
            GridBlocksX = GridCells_X;
            GridBlocksY = GridCells_Y;

            elementsY = GridBlocksY;//fieldHeight / (GridBlocksY * bbSize) ;
            elementsX = GridCells_X;//fieldLength / (GridBlocksX * bbSize) ;
            elements = elementsX * elementsY;
            
            boxSize = bbSize;

            //array used for collision checking
            fixedObjects = new Wall[elementsY, elementsX];

            x_relative = x;
            y_relative = y;

        }

        /// <summary>
        /// Add walls to each quadrant
        /// </summary>
        /// <param name="wall"></param>
        /// <param name="nBlocksX">How many Collider blocks lie on the x axis</param>
        /// <param name="nBlocksY">How many Collider blocks lie on the y axis</param>
        public void addWall(Wall wall, int nBlocksX, int nBlocksY)
        {
            int x =0;
            int y = 0;
            if (elements > 1)                
            {
                //Console.WriteLine("lolwaht" + wall.Position);
                //Console.WriteLine((boxSize + " " + GridBlocksX + " " + x_relative));
                //Console.WriteLine("subtracted: " + ((wall.Position.X - (boxSize * GridBlocksX* x_relative) )/10)+ "Equals " + (wall.Position.X - (boxSize * GridBlocksX * x_relative)) / boxSize);
                x = (int)((wall.Position.X - (boxSize * GridBlocksX* x_relative)) / boxSize);
                y = (int)((wall.Position.Y - (boxSize * GridBlocksY * y_relative)) / boxSize);
            }
            else 
            { }
           
            fixedObjects[y, x] = wall;
            statColliadable++;
            
        }

        //remove the game object from the stuffs -lols?
        public void remove(GameObject deleteme)
        {
            movingObjects.Remove(deleteme);      
        }

        public void add(GameObject addMe)
        {
            movingObjects.Add(addMe);
            //System.Console.WriteLine(fixedObjects[0, 0].Position);
            addMe.track(this);
            movingCount++;
        }

  

        public void checkCollision()
        {   
            if (!containsMoving)
            {/*do Nothing*/}            
            else if(containsMoving && constainsCollidable)
            {
               
                foreach (GameObject mvingObject in movingObjects)
                {   
                    for (int y = 0; y < fixedObjects.GetLength(0); y++)
                    {
                        for (int x = 0; x < fixedObjects.GetLength(1); x++)
                        {
                            if (!fixedObjects[y, x].isColliadble())
                            {}
                            else if (intercectionCheck(mvingObject, fixedObjects[y, x]))
                            {
                                if (mvingObject is Projectile)
                                {
                                    Projectile p = (Projectile)mvingObject;
                                    p.hitBlock();
                                    Collider.markforDest.Add(mvingObject);
                                    Collider.markforDest.Add(fixedObjects[y, x]);
               
                                    statColliadable--;
                                }
                                if (mvingObject is Player)
                                {
                                    Player p = (Player)mvingObject;
                                    if (p.PrevPosition == p.Position || p.spawningPositions.Contains(p.PrevPosition))
                                    {
                                        Collider.markforDest.Add(fixedObjects[y, x]);
                                        statColliadable--;
                                    }
                                    else
                                    {
                                        p.resetPosition();
                                    }

                                }
                            }
                        }
                    }
                }
            }
            else if (containsMoving)
            {
                for (int i=0; i<movingObjects.Count()-1;i++)
                {
                    for (int j = i + 1; j < movingObjects.Count(); j++)
                    {
                        if (intercectionCheck(movingObjects.ElementAt(i), movingObjects.ElementAt(j)))
                        {
                            GameObject o1 = movingObjects.ElementAt(i); GameObject o2 = movingObjects.ElementAt(j);

                            if (o1 is Player && o2 is Player)
                            {//Do nothing, don't allow players to collide - this will be used later (for spawning etc
                                
                            }
                            else if (o1 is Player || o2 is Player)
                            {
                                if (o1 is Projectile || o2 is Projectile)
                                {
                                    if (o1 is Projectile)
                                    {
                                        if (o2 is Agent)
                                        {
                                            Projectile temp = (Projectile)o1;
                                            Agent temp2 = (Agent)o2;
                                            if (temp.SourceObject != temp2 && temp.OldOwner !=temp2)
                                            {
                                                Collider.markforDest.Add(o1);
                                               // temp.hitPlayer();
                                                temp2.hijack(temp.SourceObject);
                                                temp2.respawn(null);
                                            }
                                        }
                                        else
                                        {
                                            Projectile temp = (Projectile)o1;
                                            Player temp2 = (Player)o2;
                                            if (temp.SourceObject != temp2)                                            {
                                                Collider.markforDest.Add(o1);
                                                temp2.respawn(temp.SourceObject);
                                            }
                                        }

                                    }else{
                                       
                                        if (o1 is Agent)
                                        {
                                            Projectile temp = (Projectile)o2;
                                            Agent temp2 = (Agent)o1;
                                            if (temp.SourceObject != temp2 && temp.OldOwner != temp2)
                                            {
                                                Collider.markforDest.Add(o2);
                                                // temp.hitPlayer();
                                                temp2.hijack(temp.SourceObject);
                                                temp2.respawn(null);
                                            }
                                        }
                                        else
                                        {
                                            Projectile temp = (Projectile)o2;
                                            Player temp2 = (Player)o1;
                                            if (temp.SourceObject != temp2)
                                            {
                                                Collider.markforDest.Add(o2);

                                                temp2.respawn(temp.SourceObject);
                                            }
                                        }
                                    }
                                }
                            }
                        } 
                    }
                }

            }
        }

        /// <summary>
        /// Check actual intersection of moving and static object
        /// </summary>
        public bool intercectionCheck(GameObject o1, GameObject o2)
        {
            //Console.WriteLine(staticO.Position);
            return o1.BoundingBox.Intersects(o2.BoundingBox);
        }

        /*
        /// <summary>
        /// Check intersection of 2 moving objects
        /// </summary>
        /// <param name="movingO1">Object 1</param>
        /// <param name="movingO2">Object 2</param>
        /// <returns>returns true of objects are colliding - may change this</returns>
        public bool intercectionCheckMovMov(GameObject movingO1, GameObject movingO2)
        {
            return false;
        }
        */
    }
}
