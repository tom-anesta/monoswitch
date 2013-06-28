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

                protected ContentManager m_content;
                protected SpriteFont m_gameFont;
                protected Vector2 m_playerPosition = new Vector2(100, 100);
                protected Vector2 m_enemyPosition = new Vector2(100, 100);
                protected Random m_random = new Random();
                protected float m_pauseAlpha;

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
                    if (this.IsActive)
                    {
                        // Apply some random jitter to make the enemy move around.
                        const float randomization = 10;
                        this.m_enemyPosition.X += (float)(this.m_random.NextDouble() - 0.5) * randomization;
                        this.m_enemyPosition.Y += (float)(this.m_random.NextDouble() - 0.5) * randomization;
                        // Apply a stabilizing force to stop the enemy moving off the screen.
                        Vector2 targetPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - this.m_gameFont.MeasureString("Insert Gameplay Here").X / 2, 200);
                        this.m_enemyPosition = Vector2.Lerp(this.m_enemyPosition, targetPosition, 0.05f);
                        // TODO: this game isn't very fun! You could probably improve
                        // it by inserting something more interesting in this space :-)
                    }
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
                        this.m_playerPosition += movement * 2;
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
                    spriteBatch.DrawString(this.m_gameFont, "// TODO", this.m_playerPosition, Color.Green);
                    spriteBatch.DrawString(this.m_gameFont, "Insert Gameplay Here", this.m_enemyPosition, Color.DarkRed);
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

            #endregion

            #region private

            #endregion

        #endregion

    }
}
