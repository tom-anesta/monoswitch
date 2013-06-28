#region File Description
//-----------------------------------------------------------------------------
// PlayerIndexEventArgs.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace monoswitchExample
{
    /// <summary>
    /// Custom event argument which includes the index of the player who
    /// triggered the event. This is used by the MenuEntry.Selected event.
    /// </summary>
    class PlayerIndexEventArgs : EventArgs
    {

        #region members

            #region public

            #endregion

            #region protected

                protected PlayerIndex m_playerIndex;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                /// <summary>
                /// Gets the index of the player who triggered this event.
                /// </summary>
                public PlayerIndex playerIndex
                {
                    get
                    {
                        return this.m_playerIndex;
                    }
                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region events

            #region public

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region functions

            #region public

                /// <summary>
                /// Constructor.
                /// </summary>
                public PlayerIndexEventArgs(PlayerIndex playerIndex)
                {
                    this.m_playerIndex = playerIndex;
                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion
       
    }
}
