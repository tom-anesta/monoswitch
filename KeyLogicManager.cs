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
        ACTIVATE, DEACTIVATE
    }
    //applying the following to operations
    /*
    
    //ACTIVATE: 
        //XOR on attempt [true, true]
        //LEFT on attempt 
    
    
    
    
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
                private static Dictionary<logics, List<methods>> m_dict;

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
                        if (value == true)
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
                    KeyLogicManager.m_dict = new Dictionary<logics, List<methods>>();
                    //assign values to the dictionary
                    //List<methods> 

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
