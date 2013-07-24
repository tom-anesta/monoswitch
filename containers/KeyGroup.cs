using System;
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
    public delegate void KeyGroupChange(KeyPair kPair, KeyGroup kGroup, ref bool kVal);

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
                protected logicStates m_state;

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
                        return this.m_state == logicStates.TRUE;
                    }
                }

                public bool isFalse
                {
                    get
                    {
                        return this.m_state == logicStates.FALSE;
                    }
                }

                public bool isIndeterminate
                {
                    get
                    {
                        return this.m_state == logicStates.INDETERMINATE;
                    }
                }

                public logicStates state
                {
                    get
                    {
                        return this.m_state;
                    }
                }

                public int Count
                {
                    get
                    {
                        return this.m_list.Count;
                    }
                }

                public List<Keys> actives
                {
                    get
                    {
                        return ((List<Keys>)(this.m_list.Where(x => x.state == KeyState.Down)).Select(x => x.key).ToList<Keys>());//get the keys that are down
                    }
                }

                public List<Keys> unactives
                {
                    get
                    {
                        return ((List<Keys>)(this.m_list.Where(x => x.state == KeyState.Up)).Select(x => x.key).ToList<Keys>());//get the keys that are up
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

                public KeyGroupChange groupChanged
                {
                    get
                    {
                        return this.m_groupAttemptChanged;
                    }
                    protected set
                    {
                        this.m_groupAttemptChanged = value;
                    }
                }

            #endregion

            #region internal

            #endregion

            #region protected

                protected event KeyGroupChange m_groupAttemptChanged;

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
                    this.m_state = logicStates.INDETERMINATE;
                }

                public KeyGroup(KeyDelegator delegator, List<Keys> kList) : this(delegator)
                {
                    foreach (Keys kVal in kList)
                    {
                        this.Add(kVal);
                    }
                }

                public KeyGroup(KeyGroup keyGroup)
                {
                    this.m_delegator = keyGroup.m_delegator;
                    this.m_list = new List<KeyPair>();
                    foreach (KeyPair kp in keyGroup.m_list)
                    {
                        this.m_list.Add(kp);
                    }
                    this.m_parent = keyGroup.m_parent;
                }

                //iTreeNode
                public TreeNode<KeyGroup> GetTreeNode()
                {
                    return this.m_parent;
                }

                //structure methods
                public bool Add(Keys kVal)
                {
                    if (!(this.m_list.Select(x => x.key).Contains(kVal)) )
                    {
                        KeyPair adder = this.m_delegator.key(kVal);
                        this.m_list.Add(adder);
                        //handle adding event listeners

                        //now is it invalidated?
                        logicStates tempState = this.m_state;
                        this.evaluate();
                        if (this.m_state != tempState)
                        {
                            bool addingattempt = true;
                            if (this.m_groupAttemptChanged != null)
                            {
                                this.m_groupAttemptChanged(adder, this, ref addingattempt);
                            }
                            if (!addingattempt)//if changed to false we are invalidated
                            {//remove it and the event listeners
                                this.m_list.Remove(adder);
                                //event listeners
                                this.evaluate();//this should fix it
                            }
                        }//if invalidated remove it and return false
                        return true;
                    }
                    return false;
                }
                public List<bool> Add(List<Keys> kList)
                {
                    List<bool> rList = new List<bool>();
                    foreach (Keys kVal in kList)
                    {
                        rList.Add(Add(kVal));
                    }
                    return rList;
                }
                public bool Remove(Keys kVal)
                {
                    KeyPair remover = this.m_list.FirstOrDefault(x => (x.key == kVal));
                    if (remover != null)
                    {
                        this.m_list.Remove(remover);//first remove
                        //handle removing event listeners

                        //now is it invalidated?
                        logicStates tempState = this.m_state;
                        this.evaluate();
                        if (this.m_state != tempState)
                        {
                            bool removingattempt = false;
                            if (this.m_groupAttemptChanged != null)
                            {
                                this.m_groupAttemptChanged(remover, this, ref removingattempt);
                            }
                            if (removingattempt)//if changed to false we are invalidated
                            {//add it and the event listeners
                                this.m_list.Add(remover);
                                //event listeners
                                this.evaluate();//this should fix it
                            }
                        }
                        //if invalidated add it again and return false
                        

                    }
                    return false;
                }
                public List<bool> Remove(List<Keys> kList)
                {
                    List<bool> rList = new List<bool>();
                    foreach (Keys kVal in kList)
                    {
                        rList.Add(Remove(kVal));
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

                public logicStates evaluate()//double check the state
                {
                    bool trueFound = false;
                    bool falseFound = false;
                    foreach (KeyPair kPair in this.m_list)
                    {
                        if (kPair.state == KeyState.Down)
                        {
                            trueFound = true;
                            if (trueFound && falseFound)
                            {
                                break;
                            }
                        }
                        else if (kPair.state == KeyState.Up)
                        {
                            falseFound = true;
                            if (trueFound && falseFound)
                            {
                                break;
                            }
                        }
                    }
                    if (trueFound && falseFound)
                    {
                        this.m_state = logicStates.INDETERMINATE;
                    }
                    else if (trueFound)
                    {
                        this.m_state = logicStates.TRUE;
                    }
                    else if (falseFound)
                    {
                        this.m_state = logicStates.FALSE;
                    }
                    else
                    {
                        this.m_state = logicStates.INDETERMINATE;
                    }
                    return this.m_state;
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
