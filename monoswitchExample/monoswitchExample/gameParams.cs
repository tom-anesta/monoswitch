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

                public static readonly int DEF_MIN_SCAN_RATE = 16;//under 1 frame at 60fps
                public static readonly int DEF_MAX_SCAN_RATE = 4992;//5 seconds
                public static readonly int DEF_SCAN_RATE = 656;//almost 2/3 of a second
                public static readonly float DEF_MIN_REFACTORY_PERIOD = 10000;//1/100th of second
                public static readonly float DEF_MAX_REFACTORY_PERIOD = 500000;//1/2 a second
                public static readonly float DEF_REFACTORY_PERIOD = 100000;//1/10th of second
                public static readonly bool DEF_DISCRETE = false;
                public static readonly bool DEF_COMPOSITE = false;
                public static readonly bool DEF_MARKER = false;

            #endregion

            #region internal

            #endregion

            #region protected

                protected int m_scanRate;
                protected float m_refactoryPeriod;
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

                public float refactoryPeriod
                {
                    get
                    {
                        return this.m_refactoryPeriod;
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

                public gameParams(int scanR, float refPer, bool disc, bool comp, bool mark)
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
                        this.m_refactoryPeriod = (float)((int)refPer - ((int)refPer % (int)gameParams.DEF_MIN_REFACTORY_PERIOD));
                    }

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
