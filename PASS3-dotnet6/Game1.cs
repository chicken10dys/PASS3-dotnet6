using Animation2D;
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
        //Make a spritebatch with no anti aliasing 
        private SpriteBatch antiAlias;

        //Random number setup
        static Random rng = new Random();

        //Store max number of balls
        const int MAXNUMBALLS = 10;
        int NUMBALLS = 1;
        //Store max number of bullets
        const int NUMBULLETS = 2;
        //Store origional lives
        const int LIVES = 5;
        
        //General storage
        //Store colours
        Color[] randomColour = new Color[9] {Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet, Color.Magenta, Color.Aqua};
        int[] randomBallColour = new int[MAXNUMBALLS];
        //Store random colour
        int randomColourNum;
        //Store if coloured balls is on or off
        bool colouredBalls = false;

        //Store window size
        int screenWidth;
        int screenHeight;

        //Store the scale
        float scale;
        
        //Store the final screen size
        int finalScreenSize;

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
        const int INSTRUCTIONS = 4;
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
        Texture2D instructionsBtnImg;
        Texture2D instructionsImg;
        Texture2D ballImg;
        Texture2D healthImg;
        
        //Textures for numbers
        Texture2D[] NumImg = new Texture2D[10];
        
        //Store arrow sprites
        Texture2D[] arrow = new Texture2D [4];

        //Easter egg
        Texture2D easterEggImg;
        
        Animation easterEggAnim;
        
        //Fonts
        SpriteFont font;

        //UI
        Rectangle playBtnRec;
        Rectangle exitBtnRec;
        Rectangle instructionsBtnRec;
        Rectangle instructionsRec;
        Rectangle exitToMenuBtnRec;
        Rectangle playAreaRec;
        Rectangle[] healthDisplayRec = new Rectangle[LIVES];
        Rectangle[] pointsDisplayRec = new Rectangle[3];

        //Game stuff
        //Balls and paths
        Rectangle horizontalRowRec;
        Rectangle verticalRowRec;
        Rectangle[] ballRec = new Rectangle[MAXNUMBALLS];
        
        //Shooting related
        Rectangle[] bulletRec = new Rectangle[NUMBULLETS];
        Rectangle arrowRec;
        Rectangle centerRec;
        
        //Easter egg
        Vector2 easterEggPos;
        
        //Vector2 for fonts
        Vector2 warningLoc;
        Vector2 deathMsgLoc;
        Vector2[] deathPointsLoc = new Vector2[2];
        Vector2 restartMsgLoc;
        Vector2[] pauseMsgLoc = new Vector2[2];
        Vector2 easterEggMsgLoc;
        
        //Movement stuff
        Vector2[] ballPos = new Vector2[MAXNUMBALLS];                 //Stores the ball’s true position
        float[] speed = new float [MAXNUMBALLS];              //Stores the speed/update movement rate
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
        int[] path = new int [MAXNUMBALLS];
        
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
        int lives = LIVES; 

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
            instructionsBtnImg = Content.Load<Texture2D>("Sprites/InstructionsBtn1");
            instructionsImg = Content.Load<Texture2D>("Sprites/Instructions");
            ballImg = Content.Load<Texture2D>("Sprites/ball");
            arrow[UP] = Content.Load<Texture2D>("Sprites/UpArrow");
            arrow[DOWN] = Content.Load<Texture2D>("Sprites/DownArrow");
            arrow[LEFT] = Content.Load<Texture2D>("Sprites/LeftArrow");
            arrow[RIGHT] = Content.Load<Texture2D>("Sprites/RightArrow");
            easterEggImg = Content.Load<Texture2D>("Sprites/69");
            healthImg = Content.Load<Texture2D>("Sprites/Heart");
            
            //Load number textures
            for (int i = 0; i < NumImg.Length; i++)
            {
                NumImg[i] = Content.Load<Texture2D>("Numbers/" + i);
            }
            
            //Load fonts
            font = Content.Load<SpriteFont>("Fonts/Font");

            playBtnRec = new Rectangle(((screenWidth / 2) - (playBtnImg.Width / 2)), ((screenHeight / 2) - (playBtnImg.Height / 2) - 50), playBtnImg.Width, playBtnImg.Height);
            exitBtnRec = new Rectangle(((screenWidth / 2) - (exitBtnImg.Width / 2)), ((screenHeight / 2) - (exitBtnImg.Height / 2) + 50), exitBtnImg.Width, exitBtnImg.Height);
            playAreaRec = new Rectangle(((screenWidth - screenHeight) / 2), 0, screenHeight, screenHeight);
            horizontalRowRec = new Rectangle(((screenWidth - screenHeight) / 2), ((screenHeight / 2) - (screenHeight / 16)), screenHeight, (screenHeight / 8));
            verticalRowRec = new Rectangle(((screenWidth / 2) - (screenHeight / 16)), 0, (screenHeight / 8), screenHeight);
            
            //Set up easter egg animation position
            easterEggPos = new Vector2(-10,0);

            Setup();
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
            //Make a spritebatch with no anti aliasing 
            antiAlias = new SpriteBatch(GraphicsDevice);;

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
            
            //Update animations
            
            
            //Update stuff based on gamestate
            switch (gamestate)
            {
                case MENU:
                    //Switch gamestates
                    if (playBtnRec.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed || kb.IsKeyDown(Keys.P) && kb != prevKb && !prevKb.IsKeyDown(Keys.P))
                    {
                        //Generate random colour
                        randomColourNum = rng.Next(0, randomColour.Length);
                        CalcPos();
                        gamestate = GAME;
                    }
                    if (exitBtnRec.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed || kb.IsKeyDown(Keys.Escape) && kb != prevKb && !prevKb.IsKeyDown(Keys.Escape))
                        Exit();
                    if (instructionsBtnRec.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed || kb.IsKeyDown(Keys.I) && kb != prevKb && !prevKb.IsKeyDown(Keys.I))
                        gamestate = INSTRUCTIONS;
                    //Change colour on right click
                    if (mouse.RightButton == ButtonState.Pressed && mouse != prevMouse || kb.IsKeyDown(Keys.C) && kb != prevKb && !prevKb.IsKeyDown(Keys.C))
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
                    playBtnRec = new Rectangle(((screenWidth / 2) - (playBtnImg.Width / 2)), ((screenHeight / 2) - (playBtnImg.Height / 2) - (exitBtnImg.Height + 25)), playBtnImg.Width, playBtnImg.Height);
                    exitBtnRec = new Rectangle(((screenWidth / 2) - (exitBtnImg.Width / 2)), ((screenHeight / 2) - (exitBtnImg.Height / 2)), exitBtnImg.Width, exitBtnImg.Height);
                    instructionsBtnRec = new Rectangle(((screenWidth / 2) - (instructionsBtnImg.Width / 2)), ((screenHeight / 2) - (instructionsBtnImg.Height / 2) + (exitBtnImg.Height + 25)), instructionsBtnImg.Width, instructionsBtnImg.Height);

                    //Center text
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
                        ballPos[i].X = ballPos[i].X + (dirX[path[i]] * (speed[i] * scale));
                        ballPos[i].Y = ballPos[i].Y + (dirY[path[i]] * (speed[i] * scale));
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
                        bulletPos[i].X = bulletPos[i].X + ((dirX[bulletDir[i]] * -1) * (bulletSpeed * scale));
                        bulletPos[i].Y = bulletPos[i].Y + ((dirY[bulletDir[i]] * -1) * (bulletSpeed * scale));
                        bulletRec[i].X = (int)bulletPos[i].X;
                        bulletRec[i].Y = (int)bulletPos[i].Y;
                    }
                    
                    
                    if(kb.IsKeyDown(Keys.E) && kb != prevKb)
                        colouredBalls = !colouredBalls;

                    if (hits >= 5 && NUMBALLS == 1)
                        NewBall();
                    if (hits >= 20 && NUMBALLS == 2)
                        NewBall();
                    if (hits >= 50 && NUMBALLS == 3)
                        NewBall();
                    if (hits >= 200 && NUMBALLS == 4)
                        NewBall();
                    
                    
                    //Check for ball death
                    CheckBallDeath();
                    
                    //Check for game over
                    if(lives <= 0)
                    //if(hits == 69)
                        gamestate = END;
                    
                    break;

                case PAUSE:
                    //unpause game
                    if (kb.IsKeyDown(Keys.Escape) && kb != prevKb)
                    {
                        gamestate = GAME;
                    }
                    pauseMsgLoc[0] = new Vector2((screenHeight / 2) - (font.MeasureString("Game paused").X / 2), (screenHeight / 2) - (font.MeasureString("Game paused").Y));
                    pauseMsgLoc[1] = new Vector2((screenHeight / 2) - (font.MeasureString("Press escape to resume").X / 2), (screenHeight / 2));
                    break;

                case END:
                    //Set screensize
                    this.graphics.PreferredBackBufferWidth = 500;
                    this.graphics.PreferredBackBufferHeight = 500;
                    this.graphics.ApplyChanges();
                    
                    //Update animations
                    easterEggAnim.Update(gameTime);
                    //Update locations based on score to keep everything centered 
                    deathMsgLoc = new Vector2((screenHeight / 2) - (font.MeasureString("GAME OVER").X / 2), (screenHeight / 4)- (font.MeasureString("GAME OVER").Y / 2));
                    deathPointsLoc[0] = new Vector2((screenHeight / 2) - (font.MeasureString("You got " + hits + " ball!").X / 2), (screenHeight / 2) - (font.MeasureString("You got " + hits + " ball!").Y / 2));
                    deathPointsLoc[1] = new Vector2((screenHeight / 2) - (font.MeasureString("You got " + hits + " balls!").X / 2), (screenHeight / 2) - (font.MeasureString("You got " + hits + " balls!").Y / 2));
                    restartMsgLoc = new Vector2((screenHeight / 2) - (font.MeasureString("Press R to restart or Escape to exit").X / 2), (screenHeight / 4) * 3  - (font.MeasureString("Press R to restart or Escape to exit").Y / 2));
                    easterEggMsgLoc = new Vector2((screenHeight / 2) - (font.MeasureString("HAHA YOU GOT FUNNY NUMBER").X / 2), screenHeight  - font.MeasureString("HAHA YOU GOT FUNNY NUMBER").Y);
                    //Check for restart
                    if (kb.IsKeyDown(Keys.R) && kb != prevKb && !prevKb.IsKeyDown(Keys.R))
                    {
                        Setup();
                        this.graphics.PreferredBackBufferWidth = finalScreenSize;
                        this.graphics.PreferredBackBufferHeight = finalScreenSize;
                        this.graphics.ApplyChanges();
                        gamestate = GAME;
                    }
                        
                    if (kb.IsKeyDown(Keys.Escape) && kb != prevKb && !prevKb.IsKeyDown(Keys.Escape))
                        Exit();
                    break;
                
                case INSTRUCTIONS:
                    //Update scale
                    scale = screenHeight / 500f;
                    instructionsRec = new Rectangle (0, 0, screenHeight, screenHeight);
                    exitToMenuBtnRec =  new Rectangle((int)(screenHeight - (exitBtnImg.Width * scale)), (int)(screenHeight - (exitBtnImg.Height * scale)), (int)(exitBtnImg.Width * scale), (int)(exitBtnImg.Height * scale));
                    if (exitToMenuBtnRec.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed || kb.IsKeyDown(Keys.Escape) && kb != prevKb && !prevKb.IsKeyDown(Keys.Escape))
                        gamestate = MENU;
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
            
            
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            antiAlias.Begin();
            GraphicsDevice.Clear(randomColour[randomColourNum]);
            switch (gamestate)
            {
                case MENU:
                    GraphicsDevice.Clear(randomColour[randomColourNum]);
                    spriteBatch.Draw(playBtnImg, playBtnRec, Color.White);
                    spriteBatch.Draw(exitBtnImg, exitBtnRec, Color.White);
                    spriteBatch.Draw(instructionsBtnImg, instructionsBtnRec, Color.White);
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
                    antiAlias.Draw(arrow[selectedArrow], arrowRec, randomColour[randomColourNum]);
                    for (int i = 0; i < NUMBULLETS; i++)
                        spriteBatch.Draw(ballImg, bulletRec[i], randomColour[randomColourNum]);
                    
                    //Draw health
                    for (int i = 0; i < LIVES; i++)
                    {
                        spriteBatch.Draw(healthImg, healthDisplayRec[i], Color.White);
                        spriteBatch.Draw(healthImg, healthDisplayRec[i], Color.Black * 0.75f);
                    }
                        
                    for (int i = 0; i < lives; i++)
                        spriteBatch.Draw(healthImg, healthDisplayRec[i], Color.White); 
                    //Draw points
                    if(hits < 10)
                        spriteBatch.Draw(NumImg[Convert.ToInt32((Convert.ToString(hits)[0]).ToString())], pointsDisplayRec[2], Color.White);
                    if (hits >= 10 && hits < 100)
                    {
                        spriteBatch.Draw(NumImg[Convert.ToInt32((Convert.ToString(hits)[0]).ToString())], pointsDisplayRec[1], Color.White);
                        spriteBatch.Draw(NumImg[Convert.ToInt32((Convert.ToString(hits)[1]).ToString())], pointsDisplayRec[2], Color.White);
                    }

                    if (hits >= 100)
                    {
                        spriteBatch.Draw(NumImg[Convert.ToInt32((Convert.ToString(hits)[0]).ToString())], pointsDisplayRec[0], Color.White);
                        spriteBatch.Draw(NumImg[Convert.ToInt32((Convert.ToString(hits)[1]).ToString())], pointsDisplayRec[1], Color.White);
                        spriteBatch.Draw(NumImg[Convert.ToInt32((Convert.ToString(hits)[2]).ToString())], pointsDisplayRec[2], Color.White);

                    }
                    //TEST
                    /*Console.WriteLine("Hits: " + hits);
                    Console.WriteLine("Misses: : " + miss);
                    Console.WriteLine("Lives: : " + lives);*/
                    
                    if(hits > 0)
                        Console.Write(Convert.ToString(hits)[0]);
                    if(hits > 10)
                        Console.Write(Convert.ToString(hits)[1]);
                    if(hits > 100)
                        Console.Write(Convert.ToString(hits)[2]);
                    Console.WriteLine();
                    break;
                
                case PAUSE:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.DrawString(font, "Game paused", pauseMsgLoc[0], Color.White);
                    spriteBatch.DrawString(font, "Press escape to resume", pauseMsgLoc[1], Color.White);
                    break;

                case END:
                    GraphicsDevice.Clear(Color.Black);
                    if (hits == 69)
                    {
                        easterEggAnim.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
                        spriteBatch.DrawString(font, "HAHA YOU GOT FUNNY NUMBER", easterEggMsgLoc, Color.Black);
                    }
                    spriteBatch.DrawString(font, "GAME OVER", deathMsgLoc, Color.Red);
                    if(hits == 1)
                        spriteBatch.DrawString(font, "You got " + hits + " ball!", deathPointsLoc[0], Color.White);
                    else
                        spriteBatch.DrawString(font, "You got " + hits + " balls!", deathPointsLoc[1], Color.White);
                    spriteBatch.DrawString(font, "Press R to restart or Escape to exit", restartMsgLoc, Color.Red);
                    
                    break;
                
                case INSTRUCTIONS:
                    GraphicsDevice.Clear(Color.Beige);
                    spriteBatch.Draw(instructionsImg, instructionsRec, Color.White);
                    spriteBatch.Draw(exitBtnImg, exitToMenuBtnRec, Color.White);
                    break;
            }
            antiAlias.End();
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        //This subprogram will set the scale and scaled position of game assets 
        void CalcPos()
        {
            //Scale all the stuff based on the window size
            //Update scale
            scale = screenHeight / 500f;

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
            
            //Health display
            for (int i = 0; i < LIVES; i++)
                healthDisplayRec[i] = new Rectangle((int)((32 * scale) * i), 0, Convert.ToInt32(32 * scale), Convert.ToInt32(32 * scale));
            
            //Animations
            easterEggAnim = new Animation(easterEggImg, 5, 9, 44, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 5, easterEggPos, 1.4f, true);
            
            //For the end gamestate 
            //Set locations to avoid glitching on the first death
            deathMsgLoc = new Vector2((screenHeight / 2) - (font.MeasureString("GAME OVER").X / 2), (screenHeight / 4)- (font.MeasureString("GAME OVER").Y / 2));
            deathPointsLoc[0] = new Vector2((screenHeight / 2) - (font.MeasureString("You got " + hits + " ball!").X / 2), (screenHeight / 2) - (font.MeasureString("You got " + hits + " ball!").Y / 2));
            deathPointsLoc[1] = new Vector2((screenHeight / 2) - (font.MeasureString("You got " + hits + " balls!").X / 2), (screenHeight / 2) - (font.MeasureString("You got " + hits + " balls!").Y / 2));
            restartMsgLoc = new Vector2((screenHeight / 2) - (font.MeasureString("Press R to restart or Escape to exit").X / 2), (screenHeight / 4) * 3  - (font.MeasureString("Press R to restart or Escape to exit").Y / 2));
            easterEggMsgLoc = new Vector2((screenHeight / 2) - (font.MeasureString("HAHA YOU GOT FUNNY NUMBER").X / 2), screenHeight  - font.MeasureString("HAHA YOU GOT FUNNY NUMBER").Y);

            //Store number positions
            pointsDisplayRec[0] = new Rectangle((int)(screenHeight - (22 * scale) * 3),0,(int)(22 * scale),(int)(32 * scale));
            pointsDisplayRec[1] = new Rectangle((int)(screenHeight - (22 * scale) * 2),0,(int)(22 * scale),(int)(32 * scale));
            pointsDisplayRec[2] = new Rectangle((int)(screenHeight - (22 * scale)),0,(int)(22 * scale),(int)(32 * scale));
            //Store the final screen size
            finalScreenSize = screenHeight;
            
            for (int i = 0; i < NUMBALLS; i++)
            {
                //Generate a spawn for each ball
                ballRec[i] = possibleSpawns[path[i]];

                //Generate a random colour for each ball
                randomBallColour[i] = rng.Next(0,randomColour.Length);
            }
        }

        //This subprogram will check for ball death
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
                    speed[i] = rng.Next(10, 21) / 10f;
                    //Regenerate a random colour for each ball
                    for (int ii = 0; ii < NUMBALLS; ii++)
                        randomBallColour[ii] = rng.Next(0,randomColour.Length);
                    miss ++;
                    lives --;
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
                        speed[i] = rng.Next(10, 21) / 10f;
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

        //This subprogram will set up and reset the game
        void Setup()
        {
            //Reset stats
            hits = 0;
            miss = 0;
            lives = LIVES;
            NUMBALLS = 1;
            
            //set up balls and bullets
            for (int i = 0; i < NUMBALLS; i++)
            {
                //Regenerate random spawn
                path[i] = rng.Next(0,4);
                //Set ball rec to new random spawn
                ballRec[i] = possibleSpawns[path[i]];
                //Regenerate random colour
                randomColourNum = rng.Next(0,randomColour.Length);
                //Regenerate random speed
                speed[i] = rng.Next(10, 21) / 10f;
                //Regenerate a random colour for each ball
                for (int ii = 0; ii < NUMBALLS; ii++)
                    randomBallColour[ii] = rng.Next(0,randomColour.Length);
            }
            for (int i = 0; i < NUMBULLETS; i++)
            {
                isShooting[i] = false;
            }
        }

        //This subprogram will add and set up new balls
        void NewBall()
        {
            NUMBALLS ++;
            //Generate random spawn
            path[NUMBALLS -1] = rng.Next(0,4);
            //Set ball rec to new random spawn
            ballRec[NUMBALLS -1] = possibleSpawns[path[NUMBALLS -1]];
            //Generate random colour
            randomColourNum = rng.Next(0,randomColour.Length);
            //Generate random speed
            speed[NUMBALLS -1] = rng.Next(10, 21) / 10f;
            //Generate a random colour for each ball
            for (int ii = 0; ii < NUMBALLS; ii++)
                randomBallColour[ii] = rng.Next(0,randomColour.Length);
        }

    }
}
