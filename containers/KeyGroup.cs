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

    public delegate logicStates KeyStatePairChange(KeyPair nPair, Stack<KeyPair> oldStack);
    public delegate void KeyStatePairSuccess(KeyPair pair);
    public delegate logicStates KeyGroupChange(KeyPair kPair, KeyGroup kGroup);

    public class KeyGroup : ITreeNode<KeyGroup>
    {
        #region members_memberlike_properties

            #region public

            #endregion

            #region internal

            #endregion

            #region protected

                protected KeyLogicNode m_parent;
                protected List<KeyPair> m_list;
                protected KeyDelegator m_delegator;
                protected logicStates m_state;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                public KeyDelegator delegator
                {
                    get
                    {
                        return this.m_delegator;
                    }
                }

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

                public List<KeyPair> pairs
                {
                    get
                    {
                        return this.m_list.ToList();
                    }
                }

                public KeyLogicNode parent
                {
                    get
                    {
                        return this.m_parent;
                    }
                    set
                    {
                        if (this.m_parent != value)
                        {
                            //if (this.m_groupPairChanged != null && this.m_parent != null)
                            //{
                            //    this.m_groupPairChanged -= this.m_parent.;
                            //}
                            //this.m_groupPairChanged += this.m_respGroupPairAttemptChanged;
                            this.m_parent = value;
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

                public event KeyGroupChange groupAttemptChanged;

            #endregion

            #region internal

            #endregion

            #region protected

                //protected event KeyGroupChange groupAttemptChanged;
                protected event KeyStatePairChange m_groupPairChanged;

                protected logicStates m_respGroupPairAttemptChanged(KeyPair newP, Stack<KeyPair> oldStack)
                {
                    //if we attempted a change, we need to evaluate
                    logicStates orig = this.state;
                    this.evaluate();
                    return logicStates.TRUE;
                }

                protected void m_respGroupPairChanged(KeyPair val)
                {//do we need the group to do anything on a successfull pair change?  maybe signal the switch

                }

                protected logicStates m_respGroupAttemptChanged (KeyPair kPair, KeyGroup kGroup)
                {
                    //does this invalidate us?
                    logicStates orig = this.state;
                    this.evaluate();
                    if(this.state == orig)//then no change and we can return true
                    {
                        return logicStates.TRUE;//change is valid
                    }
                    return logicStates.TRUE;//get from keylogic
                }

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
                    this.groupAttemptChanged += this.m_respGroupAttemptChanged;
                }

                public KeyGroup(KeyDelegator delegator, List<Keys> kList) : this(delegator)
                {
                    foreach (Keys kVal in kList)
                    {
                        this.Add(kVal);
                    }
                    this.m_state = this.evaluate();
                }

                public KeyGroup(KeyLogicNode node, List<Keys> kList)
                {
                    this.m_delegator = node.delegator;
                    this.m_list = new List<KeyPair>();
                    List<Keys> inList = new List<Keys>();
                    if(kList != null && kList.Count > 0)
                    {
                        inList = kList.Distinct<Keys>().ToList();
                    }
                    if (this.m_delegator != null)
                    {
                        for (int i = 0; i < inList.Count; i++)
                        {
                            this.m_list.Add(this.m_delegator.key(inList[i]));
                        }
                    }
                    else
                    {
                        this.m_delegator = new KeyDelegator();
                    }
                    this.m_parent = node;
                    node.Data = this;//check for validity here
                    //set parent to null if failure in logicnode

                    this.groupAttemptChanged += this.m_respGroupAttemptChanged;
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
                    this.groupAttemptChanged += this.m_respGroupAttemptChanged;
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
                        logicStates result = logicStates.TRUE;
                        if (this.groupAttemptChanged != null)
                        {
                            result = groupAttemptChanged(adder, this);//will tell if it is invalidated
                        }
                        if (result != logicStates.TRUE)//if changed to false we are invalidated
                        {//remove it
                            this.m_list.Remove(adder);
                            this.evaluate();//this should fix it
                        }
                        else
                        {
                            //handle adding event listeners
                            adder.stateChangeAttempt += this.m_respGroupPairAttemptChanged;
                            adder.stateChangeSuccess += this.m_respGroupPairChanged;
                            return true;
                        }
                        
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
                        //now is it invalidated?
                        logicStates result = logicStates.TRUE;
                        if(this.groupAttemptChanged != null)
                        {
                            result = this.groupAttemptChanged(remover, this);
                        }
                        if (result != logicStates.TRUE)
                        {
                            this.m_list.Add(remover);
                            this.evaluate();//this should fix it
                        }
                        else
                        {//handle removing event listeners
                            remover.stateChangeAttempt -= this.m_respGroupPairAttemptChanged;
                            remover.stateChangeSuccess -= this.m_respGroupPairChanged;
                            return true;
                        }
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
                    if (dVal == null || dVal.Count == 0)
                    {//do them all
                        foreach (KeyPair kPair in this.m_list)
                        {
                            kPair.setState(KeyState.Up, null);
                        }
                    }
                    else
                    {
                        foreach (KeyPair kPair in this.m_list)
                        {
                            if (dVal.Contains(kPair.key))
                            {
                                kPair.setState(KeyState.Up, null);
                            }
                        }
                    }
                    //this.evaluate();//done on change success
                }

                public void activate(List<Keys> aVal)
                {
                    if (aVal == null || aVal.Count == 0)
                    {//do them all
                        foreach (KeyPair kPair in this.m_list)
                        {
                            kPair.setState(KeyState.Down, null);
                        }
                    }
                    else
                    {
                        foreach (KeyPair kPair in this.m_list)
                        {
                            if (aVal.Contains(kPair.key))
                            {
                                kPair.setState(KeyState.Down, null);
                            }
                        }
                    }
                    //this.evaluate();//done on change success
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
                    }//set handled in public methods for recursion
                    
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
                public event KeyStatePairSuccess stateChangeSuccess;//when this state changes successfully based on its own command
                //public event KeyStatePairChange externalStateChange;//when this state is forced to change based on logical operations

                public void respondExternalStateChange(KeyPair nPair, KeyPair oPair)
                {
                    if (nPair == this)//well then the item that is trying to change our status is us and it won't work, because will change original status
                    {
                        return;
                    }
                }

                public logicStates setState(KeyState sVal, Stack<KeyPair> oldPairs)
                {
                    if(sVal == this.state)
                    {
                        return logicStates.TRUE;//well then we succeeded in setting
                    }
                    if(oldPairs == null)
                    {
                        oldPairs = new Stack<KeyPair>();
                    }
                    if (oldPairs.Select(x => x.key).ToList().Contains(this.key))//if this is a loop we can't change it from what we intend to change it anyway without reversing our intentions
                    {
                        return logicStates.FALSE;
                    }
                    KeyPair oldPair = new KeyPair(this.key, this.state);//get your old pair
                    this.m_state = sVal;
                    if (this.stateChangeAttempt != null)
                    {
                        oldPairs.Push(oldPair);
                        this.stateChangeAttempt(this, oldPairs);
                    }
                    if (this.m_state == sVal)//success
                    {
                        if (this.stateChangeSuccess != null)
                        {
                            this.stateChangeSuccess(this);//signal the game
                        }
                        if (this.stateChangeAttempt != null)
                        {
                            //pop off the stack if we popped it on
                            oldPairs.Pop();
                        }
                        return logicStates.TRUE;
                    }
                    //pop off the stack if we popped it on
                    if (this.stateChangeAttempt != null)
                    {
                        oldPairs.Pop();
                    }
                    return logicStates.FALSE;
                }

                /*
                //else//if we have nothing to check for
                //{
                //}
                /*
                if (  )
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
                */
                //otherwise don't change

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
                public KeyPair(Keys kVal, KeyState kState)
                {
                    this.m_key = kVal;
                    this.m_state = kState;
                }

                public void setExternal()
                {
                    return;
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
