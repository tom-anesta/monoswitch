using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ruminate.GUI.Content;
using Ruminate.GUI.Framework;
using Ruminate.DataStructures;
using monoswitch.content;

namespace monoswitch
{
    //this class is a replication of the ruminate input manager class.  
    //the input manager class automatically focused and selected widged based on mouse movements.  we want to work based on key presses
    public class msInputManager
    {

        #region members_memberlike_properties

            #region public

            #endregion

            #region internal

            #endregion

            #region protected

            #endregion

            #region private

                private readonly Root<Widget> _dom;
                // When ever these Widgets are not not null they are in the state
                // specified by their names. Used to trigger events and by the widget
                // class to see what state the widget is in. 
                private s_switch _hoverWidget;
                //private Widget _pressedWidget;
                //private Widget _focusedWidget;

            #endregion

        #endregion

        #region properties

            #region public

            #endregion

            #region internal

                public s_switch HoverWidget
                {
                    get
                    {
                        return _hoverWidget;
                    }
                    set
                    {
                        if (value == _hoverWidget)
                        {
                            return;
                        }
                        if (value != null)
                        {
                            value.EnterHover();
                        }
                        if (_hoverWidget != null)
                        {
                            _hoverWidget.ExitHover();
                        }
                        _hoverWidget = value;
                    }
                }
                
                /*
                internal Widget PressedWidget
                {
                    get
                    {
                        return _pressedWidget;
                    }
                    private set
                    {
                        if (value == _pressedWidget)
                        {
                            return;
                        }
                        if (value != null)
                        {
                            value.EnterPressed();
                        }
                        if (_pressedWidget != null)
                        {
                            _pressedWidget.ExitPressed();
                        }
                        _pressedWidget = value;
                    }
                }

                
                internal Widget FocusedWidget
                {
                    get
                    {
                        return _focusedWidget;
                    }
                    private set
                    {
                        if (value == _focusedWidget)
                        {
                            return;
                        }
                        if (value != null)
                        {
                            value.EnterFocus();
                        }
                        if (_focusedWidget != null)
                        {
                            _focusedWidget.ExitFocus();
                        }
                        _focusedWidget = value;
                    }
                }  
                */

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

            #endregion

            #region internal

        // Ties into the events in InputSystem. Top events are asynchronous and
                // used for state management. The rest of them are synchronous and used to
                // trigger user specified event handlers.
                internal msInputManager(Game game, Root<Widget> dom)
                {
                    _dom = dom;
                    InputSystem.Initialize(game.Window);

                    /* ## Input Events to Manage Internal State ## */
                    //removing all code involving mousemove

                    /* ## Input Events to fire the focused widget's event handlers ## */
                    InputSystem.KeyDown += delegate(Object o, KeyEventArgs e)
                    {
                        if (HoverWidget != null)
                        {
                            HoverWidget.KeyDown(e);
                        }
                    };

                    InputSystem.KeyUp += delegate(Object o, KeyEventArgs e)
                    {
                        if (HoverWidget != null)
                        {
                            HoverWidget.KeyUp(e);
                        }
                    };
                }

            #endregion

            #region protected

            #endregion

            #region private

                //should not need this anymore
                /*
                // Finds the element the mouse is currently being hovered over.
                private Widget FindHover()
                {
                    _hover = null;
                    foreach (var child in _dom.Children)
                    {
                        DfsFindHover(child);
                    }
                    return _hover;
                }
                private void DfsFindHover(TreeNode<Widget> node)
                {

                    if (!node.Data.AbsoluteArea.Contains(InputSystem.MouseLocation))
                    {
                        return;
                    }
                    if (node.Parent.Data != null && !node.Parent.Data.AbsoluteInputArea.Contains(InputSystem.MouseLocation))
                    {
                        return;
                    }
                    if (!node.Data.Active || !node.Data.Visible)
                    {
                        return;
                    }
                    _hover = node.Data;
                    foreach (var child in node.Children)
                    {
                        DfsFindHover(child);
                    }
                }
                */
            #endregion

        #endregion

    }
}
