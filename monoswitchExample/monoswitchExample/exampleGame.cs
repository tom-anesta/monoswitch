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
using System.IO;
using Ruminate.Utils;
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
                //from ruminate example
                

            #endregion

            #region protected

                protected GraphicsDeviceManager graphics;
                protected ScreenManager screenManager;
                protected SpriteFont m_GreySpriteFont;
                protected Texture2D m_GreyImageMap;
                protected string m_GreyMap;
                
                

            #endregion

            #region private

            #endregion
        
        #endregion

        #region properties

            #region public

                public SpriteFont ss_font
                {
                    get
                    {
                        return this.m_GreySpriteFont;
                    }
                }

                public Texture2D ss_imgMap
                {
                    get
                    {
                        return this.m_GreyImageMap;
                    }
                }

                public string ss_map
                {
                    get
                    {
                        return this.m_GreyMap;
                    }
                }

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
                    // Create the screen manager component.
                    screenManager = new ScreenManager(this);

                    Components.Add(screenManager);
                }

            #endregion

            #region protected

                protected override void Initialize()
                {
                    this.graphics.PreferredBackBufferWidth = 900;
                    this.graphics.PreferredBackBufferHeight = 900;
                    this.graphics.ApplyChanges();
                    IsMouseVisible = true;
                    base.Initialize();
                    var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
                    form.Location = new System.Drawing.Point(0, 0);
                }

                /// <summary>
                /// Loads graphics content.
                /// </summary>
                protected override void LoadContent()
                {
                    foreach (string asset in preloadAssets)
                    {
                        Content.Load<object>(asset);
                    }
                    m_GreyImageMap = Content.Load<Texture2D>(@"TestSkin\BlueGui");
                    m_GreyMap = File.OpenText(@"Content\TestSkin\Map.txt").ReadToEnd();
                    m_GreySpriteFont = Content.Load<SpriteFont>(@"Font");
                    DebugUtils.Init(this.graphics.GraphicsDevice, m_GreySpriteFont);
                    // Activate the first screens.
                    screenManager.AddScreen(new BackgroundScreen(), null);
                    screenManager.AddScreen(new MainMenuScreen(this), null);
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
