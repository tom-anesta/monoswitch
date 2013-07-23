﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using monoswitch.containers;
using monoswitch;
using monoswitch.content;
using Microsoft.Xna.Framework.Input;
using Ruminate.DataStructures;

namespace monoswitch.containers
{

    public delegate void KeyStatePairChange(KeyPair kPair, ref bool kVal);

    public class KeyGroup : ITreeNode<KeyGroup>
    {

        #region members_memberlike_properties

            #region public

            #endregion

            #region internal

            #endregion

            #region protected

                protected TreeNode<KeyGroup> m_parent;
                protected List<KeyPair> m_list;
                protected KeyDelegator m_delegator;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                public bool isTrue
                {
                    get
                    {
                        bool rVal = true;
                        foreach (KeyPair kPair in this.m_list)
                        {
                            if (!(kPair.state == KeyState.Down))
                            {
                                rVal = false;
                                break;
                            }
                        }
                        return rVal;
                    }
                }

                public bool isFalse
                {
                    get
                    {
                        bool rVal = true;
                        foreach (KeyPair kPair in this.m_list)
                        {
                            if (kPair.state == KeyState.Down)
                            {
                                rVal = false;
                                break;
                            }
                        }
                        return rVal;
                    }
                }

                public bool isIndeterminate
                {
                    get
                    {
                        return (this.isTrue && this.isFalse);
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

                //constructor
                public KeyGroup(KeyDelegator delegator)
                {
                    this.m_parent = null;
                    this.m_list = new List<KeyPair>();
                    if (delegator != null)
                    {
                        this.m_delegator = delegator;
                    }
                    else
                    {
                        this.m_delegator = new KeyDelegator();
                    }
                }

                public KeyGroup(KeyDelegator delegator, List<Keys> kList) : this(delegator)
                {
                    foreach (Keys kVal in kList)
                    {
                        this.add(kVal);
                    }
                }

                //iTreeNode
                public TreeNode<KeyGroup> GetTreeNode()
                {
                    return this.m_parent;
                }

                //structure methods
                public bool add(Keys kVal)
                {
                    if (!(this.m_list.Select(x => x.key).Contains(kVal)) )
                    {
                        KeyPair adder = this.m_delegator.key(kVal);
                        this.m_list.Add(adder);
                        //now is it invalidated?
                        //if invalidated remove it and return false
                        //handle adding event listeners
               
                        return true;
                    }
                    return false;
                }
                public List<bool> add(List<Keys> kList)
                {
                    List<bool> rList = new List<bool>();
                    foreach (Keys kVal in kList)
                    {
                        if (!(this.m_list.Select(x => x.key).Contains(kVal)))
                        {
                            KeyPair adder = this.m_delegator.key(kVal);
                            this.m_list.Add(adder);
                            //now is it invalidated?
                            //if invalidated remove it and return false
                            //handle adding event listeners
                    
                            rList.Add(true);
                        }
                        else
                        {
                            rList.Add(false);
                        }
                    }
                    return rList;
                }
                public bool remove(Keys kVal)
                {
                    KeyPair remover = this.m_list.FirstOrDefault(x => (x.key == kVal));
                    if (remover != null)
                    {
                        this.m_list.Remove(remover);//first remove
                        //now is it invalidated?
                        //if invalidated add it again and return false
                        //handle removing event listeners

                    }
                    return false;
                }
                public List<bool> remove(List<Keys> kList)
                {
                    List<bool> rList = new List<bool>();
                    foreach (Keys kVal in kList)
                    {
                        KeyPair remover = this.m_list.FirstOrDefault(x => (x.key == kVal));
                        if (remover != null)
                        {
                            this.m_list.Remove(remover);
                            //now is it invalidated?
                            //if invalideated add it again and return false
                            //handle removing event listeners
                    
                            rList.Add(true);
                        }
                        else
                        {
                            rList.Add(false);
                        }
                    }
                    return rList;
                }

                //control methods
                public void deactivate(List<Keys> dVal)
                {
                    if (dVal.Count == 0)
                    {
                        foreach (KeyPair kPair in this.m_list)
                        {
                            kPair.state = KeyState.Up;
                        }
                    }
                    else
                    {
                        foreach (KeyPair kPair in this.m_list)
                        {
                            if (dVal.Contains(kPair.key))
                            {
                                kPair.state = KeyState.Up;
                            }
                        }
                    }
                }

                public void activate(List<Keys> aVal)
                {
                    if (aVal.Count == 0)
                    {
                        foreach (KeyPair kPair in this.m_list)
                        {
                            kPair.state = KeyState.Down;
                        }
                    }
                    else
                    {
                        foreach (KeyPair kPair in this.m_list)
                        {
                            if (aVal.Contains(kPair.key))
                            {
                                kPair.state = KeyState.Down;
                            }
                        }
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

    }


    public class KeyPair
    {
        #region members_memberlike_properties

            #region public

            #endregion

            #region internal

            #endregion

            #region protected

                protected Keys m_key;
                protected KeyState m_state;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                public Keys key
                {
                    get
                    {
                        return this.m_key;
                    }
                }

                public KeyState state
                {
                    get
                    {
                        return this.m_state;
                    }
                    set
                    {
                        if (this.m_state != value)
                        {
                            bool rVal = true; ;
                            if (value == KeyState.Down)
                            {
                                rVal = true;
                            }
                            else if (value == KeyState.Up)
                            {
                                rVal = false;
                            }
                            else
                            {
                                rVal = false;
                            }

                            if (this.stateChangeAttempt != null)
                            {
                                this.stateChangeAttempt(this, ref rVal);
                            }
                            else//if we have nothing to check for
                            {
                                this.m_state = value;
                                if (this.stateChangeSuccess != null)
                                {
                                    bool outVal = true;
                                    if(this.m_state == KeyState.Down)
                                    {
                                        outVal = true;
                                    }
                                    else if(this.m_state == KeyState.Up)
                                    {
                                        outVal = false;
                                    }
                                    this.stateChangeSuccess(this, ref outVal);
                                }
                            }
                            if ( (value == KeyState.Down && rVal == true) || (value == KeyState.Up && rVal == false))
                            {
                                this.m_state = value;
                                if (this.stateChangeSuccess != null)
                                {
                                    bool outVal = true;
                                    if (this.m_state == KeyState.Down)
                                    {
                                        outVal = true;
                                    }
                                    else if (this.m_state == KeyState.Up)
                                    {
                                        outVal = false;
                                    }
                                    this.stateChangeSuccess(this, ref outVal);
                                }
                            }
                            //otherwise don't change
                        }
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

                public event KeyStatePairChange stateChangeAttempt;//when we check to see if this state can change
                public event KeyStatePairChange stateChangeSuccess;//when this state changes successfully based on its own command
                public event KeyStatePairChange externalStateChange;//when this state is forced to change based on logical operations

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

                //constructor
                public KeyPair(Keys kVal)
                {
                    this.m_key = kVal;
                    this.m_state = KeyState.Up;
                }

            #endregion

            #region internal

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion


    }



}
