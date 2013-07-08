using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ruminate.GUI.Framework;
using Ruminate.GUI.Content;
using Microsoft.Xna.Framework.Graphics;

//authors note: 
//as toggle button was a sealed class in the ruminate framework, 
//this class reproduces code from toggle button as opposed to extending it


namespace monoswitch.content
{

    public delegate void switchEvent(s_switch sender);
    public delegate void monoSwitchKeyEvent(List<Keys> kList, List<KeyState> sList);

    public class s_switch: Ruminate.GUI.Framework.WidgetBase<Ruminate.GUI.Content.ButtonRenderRule>//standard switch.
    {//reproduce functionality of toggle button
        #region members

            #region public

                //static const

                //enums


            #endregion

            #region protected

                protected TimeSpan m_scanningRate;
                protected List<Keys> m_controllers;
                protected Boolean m_commited;
                //based off toggle button
                protected bool m_switchedOn;
                protected int m_textPadding;
                protected int m_width;

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
                    //set
                    //{
                    //
                    //}
                }

                public Keys controller
                {
                    get
                    {
                        if(this.m_controllers.Count == 0)
                        {
                            return Keys.F9;
                        }
                        return this.m_controllers[0];
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


                /*
                protected internal override void EnterPressed()
                {
                }
                protected internal override void ExitPressed()
                {
                }
                */

                protected internal new virtual void EnterHover()
                {
                    this.highlight();
                }

                protected internal new virtual void ExitHover()
                {
                    this.deHighlight();
                }

                protected internal new virtual void KeyUp(KeyEventArgs e)
                {

                }

                protected internal new virtual void KeyDown(KeyEventArgs e)
                {

                }
                

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region methods

            #region public
            //CONSTRUCTOR
            public s_switch()
            {
                this.Area = new Rectangle(0, 0, 0, 0);
                this.label = label;
                this.textPadding = 0;
                //set events
                this.OnToggle += this.pressSwitch;
                this.OffToggle += this.unpressSwitch;
                //set monoswitchstuff
                this.m_switchedOn = false;
                this.m_scanningRate = new TimeSpan(0, 0, 0, 0, 0);
                this.m_controllers = new List<Keys>();
                this.m_commited = false;
            }

            public s_switch(Keys controllerValue) : this()
            {
                this.m_controllers.Add(controllerValue);
            }

            public s_switch(int scanR) : this()
            {
                this.m_scanningRate = new TimeSpan(0, 0, 0, 0, Math.Abs(scanR));
            }

            public s_switch(TimeSpan scanR) : this()
            {
                this.m_scanningRate = new TimeSpan(0, 0, 0, 0, (int)scanR.TotalMilliseconds);
            }

            public s_switch(Keys controllerValue, int scanR) : this()
            {
                this.m_scanningRate = new TimeSpan(0, 0, 0, 0, Math.Abs(scanR));
                this.m_controllers.Add(controllerValue);
            }

            /// <summary>
            /// Creates a new button at the location specified. The button defaults to
            /// the height of the RenderRule and width of the label.
            /// </summary>
            /// <param name="x">The X coordinate of the widget.</param>
            /// <param name="y">The Y coordinate of the widget.</param>
            /// <param name="label">The label to be rendered on the button.</param>
            /// <param name="padding">If specified the padding on either side of the label.</param>
            public s_switch(int x, int y, string label, int padding = 2)
            {
                this.Area = new Rectangle(x, y, 0, 0);
                this.label = label;
                this.textPadding = padding;
                //set events
                this.OnToggle += this.pressSwitch;
                this.OffToggle += this.unpressSwitch;
                //set switch important variables
                this.m_switchedOn = false;
                this.m_scanningRate = new TimeSpan(0, 0, 0, 0, 0);
                this.m_controllers = new List<Keys>();
                this.m_commited = false;
            }

            public s_switch(int x, int y, string label, Keys controllerValue, int padding = 2)
                : this(x, y, label, padding)
            {
                this.m_controllers.Add(controllerValue);
            }

            public s_switch(int x, int y, string label, TimeSpan scanR, int padding = 2)
                : this(x, y, label, padding)
            {
                this.m_scanningRate = new TimeSpan(0, 0, 0, 0, (int)scanR.TotalMilliseconds);
            }

            //int not allowed as an input due to padding
            public s_switch(int x, int y, string label, Keys controllerValue, TimeSpan scanR, int padding = 2)
                : this(x, y, label, padding)
            {
                this.m_controllers.Add(controllerValue);
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
            public s_switch(int x, int y, int width, string label)
            {
                this.Area = new Rectangle(x, y, 0, 0);
                this.width = width;
                this.label = label;

                //set events
                this.OnToggle += this.pressSwitch;
                this.OffToggle += this.unpressSwitch;

                this.m_switchedOn = false;
                this.m_scanningRate = new TimeSpan(0, 0, 0, 0, 0);
                this.m_controllers = new List<Keys>();
                this.m_commited = false;
            }

            public s_switch(int x, int y, int width, string label, Keys controllerValue)
                : this(x, y, width, label)
            {
                this.m_controllers.Add(controllerValue);
            }

            public s_switch(int x, int y, int width, string label, TimeSpan scanR)
                :this(x, y, width, label)
            {
                this.m_scanningRate = new TimeSpan(0, 0, 0, 0, (int)scanR.TotalMilliseconds);
            }

            public s_switch(int x, int y, int width, string label, int scanR)
                :this(x, y, width, label)
            {
                this.m_scanningRate = new TimeSpan(0, 0, 0, 0, Math.Abs(scanR));
            }

            public s_switch(int x, int y, int width, string label, Keys controllerValue, TimeSpan scanR)
                :this(x, y, width, label)
            {
                this.m_controllers.Add(controllerValue);
                this.m_scanningRate = new TimeSpan(0, 0, 0, 0, (int)scanR.TotalMilliseconds);
            }

            public s_switch(int x, int y, int width, string label, Keys controllerValue, int scanR)
                : this(x, y, width, label)
            {
                this.m_controllers.Add(controllerValue);
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
                if (this.m_controllers.Count < 1)
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
            public void toggleSwitch()
            {
                m_switchedOn = !m_switchedOn;//set it first as our events will depend on this state
                if (m_switchedOn)
                {
                    if (OnToggle != null)
                    {
                        OnToggle(this);
                    }
                }
                else
                {
                    if (OffToggle != null)
                    {
                        OffToggle(this);
                    }
                }
            }

            public void highlight()
            {
                if (!this.switchedOn)
                {
                    Console.WriteLine("entering hover");
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

            #region protected

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



    

    /*####################################################################*/
    /*                        Node Initialization                         */
    /*####################################################################*/



