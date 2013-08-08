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
using Ruminate.GUI.Content;
using Ruminate.GUI.Framework;
using System.Collections.Generic;
using System;
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

                protected gameParams m_params;

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
                public MainMenuScreen(exampleGame game, gameParams gparams = null) : base("Main Menu", game)
                {
                    if (gparams == null)
                    {
                        this.m_params = new gameParams(gameParams.DEF_SCAN_RATE, gameParams.DEF_REFACTORY_PERIOD, gameParams.DEF_DISCRETE_HOLD, gameParams.DEF_GAME_TIME, gameParams.DEF_GOAL, gameParams.DEF_DISCRETE, gameParams.DEF_COMPOSITE, gameParams.DEF_MARKER);
                    }
                    else
                    {
                        this.m_params = gparams;
                    }
                    //build the gui
                    Button startbutton = new Button(0, 0, 100, "Play Game", this.PlayGameMenuEntrySelected);
                    List<Widget> l1 = new List<Widget>();
                    l1.Add(startbutton);
                    Button optionsbutton = new Button(0, 0, 100, "Options", this.OptionsMenuEntrySelected);
                    List<Widget> l2 = new List<Widget>();
                    l2.Add(optionsbutton);
                    Button exitButton = new Button(0, 0, 100, "Exit", this.OnCancel);
                    List<Widget> l3 = new List<Widget>();
                    l3.Add(exitButton);
                    //add the gui elements to the update list
                    this.m_gui.Widgets = new Widget[] { startbutton, optionsbutton, exitButton };
                    this.m_menuEntries.Add(l1);
                    this.m_menuEntries.Add(l2);
                    this.m_menuEntries.Add(l3);
                }

            #endregion

            #region protected

                /// <summary>
                /// Event handler for when the Play Game menu entry is selected.
                /// </summary>
                protected void PlayGameMenuEntrySelected(Widget widge)
                {
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new GameplayScreen(this.m_game, this.m_params));
                }

                /// <summary>
                /// Event handler for when the Options menu entry is selected.
                /// </summary>
                protected void OptionsMenuEntrySelected(Widget widge)
                {
                    ScreenManager.AddScreen(new OptionsMenuScreen(this.m_game, ref this.m_params), PlayerIndex.One);
                }

                /// <summary>
                /// When the user cancels the main menu, ask if they want to exit the sample.
                /// </summary>
                protected override void OnCancel(Widget widge)
                {
                    const string message = "Are you sure you want to exit this sample?";
                    MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, this.m_game);
                    confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
                    ScreenManager.AddScreen(confirmExitMessageBox, m_controllingPlayer);
                }
                /// <summary>
                /// Event handler for when the user selects ok on the "are you sure
                /// you want to exit" message box.
                /// </summary>
                protected void ConfirmExitMessageBoxAccepted(Widget widge)
                {
                    ScreenManager.Game.Exit();
                }

            #endregion

            #region private

            #endregion

        #endregion

    }
}
