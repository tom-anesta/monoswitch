#region File Description
//-----------------------------------------------------------------------------
// ScreenManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
#endregion

namespace monoswitchExample
{
    /// <summary>
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        #region members

        #region public

        #endregion

        #region protected

            protected List<GameScreen> m_screens = new List<GameScreen>();
            protected List<GameScreen> m_screensToUpdate = new List<GameScreen>();

            protected InputState m_input = new InputState();

            protected SpriteBatch m_spriteBatch;
            protected SpriteFont m_font;
            protected Texture2D m_blankTexture;

            protected bool m_isInitialized;

            protected bool m_traceEnabled;

        #endregion

        #region private

        #endregion

        #endregion

        #region properties

        #region public

            /// <summary>
            /// A default SpriteBatch shared by all the screens. This saves
            /// each screen having to bother creating their own local instance.
            /// </summary>
            public SpriteBatch spriteBatch
            {
                get { return this.m_spriteBatch; }
            }

            /// <summary>
            /// A default font shared by all the screens. This saves
            /// each screen having to bother loading their own local copy.
            /// </summary>
            public SpriteFont font
            {
                get { return this.m_font; }
            }

            /// <summary>
            /// If true, the manager prints out a list of all the screens
            /// each time it is updated. This can be useful for making sure
            /// everything is being added and removed at the right times.
            /// </summary>
            public bool traceEnabled
            {
                get { return this.m_traceEnabled; }
                set { this.m_traceEnabled = value; }
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
            /// Constructs a new screen manager component.
            /// </summary>
            public ScreenManager(Game game) : base(game)
            {
                // we must set EnabledGestures before we can query for them, but
                // we don't assume the game wants to read them.
                //TouchPanel.EnabledGestures = GestureType.None;//touch panel not available
            }

            /// <summary>
            /// Initializes the screen manager component.
            /// </summary>
            public override void Initialize()
            {
                base.Initialize();
                this.m_isInitialized = true;
            }

            /// <summary>
            /// Tells each screen to draw itself.
            /// </summary>
            public override void Draw(GameTime gameTime)
            {
                foreach (GameScreen screen in this.m_screens)
                {
                    if (screen.ScreenState == ScreenState.Hidden)
                        continue;

                    screen.Draw(gameTime);
                }
            }

            /// <summary>
            /// Allows each screen to run logic.
            /// </summary>
            public override void Update(GameTime gameTime)
            {
                // Read the keyboard and gamepad.
                this.m_input.Update();
                // Make a copy of the master screen list, to avoid confusion if
                // the process of updating one screen adds or removes others.
                this.m_screensToUpdate.Clear();
                foreach (GameScreen screen in this.m_screens)
                {
                    this.m_screensToUpdate.Add(screen);
                }
                bool otherScreenHasFocus = !Game.IsActive;
                bool coveredByOtherScreen = false;
                // Loop as long as there are screens waiting to be updated.
                while (this.m_screensToUpdate.Count > 0)
                {
                    // Pop the topmost screen off the waiting list.
                    GameScreen screen = this.m_screensToUpdate[this.m_screensToUpdate.Count - 1];
                    this.m_screensToUpdate.RemoveAt(this.m_screensToUpdate.Count - 1);
                    // Update the screen.
                    screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                    if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
                    {
                        // If this is the first active screen we came across,
                        // give it a chance to handle input.
                        if (!otherScreenHasFocus)
                        {
                            screen.HandleInput(this.m_input);
                            otherScreenHasFocus = true;
                        }
                        // If this is an active non-popup, inform any subsequent
                        // screens that they are covered by it.
                        if (!screen.IsPopup)
                            coveredByOtherScreen = true;
                    }
                }
                // Print debug trace?
                if (this.m_traceEnabled)
                    TraceScreens();
            }

            /// <summary>
            /// Adds a new screen to the screen manager.
            /// </summary>
            public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
            {
                screen.ControllingPlayer = controllingPlayer;
                screen.ScreenManager = this;
                screen.IsExiting = false;
                // If we have a graphics device, tell the screen to load content.
                if (this.m_isInitialized)
                {
                    screen.LoadContent();
                }
                this.m_screens.Add(screen);
                /*
                // update the TouchPanel to respond to gestures this screen is interested in
                TouchPanel.EnabledGestures = screen.EnabledGestures;
                */
            }


            /// <summary>
            /// Removes a screen from the screen manager. You should normally
            /// use GameScreen.ExitScreen instead of calling this directly, so
            /// the screen can gradually transition off rather than just being
            /// instantly removed.
            /// </summary>
            public void RemoveScreen(GameScreen screen)
            {
                // If we have a graphics device, tell the screen to unload content.
                if (this.m_isInitialized)
                {
                    screen.UnloadContent();
                }
                this.m_screens.Remove(screen);
                this.m_screensToUpdate.Remove(screen);
                /*
                // if there is a screen still in the manager, update TouchPanel
                // to respond to gestures that screen is interested in.
                if (this.m_screens.Count > 0)
                {
                    TouchPanel.EnabledGestures = screens[screens.Count - 1].EnabledGestures;
                }
                */
            }


            /// <summary>
            /// Expose an array holding all the screens. We return a copy rather
            /// than the real master list, because screens should only ever be added
            /// or removed using the AddScreen and RemoveScreen methods.
            /// </summary>
            public GameScreen[] GetScreens()
            {
                return this.m_screens.ToArray();
            }

            /// <summary>
            /// Helper draws a translucent black fullscreen sprite, used for fading
            /// screens in and out, and for darkening the background behind popups.
            /// </summary>
            public void FadeBackBufferToBlack(float alpha)
            {
                Viewport viewport = GraphicsDevice.Viewport;
                spriteBatch.Begin();
                spriteBatch.Draw(this.m_blankTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.Black * alpha);
                spriteBatch.End();
            }

        #endregion

        #region protected

            /// <summary>
            /// Load your graphics content.
            /// </summary>
            protected override void LoadContent()
            {
                // Load content belonging to the screen manager.
                ContentManager content = Game.Content;

                this.m_spriteBatch = new SpriteBatch(GraphicsDevice);
                this.m_font = content.Load<SpriteFont>("menufont");
                this.m_blankTexture = content.Load<Texture2D>("blank");

                // Tell each of the screens to load their content.
                foreach (GameScreen screen in this.m_screens)
                {
                    screen.LoadContent();
                }
            }

            /// <summary>
            /// Unload your graphics content.
            /// </summary>
            protected override void UnloadContent()
            {
                // Tell each of the screens to unload their content.
                foreach (GameScreen screen in this.m_screens)
                {
                    screen.UnloadContent();
                }
            }

            /// <summary>
            /// Prints a list of all the screens, for debugging.
            /// </summary>
            protected void TraceScreens()
            {
                List<string> screenNames = new List<string>();
                foreach (GameScreen screen in this.m_screens)
                    screenNames.Add(screen.GetType().Name);
                Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
            }

        #endregion

        #region private

        #endregion

        #endregion

    }
}
