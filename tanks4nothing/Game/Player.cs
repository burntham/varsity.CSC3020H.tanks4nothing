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
    class Player : GameObject
    {
        GamePadState gamepad;
        KeyboardState keyboard;

        //Shoot cooldown timer
        protected int cooldown=10;
        protected int shootTimer;

        protected bool moved = false;
        int[] PlayerScores;

        /// <summary>
        /// AI STUFFS
        /// </summary>
        Boolean isAI = false;
        protected Random Randoom;
        protected int movementCooldown = 200;
        protected int movementTimer;

        
        public Color colour;

        Keys kUp = Keys.W;
        Keys kDown = Keys.S;
        Keys kLeft = Keys.A;
        Keys kRight = Keys.D;
        Keys kShoot = Keys.Space;

        protected List<Projectile> bulletList = new List<Projectile>();
        protected int bulletSize;
        protected List<Projectile> CurrentBullet = new List<Projectile>();
        //Player 1 or 2?
        int player;

        public int PlayerN{
            get { return player; }
        }

        public Agent Minion;

        //movement sspeed
        protected float MS = 4f;

        /// <summary>
        /// Construct Player Class
        /// </summary>
        /// <param name="position_">Spawn position</param>
        /// <param name="player_">player number (1 for player 1, 2 for player 2, -1 for agents)</param>
        /// <param name="boxSide_">bounding box size</param>
        /// <param name="bList">list of bullets</param>
        /// <param name="bulletSize_"></param>
        /// <param name="decalList_">list of stickers (explosions etc)</param>
        /// <param name="PlayerScores_">int array containing player scores (so Projectiles can update scores)</param>
        /// <param name="soundEffect_">the name of the sound effect that will be played for this player</param>
        public Player(Vector2 position_, int player_, int boxSide_, List<Projectile> bList, int bulletSize_, List<Decals> decalList_, int[] PlayerScores_, String soundEffect_, Boolean isAi_)
        {
            bulletSize = bulletSize_;
            bulletList = bList;
            position = position_;
            player = player_;
            bbSideLength = boxSide_;
            decalList = decalList_;
            PlayerScores = PlayerScores_;
            isAI = isAi_;
            
            if(isAI)
                Randoom = new Random(Global.p2Seed);

            if (player == 1)
            {
                colour = Color.White;
                spawningPositions.Add(position); spawningPositions.Add(position + (new Vector2(0f, (Collider.tileSize*15)))); spawningPositions.Add(position + (new Vector2(0f, Collider.tileSize*30)));
            }
            else if (player == 2)
            {
                colour = Color.Aqua;
                kUp = Keys.Up; kDown = Keys.Down; kLeft = Keys.Left; kRight = Keys.Right; kShoot = Keys.NumPad0;
                spawningPositions.Add(position); spawningPositions.Add(position - (new Vector2(0f, (Collider.tileSize * 15)))); spawningPositions.Add(position - (new Vector2(0f, Collider.tileSize * 30)));
            }
            else if (player == -1)
            {
                colour = Color.Crimson;
            }
            //position.X = MathHelper.Clamp(position.X, 2 * Collider.tileSize, (Collider.fieldLength - bbSideLength - Collider.tileSize));
            //position.Y = MathHelper.Clamp(position.Y, 2 * Collider.tileSize, (Collider.fieldHeight - bbSideLength - Collider.tileSize));
        }

        public void update(GamePadState gamepad_, KeyboardState keyboard_)
        {
            if (isAI)
                updateAi();
            else
                updatePlayer(gamepad_, keyboard_);
        }

         public void updatePlayer(GamePadState gamepad_, KeyboardState keyboard_)
        {
            shootTimer =( shootTimer == 0) ? 0 : --shootTimer;
            //Console.WriteLine(shootTimer);
            prevPosition = position;

            //if (position.X< 25)
            //    orientation = 2;

            foreach (CollideBlock hasme in containedInList)
            {
                hasme.remove(this);
            }
            containedInList.Clear();

            gamepad = gamepad_;
            keyboard = keyboard_;
            bool left = (gamepad.DPad.Left == ButtonState.Pressed || keyboard.IsKeyDown(kLeft));
            bool right = (gamepad.DPad.Right == ButtonState.Pressed || keyboard.IsKeyDown(kRight));
            bool up =(gamepad.DPad.Up == ButtonState.Pressed || keyboard.IsKeyDown(kUp));
            bool down = (gamepad.DPad.Down == ButtonState.Pressed || keyboard.IsKeyDown(kDown));
            bool shoot = (gamepad.Buttons.A == ButtonState.Pressed || keyboard.IsKeyDown(kShoot));

            if (left)
            {
                position.X -= MS;
                orientation = 3;
            }
            else if (right)
            {
                position.X += MS;
                orientation = 1;
            }
            else if (down)
            {
                position.Y += MS;
                orientation = 2;
            }
            else if (up)
            {
                position.Y -= MS;
                orientation = 0;
           }

            if (shoot)
            {
                if (CurrentBullet.Count == 0 && shootTimer==0)
                {
                    shootTimer = cooldown;
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
                    CurrentBullet.Add(new Projectile(firePos, fireDir, orientation, this, bulletList, bulletSize, decalList));
                }
            }
            
            position.X = MathHelper.Clamp(position.X, 1*Collider.tileSize, (Collider.fieldLength - bbSideLength- Collider.tileSize));
            position.Y = MathHelper.Clamp(position.Y, 1 * Collider.tileSize, (Collider.fieldHeight -bbSideLength- Collider.tileSize));

            Collider.updatePos(this, bbSideLength);
        }

         public void updateAi()
         {
                 prevPosition = position;


                 foreach (CollideBlock hasme in containedInList)
                 {
                     hasme.remove(this);
                 }
                 containedInList.Clear();

             //Handle Movement and Shooting below
             shootTimer = (shootTimer == 0) ? 0 : --shootTimer;
             movementTimer = (movementTimer == 0) ? 0 : --movementTimer;

             Boolean newOrientation = false;
             //in top left corner
             #region adjust orientation
             if (position.X <= 21 && position.Y <=21)
             {
                 if (orientation == 0 || orientation == 3)
                 {
                     orientation = Randoom.Next(1, 3);
                     newOrientation = true;
                 }
             }
             else if (position.X <= 21 && position.Y >= 449)
             {
                 if (orientation == 3 || orientation == 2)
                 {
                     orientation = Randoom.Next(0, 2);
                     newOrientation = true;
                 }
             }
             else if (position.X >=1149 && position.Y <= 21)
             {
                 if (orientation == 0 || orientation == 1)
                 {
                     orientation = Randoom.Next(2, 4);
                     newOrientation = true;
                 }
             }
             else if (position.X >= 1150 && position.Y >= 550)
             {
                 if (orientation == 1 || orientation == 2)
                 {
                     orientation = Randoom.Next(3, 5) % 4;
                     newOrientation = true;
                 }
             }
             else if (position.X <= 20)
             {
                 if (orientation == 3)
                 {
                     orientation = Randoom.Next(0, 3);
                     newOrientation = true;
                 }                 
             }
             else if (position.X >= 1150)
             {
                 if (orientation == 1)
                 {
                     orientation = Randoom.Next(0, 3) + 1;
                     newOrientation = true;
                 }
             }
             else if (position.Y <= 20)
             {
                 if (orientation == 0)
                 {
                     orientation = Randoom.Next(1, 4);
                     newOrientation = true;
                 }
             }
             else if (position.Y >= 550)
             {
                 if (orientation == 2)
                 {
                     orientation = Randoom.Next(3, 6)%4;
                     newOrientation = true;
                 }
             }
             #endregion


             if (movementTimer == 0 || newOrientation)
             {
                 //assign a random direction
                 if (!newOrientation)
                    orientation = Randoom.Next(0, 4);
                 moved = true;

                 //position.X = (float)Math.Round((float)position.X / 5) * 5;
                 //position.Y = (float)Math.Round((float)position.Y / 5) * 5;

                 if (orientation == 3)
                 {
                     velocity = new Vector2(-1f, 0f) * MS;
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
                 if (!newOrientation)
                    movementTimer = Randoom.Next(0, movementCooldown);
             }

             position += velocity;

             if (shootTimer == 0)
             {
                 if (CurrentBullet.Count == 0 && shootTimer == 0)
                 {
                     shootTimer = Randoom.Next(0, 20);

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

                         CurrentBullet.Add(new Projectile(firePos, fireDir, orientation, this, bulletList, bulletSize, decalList));

                 }
             }
             position.X = MathHelper.Clamp(position.X, Collider.tileSize, (Collider.fieldLength - bbSideLength - Collider.tileSize));
             //position.X = (float)Math.Round((float)position.X / 5) * 5;
             position.Y = MathHelper.Clamp(position.Y, Collider.tileSize, (Collider.fieldHeight - bbSideLength - Collider.tileSize));
             //position.Y = (float)Math.Round((float)position.Y / 5) * 5;

             Collider.updatePos(this, bbSideLength);
         }

         public void reload(Projectile bullet)
         {
             CurrentBullet.Remove(bullet);
         }

         public void resetPosition()
         {
             if (player == -1 && prevPosition == new Vector2(0f, 0f))
             {
             }
             else
             {
                 position = prevPosition;
             }
         }

        /// <summary>
        /// Respawn player after they are killed
        /// </summary>
         public void respawn(Player hijacker)
         {
             Random Randoom = new Random();
             if (player > 0)
             {
                 if (Minion != null)
                 {
                     if (hijacker != null)
                     {
                         Minion.hijack(hijacker);
                         Minion = null;
                     }
                 }
                 position = spawningPositions.ElementAt(Randoom.Next(0, spawningPositions.Count));
             }
             
         }

         public void updatePoints(int points)
         {
             PlayerScores[player - 1] += points;
         }

    }
}
