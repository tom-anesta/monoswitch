#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace monoswitchExample
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    class MenuEntry
    {

        #region members

            #region public

            #endregion

            #region protected

                /// <summary>
                /// The text rendered for this entry.
                /// </summary>
                protected string m_text;
                /// <summary>
                /// Tracks a fading selection effect on the entry.
                /// </summary>
                /// <remarks>
                /// The entries transition out of the selection effect when they are deselected.
                /// </remarks>
                protected float m_selectionFade;
                /// <summary>
                /// The position at which the entry is drawn. This is set by the MenuScreen
                /// each frame in Update.
                /// </summary>
                protected Vector2 m_position;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                /// <summary>
                /// Gets or sets the text of this menu entry.
                /// </summary>
                public string text
                {
                    get { return this.m_text; }
                    set { this.m_text = value; }
                }


                /// <summary>
                /// Gets or sets the position at which to draw this menu entry.
                /// </summary>
                public Vector2 position
                {
                    get { return this.m_position; }
                    set { this.m_position = value; }
                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region events

            #region public

                /// <summary>
                /// Event raised when the menu entry is selected.
                /// </summary>
                public event EventHandler<PlayerIndexEventArgs> Selected;

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region functions

            #region public

                /// <summary>
                /// Constructs a new menu entry with the specified text.
                /// </summary>
                public MenuEntry(string text)
                {
                    this.text = text;
                }

                /// <summary>
                /// Updates the menu entry.
                /// </summary>
                public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
                {
                    /*
                    // there is no such thing as a selected item on Windows Phone, so we always
                    // force isSelected to be false
                    #if WINDOWS_PHONE
                                isSelected = false;
                    #endif
                    */
                    // When the menu selection changes, entries gradually fade between
                    // their selected and deselected appearance, rather than instantly
                    // popping to the new state.
                    float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
                    if (isSelected)
                    {
                        this.m_selectionFade = Math.Min(this.m_selectionFade + fadeSpeed, 1);
                    }
                    else
                    {
                        this.m_selectionFade = Math.Max(this.m_selectionFade - fadeSpeed, 0);
                    }
                }


                /// <summary>
                /// Draws the menu entry. This can be overridden to customize the appearance.
                /// </summary>
                public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
                {
                    /*
                    // there is no such thing as a selected item on Windows Phone, so we always
                    // force isSelected to be false
                    #if WINDOWS_PHONE
                                isSelected = false;
                    #endif
                    */
                    // Draw the selected entry in yellow, otherwise white.
                    Color color = isSelected ? Color.Yellow : Color.White;
                    // Pulsate the size of the selected menu entry.
                    double time = gameTime.TotalGameTime.TotalSeconds;
                    float pulsate = (float)Math.Sin(time * 6) + 1;
                    float scale = 1 + pulsate * 0.05f * this.m_selectionFade;
                    // Modify the alpha to fade text out during transitions.
                    color *= screen.TransitionAlpha;
                    // Draw text, centered on the middle of each line.
                    ScreenManager screenManager = screen.ScreenManager;
                    SpriteBatch spriteBatch = screenManager.spriteBatch;
                    SpriteFont font = screenManager.font;
                    Vector2 origin = new Vector2(0, font.LineSpacing / 2);
                    spriteBatch.DrawString(font, this.text, this.position, color, 0, origin, scale, SpriteEffects.None, 0);
                }

                /// <summary>
                /// Queries how much space this menu entry requires.
                /// </summary>
                public virtual int GetHeight(MenuScreen screen)
                {
                    return screen.ScreenManager.font.LineSpacing;
                }
                /// <summary>
                /// Queries how wide the entry is, used for centering on the screen.
                /// </summary>
                public virtual int GetWidth(MenuScreen screen)
                {
                    return (int)screen.ScreenManager.font.MeasureString(text).X;
                }

            #endregion

            #region protected

                /// <summary>
                /// Method for raising the Selected event.
                /// </summary>
                protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
                {
                    if (Selected != null)
                    {
                        Selected(this, new PlayerIndexEventArgs(playerIndex));
                    }
                }

            #endregion

            #region private

            #endregion

        #endregion

    }
}
