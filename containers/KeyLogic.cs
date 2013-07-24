﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ruminate;
using Ruminate.DataStructures;
using Microsoft.Xna.Framework.Input;
using monoswitch;

namespace monoswitch.containers
{

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

                protected bool m_allowFalse;
                protected logics m_log;
                protected methods m_method;
                protected KeyDelegator m_delegator;
                protected KeyGroup m_group;

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
                    this.m_allowFalse = true;
                    this.m_log = logics.NONE;
                    this.m_method = methods.DEACTIVATE;
                }

                
                /*
                public KeyLogicNode() : this()
                {
                    this.m_allowFalse = true;
                    this.m_log = logics.NONE;
                    this.m_method = methods.DEACTIVATE;
                }
                */
                

                
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
