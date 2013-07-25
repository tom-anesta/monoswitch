using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using monoswitch.containers;

namespace monoswitch
{
    public class KeyDelegator
    {

        #region members_memberlike_properties

            #region public

            #endregion

            #region internal

            #endregion

            #region protected

                protected Dictionary<Keys, KeyPair> m_dict;

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
                    public KeyDelegator()
                    {
                        this.m_dict = new Dictionary<Keys, KeyPair>();
                        foreach(Keys key in Enum.GetValues(typeof(Keys)).Cast<Keys>())
                        {
                            this.m_dict[key] = new KeyPair(key, KeyState.Up);
                        }
                    }

                    public KeyPair key(Keys kVal)
                    {
                        return this.m_dict[kVal];
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
