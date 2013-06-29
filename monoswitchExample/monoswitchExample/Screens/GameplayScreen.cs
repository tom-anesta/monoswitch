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
                protected List<star> m_stars;
                protected Player m_player;

            #endregion

            #region private

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
                }

                /// <summary>
                /// Load graphics content for the game.
                /// </summary>
                public override void LoadContent()
                {
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
                    this.m_player.Initialize(playerAnimation, playerPosition);
                    m_starTexture = this.m_content.Load<Texture2D>("mineAnimation");
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
                        // Otherwise move the player position.
                        Vector2 movement = Vector2.Zero;
                        if (keyboardState.IsKeyDown(Keys.Left))
                        {
                            movement.X--;
                        }
                        if (keyboardState.IsKeyDown(Keys.Right))
                        {
                            movement.X++;
                        }
                        if (keyboardState.IsKeyDown(Keys.Up))
                        {
                            movement.Y--;
                        }
                        if (keyboardState.IsKeyDown(Keys.Down))
                        {
                            movement.Y++;
                        }
                        Vector2 thumbstick = gamePadState.ThumbSticks.Left;
                        movement.X += thumbstick.X;
                        movement.Y -= thumbstick.Y;
                        if (movement.Length() > 1)
                        {
                            movement.Normalize();
                        }
                        //this.m_player.Position += movement * 2;
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
                    if (this.m_player.isOutOfBounds)
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
                        if (rectanglePlayer.Intersects(rectangleStar) || (this.m_player.isOutOfBounds && rectangleOutPlayer.Intersects(rectangleStar)))
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
                            if (rectanglePlayer.Intersects(rectangleOutStar) || (this.m_player.isOutOfBounds && rectanglePlayer.Intersects(rectangleOutStar)))
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
                    spriteBatch.Begin();


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

            #endregion

            #region private

            #endregion

        #endregion

    }
}
