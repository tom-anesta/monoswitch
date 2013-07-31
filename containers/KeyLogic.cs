using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ruminate;
using Ruminate.DataStructures;
using Microsoft.Xna.Framework.Input;
using monoswitch;

namespace monoswitch.containers
{

    public delegate void KLNodeOperation (KeyLogicNode node);
    public delegate logicStates NodeStateOperation(KeyLogicNode node);
    public delegate List<logicStates> NodeStateListOperation(KeyLogicNode node);
    //public delegate bool NodeBoolOperation(KeyLogicNode node, bool intended);

    public class KeyLogicRoot : KeyLogicNode
    {

        #region members_memberlike_properties

            #region public

            #endregion

            #region internal

            #endregion

            #region protected

                protected selectionSet m_selectSet;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                public selectionSet selectSet
                {
                    get
                    {
                        if (this.IsRoot || this.Parent == null)
                        {
                            return this.m_selectSet;
                        }
                        else
                        {
                            return this.getSet;
                        }
                    }
                    set
                    {
                        if (this.IsRoot || this.Parent == null)
                        {
                            this.m_selectSet = value;
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

                public NodeStateOperation OnAttachedToRootAttempt { get; set; }
                public NodeStateOperation OnChildrenChanged { get; set; }
                public KLNodeOperation OnAttachedToRootSuccess { get; set; }
                public KLNodeOperation OnRemovedFromRootSuccess { get; set; }

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

                public KeyLogicRoot(KeyDelegator kDel) : base(kDel, int.MaxValue)
                {
                    KRoot = this;
                    Root = null;
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

    public class KeyLogicNode : Ruminate.DataStructures.TreeNode<KeyGroup>
    {

        #region members_memberlike_properties

            #region public

                //static
                public static int MINMAXOBJ = 1;

                //memberlike properties
                public KeyLogicRoot KRoot
                {
                    get;
                    set;
                }

            #endregion

            #region internal

            #endregion

            #region protected

                protected logics m_log;
                protected methods m_method;
                protected KeyDelegator m_delegator;
                protected KeyGroup m_group;
                protected logicStates m_state;
                protected bool m_last;//is the keygroup associated with this node placed last in a list evaluation or first?
                protected bool m_overwrite;
                protected bool m_clamp;
                protected int m_maxObj;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                new public bool IsRoot
                {
                    get
                    {
                        return KRoot == this;
                    }
                }
                new public bool Attached
                {
                    get
                    {
                        return (KRoot != null);
                    }
                }
                new public KeyGroup Data
                {
                    get;
                    protected set;
                }

                public logics log
                {
                    get
                    {
                        return this.m_log;
                    }
                    set
                    {
                        this.m_log = value;
                        this.DfsOperation(node =>
                        {
                            if (this.OnLogicChanged != null)
                            {
                                this.OnLogicChanged(node);
                            }
                        });
                    }
                }

                public KeyDelegator delegator
                {
                    get
                    {
                        return this.m_delegator;
                    }
                }

                public methods method
                {
                    get
                    {
                        return this.m_method;
                    }
                    set
                    {
                        this.m_method = value;
                        this.DfsOperation(node =>
                        {
                            if (this.OnMethodChanged != null)
                            {
                                this.OnMethodChanged(node);
                            }
                        });
                        
                    }
                }

                public logicStates state
                {
                    get
                    {
                        return this.m_state;
                    }
                }

                public bool last
                {
                    get
                    {
                        return this.m_last;
                    }
                }

                public int maxObj
                {
                    get
                    {
                        return this.m_maxObj;
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

                //http://stackoverflow.com/questions/937181/c-sharp-pattern-to-prevent-an-event-handler-hooked-twice
                public NodeOperation<KeyGroup> OnLogicChanged
                {
                    get;
                    set;
                }

                public NodeOperation<KeyGroup> OnMethodChanged
                {
                    get;
                    set;
                }

                public logicStates evalGroupChanged(KeyPair kPair, KeyGroup kGroup)
                {
                    logicStates eval = this.state;
                    this.evaluation();
                    if (eval != this.state)
                    {
                        if (Attached)
                        {
                            return this.KRoot.Dfs2StateOperation(node => node.evaluation());
                        }
                    }
                    return logicStates.TRUE;
                }

                public logicStates evalPairChanged(KeyGroup group)
                {
                    this.evaluation();
                    logicStates eval = logicStates.TRUE;
                    if (Attached)
                    {
                        eval = this.KRoot.Dfs2StateOperation(node => node.evaluation());
                        this.evaluation();
                    }
                    return eval;
                }

                

                

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
                public KeyLogicNode(KeyDelegator kDel, int maxO, bool clmp = false, bool ovrw = false, bool lst = false) : base(null)
                {
                    this.m_delegator = kDel;
                    this.m_log = logics.NONE;
                    this.m_method = methods.DEACTIVATE;
                    this.m_state = logicStates.INDETERMINATE;
                    this.Data = null;
                    this.m_last = lst;
                    this.m_overwrite = ovrw;
                    this.m_clamp = clmp;
                    this.KRoot = null;
                    this.Root = null;
                    if (maxO < KeyLogicNode.MINMAXOBJ)
                    {
                        maxO = KeyLogicNode.MINMAXOBJ;
                    }
                    this.m_maxObj = maxO;
                }
                public logicStates evaluation()
                {
                    logicStates currState = this.state;
                    List<logicStates> sList = new List<logicStates>();
                    foreach (KeyLogicNode cVal in this.Children.Cast<KeyLogicNode>().ToList())
                    {
                        sList.Add(cVal.state);
                    }
                    if (this.Data != null)
                    {
                        if (!this.m_last)
                        {
                            sList.Insert(0, this.Data.evaluate());
                        }
                        else
                        {
                            sList.Add(this.Data.evaluate());
                        }
                    }
                    logicStates result = KeyLogicManager.evaluate(sList, this.m_log, this.m_clamp, this.m_overwrite);
                    this.m_state = result;
                    return result;
                }

                public logicStates DfsStateOperation(NodeStateOperation operation, int maxObj = 2)
                {
                    List<logicStates> tempStates = new List<logicStates>();
                    logicStates adder = operation(this);
                    foreach (var child in Children)
                    {
                        tempStates.Add(((KeyLogicNode)child).DfsStateOperation(operation));
                    }
                    if(this.m_last && tempStates.Count < maxObj)
                    {
                        tempStates.Add(adder);
                    }
                    else if (tempStates.Count < maxObj)
                    {
                        tempStates.Insert(0, adder);
                    }
                    return KeyLogicManager.evaluate(tempStates, this.log, this.m_clamp, this.m_overwrite);
                }

                public logicStates DfsStateOperationChildren(NodeStateOperation operation, int maxObj = 2)
                {
                    List<logicStates> tempStates = new List<logicStates>();
                    logicStates adder = operation(this);
                    foreach (var child in Children)
                    {
                        tempStates.Add(((KeyLogicNode)child).DfsStateOperation(operation, maxObj));
                    }
                    return KeyLogicManager.evaluate(tempStates, this.log, this.m_clamp, this.m_overwrite);
                }

                public List<logicStates> DfsStatesListOperation(NodeStateListOperation operation, int maxObj = 2)
                {
                    List<logicStates> tempList = new List<logicStates>();
                    List<logicStates> itemList = new List<logicStates>();
                    int tracker = 0;
                    List<logicStates> adder = operation(this);
                    foreach (var child in Children)
                    {
                        itemList = ((KeyLogicNode)child).DfsStatesListOperation(operation, maxObj);
                        tempList.AddRange(itemList);
                        tracker++;
                    }
                    if (this.m_last && tracker < maxObj)
                    {
                        tempList.AddRange(adder);
                    }
                    else if (tracker < maxObj)
                    {
                        tempList.InsertRange(0, adder);
                    }
                    return tempList;
                }

                public logicStates DfsFilteredListStateOperation(NodeStateListOperation operation, List<logicStates> disqualifiers, int maxObj = 2)
                {
                    List<logicStates> tempList = new List<logicStates>();
                    List<logicStates> adder = operation(this);
                    foreach(var child in this.Children)
                    {
                        tempList.Add(((KeyLogicNode)child).DfsFilteredListStateOperation(operation, disqualifiers, maxObj));
                    }
                    if (this.m_last && tempList.Count < maxObj)
                    {
                        tempList.AddRange(adder);
                    }
                    else if (tempList.Count < maxObj)
                    {
                        tempList.InsertRange(0, adder);
                    }
                    if (tempList.Intersect(disqualifiers) != new List<logicStates>())
                    {
                        return logicStates.FALSE;
                    }
                    return logicStates.TRUE;
                }

                public logicStates DfsFilteredListStateOperationChildren(NodeStateListOperation operation, List<logicStates> disqualifiers, int maxObj = 2)
                {
                    List<logicStates> tempList = new List<logicStates>();
                    foreach (var child in this.Children)
                    {
                        tempList.Add(((KeyLogicNode)child).DfsFilteredListStateOperation(operation, disqualifiers, maxObj));
                    }
                    if (tempList.Intersect(disqualifiers) != new List<logicStates>())
                    {
                        return logicStates.FALSE;
                    }
                    return logicStates.TRUE;
                }

                public List<logicStates> DfsStatesListOperationChildren(NodeStateListOperation operation, int maxObj = 2)
                {
                    List<logicStates> tempList = new List<logicStates>();
                    List<logicStates> itemList = new List<logicStates>();
                    foreach (var child in Children)
                    {
                        itemList = ((KeyLogicNode)child).DfsStatesListOperation(operation, maxObj);
                        tempList.AddRange(itemList);
                    }
                    return tempList;
                }

                public void Dfs2Operation(NodeOperation<KeyGroup> operation)
                {
                    foreach (var child in Children)
                    {
                        ((KeyLogicNode)child).Dfs2Operation(operation);
                    }
                    if (this.Data != null)
                    {
                        operation(this);
                    }
                }

                public logicStates Dfs2StateOperation(NodeStateOperation operation, int maxObj = 2)
                {
                    List<logicStates> tempStates = new List<logicStates>();
                    foreach (var child in Children)
                    {
                        tempStates.Add(((KeyLogicNode)child).Dfs2StateOperation(operation));
                    }
                    logicStates adder = operation(this);
                    if(this.m_last && tempStates.Count < maxObj)
                    {
                        tempStates.Add(adder);
                    }
                    else if (tempStates.Count < maxObj)
                    {
                        tempStates.Insert(0, adder);
                    }
                    return KeyLogicManager.evaluate(tempStates, this.log, this.m_clamp, this.m_overwrite);
                }

                public logicStates Dfs2StateOperationChildren(NodeStateOperation operation, int maxObj = 2)
                {
                    List<logicStates> tempStates = new List<logicStates>();
                    foreach (var child in Children)
                    {
                        tempStates.Add(((KeyLogicNode)child).Dfs2StateOperation(operation, maxObj));
                    }
                    return KeyLogicManager.evaluate(tempStates, this.log, this.m_clamp, this.m_overwrite);
                }

                public logicStates Dfs2FilteredListStateOperation(NodeStateListOperation operation, List<logicStates> disqualifiers, int maxObj = 2)
                {
                    List<logicStates> tempList = new List<logicStates>();
                    foreach (var child in this.Children)
                    {
                        tempList.Add(((KeyLogicNode)child).Dfs2FilteredListStateOperation(operation, disqualifiers, maxObj));
                    }
                    List<logicStates> adder = operation(this);
                    if (this.m_last && tempList.Count < maxObj)
                    {
                        tempList.AddRange(adder);
                    }
                    else if (tempList.Count < maxObj)
                    {
                        tempList.InsertRange(0, adder);
                    }
                    if (tempList.Intersect(disqualifiers) != new List<logicStates>())
                    {
                        return logicStates.FALSE;
                    }
                    return logicStates.TRUE;
                }

                public logicStates Dfs2FilteredListStateOperationChildren(NodeStateListOperation operation, List<logicStates> disqualifiers, int maxObj = 2)
                {
                    List<logicStates> tempList = new List<logicStates>();
                    foreach (var child in this.Children)
                    {
                        tempList.Add(((KeyLogicNode)child).Dfs2FilteredListStateOperation(operation, disqualifiers, maxObj));
                    }
                    if (tempList.Intersect(disqualifiers) != new List<logicStates>())
                    {
                        return logicStates.FALSE;
                    }
                    return logicStates.TRUE;
                }

                public List<logicStates> Dfs2StatesListOperation(NodeStateListOperation operation, int maxObj = 2)
                {
                    List<logicStates> tempList = new List<logicStates>();
                    List<logicStates> itemList = new List<logicStates>();
                    int tracker = 0;
                    foreach (var child in Children)
                    {
                        itemList = ((KeyLogicNode)child).Dfs2StatesListOperation(operation, maxObj);
                        tempList.AddRange(itemList);
                        tracker++;
                    }
                    List<logicStates> adder = operation(this);
                    if (this.m_last && tracker < maxObj)
                    {
                        tempList.AddRange(adder);
                    }
                    else if (tracker < maxObj)
                    {
                        tempList.InsertRange(0, adder);
                    }
                    return tempList;
                }

                public List<logicStates> Dfs2StatesListOperationChildern(NodeStateListOperation operation, int maxObj = 2)
                {
                    List<logicStates> tempList = new List<logicStates>();
                    List<logicStates> itemList = new List<logicStates>();
                    int tracker = 0;
                    foreach (var child in Children)
                    {
                        itemList = ((KeyLogicNode)child).Dfs2StatesListOperation(operation, maxObj);
                        tempList.AddRange(itemList);
                        tracker++;
                    }
                    return tempList;
                }

                // Add/Remove via TreeNode
                public bool AddChild(KeyLogicNode child)
                {
                    if (Attached)
                    {
                        if (KRoot.containsNode(child))
                        {
                            return false;
                        }
                    }
                    var reserveP = child.Parent;
                    var reserveKRoot = child.KRoot;
                    child.Parent = this;
                    Children.Add(child);
                    child.Root = null;
                    child.KRoot = this.KRoot;
                    logicStates res = logicStates.TRUE;
                    this.evaluation();
                    if (Attached)
                    {
                        if (this.KRoot.OnAttachedToRootAttempt != null)
                        {
                            res = KRoot.OnAttachedToRootAttempt(this);
                        }
                    }
                    if (res != logicStates.TRUE)
                    {
                        Children.Remove(child);
                        child.Parent = reserveP;
                        child.KRoot = reserveKRoot;
                        return false;
                    }
                    if (this.Attached)
                    {
                        if (this.KRoot.OnAttachedToRootSuccess != null)
                        {
                            this.KRoot.OnAttachedToRootSuccess(this);
                        }
                    }
                    return true;
                }
                public bool RemoveChild(KeyLogicNode child)
                {
                    if (child.Parent != this || !Children.Contains(child))
                    {
                        return false;
                    }
                    Children.Remove(child);
                    logicStates res = logicStates.TRUE;
                    if (this.Attached || this.IsRoot)
                    {
                        res = this.KRoot.OnChildrenChanged(this);
                    }
                    if (res != logicStates.TRUE)
                    {
                        Children.Add(child);
                        return false;
                    }
                    else if(this.Attached || this.IsRoot)
                    {
                        this.KRoot.OnRemovedFromRootSuccess(this);
                    }
                    child.Parent = null;
                    return true;
                }
                public bool containsNode(KeyLogicNode node)//acts as dfs, recursive
                {
                    if (this == node)
                    {
                        return true;
                    }
                    foreach (KeyLogicNode child in this.Children.Cast<KeyLogicNode>().ToList())
                    {
                        if (child.containsNode(node))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                public logicStates setData(KeyGroup sVal)
                {
                    KeyGroup older = this.Data;
                    logicStates pastState = this.state;
                    this.Data = sVal;
                    this.evaluation();
                    logicStates result = logicStates.TRUE;
                    if (Attached)
                    {
                        result = this.KRoot.Dfs2StateOperation(node => node.evaluation());
                    }
                    else
                    {
                        KeyLogicNode par = this;
                        while (par.Parent != null)
                        {
                            par = (KeyLogicNode)par.Parent;
                        }
                        result = par.Dfs2StateOperation(node => node.evaluation());
                    }
                    if (result == logicStates.FALSE && this.log != logics.NONE)
                    {
                        this.Data = older;
                        this.evaluation();
                        return logicStates.FALSE;
                    }
                    else
                    {
                        if (older != null)
                        {//remove event listeners
                            older.parent = null;
                            older.signalGroupHasChanged -= this.evalGroupChanged;
                            older.groupAttemptStateChanged -= this.evalPairChanged;
                        }
                        //add event listeners to it
                        this.Data.parent = this;
                        this.Data.signalGroupHasChanged += this.evalGroupChanged;
                        this.Data.groupAttemptStateChanged += this.evalPairChanged;
                    }
                    return logicStates.TRUE;
                }
                public void setKRoot(KeyLogicRoot kr)
                {
                    this.KRoot = kr;
                }

                public bool isNode(KeyLogicNode node)//may want to get rid of this or modify
                {
                    if (this == node)
                        return true;
                    return false;
                }

                public selectionSet getSet
                {
                    get
                    {
                        if (!this.Attached)
                        {
                            KeyLogicNode test = (KeyLogicNode)this.Parent;
                            while (Parent != null && !Parent.IsRoot)
                            {
                                Parent = Parent.Parent;
                            }
                            if (Parent == null)
                            {
                                return null;
                            }
                            return ((KeyLogicRoot)Parent).selectSet;
                        }
                        return this.KRoot.selectSet;
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



}
