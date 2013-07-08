#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
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
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {

        #region members

            #region public

            #endregion

            #region protected

                protected exampleGame m_game

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
                /// Constructor fills in the menu contents.
                /// </summary>
                public MainMenuScreen(exampleGame game) : base("Main Menu")
                {
                    // Create our menu entries.
                    MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
                    MenuEntry optionsMenuEntry = new MenuEntry("Options");
                    MenuEntry exitMenuEntry = new MenuEntry("Exit");
                    // Hook up menu event handlers.
                    playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
                    optionsMenuEntry.Selected += OptionsMenuEntrySelected;
                    exitMenuEntry.Selected += OnCancel;
                    // Add entries to the menu.
                    this.m_menuEntries.Add(playGameMenuEntry);
                    this.m_menuEntries.Add(optionsMenuEntry);
                    this.m_menuEntries.Add(exitMenuEntry);
                    this.m_game = game;
                }

            #endregion

            #region protected

                /// <summary>
                /// Event handler for when the Play Game menu entry is selected.
                /// </summary>
                protected void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
                {
                    LoadingScreen.Load(ScreenManager, true, e.playerIndex, new GameplayScreen(this.m_game));
                }

                /// <summary>
                /// Event handler for when the Options menu entry is selected.
                /// </summary>
                protected void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
                {
                    ScreenManager.AddScreen(new OptionsMenuScreen(), e.playerIndex);
                }

                /// <summary>
                /// When the user cancels the main menu, ask if they want to exit the sample.
                /// </summary>
                protected override void OnCancel(PlayerIndex playerIndex)
                {
                    const string message = "Are you sure you want to exit this sample?";
                    MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);
                    confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
                    ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
                }
                /// <summary>
                /// Event handler for when the user selects ok on the "are you sure
                /// you want to exit" message box.
                /// </summary>
                protected void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
                {
                    ScreenManager.Game.Exit();
                }

            #endregion

            #region private

            #endregion

        #endregion

    }
}
