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
using tanks4nothing.Game;
using System.Diagnostics;

namespace tanks4nothing{
    class Projectile : GameObject
    {
        //movement sspeed
        float MS = 8f;

        //Where is this bullet stored?
        List<Projectile> bulletList = new List<Projectile>();

        //Current Owner
        public Player SourceObject;        

        //For Hijacking of Agent
        bool Hijacked = false;
        //source object becomes a reference to the new owner for compatability
        public Player OldOwner;


        /// <summary>
        /// Create a new bullet projectile
        /// </summary>
        /// <param name="startPos">Give the bullet a starting position</param>
        /// <param name="velocity">Give the bullet a unit vector specifying direction</param>
        /// <param name="orientation_">Give the respective orientation</param>
        /// <param name="agentSauce">Who is firing this quinte little shit - spelling is for noobs (it's 2AM)</param>
        public Projectile(Vector2 startPos, Vector2 velocity_, int orientation_, Player agentSauce, List<Projectile> bList, int bbSize_, List<Decals> decalList_)
        {
            bbSideLength = bbSize_;
            bulletList = bList;
            velocity = velocity_;
            position = startPos;
            orientation = orientation_;
            SourceObject = agentSauce;           
            bList.Add(this);
            decalList = decalList_;
        }

        public Projectile(Vector2 startPos, Vector2 velocity_, int orientation_, Player agentSauce, List<Projectile> bList, int bbSize_, List<Decals> decalList_, Player newLeader_)
        {
            bbSideLength = bbSize_;
            bulletList = bList;
            velocity = velocity_;
            position = startPos;
            orientation = orientation_;
            OldOwner = agentSauce;
            bList.Add(this);
            decalList = decalList_;
            Hijacked = true;
            SourceObject = newLeader_;
        }

        public void update()
        {

            if (position != null)
            {
                prevPosition = position;
            }

            foreach (CollideBlock hasme in containedInList)
            {
                hasme.remove(this);
            }

            position += velocity * MS;

            containedInList.Clear();

            position.X = MathHelper.Clamp(position.X, 1 * Collider.tileSize, (Collider.fieldLength - bbSideLength - Collider.tileSize));
            position.Y = MathHelper.Clamp(position.Y, 1 * Collider.tileSize, (Collider.fieldHeight - bbSideLength - Collider.tileSize));

            if (position.X == (1 * Collider.tileSize) || position.X == (Collider.fieldLength - bbSideLength - Collider.tileSize) || position.Y == (1 * Collider.tileSize) || position.Y == (Collider.fieldHeight - bbSideLength - Collider.tileSize))
            {
                Collider.markforDest.Add(this);
            }
            Collider.updatePos(this, bbSideLength);
        }

        public void destroy()
        {
            if (!destroyed)
            {
                if (Hijacked)
                {
                    OldOwner.reload(this);
                    bulletList.Remove(this);
                    foreach (CollideBlock hasme in containedInList)
                    {
                        hasme.remove(this);
                    }
                    containedInList.Clear();
                    decalList.Add(new Decals(this.position + velocity, 0, decalList, bbSideLength, velocity));
                    destroyed = true;
                }
                else
                {
                    SourceObject.reload(this);
                    bulletList.Remove(this);
                    foreach (CollideBlock hasme in containedInList)
                    {
                        hasme.remove(this);
                    }
                    containedInList.Clear();
                    decalList.Add(new Decals(this.position + velocity, 0, decalList, bbSideLength, velocity));
                    destroyed = true;
                }
            }
        }

        public void hitBlock()
        {
            if (SourceObject is Agent)
            {

            }
            else
            {
                //This is where Points are updated;
                SourceObject.updatePoints(1);
                
                //SOUND HERE!!!!
                
                Global.AudioPlayer.PlayGameSound(SourceObject.PlayerN);
            }
        }

        public void hitPlayer()
        {
            //depricated
        }

    }
}
