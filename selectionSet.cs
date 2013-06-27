using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace monoswitch
{
    class selectionSet
    {

        #region members

            #region public

                //static const

                public const uint DEFAULT_SCANNINGRATE = 666;//2/3 of a second
                public const uint DEFAULT_MIN_SCANNINGRATE = 20;//minimum 1/50th of a second

                //delegates

            #endregion

            #region protected

                protected uint m_scanningRate;
                protected List<switchNode> m_beginnings;
                protected List<switchNode> m_endings;
                protected switchNode m_startingNode;
                //relevant timer here
                //private monoswitchTimer m_timer
                protected Boolean m_commited;

            #endregion

            #region private

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

                public uint scanningRate
                {
                    get
                    {
                        return this.m_scanningRate;
                    }
                    set
                    {
                        if (!this.m_commited)
                        {
                                this.m_scanningRate = value;
                        }
                        return;
                    }
                }

                public Boolean running
                {
                    get
                    {
                        return false;
                    }
                    set
                    {
                        return;//read only
                    }
                }

                public uint timeToAdvance//the time until the next advance
                {
                    get
                    {
                        return 0;
                    }
                    set
                    {
                        return;//read only
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

                public selectionSet()
                {
                    this.m_scanningRate = selectionSet.DEFAULT_SCANNINGRATE;
                    this.m_beginnings = new List<switchNode>();
                    this.m_endings = new List<switchNode>();
                    this.m_commited = false;
                    this.m_startingNode = null;
                }

                public selectionSet(uint scanR)
                {
                    this.m_scanningRate = scanR;
                    this.m_beginnings = new List<switchNode>();
                    this.m_endings = new List<switchNode>();
                    this.m_commited = false;
                    this.m_startingNode = null;
                }

                //INITIALIZATION FUNCTIONS

                //UPDATE

                //OTHER PUBLIC FUNCTIONS

                public void Start()
                {
                    if (!this.m_commited)
                    {
                        return;
                    }
                }

                public void Stop()
                {
                    if (!this.m_commited)
                    {
                        return;//can't stop if it's not going
                    }
                }

                public void Reset()
                {
                    if (!this.m_commited)
                    {
                        return;
                    }

                }

                public void Restart()
                {
                    this.Reset();
                    this.Start();
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
