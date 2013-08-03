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
        [DirectionAttribute(logDirections.RIGHT)]NOT,
        [DirectionAttribute(logDirections.RIGHT)]AND
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
                private static Dictionary<Tuple<logics, methods, logicStates>, List<Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>>> m_methodEvalDict;
                //evaluation keys
                //all both falses are true except in not.  otherwise, indeterminate is chosen if could possibley evaluate to false, otherwise to true.  total 16
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
                //assign the pairs that are valid for the methods, item two of each item is the previous state.  in description, organizing by item 1
                //item 1 fb
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_fb = Tuple.Create(s_falseB, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_lt = Tuple.Create(s_falseB, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_rt = Tuple.Create(s_falseB, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_tb = Tuple.Create(s_falseB, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_ib = Tuple.Create(s_falseB, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_ilf = Tuple.Create(s_falseB, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_irf = Tuple.Create(s_falseB, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_ilt = Tuple.Create(s_falseB, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_irt = Tuple.Create(s_falseB, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_nb = Tuple.Create(s_falseB, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_nil = Tuple.Create(s_falseB, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_nir = Tuple.Create(s_falseB, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_nlf = Tuple.Create(s_falseB, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_nrf = Tuple.Create(s_falseB, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_nlt = Tuple.Create(s_falseB, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_fb_nrt = Tuple.Create(s_falseB, s_nonert);//base none done 16 item done
                //item 2 lt
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_fb = Tuple.Create(s_leftT, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_lt = Tuple.Create(s_leftT, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_rt = Tuple.Create(s_leftT, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_tb = Tuple.Create(s_leftT, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_ib = Tuple.Create(s_leftT, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_ilf = Tuple.Create(s_leftT, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_irf = Tuple.Create(s_leftT, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_ilt = Tuple.Create(s_leftT, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_irt = Tuple.Create(s_leftT, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_nb = Tuple.Create(s_leftT, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_nil = Tuple.Create(s_leftT, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_nir = Tuple.Create(s_leftT, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_nlf = Tuple.Create(s_leftT, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_nrf = Tuple.Create(s_leftT, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_nlt = Tuple.Create(s_leftT, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_lt_nrt = Tuple.Create(s_leftT, s_nonert);//base none done 16 item done
                //item 3 rt
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_fb = Tuple.Create(s_rightT, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_lt = Tuple.Create(s_rightT, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_rt = Tuple.Create(s_rightT, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_tb = Tuple.Create(s_rightT, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_ib = Tuple.Create(s_rightT, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_ilf = Tuple.Create(s_rightT, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_irf = Tuple.Create(s_rightT, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_ilt = Tuple.Create(s_rightT, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_irt = Tuple.Create(s_rightT, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_nb = Tuple.Create(s_rightT, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_nil = Tuple.Create(s_rightT, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_nir = Tuple.Create(s_rightT, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_nlf = Tuple.Create(s_rightT, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_nrf = Tuple.Create(s_rightT, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_nlt = Tuple.Create(s_rightT, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_rt_nrt = Tuple.Create(s_rightT, s_nonert);//base none done 16 item done
                //item 4 tb
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_fb = Tuple.Create(s_trueB, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_lt = Tuple.Create(s_trueB, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_rt = Tuple.Create(s_trueB, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_tb = Tuple.Create(s_trueB, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_ib = Tuple.Create(s_trueB, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_ilf = Tuple.Create(s_trueB, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_irf = Tuple.Create(s_trueB, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_ilt = Tuple.Create(s_trueB, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_irt = Tuple.Create(s_trueB, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_nb = Tuple.Create(s_trueB, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_nil = Tuple.Create(s_trueB, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_nir = Tuple.Create(s_trueB, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_nlf = Tuple.Create(s_trueB, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_nrf = Tuple.Create(s_trueB, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_nlt = Tuple.Create(s_trueB, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_tb_nrt = Tuple.Create(s_trueB, s_nonert);//base none done 16 item done
                //item 5 ib
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_fb = Tuple.Create(s_indB, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_lt = Tuple.Create(s_indB, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_rt = Tuple.Create(s_indB, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_tb = Tuple.Create(s_indB, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_ib = Tuple.Create(s_indB, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_ilf = Tuple.Create(s_indB, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_irf = Tuple.Create(s_indB, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_ilt = Tuple.Create(s_indB, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_irt = Tuple.Create(s_indB, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_nb = Tuple.Create(s_indB, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_nil = Tuple.Create(s_indB, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_nir = Tuple.Create(s_indB, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_nlf = Tuple.Create(s_indB, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_nrf = Tuple.Create(s_indB, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_nlt = Tuple.Create(s_indB, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ib_nrt = Tuple.Create(s_indB, s_nonert);//base none done 16 item done
                //item 6 ilf
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_fb = Tuple.Create(s_indlf, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_lt = Tuple.Create(s_indlf, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_rt = Tuple.Create(s_indlf, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_tb = Tuple.Create(s_indlf, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_ib = Tuple.Create(s_indlf, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_ilf = Tuple.Create(s_indlf, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_irf = Tuple.Create(s_indlf, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_ilt = Tuple.Create(s_indlf, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_irt = Tuple.Create(s_indlf, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_nb = Tuple.Create(s_indlf, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_nil = Tuple.Create(s_indlf, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_nir = Tuple.Create(s_indlf, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_nlf = Tuple.Create(s_indlf, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_nrf = Tuple.Create(s_indlf, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_nlt = Tuple.Create(s_indlf, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilf_nrt = Tuple.Create(s_indlf, s_nonert);//base none done 16 item done
                //item 7 irf
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_fb = Tuple.Create(s_indrf, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_lt = Tuple.Create(s_indrf, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_rt = Tuple.Create(s_indrf, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_tb = Tuple.Create(s_indrf, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_ib = Tuple.Create(s_indrf, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_ilf = Tuple.Create(s_indrf, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_irf = Tuple.Create(s_indrf, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_ilt = Tuple.Create(s_indrf, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_irt = Tuple.Create(s_indrf, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_nb = Tuple.Create(s_indrf, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_nil = Tuple.Create(s_indrf, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_nir = Tuple.Create(s_indrf, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_nlf = Tuple.Create(s_indrf, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_nrf = Tuple.Create(s_indrf, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_nlt = Tuple.Create(s_indrf, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irf_nrt = Tuple.Create(s_indrf, s_nonert);//base none done 16 item done
                //item 8 ilt
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_fb = Tuple.Create(s_indlt, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_lt = Tuple.Create(s_indlt, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_rt = Tuple.Create(s_indlt, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_tb = Tuple.Create(s_indlt, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_ib = Tuple.Create(s_indlt, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_ilf = Tuple.Create(s_indlt, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_irf = Tuple.Create(s_indlt, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_ilt = Tuple.Create(s_indlt, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_irt = Tuple.Create(s_indlt, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_nb = Tuple.Create(s_indlt, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_nil = Tuple.Create(s_indlt, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_nir = Tuple.Create(s_indlt, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_nlf = Tuple.Create(s_indlt, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_nrf = Tuple.Create(s_indlt, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_nlt = Tuple.Create(s_indlt, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_ilt_nrt = Tuple.Create(s_indlt, s_nonert);//base none done 16 item done
                //item 9 irt
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_fb = Tuple.Create(s_indrt, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_lt = Tuple.Create(s_indrt, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_rt = Tuple.Create(s_indrt, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_tb = Tuple.Create(s_indrt, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_ib = Tuple.Create(s_indrt, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_ilf = Tuple.Create(s_indrt, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_irf = Tuple.Create(s_indrt, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_ilt = Tuple.Create(s_indrt, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_irt = Tuple.Create(s_indrt, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_nb = Tuple.Create(s_indrt, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_nil = Tuple.Create(s_indrt, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_nir = Tuple.Create(s_indrt, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_nlf = Tuple.Create(s_indrt, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_nrf = Tuple.Create(s_indrt, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_nlt = Tuple.Create(s_indrt, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_irt_nrt = Tuple.Create(s_indrt, s_nonert);//base none done 16 item done
                //item 10 nb
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_fb = Tuple.Create(s_noneB, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_lt = Tuple.Create(s_noneB, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_rt = Tuple.Create(s_noneB, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_tb = Tuple.Create(s_noneB, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_ib = Tuple.Create(s_noneB, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_ilf = Tuple.Create(s_noneB, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_irf = Tuple.Create(s_noneB, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_ilt = Tuple.Create(s_noneB, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_irt = Tuple.Create(s_noneB, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_nb = Tuple.Create(s_noneB, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_nil = Tuple.Create(s_noneB, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_nir = Tuple.Create(s_noneB, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_nlf = Tuple.Create(s_noneB, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_nrf = Tuple.Create(s_noneB, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_nlt = Tuple.Create(s_noneB, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nb_nrt = Tuple.Create(s_noneB, s_nonert);//base none done 16 item done
                //item 11 nonelind
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_fb = Tuple.Create(s_nonelind, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_lt = Tuple.Create(s_nonelind, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_rt = Tuple.Create(s_nonelind, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_tb = Tuple.Create(s_nonelind, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_ib = Tuple.Create(s_nonelind, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_ilf = Tuple.Create(s_nonelind, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_irf = Tuple.Create(s_nonelind, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_ilt = Tuple.Create(s_nonelind, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_irt = Tuple.Create(s_nonelind, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_nb = Tuple.Create(s_nonelind, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_nil = Tuple.Create(s_nonelind, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_nir = Tuple.Create(s_nonelind, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_nlf = Tuple.Create(s_nonelind, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_nrf = Tuple.Create(s_nonelind, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_nlt = Tuple.Create(s_nonelind, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nil_nrt = Tuple.Create(s_nonelind, s_nonert);//base none done 16 item done
                //item 12 nonerind
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_fb = Tuple.Create(s_nonerind, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_lt = Tuple.Create(s_nonerind, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_rt = Tuple.Create(s_nonerind, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_tb = Tuple.Create(s_nonerind, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_ib = Tuple.Create(s_nonerind, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_ilf = Tuple.Create(s_nonerind, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_irf = Tuple.Create(s_nonerind, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_ilt = Tuple.Create(s_nonerind, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_irt = Tuple.Create(s_nonerind, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_nb = Tuple.Create(s_nonerind, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_nil = Tuple.Create(s_nonerind, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_nir = Tuple.Create(s_nonerind, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_nlf = Tuple.Create(s_nonerind, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_nrf = Tuple.Create(s_nonerind, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_nlt = Tuple.Create(s_nonerind, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nir_nrt = Tuple.Create(s_nonerind, s_nonert);//base none done 16 item done
                //item 13 nonelf
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_fb = Tuple.Create(s_nonelf, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_lt = Tuple.Create(s_nonelf, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_rt = Tuple.Create(s_nonelf, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_tb = Tuple.Create(s_nonelf, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_ib = Tuple.Create(s_nonelf, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_ilf = Tuple.Create(s_nonelf, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_irf = Tuple.Create(s_nonelf, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_ilt = Tuple.Create(s_nonelf, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_irt = Tuple.Create(s_nonelf, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_nb = Tuple.Create(s_nonelf, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_nil = Tuple.Create(s_nonelf, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_nir = Tuple.Create(s_nonelf, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_nlf = Tuple.Create(s_nonelf, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_nrf = Tuple.Create(s_nonelf, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_nlt = Tuple.Create(s_nonelf, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlf_nrt = Tuple.Create(s_nonelf, s_nonert);//base none done 16 item done
                //item 14 nonerf
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_fb = Tuple.Create(s_nonerf, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_lt = Tuple.Create(s_nonerf, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_rt = Tuple.Create(s_nonerf, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_tb = Tuple.Create(s_nonerf, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_ib = Tuple.Create(s_nonerf, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_ilf = Tuple.Create(s_nonerf, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_irf = Tuple.Create(s_nonerf, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_ilt = Tuple.Create(s_nonerf, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_irt = Tuple.Create(s_nonerf, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_nb = Tuple.Create(s_nonerf, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_nil = Tuple.Create(s_nonerf, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_nir = Tuple.Create(s_nonerf, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_nlf = Tuple.Create(s_nonerf, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_nrf = Tuple.Create(s_nonerf, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_nlt = Tuple.Create(s_nonerf, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrf_nrt = Tuple.Create(s_nonerf, s_nonert);//base none done 16 item done
                //item 15 nonelt
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_fb = Tuple.Create(s_nonelt, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_lt = Tuple.Create(s_nonelt, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_rt = Tuple.Create(s_nonelt, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_tb = Tuple.Create(s_nonelt, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_ib = Tuple.Create(s_nonelt, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_ilf = Tuple.Create(s_nonelt, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_irf = Tuple.Create(s_nonelt, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_ilt = Tuple.Create(s_nonelt, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_irt = Tuple.Create(s_nonelt, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_nb = Tuple.Create(s_nonelt, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_nil = Tuple.Create(s_nonelt, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_nir = Tuple.Create(s_nonelt, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_nlf = Tuple.Create(s_nonelt, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_nrf = Tuple.Create(s_nonelt, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_nlt = Tuple.Create(s_nonelt, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nlt_nrt = Tuple.Create(s_nonelt, s_nonert);//base none done 16 item done
                //item 16 nonert
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_fb = Tuple.Create(s_nonert, s_falseB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_lt = Tuple.Create(s_nonert, s_leftT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_rt = Tuple.Create(s_nonert, s_rightT);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_tb = Tuple.Create(s_nonert, s_trueB);//base tf done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_ib = Tuple.Create(s_nonert, s_indB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_ilf = Tuple.Create(s_nonert, s_indlf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_irf = Tuple.Create(s_nonert, s_indrf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_ilt = Tuple.Create(s_nonert, s_indlt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_irt = Tuple.Create(s_nonert, s_indrt);//base ind done
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_nb = Tuple.Create(s_nonert, s_noneB);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_nil = Tuple.Create(s_nonert, s_nonelind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_nir = Tuple.Create(s_nonert, s_nonerind);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_nlf = Tuple.Create(s_nonert, s_nonelf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_nrf = Tuple.Create(s_nonert, s_nonerf);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_nlt = Tuple.Create(s_nonert, s_nonelt);
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>> s_m_nrt_nrt = Tuple.Create(s_nonert, s_nonert);//base none done 16 item done
                //finished 16x16 items 256 items
                //set valid pairs for true, false, and indeterminate 16 different pairs total for each logic
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
                private static readonly Tuple<logicStates, logicStates>[] s_AndTrues = { s_trueB, s_nonelt, s_nonert  };//3
                private static readonly Tuple<logicStates, logicStates>[] s_AndFalses = { s_falseB, s_rightT, s_leftT, s_indrf, s_indlf, s_noneB, s_nonelf, s_nonerf };//11
                private static readonly Tuple<logicStates, logicStates>[] s_AndInds = { s_nonerind, s_nonelind, s_indB, s_indrt, s_indlt };//16
                private static readonly Tuple<logicStates, logicStates>[] s_NotTrues = { s_falseB, s_rightT, s_leftT, s_indrf, s_indlf, s_noneB, s_nonelf, s_nonerf };//8
                private static readonly Tuple<logicStates, logicStates>[] s_NotFalses = { s_trueB, s_nonelt, s_nonert };//3
                private static readonly Tuple<logicStates, logicStates>[] s_NotInds = { s_nonerind, s_nonelind, s_indB, s_indrt, s_indlt };//5.  not full        
                private static readonly Tuple<logicStates, logicStates>[] s_NoneTrues = { s_falseB, s_trueB, s_leftT, s_rightT, s_indrt, s_indrf, s_indlt, s_indlf, s_indB, s_noneB, s_nonelt, s_nonelf, s_nonelind, s_nonert, s_nonerf, s_nonerind};//16, none full
                private static readonly Tuple<logicStates, logicStates>[] s_NoneFalses = { };//0
                private static readonly Tuple<logicStates, logicStates>[] s_NoneInds = { };//0
                //set pairs of pairs for the method resolution
                //xor
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_XorActivateTrues = {};//blank, is already true
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_XorActivateFalses =
                {
                    s_m_tb_fb, s_m_tb_rt, s_m_tb_ib, s_m_tb_ilf, s_m_tb_irf, s_m_tb_irt, s_m_tb_nb, s_m_tb_nil, s_m_tb_nir, s_m_tb_nlf, s_m_tb_nrf, s_m_tb_nrt,
                    s_m_rt_rt, s_m_rt_fb, s_m_rt_ib, s_m_rt_ilf, s_m_rt_irf, s_m_rt_irt, s_m_rt_nb, s_m_rt_nil, s_m_rt_nir, s_m_rt_nlf, s_m_rt_nrf, s_m_rt_nrt, s_m_rt_lt, s_m_rt_ilt, s_m_rt_nlt, s_m_rt_tb, 
                    s_m_ib_ib, s_m_ib_rt, s_m_ib_fb, s_m_ib_ilf, s_m_ib_irf, s_m_ib_irt, s_m_ib_nb, s_m_ib_nil, s_m_ib_nir, s_m_rt_nlf, s_m_ib_nrf, s_m_ib_nrt, s_m_ib_lt, s_m_ib_ilt, s_m_ib_nlt, s_m_ib_tb, 
                    s_m_ilf_ilf, s_m_ilf_ib, s_m_ilf_rt, s_m_ilf_fb, s_m_ilf_irf, s_m_ilf_irt, s_m_ilf_nb, s_m_ilf_nil, s_m_ilf_nir, s_m_ilf_nlf, s_m_ilf_nrf, s_m_ilf_nrt, s_m_ilf_lt, s_m_ilf_ilt, s_m_ilf_nlt, s_m_ilf_tb, 
                    s_m_irf_irf, s_m_irf_ilf, s_m_irf_ib, s_m_irf_rt, s_m_irf_fb, s_m_irf_irt, s_m_irf_nb, s_m_irf_nil, s_m_irf_nir, s_m_irf_nlf, s_m_irf_nrf, s_m_irf_nrt, s_m_irf_lt, s_m_irf_ilt, s_m_irf_nlt, s_m_irf_tb, 
                    s_m_irt_irt, s_m_irt_irf, s_m_irt_ilf, s_m_irt_ib, s_m_irt_rt, s_m_irt_fb, s_m_irt_nb, s_m_irt_nil, s_m_irt_nir, s_m_irt_nlf, s_m_irt_nrf, s_m_irt_nrt, s_m_irt_lt, s_m_irt_ilt, s_m_irf_nlt, s_m_irt_tb, 
                    s_m_nb_nb, s_m_nb_irt, s_m_nb_irf, s_m_nb_ilf, s_m_nb_ib, s_m_nb_rt, s_m_nb_fb, s_m_nb_nil, s_m_nb_nir, s_m_nb_nlf, s_m_nb_nrf, s_m_nb_nrt, s_m_nb_lt, s_m_nb_ilt, s_m_nb_nlt, s_m_nb_tb, 
                    s_m_nil_nil, s_m_nil_nb, s_m_nil_irt, s_m_nil_irf, s_m_nil_ilf, s_m_nil_ib, s_m_nil_rt, s_m_nil_fb, s_m_nil_nir, s_m_nil_nlf, s_m_nil_nrf, s_m_nil_nrt, s_m_nil_lt, s_m_nil_ilt, s_m_nil_nlt, s_m_nil_tb, 
                    s_m_nir_nir, s_m_nir_nil, s_m_nir_nb, s_m_nir_irt, s_m_nir_irf, s_m_nir_ilf, s_m_nir_ib, s_m_nir_rt, s_m_nir_fb, s_m_nir_nlf, s_m_nir_nrf, s_m_nir_nrt, s_m_nir_lt, s_m_nir_ilt, s_m_nir_nlt, s_m_nir_tb,
                    s_m_nlf_nlf, s_m_nlf_nir, s_m_nlf_nil, s_m_nlf_nb, s_m_nlf_irt, s_m_nlf_irf, s_m_nlf_ilf, s_m_nlf_ib, s_m_nlf_rt, s_m_nlf_fb, s_m_nlf_nrf, s_m_nlf_nrt, s_m_nlf_lt, s_m_nlf_ilt, s_m_nlf_nlt, s_m_nlf_tb, 
                    s_m_nrf_nrf, s_m_nrf_nlf, s_m_nrf_nir, s_m_nrf_nil, s_m_nrf_nb, s_m_nrf_irt, s_m_nrf_irf, s_m_nrf_ilf, s_m_nrf_ib, s_m_nrf_rt, s_m_nrf_fb, s_m_nrf_nrt, s_m_nrf_lt, s_m_nrf_ilt, s_m_nrf_nlt, s_m_nrf_tb, 
                    s_m_nrt_nrt, s_m_nrt_nrf, s_m_nrt_nlf, s_m_nrt_nir, s_m_nrt_nil, s_m_nrt_nb, s_m_nrt_irt, s_m_nrt_irf, s_m_nrt_ilf, s_m_nrt_ib, s_m_nrt_rt, s_m_nrt_fb, s_m_nrt_lt, s_m_nrt_ilt, s_m_nrt_nlt, s_m_nrt_tb, 
                    s_m_lt_fb, s_m_lt_rt, s_m_lt_ib, s_m_lt_ilf, s_m_lt_irf, s_m_lt_irt, s_m_lt_nb, s_m_lt_nil, s_m_lt_nir, s_m_lt_nlf, s_m_lt_nrf, s_m_lt_nrt,
                    s_m_ilt_fb, s_m_ilt_rt, s_m_ilt_ib, s_m_ilt_ilf, s_m_ilt_irf, s_m_ilt_irt, s_m_ilt_nb, s_m_ilt_nil, s_m_ilt_nir, s_m_ilt_nlf, s_m_ilt_nrf, s_m_ilt_nrt,
                    s_m_nlt_fb, s_m_nlt_rt, s_m_nlt_ib, s_m_nlt_ilf, s_m_nlt_irf, s_m_nlt_irt, s_m_nlt_nb, s_m_nlt_nil, s_m_nlt_nir, s_m_nlt_nlf, s_m_nlt_nrf, s_m_nlt_nrt
                };
                //things above, where both left evaluates to trues not permitted all else permitted.  total
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_XorActivateInds =
                {
                    s_m_tb_fb, s_m_tb_rt, s_m_tb_ilf, s_m_tb_nb, s_m_tb_nir, s_m_tb_nlf, s_m_tb_nrf, s_m_tb_nrt, 
                    s_m_fb_fb, s_m_fb_tb, s_m_fb_rt, s_m_fb_lt, s_m_fb_ib, s_m_fb_ilf, s_m_fb_ilt, s_m_fb_irf, s_m_fb_irt, s_m_fb_nb, s_m_fb_nil, s_m_fb_nir, s_m_fb_nlf, s_m_fb_nlt, s_m_fb_nrf, s_m_fb_nrt, 
                    s_m_rt_rt, s_m_rt_tb, s_m_rt_fb, s_m_rt_lt, s_m_rt_ib, s_m_rt_ilf, s_m_rt_ilt, s_m_rt_irf, s_m_rt_irt, s_m_rt_nb, s_m_rt_nil, s_m_rt_nir, s_m_rt_nlf, s_m_rt_nlt, s_m_rt_nrf, s_m_rt_nrt, 
                    s_m_ilf_ilf, s_m_ilf_tb, s_m_ilf_rt, s_m_ilf_fb, s_m_ilf_lt, s_m_ilf_ib, s_m_ilf_ilt, s_m_ilf_irf, s_m_ilf_irt, s_m_ilf_nb, s_m_ilf_nil, s_m_ilf_nir, s_m_ilf_nlf, s_m_ilf_nlt, s_m_ilf_nrf, s_m_ilf_nrt, 
                    s_m_nb_nb, s_m_nb_tb, s_m_nb_fb, s_m_nb_rt, s_m_nb_lt, s_m_nb_ib, s_m_nb_ilf, s_m_nb_ilt, s_m_nb_irf, s_m_nb_irt, s_m_nb_nil, s_m_nb_nir, s_m_nb_nlf, s_m_nb_nlt, s_m_nb_nrt, s_m_nb_nrt, 
                    s_m_nir_nir, s_m_nir_tb, s_m_nir_rt, s_m_nir_fb, s_m_nir_lt, s_m_nir_ib, s_m_nir_ilt, s_m_nir_irf, s_m_nir_irt, s_m_nir_nb, s_m_nir_nil, s_m_nir_ilf, s_m_nir_nlf, s_m_nir_nlt, s_m_nir_nrf, s_m_nir_nrt, 
                    s_m_nlf_nlf, s_m_nlf_tb, s_m_nlf_rt, s_m_nlf_fb, s_m_nlf_lt, s_m_nlf_ib, s_m_nlf_ilt, s_m_nlf_irf, s_m_nlf_irt, s_m_nlf_nb, s_m_nlf_nil, s_m_nlf_ilf, s_m_nlf_nir, s_m_nlf_nlt, s_m_nlf_nrf, s_m_nlf_nrt, 
                    s_m_nrf_nrf, s_m_nrf_tb, s_m_nrf_rt, s_m_nrf_fb, s_m_nrf_lt, s_m_nrf_ib, s_m_nrf_ilt, s_m_nrf_irf, s_m_nrf_irt, s_m_nrf_nb, s_m_nrf_nil, s_m_nrf_ilf, s_m_nrf_nir, s_m_nrf_nlt, s_m_nrf_nlf, s_m_nrf_nrt, 
                    s_m_nrt_nrt, s_m_nrt_tb, s_m_nrt_rt, s_m_nrt_fb, s_m_nrt_lt, s_m_nrt_ib, s_m_nrt_ilt, s_m_nrt_irf, s_m_nrt_irt, s_m_nrt_nb, s_m_nrt_nil, s_m_nrt_ilf, s_m_nrt_nir, s_m_nrt_nlt, s_m_nrt_nlf, s_m_nrt_nrf, 
                    s_m_lt_fb, s_m_lt_rt, s_m_lt_ilf, s_m_lt_nb, s_m_lt_nir, s_m_lt_nlf, s_m_lt_nrf, s_m_lt_nrt, 
                    s_m_ilt_fb, s_m_ilt_rt, s_m_ilt_ilf, s_m_ilt_nb, s_m_ilt_nir, s_m_ilt_nlf, s_m_ilt_nrf, s_m_ilt_nrt, 
                    s_m_nlt_fb, s_m_nlt_rt, s_m_nlt_ilf, s_m_nlt_nb, s_m_nlt_nir, s_m_nlt_nlf, s_m_nlt_nrf, s_m_nlt_nrt
                };
                //tb, lt, ilt, nlt, 
                //things above, where both left evaluates to trues not permitted, must be able to achieve inds by an activation
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_XorDeactivateTrues =
                {
                    s_m_tb_tb, s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt, 
                    s_m_lt_lt, s_m_lt_tb, s_m_lt_ilt, s_m_lt_nlt, 
                    s_m_ilt_ilt, s_m_ilt_tb, s_m_ilt_lt, s_m_ilt_nlt, 
                    s_m_nlt_nlt, s_m_nlt_tb, s_m_nlt_lt, s_m_nlt_ilt, 
                    s_m_tb_ib, s_m_tb_irf, s_m_tb_irt, s_m_tb_nb, s_m_tb_nrf, s_m_tb_nrt, 
                    s_m_lt_ib, s_m_lt_irf, s_m_lt_irt, s_m_lt_nb, s_m_lt_nrf, s_m_lt_nrt, 
                    s_m_ilt_ib, s_m_ilt_irf, s_m_ilt_irt, s_m_ilt_nb, s_m_ilt_nrf, s_m_ilt_nrt, 
                    s_m_nlt_ib, s_m_nlt_irf, s_m_nlt_irt, s_m_nlt_nb, s_m_nlt_nrf, s_m_nlt_nrt, 
                    s_m_ib_ib, s_m_ib_tb, s_m_ib_lt, s_m_ib_ilt, s_m_ib_nlt, s_m_ib_irf, s_m_ib_irt, s_m_ib_nb, s_m_ib_nrf, s_m_ib_nrt, 
                    s_m_irf_irf, s_m_irf_tb, s_m_irf_lt, s_m_irf_ilt, s_m_irf_nlt, s_m_irf_ib, s_m_irf_irt, s_m_irt_nb, s_m_irt_nrf, s_m_irt_nrt, 
                    s_m_irt_irt, s_m_irt_tb, s_m_irt_lt, s_m_irt_ilt, s_m_irt_nlt, s_m_irt_ib, s_m_irt_irf, s_m_irt_nb, s_m_irt_nrf, s_m_irt_nrt, 
                    s_m_nb_nb, s_m_nb_tb, s_m_nb_lt, s_m_nb_ilt, s_m_nb_nlt, s_m_nb_ib, s_m_nb_irf, s_m_nb_irt, s_m_nb_nrf, s_m_nb_nrt, 
                    s_m_nrf_nrf, s_m_nrf_tb, s_m_nrf_lt, s_m_nrf_ilt, s_m_nrf_nlt, s_m_nrf_ib, s_m_nrf_irt, s_m_nrf_nb, s_m_nrf_irt, s_m_nrf_nrt, 
                    s_m_nrt_nrt, s_m_nrt_tb, s_m_nrt_lt, s_m_nrt_ilt, s_m_nrt_nlt, s_m_nrt_ib, s_m_nrt_irt, s_m_nrt_nb, s_m_nrt_irt, s_m_nrt_nrf
                };
                //things above, only where both lefts evaluate to true, or anything that is indeterminate are allowed, so false and indeterminate
                //false line 1 to 4
                //inds level 1 lines 5 to 8
                //inds level 2 lines 9 to 14
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_XorDeactivateFalses = {};//blank
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_XorDeactivateInds =
                {
                    s_m_tb_tb, s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt, 
                    s_m_lt_lt, s_m_lt_tb, s_m_lt_ilt, s_m_lt_nlt, 
                    s_m_ilt_ilt, s_m_ilt_tb, s_m_ilt_lt, s_m_ilt_nlt, 
                    s_m_nlt_nlt, s_m_nlt_tb, s_m_nlt_lt, s_m_nlt_ilt, 
                };
                //things above.  only double left trues to evaluate to false allowed
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_XorFollowTrues =
                {
                    s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt, 
                    s_m_lt_tb, s_m_ilt_tb, s_m_nlt_tb,
                    s_m_lt_ilt, s_m_lt_nlt, s_m_ilt_lt, s_m_nlt_lt, 
                    s_m_ib_lt, s_m_lt_ib, s_m_ib_ilt, s_m_ilt_ib, s_m_ib_nlt, s_m_nlt_ib, 
                    s_m_ilt_nlt, s_m_nlt_ilt, 
                    s_m_fb_irf, s_m_fb_nil, s_m_irf_fb, s_m_nil_fb, 
                    s_m_irf_lt, s_m_lt_irf, s_m_nil_lt, s_m_lt_nil, 
                };
                //things above: must not be true must be ind or false.  in false one must be evaluating to true and previously ind/false and the other present and past true or past ind if the first was false to true
                //in ind one must have evaluated to false before ind and the other must be one order of action lower that this (false to false if false to ind, false to ind or ind to ind if ind to true or false to true
                //none is treated as further away from true than indeterminate
                //false level 1 line 1 and 2 (f to t or ind to t for the victor, tb for the other)
                //false level 2 line 3 (f to t for the victor, ind to t for other)
                //ind level 1 line 4 (both ind for the other, ind/false to true for the victor)
                //ind level 2 line 5 (difference between not to true and indeterminate to true)
                //ind level 3 line 6 (false to false for other, false or none to ind for victor)
                //ind level 4 line 7 (false to true for victor, false or none to ind for other)
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_XorFollowFalses = {};//blank
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_XorFollowInds =
                {
                    s_m_fb_irf, s_m_irf_fb, s_m_fb_nil, s_m_nil_fb
                };
                //things above must go from true to true, this transforms into indeterminate.  f to f and f to ind, f to f and none to ind, all other conditions already lead to ind
                //left
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_LeftActivateTrues =
                {
                    s_m_rt_nlt, s_m_nlf_nlt, s_m_ilf_nlt, s_m_fb_nlt, s_m_ib_nlt, s_m_irt_nlt, s_m_irf_nlt, s_m_nil_nlt, 
                    s_m_rt_ilt, s_m_nlf_ilt, s_m_ilf_ilt, s_m_fb_ilt, s_m_ib_ilt, s_m_irt_ilt, s_m_irf_ilt, s_m_nil_ilt, 
                    s_m_rt_lt, s_m_nlf_lt, s_m_ilf_lt, s_m_fb_lt, s_m_ib_lt, s_m_irt_lt, s_m_irf_lt, s_m_nil_lt, 
                    s_m_rt_tb, s_m_nlf_tb, s_m_ilf_tb, s_m_fb_tb, s_m_ib_tb, s_m_irt_tb, s_m_irf_tb, s_m_nil_tb, 
                    s_m_ib_ib, s_m_irt_ib, s_m_irf_ib, s_m_nil_ib, 
                    s_m_ib_irt, s_m_irt_irt, s_m_irf_irt, s_m_nil_irt, 
                    s_m_ib_irf, s_m_irf_irf, s_m_irt_irf, s_m_nil_irf, 
                    s_m_nil_nil, s_m_ib_nil,s_m_irt_nil, s_m_irf_nil
                };
                //things here, as long as it does not evaluate to true you can use it
                //falses lines 1 through 4
                //inds lines 5 both evaluate to ind
                //inds lines 6 both evaluate to ind, finished with the nones
                //inds lines 7 to 10 right true left ind
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_LeftActivateFalses = {};//blank
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_LeftActivateInds =
                {
                    s_m_fb_fb, s_m_rt_fb, s_m_irt_fb, s_m_irf_fb, s_m_ib_fb, s_m_nlf_fb, s_m_nil_fb, s_m_ilf_fb, 
                    s_m_nlf_lt, s_m_ilf_lt, s_m_fb_lt, s_m_rt_lt, 
                    s_m_nlf_irt, s_m_ilf_irt, s_m_fb_ilt, s_m_rt_ilt, 
                    s_m_nlf_nlf, s_m_ilf_nlf, s_m_fb_nlf, s_m_rt_nlf, s_m_irf_nlf, s_m_irt_nlf, 
                    s_m_nlf_rt, s_m_rt_rt, 
                    s_m_irt_ilf, s_m_irf_ilf, 
                    s_m_fb_rt, s_m_irf_rt
                };
                //things above must go from true to true or false to true, must get left to ind and right to ind or true with activation
                //true to true lines 1-4
                //false to true lines 5-7
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_LeftDeactivateTrues =
                {
                    s_m_fb_tb, s_m_nlf_tb, s_m_ilf_tb, s_m_ib_tb, s_m_nil_tb, 
                    s_m_fb_lt, s_m_nlf_lt, s_m_ilf_lt, s_m_ib_lt, s_m_nil_lt, 
                    s_m_fb_ilt, s_m_nlf_ilt, s_m_ilf_ilt, s_m_ib_ilt, s_m_nil_ilt, 
                    s_m_fb_nlt, s_m_nlf_nlt, s_m_ilf_nlt, s_m_ib_nlt, s_m_nil_nlt, 

                    s_m_ib_tb, s_m_irf_tb, s_m_irt_tb, s_m_nil_tb, 
                    s_m_ib_lt, s_m_irf_lt, s_m_irt_lt, s_m_nil_lt, 
                    s_m_ib_ilt, s_m_irf_ilt, s_m_irt_ilt, s_m_nil_ilt, 
                    s_m_ib_nlt, s_m_irf_nlt, s_m_irt_nlt, s_m_nil_nlt, 

                    s_m_ib_ib, s_m_irf_ib, s_m_irt_ib, s_m_nil_ib, 
                    s_m_irf_irf, s_m_ib_irf, s_m_irt_irf, s_m_nil_irf, 
                    s_m_irt_irt, s_m_ib_irt, s_m_irf_irt, s_m_nil_irt, 
                    s_m_nil_nil, s_m_ib_nil, s_m_irf_nil, s_m_irt_nil
                };
                //things above must go from ind to ind, false to false, ind to false
                //false lines 1 through 4
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_LeftDeactivateFalses = {};//blank
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_LeftDeactivateInds =
                {
                    s_m_tb_tb, s_m_lt_tb, s_m_ilt_tb, s_m_nlt_tb, 
                    s_m_lt_lt, s_m_tb_lt, s_m_ilt_lt, s_m_nlt_lt, 
                    s_m_ilt_ilt, s_m_tb_ilt, s_m_lt_ilt, s_m_nlt_ilt, 
                    s_m_nlt_nlt, s_m_tb_nlt, s_m_lt_nlt, s_m_ilt_nlt, 
                    
                    s_m_tb_ib, s_m_lt_ib, s_m_ilt_ib, s_m_nlt_ib,
                    s_m_tb_irt, s_m_lt_irt, s_m_ilt_irt, s_m_nlt_irt, 
                    s_m_tb_irf, s_m_lt_irf, s_m_ilt_irf, s_m_nlt_irf, 
                    s_m_tb_nil, s_m_lt_nil, s_m_ilt_nil, s_m_nlt_nil, 

                    s_m_ib_ib, s_m_irt_ib, s_m_irf_ib, s_m_nil_ib, 
                    s_m_irt_irt, s_m_ib_irt, s_m_irf_irt, s_m_nil_irt, 
                    s_m_irf_irf, s_m_ib_irf, s_m_irt_irf, s_m_nil_irf, 
                    s_m_nil_nil, s_m_ib_nil, s_m_irt_nil, s_m_irf_nil
                };
                //things above must be true (true/true) or false (false/true)
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_LeftFollowTrues =
                {
                    s_m_irf_tb, s_m_ilf_tb, s_m_nil_tb, s_m_rt_tb, s_m_irt_tb, 
                    s_m_irf_ib, s_m_ilf_ib, s_m_nil_ib, s_m_rt_ib, s_m_irt_ib, 
                    s_m_irf_irt, s_m_ilf_irt, s_m_nil_irt, s_m_rt_irt, s_m_irt_irt, 
                    s_m_irf_irf, s_m_irt_irf, s_m_ilf_irf, s_m_nil_irf, s_m_rt_irf, 
                    s_m_irf_nil, s_m_irt_nil, s_m_nil_nil, s_m_rt_nil, s_m_ilf_nil, 

                    s_m_fb_irt, s_m_fb_irf, s_m_fb_lt, s_m_fb_ilt, s_m_fb_nil, s_m_fb_nlt, 
                    s_m_ib_irt, s_m_ib_irf, s_m_ib_lt, s_m_ib_ilt, s_m_ib_nil, s_m_ib_nlt

                };
                //things above.  something must have changed.  must start at false or ind.  must be going towards trues
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_LeftFollowFalses =
                {
                    s_m_irt_tb, s_m_irf_tb, s_m_nil_tb, 
                    s_m_irt_lt, s_m_irf_lt, s_m_nil_lt, 
                    s_m_irt_ilt, s_m_irf_ilt, s_m_nil_ilt, 
                    s_m_irt_irf, s_m_irf_irf, s_m_nil_irf, 
                    s_m_irt_nlt, s_m_irf_nlt, s_m_nil_nlt, 
                    
                    s_m_nlf_irf, s_m_nlf_irt, s_m_nlf_nil
                };
                //things above cannot be false, something must have changed.  must be going towards false
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_LeftFollowInds =
                {
                    s_m_tb_irt, s_m_tb_irf, s_m_tb_nil, s_m_tb_nlt, s_m_tb_nlf, 
                    s_m_ilt_irt, s_m_ilt_irf, s_m_ilt_nil, s_m_ilt_nlt, s_m_ilt_nlf,
                    s_m_nlt_irt, s_m_nlt_irf, s_m_nlt_nil, s_m_nlt_nlt, s_m_nlt_nlf, 
                    
                    s_m_irt_fb, s_m_irf_fb, s_m_nil_fb, s_m_nlf_fb, s_m_nlt_fb, 
                    s_m_irt_ilf, s_m_irf_ilf, s_m_nil_ilf, s_m_nlf_ilf, s_m_nlt_ilf, 
                    s_m_irt_nlf, s_m_irf_nlf, s_m_nil_nlf, s_m_nlf_nlf, s_m_nlt_nlf
                };
                //things above can start false or true but not ind
                //right
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_RightActivateTrues =
                {
                    s_m_nlt_rt, s_m_nlt_nlf, s_m_nlt_ilf, s_m_nlt_fb, s_m_nlt_ib, s_m_nlt_irt, s_m_nlt_irf, s_m_nlt_nil, 
                    s_m_ilt_rt, s_m_ilt_nlf, s_m_ilt_ilf, s_m_ilt_fb, s_m_ilt_ib, s_m_ilt_irt, s_m_ilt_irf, s_m_ilt_nil, 
                    s_m_lt_rt, s_m_lt_nlf, s_m_lt_ilf, s_m_lt_fb, s_m_lt_ib, s_m_lt_irt, s_m_lt_irf, s_m_lt_nil, 
                    s_m_tb_rt, s_m_tb_nlf, s_m_tb_ilf, s_m_tb_fb, s_m_tb_ib, s_m_tb_irt, s_m_tb_irf, s_m_tb_nil, 
                    s_m_ib_ib, s_m_ib_irt, s_m_ib_irf, s_m_ib_nil, 
                    s_m_irt_ib, s_m_irt_irt, s_m_irt_irf, s_m_irt_nil, 
                    s_m_irf_ib, s_m_irf_irf, s_m_irf_irt, s_m_irf_nil, 
                    s_m_nil_nil, s_m_nil_ib, s_m_nil_irt, s_m_nil_irf
                };
                //things above copy it and reverse
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_RightActivateFalses = {};//blank
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_RightActivateInds =
                {
                    s_m_fb_fb, s_m_fb_rt, s_m_fb_irt, s_m_fb_irf, s_m_fb_ib, s_m_fb_nlf, s_m_fb_nil, s_m_fb_ilf, 
                    s_m_lt_nlf, s_m_lt_ilf, s_m_lt_fb, s_m_lt_rt, 
                    s_m_irt_nlf, s_m_irt_ilf, s_m_ilt_fb, s_m_ilt_rt, 
                    s_m_nlf_nlf, s_m_nlf_ilf, s_m_nlf_fb, s_m_nlf_rt, s_m_nlf_irf, s_m_nlf_irt, 
                    s_m_rt_nlt, s_m_rt_rt, 
                    s_m_ilf_irt, s_m_ilf_irf, 
                    s_m_rt_fb, s_m_rt_irf
                };
                //things above copy it and reverse
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_RightDeactivateTrues =
                {
                    s_m_tb_fb, s_m_tb_nlf, s_m_tb_ilf, s_m_tb_ib, s_m_tb_nil, 
                    s_m_lt_fb, s_m_lt_nlf, s_m_lt_ilf, s_m_lt_ib, s_m_lt_nil, 
                    s_m_ilt_fb, s_m_ilt_nlf, s_m_ilt_ilf, s_m_ilt_ib, s_m_ilt_nil, 
                    s_m_nlt_fb, s_m_nlt_nlf, s_m_nlt_ilf, s_m_nlt_ib, s_m_nlt_nil, 

                    s_m_tb_ib, s_m_tb_irf, s_m_tb_irt, s_m_tb_nil, 
                    s_m_lt_ib, s_m_lt_irf, s_m_lt_irt, s_m_lt_nil, 
                    s_m_ilt_ib, s_m_ilt_irt, s_m_ilt_irt, s_m_ilt_nil, 
                    s_m_nlt_ib, s_m_nlt_irf, s_m_nlt_irt, s_m_nlt_nil, 

                    s_m_ib_ib, s_m_ib_irf, s_m_ib_irt, s_m_ib_nil, 
                    s_m_irf_irf, s_m_irf_ib, s_m_irf_irt, s_m_irf_nil, 
                    s_m_irt_irt, s_m_irt_ib, s_m_irt_irf, s_m_irt_nil, 
                    s_m_nil_nil, s_m_nil_ib, s_m_nil_irf, s_m_nil_irt
                };
                //things above copy it and reverse
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_RightDeactivateFalses = {};//blank
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_RightDeactivateInds =
                {
                    s_m_tb_tb, s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt, 
                    s_m_lt_lt, s_m_lt_tb, s_m_lt_ilt, s_m_lt_nlt, 
                    s_m_ilt_ilt, s_m_ilt_tb, s_m_ilt_lt, s_m_ilt_nlt, 
                    s_m_nlt_nlt, s_m_nlt_tb, s_m_nlt_lt, s_m_nlt_ilt, 
                    
                    s_m_ib_tb, s_m_ib_lt, s_m_ib_ilt, s_m_ib_nlt,
                    s_m_irt_tb, s_m_irt_lt, s_m_irt_ilt, s_m_irt_nlt, 
                    s_m_irf_tb, s_m_irf_lt, s_m_irf_ilt, s_m_irf_nlt, 
                    s_m_nil_tb, s_m_nil_lt, s_m_nil_ilt, s_m_nil_nlt, 

                    s_m_ib_ib, s_m_ib_irt, s_m_ib_irf, s_m_ib_nil, 
                    s_m_irt_irt, s_m_irt_ib, s_m_irt_irf, s_m_irt_nil, 
                    s_m_irf_irf, s_m_irf_ib, s_m_irf_irt, s_m_irf_nil, 
                    s_m_nil_nil, s_m_nil_ib, s_m_nil_irt, s_m_nil_irf
                };
                //things above copy it and reverse
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_RightFollowTrues =
                {
                    s_m_tb_irf, s_m_tb_ilf, s_m_tb_nil, s_m_tb_rt, s_m_tb_irt, 
                    s_m_ib_irf, s_m_ib_ilf, s_m_ib_nil, s_m_ib_rt, s_m_ib_irt, 
                    s_m_irt_irf, s_m_irt_irf, s_m_irt_nil, s_m_irt_rt, s_m_irt_irt, 
                    s_m_irf_irf, s_m_irf_irt, s_m_irf_ilf, s_m_irf_nil, s_m_irf_rt, 
                    s_m_nil_irf, s_m_nil_irt, s_m_nil_nil, s_m_nil_rt, s_m_nil_ilf, 

                    s_m_irt_fb, s_m_irf_fb, s_m_lt_fb, s_m_ilt_fb, s_m_nil_fb, s_m_nlt_fb, 
                    s_m_irt_ib, s_m_irf_ib, s_m_lt_ib, s_m_ilt_ib, s_m_nil_ib, s_m_nlt_ib 
                };
                //things above copy it and reverse
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_RightFollowFalses =
                {
                    s_m_tb_irt, s_m_tb_irf, s_m_tb_nil, 
                    s_m_lt_irt, s_m_lt_irf, s_m_lt_nil, 
                    s_m_ilt_irt, s_m_ilt_irf, s_m_ilt_nil, 
                    s_m_irf_irt, s_m_irf_irf, s_m_irf_nil, 
                    s_m_nlt_irt, s_m_nlt_irf, s_m_nil_nlt, 
                    
                    s_m_irf_nlf, s_m_irt_nlf, s_m_nil_nlf
                };
                //things above copy it and reverse
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_RightFollowInds =
                {
                    s_m_irt_tb, s_m_irf_tb, s_m_nil_tb, s_m_nlt_tb, s_m_nlf_tb, 
                    s_m_irt_ilt, s_m_irf_ilt, s_m_nil_irt, s_m_nlt_ilt, s_m_nlf_ilt,
                    s_m_irt_nlt, s_m_irf_nlt, s_m_nil_nlt, s_m_nlt_nlt, s_m_nlf_nlt, 
                    
                    s_m_fb_irt, s_m_fb_irf, s_m_fb_nil, s_m_fb_nlf, s_m_fb_nlt, 
                    s_m_ilf_irt, s_m_ilf_irf, s_m_ilf_nil, s_m_ilf_nlf, s_m_ilf_nlt, 
                    s_m_nlf_irt, s_m_nlf_irf, s_m_nlf_nil, s_m_nlf_nlf, s_m_nlf_nlt 
                };
                //things above copy it and reverse
                //bi
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_BiActivateTrues =
                {
                    s_m_fb_irt, s_m_fb_irf, s_m_fb_nil, s_m_fb_ib,
                    s_m_irt_fb, s_m_irf_fb, s_m_nil_fb, s_m_ib_fb, 

                    s_m_rt_irt, s_m_rt_irf, s_m_rt_nil, s_m_rt_ib, 
                    s_m_irt_rt, s_m_irf_rt, s_m_nil_rt, s_m_ib_rt, 
                    s_m_ilf_irt, s_m_ilf_irf, s_m_ilf_nil, s_m_ilf_ib, 
                    s_m_irt_ilf, s_m_irf_ilf, s_m_nil_ilf, s_m_ib_ilf, 
                    s_m_nlf_irt, s_m_nlf_irf, s_m_nlf_nil, s_m_nlf_ib, 
                    s_m_irt_nlf, s_m_irf_nlt, s_m_nil_nlf, s_m_ib_nlf, 

                    s_m_irt_irt, s_m_irt_irf, s_m_irt_nil, s_m_irt_ib, 
                    s_m_irf_irf, s_m_irf_irt, s_m_irf_nil, s_m_irf_ib, 
                    s_m_nil_nil, s_m_nil_irf, s_m_nil_irt, s_m_nil_ib,
                    s_m_ib_ib, s_m_ib_irf, s_m_ib_nil, s_m_ib_irt,

                    s_m_tb_fb, s_m_tb_ilf, s_m_tb_nlf, s_m_tb_rt, 
                    s_m_fb_tb, s_m_ilf_tb,  s_m_nlf_tb,  s_m_rt_tb, 

                    s_m_nlt_fb, s_m_nlt_ilf, s_m_nlt_nlf, s_m_nlt_rt, 
                    s_m_fb_nlt, s_m_ilf_nlt, s_m_nlf_nlt, s_m_rt_nlt, 

                    s_m_ilt_fb, s_m_ilt_ilf, s_m_ilt_nlf, s_m_ilt_rt, 
                    s_m_fb_ilt, s_m_ilf_ilt, s_m_nlf_ilt, s_m_rt_ilt,

                    s_m_lt_fb, s_m_lt_ilf, s_m_lt_nlf, s_m_lt_rt, 
                    s_m_fb_lt, s_m_ilf_lt, s_m_nlf_lt, s_m_rt_lt
                };
                //things above cannot be true and must be able to be made true by activation
                //one indeterminate one both false lines 1 and 2
                //one indeterminate one partially false lines 4-9
                //both indeterminate lines 11 to 14
                //one false one true lines rest
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_BiActivateFalses = 
                {
                    s_m_fb_fb, s_m_fb_ilf, s_m_fb_nlf, s_m_fb_rt, 
                    s_m_ilf_ilf, s_m_ilf_fb, s_m_ilf_nlf, s_m_ilf_rt, 
                    s_m_nlf_nlf, s_m_nlf_fb, s_m_nlf_ilf, s_m_nlf_rt, 
                    s_m_rt_rt, s_m_rt_fb, s_m_rt_nlf, s_m_rt_ilf, 

                    s_m_fb_irt, s_m_fb_irf, s_m_fb_nil, s_m_fb_ib,
                    s_m_irt_fb, s_m_irf_fb, s_m_nil_fb, s_m_ib_fb, 

                    s_m_rt_irt, s_m_rt_irf, s_m_rt_nil, s_m_rt_ib, 
                    s_m_irt_rt, s_m_irf_rt, s_m_nil_rt, s_m_ib_rt, 
                    s_m_ilf_irt, s_m_ilf_irf, s_m_ilf_nil, s_m_ilf_ib, 
                    s_m_irt_ilf, s_m_irf_ilf, s_m_nil_ilf, s_m_ib_ilf, 
                    s_m_nlf_irt, s_m_nlf_irf, s_m_nlf_nil, s_m_nlf_ib, 
                    s_m_irt_nlf, s_m_irf_nlt, s_m_nil_nlf, s_m_ib_nlf

                };
                //things above must have one false and one ind or both false
                //both false lines 1 to 4
                //1 false 1 ind the rest
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_BiActivateInds =
                {
                    s_m_fb_fb, s_m_fb_ilf, s_m_fb_nlf, s_m_fb_rt, 
                    s_m_ilf_ilf, s_m_ilf_fb, s_m_ilf_nlf, s_m_ilf_rt, 
                    s_m_nlf_nlf, s_m_nlf_fb, s_m_nlf_ilf, s_m_nlf_rt, 
                    s_m_rt_rt, s_m_rt_fb, s_m_rt_nlf, s_m_rt_ilf
                };
                //things above must be both false
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_BiDeactivateTrues = 
                {
                    s_m_fb_irt, s_m_fb_irf, s_m_fb_nil, s_m_fb_ib,
                    s_m_irt_fb, s_m_irf_fb, s_m_nil_fb, s_m_ib_fb,
                    s_m_rt_irt, s_m_rt_irf, s_m_rt_nil, s_m_rt_ib, 
                    s_m_irt_rt, s_m_irf_rt, s_m_nil_rt, s_m_ib_rt, 
                    s_m_ilf_irt, s_m_ilf_irf, s_m_ilf_nil, s_m_ilf_ib, 
                    s_m_irt_ilf, s_m_irf_ilf, s_m_nil_ilf, s_m_ib_ilf, 
                    s_m_nlf_irt, s_m_nlf_irf, s_m_nlf_nil, s_m_nlf_ib, 
                    s_m_irt_nlf, s_m_irf_nlt, s_m_nil_nlf, s_m_ib_nlf,
 
                    s_m_tb_fb, s_m_tb_ilf, s_m_tb_nlf, s_m_tb_rt, 
                    s_m_fb_tb, s_m_ilf_tb,  s_m_nlf_tb,  s_m_rt_tb, 

                    s_m_nlt_fb, s_m_nlt_ilf, s_m_nlt_nlf, s_m_nlt_rt, 
                    s_m_fb_nlt, s_m_ilf_nlt, s_m_nlf_nlt, s_m_rt_nlt, 

                    s_m_ilt_fb, s_m_ilt_ilf, s_m_ilt_nlf, s_m_ilt_rt, 
                    s_m_fb_ilt, s_m_ilf_ilt, s_m_nlf_ilt, s_m_rt_ilt, 

                    s_m_lt_fb, s_m_lt_ilf, s_m_lt_nlf, s_m_lt_rt, 
                    s_m_fb_lt, s_m_ilf_lt, s_m_nlf_lt, s_m_rt_lt,

                    s_m_irt_irt, s_m_irt_irf, s_m_irt_nil, s_m_irt_ib, 
                    s_m_irf_irf, s_m_irf_irt, s_m_irf_nil, s_m_irf_ib, 
                    s_m_nil_nil, s_m_nil_irf, s_m_nil_irt, s_m_nil_ib,
                    s_m_ib_ib, s_m_ib_irf, s_m_ib_nil, s_m_ib_irt,

                    s_m_tb_irt, s_m_tb_irf, s_m_tb_nil, s_m_tb_ib,
                    s_m_irt_tb, s_m_irf_tb, s_m_nil_tb, s_m_ib_tb,
                    s_m_lt_irt, s_m_lt_irf, s_m_lt_nil, s_m_lt_ib, 
                    s_m_irt_lt, s_m_irf_lt, s_m_nil_lt, s_m_ib_lt, 
                    s_m_ilt_irt, s_m_ilt_irf, s_m_ilt_nil, s_m_ilt_ib, 
                    s_m_irt_ilt, s_m_irf_ilt, s_m_nil_ilt, s_m_ib_ilt, 
                    s_m_nlt_irt, s_m_nlt_irf, s_m_nlt_nil, s_m_nlt_ib, 
                    s_m_irt_nlt, s_m_irf_nlt, s_m_nil_nlt, s_m_ib_nlt
                };
                //things above as long as both not false and both not true
                //one ind one false lines 1-8
                //one false one true lines to 20
                //both ind lines 22-25
                //one true one ind the rest
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_BiDeactivateFalses = 
                {
                    s_m_tb_tb, s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt, 
                    s_m_lt_lt, s_m_lt_tb, s_m_lt_ilt, s_m_lt_nlt, 
                    s_m_ilt_ilt, s_m_ilt_tb, s_m_ilt_lt, s_m_ilt_nlt, 
                    s_m_nlt_nlt, s_m_nlt_tb, s_m_nlt_lt, s_m_nlt_ilt, 
                    s_m_tb_irt, s_m_tb_irf, s_m_tb_nil, s_m_tb_ib,
                    s_m_irt_tb, s_m_irf_tb, s_m_nil_tb, s_m_ib_tb,
                    s_m_lt_irt, s_m_lt_irf, s_m_lt_nil, s_m_lt_ib, 
                    s_m_irt_lt, s_m_irf_lt, s_m_nil_lt, s_m_ib_lt, 
                    s_m_ilt_irt, s_m_ilt_irf, s_m_ilt_nil, s_m_ilt_ib, 
                    s_m_irt_ilt, s_m_irf_ilt, s_m_nil_ilt, s_m_ib_ilt, 
                    s_m_nlt_irt, s_m_nlt_irf, s_m_nlt_nil, s_m_nlt_ib, 
                    s_m_irt_nlt, s_m_irf_nlt, s_m_nil_nlt, s_m_ib_nlt
                };
                //things above must be both true or one true one ind
                //both true lines 1 to 4
                //the rest one true one ind
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_BiDeactivateInds =
                {
                    s_m_tb_tb, s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt, 
                    s_m_lt_lt, s_m_lt_tb, s_m_lt_ilt, s_m_lt_nlt, 
                    s_m_ilt_ilt, s_m_ilt_tb, s_m_ilt_lt, s_m_ilt_nlt, 
                    s_m_nlt_nlt, s_m_nlt_tb, s_m_nlt_lt, s_m_nlt_ilt
                };
                //things above must both be true
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_BiFollowTrues =
                {
                    s_m_tb_irt, s_m_tb_irf, s_m_tb_nil, 
                    s_m_irt_tb, s_m_irf_tb, s_m_nil_tb, 
                    s_m_lt_irt, s_m_lt_irf, s_m_lt_nil, s_m_lt_ib, 
                    s_m_irt_lt, s_m_irf_lt, s_m_nil_lt, s_m_ib_lt, 
                    s_m_ilt_irt, s_m_ilt_irf, s_m_ilt_nil, s_m_ilt_ib, 
                    s_m_irt_ilt, s_m_irf_ilt, s_m_nil_ilt, s_m_ib_ilt, 
                    s_m_nlt_irt, s_m_nlt_irf, s_m_nlt_nil, s_m_nlt_ib, 
                    s_m_irt_nlt, s_m_irf_nlt, s_m_nil_nlt, s_m_ib_nlt,
                    s_m_fb_irt, s_m_fb_irf, s_m_fb_nil, 
                    s_m_irt_fb, s_m_irf_fb, s_m_nil_fb, 
                    s_m_rt_irt, s_m_rt_irf, s_m_rt_nil, s_m_rt_ib, 
                    s_m_irt_rt, s_m_irf_rt, s_m_nil_rt, s_m_ib_rt, 
                    s_m_ilf_irt, s_m_ilf_irf, s_m_ilf_nil, s_m_ilf_ib, 
                    s_m_irt_ilf, s_m_irf_ilf, s_m_nil_ilf, s_m_ib_ilf, 
                    s_m_nlf_irt, s_m_nlf_irf, s_m_nlf_nil, s_m_nlf_ib, 
                    s_m_irt_nlf, s_m_irf_nlt, s_m_nil_nlf, s_m_ib_nlf,
                    s_m_tb_ilf, s_m_tb_nlf, s_m_tb_rt, 
                    s_m_ilf_tb,  s_m_nlf_tb,  s_m_rt_tb, 
                    s_m_nlt_fb, s_m_nlt_ilf, s_m_nlt_nlf, s_m_nlt_rt, 
                    s_m_fb_nlt, s_m_ilf_nlt, s_m_nlf_nlt, s_m_rt_nlt, 
                    s_m_ilt_fb, s_m_ilt_ilf, s_m_ilt_nlf, s_m_ilt_rt, 
                    s_m_fb_ilt, s_m_ilf_ilt, s_m_nlf_ilt, s_m_rt_ilt, 
                    s_m_lt_fb, s_m_lt_ilf, s_m_lt_nlf, s_m_lt_rt, 
                    s_m_fb_lt, s_m_ilf_lt, s_m_nlf_lt, s_m_rt_lt,
                    s_m_irt_irt, s_m_irt_irf, s_m_irt_nil, s_m_irt_ib, 
                    s_m_irf_irf, s_m_irf_irt, s_m_irf_nil, s_m_irf_ib, 
                    s_m_nil_nil, s_m_nil_irf, s_m_nil_irt, s_m_nil_ib,
                    s_m_ib_irf, s_m_ib_nil, s_m_ib_irt
                };
                //things above something must have changed, must not be true
                //one true one ind lines 1 to 8
                //one false one ind lines 9 to 16
                //one true one false with change lines 17 to 24
                //both inds lines the rest
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_BiFollowFalses = 
                {
                    s_m_tb_irt, s_m_tb_irf, s_m_tb_nil, 
                    s_m_irt_tb, s_m_irf_tb, s_m_nil_tb, 
                    s_m_lt_irt, s_m_lt_irf, s_m_lt_nil, s_m_lt_ib, 
                    s_m_irt_lt, s_m_irf_lt, s_m_nil_lt, s_m_ib_lt, 
                    s_m_ilt_irt, s_m_ilt_irf, s_m_ilt_nil, s_m_ilt_ib, 
                    s_m_irt_ilt, s_m_irf_ilt, s_m_nil_ilt, s_m_ib_ilt, 
                    s_m_nlt_irt, s_m_nlt_irf, s_m_nlt_nil, s_m_nlt_ib, 
                    s_m_irt_nlt, s_m_irf_nlt, s_m_nil_nlt, s_m_ib_nlt,
                    s_m_fb_irt, s_m_fb_irf, s_m_fb_nil, 
                    s_m_irt_fb, s_m_irf_fb, s_m_nil_fb, 
                    s_m_rt_irt, s_m_rt_irf, s_m_rt_nil, s_m_rt_ib, 
                    s_m_irt_rt, s_m_irf_rt, s_m_nil_rt, s_m_ib_rt, 
                    s_m_ilf_irt, s_m_ilf_irf, s_m_ilf_nil, s_m_ilf_ib, 
                    s_m_irt_ilf, s_m_irf_ilf, s_m_nil_ilf, s_m_ib_ilf, 
                    s_m_nlf_irt, s_m_nlf_irf, s_m_nlf_nil, s_m_nlf_ib, 
                    s_m_irt_nlf, s_m_irf_nlt, s_m_nil_nlf, s_m_ib_nlf,
                    s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt, 
                    s_m_lt_lt, s_m_lt_tb, s_m_lt_ilt, s_m_lt_nlt, 
                    s_m_ilt_ilt, s_m_ilt_tb, s_m_ilt_lt, s_m_ilt_nlt, 
                    s_m_nlt_nlt, s_m_nlt_tb, s_m_nlt_lt, s_m_nlt_ilt,
                    s_m_fb_ilf, s_m_fb_nlf, s_m_fb_rt, 
                    s_m_ilf_ilf, s_m_ilf_fb, s_m_ilf_nlf, s_m_ilf_rt, 
                    s_m_nlf_nlf, s_m_nlf_fb, s_m_nlf_ilf, s_m_nlf_rt, 
                    s_m_rt_rt, s_m_rt_fb, s_m_rt_nlf, s_m_rt_ilf, 
                    s_m_irt_irt, s_m_irt_irf, s_m_irt_nil, s_m_irt_ib, 
                    s_m_irf_irf, s_m_irf_irt, s_m_irf_nil, s_m_irf_ib, 
                    s_m_nil_nil, s_m_nil_irf, s_m_nil_irt, s_m_nil_ib,
                    s_m_ib_irf, s_m_ib_nil, s_m_ib_irt
                };
                //things above something must have change must not have been originally false
                //one true or false one ind lines 1 to 16
                //both true or both false with a change lines 17 to 24
                //both inds lines the rest
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_BiFollowInds =
                {
                    s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt, 
                    s_m_lt_lt, s_m_lt_tb, s_m_lt_ilt, s_m_lt_nlt, 
                    s_m_ilt_ilt, s_m_ilt_tb, s_m_ilt_lt, s_m_ilt_nlt, 
                    s_m_nlt_nlt, s_m_nlt_tb, s_m_nlt_lt, s_m_nlt_ilt,
                    s_m_fb_ilf, s_m_fb_nlf, s_m_fb_rt, 
                    s_m_ilf_ilf, s_m_ilf_fb, s_m_ilf_nlf, s_m_ilf_rt, 
                    s_m_nlf_nlf, s_m_nlf_fb, s_m_nlf_ilf, s_m_nlf_rt, 
                    s_m_rt_rt, s_m_rt_fb, s_m_rt_nlf, s_m_rt_ilf, 
                    s_m_tb_ilf, s_m_tb_nlf, s_m_tb_rt, 
                    s_m_ilf_tb,  s_m_nlf_tb,  s_m_rt_tb, 
                    s_m_nlt_fb, s_m_nlt_ilf, s_m_nlt_nlf, s_m_nlt_rt, 
                    s_m_fb_nlt, s_m_ilf_nlt, s_m_nlf_nlt, s_m_rt_nlt, 
                    s_m_ilt_fb, s_m_ilt_ilf, s_m_ilt_nlf, s_m_ilt_rt, 
                    s_m_fb_ilt, s_m_ilf_ilt, s_m_nlf_ilt, s_m_rt_ilt, 
                    s_m_lt_fb, s_m_lt_ilf, s_m_lt_nlf, s_m_lt_rt, 
                    s_m_fb_lt, s_m_ilf_lt, s_m_nlf_lt, s_m_rt_lt
                };
                //things above must not be ind and must have a change so no inds allowed
                //both true or both false lines 1-8
                //one true one false lines 9 to 16
                //and
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_AndActivateTrues =
                {
                    s_m_tb_fb, s_m_tb_rt, s_m_tb_ilf, s_m_tb_nlf, 
                    s_m_fb_tb, s_m_rt_tb, s_m_ilf_tb, s_m_nlf_tb, 
                    s_m_lt_fb, s_m_lt_rt, s_m_lt_ilf, s_m_lt_nlf, 
                    s_m_fb_lt, s_m_rt_lt, s_m_ilf_lt, s_m_nlf_lt, 
                    s_m_ilt_fb, s_m_ilt_lt, s_m_ilt_ilf, s_m_ilt_nlf, 
                    s_m_fb_ilt, s_m_lt_ilt, s_m_ilf_ilt, s_m_nlf_ilt, 
                    s_m_nlt_fb, s_m_nlt_rt, s_m_nlt_ilf, s_m_nlt_nlf, 
                    s_m_fb_nlt, s_m_rt_nlt, s_m_ilf_nlt, s_m_nlf_nlt, 
                    s_m_tb_ib, s_m_tb_irf, s_m_tb_irt, s_m_tb_nil, 
                    s_m_ib_tb, s_m_irf_tb, s_m_irt_tb, s_m_nil_tb, 
                    s_m_lt_ib, s_m_lt_irf, s_m_lt_irt, s_m_lt_nil, 
                    s_m_ib_lt, s_m_irt_lt, s_m_irt_lt, s_m_nil_lt, 
                    s_m_ilt_ib, s_m_ilt_irf, s_m_ilt_irt, s_m_ilt_nil, 
                    s_m_ib_ilt, s_m_irf_ilt, s_m_irt_ilt, s_m_nil_ilt, 
                    s_m_nlt_ib, s_m_nlt_irf, s_m_nlt_irt, s_m_nlt_nil, 
                    s_m_ib_nlt, s_m_irf_nlt, s_m_irf_nlt, s_m_nil_nlt, 
                    s_m_irt_irt, s_m_irt_irf, s_m_irt_nil, s_m_irt_ib, 
                    s_m_irf_irf, s_m_irf_irt, s_m_irf_nil, s_m_irf_ib, 
                    s_m_nil_nil, s_m_nil_irf, s_m_nil_irt, s_m_nil_ib,
                    s_m_ib_ib, s_m_ib_irf, s_m_ib_nil, s_m_ib_irt
                };
                //things above all they need to be is not true for and
                //lines 1 to 8 one false one true
                //lines one true one ind (no falses allowed) lines 9 to 16
                //both ind to line 20
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_AndActivateFalses = {};//blank
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_AndActivateInds =
                {
                    s_m_tb_fb, s_m_tb_ilf, s_m_tb_nlf, s_m_tb_rt, 
                    s_m_fb_tb, s_m_ilf_tb,  s_m_nlf_tb,  s_m_rt_tb, 
                    s_m_nlt_fb, s_m_nlt_ilf, s_m_nlt_nlf, s_m_nlt_rt, 
                    s_m_fb_nlt, s_m_ilf_nlt, s_m_nlf_nlt, s_m_rt_nlt, 
                    s_m_ilt_fb, s_m_ilt_ilf, s_m_ilt_nlf, s_m_ilt_rt, 
                    s_m_fb_ilt, s_m_ilf_ilt, s_m_nlf_ilt, s_m_rt_ilt,
                    s_m_lt_fb, s_m_lt_ilf, s_m_lt_nlf, s_m_lt_rt, 
                    s_m_fb_lt, s_m_ilf_lt, s_m_nlf_lt, s_m_rt_lt
                };
                //things above one must be true and one must be false
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_AndDeactivateTrues = {};//blank
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_AndDeactivateFalses =
                {
                    s_m_irt_irt, s_m_irt_irf, s_m_irt_nil, s_m_irt_ib, 
                    s_m_irf_irf, s_m_irf_irt, s_m_irf_nil, s_m_irf_ib, 
                    s_m_nil_nil, s_m_nil_irf, s_m_nil_irt, s_m_nil_ib,
                    s_m_ib_ib, s_m_ib_irf, s_m_ib_nil, s_m_ib_irt,
                    s_m_tb_ib, s_m_tb_irf, s_m_tb_irt, s_m_tb_nil, 
                    s_m_ib_tb, s_m_irf_tb, s_m_irt_tb, s_m_nil_tb, 
                    s_m_lt_ib, s_m_lt_irf, s_m_lt_irt, s_m_lt_nil, 
                    s_m_ib_lt, s_m_irt_lt, s_m_irt_lt, s_m_nil_lt, 
                    s_m_ilt_ib, s_m_ilt_irf, s_m_ilt_irt, s_m_ilt_nil, 
                    s_m_ib_ilt, s_m_irf_ilt, s_m_irt_ilt, s_m_nil_ilt, 
                    s_m_nlt_ib, s_m_nlt_irf, s_m_nlt_irt, s_m_nlt_nil, 
                    s_m_ib_nlt, s_m_irf_nlt, s_m_irf_nlt, s_m_nil_nlt,
                    s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt, s_m_tb_tb,
                    s_m_lt_lt, s_m_lt_tb, s_m_lt_ilt, s_m_lt_nlt, 
                    s_m_ilt_ilt, s_m_ilt_tb, s_m_ilt_lt, s_m_ilt_nlt, 
                    s_m_nlt_nlt, s_m_nlt_tb, s_m_nlt_lt, s_m_nlt_ilt
                };
                //things above must be true and ind or both true or both ind
                //both ind lines 1 to 4
                //one ind one true lines 5 to 12
                //both true lines 13 to 16
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_AndDeactivateInds =
                {
                    s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt, s_m_tb_tb,
                    s_m_lt_lt, s_m_lt_tb, s_m_lt_ilt, s_m_lt_nlt, 
                    s_m_ilt_ilt, s_m_ilt_tb, s_m_ilt_lt, s_m_ilt_nlt, 
                    s_m_nlt_nlt, s_m_nlt_tb, s_m_nlt_lt, s_m_nlt_ilt
                };
                //things above must be both true
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_AndFollowTrues =
                {
                    s_m_irt_irt, s_m_irt_irf, s_m_irt_nil, s_m_irt_ib, 
                    s_m_irf_irf, s_m_irf_irt, s_m_irf_nil, s_m_irf_ib, 
                    s_m_nil_nil, s_m_nil_irf, s_m_nil_irt, s_m_nil_ib,
                    s_m_ib_irf, s_m_ib_nil, s_m_ib_irt,
                    s_m_tb_irf, s_m_tb_irt, s_m_tb_nil, 
                    s_m_irf_tb, s_m_irt_tb, s_m_nil_tb, 
                    s_m_lt_ib, s_m_lt_irf, s_m_lt_irt, s_m_lt_nil, 
                    s_m_ib_lt, s_m_irt_lt, s_m_irt_lt, s_m_nil_lt, 
                    s_m_ilt_ib, s_m_ilt_irf, s_m_ilt_irt, s_m_ilt_nil, 
                    s_m_ib_ilt, s_m_irf_ilt, s_m_irt_ilt, s_m_nil_ilt, 
                    s_m_nlt_ib, s_m_nlt_irf, s_m_nlt_irt, s_m_nlt_nil, 
                    s_m_ib_nlt, s_m_irf_nlt, s_m_irf_nlt, s_m_nil_nlt,
                    s_m_tb_ilf, s_m_tb_nlf, s_m_tb_rt, 
                    s_m_ilf_tb,  s_m_nlf_tb,  s_m_rt_tb, 
                    s_m_nlt_fb, s_m_nlt_ilf, s_m_nlt_nlf, s_m_nlt_rt, 
                    s_m_fb_nlt, s_m_ilf_nlt, s_m_nlf_nlt, s_m_rt_nlt, 
                    s_m_ilt_fb, s_m_ilt_ilf, s_m_ilt_nlf, s_m_ilt_rt, 
                    s_m_fb_ilt, s_m_ilf_ilt, s_m_nlf_ilt, s_m_rt_ilt,
                    s_m_lt_fb, s_m_lt_ilf, s_m_lt_nlf, s_m_lt_rt, 
                    s_m_fb_lt, s_m_ilf_lt, s_m_nlf_lt, s_m_rt_lt
                };
                //things above must be true false with a change or true ind with a change or both ind with a change
                //both ind lines 1 to 4, 1 ind one true lines 5 to 12
                //one true one false to line 20
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_AndFollowFalses =
                {
                    s_m_irt_irt, s_m_irt_irf, s_m_irt_nil, s_m_irt_ib, 
                    s_m_irf_irf, s_m_irf_irt, s_m_irf_nil, s_m_irf_ib, 
                    s_m_nil_nil, s_m_nil_irf, s_m_nil_irt, s_m_nil_ib,
                    s_m_ib_irf, s_m_ib_nil, s_m_ib_irt,
                    s_m_tb_irf, s_m_tb_irt, s_m_tb_nil, 
                    s_m_irf_tb, s_m_irt_tb, s_m_nil_tb, 
                    s_m_lt_ib, s_m_lt_irf, s_m_lt_irt, s_m_lt_nil, 
                    s_m_ib_lt, s_m_irt_lt, s_m_irt_lt, s_m_nil_lt, 
                    s_m_ilt_ib, s_m_ilt_irf, s_m_ilt_irt, s_m_ilt_nil, 
                    s_m_ib_ilt, s_m_irf_ilt, s_m_irt_ilt, s_m_nil_ilt, 
                    s_m_nlt_ib, s_m_nlt_irf, s_m_nlt_irt, s_m_nlt_nil, 
                    s_m_ib_nlt, s_m_irf_nlt, s_m_irf_nlt, s_m_nil_nlt
                };
                //things above must be one true one ind with a change or both ind with a change
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_AndFollowInds =
                {
                    s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt,
                    s_m_lt_lt, s_m_lt_tb, s_m_lt_ilt, s_m_lt_nlt, 
                    s_m_ilt_ilt, s_m_ilt_tb, s_m_ilt_lt, s_m_ilt_nlt, 
                    s_m_nlt_nlt, s_m_nlt_tb, s_m_nlt_lt, s_m_nlt_ilt,
                    s_m_fb_ilf, s_m_fb_nlf, s_m_fb_rt, 
                    s_m_ilf_ilf, s_m_ilf_fb, s_m_ilf_nlf, s_m_ilf_rt, 
                    s_m_nlf_nlf, s_m_nlf_fb, s_m_nlf_ilf, s_m_nlf_rt, 
                    s_m_rt_rt, s_m_rt_fb, s_m_rt_nlf, s_m_rt_ilf
                };
                //things above must be both true or both false with a change
                //both true lines 1 to 4
                //both false lines 5 to 8
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_NotActivateTrues = {};//blank
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_NotActivateFalses =
                {
                    s_m_fb_fb, s_m_fb_rt, s_m_fb_ilf, s_m_fb_nlf,
                    s_m_rt_rt, s_m_rt_fb, s_m_rt_ilf, s_m_rt_nlf, 
                    s_m_ilf_ilf, s_m_ilf_fb, s_m_ilf_rt, s_m_ilf_nlf, 
                    s_m_nlf_nlf, s_m_nlf_fb, s_m_nlf_rt, s_m_nlf_ilf, 
                    s_m_irt_irt, s_m_irt_irf, s_m_irt_nil, s_m_irt_ib, 
                    s_m_irf_irf, s_m_irf_irt, s_m_irf_nil, s_m_irf_ib, 
                    s_m_nil_nil, s_m_nil_irf, s_m_nil_irt, s_m_nil_ib,
                    s_m_ib_ib, s_m_ib_irf, s_m_ib_nil, s_m_ib_irt,
                    s_m_fb_irt, s_m_fb_irf, s_m_fb_nil, s_m_fb_ib,
                    s_m_irt_fb, s_m_irf_fb, s_m_nil_fb, s_m_ib_fb,
                    s_m_rt_irt, s_m_rt_irf, s_m_rt_nil, s_m_rt_ib, 
                    s_m_irt_rt, s_m_irf_rt, s_m_nil_rt, s_m_ib_rt, 
                    s_m_ilf_irt, s_m_ilf_irf, s_m_ilf_nil, s_m_ilf_ib, 
                    s_m_irt_ilf, s_m_irf_ilf, s_m_nil_ilf, s_m_ib_ilf, 
                    s_m_nlf_irt, s_m_nlf_irf, s_m_nlf_nil, s_m_nlf_ib, 
                    s_m_irt_nlf, s_m_irf_nlt, s_m_nil_nlf, s_m_ib_nlf
                };
                //things above must be both false or one false one ind or both ind
                //both false lines 1 to 4
                //both ind lines 5 to 8
                //one ind one false lines 9 to 16
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_NotActivateInds =
                {
                    s_m_fb_fb, s_m_fb_rt, s_m_fb_ilf, s_m_fb_nlf,
                    s_m_rt_rt, s_m_rt_fb, s_m_rt_ilf, s_m_rt_nlf, 
                    s_m_ilf_ilf, s_m_ilf_fb, s_m_ilf_rt, s_m_ilf_nlf, 
                    s_m_nlf_nlf, s_m_nlf_fb, s_m_nlf_rt, s_m_nlf_ilf
                };
                //things above must be both false
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_NotDeactivateTrues =
                {
                    s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt, s_m_tb_tb,
                    s_m_lt_lt, s_m_lt_tb, s_m_lt_ilt, s_m_lt_nlt, 
                    s_m_ilt_ilt, s_m_ilt_tb, s_m_ilt_lt, s_m_ilt_nlt, 
                    s_m_nlt_nlt, s_m_nlt_tb, s_m_nlt_lt, s_m_nlt_ilt,
                    s_m_irt_irt, s_m_irt_irf, s_m_irt_nil, s_m_irt_ib, 
                    s_m_irf_irf, s_m_irf_irt, s_m_irf_nil, s_m_irf_ib, 
                    s_m_nil_nil, s_m_nil_irf, s_m_nil_irt, s_m_nil_ib,
                    s_m_ib_ib, s_m_ib_irf, s_m_ib_nil, s_m_ib_irt,
                    s_m_fb_irt, s_m_fb_irf, s_m_fb_nil, s_m_fb_ib,
                    s_m_irt_fb, s_m_irf_fb, s_m_nil_fb, s_m_ib_fb,
                    s_m_rt_irt, s_m_rt_irf, s_m_rt_nil, s_m_rt_ib, 
                    s_m_irt_rt, s_m_irf_rt, s_m_nil_rt, s_m_ib_rt, 
                    s_m_ilf_irt, s_m_ilf_irf, s_m_ilf_nil, s_m_ilf_ib, 
                    s_m_irt_ilf, s_m_irf_ilf, s_m_nil_ilf, s_m_ib_ilf, 
                    s_m_nlf_irt, s_m_nlf_irf, s_m_nlf_nil, s_m_nlf_ib, 
                    s_m_irt_nlf, s_m_irf_nlt, s_m_nil_nlf, s_m_ib_nlf,
                    s_m_tb_ib, s_m_tb_irf, s_m_tb_irt, s_m_tb_nil, 
                    s_m_ib_tb, s_m_irf_tb, s_m_irt_tb, s_m_nil_tb, 
                    s_m_lt_ib, s_m_lt_irf, s_m_lt_irt, s_m_lt_nil, 
                    s_m_ib_lt, s_m_irt_lt, s_m_irt_lt, s_m_nil_lt, 
                    s_m_ilt_ib, s_m_ilt_irf, s_m_ilt_irt, s_m_ilt_nil, 
                    s_m_ib_ilt, s_m_irf_ilt, s_m_irt_ilt, s_m_nil_ilt, 
                    s_m_nlt_ib, s_m_nlt_irf, s_m_nlt_irt, s_m_nlt_nil, 
                    s_m_ib_nlt, s_m_irf_nlt, s_m_irf_nlt, s_m_nil_nlt
                };
                //things above can be anything accept both falses
                //both true lines 1 to 4
                //both ind lines 5 to 8
                //one ind one false lines 9 to 16
                //one ind one true lines 17 to 24
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_NotDeactivateFalses = {};//blank
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_NotDeactivateInds =
                {
                    s_m_irt_irt, s_m_irt_irf, s_m_irt_nil, s_m_irt_ib, 
                    s_m_irf_irf, s_m_irf_irt, s_m_irf_nil, s_m_irf_ib, 
                    s_m_nil_nil, s_m_nil_irf, s_m_nil_irt, s_m_nil_ib,
                    s_m_ib_ib, s_m_ib_irf, s_m_ib_nil, s_m_ib_irt,
                    s_m_tb_ib, s_m_tb_irf, s_m_tb_irt, s_m_tb_nil, 
                    s_m_ib_tb, s_m_irf_tb, s_m_irt_tb, s_m_nil_tb, 
                    s_m_lt_ib, s_m_lt_irf, s_m_lt_irt, s_m_lt_nil, 
                    s_m_ib_lt, s_m_irt_lt, s_m_irt_lt, s_m_nil_lt, 
                    s_m_ilt_ib, s_m_ilt_irf, s_m_ilt_irt, s_m_ilt_nil, 
                    s_m_ib_ilt, s_m_irf_ilt, s_m_irt_ilt, s_m_nil_ilt, 
                    s_m_nlt_ib, s_m_nlt_irf, s_m_nlt_irt, s_m_nlt_nil, 
                    s_m_ib_nlt, s_m_irf_nlt, s_m_irf_nlt, s_m_nil_nlt,
                    s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt, s_m_tb_tb,
                    s_m_lt_lt, s_m_lt_tb, s_m_lt_ilt, s_m_lt_nlt, 
                    s_m_ilt_ilt, s_m_ilt_tb, s_m_ilt_lt, s_m_ilt_nlt, 
                    s_m_nlt_nlt, s_m_nlt_tb, s_m_nlt_lt, s_m_nlt_ilt,
                    s_m_tb_fb, s_m_tb_ilf, s_m_tb_nlf, s_m_tb_rt, 
                    s_m_fb_tb, s_m_ilf_tb,  s_m_nlf_tb,  s_m_rt_tb, 
                    s_m_nlt_fb, s_m_nlt_ilf, s_m_nlt_nlf, s_m_nlt_rt, 
                    s_m_fb_nlt, s_m_ilf_nlt, s_m_nlf_nlt, s_m_rt_nlt, 
                    s_m_ilt_fb, s_m_ilt_ilf, s_m_ilt_nlf, s_m_ilt_rt, 
                    s_m_fb_ilt, s_m_ilf_ilt, s_m_nlf_ilt, s_m_rt_ilt,
                    s_m_lt_fb, s_m_lt_ilf, s_m_lt_nlf, s_m_lt_rt, 
                    s_m_fb_lt, s_m_ilf_lt, s_m_nlf_lt, s_m_rt_lt
                };
                //things above must be both ind, or one true one false, or both true, of one ind one true
                //both inds line 1-4
                //one ind one true lines 5-12
                //both true lines 13 to 16
                //one true one false lines 17 to 24
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_NotFollowTrues =
                {
                    s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt,
                    s_m_lt_lt, s_m_lt_tb, s_m_lt_ilt, s_m_lt_nlt, 
                    s_m_ilt_ilt, s_m_ilt_tb, s_m_ilt_lt, s_m_ilt_nlt, 
                    s_m_nlt_nlt, s_m_nlt_tb, s_m_nlt_lt, s_m_nlt_ilt,
                    s_m_irt_irt, s_m_irt_irf, s_m_irt_nil, s_m_irt_ib, 
                    s_m_irf_irf, s_m_irf_irt, s_m_irf_nil, s_m_irf_ib, 
                    s_m_nil_nil, s_m_nil_irf, s_m_nil_irt, s_m_nil_ib,
                    s_m_ib_irf, s_m_ib_nil, s_m_ib_irt,
                    s_m_fb_irt, s_m_fb_irf, s_m_fb_nil, 
                    s_m_irt_fb, s_m_irf_fb, s_m_nil_fb, 
                    s_m_rt_irt, s_m_rt_irf, s_m_rt_nil, s_m_rt_ib, 
                    s_m_irt_rt, s_m_irf_rt, s_m_nil_rt, s_m_ib_rt, 
                    s_m_ilf_irt, s_m_ilf_irf, s_m_ilf_nil, s_m_ilf_ib, 
                    s_m_irt_ilf, s_m_irf_ilf, s_m_nil_ilf, s_m_ib_ilf, 
                    s_m_nlf_irt, s_m_nlf_irf, s_m_nlf_nil, s_m_nlf_ib, 
                    s_m_irt_nlf, s_m_irf_nlt, s_m_nil_nlf, s_m_ib_nlf,
                    s_m_tb_irf, s_m_tb_irt, s_m_tb_nil, 
                    s_m_irf_tb, s_m_irt_tb, s_m_nil_tb, 
                    s_m_lt_ib, s_m_lt_irf, s_m_lt_irt, s_m_lt_nil, 
                    s_m_ib_lt, s_m_irt_lt, s_m_irt_lt, s_m_nil_lt, 
                    s_m_ilt_ib, s_m_ilt_irf, s_m_ilt_irt, s_m_ilt_nil, 
                    s_m_ib_ilt, s_m_irf_ilt, s_m_irt_ilt, s_m_nil_ilt, 
                    s_m_nlt_ib, s_m_nlt_irf, s_m_nlt_irt, s_m_nlt_nil, 
                    s_m_ib_nlt, s_m_irf_nlt, s_m_irf_nlt, s_m_nil_nlt
                };
                //things above must not be both false with a change
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_NotFollowFalses =
                {
                    s_m_irt_irt, s_m_irt_irf, s_m_irt_nil, s_m_irt_ib, 
                    s_m_irf_irf, s_m_irf_irt, s_m_irf_nil, s_m_irf_ib, 
                    s_m_nil_nil, s_m_nil_irf, s_m_nil_irt, s_m_nil_ib,
                    s_m_ib_irf, s_m_ib_nil, s_m_ib_irt,
                    s_m_fb_rt, s_m_fb_ilf, s_m_fb_nlf,
                    s_m_rt_rt, s_m_rt_fb, s_m_rt_ilf, s_m_rt_nlf, 
                    s_m_ilf_ilf, s_m_ilf_fb, s_m_ilf_rt, s_m_ilf_nlf, 
                    s_m_nlf_nlf, s_m_nlf_fb, s_m_nlf_rt, s_m_nlf_ilf, 
                    s_m_fb_irt, s_m_fb_irf, s_m_fb_nil, 
                    s_m_irt_fb, s_m_irf_fb, s_m_nil_fb, 
                    s_m_rt_irt, s_m_rt_irf, s_m_rt_nil, s_m_rt_ib, 
                    s_m_irt_rt, s_m_irf_rt, s_m_nil_rt, s_m_ib_rt, 
                    s_m_ilf_irt, s_m_ilf_irf, s_m_ilf_nil, s_m_ilf_ib, 
                    s_m_irt_ilf, s_m_irf_ilf, s_m_nil_ilf, s_m_ib_ilf, 
                    s_m_nlf_irt, s_m_nlf_irf, s_m_nlf_nil, s_m_nlf_ib, 
                    s_m_irt_nlf, s_m_irf_nlt, s_m_nil_nlf, s_m_ib_nlf,
                };
                //things above must evaluate to false or ind with a change, no trues allowed
                //both inds lines 1 to 4
                //both falses lines 5 to 8
                //one false one ind lines 9 to 16
                private static readonly Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>[] s_m_NotFollowInds =
                {
                    s_m_fb_rt, s_m_fb_ilf, s_m_fb_nlf,
                    s_m_rt_rt, s_m_rt_fb, s_m_rt_ilf, s_m_rt_nlf, 
                    s_m_ilf_ilf, s_m_ilf_fb, s_m_ilf_rt, s_m_ilf_nlf, 
                    s_m_nlf_nlf, s_m_nlf_fb, s_m_nlf_rt, s_m_nlf_ilf, 
                    s_m_tb_lt, s_m_tb_ilt, s_m_tb_nlt, s_m_tb_tb,
                    s_m_lt_lt, s_m_lt_tb, s_m_lt_ilt, s_m_lt_nlt, 
                    s_m_ilt_ilt, s_m_ilt_tb, s_m_ilt_lt, s_m_ilt_nlt, 
                    s_m_nlt_nlt, s_m_nlt_tb, s_m_nlt_lt, s_m_nlt_ilt
                };
                //things above must be both false or both true with a change
                

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
                    KeyLogicManager.m_methodEvalDict = new Dictionary<Tuple<logics, methods, logicStates>, List<Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>>>();
                    //assign values to the dictionary for matching methods
                    //XOR, LEFT, RIGHT, BI, NONE, NOT
                    KeyLogicManager.m_methodDict[logics.XOR] = new List<methods>(new methods[]{methods.DEACTIVATE, methods.FOLLOW, methods.NONE});
                    List<methods> leftlist = new List<methods>(new methods[] { methods.ACTIVATE, methods.DEACTIVATE, methods.FOLLOW, methods.NONE });
                    KeyLogicManager.m_methodDict[logics.LEFT] = leftlist;
                    KeyLogicManager.m_methodDict[logics.RIGHT] = leftlist.ToList();
                    KeyLogicManager.m_methodDict[logics.BI] = leftlist.ToList();
                    KeyLogicManager.m_methodDict[logics.AND] = new List<methods>(new methods[] { methods.ACTIVATE, methods.NONE });
                    KeyLogicManager.m_methodDict[logics.NOT] = new List<methods>(new methods[] { methods.DEACTIVATE, methods.NONE });
                    KeyLogicManager.m_methodDict[logics.NONE] = new List<methods>(new methods[] { methods.NONE });
                    //assign values to the dictionary for matching
                    //XOR : false, Left : false, Right : false, BI : false, NOT : false
                    KeyLogicManager.m_indEvalDict[logics.XOR] = logicStates.FALSE;
                    KeyLogicManager.m_indEvalDict[logics.LEFT] = logicStates.FALSE;
                    KeyLogicManager.m_indEvalDict[logics.RIGHT] = logicStates.FALSE;
                    KeyLogicManager.m_indEvalDict[logics.BI] = logicStates.FALSE;
                    KeyLogicManager.m_indEvalDict[logics.NOT] = logicStates.FALSE;
                    KeyLogicManager.m_indEvalDict[logics.NONE] = logicStates.FALSE;
                    KeyLogicManager.m_indEvalDict[logics.AND] = logicStates.FALSE;
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
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.AND, logicStates.TRUE)] = KeyLogicManager.s_AndTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.AND, logicStates.FALSE)] = KeyLogicManager.s_AndFalses.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.AND, logicStates.INDETERMINATE)] = KeyLogicManager.s_AndInds.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NOT, logicStates.TRUE)] = KeyLogicManager.s_NotTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NOT, logicStates.FALSE)] = KeyLogicManager.s_NotFalses.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NOT, logicStates.INDETERMINATE)] = KeyLogicManager.s_NotInds.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NONE, logicStates.TRUE)] = KeyLogicManager.s_NoneTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NONE, logicStates.FALSE)] = KeyLogicManager.s_NoneTrues.ToList();
                    KeyLogicManager.m_evalDict[Tuple.Create(logics.NONE, logicStates.INDETERMINATE)] = KeyLogicManager.s_NoneInds.ToList();
                    //set the lists in the method evaluation dictionary
                    //method evaluation dictionary xor
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.XOR, methods.DEACTIVATE, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.XOR, methods.DEACTIVATE, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.XOR, methods.DEACTIVATE, logicStates.INDETERMINATE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.XOR, methods.ACTIVATE, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.XOR, methods.ACTIVATE, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.XOR, methods.ACTIVATE, logicStates.INDETERMINATE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.XOR, methods.FOLLOW, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.XOR, methods.FOLLOW, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.XOR, methods.FOLLOW, logicStates.INDETERMINATE)] = null;
                    //method evaluation dictionary left
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.LEFT, methods.DEACTIVATE, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.LEFT, methods.DEACTIVATE, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.LEFT, methods.DEACTIVATE, logicStates.INDETERMINATE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.LEFT, methods.ACTIVATE, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.LEFT, methods.ACTIVATE, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.LEFT, methods.ACTIVATE, logicStates.INDETERMINATE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.LEFT, methods.FOLLOW, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.LEFT, methods.FOLLOW, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.LEFT, methods.FOLLOW, logicStates.INDETERMINATE)] = null;
                    //method evaluation dictionary right
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.RIGHT, methods.DEACTIVATE, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.RIGHT, methods.DEACTIVATE, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.RIGHT, methods.DEACTIVATE, logicStates.INDETERMINATE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.RIGHT, methods.ACTIVATE, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.RIGHT, methods.ACTIVATE, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.RIGHT, methods.ACTIVATE, logicStates.INDETERMINATE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.RIGHT, methods.FOLLOW, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.RIGHT, methods.FOLLOW, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.RIGHT, methods.FOLLOW, logicStates.INDETERMINATE)] = null;
                    //method evaluation dictionary bi
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.BI, methods.DEACTIVATE, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.BI, methods.DEACTIVATE, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.BI, methods.DEACTIVATE, logicStates.INDETERMINATE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.BI, methods.ACTIVATE, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.BI, methods.ACTIVATE, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.BI, methods.ACTIVATE, logicStates.INDETERMINATE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.BI, methods.FOLLOW, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.BI, methods.FOLLOW, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.BI, methods.FOLLOW, logicStates.INDETERMINATE)] = null;
                    //method evaluation dictionary and
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.DEACTIVATE, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.DEACTIVATE, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.DEACTIVATE, logicStates.INDETERMINATE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.ACTIVATE, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.ACTIVATE, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.ACTIVATE, logicStates.INDETERMINATE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.FOLLOW, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.FOLLOW, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.FOLLOW, logicStates.INDETERMINATE)] = null;
                    //method evaluation dictionary not
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.DEACTIVATE, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.DEACTIVATE, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.DEACTIVATE, logicStates.INDETERMINATE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.ACTIVATE, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.ACTIVATE, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.ACTIVATE, logicStates.INDETERMINATE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.FOLLOW, logicStates.TRUE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.FOLLOW, logicStates.FALSE)] = null;
                    KeyLogicManager.m_methodEvalDict[Tuple.Create(logics.AND, methods.FOLLOW, logicStates.INDETERMINATE)] = null;
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
                        Console.WriteLine("dictionaries missing logic type");
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

                public static List<Tuple<logicStates, logicStates>> qualifiers(logics ltype, logicStates stype)
                {
                    if (!KeyLogicManager.inited)
                    {
                        KeyLogicManager.init();
                    }
                    Tuple<logics, logicStates> key = Tuple.Create(ltype, stype);
                    if (KeyLogicManager.m_evalDict.ContainsKey(key))
                    {
                        return KeyLogicManager.m_evalDict[key];
                    }
                    else
                    {
                        return null;
                    }
                }

                public static List<Tuple<Tuple<ILogicState, logicStates>, Tuple<ILogicState, logicStates>>> relevants (Dictionary<ILogicState, logicStates> dict, logics ltype, methods mtype, logicStates stype)
                {
                    if (!KeyLogicManager.m_methodDict.ContainsKey(ltype))
                    {
                        return new List<Tuple<Tuple<ILogicState, logicStates>, Tuple<ILogicState, logicStates>>>();
                    }
                    Tuple<logics, methods, logicStates> key = new Tuple<logics,methods,logicStates>(ltype, mtype, stype);
                    if (!KeyLogicManager.m_methodEvalDict.ContainsKey(key))
                    {
                        return new List<Tuple<Tuple<ILogicState, logicStates>, Tuple<ILogicState, logicStates>>>();
                    }
                    List<Tuple<Tuple<logicStates, logicStates>, Tuple<logicStates, logicStates>>> compareList = KeyLogicManager.m_methodEvalDict[key];
                    return new List<Tuple<Tuple<ILogicState, logicStates>, Tuple<ILogicState, logicStates>>>();
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
