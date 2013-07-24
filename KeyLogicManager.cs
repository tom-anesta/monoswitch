using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace monoswitch
{
    //these logics should be constrained by constraint satisfaction algorithms
    //the author does not currently have enough knowledge to implement constraint satisfaction algorithms

    public enum logicStates
    {
        TRUE, FALSE, INDETERMINATE
    }

    public enum logics
    {
        XOR, LEFT, RIGHT, BI, NONE, NOT
    }
    //this logics does not confirm to propositional calculus.  all binary operators allow both arguments to be false
    //following pairs evaluate to true;
    /*
    //XOR: [false, false], [true, false], [false, true]
    //LEFT: [false, false], [true, false], [true, true]
    //RIGHT: [false, false], [false, true], [true, true]
    //BI: [false, false], [true, true]
    //NONE: [false, false], [true, false], [false, true], [true, true]
    //NOT: [false] (only applicable to end nodes?)
    */

    public enum methods
    {
        ACTIVATE, DEACTIVATE, FOLLOW, NONE
    }
    //applying the following to operations
    /*
    
    //ACTIVATE: 
        //XOR INVALID
        //LEFT on attempt [false, true] : [true, true]
        //RIGHT on attempt [true, false] : [true, true]
        //BI on attempt [true, false], [false, true] : [true, true]
        //NONE: INVALID
        //NOT : INVALID
    //DEACTIVATE
        //XOR on attempt [true, true] : [false, false]
        //LEFT on attempt [false, true] : [false, false]
        //RIGHT on attempt [true, false] : [false, false]
        //BI on attempt [true, false], [false, true] : [false, false]
        //NONE: INVALID
        //NOT on attempt [true], [false]
    //FOLLOW
        //XOR on attempt [true, true] from [true, false] : [false, true]
        //XOR on attempt [true, true] from [false, true] : [true, false]
        //LEFT on attempt [false, true] from [false, false] : [true, true] //does activate and deactivate based on situation
        //LEFT on attempt [false, true] from [true, true] : [false, false] //does activate and deactivate based on situation
        //RIGHT on attempt [true, false] from [false, false] : [true, true] //does activate and deactivate based on situation
        //RIGHT on attempt [true, false] from [true, true] : [false, false] //does activate and deactivate based on situation
        //BI on attempt [true, false] or [false, true] from [false, false] : [true, true] //does activate and deactivate based on situation
        //BI on attempt [true, false] or [false, true] from [true, true] : [false, false] //does activate and deactivate based on situation
        //NONE : INVALID
        //NOT : INVALID
    //NONE
        //XOR INVALID
        //LEFT INVALID
        //RIGHT INVALID
        //LEFT INVALID
        //BI : INVALID
        //NONE : no action taken
     
    
    
    */


    public static class KeyLogicManager
    {

        #region members_memberlike_properties

            #region public

            #endregion

            #region internal

            #endregion

            #region protected

            //following pairs evaluate to true;
            /*
            //XOR: [false, false], [true, false], [false, true]
            //LEFT: [false, false], [true, false], [true, true]
            //RIGHT: [false, false], [false, true], [true, true]
            //BI: [false, false], [true, true]
            //NONE: [false, false], [true, false], [false, true], [true, true]
            //NOT: [false] (only applicable to end nodes?)
            */

                private static bool m_inited;
                private static Dictionary<logics, List<methods>> m_methodDict;
                private static Dictionary<logics, logicStates> m_indEvalDict;
                private static Dictionary<Tuple<logics, bool>, List<Tuple<logicStates, logicStates>>> m_evalDict;
                //evaluation keys
                private static Tuple<logicStates, logicStates> s_falseB = Tuple.Create(logicStates.FALSE, logicStates.FALSE);
                private static Tuple<logicStates, logicStates> s_trueB = Tuple.Create(logicStates.TRUE, logicStates.TRUE);
                private static Tuple<logicStates, logicStates> s_leftT = Tuple.Create(logicStates.TRUE, logicStates.FALSE);
                private static Tuple<logicStates, logicStates> s_rightT = Tuple.Create(logicStates.FALSE, logicStates.TRUE);
                private static Tuple<logicStates, logicStates> s_indlt = Tuple.Create(logicStates.TRUE, logicStates.INDETERMINATE);
                private static Tuple<logicStates, logicStates> s_indrt = Tuple.Create(logicStates.INDETERMINATE, logicStates.TRUE);
                private static Tuple<logicStates, logicStates> s_indlf = Tuple.Create(logicStates.FALSE, logicStates.INDETERMINATE);
                private static Tuple<logicStates, logicStates> s_indrf = Tuple.Create(logicStates.INDETERMINATE, logicStates.FALSE);
                private static Tuple<logicStates, logicStates> s_indB = Tuple.Create(logicStates.INDETERMINATE, logicStates.INDETERMINATE);
                private static Tuple<logicStates, logicStates>[] s_xorTrues = {s_falseB, s_leftT, s_rightT};
                private static Tuple<logicStates, logicStates>[] s_xorFalses = {s_trueB, s_indlf, s_indlt, s_indrf, s_indrt, s_indB};
                private static Tuple<logicStates, logicStates>[] s_leftTrues = {s_falseB, s_leftT, s_trueB};
                private static Tuple<logicStates, logicStates>[] s_leftFalses = {s_rightT, s_indlf, s_indlt, s_indrf, s_indrt, s_indB};
                private static Tuple<logicStates, logicStates>[] s_rightTrues = {s_falseB, s_rightT, s_trueB};
                private static Tuple<logicStates, logicStates>[] s_rightFalses = {s_leftT, s_indlf, s_indlt, s_indrf, s_indrt, s_indB};
                private static Tuple<logicStates, logicStates>[] s_biTrues = {s_falseB, s_trueB};
                private static Tuple<logicStates, logicStates>[] s_biFalses = {s_leftT, s_rightT, s_indlf, s_indlt, s_indrf, s_indrt, s_indB};
                private static Tuple<logicStates, logicStates>[] s_NoneTrues = {s_falseB, s_trueB, s_leftT, s_rightT, s_indrt, s_indrf, s_indlt, s_indlf, s_indB};
                private static Tuple<logicStates, logicStates>[] s_NoneFalses = {};
                private static Tuple<logicStates, logicStates>[] s_NotTrues = {s_falseB, s_rightT, s_indrt, s_indrf, s_indlf, s_indB};
                private static Tuple<logicStates, logicStates>[] s_NotFalses = {s_leftT, s_indlt, s_trueB};

                
                

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                public static bool inited
                {
                    get
                    {
                        return KeyLogicManager.m_inited;
                    }
                    set
                    {
                        if (value == true && KeyLogicManager.m_inited == false)
                        {
                            KeyLogicManager.init();
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

                public static void init()
                {
                    //initialize the dictionary
                    KeyLogicManager.m_methodDict = new Dictionary<logics, List<methods>>();
                    KeyLogicManager.m_indEvalDict = new Dictionary<logics, logicStates>();
                    KeyLogicManager.m_evalDict = new Dictionary<Tuple<logics,bool>,List<Tuple<logicStates,logicStates>>>();
                    //assign values to the dictionary for matching methods
                    //XOR, LEFT, RIGHT, BI, NONE, NOT
                    KeyLogicManager.m_methodDict[logics.XOR] = new List<methods>(new methods[]{methods.DEACTIVATE, methods.FOLLOW});
                    List<methods> leftlist = new List<methods>(new methods[] { methods.ACTIVATE, methods.DEACTIVATE, methods.FOLLOW });
                    KeyLogicManager.m_methodDict[logics.LEFT] = leftlist;
                    KeyLogicManager.m_methodDict[logics.RIGHT] = leftlist.ToList();
                    KeyLogicManager.m_methodDict[logics.BI] = leftlist.ToList();
                    KeyLogicManager.m_methodDict[logics.NOT] = new List<methods>(new methods[] { methods.DEACTIVATE });
                    KeyLogicManager.m_methodDict[logics.NONE] = new List<methods>(new methods[] { methods.NONE });
                    //assign values to the dictionary for matching
                    //XOR : false, Left : false, Right : false, BI : false, NOT : false
                    KeyLogicManager.m_indEvalDict[logics.XOR] = logicStates.FALSE;
                    KeyLogicManager.m_indEvalDict[logics.LEFT] = logicStates.FALSE;
                    KeyLogicManager.m_indEvalDict[logics.RIGHT] = logicStates.FALSE;
                    KeyLogicManager.m_indEvalDict[logics.BI] = logicStates.FALSE;
                    KeyLogicManager.m_indEvalDict[logics.NOT] = logicStates.FALSE;
                    KeyLogicManager.m_indEvalDict[logics.NONE] = logicStates.FALSE;
                    //following pairs evaluate to true;
                    /*
                    //XOR: [false, false], [true, false], [false, true]
                    //LEFT: [false, false], [true, false], [true, true]
                    //RIGHT: [false, false], [false, true], [true, true]
                    //BI: [false, false], [true, true]
                    //NONE: [false, false], [true, false], [false, true], [true, true]
                    //NOT: [false] (only applicable to end nodes?)
                    */
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.XOR, true)] = KeyLogicManager.s_xorTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.XOR, false)] = KeyLogicManager.s_xorFalses.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.LEFT, true)] = KeyLogicManager.s_leftTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.LEFT, false)] = KeyLogicManager.s_leftFalses.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.RIGHT, true)] = KeyLogicManager.s_rightTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.RIGHT, false)] = KeyLogicManager.s_rightFalses.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.BI, true)] = KeyLogicManager.s_biTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.BI, false)] = KeyLogicManager.s_biFalses.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NOT, true)] = KeyLogicManager.s_NotTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NOT, false)] = KeyLogicManager.s_NotFalses.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NONE, true)] = KeyLogicManager.s_NoneTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NONE, false)] = KeyLogicManager.s_NoneTrues.ToList();

                    //mark as inited
                    KeyLogicManager.m_inited = true;
                }

                public static logicStates clampedEvaluate(List<logicStates> logList, logics lType)
                {


                    return logicStates.TRUE;
                }

                public static logicStates clampedEvaluate(logicStates left, logicStates right, logics lType)
                {//ignore right if not
                    if (!KeyLogicManager.m_methodDict.ContainsKey(lType) || !KeyLogicManager.m_indEvalDict.ContainsKey(lType))
                    {
                        return logicStates.FALSE;
                    }
                    if(KeyLogicManager.m_indEvalDict.ContainsKey(lType))
                    {
                        if(left == logicStates.INDETERMINATE)
                        {
                            left = m_indEvalDict[lType];
                        }
                        if(right == logicStates.INDETERMINATE)
                        {
                            right = m_indEvalDict[lType];
                        }
                    }
                    if(KeyLogicManager.m_evalDict.ContainsKey(Tuple.Create(lType, true)) && KeyLogicManager.m_evalDict.ContainsKey( Tuple.Create(lType, false)))
                    {
                        Tuple<logicStates, logicStates> eVal = Tuple.Create(left, right);
                        if(KeyLogicManager.m_evalDict[Tuple.Create(lType, true)].Contains(eVal))
                        {
                            return logicStates.TRUE;
                        }
                        else if(KeyLogicManager.m_evalDict[Tuple.Create(lType, false)].Contains(eVal))
                        {
                            return logicStates.FALSE;
                        }
                        else
                        {
                            return logicStates.INDETERMINATE;
                        }
                    }
                    else
                    {
                        return logicStates.INDETERMINATE;
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
