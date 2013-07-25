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

    public delegate logicStates NodeStateOperation<T>(TreeNode<T> node) where T : class;

    public class KeyLogicRoot : KeyLogicNode
    {

        #region members_memberlike_properties

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

                public NodeOperation<KeyGroup> OnAttachedToRoot { get; set; }
                public NodeOperation<KeyGroup> OnChildrenChanged { get; set; }

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

                //respond to the thing
                public bool testValid()
                {
                    List<Boolean> valList = new List<bool>();
                    return true;
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
                public KeyLogicNode(KeyDelegator kDel) : base(null)
                {
                    this.m_delegator = kDel;
                    this.m_log = logics.NONE;
                    this.m_method = methods.DEACTIVATE;
                    this.m_state = logicStates.INDETERMINATE;
                    this.Data = null;
                    this.m_last = false;
                }

                
                /*
                public KeyLogicNode() : this()
                {
                    this.m_log = logics.NONE;
                    this.m_method = methods.DEACTIVATE;
                }
                */
                public logicStates clampedEvaluation()
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
                    return KeyLogicManager.evaluate(sList, this.m_log, true);
                }


                //statics
                public static bool evaluate(bool left, logics? type, bool? right = null)
                {
                    if (!type.HasValue)
                    {
                        return left;
                    }
                    if (type != logics.NOT && !right.HasValue)
                    {
                        return false;
                    }
                    bool rVal = false;
                    switch (type.Value)
                    {
                        case logics.NOT:
                            rVal = !left;
                            break;
                        case logics.XOR:
                            rVal = ((left && !right.Value) || (!left && right.Value));
                            break;
                        case logics.LEFT:
                            rVal = (!(right.Value && !left));
                            break;
                        case logics.RIGHT:
                            rVal = (!(!right.Value && left));
                            break;
                        case logics.BI:
                            rVal = (!((left && !right.Value) || (!left && right.Value)));
                            break;
                        default:
                            rVal = false;
                            break;
                    }
                    return rVal;
                }

                public logicStates DfsOperation(NodeStateOperation<KeyGroup> operation)
                {
                    List<logicStates> tempStates = new List<logicStates>();
                    if (this.Data != null)
                    {
                        tempStates.Add(operation(this));
                    }
                    foreach (var child in Children)
                    {
                        tempStates.Add(((KeyLogicNode)child).DfsOperation(operation));
                    }
                    return this.clampedEvaluation();
                }

                public void CompositeOperation(NodeOperation<KeyGroup> operation)
                {
                    foreach (var child in Children)
                    {
                        child.DfsOperation(operation);
                    }
                    if (this.Data != null)
                    {
                        operation(this);
                    }
                }

                public void CompositeOperation(NodeStateOperation<KeyGroup> operation)
                {
                    List<logicStates> tempStates = new List<logicStates>();
                    logicStates tempB = logicStates.INDETERMINATE;
                    foreach (var child in Children)
                    {
                        tempB = ((KeyLogicNode)child).DfsOperation(operation);
                        //should we do something if tempB is false?
                        tempStates.Add(tempB);
                    }
                    if (this.Data != null)
                    {
                        tempB = operation(this);
                        //should we do something if tempB is false?
                        tempStates.Add(tempB);
                    }
                    this.clampedEvaluation();
                }

                // Add/Remove via TreeNode
                public new void AddChild(KeyLogicNode child)
                {
                    child.Parent = this;
                    Children.Add(child);
                    if (Root == null)
                    {
                        return;
                    }
                    child.Root = null;
                    child.KRoot = this.KRoot;
                    KRoot.OnAttachedToRoot(this);
                    //Root.OnAttachedToRoot(this);
                    if (this.Attached || this.IsRoot)
                    {
                        DfsOperation(node => KRoot.OnChildrenChanged(node));
                    }
                }

                public new void RemoveChild(KeyLogicNode child)
                {
                    if (child.Parent != this || !Children.Contains(child))
                    {
                        return;
                    }
                    child.Parent = null;
                    Children.Remove(child);
                    child.KRoot = null;
                    if (this.Attached || this.IsRoot)
                    {
                        DfsOperation(node => KRoot.OnChildrenChanged(node));
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
