#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Ruminate.GUI.Content;
using Ruminate.GUI.Framework;
#endregion

namespace monoswitchExample
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {

        #region members

            #region public

            #endregion

            #region protected

                protected gameParams m_params;
                protected SingleLineTextBox m_scanBox;
                protected SingleLineTextBox m_refBox;

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
                /// Constructor.
                /// </summary>
                public OptionsMenuScreen(exampleGame game, gameParams gparams) : base("Options", game)
                {
                    this.m_menuEntries = new List<List<Widget>>();
                    
                    if (gparams != null)
                    {
                        this.m_params = gparams;
                    }
                    else
                    {
                        this.m_params = new gameParams(gameParams.DEF_SCAN_RATE, gameParams.DEF_REFACTORY_PERIOD, gameParams.DEF_DISCRETE, gameParams.DEF_COMPOSITE, gameParams.DEF_MARKER);
                    }
                    int midPoint = this.m_game.GraphicsDevice.Viewport.X + this.m_game.GraphicsDevice.Viewport.Width / 2;
                    int vert = this.m_game.GraphicsDevice.Viewport.Y + this.m_game.GraphicsDevice.Viewport.Height / 12;
                    CheckBox discBox = new CheckBox(-100, 0, "discrete");
                    discBox.IsToggled = this.m_params.discrete;
                    discBox.OnToggle = delegate(Widget widget) { this.m_params.discrete = true; };
                    discBox.OffToggle = delegate(Widget widget) { this.m_params.discrete = false; };
                    CheckBox compBox = new CheckBox(-50, 0, "composite");
                    compBox.IsToggled = this.m_params.composite;
                    compBox.OnToggle = delegate(Widget widget) { this.m_params.composite = true; };
                    compBox.OffToggle = delegate(Widget widget) { this.m_params.composite = false; };
                    CheckBox markerBox = new CheckBox(-100, 0, "use marker");
                    markerBox.IsToggled = this.m_params.useMarker;
                    markerBox.OnToggle = delegate(Widget widget) { this.m_params.useMarker = true; };
                    markerBox.OffToggle = delegate(Widget widget) { this.m_params.useMarker = false; };
                    this.m_scanBox = new SingleLineTextBox(-100, 0, 100, 4);
                    this.m_scanBox.AbsoluteArea = new Rectangle(-100, 0, 100, 35);
                    this.m_scanBox.Text = (this.m_params.scanRate).ToString();
                    Button scanUp = new Button(0, 0, 50, "+", this.incScan);
                    Button scanDown = new Button(0, 0, 50, "-", this.decScan);
                    this.m_refBox = new SingleLineTextBox(-100, 0, 100, 4);
                    this.m_refBox.AbsoluteArea = new Rectangle(-100, 0, 100, 35);
                    this.m_refBox.Text = (this.m_params.refactoryPeriod / 1000).ToString();
                    Button refUp = new Button(0, 0, 50, "+", this.incRef);
                    Button refDown = new Button(0, 0, 50, "-", this.decRef);
                    Button back = new Button(0, 0, 100, "BACK", OnCancel);
                    List<Widget> l1 = new List<Widget>();
                    l1.Add(discBox);
                    l1.Add(compBox);
                    List<Widget> l2 = new List<Widget>();
                    l2.Add(markerBox);
                    List<Widget> l3 = new List<Widget>();
                    l3.Add(this.m_scanBox);
                    List<Widget> l4 = new List<Widget>();
                    l4.Add(scanUp);
                    l4.Add(scanDown);
                    List<Widget> l5 = new List<Widget>();
                    l5.Add(this.m_refBox);
                    List<Widget> l6 = new List<Widget>();
                    l6.Add(refUp);
                    l6.Add(refDown);
                    List<Widget> l7 = new List<Widget>();
                    l7.Add(back);
                    this.m_menuEntries.Add(l1);
                    this.m_menuEntries.Add(l2);
                    this.m_menuEntries.Add(l3);
                    this.m_menuEntries.Add(l4);
                    this.m_menuEntries.Add(l5);
                    this.m_menuEntries.Add(l6);
                    this.m_menuEntries.Add(l7);
                    this.m_gui.Widgets = new Widget[]
                    {
                            discBox, compBox, markerBox, m_scanBox, scanUp, scanDown, m_refBox, refUp, refDown, back
                    };
                }

                protected void incScan(Widget widge)
                {
                    this.m_params.incrementScanningRate();
                    this.m_scanBox.Text = this.m_params.scanRate.ToString();
                }

                protected void decScan(Widget widge)
                {
                    this.m_params.decrementScanningRate();
                    this.m_scanBox.Text = this.m_params.scanRate.ToString();
                }

                protected void incRef(Widget widge)
                {
                    this.m_params.incrementRefactoryPeriod();
                    this.m_refBox.Text = (this.m_params.refactoryPeriod / 1000).ToString();
                }

                protected void decRef(Widget widge)
                {
                    this.m_params.decrementRefactoryPeriod();
                    this.m_refBox.Text = (this.m_params.refactoryPeriod / 1000).ToString();
                }



            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

    }
}
