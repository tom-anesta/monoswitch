#region File Description
//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace monoswitchExample
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    class MessageBoxScreen : GameScreen
    {

        #region members

            #region public

            #endregion

            #region protected

                protected string m_message;
                protected Texture2D m_gradientTexture;

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

                public event EventHandler<PlayerIndexEventArgs> Accepted;
                public event EventHandler<PlayerIndexEventArgs> Cancelled;

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region functions

            #region public

                /// <summary>
                /// Constructor automatically includes the standard "A=ok, B=cancel"
                /// usage text prompt.
                /// </summary>
                public MessageBoxScreen(string message) : this(message, true)
                {
                }

                /// <summary>
                /// Constructor lets the caller specify whether to include the standard
                /// "A=ok, B=cancel" usage text prompt.
                /// </summary>
                public MessageBoxScreen(string message, bool includeUsageText)
                {
                    const string usageText = "\nA button, Space, Enter = ok" + "\nB button, Esc = cancel";
                    if (includeUsageText)
                    {
                        this.m_message = message + usageText;
                    }
                    else
                    {
                        this.m_message = message;
                    }
                    this.IsPopup = true;
                    this.transitionOnTime = TimeSpan.FromSeconds(0.2);
                    this.transitionOffTime = TimeSpan.FromSeconds(0.2);
                }

                /// <summary>
                /// Loads graphics content for this screen. This uses the shared ContentManager
                /// provided by the Game class, so the content will remain loaded forever.
                /// Whenever a subsequent MessageBoxScreen tries to load this same content,
                /// it will just get back another reference to the already loaded data.
                /// </summary>
                public override void LoadContent()
                {
                    ContentManager content = ScreenManager.Game.Content;
                    this.m_gradientTexture = content.Load<Texture2D>("gradient");
                }

                /// <summary>
                /// Responds to user input, accepting or cancelling the message box.
                /// </summary>
                public override void HandleInput(InputState input)
                {
                    PlayerIndex playerIndex;
                    // We pass in our ControllingPlayer, which may either be null (to
                    // accept input from any player) or a specific index. If we pass a null
                    // controlling player, the InputState helper returns to us which player
                    // actually provided the input. We pass that through to our Accepted and
                    // Cancelled events, so they can tell which player triggered them.
                    if (input.IsMenuSelect(this.controllingPlayer, out playerIndex))
                    {
                        // Raise the accepted event, then exit the message box.
                        if (Accepted != null)
                        {
                            Accepted(this, new PlayerIndexEventArgs(playerIndex));
                        }
                        ExitScreen();
                    }
                    else if (input.IsMenuCancel(this.controllingPlayer, out playerIndex))
                    {
                        // Raise the cancelled event, then exit the message box.
                        if (Cancelled != null)
                        {
                            Cancelled(this, new PlayerIndexEventArgs(playerIndex));
                        }
                        ExitScreen();
                    }
                }

                /// <summary>
                /// Draws the message box.
                /// </summary>
                public override void Draw(GameTime gameTime)
                {
                    SpriteBatch spriteBatch = ScreenManager.spriteBatch;
                    SpriteFont font = ScreenManager.font;
                    // Darken down any other screens that were drawn beneath the popup.
                    ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
                    // Center the message text in the viewport.
                    Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                    Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                    Vector2 textSize = font.MeasureString(this.m_message);
                    Vector2 textPosition = (viewportSize - textSize) / 2;
                    // The background includes a border somewhat larger than the text itself.
                    const int hPad = 32;
                    const int vPad = 16;
                    Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad, (int)textPosition.Y - vPad, (int)textSize.X + hPad * 2, (int)textSize.Y + vPad * 2);
                    // Fade the popup alpha during transitions.
                    Color color = Color.White * TransitionAlpha;
                    spriteBatch.Begin();
                    // Draw the background rectangle.
                    spriteBatch.Draw(this.m_gradientTexture, backgroundRectangle, color);
                    // Draw the message box text.
                    spriteBatch.DrawString(font, this.m_message, textPosition, color);
                    //end drawing
                    spriteBatch.End();
                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

    }
}
