using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ruminate;
using Ruminate.DataStructures;
using Microsoft.Xna.Framework.Input;

namespace monoswitch.containers
{
    public enum logics
    {
        NOT, OR, AND, XOR, LEFT, RIGHT, BI
    }

    public enum methods
    {
        ACTIVATE, DEACTIVATE
    }

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

                public NodeOperation<Dictionary<Keys, bool>> OnAttachedToRoot { get; set; }
                public NodeOperation<Dictionary<Keys, bool>> OnChildrenChanged { get; set; }

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

                public KeyLogicRoot() : base(null)
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

    public class KeyLogicNode : Ruminate.DataStructures.TreeNode<KeyLogicDictionary>
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
                public KeyLogicNode(KeyLogicDictionary dat) : base(dat)
                {
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
                        case logics.OR:
                            rVal = (left || right.Value);
                            break;
                        case logics.AND:
                            rVal = (left && right.Value);
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

            #endregion

            #region internal

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

    }

    public class KeyLogicDictionary : ITreeNode<KeyLogicDictionary>
    {

        #region members_memberlike_properties

            #region public

            #endregion

            #region internal

            #endregion

            #region protected

                protected TreeNode<KeyLogicDictionary> m_parent;
                protected Dictionary<Keys, bool> m_dict;

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
                        foreach (Keys kVal in this.m_dict.Keys)
                        {
                            if (!this.m_dict[kVal])
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
                        foreach (Keys kVal in this.m_dict.Keys)
                        {
                            if (this.m_dict[kVal])
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
                public KeyLogicDictionary()
                {
                    this.m_parent = null;
                    this.m_dict = new Dictionary<Keys, bool>();
                }

                public TreeNode<KeyLogicDictionary> GetTreeNode()
                {
                    return this.m_parent;
                }

                public void add(Keys kVal)
                {
                    if (!this.m_dict.ContainsKey(kVal))
                    {
                        this.m_dict[kVal] = false;
                    }
                }

                public void add(List<Keys> kList)
                {
                    foreach (Keys kVal in kList)
                    {
                        this.add(kVal);
                    }
                }

                //control methods
                public void deactivate(List<Keys> dVal)
                {
                    if (dVal.Count == 0)
                    {
                        foreach (Keys kv in this.m_dict.Keys)
                        {
                            this.m_dict[kv] = false;
                        }
                    }
                    foreach (Keys kv in dVal)
                    {
                        if (this.m_dict.ContainsKey(kv))
                        {
                            this.m_dict[kv] = false;
                        }
                    }
                }

                public void activate(List<Keys> aVal)
                {
                    if (aVal.Count == 0)
                    {
                        foreach (Keys kv in this.m_dict.Keys)
                        {
                            this.m_dict[kv] = true;
                        }
                    }
                    foreach (Keys kv in aVal)
                    {
                        if (this.m_dict.ContainsKey(kv))
                        {
                            this.m_dict[kv] = true;
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

}
