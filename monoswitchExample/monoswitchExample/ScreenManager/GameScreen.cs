#region File Description
//-----------------------------------------------------------------------------
// GameScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;
#endregion

namespace monoswitchExample
{
    /// <summary>
    /// Enum describes the screen transition state.
    /// </summary>
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    /// <summary>
    /// A screen is a single layer that has update and draw logic, and which
    /// can be combined with other layers to build up a complex menu system.
    /// For instance the main menu, the options menu, the "are you sure you
    /// want to quit" message box, and the main game itself are all implemented
    /// as screens.
    /// </summary>
    public abstract class GameScreen
    {
        #region members

            #region public

            #endregion

            #region protected

                protected bool m_isPopup = false;
                protected TimeSpan m_transitionOnTime = TimeSpan.Zero;
                protected TimeSpan m_transitionOffTime = TimeSpan.Zero;
                protected float m_transitionPosition = 1;
                protected ScreenState m_screenState = ScreenState.TransitionOn;
                protected bool m_isExiting = false;
                protected bool m_otherScreenHasFocus;
                protected ScreenManager m_screenManager;
                protected PlayerIndex? m_controllingPlayer;
                //protected GestureType enabledGestures = GestureType.None;//for some reason touch not available?

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                /// <summary>
                /// Normally when one screen is brought up over the top of another,
                /// the first screen will transition off to make room for the new
                /// one. This property indicates whether the screen is only a small
                /// popup, in which case screens underneath it do not need to bother
                /// transitioning off.
                /// </summary>
                public bool IsPopup
                {
                    get
                    {
                        return this.m_isPopup;
                    }
                    protected set
                    {
                        this.m_isPopup = value;
                    }
                }

                /// <summary>
                /// Indicates how long the screen takes to
                /// transition on when it is activated.
                /// </summary>
                public TimeSpan transitionOnTime
                {
                    get
                    {
                        return this.m_transitionOnTime;
                    }
                    protected set
                    {
                        this.m_transitionOnTime = value;
                    }
                }

                /// <summary>
                /// Indicates how long the screen takes to
                /// transition off when it is deactivated.
                /// </summary>
                public TimeSpan transitionOffTime
                {
                    get
                    {
                        return this.m_transitionOffTime;
                    }
                    protected set
                    {
                        this.m_transitionOffTime = value;
                    }
                }


                /// <summary>
                /// Gets the current position of the screen transition, ranging
                /// from zero (fully active, no transition) to one (transitioned
                /// fully off to nothing).
                /// </summary>
                public float transitionPosition
                {
                    get
                    {
                        return this.m_transitionPosition;
                    }
                    protected set
                    {
                        this.m_transitionPosition = value;
                    }
                }

                /// <summary>
                /// Gets the current alpha of the screen transition, ranging
                /// from 1 (fully active, no transition) to 0 (transitioned
                /// fully off to nothing).
                /// </summary>
                public float TransitionAlpha
                {
                    get
                    {
                        return 1f - this.m_transitionPosition;
                    }
                }

                /// <summary>
                /// Gets the current screen transition state.
                /// </summary>
                public ScreenState ScreenState
                {
                    get
                    {
                        return this.m_screenState;
                    }
                    protected set
                    {
                        this.m_screenState = value;
                    }
                }

                /// <summary>
                /// There are two possible reasons why a screen might be transitioning
                /// off. It could be temporarily going away to make room for another
                /// screen that is on top of it, or it could be going away for good.
                /// This property indicates whether the screen is exiting for real:
                /// if set, the screen will automatically remove itself as soon as the
                /// transition finishes.
                /// </summary>
                public bool IsExiting
                {
                    get
                    {
                        return this.m_isExiting;
                    }
                    protected internal set
                    {
                        this.m_isExiting = value;
                    }
                }

                /// <summary>
                /// Checks whether this screen is active and can respond to user input.
                /// </summary>
                public bool IsActive
                {
                    get
                    {
                        return !this.m_otherScreenHasFocus && (this.m_screenState == ScreenState.TransitionOn || this.m_screenState == ScreenState.Active);
                    }
                }
        
