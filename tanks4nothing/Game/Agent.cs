using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using tanks4nothing.Game;

namespace tanks4nothing
{
    class Agent : Player
    {   
        protected Random Randoom;

        /// <summary>
        /// Movement cooldown is the max time for a change in direction
        /// </summary>
        protected int movementCooldown = 100;
        protected int movementTimer;
        public bool Hijacked = false;
        Player newLeader;

        protected List<Agent> gameAgentList = new List<Agent>();
       // public L


        /// <summary>
        /// Agent Constructor - is constructed the exam same way as a normal Player (for now)
        /// </summary>
        /// <param name="position_">Player position</param>
        /// <param name="player_">set to 0 so it is not a player</param>
        /// <param name="boxSide_">collision box size</param>
        /// <param name="bList">//bullet list (this is needed)</param>
        /// <param name="bulletSize_">this might be needed</param>
        public Agent(Vector2 position_, int boxSide_, List<Projectile> bList, int bulletSize_, List<Agent> gameAgentList_, List<Decals> decalList_, int[] PlayerScores_)
            : base(position_, -1, boxSide_, bList, bulletSize_, decalList_,PlayerScores_,null, true)
        {
            Randoom=new Random(Global.agentSeed);
            gameAgentList = gameAgentList_;
            //gameAgentList.Add(this);

        }

        public void hijack(Player newLeader_)
        {
            
            newLeader = newLeader_;
            colour = newLeader.colour;
            Hijacked = true;
            newLeader.Minion = this;
        }

        public void update()
        {
            if (moved)
            {
                prevPosition = position;
            }

            foreach (CollideBlock hasme in containedInList)
            {
                hasme.remove(this);
            }
            containedInList.Clear();

            //Handle Movement and Shooting below
            shootTimer = (shootTimer == 0) ? 0 : --shootTimer;
            movementTimer = (movementTimer == 0) ? 0 : --movementTimer;

            if (movementTimer == 0)
            {
                //assign a random direction
                orientation = Randoom.Next(0, 4);
                moved = true;

                //position.X = (float)Math.Round((float)position.X / 5) * 5;
                //position.Y = (float)Math.Round((float)position.Y / 5) * 5;

                if (orientation == 3)
                {
                    velocity =new Vector2(-1f,0f)*MS;
                }
                else if (orientation == 1)
                {
                    velocity = new Vector2(1f, 0f) * MS;

                }
                else if (orientation == 2)
                {
                    velocity = new Vector2(0f, 1f) * MS;

                }
                else if (orientation == 0)
                {
                    velocity = new Vector2(0f, -1f) * MS;

                }
                movementTimer = Randoom.Next(0, movementCooldown);
            }

            position += velocity;

            if (shootTimer==0)
            {
                if (CurrentBullet.Count == 0 && shootTimer == 0)
                {
                    shootTimer =Randoom.Next(0, 30);

                    Vector2 firePos;
                    Vector2 fireDir;
                    float fireAider = bbSideLength / 2;


                    if (orientation == 0)
                    {
                        firePos = new Vector2(position.X + fireAider - 5, position.Y - 5);
                        fireDir = new Vector2(0f, -1f);

                    }
                    else if (orientation == 1)
                    {
                        firePos = new Vector2(position.X + bbSideLength - 5, position.Y + fireAider - 5);
                        fireDir = new Vector2(1f, 0f);
                    }
                    else if (orientation == 2)
                    {
                        firePos = new Vector2(position.X + fireAider - 5, position.Y + bbSideLength - 5);
                        fireDir = new Vector2(0f, 1f);
                    }
                    else
                    {
                        firePos = new Vector2(position.X - 5, position.Y + fireAider - 5);
                        fireDir = new Vector2(-1f, 0f);
                    }

                    if (!Hijacked)
                    {
                        CurrentBullet.Add(new Projectile(firePos, fireDir, orientation, this, bulletList, bulletSize, decalList));
                    }
                    else
                    {
                        CurrentBullet.Add(new Projectile(firePos, fireDir, orientation, this, bulletList, bulletSize, decalList,newLeader));

                    }
                }
            }
            position.X = MathHelper.Clamp(position.X,  Collider.tileSize, (Collider.fieldLength - bbSideLength - Collider.tileSize));
            //position.X = (float)Math.Round((float)position.X / 5) * 5;
            position.Y = MathHelper.Clamp(position.Y,  Collider.tileSize, (Collider.fieldHeight - bbSideLength - Collider.tileSize));
            //position.Y = (float)Math.Round((float)position.Y / 5) * 5;
            
            Collider.updatePos(this, bbSideLength);
        }
    }
}
