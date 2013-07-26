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
                        return this.m_selectSet;
                    }
                    set
                    {
                        this.m_selectSet = value;
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

                public NodeStateOperation OnAttachedToRoot { get; set; }
                public NodeStateOperation OnChildrenChanged { get; set; }

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

                public KeyLogicRoot(KeyDelegator kDel) : base(kDel)
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

                public logics log
                {
                    get
                    {
                        return this.m_log;
                    }
                    set
                    {
                        this.m_log = value;
                        this.DfsOperation(node => this.OnLogicChanged(node));
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
                        this.DfsOperation(node => this.OnMethodChanged(node));
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

                public bool isNode(KeyLogicNode node, bool intended)//may want to get rid of this or modify
                {
                    if (this == node)
                        return intended;
                    return !intended;
                }

                public selectionSet getSet
                {
                    get
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
                public KeyLogicNode(KeyDelegator kDel, bool clmp = false, bool ovrw = false, bool lst = false) : base(null)
                {
                    this.m_delegator = kDel;
                    this.m_log = logics.NONE;
                    this.m_method = methods.DEACTIVATE;
                    this.m_state = logicStates.INDETERMINATE;
                    this.Data = null;
                    this.m_last = lst;
                    this.m_overwrite = ovrw;
                    this.m_clamp = clmp;
                }
                public logicStates evaluation()
                {
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
                    return KeyLogicManager.evaluate(sList, this.m_log, this.m_clamp, this.m_overwrite);
                }

                public logicStates DfsStateOperation(NodeStateOperation operation)
                {
                    List<logicStates> tempStates = new List<logicStates>();
                    logicStates adder = operation(this);
                    foreach (var child in Children)
                    {
                        tempStates.Add(((KeyLogicNode)child).DfsStateOperation(operation));
                    }
                    if(this.m_last)
                    {
                        tempStates.Add(adder);
                    }
                    else
                    {
                        tempStates.Insert(0, adder);
                    }
                    return KeyLogicManager.evaluate(tempStates, this.log, this.m_clamp, this.m_overwrite);
                }

                public List<logicStates> DfsStatesListOperation(NodeStateListOperation operation)
                {
                    List<logicStates> tempList = new List<logicStates>();
                    List<logicStates> itemList = new List<logicStates>();
                    List<logicStates> adder = operation(this);
                    foreach (var child in Children)
                    {
                        itemList = ((KeyLogicNode)child).DfsStatesListOperation(operation);
                        tempList.AddRange(itemList);
                    }
                    if (this.m_last)
                    {
                        tempList.AddRange(adder);
                    }
                    else
                    {
                        tempList.InsertRange(0, adder);
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

                public logicStates Dfs2StateOperation(NodeStateOperation operation)
                {
                    List<logicStates> tempStates = new List<logicStates>();
                    foreach (var child in Children)
                    {
                        tempStates.Add(((KeyLogicNode)child).Dfs2StateOperation(operation));
                    }
                    logicStates adder = operation(this);
                    if(this.m_last)
                    {
                        tempStates.Add(adder);
                    }
                    else
                    {
                        tempStates.Insert(0, adder);
                    }
                    return KeyLogicManager.evaluate(tempStates, this.log, this.m_clamp, this.m_overwrite);
                }

                public List<logicStates> Dfs2StatesListOperation(NodeStateListOperation operation)
                {
                    List<logicStates> tempList = new List<logicStates>();
                    List<logicStates> itemList = new List<logicStates>();
                    foreach (var child in Children)
                    {
                        itemList = ((KeyLogicNode)child).DfsStatesListOperation(operation);
                        tempList.AddRange(itemList);
                    }
                    List<logicStates> adder = operation(this);
                    if (this.m_last)
                    {
                        tempList.AddRange(adder);
                    }
                    else
                    {
                        tempList.InsertRange(0, adder);
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
                    else
                    {

                    }
                    var reserveP = child.Parent;
                    var reserveKRoot = child.KRoot;
                    child.Parent = this;
                    Children.Add(child);
                    child.Root = null;
                    child.KRoot = this.KRoot;
                    logicStates res = logicStates.FALSE;
                    if (Attached)
                    {
                        res = KRoot.OnAttachedToRoot(this);
                    }
                    if (res != logicStates.TRUE)
                    {
                        Children.Remove(child);
                        child.Parent = reserveP;
                        child.KRoot = reserveKRoot;
                        return false;
                    }
                    if (this.Attached || this.IsRoot)
                    {
                        res = Dfs2StateOperation(node => KRoot.OnChildrenChanged(node));
                    }
                    if (res != logicStates.TRUE)
                    {
                        Children.Remove(child);
                        child.Parent = reserveP;
                        child.KRoot = reserveKRoot;
                        return false;
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
                    logicStates res = logicStates.FALSE;
                    if (this.Attached || this.IsRoot)
                    {
                        res = Dfs2StateOperation(node => KRoot.OnChildrenChanged(node));
                    }
                    if (res != logicStates.TRUE)
                    {
                        Children.Add(child);
                        return false;
                    }
                    child.Parent = null;
                    child.KRoot = null;
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
