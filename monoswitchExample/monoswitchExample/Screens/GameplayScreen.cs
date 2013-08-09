#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ruminate.GUI.Framework;
using monoswitchExample.game;
using monoswitch;
using monoswitch.containers;
using monoswitch.content;
using monoswitch.misc;
using monoswitch.singletons;
using Ruminate.GUI.Content;
#endregion

namespace monoswitchExample
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {

        #region members

            #region public

                public const int MAX_STARS = 3;

            #endregion

            #region protected

                //statemanagement
                protected ContentManager m_content;
                protected SpriteFont m_gameFont;
                protected Random m_random = new Random();
                protected float m_pauseAlpha;

                //game objects
                protected int m_score;
                protected Texture2D m_starTexture;
                protected List<star> m_stars;
                protected Player m_player;
                //the timer
                protected gameTimer m_timer;
                //selection set
                protected selectionSet m_selectSet;
                //nl_selection set for future testing;
                //protected nl_SelectionSet m_nlss;
                //need a reference to the game
                protected exampleGame m_game;
                protected gameParams m_params;
                protected Viewport m_mainView;
                protected Viewport m_menuView;
                protected Viewport m_defViewPort;
                protected Texture2D m_menuTexture;
                
            #endregion

            #region private

                //hackyness.
                private bool m_inited;

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
                public GameplayScreen(exampleGame game, gameParams gparams)
                {
                    this.m_transitionOnTime = TimeSpan.FromSeconds(1.5);
                    this.m_transitionOffTime = TimeSpan.FromSeconds(0.5);
                    this.m_inited = false;
                    this.m_timer = null;
                    this.m_selectSet = null;
                    this.m_game = game;
                    this.m_menuTexture = new Texture2D(this.m_game.GraphicsDevice, 1, 1);
                    if (gparams == null)
                    {
                        this.m_params = new gameParams(gameParams.DEF_SCAN_RATE, gameParams.DEF_REFACTORY_PERIOD, gameParams.DEF_DISCRETE_HOLD, gameParams.DEF_GAME_TIME, gameParams.DEF_GOAL, gameParams.DEF_DISCRETE, gameParams.DEF_COMPOSITE, gameParams.DEF_MARKER);
                    }
                    else
                    {
                        this.m_params = gparams;
                    }
                    Console.WriteLine("the current goal is " + this.m_params.goal);
                    this.m_defViewPort = this.m_game.GraphicsDevice.Viewport;
                    this.m_menuView = new Viewport(this.m_defViewPort.X, this.m_defViewPort.Y, this.m_defViewPort.Width, 125);
                    this.m_mainView = new Viewport(this.m_defViewPort.X, this.m_defViewPort.Y + this.m_menuView.Height, this.m_defViewPort.Width, this.m_defViewPort.Height - this.m_menuView.Height);
                    //debg
                    Console.WriteLine("the status of menu view is " + this.m_menuView.Bounds);
                    Console.WriteLine("the status of the title safe area is " + this.m_menuView.TitleSafeArea);
                    Console.WriteLine("the status of the main area is " + this.m_mainView.Bounds);
                    Console.WriteLine("the status of the title safe area is " + this.m_mainView.TitleSafeArea);
                }

                /// <summary>
                /// Load graphics content for the game.
                /// </summary>
                public override void LoadContent()
                {
                    var beaker = m_game.Content.Load<Texture2D>("beaker");
                    int discSpeed = this.m_params.discreteHold;
                    if (!this.m_inited)
                    {
                        this.Initialize();
                    }
                    if (this.m_content == null)
                    {
                        this.m_content = new ContentManager(ScreenManager.Game.Services, "Content");
                    }
                    this.m_gameFont = this.m_content.Load<SpriteFont>("gamefont");
                    // Load the player resources
                    Animation playerAnimation = new Animation();
                    Texture2D playerTexture = this.m_content.Load<Texture2D>("shipAnimation");
                    playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);
                    Vector2 playerPosition = new Vector2(this.m_mainView.TitleSafeArea.X + this.m_mainView.TitleSafeArea.Width/2, this.m_mainView.TitleSafeArea.Y + this.m_mainView.TitleSafeArea.Height/2);
                    this.m_player.Initialize(playerAnimation, playerPosition, this.m_mainView);
                    this.m_starTexture = this.m_content.Load<Texture2D>("mineAnimation");
                    this.repopulateStars();
                    this.m_timer = null;
                    //set up your selection set or nl_selectionset
                    Skin skin = new Skin(this.m_game.ss_imgMap, this.m_game.ss_map);
                    Text text = new Text(this.m_game.ss_font, Color.Green);

                    //do the selection set
                    KeyDelegator KDELEGATOR = new KeyDelegator();
                    mkey signal = new mkey(MouseButton.Left);
                    signal.state = monoswitch.genericStates.DOWN;
                    this.m_selectSet = new selectionSet(this.m_game, skin, text, signal, KDELEGATOR);
                    if(!this.m_params.discrete && !this.m_params.composite)//if using basic switches
                    {
                        //DATA: SWITCHES AND GROUP
                        List<Keys> list1 = new List<Keys>();
                        list1.Add(Keys.D);
                        KeyGroup group1 = new KeyGroup(KDELEGATOR, list1);
                        s_switch temp1 = new s_switch(group1, 10, 2, 150, "right");
                        List<Keys> list2 = new List<Keys>();
                        list2.Add(Keys.W);
                        KeyGroup group2 = new KeyGroup(KDELEGATOR, list2);
                        s_switch temp2 = new s_switch(group2, 160, 2, 150, "up");
                        List<Keys> list3 = new List<Keys>();
                        list3.Add(Keys.A);
                        KeyGroup group3 = new KeyGroup(KDELEGATOR, list3);
                        s_switch temp3 = new s_switch(group3, 310, 2, 150, "left");
                        List<Keys> list4 = new List<Keys>();
                        list4.Add(Keys.S);
                        KeyGroup group4 = new KeyGroup(KDELEGATOR, list4);
                        s_switch temp4 = new s_switch(group4, 460, 2, 150, "down");

                        //NODES BASE GROUPS AND SWITCHNODES
                        KeyLogicNode noder = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        noder.log = logics.NONE;
                        noder.setData(group1);
                        KeyLogicNode nodeu = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        nodeu.log = logics.NONE;
                        nodeu.setData(group2);
                        KeyLogicNode nodel = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        nodel.log = logics.NONE;
                        nodel.setData(group3);
                        KeyLogicNode noded = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        noded.log = logics.NONE;
                        noded.setData(group4);
                        switchNode temp1Node = new switchNode(temp1);
                        switchNode temp2Node = new switchNode(temp2);
                        switchNode temp3Node = new switchNode(temp3);
                        switchNode temp4Node = new switchNode(temp4);
                        //SET UP, ASSIGN SUCCESSORS AND BUILD LOGIC HIERARCHY
                        temp1Node.addSuccessor(temp2Node);
                        temp2Node.addSuccessor(temp3Node);
                        temp3Node.addSuccessor(temp4Node);
                        temp4Node.addSuccessor(temp1Node);
                        switchNode[] nodeArr = { temp1Node, temp2Node, temp3Node, temp4Node };
                        KeyLogicNode lrxor = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        lrxor.log = logics.XOR;
                        lrxor.AddChild(nodel);
                        lrxor.AddChild(noder);
                        KeyLogicNode udxor = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        udxor.log = logics.XOR;
                        udxor.AddChild(nodeu);
                        udxor.AddChild(noded);
                       
                        //FINALIZE
                        this.m_selectSet.addLogic(lrxor);
                        this.m_selectSet.addLogic(udxor);
                        this.m_selectSet.assignNodes(nodeArr, KDELEGATOR);
                        this.m_selectSet.Commit(0, 0);
                        //marker
                        if (this.m_params.useMarker)
                        {
                            this.m_selectSet.marker = new marker(beaker, 0f, 1f, 33f);
                        }

                    }
                    else if(this.m_params.discrete && !this.m_params.composite)
                    {
                        List<Keys> list1 = new List<Keys>();
                        list1.Add(Keys.D);
                        KeyGroup group1 = new KeyGroup(KDELEGATOR, list1);
                        discrete_switch temp1 = new discrete_switch(group1, 10, 2, 150, "right", discSpeed);
                        List<Keys> list2 = new List<Keys>();
                        list2.Add(Keys.W);
                        KeyGroup group2 = new KeyGroup(KDELEGATOR, list2);
                        discrete_switch temp2 = new discrete_switch(group2, 160, 2, 150, "up", discSpeed);
                        List<Keys> list3 = new List<Keys>();
                        list3.Add(Keys.A);
                        KeyGroup group3 = new KeyGroup(KDELEGATOR, list3);
                        discrete_switch temp3 = new discrete_switch(group3, 310, 2, 150, "left", discSpeed);
                        List<Keys> list4 = new List<Keys>();
                        list4.Add(Keys.S);
                        KeyGroup group4 = new KeyGroup(KDELEGATOR, list4);
                        discrete_switch temp4 = new discrete_switch(group4, 460, 2, 150, "down", discSpeed);

                        //NODES BASE GROUPS AND SWITCHNODES
                        KeyLogicNode noder = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        noder.log = logics.NONE;
                        noder.setData(group1);
                        KeyLogicNode nodeu = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        nodeu.log = logics.NONE;
                        nodeu.setData(group2);
                        KeyLogicNode nodel = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        nodel.log = logics.NONE;
                        nodel.setData(group3);
                        KeyLogicNode noded = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        noded.log = logics.NONE;
                        noded.setData(group4);
                        switchNode temp1Node = new switchNode(temp1);
                        switchNode temp2Node = new switchNode(temp2);
                        switchNode temp3Node = new switchNode(temp3);
                        switchNode temp4Node = new switchNode(temp4);
                        //SET UP, ASSIGN SUCCESSORS AND BUILD LOGIC HIERARCHY
                        temp1Node.addSuccessor(temp2Node);
                        temp2Node.addSuccessor(temp3Node);
                        temp3Node.addSuccessor(temp4Node);
                        temp4Node.addSuccessor(temp1Node);
                        switchNode[] nodeArr = { temp1Node, temp2Node, temp3Node, temp4Node };
                        KeyLogicNode lrxor = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        lrxor.log = logics.XOR;
                        lrxor.AddChild(nodel);
                        lrxor.AddChild(noder);
                        KeyLogicNode udxor = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        udxor.log = logics.XOR;
                        udxor.AddChild(nodeu);
                        udxor.AddChild(noded);

                        //FINALIZE
                        this.m_selectSet.addLogic(lrxor);
                        this.m_selectSet.addLogic(udxor);
                        this.m_selectSet.assignNodes(nodeArr, KDELEGATOR);
                        this.m_selectSet.Commit(0, 0);

                        if (this.m_params.useMarker)
                        {
                            this.m_selectSet.marker = new marker(beaker, 0f, 1f, 33f);
                        }
                    }
                    else if(!this.m_params.discrete && this.m_params.composite)
                    {
                        List<Keys> list1 = new List<Keys>();
                        list1.Add(Keys.D);
                        KeyGroup group1 = new KeyGroup(KDELEGATOR, list1);
                        composite_switch temp1 = new composite_switch(group1, 10, 2, 140, "right");
                        List<Keys> list2 = new List<Keys>();
                        list2.Add(Keys.W);
                        KeyGroup group2 = new KeyGroup(KDELEGATOR, list2);
                        composite_switch temp2 = new composite_switch(group2, 290, 2, 140, "up");
                        List<Keys> list3 = new List<Keys>();
                        list3.Add(Keys.A);
                        KeyGroup group3 = new KeyGroup(KDELEGATOR, list3);
                        composite_switch temp3 = new composite_switch(group3, 570, 2, 150, "left");
                        List<Keys> list4 = new List<Keys>();
                        list4.Add(Keys.S);
                        KeyGroup group4 = new KeyGroup(KDELEGATOR, list4);
                        composite_switch temp4 = new composite_switch(group4, 850, 2, 140, "down");

                        List<Keys> listUL = new List<Keys>();
                        listUL.Add(Keys.D);
                        listUL.Add(Keys.W);
                        KeyGroup groupUL = new KeyGroup(KDELEGATOR, listUL);
                        composite_switch temp1x5 = new composite_switch(groupUL, 150, 2, 140, "upleft");
                        List<Keys> listUR = new List<Keys>();
                        listUR.Add(Keys.W);
                        listUR.Add(Keys.A);
                        KeyGroup groupUR = new KeyGroup(KDELEGATOR, listUR);
                        composite_switch temp2x5 = new composite_switch(groupUR, 430, 2, 140, "upright");
                        List<Keys> listRD = new List<Keys>();
                        listRD.Add(Keys.A);
                        listRD.Add(Keys.S);
                        KeyGroup groupRD = new KeyGroup(KDELEGATOR, listRD);
                        composite_switch temp3x5 = new composite_switch(groupRD, 710, 2, 140, "downright");
                        List<Keys> listRL = new List<Keys>();
                        listRL.Add(Keys.S);
                        listRL.Add(Keys.D);
                        KeyGroup groupRL = new KeyGroup(KDELEGATOR, listRL);
                        composite_switch temp4x5_0x5 = new composite_switch(groupRL, 990, 2, 140, "downleft");

                        //NODES BASE GROUPS AND SWITCHNODES
                        KeyLogicNode noder = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        noder.log = logics.NONE;
                        noder.setData(group1);
                        KeyLogicNode nodeu = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        nodeu.log = logics.NONE;
                        nodeu.setData(group2);
                        KeyLogicNode nodel = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        nodel.log = logics.NONE;
                        nodel.setData(group3);
                        KeyLogicNode noded = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        noded.log = logics.NONE;
                        noded.setData(group4);
                        switchNode temp1Node = new switchNode(temp1);
                        switchNode temp1x5Node = new switchNode(temp1x5);
                        switchNode temp2Node = new switchNode(temp2);
                        switchNode temp2x5Node = new switchNode(temp2x5);
                        switchNode temp3Node = new switchNode(temp3);
                        switchNode temp3x5Node = new switchNode(temp3x5);
                        switchNode temp4Node = new switchNode(temp4);
                        switchNode temp4x5_0x5Node = new switchNode(temp4x5_0x5);
                        //SET UP, ASSIGN SUCCESSORS AND BUILD LOGIC HIERARCHY
                        temp1Node.addSuccessor(temp1x5Node);
                        temp1x5Node.addSuccessor(temp2Node);
                        temp2Node.addSuccessor(temp2x5Node);
                        temp2x5Node.addSuccessor(temp3Node);
                        temp3Node.addSuccessor(temp3x5Node);
                        temp3x5Node.addSuccessor(temp4Node);
                        temp4Node.addSuccessor(temp4x5_0x5Node);
                        temp4x5_0x5Node.addSuccessor(temp1Node);
                        switchNode[] nodeArr = { temp1Node, temp1x5Node, temp2Node, temp2x5Node, temp3Node, temp3x5Node, temp4Node, temp4x5_0x5Node };
                        KeyLogicNode lrxor = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        lrxor.log = logics.XOR;
                        lrxor.AddChild(nodel);
                        lrxor.AddChild(noder);
                        KeyLogicNode udxor = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        udxor.log = logics.XOR;
                        udxor.AddChild(nodeu);
                        udxor.AddChild(noded);

                        //FINALIZE
                        this.m_selectSet.addLogic(lrxor);
                        this.m_selectSet.addLogic(udxor);
                        this.m_selectSet.assignNodes(nodeArr, KDELEGATOR);
                        this.m_selectSet.Commit(0, 0);

                        if (this.m_params.useMarker)
                        {
                            this.m_selectSet.marker = new marker(beaker, 0f, 1f, 33f);
                        }
                    }
                    else if(this.m_params.discrete && this.m_params.composite)
                    {
                        List<Keys> list1 = new List<Keys>();
                        list1.Add(Keys.D);
                        KeyGroup group1 = new KeyGroup(KDELEGATOR, list1);
                        discretecomposite_switch temp1 = new discretecomposite_switch(group1, 10, 2, 140, "right", discSpeed);
                        List<Keys> list2 = new List<Keys>();
                        list2.Add(Keys.W);
                        KeyGroup group2 = new KeyGroup(KDELEGATOR, list2);
                        discretecomposite_switch temp2 = new discretecomposite_switch(group2, 290, 2, 140, "up", discSpeed);
                        List<Keys> list3 = new List<Keys>();
                        list3.Add(Keys.A);
                        KeyGroup group3 = new KeyGroup(KDELEGATOR, list3);
                        discretecomposite_switch temp3 = new discretecomposite_switch(group3, 570, 2, 140, "left", discSpeed);
                        List<Keys> list4 = new List<Keys>();
                        list4.Add(Keys.S);
                        KeyGroup group4 = new KeyGroup(KDELEGATOR, list4);
                        discretecomposite_switch temp4 = new discretecomposite_switch(group4, 850, 2, 140, "down", discSpeed);

                        List<Keys> listUL = new List<Keys>();
                        listUL.Add(Keys.D);
                        listUL.Add(Keys.W);
                        KeyGroup groupUL = new KeyGroup(KDELEGATOR, listUL);
                        discretecomposite_switch temp1x5 = new discretecomposite_switch(groupUL, 150, 2, 140, "upleft", 500000f);
                        List<Keys> listUR = new List<Keys>();
                        listUR.Add(Keys.W);
                        listUR.Add(Keys.A);
                        KeyGroup groupUR = new KeyGroup(KDELEGATOR, listUR);
                        discretecomposite_switch temp2x5 = new discretecomposite_switch(groupUR, 430, 2, 140, "upright", 500000f);
                        List<Keys> listRD = new List<Keys>();
                        listRD.Add(Keys.A);
                        listRD.Add(Keys.S);
                        KeyGroup groupRD = new KeyGroup(KDELEGATOR, listRD);
                        discretecomposite_switch temp3x5 = new discretecomposite_switch(groupRD, 710, 2, 140, "downright", 500000f);
                        List<Keys> listRL = new List<Keys>();
                        listRL.Add(Keys.S);
                        listRL.Add(Keys.D);
                        KeyGroup groupRL = new KeyGroup(KDELEGATOR, listRL);
                        discretecomposite_switch temp4x5_0x5 = new discretecomposite_switch(groupRL, 990, 2, 140, "downleft", 500000f);

                        //NODES BASE GROUPS AND SWITCHNODES
                        KeyLogicNode noder = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        noder.log = logics.NONE;
                        noder.setData(group1);
                        KeyLogicNode nodeu = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        nodeu.log = logics.NONE;
                        nodeu.setData(group2);
                        KeyLogicNode nodel = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        nodel.log = logics.NONE;
                        nodel.setData(group3);
                        KeyLogicNode noded = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        noded.log = logics.NONE;
                        noded.setData(group4);
                        switchNode temp1Node = new switchNode(temp1);
                        switchNode temp1x5Node = new switchNode(temp1x5);
                        switchNode temp2Node = new switchNode(temp2);
                        switchNode temp2x5Node = new switchNode(temp2x5);
                        switchNode temp3Node = new switchNode(temp3);
                        switchNode temp3x5Node = new switchNode(temp3x5);
                        switchNode temp4Node = new switchNode(temp4);
                        switchNode temp4x5_0x5Node = new switchNode(temp4x5_0x5);
                        //SET UP, ASSIGN SUCCESSORS AND BUILD LOGIC HIERARCHY
                        temp1Node.addSuccessor(temp1x5Node);
                        temp1x5Node.addSuccessor(temp2Node);
                        temp2Node.addSuccessor(temp2x5Node);
                        temp2x5Node.addSuccessor(temp3Node);
                        temp3Node.addSuccessor(temp3x5Node);
                        temp3x5Node.addSuccessor(temp4Node);
                        temp4Node.addSuccessor(temp4x5_0x5Node);
                        temp4x5_0x5Node.addSuccessor(temp1Node);
                        switchNode[] nodeArr = { temp1Node, temp1x5Node, temp2Node, temp2x5Node, temp3Node, temp3x5Node, temp4Node, temp4x5_0x5Node };
                        KeyLogicNode lrxor = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        lrxor.log = logics.XOR;
                        lrxor.AddChild(nodel);
                        lrxor.AddChild(noder);
                        KeyLogicNode udxor = new KeyLogicNode(KDELEGATOR, 2, true, true, true);
                        udxor.log = logics.XOR;
                        udxor.AddChild(nodeu);
                        udxor.AddChild(noded);

                        //FINALIZE
                        this.m_selectSet.addLogic(lrxor);
                        this.m_selectSet.addLogic(udxor);
                        this.m_selectSet.assignNodes(nodeArr, KDELEGATOR);
                        this.m_selectSet.Commit(0, 0);

                        if (this.m_params.useMarker)
                        {
                            this.m_selectSet.marker = new marker(beaker, 0f, 1f, 33f);
                        }
                    }
                    this.m_selectSet.sendKeyDown += this.kdown;
                    this.m_selectSet.sendKeyUp += this.kup;
                    //do the nl selection set
                    //this.m_nlss = null;

                    // A real game would probably have more content than this sample, so
                    // it would take longer to load. We simulate that by delaying for a
                    // while, giving you a chance to admire the beautiful loading screen.
                    Thread.Sleep(1000);
                    // once the load has finished, we use ResetElapsedTime to tell the game's
                    // timing mechanism that we have just finished a very long frame, and that
                    // it should not try to catch up.
                    ScreenManager.Game.ResetElapsedTime();
                }

                /// <summary>
                /// Unload graphics content used by the game.
                /// </summary>
                public override void UnloadContent()
                {
                    this.m_content.Unload();
                }

                /// <summary>
                /// Updates the state of the game. This method checks the GameScreen.IsActive
                /// property, so the game will stop updating when the pause menu is active,
                /// or if you tab away to a different application.
                /// </summary>
                public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
                {
                    base.Update(gameTime, otherScreenHasFocus, false);
                    // Gradually fade in or out depending on whether we are covered by the pause screen.
                    if (coveredByOtherScreen)
                    {
                        this.m_pauseAlpha = Math.Min(this.m_pauseAlpha + 1f / 32, 1);
                    }
                    else
                    {
                        this.m_pauseAlpha = Math.Max(this.m_pauseAlpha - 1f / 32, 0);
                    }
                    UpdatePlayer(gameTime);
                    UpdateStars(gameTime);
                    // Update the collision
                    UpdateCollision();
                    //update the selection sets
                    if (this.m_selectSet != null)
                    {
                        this.m_selectSet.Update();
                        this.m_selectSet.UpdateByTime(gameTime);
                    }
                    if (this.m_timer == null)
                    {
                        this.m_timer = new gameTimer();
                        this.m_timer.set(gameTime, this.m_params.gameTime);//switch to params
                        //add your event handler
                        this.m_timer.timerComplete += this.respondTimerComplete;
                    }
                    else
                    {
                        this.m_timer.Update(gameTime);
                    }

                }

                /// <summary>
                /// Lets the game respond to player input. Unlike the Update method,
                /// this will only be called when the gameplay screen is active.
                /// </summary>
                public override void HandleInput(InputState input)
                {
                    if (input == null)
                    {
                        throw new ArgumentNullException("input");
                    }
                    // Look up inputs for the active player profile.
                    int playerIndex = (int)this.controllingPlayer.Value;
                    KeyboardState keyboardState = input.currentKeyboardStates[playerIndex];
                    GamePadState gamePadState = input.currentGamePadStates[playerIndex];
                    // The game pauses either if the user presses the pause button, or if
                    // they unplug the active gamepad. This requires us to keep track of
                    // whether a gamepad was ever plugged in, because we don't want to pause
                    // on PC if they are playing with a keyboard and have no gamepad at all!
                    bool gamePadDisconnected = !gamePadState.IsConnected && input.gamePadWasConnected[playerIndex];
                    if (input.IsPauseGame(this.controllingPlayer) || gamePadDisconnected)
                    {
                        ScreenManager.AddScreen(new PauseMenuScreen(this.m_game), this.controllingPlayer);
                    }
                    else
                    {
                        //have the player handle input
                        this.m_player.HandleInput(input.currentKeyboardStates[playerIndex], input.currentGamePadStates[playerIndex], (!gamePadDisconnected && gamePadState.IsConnected));
                    }
                }

                private void UpdatePlayer(GameTime gameTime)
                {
                    this.m_player.Update(gameTime);
                }

                private void UpdateStars(GameTime gameTime)
                {
                    
                    // Update the stars
                    for (int i = this.m_stars.Count - 1; i >= 0; i--)
                    {
                        this.m_stars[i].Update(gameTime);
                        if (this.m_stars[i].active == false)
                        {
                            // If not active and health <= 0
                            if (this.m_stars[i].health <= 0)
                            {
                                //Add to the player's score
                                this.m_score += this.m_stars[i].value;
                            }
                            this.m_stars.RemoveAt(i);
                        }
                    }
                    this.repopulateStars();//get more stars
                }

                
                /// <summary>
                /// Draws the gameplay screen.
                /// </summary>
                public override void Draw(GameTime gameTime)
                {
                    ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);
                    //after clearing the background, switch to the game viewport and draw
                    ScreenManager.GraphicsDevice.Viewport = this.m_mainView;
                    SpriteBatch spriteBatch = ScreenManager.spriteBatch;
                    spriteBatch.Begin();
                    this.m_player.Draw(spriteBatch);
                    foreach (star sVal in this.m_stars)//draw the stars
                    {
                        sVal.Draw(spriteBatch);
                    }
                    spriteBatch.End();
                    //switch ports again
                    ScreenManager.GraphicsDevice.Viewport = this.m_menuView;
                    //fill in a rectangle
                    this.m_menuTexture.SetData(new Color[] { Color.Blue });
                    //draw your timer
                    spriteBatch = ScreenManager.spriteBatch;
                    spriteBatch.Begin();
                    spriteBatch.Draw(this.m_menuTexture, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea, Color.Blue);
                    if (this.m_timer != null && this.m_timer.isActive)
                    {
                        string tString = "Time: " + this.m_timer.displayValue;
                        string gString = ("Progresss: " + this.m_player.score + "\\" + this.m_params.goal);
                        spriteBatch.DrawString(this.m_gameFont, tString, new Vector2(this.m_menuView.X, this.m_menuView.Y +65), Color.Red);
                        spriteBatch.DrawString(this.m_gameFont, gString, new Vector2(this.m_menuView.X + 200, this.m_menuView.Y + 65), Color.White);
                    }
                    spriteBatch.End();
                    // If the game is transitioning on or off, fade it out to black.
                    //nests spritebatch.begin can;t put it above
                    if (this.m_selectSet != null)
                    {
                        this.m_selectSet.Draw();
                    }
                    //reset the viewport to original
                    ScreenManager.GraphicsDevice.Viewport = this.m_defViewPort;
                    if (this.m_transitionPosition > 0 || this.m_pauseAlpha > 0)
                    {
                        float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, this.m_pauseAlpha / 2);
                        ScreenManager.FadeBackBufferToBlack(alpha);
                    }
                    
                }

            #endregion

            #region protected

                protected void Initialize()
                {
                    //Set player's score to zero
                    this.m_score = 0;
                    this.m_player = new Player();
                    // Initialize the enemies list
                    this.m_stars = new List<star>();
                    // Initialize our random number generator
                    this.m_random = new Random();
                    //base.Initialize();
                }

                protected void repopulateStars()
                {
                    while (this.m_stars.Count < GameplayScreen.MAX_STARS)
                    {
                        star sVal = new star();
                        while (!this.reroll(ref sVal))
                        {
                        }
                        this.m_stars.Add(sVal);
                    }
                }

                protected bool reroll(ref star sVal)
                {
                    float xloc = this.m_random.Next(this.m_mainView.TitleSafeArea.Width) + this.m_mainView.TitleSafeArea.X;//(this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width) + this.ScreenManager.GraphicsDevice.Viewport.X;
                    float yloc = this.m_random.Next(this.m_mainView.TitleSafeArea.Height) + this.m_mainView.TitleSafeArea.Y;//(this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height) + this.ScreenManager.GraphicsDevice.Viewport.Y;
                    Vector2 loc = new Vector2(xloc, yloc);
                    Animation anim = new Animation();
                    anim.Initialize(this.m_starTexture, loc, 47, 61, 8, 30,Color.White, 1f, true);
                    sVal.Initialize(anim, loc, this.m_mainView);//this.ScreenManager.GraphicsDevice.Viewport);
                    foreach (star sVal2 in this.m_stars)
                    {
                        if(safeCircleIntersects(sVal2, sVal))
                        {
                            return false;
                        }
                    }
                    if (safeCircleIntersects(sVal, this.m_player))
                    {
                        return false;
                    }
                    return true;
                }

                protected bool safeCircleIntersects(star sVal1, star sVal2)
                {
                    return this.circleIntersects(sVal1, sVal2, 1.2f);
                }

                protected bool safeCircleIntersects(star sVal1, Player pVal1)
                {
                    return this.circleIntersects(sVal1, pVal1, 1.2f);
                }

                protected bool closeCircleIntersects(star sVal1, star sVal2)
                {
                    return this.circleIntersects(sVal1, sVal2, 0.65f);
                }

                protected bool closeCircleIntersects(star sVal1, Player pVal1)
                {
                    return this.circleIntersects(sVal1, pVal1, 0.65f);
                }

                protected bool circleIntersects(star sVal1, star sVal2, float scale)
                {
                    float eighthX = this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width / 8;
                    float eighth8X = this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width - eighthX;
                    float eighthY = this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height / 8;
                    float eighth8Y = this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height - eighthY;
                    float dif = Math.Abs(sVal1.radius * scale) + Math.Abs(sVal2.radius * scale);
                    float standardX1 = sVal1.position.X - this.ScreenManager.GraphicsDevice.Viewport.X;
                    float standardX2 = sVal2.position.X - this.ScreenManager.GraphicsDevice.Viewport.X;
                    float standardY1 = sVal1.position.Y - this.ScreenManager.GraphicsDevice.Viewport.Y;
                    float standardY2 = sVal2.position.Y - this.ScreenManager.GraphicsDevice.Viewport.Y;
                    float altX1;
                    float altX2;
                    float altY1;
                    float altY2;
                    if(standardX1 < eighthX)
                    {
                        altX1 = standardX1 + this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    else if (standardX1 > eighth8X)
                    {
                        altX1 = standardX1 - this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    else
                    {
                        altX1 = standardX1;
                    }
                    if(standardX2 < eighthX)
                    {
                        altX2 = standardX2 + this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    else if (standardX2 > eighth8X)
                    {
                        altX2 = standardX2 - this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    else
                    {
                        altX2 = standardX2;
                    }
                    if(standardY1 < eighthY)
                    {
                        altY1 = standardY1 + this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    else if (standardY1 > eighth8Y)
                    {
                        altY1 = standardY1 - this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    else
                    {
                        altY1 = standardY1;
                    }
                    if(standardY2 < eighthY)
                    {
                        altY2 = standardY2 + this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    else if (standardY2 > eighth8Y)
                    {
                        altY2 = standardY2 - this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    else
                    {
                        altY2 = standardY2;
                    }
                    float tbase = 0f;
                    float taltitude = 0f;
                    float hyp = 0f;
                    //compare standard 1 to standard 2
                    tbase = standardX1-standardX2;
                    taltitude = standardY1-standardY2;
                    hyp = (float)Math.Sqrt(Math.Pow(tbase, 2) + Math.Pow(taltitude, 2));
                    if (hyp < dif)
                    {
                        return true;
                    }
                    //compare standard 1 to alt 2
                    tbase = standardX1 - altX2;
                    taltitude = standardY1 - altY2;
                    hyp = (float)Math.Sqrt(Math.Pow(tbase, 2) + Math.Pow(taltitude, 2));
                    if (hyp < dif)
                    {
                        return true;
                    }
                    //compare alt 1 to standard 2
                    tbase = altX1 - standardX2;
                    taltitude = altY1 - standardY2;
                    hyp = (float)Math.Sqrt(Math.Pow(tbase, 2) + Math.Pow(taltitude, 2));
                    if (hyp < dif)
                    {
                        return true;
                    }
                    return false;
                }

                protected bool circleIntersects(star sVal1, Player pVal1, float scale)
                {
                    float eighthX = this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width / 8;
                    float eighth8X = this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width - eighthX;
                    float eighthY = this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height / 8;
                    float eighth8Y = this.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height - eighthY;
                    float dif = Math.Abs(sVal1.radius * scale) + Math.Abs(pVal1.radius * scale);
                    float standardX1 = sVal1.position.X - this.ScreenManager.GraphicsDevice.Viewport.X;
                    float standardX2 = pVal1.position.X - this.ScreenManager.GraphicsDevice.Viewport.X;
                    float standardY1 = sVal1.position.Y - this.ScreenManager.GraphicsDevice.Viewport.Y;
                    float standardY2 = pVal1.position.Y - this.ScreenManager.GraphicsDevice.Viewport.Y;
                    float altX1;
                    float altX2;
                    float altY1;
                    float altY2;
                    if (standardX1 < eighthX)
                    {
                        altX1 = standardX1 + this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    else if (standardX1 > eighth8X)
                    {
                        altX1 = standardX1 - this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    else
                    {
                        altX1 = standardX1;
                    }
                    if (standardX2 < eighthX)
                    {
                        altX2 = standardX2 + this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    else if (standardX2 > eighth8X)
                    {
                        altX2 = standardX2 - this.ScreenManager.GraphicsDevice.Viewport.Width;
                    }
                    else
                    {
                        altX2 = standardX2;
                    }
                    if (standardY1 < eighthY)
                    {
                        altY1 = standardY1 + this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    else if (standardY1 > eighth8Y)
                    {
                        altY1 = standardY1 - this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    else
                    {
                        altY1 = standardY1;
                    }
                    if (standardY2 < eighthY)
                    {
                        altY2 = standardY2 + this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    else if (standardY2 > eighth8Y)
                    {
                        altY2 = standardY2 - this.ScreenManager.GraphicsDevice.Viewport.Height;
                    }
                    else
                    {
                        altY2 = standardY2;
                    }
                    float tbase = 0f;
                    float taltitude = 0f;
                    float hyp = 0f;
                    //compare standard 1 to standard 2
                    tbase = standardX1 - standardX2;
                    taltitude = standardY1 - standardY2;
                    hyp = (float)Math.Sqrt(Math.Pow(tbase, 2) + Math.Pow(taltitude, 2));
                    if (hyp < dif)
                    {
                        return true;
                    }
                    //compare standard 1 to alt 2
                    tbase = standardX1 - altX2;
                    taltitude = standardY1 - altY2;
                    hyp = (float)Math.Sqrt(Math.Pow(tbase, 2) + Math.Pow(taltitude, 2));
                    if (hyp < dif)
                    {
                        return true;
                    }
                    //compare alt 1 to standard 2
                    tbase = altX1 - standardX2;
                    taltitude = altY1 - standardY2;
                    hyp = (float)Math.Sqrt(Math.Pow(tbase, 2) + Math.Pow(taltitude, 2));
                    if (hyp < dif)
                    {
                        return true;
                    }
                    return false;
                }

                protected void respondTimerComplete(object sender, EventArgs e)
                {//also use for achieve score
                    ScreenManager.AddScreen(new BackgroundScreen(), this.controllingPlayer);
                    ScreenManager.AddScreen(new MainMenuScreen(this.m_game), this.controllingPlayer);
                    this.ExitScreen();
                }


            #endregion

            #region private

                private void UpdateCollision()
                {
                    // Do the collision between the player and the stars
                    for (int i = 0; i < this.m_stars.Count; i++)
                    {
                        if (closeCircleIntersects(this.m_stars[i], this.m_player))
                        {
                            this.m_stars[i].active = false;
                            this.m_player.addScore();
                            if (this.m_player.score >= this.m_params.goal)
                            {
                                this.respondTimerComplete(null, new EventArgs());
                            }
                        }
                    }
                }

                private void kdown(Object o, KeyEventArgs e)
                {
                    if (e.KeyCode == Keys.D)
                    {
                        this.m_player.dpressed = true;
                    }
                    else if (e.KeyCode == Keys.W)
                    {
                        this.m_player.wpressed = true;
                    }
                    else if (e.KeyCode == Keys.A)
                    {
                        this.m_player.apressed = true;
                    }
                    else if (e.KeyCode == Keys.S)
                    {
                        this.m_player.spressed = true;
                    }

                }

                private void kup(Object o, KeyEventArgs e)
                {
                    if (e.KeyCode == Keys.D)
                    {
                        this.m_player.dpressed = false;
                    }
                    else if (e.KeyCode == Keys.W)
                    {
                        this.m_player.wpressed = false;
                    }
                    else if (e.KeyCode == Keys.A)
                    {
                        this.m_player.apressed = false;
                    }
                    else if (e.KeyCode == Keys.S)
                    {
                        this.m_player.spressed = false;
                    }

                }

            #endregion

        #endregion

    }
}
