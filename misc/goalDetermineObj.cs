using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using monoswitch;

namespace monoswitch.misc
{
    public class goalDetermineObj
    {
        #region members_memberlike_properties

            #region public

            #endregion

            #region internal

            #endregion

            #region protected

                protected Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> m_currexamined;
                protected Dictionary<int, List<Tuple<logicStates, logicStates>>> m_goalStates;
                protected Dictionary<int, List<Tuple<logicStates, logicStates>>> m_lastStates;

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
                public goalDetermineObj(Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> cVal, List<Tuple<logicStates, logicStates>> gVals)
                {
                    this.m_currexamined = cVal;
                    this.m_goalStates = new Dictionary<int, List<Tuple<logicStates, logicStates>>>();
                    this.m_lastStates = new Dictionary<int, List<Tuple<logicStates, logicStates>>>();
                    int result = 0;
                    logicStates[] curr = new logicStates[]{this.m_currexamined.Item1.Item1, this.m_currexamined.Item2.Item1};
                    logicStates[] past = new logicStates[] { this.m_currexamined.Item1.Item2, this.m_currexamined.Item2.Item2 };
                    foreach (Tuple<logicStates, logicStates> gVal in gVals)
                    {
                        result = KeyLogicManager.distance(curr, new logicStates[]{gVal.Item1, gVal.Item2});
                        if(!this.m_goalStates.ContainsKey(result))
                        {
                            this.m_goalStates[result] = new List<Tuple<logicStates, logicStates>>();
                        }
                        this.m_goalStates[result].Add(gVal);
                    }
                    foreach (Tuple<logicStates, logicStates> lVal in gVals)
                    {
                        result = KeyLogicManager.distance(past, new logicStates[]{lVal.Item1, lVal.Item2});
                        if(!this.m_lastStates.ContainsKey(result))
                        {
                            this.m_lastStates[result] = new List<Tuple<logicStates, logicStates>>();
                        }
                        this.m_lastStates[result].Add(lVal);
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
