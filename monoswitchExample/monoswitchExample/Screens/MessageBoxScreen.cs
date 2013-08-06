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
using Ruminate.GUI.Content;
using Ruminate.GUI.Framework;
using System.Collections.Generic;
#endregion

namespace monoswitchExample
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    class MessageBoxScreen : MenuScreen
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

                public event WidgetEvent Accepted;
                public event WidgetEvent Cancelled;

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
                public MessageBoxScreen(string message, exampleGame game): base(message, game)
                {
                    //set up your buttons
                    Button OKButton = new Button(0, 0, 100, "OK", Accepted);
                    Button NoButton = new Button(0, 0, 100, "Cancel", Cancelled);
                    this.Cancelled += this.CancelMenu;
                    this.IsPopup = true;
                    this.transitionOnTime = TimeSpan.FromSeconds(0.2);
                    this.transitionOffTime = TimeSpan.FromSeconds(0.2);
                    //set up your display
                    List<Widget> l1 = new List<Widget>();
                    l1.Add(OKButton);
                    List<Widget> l2 = new List<Widget>();
                    l2.Add(NoButton);
                    this.m_menuEntries.Add(l1);
                    this.m_menuEntries.Add(l2);
                    //set up your gui
                    this.m_gui.Widgets = new Widget[] { OKButton, NoButton };
                }

                /// <summary>
                /// Constructor lets the caller specify whether to include the standard
                /// "A=ok, B=cancel" usage text prompt.
                /// </summary>

                public void CancelMenu(Widget widge)
                {
                    ScreenManager.RemoveScreen(this);
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
