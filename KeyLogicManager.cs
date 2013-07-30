using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace monoswitch
{
    //these logics should be constrained by constraint satisfaction algorithms
    //the author does not currently have enough knowledge to implement constraint satisfaction algorithms
    //possible states
    public enum logicStates
    {
        TRUE, FALSE, INDETERMINATE, NONE//none for there is no item
    }
    //in what order are they evaluated?
    public enum logDirections
    {
        LEFT, RIGHT, BOTH
    }
    //attribute to say how to evaluate
    [AttributeUsage(AttributeTargets.Field)]
    public class DirectionAttribute : System.Attribute
    {
        public readonly logDirections direction;
        public DirectionAttribute(logDirections dir)  // url is a positional parameter
        {
            this.direction = dir;
        }
    }
    
    public enum logics
    {
        [DirectionAttribute(logDirections.RIGHT)]XOR,
        [DirectionAttribute(logDirections.LEFT)]LEFT,
        [DirectionAttribute(logDirections.RIGHT)]RIGHT,
        [DirectionAttribute(logDirections.BOTH)]BI,//we should to check from both sides
        [DirectionAttribute(logDirections.RIGHT)]NONE,//doesn't really matter
        [DirectionAttribute(logDirections.BOTH)]NOT
    }
    
    //what should we do?
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

                private static bool m_inited = false;
                private static Dictionary<logics, List<methods>> m_methodDict;
                private static Dictionary<logics, logicStates> m_indEvalDict;
                private static Dictionary<Tuple<logics, logicStates>, List<Tuple<logicStates, logicStates>>> m_evalDict;
                //evaluation keys
                //all both falses are true except in not.  otherwise, indeterminate is chosen if could possibley evaluate to false, otherwise to true
                private static readonly Tuple<logicStates, logicStates> s_falseB = Tuple.Create(logicStates.FALSE, logicStates.FALSE);
                private static readonly Tuple<logicStates, logicStates> s_trueB = Tuple.Create(logicStates.TRUE, logicStates.TRUE);
                private static readonly Tuple<logicStates, logicStates> s_leftT = Tuple.Create(logicStates.TRUE, logicStates.FALSE);
                private static readonly Tuple<logicStates, logicStates> s_rightT = Tuple.Create(logicStates.FALSE, logicStates.TRUE);
                private static readonly Tuple<logicStates, logicStates> s_indlt = Tuple.Create(logicStates.TRUE, logicStates.INDETERMINATE);
                private static readonly Tuple<logicStates, logicStates> s_indrt = Tuple.Create(logicStates.INDETERMINATE, logicStates.TRUE);
                private static readonly Tuple<logicStates, logicStates> s_indlf = Tuple.Create(logicStates.FALSE, logicStates.INDETERMINATE);
                private static readonly Tuple<logicStates, logicStates> s_indrf = Tuple.Create(logicStates.INDETERMINATE, logicStates.FALSE);
                private static readonly Tuple<logicStates, logicStates> s_indB = Tuple.Create(logicStates.INDETERMINATE, logicStates.INDETERMINATE);
                private static readonly Tuple<logicStates, logicStates> s_noneB = Tuple.Create(logicStates.NONE, logicStates.NONE);
                private static readonly Tuple<logicStates, logicStates> s_nonelt = Tuple.Create(logicStates.TRUE, logicStates.NONE);
                private static readonly Tuple<logicStates, logicStates> s_nonelf = Tuple.Create(logicStates.FALSE, logicStates.NONE);
                private static readonly Tuple<logicStates, logicStates> s_nonert = Tuple.Create(logicStates.NONE, logicStates.TRUE);
                private static readonly Tuple<logicStates, logicStates> s_nonerf = Tuple.Create(logicStates.NONE, logicStates.FALSE);
                private static readonly Tuple<logicStates, logicStates> s_nonelind = Tuple.Create(logicStates.INDETERMINATE, logicStates.NONE);
                private static readonly Tuple<logicStates, logicStates> s_nonerind = Tuple.Create(logicStates.NONE, logicStates.INDETERMINATE);
                //16 different pairs
                private static readonly Tuple<logicStates, logicStates>[] s_xorTrues = { s_falseB, s_leftT, s_rightT, s_indrf, s_indlf, s_nonelf, s_nonelt, s_nonerf, s_nonert, s_nonerind, s_nonelind, s_noneB };//12
                private static readonly Tuple<logicStates, logicStates>[] s_xorFalses = { s_trueB };//13
                private static readonly Tuple<logicStates, logicStates>[] s_xorInds = { s_indrt, s_indlt, s_indB };//16.  xor full
                private static readonly Tuple<logicStates, logicStates>[] s_leftTrues = { s_falseB, s_leftT, s_trueB, s_indrf, s_indlt, s_noneB, s_nonelt, s_nonelf, s_nonelind, s_nonert, s_nonerf, s_nonerind };//12
                private static readonly Tuple<logicStates, logicStates>[] s_leftFalses = {s_rightT};//13
                private static readonly Tuple<logicStates, logicStates>[] s_leftInds = {s_indrt, s_indB, s_indlf};//16.  left full
                private static readonly Tuple<logicStates, logicStates>[] s_rightTrues = { s_falseB, s_rightT, s_trueB, s_indlf, s_indrt, s_noneB, s_nonelt, s_nonelf, s_nonelind, s_nonert, s_nonerf, s_nonerind};//12
                private static readonly Tuple<logicStates, logicStates>[] s_rightFalses = { s_leftT};//13
                private static readonly Tuple<logicStates, logicStates>[] s_rightInds = { s_indB, s_indrf, s_indlt };//16.  right full
                private static readonly Tuple<logicStates, logicStates>[] s_biTrues = { s_falseB, s_trueB, s_noneB, s_nonelt, s_nonelf, s_nonelind, s_nonert, s_nonerf, s_nonerind };//9
                private static readonly Tuple<logicStates, logicStates>[] s_biFalses = { s_leftT, s_rightT};//11
                private static readonly Tuple<logicStates, logicStates>[] s_biInds = { s_indlf, s_indlt, s_indrf, s_indrt, s_indB };//16.  bi full
                private static readonly Tuple<logicStates, logicStates>[] s_NoneTrues = { s_falseB, s_trueB, s_leftT, s_rightT, s_indrt, s_indrf, s_indlt, s_indlf, s_indB, s_noneB, s_nonelt, s_nonelf, s_nonelind, s_nonert, s_nonerf, s_nonerind};//16, none full
                private static readonly Tuple<logicStates, logicStates>[] s_NoneFalses = { };//0
                private static readonly Tuple<logicStates, logicStates>[] s_NoneInds = { };//0
                private static readonly Tuple<logicStates, logicStates>[] s_NotTrues = { s_falseB, s_rightT, s_leftT, s_indrf, s_indlf, s_noneB, s_nonelf, s_nonerf };//8
                private static readonly Tuple<logicStates, logicStates>[] s_NotFalses = { s_trueB, s_nonelt, s_nonert };//3
                private static readonly Tuple<logicStates, logicStates>[] s_NotInds = {s_nonerind, s_nonelind, s_indB, s_indrt, s_indlt};//5.  not full

                
                

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
                    KeyLogicManager.m_evalDict = new Dictionary<Tuple<logics, logicStates>,List<Tuple<logicStates,logicStates>>>();
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
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.XOR, logicStates.TRUE)] = KeyLogicManager.s_xorTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.XOR, logicStates.FALSE)] = KeyLogicManager.s_xorFalses.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.XOR, logicStates.INDETERMINATE)] = KeyLogicManager.s_xorInds.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.LEFT, logicStates.TRUE)] = KeyLogicManager.s_leftTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.LEFT, logicStates.FALSE)] = KeyLogicManager.s_leftFalses.ToList();
                     KeyLogicManager.m_evalDict[Tuple.Create(logics.LEFT, logicStates.INDETERMINATE)] = KeyLogicManager.s_leftInds.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.RIGHT, logicStates.TRUE)] = KeyLogicManager.s_rightTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.RIGHT, logicStates.FALSE)] = KeyLogicManager.s_rightFalses.ToList();
                     KeyLogicManager.m_evalDict[Tuple.Create(logics.RIGHT, logicStates.INDETERMINATE)] = KeyLogicManager.s_rightInds.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.BI, logicStates.TRUE)] = KeyLogicManager.s_biTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.BI, logicStates.FALSE)] = KeyLogicManager.s_biFalses.ToList();
                     KeyLogicManager.m_evalDict[Tuple.Create(logics.BI, logicStates.INDETERMINATE)] = KeyLogicManager.s_biInds.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NOT, logicStates.TRUE)] = KeyLogicManager.s_NotTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NOT, logicStates.FALSE)] = KeyLogicManager.s_NotFalses.ToList();
                     KeyLogicManager.m_evalDict[Tuple.Create(logics.NOT, logicStates.INDETERMINATE)] = KeyLogicManager.s_NotInds.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NONE, logicStates.TRUE)] = KeyLogicManager.s_NoneTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NONE, logicStates.FALSE)] = KeyLogicManager.s_NoneTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NONE, logicStates.INDETERMINATE)] = KeyLogicManager.s_NoneInds.ToList();

                    //mark as inited
                    KeyLogicManager.m_inited = true;
                }
                public static logicStates evaluate(List<logicStates> logList, logics lType, bool clamp = false, bool overwrite = false)
                {//clamp says results should be true or false based on dictionary, overwrite says make the first argument the evaluation of the last two arguments
                    if (!KeyLogicManager.inited)
                    {
                        KeyLogicManager.init();
                    }
                    List<logicStates> indList = null;
                    bool successClamp = false;
                    if (clamp && KeyLogicManager.m_indEvalDict.ContainsKey(lType))
                    {
                        indList = logList.ToList();
                        for (int i = 0; i < indList.Count; i++ )
                        {
                            if (indList[i] == logicStates.INDETERMINATE)
                            {
                                indList[i] = KeyLogicManager.m_indEvalDict[lType];
                            }
                        }
                    }
                    logDirections dir = logDirections.RIGHT;
                    DirectionAttribute dirAtt = KeyLogicManager.GetDirection(lType);
                    if(dirAtt != null)
                    {
                        dir = KeyLogicManager.GetDirection(lType).direction;
                    }
                    List<logicStates> results = new List<logicStates>();
                    if(dir == logDirections.RIGHT || dir == logDirections.BOTH)
                    {
                        List<logicStates> statesRight = successClamp ? indList.ToList() : logList.ToList();
                        logicStates val = logicStates.NONE;
                        if(statesRight.Count > 0)
                        {
                            val = statesRight[0];
                        }
                        while (statesRight.Count > 1)
                        {
                            val = KeyLogicManager.evaluate(statesRight[0], statesRight[1], lType);//no reason to clamp if we've already done
                            //Console.WriteLine("from right, evaluates to " + val);
                            if (val == logicStates.FALSE)
                            {
                                return logicStates.FALSE;
                            }
                            statesRight.RemoveAt(0);
                            if (overwrite)
                            {
                                statesRight[0] = val;
                            }
                        }
                        results.Add(val);
                    }
                    if (dir == logDirections.LEFT || dir == logDirections.BOTH)
                    {
                        List<logicStates> statesLeft = successClamp ? indList.ToList() : logList.ToList();
                        logicStates val = logicStates.NONE;
                        if(statesLeft.Count > 1)
                        {
                            val = statesLeft[statesLeft.Count-1];
                        }
                        while (statesLeft.Count > 1)
                        {
                            val = KeyLogicManager.evaluate(statesLeft[statesLeft.Count-2], statesLeft[statesLeft.Count-1], lType);//no reason to clamp if we've already don
                            //Console.WriteLine("from left, evaluates to " + val);
                            if (val == logicStates.FALSE)
                            {
                                return logicStates.FALSE;
                            }
                            statesLeft.RemoveAt(statesLeft.Count-1);
                            if (overwrite)
                            {
                                statesLeft[statesLeft.Count-1] = val;
                            }
                        }
                        results.Add(val);
                    }
                    if(results.Contains(logicStates.FALSE))
                    {
                        return logicStates.FALSE;
                    }
                    if(results.Contains(logicStates.INDETERMINATE))
                    {
                        return logicStates.INDETERMINATE;
                    }
                    if(results.Contains(logicStates.TRUE))
                    {
                        return logicStates.TRUE;
                    }
                    //Console.WriteLine("no false or true or indeterminate present in results, returning none");
                    return logicStates.NONE;
                }

                public static logicStates evaluate(logicStates left, logicStates right, logics lType, bool clamp = false)
                {//ignore right if not
                    if (!KeyLogicManager.inited)
                    {
                        KeyLogicManager.init();
                    }
                    if (!KeyLogicManager.m_methodDict.ContainsKey(lType) || !KeyLogicManager.m_indEvalDict.ContainsKey(lType))
                    {
                        //Console.WriteLine("dictionaries missing logic type");
                        return logicStates.FALSE;
                    }
                    if(clamp && KeyLogicManager.m_indEvalDict.ContainsKey(lType))
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
                    if (KeyLogicManager.m_evalDict.ContainsKey(Tuple.Create(lType, logicStates.TRUE)) && KeyLogicManager.m_evalDict.ContainsKey(Tuple.Create(lType, logicStates.FALSE)) && KeyLogicManager.m_evalDict.ContainsKey(Tuple.Create(lType, logicStates.INDETERMINATE)))
                    {
                        Tuple<logicStates, logicStates> eVal = Tuple.Create(left, right);
                        if (KeyLogicManager.m_evalDict[Tuple.Create(lType, logicStates.TRUE)].Contains(eVal))
                        {
                            return logicStates.TRUE;
                        }
                        else if (KeyLogicManager.m_evalDict[Tuple.Create(lType, logicStates.FALSE)].Contains(eVal))
                        {
                            return logicStates.FALSE;
                        }
                        else if (KeyLogicManager.m_evalDict[Tuple.Create(lType, logicStates.INDETERMINATE)].Contains(eVal))
                        {
                            return logicStates.INDETERMINATE;
                        }
                        else
                        {
                            //Console.WriteLine("cannot evaluate because item is not present in true, false, or ind");
                            return logicStates.NONE;
                        }
                    }
                    else
                    {
                        //Console.WriteLine("cannot evaluate because missing definition for true false and ind");
                        return logicStates.NONE;
                    }
                    /*
                    List<Tuple<logics, logicStates>> refList = new List<Tuple<logics,logicStates>>();//must get the dictionary objects from the dictionary
                    refList = KeyLogicManager.m_evalDict.Keys.Where(x => (x.Item1 == lType)).ToList();
                    if (refList.Count != 3)//if we don't have true, false, and indeterminate, we are done
                    {
                        return logicStates.NONE;
                    }
                    if(KeyLogicManager.m_evalDict[refList[0]].Where(x => ( (x.Item1 == left) && (x.Item2 == right) ) ).ToList().Count > 0 )
                    {
                        return refList[0].Item2;
                    }
                    else if (KeyLogicManager.m_evalDict[refList[1]].Where(x => ((x.Item1 == left) && (x.Item2 == right))).ToList().Count > 0)
                    {
                        return refList[1].Item2;
                    }
                    else if (KeyLogicManager.m_evalDict[refList[2]].Where(x => ((x.Item1 == left) && (x.Item2 == right))).ToList().Count > 0)
                    {
                        return refList[2].Item2;
                    }
                    else
                    {
                        return logicStates.NONE;
                    }
                    */
                   
                }

                //http://stackoverflow.com/questions/5097766/how-to-get-custom-attribute-values-for-enums
                public static DirectionAttribute GetDirection(logics logType)
                {
                    if (!KeyLogicManager.inited)
                    {
                        KeyLogicManager.init();
                    }

                    MemberInfo memberInfo = typeof(logics).GetMember(logType.ToString()).FirstOrDefault();
                    if (memberInfo != null)
                    {
                        DirectionAttribute attribute = (DirectionAttribute)memberInfo.GetCustomAttributes(typeof(DirectionAttribute), false).FirstOrDefault();
                        return attribute;
                    }
                    return null;
                }

                public static List<methods> validMethods(logics logVal)
                {
                    if (!KeyLogicManager.inited)
                    {
                        KeyLogicManager.init();
                    }

                    if (KeyLogicManager.m_methodDict.ContainsKey(logVal))
                    {
                        return KeyLogicManager.m_methodDict[logVal];
                    }
                    else
                    {
                        return null;
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


    }
}
