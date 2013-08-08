﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ruminate.GUI.Framework;
using Ruminate.GUI.Content;
using Microsoft.Xna.Framework.Graphics;
using monoswitch.containers;
using monoswitch.content;
using monoswitch;
using monoswitch.singletons;

//authors note: 
//as toggle button was a sealed class in the ruminate framework, 
//this class reproduces code from toggle button as opposed to extending it


namespace monoswitch.content
{

    public delegate void switchEvent(s_switch sender);

    public class s_switch: Ruminate.GUI.Framework.WidgetBase<Ruminate.GUI.Content.ButtonRenderRule>//standard switch.
    {//reproduce functionality of toggle button
        #region members

            #region public

                //static const

                //enums


            #endregion

            #region internal

            #endregion

            #region protected

                protected TimeSpan m_scanningRate;
                protected KeyGroup m_group;
                protected Boolean m_commited;
                //based off toggle button
                protected bool m_switchedOn;
                protected int m_textPadding;
                protected int m_width;
                protected switchNode m_switchnode;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public
                //based on toggle button

                /// <summary>
                /// Label displaced on the button.
                /// </summary>
                public string label
                {
                    get { return RenderRule.Label; }
                    set { RenderRule.Label = value; }
                }
                
                /// <summary>
                /// The gap between the text and the edge of the button.
                /// </summary>
                public int textPadding
                {
                    get
                    {
                        return this.m_textPadding;
                    }
                    set
                    {
                        this.m_textPadding = value;
                        this.m_width = 0;
                    }
                }

                /// <summary>
                /// The width of the button.
                /// </summary>
                public int width
                {
                    get
                    {
                        return this.m_width;
                    }
                    set
                    {
                        this.m_width = value;
                        this.m_textPadding = 0;
                    }
                }

                public TimeSpan scanningRate
                {
                    get
                    {
                        return this.m_scanningRate;
                    }
                }

                public KeyGroup controllers
                {
                    get
                    {
                        return this.m_group;
                    }
                }

                public Boolean commited
                {
                    get
                    {
                        return this.m_commited;
                    }
                }
                
                /// <summary>
                /// Returns true when the button is pressed and false when
                /// released.
                /// </summary>
                public bool switchedOn
                {
                    get
                    {
                        return m_switchedOn;
                    }
                }

                public switchNode switchnode
                {
                    get
                    {
                        return this.m_switchnode;
                    }
                    set
                    {
                        this.m_switchnode = value;
                    }
                }
                

            #endregion

            #region internal

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region events

            #region public

                public event switchEvent OnToggle;
                public event switchEvent OffToggle;

            #endregion

            #region internal

            #endregion

            #region protected

                protected virtual void RaiseOnToggle()
                {
                    if (this.OnToggle != null)
                    {
                        this.OnToggle(this);
                    }
                }

                protected virtual void RaiseOffTogle()
                {
                    if (this.OffToggle != null)
                    {
                        this.OffToggle(this);
                    }
                }

                protected internal new virtual void EnterHover()
                {
                    this.highlight();
                }

                protected internal new virtual void ExitHover()
                {
                    this.deHighlight();
                }

                protected internal virtual void respKeyChange(ILogicState kPair, float interval)
                {
                    if (this.m_group.isTrue)
                    {
                        if(this.switchedOn == false)
                        {
                            this.m_switchedOn = true;
                        }
                        if (this.OnToggle != null)
                        {
                            this.OnToggle(this);
                        }
                    }
                    if (this.m_group.isFalse)
                    {
                        if (this.switchedOn == true)
                        {
                            this.m_switchedOn = true;
                        }
                        if (this.OffToggle != null)
                        {
                            this.OffToggle(this);
                        }
                    }
                }

            #endregion

            #region private

            #endregion

        #endregion

        #region methods

            #region public
                //CONSTRUCTOR
                public s_switch(KeyDelegator kdel) : this(kdel, new List<Keys>())
                {
                }

                public s_switch(KeyDelegator kdel, List<Keys> kList)
                {
                    //set graphics
                    this.Area = new Rectangle(0, 0, 0, 0);
                    this.label = label;
                    this.textPadding = 0;
                    //set events
                    this.OnToggle += this.pressSwitch;
                    this.OffToggle += this.unpressSwitch;
                    //set controls
                    this.m_switchedOn = false;
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, 0);
                    this.setGroup(kdel, kList);
                    this.m_commited = false;
                }

