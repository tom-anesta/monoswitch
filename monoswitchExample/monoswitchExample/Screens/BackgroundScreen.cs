#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
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
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class BackgroundScreen : GameScreen
    {
        #region members

            #region public

            #endregion

            #region protected

                protected ContentManager m_content;
                protected Texture2D m_backgroundTexture;

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
                public BackgroundScreen()
                {
                    this.m_transitionOnTime = TimeSpan.FromSeconds(0.5);
                    this.m_transitionOffTime = TimeSpan.FromSeconds(0.5);
                }

                /// <summary>
                /// Loads graphics content for this screen. The background texture is quite
                /// big, so we use our own local ContentManager to load it. This allows us
                /// to unload before going from the menus into the game itself, wheras if we
                /// used the shared ContentManager provided by the Game class, the content
                /// would remain loaded forever.
                /// </summary>
                public override void LoadContent()
                {
                    if (this.m_content == null)
                    {
                        this.m_content = new ContentManager(ScreenManager.Game.Services, "Content");
                    }

                    this.m_backgroundTexture = this.m_content.Load<Texture2D>("background");
                }


                /// <summary>
                /// Unloads graphics content for this screen.
                /// </summary>
                public override void UnloadContent()
                {
                    m_content.Unload();
                }

                /// <summary>
                /// Updates the background screen. Unlike most screens, this should not
                /// transition off even if it has been covered by another screen: it is
                /// supposed to be covered, after all! This overload forces the
                /// coveredByOtherScreen parameter to false in order to stop the base
                /// Update method wanting to transition off.
                /// </summary>
                public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
                {
                    base.Update(gameTime, otherScreenHasFocus, false);
                }


                /// <summary>
                /// Draws the background screen.
                /// </summary>
                public override void Draw(GameTime gameTime)
                {
                    SpriteBatch spriteBatch = ScreenManager.spriteBatch;
                    Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                    Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
                    spriteBatch.Begin();
                    spriteBatch.Draw(this.m_backgroundTexture, fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
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
