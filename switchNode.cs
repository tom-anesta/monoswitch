using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using monoswitch.content;

namespace monoswitch
{
    public class switchNode
    {

        #region members

            #region public

            #endregion

            #region protected

            #endregion

            #region private

                private s_switch m_child;
                private List<switchNode> m_successors;
                private List<switchNode> m_predecessors;
                private List<selectionSet> m_parents;//the selectionsets that use this switch node

                private switchNode m_lastPredecessor;//the last node that led into this one
                private switchNode m_lastSuccessor;//the last node that led out of this one.
                private switchNode m_intendedSuccessor;//where the node is currently pointing for next move
                private Boolean m_commited;//has the graph structure been committed?

            #endregion

        #endregion

        #region properties

            #region public

                public Boolean commited
                {
                    get
                    {
                        return this.m_commited;
                    }
                    set
                    {
                        return;//read only
                    }
                }

                public s_switch child
                {
                    get
                    {
                        return this.m_child;
                    }
                    set
                    {
                        this.m_child = value;
                    }
                }

                public List<switchNode> successors
                {
                    get
                    {//the user should not be able to modify the original list, send a shallow copy
                        List<switchNode> rVal = new List<switchNode>();
                        foreach (switchNode sn in this.m_successors)
                        {
                            rVal.Add(sn);
                        }
                        return rVal;
                    }
                    set//read only
                    {
                        return;
                    }
                }

                public List<switchNode> predecessors
                {//the user should not be able to modify the original list, send a shallow copy
                    get
                    {
                        List<switchNode> rVal = new List<switchNode>();
                        foreach (switchNode sn in this.m_predecessors)
                        {
                            rVal.Add(sn);
                        }
                        return rVal;
                    }
                    set//read only
                    {
                        return;
                    }
                }

                public List<selectionSet> parents
                {//the user should not be able to modify the original list, send a shallow copy
                    get
                    {//the user should not be able to modify the original list, send a shallow copy
                        List<selectionSet> rVal = new List<selectionSet>();
                        foreach (selectionSet ss in this.m_parents)
                        {
                            rVal.Add(ss);
                        }
                        return rVal;
                    }
                    set
                    {
                        return;//read only
                    }
                }

                public switchNode lastPredecessor
                {
                    get
                    {
                        return this.m_lastPredecessor;
                    }
                    set
                    {
                        foreach (switchNode sn in this.m_predecessors)
                        {
                            if (sn == value)
                            {
                                this.m_lastPredecessor = value;
                                return;
                            }
                        }
                        
                    }
                }

                public switchNode lastSuccesor
                {
                    get
                    {
                        return this.m_lastSuccessor;
                    }
                    set
                    {
                        foreach (switchNode sn in this.m_successors)
                        {
                            if (sn == value)
                            {
                                this.m_lastSuccessor = value;
                                return;
                            }
                        }
                    }
                }

                public switchNode intendedSuccessor
                {
                    get
                    {
                        return this.m_intendedSuccessor;
                    }
                    set
                    {
                        foreach (switchNode sn in this.m_successors)
                        {
                            if (sn == value)
                            {
                                this.m_intendedSuccessor = value;
                            }
                        }
                    }
                }
        

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region methods

            #region public

            //CONSTRUCTOR

            public switchNode()
            {
                this.m_child = null;
                this.m_lastPredecessor = null;
                this.m_lastSuccessor = null;
                this.m_intendedSuccessor = null;
                this.m_predecessors = new List<switchNode>();
                this.m_successors = new List<switchNode>();
                this.m_parents = new List<selectionSet>();
                this.m_commited = false;
            }

            public switchNode(s_switch val)
            {
                this.m_child = val;
                this.m_lastPredecessor = null;
                this.m_lastSuccessor = null;
                this.m_intendedSuccessor = null;
                this.m_predecessors = new List<switchNode>();
                this.m_successors = new List<switchNode>();
                this.m_parents = new List<selectionSet>();
                this.m_commited = false;
            }

            //INITIALIZATION FUNCTIONS

            //ends initialization
            public Boolean commit()
            {
                if (this.m_child == null)//does this node contain anything?
                {
                    this.m_commited = false;
                    return this.m_commited;
                }
                if(this.m_successors == null || this.m_successors.Count == 0)//check quant errors in successors
                {
                    this.m_commited = false;
                    return this.m_commited;
                }
                if(this.m_predecessors == null || this.m_predecessors.Count == 0)//check quant errors in predecessors
                {
                    this.m_commited = false;
                    return this.m_commited;
                }
                /*
                foreach (switchNode sn in this.m_successors)//check for self-references
                {
                    if(sn == this)
                    {
                        return this.m_commited;
                    }
                }
                foreach (switchNode sn in this.m_predecessors)//check for self-references
                {
                    if(sn == this)
                    {
                        this.m_commited = false;
                        return this.m_commited;
                    }
                }
                */
                if (!this.m_child.commit())
                {
                    this.m_commited = false;
                    return this.m_commited;
                }
                this.m_commited = true;
                return this.m_commited;
            }

