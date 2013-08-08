using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using monoswitch;
using monoswitch.containers;
using monoswitch.content;
using Microsoft.Xna.Framework.Input;
using monoswitch.singletons;

namespace monoswitch.content
{
    public class composite_switch: s_switch
    {

        #region members

            #region public

            #endregion

            #region intenal

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

        #region public

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

            #endregion

            #region internal

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region methods

        #region public

        //CONSTRUCTOR

                //constructors
                public composite_switch(KeyDelegator kdel) : base(kdel, new List<Keys>())
                {
                }

                public composite_switch(KeyDelegator kdel, List<Keys> kList) : base(kdel, kList)
                {
                }

                public composite_switch(KeyGroup kgroup) : base(kgroup)
                {
                }

                public composite_switch(KeyDelegator kdel, int scanR) : base(kdel, scanR)
                {
                }

                public composite_switch(KeyDelegator kdel, TimeSpan scanR) : base(kdel, scanR)
                {
                }

                public composite_switch(KeyDelegator kdel, List<Keys> kList, int scanR) : base(kdel, kList, scanR)
                {
                }

                public composite_switch(KeyDelegator kdel, List<Keys> kList, TimeSpan scanR) : base(kdel, kList, scanR)
                {
                }

                public composite_switch(KeyGroup kg, int scanR) : base(kg, scanR)
                {
                }

                public composite_switch(KeyGroup kg, TimeSpan scanR) : base(kg, scanR)
                {
                }

                /// <summary>
                /// Creates a new button at the location specified. The button defaults to
                /// the height of the RenderRule and width of the label.
                /// </summary>
                /// <param name="x">The X coordinate of the widget.</param>
                /// <param name="y">The Y coordinate of the widget.</param>
                /// <param name="label">The label to be rendered on the button.</param>
                /// <param name="padding">If specified the padding on either side of the label.</param>
                public composite_switch(KeyDelegator kdel, int x, int y, string label, int padding = 2)
                    : base(kdel, new List<Keys>(), x, y, label, padding)
                {
                }

                public composite_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, string label, int padding = 2) : base(kdel, kList, x, y, label, padding)
                {
                }

                public composite_switch(KeyGroup kgroup, int x, int y, string label, int padding = 2) : base(kgroup, x, y, label, padding)
                {
                }

                public composite_switch(KeyDelegator kdel, int x, int y, string label, TimeSpan scanR, int padding = 2) : base(kdel, x, y, label, scanR, padding)
                {
                }

                //int not allowed as an input due to padding
                public composite_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, string label, TimeSpan scanR, int padding = 2) : base(kdel, kList, x, y, label, scanR, padding)
                {
                }

                public composite_switch(KeyGroup kgroup, int x, int y, string label, TimeSpan scanR, int padding = 2) : base(kgroup, x, y, label, scanR, padding)
                {
                }

                /// <summary>
                /// Creates a new button at the location specified. The button defaults to
                /// the height of the RenderRule and width of the label.
                /// </summary>
                /// <param name="x">The X coordinate of the widget.</param>
                /// <param name="y">The Y coordinate of the widget.</param>
                /// <param name="width">The width of the Button. Ignored if the width is less that the width of the label.</param>
                /// <param name="label">The label to be rendered on the button.</param>
                public composite_switch(KeyDelegator kdel, int x, int y, int width, string label)
                    : base(kdel, new List<Keys>(), x, y, width, label)
                {
                }

                public composite_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, int width, string label): base(kdel, kList, x, y, width, label)
                {
                }

                public composite_switch(KeyGroup kgroup, int x, int y, int width, string label) : base(kgroup, x, y, width, label)
                {
                }

                public composite_switch(KeyDelegator kdel, int x, int y, int width, string label, TimeSpan scanR) : base(kdel, x, y, width, label, scanR)
                {
                }

                public composite_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, int width, string label, TimeSpan scanR) : base(kdel, kList, x, y, width, label, scanR)
                {
                }

                public composite_switch(KeyGroup kgroup, int x, int y, int width, string label, TimeSpan scanR) : base(kgroup, x, y, width, label, scanR)
                {
                }

                public composite_switch(KeyDelegator kdel, int x, int y, int width, string label, int scanR) : base(kdel, x, y, width, label, scanR)
                {
                }

                public composite_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, int width, string label, int scanR) : base(kdel, kList, x, y, width, label, scanR)
                {
                }

                public composite_switch(KeyGroup kgroup, int x, int y, int width, string label, int scanR) : base(kgroup, x, y, width, label, scanR)
                {
                }

                //INITIALIZATION FUNCTIONS

                //UPDATE         

                //OTHER PUBLIC FUNCTIONS

            #endregion

            #region internal

            #endregion

            #region protected

                protected override void setGroup(KeyDelegator kdel, List<Keys> kList)
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
                    KeyGroup rGroup = new KeyGroup(kdel, kList);
                    if (kList != null && kList.Count > 0)
                    {
                        if (this.switchnode != null && this.switchnode.parents != null && this.switchnode.parents.Count > 1)
                        {
                            foreach (selectionSet setVal in this.switchnode.parents)
                            {
                                foreach (KeyPair kp in rGroup.pairs)
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
                        this.m_group.groupAttemptStateChangedSuccess += this.respondSwitchToggled;
                    }
                    else
                    {
                        rGroup = new KeyGroup(kdel);
                        this.m_group.groupAttemptStateChangedSuccess += this.respondSwitchToggled;
                    }
                    this.m_group = rGroup;
                    
                }

                protected override void setGroup(KeyGroup kGroup)
                {
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
                }

            #endregion

            #region private

            #endregion

        #endregion

        

    }
}