                /// <summary>
                /// Gets the manager that this screen belongs to.
                /// </summary>
                public ScreenManager ScreenManager
                {
                    get
                    {
                        return this.m_screenManager;
                    }
                    internal set
                    {
                        this.m_screenManager = value;
                    }
                }

                /// <summary>
                /// Gets the index of the player who is currently controlling this screen,
                /// or null if it is accepting input from any player. This is used to lock
                /// the game to a specific player profile. The main menu responds to input
                /// from any connected gamepad, but whichever player makes a selection from
                /// this menu is given control over all subsequent screens, so other gamepads
                /// are inactive until the controlling player returns to the main menu.
                /// </summary>
                public PlayerIndex? controllingPlayer
                {
                    get
                    {
                        return this.m_controllingPlayer;
                    }
                    internal set
                    {
                        this.m_controllingPlayer = value;
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
                /// Load graphics content for the screen.
                /// </summary>
                public virtual void LoadContent()
                {
                }

                /// <summary>
                /// Unload content for the screen.
                /// </summary>
                public virtual void UnloadContent()
                {
                }

                /// <summary>
                /// Allows the screen to run logic, such as updating the transition position.
                /// Unlike HandleInput, this method is called regardless of whether the screen
                /// is active, hidden, or in the middle of a transition.
                /// </summary>
                public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
                {
                    this.m_otherScreenHasFocus = otherScreenHasFocus;

                    if (this.m_isExiting)
                    {
                        // If the screen is going away to die, it should transition off.
                        this.m_screenState = ScreenState.TransitionOff;

                        if (!UpdateTransition(gameTime, transitionOffTime, 1))
                        {
                            // When the transition finishes, remove the screen.
                            ScreenManager.RemoveScreen(this);
                        }
                    }
                    else if (coveredByOtherScreen)
                    {
                        // If the screen is covered by another, it should transition off.
                        if (UpdateTransition(gameTime, transitionOffTime, 1))
                        {
                            // Still busy transitioning.
                            this.m_screenState = ScreenState.TransitionOff;
                        }
                        else
                        {
                            // Transition finished!
                            this.m_screenState = ScreenState.Hidden;
                        }
                    }
                    else
                    {
                        // Otherwise the screen should transition on and become active.
                        if (UpdateTransition(gameTime, transitionOnTime, -1))
                        {
                            // Still busy transitioning.
                            this.m_screenState = ScreenState.TransitionOn;
                        }
                        else
                        {
                            // Transition finished!
                            this.m_screenState = ScreenState.Active;
                        }
                    }
                }

                /// <summary>
                /// Helper for updating the screen transition position.
                /// </summary>
                bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
                {

                    // How much should we move by?
                    float transitionDelta;

                    if (time == TimeSpan.Zero)
                        transitionDelta = 1;
                    else
                        transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                                  time.TotalMilliseconds);

                    // Update the transition position.
                    this.m_transitionPosition += transitionDelta * direction;

                    // Did we reach the end of the transition?
                    if (((direction < 0) && (transitionPosition <= 0)) ||
                        ((direction > 0) && (transitionPosition >= 1)))
                    {
                        this.m_transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                        return false;
                    }
                    // Otherwise we are still busy transitioning.
                    return true;
                }

                /// <summary>
                /// Allows the screen to handle user input. Unlike Update, this method
                /// is only called when the screen is active, and not when some other
                /// screen has taken the focus.
                /// </summary>
                public virtual void HandleInput(InputState input)
                {
                }

                /// <summary>
                /// This is called when the screen should draw itself.
                /// </summary>
                public virtual void Draw(GameTime gameTime)
                {
                }

                /// <summary>
                /// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
                /// instantly kills the screen, this method respects the transition timings
                /// and will give the screen a chance to gradually transition off.
                /// </summary>
                public void ExitScreen()
                {
                    if (this.m_transitionOffTime == TimeSpan.Zero)
                    {
                        // If the screen has a zero transition time, remove it immediately.
                        ScreenManager.RemoveScreen(this);
                    }
                    else
                    {
                        // Otherwise flag that it should transition off and then exit.
                        this.m_isExiting = true;
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
