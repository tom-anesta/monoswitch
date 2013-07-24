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
    //resolution of indeterminate: 
    /*

    //XOR : false
    //Left : false
    //Right : false
    //BI : false
    //NOT : false
    
    */

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
    //NOT: 
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

                private static bool m_inited;
                private static Dictionary<logics, List<methods>> m_methodDict;
                private static Dictionary<logics, logicStates> m_evalDict;

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
                    KeyLogicManager.m_evalDict = new Dictionary<logics, logicStates>();
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
                    KeyLogicManager.m_evalDict[logics.XOR] = logicStates.FALSE;
                    KeyLogicManager.m_evalDict[logics.LEFT] = logicStates.FALSE;
                    KeyLogicManager.m_evalDict[logics.RIGHT] = logicStates.FALSE;
                    KeyLogicManager.m_evalDict[logics.BI] = logicStates.FALSE;
                    KeyLogicManager.m_evalDict[logics.NOT] = logicStates.FALSE;
                    KeyLogicManager.m_evalDict[logics.NONE] = logicStates.FALSE;
                    //mark as inited
                    KeyLogicManager.m_inited = true;
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
