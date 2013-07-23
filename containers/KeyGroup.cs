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

    public class KeyGroup : ITreeNode<KeyGroup>
    {

        #region members_memberlike_properties

        #region public

        #endregion

        #region internal

        #endregion

        #region protected

        protected TreeNode<KeyGroup> m_parent;
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
        public KeyGroup()
        {
            this.m_parent = null;
            this.m_dict = new Dictionary<Keys, bool>();
        }


        public TreeNode<KeyGroup> GetTreeNode()
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
