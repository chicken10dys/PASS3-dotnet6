//using Animation2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

//using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace PASS3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Random number setup
        static Random rng = new Random();

        //Store max number of balls
        const int NUMBALLS = 3;
        //Store max number of bullets
        const int NUMBULLETS = 2;
        
        //General storage
        //Store colours
        Color[] randomColour = new Color[9] {Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet, Color.Magenta, Color.Aqua};
        int[] randomBallColour = new int[NUMBALLS];
        //Store random colour
        int randomColourNum;
        //Store if coloured balls is on or off
        bool colouredBalls = false;

        //Store window size
        int screenWidth;
        int screenHeight;

        //Store the scale
        double scale;

        //Store keyboard state
        KeyboardState kb;
        KeyboardState prevKb;

        //Store the mouse state
        MouseState mouse;
        MouseState prevMouse;

        //Store gamestates
        const int MENU = 0;
        const int GAME = 1;
        const int PAUSE = 2;
        const int END = 3;
        int gamestate = 0;
        
        //Store arrow directions
        const int UP = 0;
        const int DOWN = 1;
        const int LEFT = 2;
        const int RIGHT = 3;
        int selectedArrow;

        //Stuff related to sprites and their positions
        Texture2D blank;
        Texture2D playBtnImg;
        Texture2D exitBtnImg;
        Texture2D ballImg;
        
        //Store arrow sprites
        Texture2D[] arrow = new Texture2D [4];

        //Fonts
        SpriteFont font;
        
        //Font locations
        Vector2 warningLoc;
        
        //UI
        Rectangle playBtnRec;
        Rectangle exitBtnRec;
        Rectangle playAreaRec;

        //Game stuff
        //Balls and paths
        Rectangle horizontalRowRec;
        Rectangle verticalRowRec;
        Rectangle[] ballRec = new Rectangle[NUMBALLS];
        
        //Shooting related
        Rectangle[] bulletRec = new Rectangle[NUMBULLETS];
        Rectangle arrowRec;
        Rectangle centerRec;
        
        //Vector2 for fonts
        Vector2 textLoc;
        
        //Movement stuff
        Vector2[] ballPos = new Vector2[NUMBALLS];                 //Stores the ball’s true position
        float[] speed = new float [NUMBALLS];              //Stores the speed/update movement rate
        //int dirX = 1;       //Stores the x direction, which will be one of 1(right), -1(left) or 0(stopped)
        //int dirY = 0;      //Stores the y direction, which will be one of 1(down), -1(up) or 0(stopped)
        int[] dirX = new int[]  { 0, 0, 1, -1};       //Stores the x direction, which will be one of 1(right), -1(left) or 0(stopped)
        int[] dirY = new int[] { 1, -1, 0, 0};      //Stores the y direction, which will be one of 1(down), -1(up) or 0(stopped)
        
        //Store the directions and spawns for the paths
        const int TOPPATH = 0;
        const int BOTTOMPATH = 1;
        const int LEFTPATH = 2;
        const int RIGHTPATH = 3;
        Rectangle[] possibleSpawns = new Rectangle[4];
        int[] path = new int [NUMBALLS];
        
        //Store the bullet spawns
        Rectangle[] bulletSpawns = new Rectangle[4];
        
        //Store bullet movement stuff
        
        bool[] isShooting = new bool[NUMBULLETS];
        float bulletSpeed = 2f;
        Vector2[] bulletPos = new Vector2[NUMBULLETS];
        int[] bulletDir = new int[NUMBULLETS];
        
        //Store performance
        int hits;
        int miss;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            //Make window borderless
            Window.IsBorderless = true;

            //Set resolution 
            this.graphics.PreferredBackBufferWidth = 500;
            this.graphics.PreferredBackBufferHeight = 500;
            this.graphics.ApplyChanges();
            Window.AllowUserResizing = true;

            //Generate random colour
            randomColourNum = rng.Next(0,randomColour.Length);
            
            //Make mouse visible
            IsMouseVisible = true;

            //Get the window size
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            //Load sprites
            blank = Content.Load<Texture2D>("Sprites/Blank");
            playBtnImg = Content.Load<Texture2D>("Sprites/PlayBtn1");
            exitBtnImg = Content.Load<Texture2D>("Sprites/ExitBtn1");
            ballImg = Content.Load<Texture2D>("Sprites/ball");
            arrow[UP] = Content.Load<Texture2D>("Sprites/UpArrow");
            arrow[DOWN] = Content.Load<Texture2D>("Sprites/DownArrow");
            arrow[LEFT] = Content.Load<Texture2D>("Sprites/LeftArrow");
            arrow[RIGHT] = Content.Load<Texture2D>("Sprites/RightArrow");
            
            //Load fonts
            font = Content.Load<SpriteFont>("Fonts/Font");

            playBtnRec = new Rectangle(((screenWidth / 2) - (playBtnImg.Width / 2)), ((screenHeight / 2) - (playBtnImg.Height / 2) - 50), playBtnImg.Width, playBtnImg.Height);
            exitBtnRec = new Rectangle(((screenWidth / 2) - (exitBtnImg.Width / 2)), ((screenHeight / 2) - (exitBtnImg.Height / 2) + 50), exitBtnImg.Width, exitBtnImg.Height);
            playAreaRec = new Rectangle(((screenWidth - screenHeight) / 2), 0, screenHeight, screenHeight);
            horizontalRowRec = new Rectangle(((screenWidth - screenHeight) / 2), ((screenHeight / 2) - (screenHeight / 16)), screenHeight, (screenHeight / 8));
            verticalRowRec = new Rectangle(((screenWidth / 2) - (screenHeight / 16)), 0, (screenHeight / 8), screenHeight);
            
            //Load vector2
            textLoc = new Vector2(0, (int)(100 * scale));
            
            //set up balls and bullets
            for (int i = 0; i < NUMBALLS; i++)
            {
                path[i] = rng.Next(0,4);
                speed[i] = (rng.Next(10, 31) / 10);
            }
            for (int i = 0; i < NUMBULLETS; i++)
            {
                isShooting[i] = false;
            }
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            //Update keyboard states
            prevKb = kb;
            kb = Keyboard.GetState();

            //Update mouse states
            prevMouse = mouse;
            mouse = Mouse.GetState();

            //Update window size
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;
            
            //Update stuff based on gamestate
            switch (gamestate)
            {
                case MENU:

                    if (playBtnRec.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed)
                    {
                        //Generate random colour
                        randomColourNum = rng.Next(0, randomColour.Length);
                        CalcPos();
                        gamestate = GAME;
                    }
                    if (exitBtnRec.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed)
                        Exit();

                    //Change colour on right click
                    if (mouse.RightButton == ButtonState.Pressed && mouse != prevMouse)
                        randomColourNum = rng.Next(0, randomColour.Length);

                    //Logic to change window size
                    if (mouse.ScrollWheelValue > prevMouse.ScrollWheelValue)
                    {
                        this.graphics.PreferredBackBufferWidth += 20;
                        this.graphics.PreferredBackBufferHeight += 20;
                        this.graphics.ApplyChanges();
                    }
                    if (mouse.ScrollWheelValue < prevMouse.ScrollWheelValue)
                    {
                        this.graphics.PreferredBackBufferWidth -= 20;
                        this.graphics.PreferredBackBufferHeight -= 20;
                        this.graphics.ApplyChanges();
                    }
                    
                    //Update locations
                    playBtnRec = new Rectangle(((screenWidth / 2) - (playBtnImg.Width / 2)), ((screenHeight / 2) - (playBtnImg.Height / 2) - 50), playBtnImg.Width, playBtnImg.Height);
                    exitBtnRec = new Rectangle(((screenWidth / 2) - (exitBtnImg.Width / 2)), ((screenHeight / 2) - (exitBtnImg.Height / 2) + 50), exitBtnImg.Width, exitBtnImg.Height);
                    
                    warningLoc = new Vector2((screenHeight / 2) - (font.MeasureString("WARNING:\nflashing colours may cause seizure").X / 2), 0);
                    
                    break;

                case GAME:
                    //Pause game
                    if (kb.IsKeyDown(Keys.Escape) && kb != prevKb)
                    {
                        gamestate = PAUSE;
                    }
                    //Change colour when pressing space
                    if (kb.IsKeyDown(Keys.Space) && kb != prevKb)
                    {
                        randomColourNum = rng.Next(0, randomColour.Length);
                        
                    }

                        
                    
                    this.graphics.PreferredBackBufferWidth = Math.Min(screenHeight, screenWidth);
                    this.graphics.PreferredBackBufferHeight = Math.Min(screenHeight, screenWidth);
                    this.graphics.ApplyChanges();

                    if (screenWidth < 500)
                    {
                        this.graphics.PreferredBackBufferWidth = 500;
                        this.graphics.PreferredBackBufferHeight = 500;
                        this.graphics.ApplyChanges();
                    }

                    //Control arrow
                    if (kb.IsKeyDown(Keys.W))
                        selectedArrow = UP;
                    
                    if (kb.IsKeyDown(Keys.S))
                        selectedArrow = DOWN;
                    
                    if (kb.IsKeyDown(Keys.A))
                        selectedArrow = LEFT;
                    
                    if (kb.IsKeyDown(Keys.D))
                        selectedArrow = RIGHT;
                    
                    //Update ball position
                    //Calculate the speed in each direction and move the ball
                    for (int i = 0; i < NUMBALLS; i++)
                    {
                        ballPos[i].X = ballRec[i].X;
                        ballPos[i].Y = ballRec[i].Y;
                        ballPos[i].X = (float)(ballPos[i].X + (dirX[path[i]] * (speed[i] * scale)));
                        ballPos[i].Y = (float)(ballPos[i].Y + (dirY[path[i]] * (speed[i] * scale)));
                        ballRec[i].X = (int)ballPos[i].X;
                        ballRec[i].Y = (int)ballPos[i].Y;
                        
                    }
                    
                    for (int i = 0; i < NUMBULLETS; i++)
                        if(isShooting[i] == false)
                            bulletRec[i] = bulletSpawns[selectedArrow];
                    
                    //Shoot ball
                    if (kb.IsKeyDown(Keys.Space) && kb != prevKb && !prevKb.IsKeyDown(Keys.Space))
                    {
                        for (int i = 0; i < NUMBULLETS; i++)
                        {
                            if (isShooting[i] != true)
                            {
                                isShooting[i] = !isShooting[i];
                                bulletDir[i] = selectedArrow;
                                break;
                            }
                        }
                        
                    }
                    
                    
                    //Update bullet position
                    for (int i = 0; i < NUMBULLETS; i++)
                    {
                        bulletPos[i].X = bulletRec[i].X;
                        bulletPos[i].Y = bulletRec[i].Y;
                        bulletPos[i].X = (float)(bulletPos[i].X + ((dirX[bulletDir[i]] * -1) * (bulletSpeed * scale)));
                        bulletPos[i].Y = (float)(bulletPos[i].Y + ((dirY[bulletDir[i]] * -1) * (bulletSpeed * scale)));
                        bulletRec[i].X = (int)bulletPos[i].X;
                        bulletRec[i].Y = (int)bulletPos[i].Y;
                    }
                    
                    
                    if(kb.IsKeyDown(Keys.E) && kb != prevKb)
                        colouredBalls = !colouredBalls;
                    
                    
                    //Check for ball death
                    CheckBallDeath();

                    break;

                case PAUSE:
                    //unpause game
                    if (kb.IsKeyDown(Keys.Escape) && kb != prevKb)
                    {
                        gamestate = GAME;
                    }
                    break;

                case END:

                    break;

            }

                    // TODO: Add your update logic here

                    base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin();
            switch (gamestate)
            {
                case MENU:
                    GraphicsDevice.Clear(randomColour[randomColourNum]);
                    spriteBatch.Draw(playBtnImg, playBtnRec, Color.White);
                    spriteBatch.Draw(exitBtnImg, exitBtnRec, Color.White);
                    if(randomColourNum != 0)
                        spriteBatch.DrawString(font, "WARNING:\nflashing colours may cause seizure", warningLoc, Color.Red);
                    if(randomColourNum == 0)
                        spriteBatch.DrawString(font, "WARNING:\nflashing colours may cause seizure", warningLoc, Color.White);
                    break;

                case GAME:
                    GraphicsDevice.Clear(randomColour[randomColourNum]);
                    spriteBatch.Draw(blank, playAreaRec, Color.Black * 0.5f);
                    spriteBatch.Draw(blank, horizontalRowRec, Color.Black * 0.5f);
                    spriteBatch.Draw(blank, verticalRowRec, Color.Black * 0.5f);
                    if (colouredBalls == true)
                    {
                        for (int i = 0; i < NUMBALLS; i++)
                        {
                            spriteBatch.Draw(ballImg, ballRec[i], randomColour[randomBallColour[i]]);
                        }
                    }
                    if (colouredBalls == false)
                    {
                        for (int i = 0; i < NUMBALLS; i++)
                        {
                            spriteBatch.Draw(ballImg, ballRec[i], Color.White);
                        }
                    }
                    
                    spriteBatch.Draw(blank, centerRec, Color.Black);
                    spriteBatch.Draw(arrow[selectedArrow], arrowRec, randomColour[randomColourNum]);
                    for (int i = 0; i < NUMBULLETS; i++)
                        spriteBatch.Draw(ballImg, bulletRec[i], randomColour[randomColourNum]);

                    
                    //TEST
                    Console.WriteLine("Hits: " + hits);
                    Console.WriteLine("Misses: : " + miss);
                    
                    //spriteBatch.DrawString((font * scale), "test", textLoc, Color.White);
                    
                    //Draw center hitbox
                    
                    break;

                case PAUSE:
                    GraphicsDevice.Clear(Color.Black);
                    break;

                case END:

                    break;
            }
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        void CalcPos()
        {
            //Scale all the stuff based on the window size
            //Update scale
            scale = (double)(screenHeight) / 500;

            playAreaRec = new Rectangle(((screenWidth - screenHeight) / 2), 0, screenHeight, screenHeight);
            horizontalRowRec = new Rectangle(((screenWidth - screenHeight) / 2), ((screenHeight / 2) - (screenHeight / 24)), screenHeight, (screenHeight / 12));
            verticalRowRec = new Rectangle(((screenWidth / 2) - (screenHeight / 24)), 0, (screenHeight / 12), screenHeight);
            //Make a hitbox for the center
            centerRec = new Rectangle(Convert.ToInt32((screenHeight / 2) - (screenHeight / 24)), Convert.ToInt32((screenHeight / 2) - screenHeight / 24), screenHeight / 12, screenHeight / 12);

            //Balls
            possibleSpawns[0] = new Rectangle((int)((screenHeight / 2) - 8 * scale), 0, (int)(16 * scale), (int)(16 * scale));
            possibleSpawns[1] = new Rectangle((int)((screenHeight / 2) - 8 * scale), (int)(screenHeight - (16 * scale)), (int)(16 * scale), (int)(16 * scale));
            possibleSpawns[2] = new Rectangle(0, (int)((screenHeight / 2) - 8 * scale), (int)(16 * scale), (int)(16 * scale));
            possibleSpawns[3] = new Rectangle((int)(screenHeight - (16 * scale)), (int)((screenHeight / 2) - 8 * scale), (int)(16 * scale), (int)(16 * scale));
            
            //Arrow and bullets
            arrowRec = new Rectangle(Convert.ToInt32((screenHeight / 2) - (screenHeight / 24)), Convert.ToInt32((screenHeight / 2) - screenHeight / 24), screenHeight / 12, screenHeight / 12);
            bulletSpawns[0] = new Rectangle(Convert.ToInt32((screenHeight / 2) - (4 * scale)),Convert.ToInt32((screenHeight / 2) - (screenHeight / 24) - (8 * scale)),(int)(8 * scale),(int)(8 * scale));
            bulletSpawns[1] = new Rectangle(Convert.ToInt32((screenHeight / 2) - (4 * scale)),Convert.ToInt32((screenHeight / 2) + (screenHeight / 24)),(int)(8 * scale),(int)(8 * scale));
            bulletSpawns[2] = new Rectangle(Convert.ToInt32((screenHeight / 2) - (screenHeight / 24) - (8 * scale)),Convert.ToInt32((screenHeight / 2) - (int)(4 * scale)),(int)(8 * scale),(int)(8 * scale));
            bulletSpawns[3] = new Rectangle(Convert.ToInt32((screenHeight / 2) + (screenHeight / 24) ),Convert.ToInt32((screenHeight / 2) - (4 * scale)),(int)(8 * scale),(int)(8 * scale));
            

            for (int i = 0; i < NUMBALLS; i++)
            {
                //Generate a spawn for each ball
                ballRec[i] = possibleSpawns[path[i]];
                
                //Scale the speed
                //speed[i] = (float)(speed[i] * scale);
                //bulletSpeed = (float)(bulletSpeed * scale);
                
                //Generate a random colour for each ball
                randomBallColour[i] = rng.Next(0,randomColour.Length);
            }
        }

        void CheckBallDeath()
        {
            
            for (int i = 0; i < NUMBALLS; i++)
            {
                //Check for a miss and reset the ball
                if (centerRec.Contains(ballRec[i]))
                {
                    //Regenerate random spawn
                    path[i] = rng.Next(0,4);
                    //Set ball rec to new random spawn
                    ballRec[i] = possibleSpawns[path[i]];
                    //Regenerate random colour
                    randomColourNum = rng.Next(0,randomColour.Length);
                    //Regenerate random speed
                    speed[i] = (rng.Next(10, 31) / 10);
                    //Scale the speed
                    //speed[i] = (float)(speed[i] * scale);
                    //Regenerate a random colour for each ball
                    for (int ii = 0; ii < NUMBALLS; ii++)
                        randomBallColour[ii] = rng.Next(0,randomColour.Length);
                    miss ++;
                }

                //Check for a hit and reset both the bullet and the ball
                for (int ii = 0; ii < NUMBULLETS; ii++)
                {
                    if (ballRec[i].Contains(bulletRec[ii]) && isShooting[ii] == true)
                    {
                        isShooting[ii] = false;
                        
                        //Regenerate random spawn
                        path[i] = rng.Next(0,4);
                        //Set ball rec to new random spawn
                        ballRec[i] = possibleSpawns[path[i]];
                        //Regenerate random colour
                        randomColourNum = rng.Next(0,randomColour.Length);
                        //Regenerate random speed
                        speed[i] = (rng.Next(10, 31) / 10);
                        //Scale the speed
                        //speed[i] = (float)(speed[i] * scale);
                        //Regenerate a random colour for each ball
                        for (int iii = 0; iii < NUMBALLS; iii++)
                            randomBallColour[iii] = rng.Next(0,randomColour.Length);
                        hits ++;
                    }
                }
            }

            for (int i = 0; i < NUMBULLETS; i++)
            {
                if(bulletRec[i].X > screenHeight || bulletRec[i].X < 0 || bulletRec[i].Y > screenHeight || bulletRec[i].Y < 0)
                {
                    isShooting[i] = false;
                }
            }
        }

    }
}
