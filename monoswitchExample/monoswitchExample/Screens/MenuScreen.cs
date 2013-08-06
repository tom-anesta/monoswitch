#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ruminate.GUI.Content;
using Ruminate.GUI.Framework;
#endregion

namespace monoswitchExample
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {

        #region members

            #region public

            #endregion

            #region protected

                protected List<List<Widget>> m_menuEntries = new List<List<Widget>>();
                //protected int m_selectedEntry = 0;
                protected string m_menuTitle;
                protected Gui m_gui;
                protected exampleGame m_game;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

            #endregion

            #region protected

                /// <summary>
                /// Gets the list of menu entries, so derived classes can add
                /// or change the menu contents.
                /// </summary>
                /// 
                /*
                protected IList<MenuEntry> menuEntries
                {
                    get { return this.m_menuEntries; }
                }
                */
                


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
                public MenuScreen(string menuTitle, exampleGame game)
                {
                    this.m_game = game;
                    this.m_menuTitle = menuTitle;
                    this.m_transitionOnTime = TimeSpan.FromSeconds(0.5);
                    this.m_transitionOffTime = TimeSpan.FromSeconds(0.5);
                    Skin nskin = new Skin(game.ss_imgMap, game.ss_map);
                    Text ntext = new Text(game.ss_font, Color.CadetBlue);
                    this.m_gui = new Gui(game, nskin, ntext);
                }
                /// <summary>
                /// Updates the menu.
                /// </summary>
                public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
                {
                    base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                    /*
                    // Update each nested MenuEntry object.
                    for (int i = 0; i < menuEntries.Count; i++)
                    {
                        bool isSelected = this.IsActive && (i == this.m_selectedEntry);
                        this.menuEntries[i].Update(this, isSelected, gameTime);
                    }
                    */
                    this.m_gui.Update();
                }

                /// <summary>
                /// Draws the menu.
                /// </summary>
                public override void Draw(GameTime gameTime)
                {
                    // make sure our entries are in the right place before we draw them
                    UpdateMenuEntryLocations();
                    GraphicsDevice graphics = ScreenManager.GraphicsDevice;
                    SpriteBatch spriteBatch = ScreenManager.spriteBatch;
                    SpriteFont font = ScreenManager.font;
                    //begin drawing
                    spriteBatch.Begin();
                    float transitionOffset = (float)Math.Pow(this.m_transitionPosition, 2);
                    // Draw the menu title centered on the screen
                    Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
                    Vector2 titleOrigin = font.MeasureString(this.m_menuTitle) / 2;
                    Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
                    float titleScale = 1.25f;
                    titlePosition.Y -= transitionOffset * 100;
                    //draw the title
                    spriteBatch.DrawString(font, this.m_menuTitle, titlePosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);
                    //end drawing
                    spriteBatch.End();
                    this.m_gui.Draw();
                }
                
            #endregion

            #region protected
                
                protected virtual void OnCancel(Widget widge)
                {
                    this.ExitScreen();
                }
                /// <summary>
                /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
                /// </summary>
                /*
                protected void OnCancel(object sender, PlayerIndexEventArgs e)
                {
                    OnCancel(e.playerIndex);
                }
                */
                /// <summary>
                /// Allows the screen the chance to position the menu entries. By default
                /// all menu entries are lined up in a vertical list, centered on the screen.
                /// </summary>
                protected virtual void UpdateMenuEntryLocations()
                {
                    // Make the menu slide into place during transitions, using a
                    // power curve to make things look more interesting (this makes
                    // the movement slow down as it nears the end).
                    float transitionOffset = (float)Math.Pow(this.m_transitionPosition, 2);
                    // start at Y = 175; each X value is generated per entry
                    Vector2 position = new Vector2(0f, 175f);
                    // update each menu entry's location in turn
                    Widget menuEntry = null;
                    for (int i = 0; i < m_menuEntries.Count; i++)
                    {
                        if (menuEntry != null)
                        {
                            position.Y += menuEntry.AbsoluteArea.Height+10;
                        }
                        int sum = 0;
                        position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2;
                        if (ScreenState == ScreenState.TransitionOn)
                        {
                            position.X -= transitionOffset * 256;
                        }
                        else
                        {
                            position.X -= transitionOffset * 512;
                        }
                        
                        for (int j = 0; j < m_menuEntries[i].Count; j++)
                        {
                            menuEntry = this.m_menuEntries[i][j];
                            sum += menuEntry.AbsoluteArea.Width;
                        }
                        position.X = position.X - sum / 2;
                        for (int k = 0; k < m_menuEntries[i].Count; k++)
                        {
                            menuEntry = m_menuEntries[i][k];
                            menuEntry.AbsoluteArea = new Rectangle((int)position.X, (int)position.Y, menuEntry.AbsoluteArea.Width, menuEntry.AbsoluteArea.Height);
                            position.X += menuEntry.AbsoluteArea.Width+10;
                        }
                    }
                }

                


            #endregion

            #region private

            #endregion

        #endregion

    }
}
