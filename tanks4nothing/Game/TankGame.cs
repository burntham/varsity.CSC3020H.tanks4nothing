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
using tanks4nothing.Game.Sound;
using tanks4nothing.Game;

namespace tanks4nothing
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TankGame : Microsoft.Xna.Framework.Game
    {
        string player1Sound = "1";
        string player2Sound = "2";

        int Time = 90;
        Timer timer;
        Boolean withAi = true;
        Boolean fullScreen = true;
        int soundInstances = 30;

        string player1SoundFile = "Seffects/laser";
        string player2SoundFile = "Seffects/appear-online";

        SoundEffect player1SoundEffect;
        SoundEffect player2SoundEffect;
        
        enum GameState{
            Menu, Playing, Paused
        }

        public static float Volume = 1.0f;

        bool gameOver;
        bool restarting = false;

        int timesRun = 0;


        GameState gameIsIn = GameState.Menu;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //font
        private SpriteFont hudFont;

        int menuSelection;
        int pauseMenuSelection;

        /// <summary>
        /// Visual Settings
        /// These will affect the game looks, not its core
        /// </summary>
        int playerDrawSize = 30;
        int tileDrawSize = 20;
        int bulletDrawSize = 10;
        int tilesLong=60;
        int tilesHigh=30;

        int tempSelectCount = 1;
        
        /// <summary>
        /// These values affect the way the game calculates collisions note: bounding boxes are squares
        /// </summary>
        int playerBoundingBoxSize=30;
        int wallBoundingBoxSize=20;
        int bulletBoundingBoxSize = 10;
        //offsets (relate grid to screen) - will move drawn images - still to be implemented
        int xOff = 40;//70;
        int yOff = 60;//60;

        int collideCellSizeX =1;
        int collideCellSizeY = 1;
        //How often the collision detection will run
        int collisionTiker=0;

        //input stuffs
        private GamePadState GPstateP1;
        private GamePadState GPstateP1Old;
        private GamePadState GPstateP2;
        private GamePadState GPstateP2Old;
        private KeyboardState KBstate;
        private KeyboardState KBstateOld;
        //private float rotationHelper = MathHelper.Pi/2;

        //Usefull Textures.
        //Texture2D backGround;
        List<Texture2D> wallBlocks = new List<Texture2D>();
        List<Texture2D> playerTex = new List<Texture2D>();
        List<Texture2D> bulletTex = new List<Texture2D>();
        List<Texture2D> DecalTex = new List<Texture2D>();
        List<Texture2D> backGround = new List<Texture2D>();
        Texture2D menu;
        Texture2D pauseMenu;

        //ProjectileList
        List<Projectile> BulletList;// = new List<Projectile>();
        //Decal List (explosions etc)
        List<Decals> decalList;//new List<Decals>();

        //Agent List (AI)
        List<Agent> agentList; //= new List<Agent>();

        //Initial Player Poisition
        Player player1 ;
        Player player2;

/// <summary>
/// NB!!!
/// </summary>
        public TankGame()
        {
            graphics = new GraphicsDeviceManager(this);

            //Set preferred resolution
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = fullScreen;

            //set conent directory
            Content.RootDirectory = "Content";

            Global.AudioPlayer = new AudioManager(this);
            Global.timeLeft = Time;
            //Initialize Global stuffs - used for sound.

        }

        public void InputManager(){
            
            GPstateP1Old = GPstateP1;
            GPstateP2Old = GPstateP2;
            KBstateOld = KBstate;
            GPstateP1 = GamePad.GetState(PlayerIndex.One);
            GPstateP2 = GamePad.GetState(PlayerIndex.Two);
            KBstate = Keyboard.GetState();
        }
        public void menuSelectionItem(int options)
        {
            bool up = ((GPstateP1.IsButtonUp(Buttons.DPadUp) && GPstateP1Old.IsButtonDown(Buttons.DPadUp)) || (KBstate.IsKeyUp(Keys.Up) && KBstateOld.IsKeyDown(Keys.Up)));
            bool down = ((GPstateP1.IsButtonUp(Buttons.DPadDown) && GPstateP1Old.IsButtonDown(Buttons.DPadDown)) || (KBstate.IsKeyUp(Keys.Down) && KBstateOld.IsKeyDown(Keys.Down)));
            bool enter = ((GPstateP1.IsButtonUp(Buttons.A) && GPstateP1Old.IsButtonDown(Buttons.A) || (KBstate.IsKeyUp(Keys.Enter) && KBstateOld.IsKeyDown(Keys.Enter))));

            menuSelection += ((up) ? -1 : (down) ? 1 : 0);
            menuSelection = Math.Abs(menuSelection) % options;

            if (enter)
            {
                if (gameIsIn == GameState.Menu)
                {
                    if (menuSelection == 1)
                    {
                        this.Exit();
                    }
                    else
                    {
                        gameOver = false;
                        restarting = true;
                        
                        gameIsIn = GameState.Playing;
                        Initialize();
                    }
                }else {
                    if (menuSelection == 0)
                    {
                        gameIsIn = GameState.Playing;
                    }
                    else if (menuSelection == 1)
                    {
                        Initialize();
                        gameIsIn = GameState.Playing;
                    }
                    else
                    {
                        this.Exit();
                    }

                }
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Global.timeLeft = Time;
            Global.AudioPlayer = new AudioManager(this);
            
            
            gameOver = false;
            //gameIsIn = GameState.Playing;
            restarting = false;

            timer = new Timer(Time);

            menuSelection = 0;
            pauseMenuSelection = 0;

            if (timesRun == 0)
            {
                BulletList = new List<Projectile>();
                decalList = new List<Decals>();
                agentList = new List<Agent>();
            }
            else
            {
                BulletList.Clear();
                decalList.Clear();
                agentList.Clear();
            }

            Collider.initialize(wallBoundingBoxSize * tilesHigh, wallBoundingBoxSize * tilesLong, tilesLong, tilesHigh, wallBoundingBoxSize, collideCellSizeX, collideCellSizeY);
            LevelMan.initialize(tilesLong, tilesHigh, wallBoundingBoxSize);
            Collider.populate(LevelMan.wallObjects);

            Global.PlayerScores = new int[2]{0,0};

            player1 = new Player(new Vector2(0f * wallBoundingBoxSize, 0f * wallBoundingBoxSize), 1, playerBoundingBoxSize, BulletList, bulletBoundingBoxSize, decalList, Global.PlayerScores, player1Sound, false);
            player2 = new Player(new Vector2(57f * wallBoundingBoxSize, 27f * wallBoundingBoxSize), 2, playerBoundingBoxSize, BulletList, bulletBoundingBoxSize, decalList, Global.PlayerScores, player2Sound, withAi);

            agentList.Add(new Agent(new Vector2(30f * wallBoundingBoxSize, 15f * wallBoundingBoxSize), wallBoundingBoxSize, BulletList, bulletBoundingBoxSize, agentList, decalList, new int[2]));

            player1SoundEffect = Content.Load<SoundEffect>(player1SoundFile);
            player2SoundEffect = Content.Load<SoundEffect>(player2SoundFile);

            Global.AudioPlayer.LoadGameSound(player1SoundEffect, player1Sound, soundInstances, 1.0f, false);
            Global.AudioPlayer.LoadGameSound(player2SoundEffect, player2Sound, soundInstances, 1.0f, false);
            // TODO: Add your initialization logic here
            base.Initialize();
        }

/// <summary>
/// NB
/// </summary>
        protected override void LoadContent()
        {
            //Console.WriteLine(2);
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Create grids that manage collision detection, the inner grid in each quadrant will be built with wallbounding size

            for (int i = 0; i < 2; i++)
            {
                string bgtex = string.Format("BackGround/BackGround{0}",i);
                backGround.Add(Content.Load<Texture2D>(bgtex));
            }
            //backGround = Content.Load<Texture2D>("battleground");
            for (int i = 0; i < 16; i++)
            {
                string texName = string.Format("Walls/stdWallBlocky{0}",i);wallBlocks.Add(Content.Load<Texture2D>(texName));
            }
            for (int i = 0; i < 4; i++)
            {
                string texName = string.Format("Player/tank{0}", i);playerTex.Add(Content.Load<Texture2D>(texName));
                string btexName = string.Format("Projectiles/Bullet{0}", i); bulletTex.Add(Content.Load<Texture2D>(btexName));
            }

            DecalTex.Add(Content.Load<Texture2D>("Decals/decal0"));

            hudFont = Content.Load<SpriteFont>("HudFontal");

            menu = Content.Load<Texture2D>("Menu/MenuScreen");

            pauseMenu = Content.Load<Texture2D>("Menu/PauseScreen");


           
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            timer.update();
            InputManager();

            if (gameIsIn == GameState.Menu)
            {
                if (GPstateP1.Buttons.Back == ButtonState.Pressed || GPstateP2.Buttons.Back == ButtonState.Pressed)
                    this.Exit();
                else if (KBstate.IsKeyDown(Keys.Escape))
                    this.Exit();

               menuSelectionItem(2);
                
            }

            else if (gameIsIn == GameState.Playing)
            {
                // Allows the game to exit
                if ((GPstateP1.IsButtonUp(Buttons.Back)&& GPstateP1Old.IsButtonDown(Buttons.Back) || (GPstateP2.IsButtonUp(Buttons.Back) && GPstateP2Old.IsButtonDown(Buttons.Back))))
                    gameIsIn = GameState.Paused;
                else if (KBstate.IsKeyUp(Keys.Escape)&& KBstateOld.IsKeyDown(Keys.Escape))
                    gameIsIn = GameState.Paused;

                player1.update(GPstateP1, KBstate);
                player2.update(GPstateP2, KBstate);

                foreach (Projectile bullet in BulletList)
                {
                    bullet.update();
                }

                if (agentList.Count > 0)
                {

                    foreach (Agent ai in agentList)
                    {
                        ai.update();
                    }
                }

                for (int i = 0; i < decalList.Count; i++)
                {
                    if (decalList.ElementAt(i).marked)
                    {
                        decalList.RemoveAt(i);
                        --i;
                    }
                }

                Collider.CollisionChecker(0);

                // TODO: Add your update logic here
            }
            else if (gameIsIn == GameState.Paused)
            {
                timer.startPause();
                if ((GPstateP1.IsButtonUp(Buttons.Back) && GPstateP1Old.IsButtonDown(Buttons.Back) || (GPstateP2.IsButtonUp(Buttons.Back) && GPstateP2Old.IsButtonDown(Buttons.Back)))){

                    gameIsIn = GameState.Playing;
                    timer.stopPause();
                }

                else if (KBstate.IsKeyUp(Keys.Escape) && KBstateOld.IsKeyDown(Keys.Escape))
                {
                    gameIsIn = GameState.Playing;
                    timer.stopPause();
                }

                menuSelectionItem(3);
            }

            if (timer.elapsedTime() <= 0 && !restarting)
            {
                gameOver = true;
                gameIsIn = GameState.Menu;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //default stuffs
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Rectangle fullscreen = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            spriteBatch.Begin();

            if (gameIsIn == GameState.Playing)
            {
                //draw blocks:
               spriteBatch.Draw(backGround.ElementAt(0), fullscreen, Color.SaddleBrown);

                foreach (Wall wall in LevelMan.wallObjects)
                {
                    if (wall.type == 1)
                    {
                        //Console.WriteLine(wall.tex);
                        spriteBatch.Draw(wallBlocks.ElementAt(wall.tex), new Rectangle(xOff +((int)wall.Position.X), (yOff +(int)wall.Position.Y), tileDrawSize, tileDrawSize), null, Color.SaddleBrown, 0f, new Vector2(0f, 0f), SpriteEffects.None, 0f);

                    }
                }

                foreach (Projectile bullet in BulletList)
                {
                    //Console.WriteLine(bullet.orientation);
                    spriteBatch.Draw((bulletTex.ElementAt(bullet.orientation)), new Rectangle((xOff + (int)bullet.Position.X), (yOff + (int)bullet.Position.Y), bulletDrawSize, bulletDrawSize), null, Color.White, 0f, new Vector2(0f, 0f), SpriteEffects.None, 0f);
                }

                if (agentList.Count > 0)
                {
                    foreach (Agent ai in agentList)
                    {
                        spriteBatch.Draw(((playerTex.ElementAt(ai.orientation))), new Rectangle((xOff + (int)ai.Position.X), (yOff + (int)ai.Position.Y), wallBoundingBoxSize, wallBoundingBoxSize), null, ai.colour, 0f, new Vector2(0f, 0f), SpriteEffects.None, 0f);
                    }
                }
                
                //Draw Players
                spriteBatch.Draw(playerTex.ElementAt(player1.orientation), new Rectangle((xOff + (int)player1.Position.X), yOff + (int)(player1.Position.Y), playerDrawSize, playerDrawSize), player1.colour);
                spriteBatch.Draw(playerTex.ElementAt(player2.orientation), new Rectangle((xOff + (int)player2.Position.X), yOff + (int)(player2.Position.Y), playerDrawSize, playerDrawSize), player2.colour);

                spriteBatch.Draw(backGround.ElementAt(1), fullscreen, Color.White);
                if (decalList.Count > 0)
                {
                    // Console.WriteLine(decalList.Count);

                    foreach (Decals dec in decalList)
                    {
                        float scale = dec.scalaMate();
                        spriteBatch.Draw((DecalTex.ElementAt(dec.decalType)), new Rectangle((xOff + (int)dec.Position.X), (yOff + (int)dec.Position.Y), (int)(dec.bbSideLength * scale), (int)(dec.bbSideLength * scale)), null, Color.White, 0f, new Vector2(5f, 5f), SpriteEffects.None, 0f);
                    }
                }

                ///Drawer score etc
                string PlayerScore1 = "" + Global.PlayerScores[0];
                string PlayerScore2 = "" + Global.PlayerScores[1];
                string timeLeft = "" + timer.elapsedTime();
                Global.timeLeft = timer.elapsedTime();

                spriteBatch.DrawString(hudFont, PlayerScore1, new Vector2(210, 10f), Color.Red);
                spriteBatch.DrawString(hudFont, PlayerScore2, new Vector2(1080, 10f), Color.Red);
                spriteBatch.DrawString(hudFont, timeLeft, new Vector2(610, 10f), Color.BlueViolet);
            }

            else if (gameIsIn == GameState.Menu)
            {
                spriteBatch.Draw(menu, fullscreen, Color.White);

                spriteBatch.Draw(playerTex.ElementAt(1), new Rectangle(220, 385 + menuSelection * 60, playerDrawSize, playerDrawSize), Color.White);

                if (gameOver)
                {
                    int win = (Global.PlayerScores[0] >= Global.PlayerScores[1]) ? 1 : 2;
                    string Winner = string.Format("Player{0} wins!" + Global.PlayerScores[1], win);
                    spriteBatch.DrawString(hudFont, Winner, new Vector2(250, 100), Color.Firebrick);
                }


            }
            else if (gameIsIn == GameState.Paused)
            {
                spriteBatch.Draw(pauseMenu, fullscreen, Color.White);

                spriteBatch.Draw(playerTex.ElementAt(1), new Rectangle(220, 385 + menuSelection * 60, playerDrawSize, playerDrawSize), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
