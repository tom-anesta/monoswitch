#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Input.Touch;//touch not available
using System.Collections.Generic;
#endregion

namespace monoswitchExample
{
    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        #region members

            #region public

                public const int MaxInputs = 4;
                

                //public TouchCollection TouchState;

                //public readonly List<GestureSample> Gestures = new List<GestureSample>();

            #endregion

            #region protected

                protected readonly KeyboardState[] m_currentKeyboardStates;
                protected readonly GamePadState[] m_currentGamePadStates;
                protected readonly KeyboardState[] m_lastKeyboardStates;
                protected readonly GamePadState[] m_lastGamePadStates;
                protected readonly bool[] m_gamePadWasConnected;

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

                public KeyboardState[] currentKeyboardStates
                {
                    get
                    {
                        return this.m_currentKeyboardStates;
                    }
                }
                public GamePadState[] currentGamePadStates
                {
                    get
                    {
                        return this.m_currentGamePadStates;
                    }
                }
                public KeyboardState[] lastKeyBoardStates
                {
                    get
                    {
                        return this.m_lastKeyboardStates;
                    }
                }
                public GamePadState[] lastGamePadStates
                {
                    get
                    {
                        return this.m_lastGamePadStates;
                    }
                }
                public bool[] gamePadWasConnected
                {
                    get
                    {
                        return this.m_gamePadWasConnected;
                    }
                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region functions

            #region public

                /// <summary>
                /// Constructs a new input state.
                /// </summary>
                public InputState()
                {
                    this.m_currentKeyboardStates = new KeyboardState[MaxInputs];
                    this.m_currentGamePadStates = new GamePadState[MaxInputs];
                    this.m_lastKeyboardStates = new KeyboardState[MaxInputs];
                    this.m_lastGamePadStates = new GamePadState[MaxInputs];
                    this.m_gamePadWasConnected = new bool[MaxInputs];
                }

                /// <summary>
                /// Reads the latest state of the keyboard and gamepad.
                /// </summary>
                public void Update()
                {
                    for (int i = 0; i < MaxInputs; i++)
                    {
                        this.m_lastKeyboardStates[i] = this.m_currentKeyboardStates[i];
                        this.m_lastGamePadStates[i] = this.m_currentGamePadStates[i];

                        this.m_currentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                        this.m_currentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                        // Keep track of whether a gamepad has ever been
                        // connected, so we can detect if it is unplugged.
                        if (this.m_currentGamePadStates[i].IsConnected)
                        {
                            this.m_gamePadWasConnected[i] = true;
                        }
                    }

                    /*
                    TouchState = TouchPanel.GetState();
                    Gestures.Clear();
                    while (TouchPanel.IsGestureAvailable)
                    {
                        Gestures.Add(TouchPanel.ReadGesture());
                    }
                    */

                }

                /// <summary>
                /// Helper for checking if a key was newly pressed during this update. The
                /// controllingPlayer parameter specifies which player to read input for.
                /// If this is null, it will accept input from any player. When a keypress
                /// is detected, the output playerIndex reports which player pressed it.
                /// </summary>
                public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer,
                                                    out PlayerIndex playerIndex)
                {
                    if (controllingPlayer.HasValue)
                    {
                        // Read input from the specified player.
                        playerIndex = controllingPlayer.Value;
                        int i = (int)playerIndex;
                        return (this.m_currentKeyboardStates[i].IsKeyDown(key) && this.m_lastKeyboardStates[i].IsKeyUp(key));
                    }
                    else
                    {
                        // Accept input from any player.
                        return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) || IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) || IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) || IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
                    }
                }

                /// <summary>
                /// Helper for checking if a button was newly pressed during this update.
                /// The controllingPlayer parameter specifies which player to read input for.
                /// If this is null, it will accept input from any player. When a button press
                /// is detected, the output playerIndex reports which player pressed it.
                /// </summary>
                public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
                {
                    if (controllingPlayer.HasValue)
                    {
                        // Read input from the specified player.
                        playerIndex = controllingPlayer.Value;
                        int i = (int)playerIndex;
                        return (this.m_currentGamePadStates[i].IsButtonDown(button) && this.m_lastGamePadStates[i].IsButtonUp(button));
                    }
                    else
                    {
                        // Accept input from any player.
                        return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) || IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) || IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) || IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
                    }
                }

                /// <summary>
                /// Checks for a "menu select" input action.
                /// The controllingPlayer parameter specifies which player to read input for.
                /// If this is null, it will accept input from any player. When the action
                /// is detected, the output playerIndex reports which player pressed it.
                /// </summary>
                public bool IsMenuSelect(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
                {
                    return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) || IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) || IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) || IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
                }

                /// <summary>
                /// Checks for a "menu cancel" input action.
                /// The controllingPlayer parameter specifies which player to read input for.
                /// If this is null, it will accept input from any player. When the action
                /// is detected, the output playerIndex reports which player pressed it.
                /// </summary>
                public bool IsMenuCancel(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
                {
                    return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) || IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) || IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
                }

                /// <summary>
                /// Checks for a "menu up" input action.
                /// The controllingPlayer parameter specifies which player to read
                /// input for. If this is null, it will accept input from any player.
                /// </summary>
                public bool IsMenuUp(PlayerIndex? controllingPlayer)
                {
                    PlayerIndex playerIndex;
                    return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) || IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) || IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
                }

                /// <summary>
                /// Checks for a "menu down" input action.
                /// The controllingPlayer parameter specifies which player to read
                /// input for. If this is null, it will accept input from any player.
                /// </summary>
                public bool IsMenuDown(PlayerIndex? controllingPlayer)
                {
                    PlayerIndex playerIndex;
                    return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) || IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) || IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
                }

                /// <summary>
                /// Checks for a "pause the game" input action.
                /// The controllingPlayer parameter specifies which player to read
                /// input for. If this is null, it will accept input from any player.
                /// </summary>
                public bool IsPauseGame(PlayerIndex? controllingPlayer)
                {
                    PlayerIndex playerIndex;
                    return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) || IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) || IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion


    }
}