            //UPDATE

            //OTHER PUBLIC FUNCTIONS

            //for setting up the thing

            public Boolean addPredecessor(switchNode val)
            {
                if (this.m_commited || val == null || val.commited)
                {
                    return false;
                }
                if (val != null)
                {
                    foreach (switchNode sn in this.m_predecessors)
                    {
                        if (sn == val)//if already present
                        {
                            return true;
                        }
                    }
                    this.m_predecessors.Add(val);
                    val.addSuccessor(this);
                    return true;
                }
                return false;
            }

            public Boolean addPredecessorAt(switchNode val, int index)
            {
                if (this.m_commited || val == null || val.commited)
                {
                    return false;
                }
                if(val != null && index >= 0 && index < this.m_predecessors.Count)
                {
                    if (this.m_predecessors[index] == val)//if already present
                    {
                        return true;
                    }
                    this.m_predecessors.Insert(index, val);
                    val.addSuccessor(this);
                    return true;
                }
                if (val != null && index >= 0)
                {
                    foreach (switchNode sn in this.m_predecessors)
                    {
                        if (sn == val)//if already present
                            return true;
                    }
                    this.m_predecessors.Add(val);
                    val.addSuccessor(this);
                    return true;
                }
                return false;
            }

            public Boolean addSuccessor(switchNode val)
            {
                if (this.m_commited || val == null || val.commited)
                {
                    return false;
                }
                if (val != null)
                {
                    foreach (switchNode sn in this.m_successors)
                    {
                        if (sn == val)//if already present
                        {
                            return true;
                        }
                    }
                    this.m_successors.Add(val);
                    val.addPredecessor(this);
                    return true;
                }
                return false;
            }

            public Boolean addSuccessorAt(switchNode val, int index)
            {
                if (this.m_commited || val == null || val.commited)
                {
                    return false;
                }
                if (val != null && index >= 0 && index < this.m_successors.Count)
                {
                    if (this.m_successors[index] == val)//if already present
                    {
                        return true;
                    }
                    this.m_successors.Insert(index, val);
                    val.addPredecessor(this);
                    return true;
                }
                if (val != null && index >= 0)
                {
                    foreach (switchNode sn in this.m_successors)
                    {
                        if (sn == val)//if already present
                            return true;
                    }
                    this.m_successors.Add(val);
                    val.addPredecessor(this);
                    return true;
                }
                return false;
            }

            public Boolean removePredecessor(switchNode val)
            {
                if (this.m_commited || val == null || val.commited)
                    return false;
                for (int i = 0; i < this.m_predecessors.Count; i++)
                {
                    if (this.m_predecessors[i] == val)
                    {
                        switchNode tempNode = this.m_successors[i];
                        if (tempNode.commited)
                            return false;
                        this.m_predecessors.RemoveAt(i);
                        tempNode.removeSuccessor(this);
                        return true;
                    }
                }
                return false;
            }

            public Boolean removePredecessorAt(int index)
            {
                if (this.m_commited)
                    return false;
                if (index >= this.m_predecessors.Count)
                    return false;
                switchNode tempNode = this.m_predecessors[index];
                if (tempNode.commited)
                    return false;
                this.m_predecessors.RemoveAt(index);
                tempNode.removeSuccessor(this);
                return true;
            }

            public Boolean removeSuccessor(switchNode val)
            {
                if (this.m_commited || val == null || val.commited)
                    return false;
                for (int i = 0; i < this.m_successors.Count; i++)
                {
                    if (this.m_successors[i] == val)
                    {
                        switchNode tempNode = this.m_successors[i];
                        if (tempNode.commited)
                            return false;
                        this.m_successors.RemoveAt(i);
                        tempNode.removePredecessor(this);
                        return true;
                    }
                }
                return false;
            }

            public Boolean removeSuccessorAt(int index)
            {
                if (this.m_commited)
                    return false;
                if (index >= this.m_successors.Count)
                    return false;
                switchNode tempNode = this.m_successors[index];
                if (tempNode.commited)
                    return false;
                this.m_successors.RemoveAt(index);
                tempNode.removePredecessor(this);
                return true;
            }

            public Boolean addParent(selectionSet p_val)
            {
                if(this.m_parents.Contains(p_val))
                {
                    return false;
                }
                //add to the list
                this.m_parents.Add(p_val);
                //add event handlers
                //return
                return true;
            }

            public Boolean removeParent(selectionSet p_val)
            {
                if (!this.m_parents.Contains(p_val))
                {
                    return false;
                }
                int index = this.m_parents.IndexOf(p_val);
                //remove event handlers
                //remove the item
                this.m_parents.RemoveAt(index);
                //return
                return true;
            }

            



            #endregion

        #region protected

        #endregion

        #region private

        #endregion

        #endregion

        #region events

        #region public

        #endregion

        #region protected

        #endregion

        #region private

        #endregion

        #endregion

    }
}
