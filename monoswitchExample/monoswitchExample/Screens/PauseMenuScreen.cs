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
                public PauseMenuScreen() : base("Paused")
                {
                    // Create our menu entries.
                    MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
                    MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");
                    // Hook up menu event handlers.
                    resumeGameMenuEntry.Selected += OnCancel;
                    quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;
                    // Add entries to the menu.
                    this.m_menuEntries.Add(resumeGameMenuEntry);
                    this.m_menuEntries.Add(quitGameMenuEntry);
                }

                /// <summary>
                /// Event handler for when the Quit Game menu entry is selected.
                /// </summary>
                void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
                {
                    const string message = "Are you sure you want to quit this game?";
                    MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);
                    confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;
                    ScreenManager.AddScreen(confirmQuitMessageBox, this.controllingPlayer);
                }

                /// <summary>
                /// Event handler for when the user selects ok on the "are you sure
                /// you want to quit" message box. This uses the loading screen to
                /// transition from the game back to the main menu screen.
                /// </summary>
                void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
                {
                    LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
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
