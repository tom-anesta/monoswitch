using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ruminate.GUI.Framework;
using Ruminate.GUI.Content;

//authors note: 
//as toggle button was a sealed class in the ruminate framework, 
//this class reproduces code from toggle button as opposed to extending it


namespace monoswitch
{
    public class s_switch: Ruminate.GUI.Framework.WidgetBase<Ruminate.GUI.Content.ButtonRenderRule>//standard switch.
    {//reproduce functionality of toggle button
        #region members

            #region public

                //static const

                public const uint DEFAULT_SCANNING_RATE = 666;

                //enums

                //delegates

            #endregion

            #region protected

                protected uint m_scanningRate;
                //private uint m_discreteTime;//the amount of time that you should hold a discrete timer
                protected List<char> m_controllers;
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
                    get {
                        return this.m_width;
                    }
                    set
                    {
                        this.m_width = value;
                        this.m_textPadding = 0;
                    }
                }

                public uint scanningRate
                {
                    get
                    {
                        return this.m_scanningRate;
                    }
                    set
                    {

                    }
                }

                public char controller
                {
                    get
                    {
                        if(this.m_controllers.Count == 0)
                        {
                            return new char();
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
                    set
                    {
                        return;//read only
                    }
                }

                /// <summary>
                /// Event fired when the button is pressed.
                /// </summary>
                public WidgetEvent OnToggle
                {
                    get;
                    set;
                }

                /// <summary>
                /// Event fired when the button is released.
                /// </summary>
                public WidgetEvent OffToggle
                {
                    get;
                    set;
                }
        
                /// <summary>
                /// Returns true when the button is pressed and false when
                /// released. Changing the value fires the OnToggle/OffToggle
                /// events.
                /// </summary>
                public bool switchedOn
                {
                    get
                    {
                        return m_switchedOn;
                    }
                    set
                    {
                        m_switchedOn = value;//set it first as our events will depend on this state
                        if (value != m_switchedOn)
                        {
                            if (value)
                            {
                                if (OnToggle != null)
                                {
                                    OnToggle(this);
                                    RenderRule.Mode = ButtonRenderRule.RenderMode.Pressed;
                                }
                            }
                            else
                            {
                                if (OffToggle != null)
                                {
                                    OffToggle(this);
                                    RenderRule.Mode = ButtonRenderRule.RenderMode.Default;
                                }
                            }
                        }
                    }
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

                this.m_switchedOn = false;
                this.m_scanningRate = 0;
                this.m_controllers = new List<char>();
                this.m_commited = false;
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
                this.OnToggle += this.unpressSwitch;
                this.OffToggle += this.unpressSwitch;

                this.m_switchedOn = false;
                this.m_scanningRate = 0;
                this.m_controllers = new List<char>();
                this.m_commited = false;
            }
            

            //INITIALIZATION FUNCTIONS

            protected override ButtonRenderRule BuildRenderRule()
            {
                return new ButtonRenderRule();
            }

            protected override void Attach()
            {
                var minWidth = (int)this.RenderRule.Font.MeasureString(this.label).X + (2 * this.RenderRule.Edge);
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
            {
            }
            

            //OTHER PUBLIC FUNCTIONS
            //switch the device

            public void toggleSwitch()
            {
                this.m_switchedOn = !(this.m_switchedOn);//reverse the condition
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

            #endregion

            #region protected

            #endregion

            #region private

            private void pressSwitch(Widget widge)
            {
                if (widge is s_switch)
                {
                    (widge as s_switch).RenderRule.Mode = ButtonRenderRule.RenderMode.Pressed;
                }
            }

            private void unpressSwitch(Widget widge)
            {
                if (widge is s_switch)
                {
                    if (!(widge as s_switch).switchedOn)
                    {
                        (widge as s_switch).RenderRule.Mode = ButtonRenderRule.RenderMode.Default;
                    }
                }
            }

            #endregion

        #endregion

        #region events

            #region public

                protected override void MouseClick(MouseEventArgs e)
                {
                    /*
                    IsToggled = !IsToggled;
                    if (!IsToggled)
                    {
                        RenderRule.Mode = IsHover ? ButtonRenderRule.RenderMode.Hover : ButtonRenderRule.RenderMode.Default;
                    }
                    else
                    {
                        RenderRule.Mode = ButtonRenderRule.RenderMode.Pressed;
                    }
                    */
                    //we do not want this object to respond to the mouse
                }

                protected override void EnterPressed()
                {
                    //we do not want this object to respond to the mouse
                }

                protected override void ExitPressed()
                {
                    //we do not want this object to respond to the mouse
                }

                protected override void EnterHover()
                {
                    //we do not want this object to respond to the mouse
                }

                protected override void ExitHover()
                {
                    //we do not want this object to respond to the mouse
                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

    }

}



    

    /*####################################################################*/
    /*                        Node Initialization                         */
    /*####################################################################*/



