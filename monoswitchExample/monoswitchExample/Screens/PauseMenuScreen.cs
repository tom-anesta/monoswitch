#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Ruminate.GUI.Framework;
using Ruminate.GUI.Content;
using System.Collections.Generic;
#endregion

namespace monoswitchExample
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {

        #region members

            #region public

            #endregion

            #region protected

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

                /// <summary>
                /// Constructor.
                /// </summary>
                public PauseMenuScreen(exampleGame game) : base("Paused", game)
                {
                    // Hook up menu event handlers.
                    //quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;
                    Button resumeGameButton = new Button(0, 0, 100, "Resume", OnCancel);
                    Button quitGameButton = new Button(0, 0, 100, "Quit");
                    List<Widget> l1 = new List<Widget>();
                    l1.Add(resumeGameButton);
                    List<Widget> l2 = new List<Widget>();
                    l2.Add(quitGameButton);
                    this.m_menuEntries.Add(l1);
                    this.m_menuEntries.Add(l2);
                    this.m_gui.Widgets = new Widget[] { resumeGameButton, quitGameButton };
                }

                /// <summary>
                /// Event handler for when the Quit Game menu entry is selected.
                /// </summary>
                void QuitGameMenuEntrySelected(Widget widge)
                {
                    const string message = "Are you sure you want to quit this game?";
                    MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message, this.m_game);
                    confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;
                    ScreenManager.AddScreen(confirmQuitMessageBox, this.controllingPlayer);
                }

                /// <summary>
                /// Event handler for when the user selects ok on the "are you sure
                /// you want to quit" message box. This uses the loading screen to
                /// transition from the game back to the main menu screen.
                /// </summary>
                void ConfirmQuitMessageBoxAccepted(Widget widge)
                {
                    LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen(this.m_game));
                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region functions

            #region public

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

    }
}
