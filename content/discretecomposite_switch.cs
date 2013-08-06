﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using monoswitch.containers;
using monoswitch.content;
using Microsoft.Xna.Framework.Input;

namespace monoswitch.content
{
    public class discretecomposite_switch: discrete_switch
    {

        #region members

            #region public

            #endregion

            #region internal

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
                public discretecomposite_switch(KeyDelegator kDel, float interval)
                    : base(kDel, interval)
                {
                }

                public discretecomposite_switch(KeyDelegator kdel, List<Keys> kList, float interval) : base (kdel, kList, interval)
                {
                }

                public discretecomposite_switch(KeyGroup kgroup, float interval) : base(kgroup, interval)
                {
                }

                public discretecomposite_switch(KeyDelegator kdel, int scanR, float interval) : base(kdel, scanR, interval)
                {
                }

                public discretecomposite_switch(KeyDelegator kdel, TimeSpan scanR, float interval) : base(kdel, scanR, interval)
                {
                }

                public discretecomposite_switch(KeyDelegator kdel, List<Keys> kList, int scanR, float interval) : base(kdel, kList, scanR, interval)
                {
                }

                public discretecomposite_switch(KeyDelegator kdel, List<Keys> kList, TimeSpan scanR, float interval)
                    : base(kdel, kList, scanR, interval)
                {
                }

                public discretecomposite_switch(KeyGroup kg, int scanR, float interval) : base(kg, scanR, interval)
                {
                }

                public discretecomposite_switch(KeyGroup kg, TimeSpan scanR, float interval) : base(kg, scanR, interval)
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
                public discretecomposite_switch(KeyDelegator kdel, int x, int y, string label, float interval, int padding = 2)
                    : base(kdel, new List<Keys>(), x, y, label, interval, padding)
                {
                }

                public discretecomposite_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, string label, float interval, int padding = 2): base(kdel, kList, x, y, label, interval, padding)
                {
                }

                public discretecomposite_switch(KeyGroup kgroup, int x, int y, string label, float interval, int padding = 2) : base(kgroup, x, y, label, interval, padding)
                {
                }

                public discretecomposite_switch(KeyDelegator kdel, int x, int y, string label, TimeSpan scanR, float interval, int padding = 2) : base(kdel, x, y, label, scanR, interval, padding)
                {
                }

                //int not allowed as an input due to padding
                public discretecomposite_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, string label, TimeSpan scanR, float interval, int padding = 2) : base(kdel, kList, x, y, label, scanR, interval, padding)
                {
                }

                public discretecomposite_switch(KeyGroup kgroup, int x, int y, string label, TimeSpan scanR, float interval, int padding = 2) : base(kgroup, x, y, label, scanR, interval, padding)
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
                public discretecomposite_switch(KeyDelegator kdel, int x, int y, int width, string label, float interval)
                    : base(kdel, new List<Keys>(), x, y, width, label, interval)
                {
                }

                public discretecomposite_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, int width, string label, float interval)
                    : base(kdel, kList, x, y, width, label, interval)
                {
                }

                public discretecomposite_switch(KeyGroup kgroup, int x, int y, int width, string label, float interval)
                    : base(kgroup, x, y, width, label, interval)
                {
                }

                public discretecomposite_switch(KeyDelegator kdel, int x, int y, int width, string label, TimeSpan scanR, float interval)
                    : base(kdel, x, y, width, label, scanR, interval)
                {
                }

                public discretecomposite_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, int width, string label, TimeSpan scanR, float interval)
                    : base(kdel, kList, x, y, width, label, scanR, interval)
                {
                }

                public discretecomposite_switch(KeyGroup kgroup, int x, int y, int width, string label, TimeSpan scanR, float interval)
                    : base(kgroup, x, y, width, label, scanR, interval)
                {
                }

                public discretecomposite_switch(KeyDelegator kdel, int x, int y, int width, string label, int scanR, float interval)
                    : base(kdel, x, y, width, label, scanR, interval)
                {
                }

                public discretecomposite_switch(KeyDelegator kdel, List<Keys> kList, int x, int y, int width, string label, int scanR, float interval)
                    : base(kdel, kList, x, y, width, label, scanR, interval)
                {
                }

                public discretecomposite_switch(KeyGroup kgroup, int x, int y, int width, string label, int scanR, float interval)
                    : base(kgroup, x, y, width, label, scanR, interval)
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
                    }
                    else
                    {
                        rGroup = new KeyGroup(kdel);
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