                public s_switch(KeyGroup kgroup)
                {
                    //set controls
                    this.Area = new Rectangle(0, 0, 0, 0);
                    this.label = label;
                    this.textPadding = 0;
                    //set events
                    this.OnToggle += this.pressSwitch;
                    this.OffToggle += this.unpressSwitch;
                    //set controls
                    this.m_switchedOn = false;
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, 0);
                    this.setGroup(kgroup);
                    this.m_commited = false;
                }

                public s_switch(KeyDelegator kdel, int scanR) : this(kdel)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, Math.Abs(scanR));
                }

                public s_switch(KeyDelegator kdel, TimeSpan scanR) : this(kdel)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, (int)scanR.TotalMilliseconds);
                }

                public s_switch(KeyDelegator kdel, List<Keys> kList, int scanR) : this(kdel, kList)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, Math.Abs(scanR));
                }

                public s_switch(KeyDelegator kdel, List<Keys> kList, TimeSpan scanR)
                    : this(kdel, kList)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, (int)scanR.TotalMilliseconds);
                }

                public s_switch(KeyGroup kg, int scanR) : this(kg)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, Math.Abs(scanR));
                }

                public s_switch(KeyGroup kg, TimeSpan scanR) : this(kg)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, (int)scanR.TotalMilliseconds);
                }

                /// <summary>
                /// Creates a new button at the location specified. The button defaults to
                /// the height of the RenderRule and width of the label.
                /// </summary>
                /// <param name="x">The X coordinate of the widget.</param>
                /// <param name="y">The Y coordinate of the widget.</param>
                /// <param name="label">The label to be rendered on the button.</param>
                /// <param name="padding">If specified the padding on either side of the label.</param>
                public s_switch(KeyDelegator kdel, int x, int y, string label, int padding = 2)
                    : this(kdel, new List<Keys>(), x, y, label, padding)
                {
                }

                public s_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, string label, int padding = 2)
                {
                    //set graphics
                    this.Area = new Rectangle(x, y, 0, 0);
                    this.label = label;
                    this.textPadding = padding;
                    //set events
                    this.OnToggle += this.pressSwitch;
                    this.OffToggle += this.unpressSwitch;
                    //set controls
                    this.m_switchedOn = false;
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, 0);
                    setGroup(kdel, kList);
                    this.m_commited = false;
                }

                public s_switch(KeyGroup kgroup, int x, int y, string label, int padding = 2)
                {
                    //set graphics
                    this.Area = new Rectangle(x, y, 0, 0);
                    this.label = label;
                    this.textPadding = padding;
                    //set events
                    this.OnToggle += this.pressSwitch;
                    this.OffToggle += this.unpressSwitch;
                    //set controls
                    this.m_switchedOn = false;
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, 0);
                    setGroup(kgroup);
                    this.m_commited = false;
                }

                public s_switch(KeyDelegator kdel, int x, int y, string label, TimeSpan scanR, int padding = 2) : this(kdel, x, y, label, padding)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, (int)scanR.TotalMilliseconds);
                }

                //int not allowed as an input due to padding
                public s_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, string label, TimeSpan scanR, int padding = 2) : this(kdel, kList, x, y, label, padding)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, (int)scanR.TotalMilliseconds);
                }

                public s_switch(KeyGroup kgroup, int x, int y, string label, TimeSpan scanR, int padding = 2) : this(kgroup, x, y, label, padding)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, (int)scanR.TotalMilliseconds);
                }

                /// <summary>
                /// Creates a new button at the location specified. The button defaults to
                /// the height of the RenderRule and width of the label.
                /// </summary>
                /// <param name="x">The X coordinate of the widget.</param>
                /// <param name="y">The Y coordinate of the widget.</param>
                /// <param name="width">The width of the Button. Ignored if the width is less that the width of the label.</param>
                /// <param name="label">The label to be rendered on the button.</param>
                public s_switch(KeyDelegator kdel, int x, int y, int width, string label)
                    : this(kdel, new List<Keys>(), x, y, width, label)
                {
                }

                public s_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, int width, string label)
                {
                    //set graphics
                    this.Area = new Rectangle(x, y, 0, 0);
                    this.width = width;
                    this.label = label;
                    //set events
                    this.OnToggle += this.pressSwitch;
                    this.OffToggle += this.unpressSwitch;
                    //setcontrols
                    this.m_switchedOn = false;
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, 0);
                    setGroup(kdel, kList);
                    this.m_commited = false;
                }

                public s_switch(KeyGroup kgroup, int x, int y, int width, string label)
                {
                    //set graphics
                    this.Area = new Rectangle(x, y, 0, 0);
                    this.width = width;
                    this.label = label;
                    //set events
                    this.OnToggle += this.pressSwitch;
                    this.OffToggle += this.unpressSwitch;
                    //set controls
                    this.m_switchedOn = false;
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, 0);
                    setGroup(kgroup);
                    this.m_commited = false;
                }

                public s_switch(KeyDelegator kdel, int x, int y, int width, string label, TimeSpan scanR) :this(kdel, x, y, width, label)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, (int)scanR.TotalMilliseconds);
                }

                public s_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, int width, string label, TimeSpan scanR) : this(kdel, kList, x, y, width, label)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, (int)scanR.TotalMilliseconds);
                }

                public s_switch(KeyGroup kgroup, int x, int y, int width, string label, TimeSpan scanR) : this(kgroup, x, y, width, label)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, (int)scanR.TotalMilliseconds);
                }

                public s_switch(KeyDelegator kdel, int x, int y, int width, string label, int scanR) : this(kdel, x, y, width, label)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, Math.Abs(scanR));
                }

                public s_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, int width, string label, int scanR) : this(kdel, kList, x, y, width, label)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, Math.Abs(scanR));
                }

                public s_switch(KeyGroup kgroup, int x, int y, int width, string label, int scanR) : this(kgroup, x, y, width, label)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, Math.Abs(scanR));
                }


                //INITIALIZATION FUNCTIONS

                protected override ButtonRenderRule BuildRenderRule()
                {
                    return new ButtonRenderRule();
                }
                protected override void Attach()
                {
                    int minWidth = (int)this.RenderRule.Font.MeasureString(this.label).X + (2 * this.RenderRule.Edge);
                    if (this.textPadding > 0)
                    {
                        this.Area = new Rectangle(this.Area.X, this.Area.Y, minWidth + (this.textPadding * 2), this.RenderRule.Height);
                    }
                    else if (this.width > 0)
                    {
                        this.Area = new Rectangle(this.Area.X, this.Area.Y, (minWidth > this.width) ? minWidth : this.width, this.RenderRule.Height);
                    }
                }
                //ends initialization
                public Boolean commit()
                {
                    if (this.m_group == null || this.m_group.Count < 1)
                    {
                        this.m_commited = false;
                        return this.m_commited;
                    }
                    this.m_commited = true;
                    return this.m_commited;
                }
                //UPDATE
                protected override void Update()
                {//do nothing in gui update
                }
                //OTHER PUBLIC FUNCTIONS
                //switch the device
                public virtual void toggleSwitch()
                {
                    if (!m_switchedOn)//we are trying to activate
                    {
                        this.m_group.activate(new List<Keys>());
                        if (!this.m_group.isTrue)//if activation failed completely
                        {
                            if (OffToggle != null)
                            {
                                OffToggle(this);
                            }
                            this.m_switchedOn = false;
                        }
                        else
                        {
                            if (OnToggle != null)
                            {
                                OnToggle(this);
                            }
                            this.m_switchedOn = true;
                        }
                    }
                    else
                    {
                        this.m_group.deactivate(new List<Keys>());
                        if (this.m_group.isTrue)//if deactivation failed completely
                        {
                            if (OnToggle != null)
                            {
                                OnToggle(this);
                            }
                            this.m_switchedOn = true;
                        }
                        else
                        {
                            if (OffToggle != null)
                            {
                                OffToggle(this);
                            }
                            this.m_switchedOn = false;
                        }
                    }
                }

                public virtual logicStates respondSwitchToggled(ILogicState group, List<ILogicState> oldpairs, List<ILogicState> oldgroups, float interval = 0f)
                {
                    Console.WriteLine("responding to switch toggled");
                    if (this.m_group == group)
                    {
                        if (group.state == logicStates.TRUE)
                        {
                            Console.WriteLine("toggling on");
                            this.m_switchedOn = true;
                            if (OnToggle != null)
                            {
                                OnToggle(this);
                            }
                        }
                        else
                        {
                            Console.WriteLine("toggling off");
                            this.m_switchedOn = false;
                            if (OffToggle != null)
                            {
                                OffToggle(this);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("group does not match");
                    }
                    return logicStates.TRUE;
                }

                public void highlight()
                {
                    if (!this.switchedOn)
                    {
                        RenderRule.Mode = ButtonRenderRule.RenderMode.Hover;
                    }
                }
                public void deHighlight()
                {
                    if (!this.switchedOn)
                    {
                        RenderRule.Mode = ButtonRenderRule.RenderMode.Default;
                    }
                }

                public void UpdateByTime(GameTime gametime)
                {
                    return;//nothing yet
                }

            #endregion

            #region internal

            #endregion

            #region protected

                protected virtual void setGroup(KeyDelegator kdel, List<Keys> kList)
                {
                    if (this.m_group != null)
                    {
                        //remove this event listeners
                        if (this.switchnode != null && this.switchnode.parents != null && this.switchnode.parents.Count > 1)
                        {
                            foreach (selectionSet setVal in this.switchnode.parents)
                            {
                                if (setVal.containsNode(this.m_switchnode) && this.m_group != null)
                                {
                                    foreach (KeyPair kp in this.m_group.pairs)
                                    {
                                        if (setVal.containsPairBySwitch(kp))
                                        {
                                            kp.stateChangeSuccess -= setVal.respondKeyChanged;
                                        }
                                    }
                                }
                            }
                        }
                        foreach (KeyPair kp in this.m_group.pairs)
                        {
                            kp.stateChangeSuccess -= this.respKeyChange;
                        }
                        this.m_group.groupAttemptStateChangedSuccess -= this.respondSwitchToggled;
                    }
                    while(kList.Count > 1)//limit to one
                    {
                        kList.RemoveAt(kList.Count-1);
                    }
                    KeyGroup rGroup = new KeyGroup(kdel, kList);
                    if (kList != null && kList.Count > 0)
                    {
                        if (this.switchnode != null && this.switchnode.parents != null && this.switchnode.parents.Count > 1)
                        {
                            foreach (selectionSet setVal in this.switchnode.parents)
                            {
                                foreach(KeyPair kp in rGroup.pairs)
                                {
                                    if (!setVal.containsPairBySwitch(kp))
                                    {
                                        kp.stateChangeSuccess += setVal.respondKeyChanged;
                                    }
                                }
                            }
                        }
                        foreach (KeyPair kp in rGroup.pairs)
                        {
                            kp.stateChangeSuccess += this.respKeyChange;
                        }
                    }
                    else
                    {
                        rGroup = new KeyGroup(kdel);
                    }
                    this.m_group = rGroup;
                    this.m_group.groupAttemptStateChangedSuccess += this.respondSwitchToggled;
                }

                protected virtual void setGroup(KeyGroup kGroup)
                {
                    //if the size is over we will need to return
                    if (kGroup.Count > 1)
                    {
                        return;
                    }

                    if (kGroup == this.m_group)
                    {
                        return;
                    }
                    if (this.m_group != null)
                    {
                        //remove this event listeners
                        if (this.switchnode != null && this.switchnode.parents != null && this.switchnode.parents.Count > 1)
                        {
                            foreach (selectionSet setVal in this.switchnode.parents)
                            {
                                if (setVal.containsNode(this.m_switchnode) && this.m_group != null)
                                {
                                    foreach (KeyPair kp in this.m_group.pairs)
                                    {
                                        if (setVal.containsPairBySwitch(kp))
                                        {
                                            kp.stateChangeSuccess -= setVal.respondKeyChanged;
                                        }
                                    }
                                }
                            }
                        }
                        foreach (KeyPair kp in this.m_group.pairs)
                        {
                            kp.stateChangeSuccess -= this.respKeyChange;
                        }
                        this.m_group.groupAttemptStateChangedSuccess -= this.respondSwitchToggled;
                    }

                    if (this.switchnode != null && this.switchnode.parents != null && this.switchnode.parents.Count > 1)
                    {
                        foreach (selectionSet setVal in this.switchnode.parents)
                        {
                            if (setVal.containsNode(this.m_switchnode) && this.m_group != null)
                            {
                                foreach (KeyPair kp in kGroup.pairs)
                                {
                                    if (!setVal.containsPairBySwitch(kp))
                                    {
                                        kp.stateChangeSuccess += setVal.respondKeyChanged;
                                    }
                                }
                            }
                        }
                    }
                    foreach (KeyPair kp in kGroup.pairs)
                    {
                        kp.stateChangeSuccess += this.respKeyChange;
                    }
                    this.m_group = kGroup;
                    this.m_group.groupAttemptStateChangedSuccess += this.respondSwitchToggled;
                }

            #endregion

            #region private

                

                private void pressSwitch(s_switch widge)
                {
                     widge.RenderRule.Mode = ButtonRenderRule.RenderMode.Pressed;
                }

                private void unpressSwitch(s_switch widge)
                {
                    if (!widge.switchedOn)
                    {
                        widge.RenderRule.Mode = ButtonRenderRule.RenderMode.Default;
                    }
                }

            #endregion

        #endregion
                
    }

}



