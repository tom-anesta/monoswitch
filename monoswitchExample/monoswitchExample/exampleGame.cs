#region File Description
//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace monoswitchExample
{
    /// <summary>
    /// Sample showing how to manage different game states, with transitions
    /// between menu screens, a loading screen, the game itself, and a pause
    /// menu. This main game class is extremely simple: all the interesting
    /// stuff happens in the ScreenManager component.
    /// </summary>
    public class exampleGame : Microsoft.Xna.Framework.Game
    {

        #region members

            #region public

                // By preloading any assets used by UI rendering, we avoid framerate glitches
                // when they suddenly need to be loaded in the middle of a menu transition.
                public static readonly string[] preloadAssets =
                {
                    "gradient",
                };

            #endregion

            #region protected

                protected GraphicsDeviceManager graphics;
                protected ScreenManager screenManager;

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
                /// The main game constructor.
                /// </summary>
                public exampleGame()
                {
                    Content.RootDirectory = "Content";

                    graphics = new GraphicsDeviceManager(this);
                    graphics.PreferredBackBufferWidth = 853;
                    graphics.PreferredBackBufferHeight = 480;

                    // Create the screen manager component.
                    screenManager = new ScreenManager(this);

                    Components.Add(screenManager);

                    // Activate the first screens.
                    screenManager.AddScreen(new BackgroundScreen(), null);
                    screenManager.AddScreen(new MainMenuScreen(), null);
                }

            #endregion

            #region protected

                /// <summary>
                /// Loads graphics content.
                /// </summary>
                protected override void LoadContent()
                {
                    foreach (string asset in preloadAssets)
                    {
                        Content.Load<object>(asset);
                    }
                }

                /// <summary>
                /// This is called when the game should draw itself.
                /// </summary>
                protected override void Draw(GameTime gameTime)
                {
                    graphics.GraphicsDevice.Clear(Color.Black);

                    // The real drawing happens inside the screen manager component.
                    base.Draw(gameTime);
                }

            #endregion

            #region private

            #endregion

        #endregion

  

        

        


    }


}
