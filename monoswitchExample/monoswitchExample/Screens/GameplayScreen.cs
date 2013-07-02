#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace monoswitchExample
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {

        #region members

            #region public

                public const int MAX_STARS = 3;

            #endregion

            #region protected

                //gamestatemanagement obsolete variables
                //protected Vector2 m_enemyPosition = new Vector2(100, 100);
                //protected Vector2 m_playerPosition = new Vector2(100, 100);

                //statemanagement
                protected ContentManager m_content;
                protected SpriteFont m_gameFont;
                protected Random m_random = new Random();
                protected float m_pauseAlpha;

                //game objects

                //obsolete variables
                /*
                // Keyboard states used to determine key presses
                //KeyboardState currentKeyboardState;
                //KeyboardState previousKeyboardState;
                // Gamepad states used to determine button presses
                //GamePadState currentGamePadState;
                //GamePadState previousGamePadState;
                // A movement speed for the player
                //float playerMoveSpeed;
                // Image used to display the static background
                //Texture2D mainBackground;
                // Parallaxing Layers
                //parallaxingbackground bgLayer1;
                //parallaxingbackground bgLayer2;
                // Enemies
                // The rate at which the enemies appear
                //TimeSpan enemySpawnTime;
                //TimeSpan previousSpawnTime;
                //Texture2D projectileTexture;
                //List<Projectile> projectiles;
                // The rate of fire of the player laser
                //TimeSpan fireTime;
                //TimeSpan previousFireTime;
                //Texture2D explosionTexture;
                //List<Animation> explosions;
                // The sound that is played when a laser is fired
                //SoundEffect laserSound;
                // The sound used when the player or an enemy dies
                //SoundEffect explosionSound;
                // The music played during gameplay
                //Song gameplayMusic;
                //Number that holds the player score
                */
        
                protected int m_score;
                protected Texture2D m_starTexture;
                //protected Animation m_starAnimation;
                protected List<star> m_stars;
                protected Player m_player;

            #endregion

            #region private

                //hackyness.
                private bool m_inited;

            #endregion

        #endregion

        #region properties

            #region public

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region events

            #region public

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region functions

            #region public

                /// <summary>
                /// Constructor.
                /// </summary>
                public GameplayScreen()
                {
                    this.m_transitionOnTime = TimeSpan.FromSeconds(1.5);
                    this.m_transitionOffTime = TimeSpan.FromSeconds(0.5);
                    this.m_inited = false;
                }

                /// <summary>
                /// Load graphics content for the game.
                /// </summary>
                public override void LoadContent()
                {
                    if (!this.m_inited)
                    {
                        this.Initialize();
                    }
                    if (this.m_content == null)
                    {
                        this.m_content = new ContentManager(ScreenManager.Game.Services, "Content");
                    }
                    this.m_gameFont = this.m_content.Load<SpriteFont>("gamefont");
                    // Load the player resources
                    Animation playerAnimation = new Animation();
                    Texture2D playerTexture = this.m_content.Load<Texture2D>("shipAnimation");
                    playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);
                    Vector2 playerPosition = new Vector2(this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X+this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width/2, this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
                    //  - playerAnimation.frameWidth/2
                    // - playerAnimation.frameHeight /2
                    this.m_player.Initialize(playerAnimation, playerPosition, this.ScreenManager.GraphicsDevice.Viewport);
                    this.m_starTexture = this.m_content.Load<Texture2D>("mineAnimation");
                    this.repopulateStars();
                    //this.m_starAnimation = new Animation();
                    // A real game would probably have more content than this sample, so
                    // it would take longer to load. We simulate that by delaying for a
                    // while, giving you a chance to admire the beautiful loading screen.
                    Thread.Sleep(1000);
                    // once the load has finished, we use ResetElapsedTime to tell the game's
                    // timing mechanism that we have just finished a very long frame, and that
                    // it should not try to catch up.
                    ScreenManager.Game.ResetElapsedTime();
                }

                

                /// <summary>
                /// Unload graphics content used by the game.
                /// </summary>
                public override void UnloadContent()
                {
                    this.m_content.Unload();
                }

                /// <summary>
                /// Updates the state of the game. This method checks the GameScreen.IsActive
                /// property, so the game will stop updating when the pause menu is active,
                /// or if you tab away to a different application.
                /// </summary>
                public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
                {
                    base.Update(gameTime, otherScreenHasFocus, false);
                    // Gradually fade in or out depending on whether we are covered by the pause screen.
                    if (coveredByOtherScreen)
                    {
                        this.m_pauseAlpha = Math.Min(this.m_pauseAlpha + 1f / 32, 1);
                    }
                    else
                    {
                        this.m_pauseAlpha = Math.Max(this.m_pauseAlpha - 1f / 32, 0);
                    }
                    UpdatePlayer(gameTime);
                    UpdateStars(gameTime);
                    // Update the collision
                    UpdateCollision();

                }

                /// <summary>
                /// Lets the game respond to player input. Unlike the Update method,
                /// this will only be called when the gameplay screen is active.
                /// </summary>
                public override void HandleInput(InputState input)
                {
                    if (input == null)
                    {
                        throw new ArgumentNullException("input");
                    }
                    // Look up inputs for the active player profile.
                    int playerIndex = (int)this.controllingPlayer.Value;
                    KeyboardState keyboardState = input.currentKeyboardStates[playerIndex];
                    GamePadState gamePadState = input.currentGamePadStates[playerIndex];
                    // The game pauses either if the user presses the pause button, or if
                    // they unplug the active gamepad. This requires us to keep track of
                    // whether a gamepad was ever plugged in, because we don't want to pause
                    // on PC if they are playing with a keyboard and have no gamepad at all!
                    bool gamePadDisconnected = !gamePadState.IsConnected && input.gamePadWasConnected[playerIndex];
                    if (input.IsPauseGame(this.controllingPlayer) || gamePadDisconnected)
                    {
                        ScreenManager.AddScreen(new PauseMenuScreen(), this.controllingPlayer);
                    }
                    else
                    {
                        //have the player handle input
                        this.m_player.HandleInput(input.currentKeyboardStates[playerIndex], input.currentGamePadStates[playerIndex], (!gamePadDisconnected && gamePadState.IsConnected));
                    }
                }

                private void UpdatePlayer(GameTime gameTime)
                {
                    this.m_player.Update(gameTime);
                    if (this.m_player.health <= 0)
                    {
                        this.m_player.health = 100;
                        this.m_score = 0;
                    }
                }

                private void UpdateStars(GameTime gameTime)
                {
                    this.repopulateStars();//get more stars
                    // Update the Enemies
                    for (int i = this.m_stars.Count - 1; i >= 0; i--)
                    {
                        this.m_stars[i].Update(gameTime);

                        if (this.m_stars[i].active == false)
                        {
                            // If not active and health <= 0
                            if (this.m_stars[i].health <= 0)
                            {
                                //Add to the player's score
                                this.m_score += this.m_stars[i].value;
                            }
                            this.m_stars.RemoveAt(i);
                        }
                    }
                }

                private void UpdateCollision()
                {
                    // Use the Rectangle's built-in intersect function to 
                    // determine if two objects are overlapping
                    Rectangle rectanglePlayer;
                    Rectangle rectangleOutPlayer = new Rectangle();
                    Rectangle rectangleStar;
                    Rectangle rectangleOutStar = new Rectangle();
                    // Only create the rectangle once for the player
                    rectanglePlayer = new Rectangle((int)this.m_player.position.X, (int)this.m_player.position.Y, this.m_player.Width, this.m_player.Height);
                    if (this.m_player.iOOB)
                    {
                        rectangleOutPlayer = new Rectangle();//fill in the stuff later
                    }
                    //unless the player is farther out
                    /*
                    //do outside collision detection here
                    */
                    // Do the collision between the player and the enemies
                    for (int i = 0; i < this.m_stars.Count; i++)
                    {
                        rectangleStar = new Rectangle((int)this.m_stars[i].position.X,
                        (int)this.m_stars[i].position.Y, this.m_stars[i].Width, this.m_stars[i].Height);

                        // Determine if the two objects collided with each
                        // other
                        if (rectanglePlayer.Intersects(rectangleStar) || (this.m_player.iOOB && rectangleOutPlayer.Intersects(rectangleStar)))
                        {
                            // Since the enemy collided with the player
                            // destroy it
                            this.m_stars[i].health = 0;
                            // If the player health is less than zero we died
                            if (this.m_player.health <= 0)
                            {
                                this.m_player.active = false;
                            }
                        }
                        else if (this.m_stars[i].isOutOfBounds)
                        {
                            rectangleOutStar = new Rectangle();//fill in the stuff later
                            if (rectanglePlayer.Intersects(rectangleOutStar) || (this.m_player.iOOB && rectanglePlayer.Intersects(rectangleOutStar)))
                            {
                                // Since the enemy collided with the player
                                // destroy it
                                this.m_stars[i].health = 0;
                                // If the player health is less than zero we died
                                if (this.m_player.health <= 0)
                                {
                                    this.m_player.active = false;
                                }
                            }
                        }

                    }
                }
                /// <summary>
                /// Draws the gameplay screen.
                /// </summary>
                public override void Draw(GameTime gameTime)
                {
                    // This game has a blue background. Why? Because!
                    ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);
                    // Our player and enemy are both actually just text strings.
                    SpriteBatch spriteBatch = ScreenManager.spriteBatch;
                    //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                    spriteBatch.Begin();
                    this.m_player.Draw(spriteBatch);
                    foreach (star sVal in this.m_stars)//draw the stars
                    {
                        sVal.Draw(spriteBatch);
                    }
                    spriteBatch.End();
                    // If the game is transitioning on or off, fade it out to black.
                    if (this.m_transitionPosition > 0 || this.m_pauseAlpha > 0)
                    {
                        float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, this.m_pauseAlpha / 2);
                        ScreenManager.FadeBackBufferToBlack(alpha);
                    }
                }

            #endregion

            #region protected

                protected void Initialize()
                {
                    //Set player's score to zero
                    this.m_score = 0;
                    this.m_player = new Player();
                    // Initialize the enemies list
                    this.m_stars = new List<star>();
                    // Initialize our random number generator
                    this.m_random = new Random();
                    //base.Initialize();
                }

                protected void repopulateStars()
                {
                    while (this.m_stars.Count < GameplayScreen.MAX_STARS)
                    {
                        star sVal = new star();
                        while (!this.reroll(ref sVal))
                        {
                        }
                        this.m_stars.Add(sVal);
                    }
                }

                protected bool reroll(ref star sVal)
                {
                    float xloc = this.m_random.Next(this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width) + this.ScreenManager.GraphicsDevice.Viewport.X;
                    float yloc = this.m_random.Next(this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height) + this.ScreenManager.GraphicsDevice.Viewport.Y;
                    Vector2 loc = new Vector2(xloc, yloc);
                    Animation anim = new Animation();
                    anim.Initialize(this.m_starTexture, loc, 47, 61, 8, 30,Color.White, 1f, true);
                    sVal.Initialize(anim, loc);
                    foreach (star sVal2 in this.m_stars)
                    {
                        if(safeCircleIntersects(sVal2, sVal))
                        {
                            return false;
                        }
                    }
                    if (safeCircleIntersects(sVal, this.m_player))
                    {
                        return false;
                    }
                    return true;
                }

                protected bool safeCircleIntersects(star sVal1, star sVal2)
                {
                    return this.circleIntersects(sVal1, sVal2, 1.2f);
                }

                protected bool safeCircleIntersects(star sVal1, Player pVal1)
                {
                    return this.circleIntersects(sVal1, pVal1, 1.2f);
                }

                protected bool closeCircleIntersects(star sVal1, star sVal2)
                {
                    return this.circleIntersects(sVal1, sVal2, 0.65f);
                }

                protected bool closeCircleIntersects(star sVal1, Player pVal1)
                {
                    return this.circleIntersects(sVal1, pVal1, 0.65f);
                }

                protected bool circleIntersects(star sVal1, star sVal2, float scale)
                {
                    //float midX = this.ScreenManager.GraphicsDevice.Viewport.X + this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width / 2;
                    float midX = this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width / 2;
                    //float midY = this.ScreenManager.GraphicsDevice.Viewport.Y + this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height / 2;
                    float midY = this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height / 2;
                    float dif = Math.Abs(sVal1.radius * scale) + Math.Abs(sVal2.radius * scale);
                    float standardX1 = sVal1.position.X - this.ScreenManager.GraphicsDevice.Viewport.X;
                    float standardX2 = sVal2.position.X - this.ScreenManager.GraphicsDevice.Viewport.X;
                    float standardY1 = sVal1.position.Y - this.ScreenManager.GraphicsDevice.Viewport.Y;
                    float standardY2 = sVal2.position.Y - this.ScreenManager.GraphicsDevice.Viewport.Y;
                    float altX1;
                    float altX2;
                    float altY1;
                    float altY2;
                    if(standardX1 < midX)
                    {
                        altX1 = standardX1 + this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    else
                    {
                        altX1 = standardX1 - this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    if(standardX2 < midX)
                    {
                        altX2 = standardX2 + this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    else
                    {
                        altX2 = standardX2 - this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    if(standardY1 < midY)
                    {
                        altY1 = standardY1 + this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    else
                    {
                        altY1 = standardY1 - this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    if(standardY2 < midY)
                    {
                        altY2 = standardY2 + this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    else
                    {
                        altY2 = standardY2 - this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    float tbase = 0f;
                    float taltitude = 0f;
                    float hyp = 0f;
                    //compare standard 1 to standard 2
                    tbase = standardX1-standardX2;
                    taltitude = standardY1-standardY2;
                    hyp = (float)Math.Sqrt(Math.Pow(tbase, 2) + Math.Pow(taltitude, 2));
                    if (hyp < dif)
                    {
                        return true;
                    }
                    //compare standard 1 to alt 2
                    tbase = standardX1 - altX2;
                    taltitude = standardY1 - altY2;
                    hyp = (float)Math.Sqrt(Math.Pow(tbase, 2) + Math.Pow(taltitude, 2));
                    if (hyp < dif)
                    {
                        return true;
                    }
                    //compare alt 1 to standard 2
                    tbase = altX1 - standardX2;
                    taltitude = altY1 - standardY2;
                    hyp = (float)Math.Sqrt(Math.Pow(tbase, 2) + Math.Pow(taltitude, 2));
                    if (hyp < dif)
                    {
                        return true;
                    }
                    return false;
                }

                protected bool circleIntersects(star sVal1, Player pVal1, float scale)
                {
                    //float midX = this.ScreenManager.GraphicsDevice.Viewport.X + this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width / 2;
                    float midX = this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width / 2;
                    //float midY = this.ScreenManager.GraphicsDevice.Viewport.Y + this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height / 2;
                    float midY = this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height / 2;
                    float dif = Math.Abs(sVal1.radius * scale) + Math.Abs(pVal1.radius * scale);
                    float standardX1 = sVal1.position.X - this.ScreenManager.GraphicsDevice.Viewport.X;
                    float standardX2 = pVal1.position.X - this.ScreenManager.GraphicsDevice.Viewport.X;
                    float standardY1 = pVal1.position.Y - this.ScreenManager.GraphicsDevice.Viewport.Y;
                    float standardY2 = pVal1.position.Y - this.ScreenManager.GraphicsDevice.Viewport.Y;
                    float altX1;
                    float altX2;
                    float altY1;
                    float altY2;
                    if (standardX1 < midX)
                    {
                        altX1 = standardX1 + this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    else
                    {
                        altX1 = standardX1 - this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    if (standardX2 < midX)
                    {
                        altX2 = standardX2 + this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    else
                    {
                        altX2 = standardX2 - this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    if (standardY1 < midY)
                    {
                        altY1 = standardY1 + this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    else
                    {
                        altY1 = standardY1 - this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    if (standardY2 < midY)
                    {
                        altY2 = standardY2 + this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    else
                    {
                        altY2 = standardY2 - this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    float tbase = 0f;
                    float taltitude = 0f;
                    float hyp = 0f;
                    //compare standard 1 to standard 2
                    tbase = standardX1 - standardX2;
                    taltitude = standardY1 - standardY2;
                    hyp = (float)Math.Sqrt(Math.Pow(tbase, 2) + Math.Pow(taltitude, 2));
                    if (hyp < dif)
                    {
                        return true;
                    }
                    //compare standard 1 to alt 2
                    tbase = standardX1 - altX2;
                    taltitude = standardY1 - altY2;
                    hyp = (float)Math.Sqrt(Math.Pow(tbase, 2) + Math.Pow(taltitude, 2));
                    if (hyp < dif)
                    {
                        return true;
                    }
                    //compare alt 1 to standard 2
                    tbase = altX1 - standardX2;
                    taltitude = altY1 - standardY2;
                    hyp = (float)Math.Sqrt(Math.Pow(tbase, 2) + Math.Pow(taltitude, 2));
                    if (hyp < dif)
                    {
                        return true;
                    }
                    return false;
                }


            #endregion

            #region private

            #endregion

        #endregion

    }
}
