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

                public List<Tuple<int, int>> solutions()
                {
                    Console.WriteLine("retrieving solutions");
                    Console.WriteLine("the options to draw from are: ");
                    foreach (List<Tuple<logicStates, logicStates>> pGoalList in this.m_goalStates.Values)
                    {
                        foreach (Tuple<logicStates, logicStates> pGoal in pGoalList)
                        {
                            Console.Write(pGoal + " . ");
                        }
                    }
                    Console.WriteLine();
                    List<Tuple<int, int>> results = new List<Tuple<int, int>>();
                    List<Tuple<logicStates, logicStates>> converterList = new List<Tuple<logicStates, logicStates>>();
                    List<int> genum = this.m_goalStates.Keys.ToList();
                    genum.OrderBy(x => x);
                    List<int> lenum = this.m_lastStates.Keys.ToList();
                    lenum.OrderByDescending(x => x);
                    int gInt = 0;
                    int lInt = 0;
                    while (gInt < genum.Count)//prioritized so that the items with lowest distance to success first then by greater distance from the original (follow)
                    {
                        while (lInt < lenum.Count)
                        {
                            converterList.AddRange(this.m_goalStates[genum[gInt]].Intersect(this.m_lastStates[lenum[lInt]]));
                            lInt++;
                        }
                        gInt++;
                        lInt = lenum.Count-1;
                    }
                    Console.WriteLine("The number of solutions = " + converterList.Count);
                    Console.WriteLine("the currently examined item is " + this.m_currexamined.Item1.Item1 + " , " + this.m_currexamined.Item2.Item1);
                    int index = 0;
                    foreach (Tuple<logicStates, logicStates> tVal in converterList)
                    {
                        results.Add(Tuple.Create(KeyLogicManager.biDirectionalDistance(this.m_currexamined.Item1.Item1, tVal.Item1), KeyLogicManager.biDirectionalDistance(this.m_currexamined.Item2.Item1, tVal.Item2)));
                        //debug
                        Console.WriteLine("the command at index " + index + " = " + results[results.Count - 1].Item1 + " , " + results[results.Count - 1].Item2);
                        Console.WriteLine("The result for this operation will be " + (logicStates)(KeyLogicManager.biDirectionalDistance(this.m_currexamined.Item1.Item1, tVal.Item1) + (int)this.m_currexamined.Item1.Item1) + " , " + (logicStates)(KeyLogicManager.biDirectionalDistance(this.m_currexamined.Item2.Item1, tVal.Item2) + (int)this.m_currexamined.Item2.Item1));
                    }
                    results = results.Distinct().ToList();
                    Console.WriteLine("leaving getting solutions");
                    return results;
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
