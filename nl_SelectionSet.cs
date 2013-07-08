using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using Microsoft.Xna.Framework.Input;
using Ruminate.GUI;
using Ruminate.GUI.Content;
using Ruminate.GUI.Framework;
using monoswitch.content;

namespace monoswitch
{
    public class nl_SelectionSet: selectionSet
    {
        #region members

            #region public

            //delegates

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region methods

            #region public

            //CONSTRUCTOR

            //INITIALIZATION FUNCTIONS

            //UPDATE         

            //OTHER PUBLIC FUNCTIONS

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region events

            #region public

                public nl_SelectionSet(Game p_game, Skin p_defaultSkin, Text p_defaultText, Keys p_activatorKey, KeyState p_akstate, IEnumerable<Tuple<string, Skin>> p_skins = null, IEnumerable<Tuple<string, Text>> p_textRenderers = null)
                    : base(p_game, p_defaultSkin, p_defaultText, p_activatorKey, p_akstate, p_skins, p_textRenderers)
                {

                }

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

    }
}
