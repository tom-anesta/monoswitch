using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace monoswitchExample
{
    public class gameParams
    {

        #region members_memberlike_properties

            #region public
                //one ms is 10,000 ticks
                //selection set time vars
                public static readonly int DEF_MIN_SCAN_RATE = 166667;//under 1 frame at 60fps
                public static readonly int DEF_MAX_SCAN_RATE = 50000100;//5 seconds
                public static readonly int DEF_SCAN_RATE = 6666680;//almost 2/3 of a second
                public static readonly int DEF_MIN_REFACTORY_PERIOD = 100000;//1/100th of second
                public static readonly int DEF_MAX_REFACTORY_PERIOD = 5000000;//1/2 a second
                public static readonly int DEF_REFACTORY_PERIOD = 1000000;//1/10th of second
                public static readonly int DEF_MIN_DISCRETE_HOLD = 250000;//quarter second
                public static readonly int DEF_MAX_DISCRETE_HOLD = 60000000;//1 minute
                public static readonly int DEF_DISCRETE_HOLD = 1250000;//1.25 seconds
                //selection set type vars
                public static readonly bool DEF_DISCRETE = false;
                public static readonly bool DEF_COMPOSITE = false;
                public static readonly bool DEF_MARKER = false;
                //game goal vars
                public static readonly int DEF_MIN_GAME_TIME = 30;//.5min
                public static readonly int DEF_MAX_GAME_TIME = 240;//4 min
                public static readonly int DEF_GAME_TIME = 90;//1.5 min
                public static readonly int DEF_MIN_GOAL = 3;
                public static readonly int DEF_MAX_GOAL = 24;
                public static readonly int DEF_GOAL = 9;

            #endregion

            #region internal

            #endregion

            #region protected

                protected int m_scanRate;
                protected int m_refactoryPeriod;
                protected int m_discreteHold;
                protected int m_gameTime;
                protected int m_goal;
                protected bool m_discrete;
                protected bool m_composite;
                protected bool m_useMarker;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                public int scanRate
                {
                    get
                    {
                        return this.m_scanRate;
                    }
                }

                public int refactoryPeriod
                {
                    get
                    {
                        return this.m_refactoryPeriod;
                    }
                }

                public int discreteHold
                {
                    get
                    {
                        return this.m_discreteHold;
                    }
                }

                public int gameTime
                {
                    get
                    {
                        return this.m_gameTime;
                    }
                }

                public int goal
                {
                    get
                    {
                        return this.m_goal;
                    }
                }

                public bool discrete
                {
                    get
                    {
                        return this.m_discrete;
                    }
                    set
                    {
                        this.m_discrete = value;
                    }
                }

                public bool composite
                {
                    get
                    {
                        return this.m_composite;
                    }
                    set
                    {
                        this.m_composite = value;
                    }
                }

                public bool useMarker
                {
                    get
                    {
                        return this.m_useMarker;
                    }
                    set
                    {
                        this.m_useMarker = value;
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

                //constructor

                public gameParams(int scanR, int refPer, int dischd, int gt, int gl, bool disc, bool comp, bool mark)
                {
                    this.m_composite = comp;
                    this.m_discrete = disc;
                    this.m_useMarker = mark;
                    if (scanR < gameParams.DEF_MIN_SCAN_RATE)
                    {
                        this.m_scanRate = gameParams.DEF_MIN_SCAN_RATE;
                    }
                    else if (scanR > gameParams.DEF_MAX_SCAN_RATE)
                    {
                        this.m_scanRate = gameParams.DEF_MAX_SCAN_RATE;
                    }
                    else
                    {
                        this.m_scanRate = scanR - (scanR % gameParams.DEF_MIN_SCAN_RATE);
                    }
                    if (refPer < gameParams.DEF_MIN_REFACTORY_PERIOD)
                    {
                        this.m_refactoryPeriod = gameParams.DEF_MIN_REFACTORY_PERIOD;
                    }
                    else if (refPer > gameParams.DEF_MAX_REFACTORY_PERIOD)
                    {
                        this.m_refactoryPeriod = gameParams.DEF_MAX_REFACTORY_PERIOD;
                    }
                    else
                    {
                        this.m_refactoryPeriod = (refPer - (refPer % gameParams.DEF_MIN_REFACTORY_PERIOD));
                    }
                    if (dischd < gameParams.DEF_MIN_DISCRETE_HOLD)
                    {
                        this.m_discreteHold = gameParams.DEF_MIN_DISCRETE_HOLD;
                    }
                    else if (dischd > gameParams.DEF_MAX_DISCRETE_HOLD)
                    {
                        this.m_discreteHold = gameParams.DEF_MAX_DISCRETE_HOLD;
                    }
                    else
                    {
                        dischd -= gameParams.DEF_MIN_DISCRETE_HOLD;
                        this.m_refactoryPeriod = dischd - (dischd % gameParams.DEF_MIN_REFACTORY_PERIOD);
                        this.m_refactoryPeriod += DEF_MIN_DISCRETE_HOLD;
                    }
                    if (gt < gameParams.DEF_MIN_GAME_TIME)
                    {
                        this.m_gameTime = gameParams.DEF_MIN_GAME_TIME;
                    }
                    else if (gt > gameParams.DEF_MAX_GAME_TIME)
                    {
                        this.m_gameTime = gameParams.DEF_MAX_GAME_TIME;
                    }
                    else
                    {
                        this.m_gameTime = gt - gt % 5;
                    }
                    //if (gl < gameParams.DEF_MIN_GOAL)
                    //{

                    //}

                }

                public void incrementScanningRate()
                {
                    if (this.m_scanRate < gameParams.DEF_MAX_SCAN_RATE)
                    {
                        this.m_scanRate = Math.Min(this.m_scanRate + gameParams.DEF_MIN_SCAN_RATE, gameParams.DEF_MAX_SCAN_RATE);
                    }
                }

                public void decrementScanningRate()
                {
                    if (this.m_scanRate > gameParams.DEF_MIN_SCAN_RATE)
                    {
                        this.m_scanRate = Math.Max(this.m_scanRate - gameParams.DEF_MIN_SCAN_RATE, gameParams.DEF_MIN_SCAN_RATE);
                    }
                }

                public void incrementRefactoryPeriod()
                {
                    if (this.m_refactoryPeriod < gameParams.DEF_MAX_REFACTORY_PERIOD)
                    {
                        this.m_refactoryPeriod = Math.Min(this.m_refactoryPeriod + gameParams.DEF_MIN_REFACTORY_PERIOD, gameParams.DEF_MAX_SCAN_RATE);
                    }
                }

                public void decrementRefactoryPeriod()
                {
                    if (this.m_refactoryPeriod > gameParams.DEF_MIN_REFACTORY_PERIOD)
                    {
                        this.m_refactoryPeriod = Math.Max(this.m_refactoryPeriod - gameParams.DEF_MIN_REFACTORY_PERIOD, gameParams.DEF_MIN_REFACTORY_PERIOD);
                    }
                }

                public void incrementDiscreteHold()
                {
                    if (this.m_discreteHold < gameParams.DEF_MAX_DISCRETE_HOLD)
                    {
                        this.m_discreteHold = Math.Min(this.m_discreteHold + gameParams.DEF_MIN_REFACTORY_PERIOD, gameParams.DEF_MAX_DISCRETE_HOLD);//don't want to use something different for hold
                    }
                }

                public void decrementDiscreteHold()
                {
                    if (this.m_discreteHold > gameParams.DEF_MIN_DISCRETE_HOLD)
                    {
                        this.m_discreteHold = Math.Min(this.m_discreteHold - gameParams.DEF_MIN_REFACTORY_PERIOD, gameParams.DEF_MIN_DISCRETE_HOLD);
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

        #region methods

            #region public

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
