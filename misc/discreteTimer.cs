using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using monoswitch;
using monoswitch.containers;

namespace monoswitch.misc
{
    public delegate void keyAlert(discreteTimer kp);

    public class discreteTimer
    {

        #region members_memberlike_properties

            #region public

            #endregion

            #region internal

            #endregion

            #region protected

                protected KeyPair m_pair;
                protected TimeSpan m_timer;
                protected bool m_running = false;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                public bool running
                {
                    get
                    {
                        return this.m_running;
                    }
                    set
                    {
                        this.m_running = value;
                    }
                }

                public KeyPair pair
                {
                    get
                    {
                        return this.m_pair;
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

                public event keyAlert timerUp;

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

                public discreteTimer(KeyPair kVal, int ticks)
                {
                    this.m_pair = kVal;
                    this.m_timer = new TimeSpan(ticks);
                    this.m_running = false;
                }

                public void UpdateByTime(TimeSpan nTime)
                {
                    if (this.m_timer > TimeSpan.Zero && this.m_running)
                    {
                        this.m_timer -= nTime;
                        if (this.m_timer <= TimeSpan.Zero)
                        {
                            //throw event
                            if (this.timerUp != null)
                            {
                                this.timerUp(this);
                            }
                            this.m_running = false;
                        }
                    }
                }

                public void topOff(int ticks)
                {
                    if (this.m_timer.Ticks >= ticks)
                    {
                        this.m_timer = new TimeSpan(ticks);
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
