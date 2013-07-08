using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ruminate.GUI.Framework;
using Ruminate.GUI.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using Microsoft.Xna.Framework.Input;
using monoswitch.content;

namespace monoswitch
{
    public class selectionSet : Gui
    {

        #region members

            #region public

                //static const
                public const int DEFAULT_SCANNINGRATE = 666;//2/3 of a second
                public const int DEFAULT_MIN_SCANNINGRATE = 20;//minimum 1/50th of a second
                public const int DEFAULT_PREF_BEGINNINGS = 1;//the default number of preferred beginnings
                public const int DEFAULT_PREF_ENDINGS = 1;//the default number of preferred endings
                public const int DEFAULT_MAX_WIDTH = 300;//the default maximum width
                public const int DEFAULT_MAX_HEIGHT = 300;//the default maximum height

            #endregion

            #region protected

                //time related
                protected TimeSpan m_scanningRate;
                protected TimeSpan m_timeToAdvance;
                protected TimeSpan m_refractoryPeriod;
                protected TimeSpan m_timeToRefract;
                protected int m_cutoffTime;//the time in milliseconds that timeToAdvance was stopped at
                //render and layout related
                protected int? m_maxWidth;
                protected int? m_maxHeight;
                //initialize related
                protected List<switchNode> m_nodes;//a list of all nodes contained in the selection set
                protected List<switchNode> m_beginnings;//a list of all nodes that can serve as valid starting points for the selection set
                protected List<switchNode> m_endings;//a list of all nodes that can serve as valid starting points for the selection set
                protected Boolean m_commited;
                //display and control related
                protected switchNode m_startingNode;//the node to start/restart the selection set at
                protected switchNode m_currentNode;//the current node that will be selected or deselected on input
                protected Keys m_signalKey;
                protected KeyState m_signalState;
                //the input manager
                //elements
                protected Panel m_panel;
                protected ScrollBars m_scroller;

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

                public Boolean commited
                {
                    get
                    {
                        return this.m_commited;
                    }
                }

                public TimeSpan scanningRate
                {
                    get
                    {
                        return new TimeSpan(this.m_scanningRate.Ticks);
                    }
                    set
                    {
                        if (!this.m_commited)
                        {
                                this.m_scanningRate = new TimeSpan(value.Ticks);
                        }
                        return;
                    }
                }

                public Boolean running
                {
                    get
                    {
                        return this.m_timeToAdvance != new TimeSpan(0, 0, 0, 0, 0);
                    }
                }

                public TimeSpan timeToAdvance//the time until the next advance
                {
                    get
                    {
                        return new TimeSpan(this.m_timeToAdvance.Ticks);
                    }
                }

                public int? maxWidth
                {
                    get
                    {
                        return this.m_maxWidth;
                    }
                    set
                    {
                        if (value.HasValue)
                        {
                            this.m_maxWidth = (int)Math.Abs((float)value);
                        }
                        else
                        {
                            this.m_maxWidth = value;
                        }
                    }
                }

                public int? maxHeight
                {
                    get
                    {
                        return this.m_maxHeight;
                    }
                    set
                    {
                        if (value.HasValue)
                        {
                            this.m_maxHeight = (int)Math.Abs((float)value);
                        }
                        else
                        {
                            this.m_maxHeight = value;
                        }
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

                //delegates
                //public void
                public void respondKeyDown(Object o, KeyEventArgs e)
                {
                }

                public void respondKeyUp(Object o, KeyEventArgs e)
                {

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

                //CONSTRUCTOR

                public selectionSet(Game p_game, Skin p_defaultSkin, Text p_defaultText, IEnumerable<Tuple<string, Skin>> p_skins = null, IEnumerable<Tuple<string, Text>> p_textRenderers = null)
                    : base(p_game, p_defaultSkin, p_defaultText, p_skins, p_textRenderers)
                {
                    this.m_scanningRate = new TimeSpan(0, 0, 0, 0, selectionSet.DEFAULT_SCANNINGRATE);
                    this.m_timeToAdvance = new TimeSpan(0, 0, 0, 0, 0);
                    this.m_cutoffTime = 0;
                    this.m_refractoryPeriod = new TimeSpan(0, 0, 0, 0, 0);
                    this.m_timeToRefract = new TimeSpan(0, 0, 0, 0, 0);
                    this.m_beginnings = new List<switchNode>();
                    this.m_endings = new List<switchNode>();
                    this.m_commited = false;
                    this.m_startingNode = null;
                    this.m_currentNode = null;
                    this.m_panel = null;
                    this.m_scroller = null;
                    this.m_signalKey = Keys.F9;
                    this.m_signalState = KeyState.Down;
                }

                public selectionSet(Game p_game, Skin p_defaultSkin, Text p_defaultText, TimeSpan p_scanR, IEnumerable<Tuple<string, Skin>> p_skins = null, IEnumerable<Tuple<string, Text>> p_textRenderers = null)
                    : base(p_game, p_defaultSkin, p_defaultText, p_skins, p_textRenderers)
                {
                    
                    if((int)p_scanR.TotalMilliseconds >= selectionSet.DEFAULT_MIN_SCANNINGRATE)
                    {
                        this.m_scanningRate = p_scanR;
                    }
                    else
                    {
                        this.m_scanningRate = new TimeSpan(0, 0, 0, 0, selectionSet.DEFAULT_SCANNINGRATE);
                    }
                    this.m_timeToAdvance = new TimeSpan(0, 0, 0, 0, 0);
                    this.m_cutoffTime = 0;
                    this.m_refractoryPeriod = new TimeSpan(0, 0, 0, 0, 0);
                    this.m_timeToRefract = new TimeSpan(0, 0, 0, 0, 0);
                    this.m_beginnings = new List<switchNode>();
                    this.m_endings = new List<switchNode>();
                    this.m_commited = false;
                    this.m_startingNode = null;
                    this.m_currentNode = null;
                    this.m_panel = null;
                    this.m_scroller = null;
                    this.m_maxHeight = null;
                    this.m_maxWidth = null;
                    this.m_signalKey = Keys.F9;
                    this.m_signalState = KeyState.Down;
                }

                public selectionSet(Game p_game, Skin p_defaultSkin, Text p_defaultText, int p_scanR, IEnumerable<Tuple<string, Skin>> p_skins = null, IEnumerable<Tuple<string, Text>> p_textRenderers = null)
                    : base(p_game, p_defaultSkin, p_defaultText, p_skins, p_textRenderers)
                {
                    if (p_scanR >= selectionSet.DEFAULT_MIN_SCANNINGRATE)
                    {
                        this.m_scanningRate = new TimeSpan(0, 0, 0, 0, p_scanR);
                    }
                    this.m_timeToAdvance = new TimeSpan(0, 0, 0, 0, 0);
                    this.m_cutoffTime = 0;
                    this.m_beginnings = new List<switchNode>();
                    this.m_endings = new List<switchNode>();
                    this.m_commited = false;
                    this.m_currentNode = null;
                    this.m_startingNode = null;
                    this.m_panel = null;
                    this.m_scroller = null;
                    this.m_maxHeight = null;
                    this.m_maxWidth = null;
                    this.m_signalKey = Keys.F9;
                    this.m_signalState = KeyState.Down;
                }

                public selectionSet(Game p_game, Skin p_defaultSkin, Text p_defaultText, IEnumerable<switchNode> p_intendedNodes, int p_scanR = 0, IEnumerable<Tuple<string, Skin>> p_skins = null, IEnumerable<Tuple<string, Text>> p_textRenderers = null)
                    : base(p_game, p_defaultSkin, p_defaultText, p_skins, p_textRenderers)
                {
                    switchNode[] arr = p_intendedNodes.ToArray();
                    delayedArrayConstructor(arr, p_scanR);
                }

                public selectionSet(Game p_game, Skin p_defaultSkin, Text p_defaultText, IList<switchNode> p_intendedNodes, int p_scanR = 0, IEnumerable<Tuple<string, Skin>> p_skins = null, IEnumerable<Tuple<string, Text>> p_textRenderers = null)
                    : base(p_game, p_defaultSkin, p_defaultText, p_skins, p_textRenderers)
                {
                    switchNode[] arr = p_intendedNodes.ToArray();
                    delayedArrayConstructor(arr, p_scanR);
                }

                public selectionSet(Game p_game, Skin p_defaultSkin, Text p_defaultText, IEnumerable<switchNode> p_intendedNodes, TimeSpan p_scanR, IEnumerable<Tuple<string, Skin>> p_skins = null, IEnumerable<Tuple<string, Text>> p_textRenderers = null)
                    : base(p_game, p_defaultSkin, p_defaultText, p_skins, p_textRenderers)
                {
                    switchNode[] arr = p_intendedNodes.ToArray();
                    delayedArrayConstructor(arr, p_scanR);
                }

                public selectionSet(Game p_game, Skin p_defaultSkin, Text p_defaultText, IList<switchNode> p_intendedNodes, TimeSpan p_scanR, IEnumerable<Tuple<string, Skin>> p_skins = null, IEnumerable<Tuple<string, Text>> p_textRenderers = null)
                    : base(p_game, p_defaultSkin, p_defaultText, p_skins, p_textRenderers)
                {
                    switchNode[] arr = p_intendedNodes.ToArray();
                    delayedArrayConstructor(arr, p_scanR);
                }

                public selectionSet( Game p_game, Skin p_defaultSkin, Text p_defaultText, switchNode[] p_intendedNodes, int p_scanR, IEnumerable<Tuple<string, Skin>> p_skins = null, IEnumerable<Tuple<string, Text>> p_textRenderers = null )
                    : base(p_game, p_defaultSkin, p_defaultText, p_skins, p_textRenderers)
                {
                    if (p_scanR >= selectionSet.DEFAULT_MIN_SCANNINGRATE)
                    {
                        this.m_scanningRate = new TimeSpan(0, 0, 0, 0, p_scanR);
                    }
                    this.m_timeToAdvance = new TimeSpan(0, 0, 0, 0, 0);
                    this.m_cutoffTime = 0;
                    this.m_refractoryPeriod = new TimeSpan(0, 0, 0, 0, 0);
                    this.m_timeToRefract = new TimeSpan(0, 0, 0, 0, 0);
                    this.m_beginnings = new List<switchNode>();
                    this.m_endings = new List<switchNode>();
                    if (this.isValidList(p_intendedNodes))
                    {
                        this.m_beginnings.Add(p_intendedNodes[0]);
                        this.m_endings.Add(p_intendedNodes[p_intendedNodes.Length - 1]);
                        this.m_startingNode = this.m_beginnings[0];
                        this.m_currentNode = m_startingNode;
                    }
                    else
                    {
                        this.m_startingNode = null;
                        this.m_currentNode = null;
                    }
                    this.m_panel = null;
                    this.m_scroller = null;
                    this.m_maxHeight = null;
                    this.m_maxWidth = null;
                    this.m_commited = false;
                    this.m_signalKey = Keys.F9;
                    this.m_signalState = KeyState.Down;
                }

                public selectionSet( Game p_game, Skin p_defaultSkin, Text p_defaultText, switchNode[] p_intendedNodes, TimeSpan p_scanR, IEnumerable<Tuple<string, Skin>> p_skins = null, IEnumerable<Tuple<string, Text>> p_textRenderers = null)
                    : base(p_game, p_defaultSkin, p_defaultText, p_skins, p_textRenderers)
                {
                    if ((int)p_scanR.TotalMilliseconds >= selectionSet.DEFAULT_MIN_SCANNINGRATE)
                    {
                        this.m_scanningRate = p_scanR;
                    }
                    this.m_timeToAdvance = new TimeSpan(0, 0, 0, 0, 0);
                    this.m_cutoffTime = 0;
                    this.m_refractoryPeriod = new TimeSpan(0, 0, 0, 0, 0);
                    this.m_timeToRefract = new TimeSpan(0, 0, 0, 0, 0);
                    this.m_beginnings = new List<switchNode>();
                    this.m_endings = new List<switchNode>();
                    if (this.isValidList(p_intendedNodes))
                    {
                        this.m_beginnings.Add(p_intendedNodes[0]);
                        this.m_endings.Add(p_intendedNodes[p_intendedNodes.Length - 1]);
                        this.m_startingNode = this.m_beginnings[0];
                        this.m_currentNode = m_startingNode;
                    }
                    else
                    {
                        this.m_startingNode = null;
                        this.m_currentNode = null;
                    }
                    this.m_panel = null;
                    this.m_scroller = null;
                    this.m_maxHeight = null;
                    this.m_maxWidth = null;
                    this.m_commited = false;
                    this.m_signalKey = Keys.F9;
                    this.m_signalState = KeyState.Down;
                }

                //INITIALIZE

                public virtual bool Commit(int p_x, int p_y, int? p_width = null, int? p_height = null)
                {
                    int act_width = 0;
                    int act_height = 0;
                    bool widthAssigned = false;
                    bool heightAssigned= false;
                    if (this.m_commited)
                    {
                        return true;
                    }
                    switchNode[] itemsToCommit = nodes_From_BandE(this.m_beginnings, this.m_endings);
                    if (itemsToCommit == null)
                    {
                        this.m_commited = false;
                        return false;
                    }
                    Widget[] itemsToAdd = itemsToCommit.Select(x=>x.child).ToArray();
                    if (p_width.HasValue)
                    {
                        act_width = Math.Abs((int)p_width);
                        widthAssigned = true;
                    }
                    else
                    {
                        act_width = width_from_widgets(itemsToAdd);
                    }

                    this.m_panel = new Panel(p_x, p_y, Math.Min(act_width, ((int)(this.m_maxWidth.HasValue ? this.m_maxWidth : int.MaxValue)) ), Math.Min(act_height, ((int)(this.m_maxHeight.HasValue ? this.m_maxHeight : int.MaxValue))) );
                    if (!widthAssigned && !heightAssigned)
                    {//size should be fine? debug later
                        this.m_panel.Children = itemsToAdd;
                    }
                    else
                    {
                        this.m_scroller = new ScrollBars();
                        this.m_scroller.Children = itemsToAdd;
                        this.m_panel.Children = new Widget[] { this.m_scroller };
                    }
                    this.Widgets = new Widget[] { this.m_panel };
                    this.m_commited = true;
                    return true;
                }

                //UPDATE

                //OTHER PUBLIC FUNCTIONS

                public void Start()
                {
                    if (!this.m_commited)
                    {
                        return;
                    }
                    int pref_time = ((int)this.m_currentNode.child.scanningRate.TotalMilliseconds > selectionSet.DEFAULT_MIN_SCANNINGRATE) ? (int)this.m_currentNode.child.scanningRate.TotalMilliseconds : (int)this.m_scanningRate.TotalMilliseconds;
                    this.m_timeToAdvance = new TimeSpan(0, 0, 0, 0, pref_time);
                }

                public void Stop()
                {
                    if (!this.m_commited)
                    {
                        return;//can't stop if it's not going
                    }
                    this.m_cutoffTime = (int)this.m_timeToAdvance.TotalMilliseconds;
                    this.m_timeToAdvance = new TimeSpan(0, 0, 0, 0, 0);
                }
                public void Reset()
                {
                    if (!this.m_commited)
                    {
                        return;
                    }
                    this.m_currentNode = this.m_startingNode;
                    this.m_cutoffTime = 0;
                    this.m_timeToAdvance = new TimeSpan(0, 0, 0, 0, 0);
                }

                public void Restart()
                {
                    this.Reset();
                    this.Start();
                }

                //update functions
                public void UpdateByTime(GameTime gametime)
                {
                    //we don't have access to dom because it's private
                    Widget[] temps = this.Widgets;
                    foreach (Widget widge in temps)
                    {
                        widge.GetTreeNode().DfsOperation(node =>
                        {
                            if (!node.Data.Active || !(node.Data is s_switch) )
                            {
                                return;
                            }
                            (node.Data as s_switch).UpdateByTime(gametime);
                        });
                    }

                }
                

                //virtual functions

                public virtual bool isValidList(switchNode[] testList, int endStartsNI = 0, int beginEndsI = 0)//override in subclasses with graph algorithms
                {
                    if (testList == null || testList.Length < 1)
                    {
                        return false;
                    }
                    if (testList.Length == 1 && testList[0] != null)
                    {
                        switchNode tVal = testList[0];
                        if( tVal.successors.Count == 1 && tVal.predecessors.Count == 1 && tVal.successors[0] == tVal && testList[0].predecessors[0] == tVal)
                        {
                            return true;
                        }
                        return false;
                    }
                    List<switchNode> compList = new List<switchNode>(testList.Length);
                    switchNode s_root = testList[0];
                    switchNode s_last = s_root;
                    compList[0] = s_root;
                    int index = 1;
                    //can we iterate over?
                    while (s_root.successors.Count == 1 && s_root.predecessors.Count == 1 && ( (s_root = s_root.successors[0]).predecessors[0] == s_last && !(compList.Contains(s_root)) ) && index < testList.Length)
                    {
                        compList[index] = s_root;
                        s_last = s_root;
                        index++;
                    }//check to make sure it is full
                    if (compList[compList.Count - 1] == null || !(compList[compList.Count - 1] == testList[testList.Length - 1]))
                    {
                        return false;
                    }
                    //check if the one we couldn't enter was the one at the beginning
                    if (s_root == compList[0] && s_root.predecessors.Count == 1 && s_root.predecessors[0] == compList[compList.Count - 1])
                    {//check to make sure we only used items from the array
                        foreach (switchNode cVal in testList)
                        {
                            if (!compList.Contains(cVal))
                            {
                                return false;
                            }
                        }//now we're good to go
                        return true;
                    }
                    return false;
                }

                public virtual switchNode[] nodes_From_BandE(List<switchNode> begin, List<switchNode> end)//modify in inheriting classes
                {//like is valid list, but it takes the beginning and the end instead of starting with a list
                    if (begin == null || end == null || begin.Count < 1 || end.Count < 1 || begin[0] == null || end[0] == null || begin.Count > 1 || end.Count > 1)
                    {
                        return null;//error/default return;
                    }
                    //check your beginning and endings
                    switchNode beginner = begin[0];
                    switchNode last = beginner;
                    switchNode ender = end[0];
                    if(beginner.predecessors == null || ender.successors == null || beginner.predecessors.Count != 1 || ender.successors.Count != 1 || beginner.predecessors[0] != ender || ender.successors[0] != beginner)
                    {
                        return null;
                    }
                    List<switchNode> compList = new List<switchNode>();
                    while (!compList.Contains(beginner))
                    {
                        compList.Add(beginner);
                        if (beginner.successors.Count != 1 || beginner.successors[0] == null)
                        {
                            return null;
                        }
                        last = beginner;
                        beginner = beginner.successors[0];
                        if (beginner.predecessors.Count != 1 || beginner.predecessors[0] != last)
                        {
                            return null;
                        }
                    }//now that we've found one that compList contains, wehave to make sure it contains the end
                    if (!compList.Contains(ender))
                    {
                        return null;
                    }
                    return compList.ToArray();//error/default return
                }

                public virtual int width_from_widgets(Widget[] list)
                {
                    //find the lowest x and the highest x+width and get their difference
                    int min = int.MaxValue;
                    int max = int.MinValue;
                    foreach (Widget widge in list)
                    {
                        min = Math.Min(min, widge.AbsoluteArea.X);
                        max = Math.Max(max, widge.AbsoluteArea.X + widge.AbsoluteArea.Width);
                    }
                    min = Math.Min(min, 0);
                    return (int)Math.Abs(max-min);
                }

                public virtual int height_from_widgets(Widget[] list)
                {
                    int min = int.MaxValue;
                    int max = int.MinValue;
                    foreach (Widget widge in list)
                    {
                        min = Math.Min(min, widge.AbsoluteArea.Y);
                        max = Math.Max(max, widge.AbsoluteArea.Y + widge.AbsoluteArea.Height);
                    }
                    min = Math.Min(min, 0);
                    return (int)Math.Abs(max - min);
                }

            #endregion

            #region internal

            #endregion

            #region protected

                protected void delayedArrayConstructor(switchNode[] p_intendedNodes, int p_scanR)
                {
                    if (p_scanR >= selectionSet.DEFAULT_MIN_SCANNINGRATE)
                    {
                        this.m_scanningRate = new TimeSpan(0, 0, 0, 0, p_scanR);
                    }
                    this.m_beginnings = new List<switchNode>();
                    this.m_endings = new List<switchNode>();
                    if (this.isValidList(p_intendedNodes))
                    {
                        this.m_beginnings.Add(p_intendedNodes[0]);
                        this.m_endings.Add(p_intendedNodes[p_intendedNodes.Length - 1]);
                        this.m_startingNode = this.m_beginnings[0];
                        this.m_currentNode = m_startingNode;
                    }
                    else
                    {
                        this.m_startingNode = null;
                        this.m_currentNode = null;
                    }
                    this.m_maxHeight = null;
                    this.m_maxWidth = null;
                    this.m_commited = false;
                }

                protected void delayedArrayConstructor(switchNode[] p_intendedNodes, TimeSpan p_scanR)
                {
                    if (p_scanR.Milliseconds >= selectionSet.DEFAULT_MIN_SCANNINGRATE && p_scanR.Seconds == 0 && p_scanR.Minutes == 0 && p_scanR.Hours == 0 && p_scanR.Days == 0)
                    {
                        this.m_scanningRate = p_scanR;
                    }
                    this.m_beginnings = new List<switchNode>();
                    this.m_endings = new List<switchNode>();
                    if (this.isValidList(p_intendedNodes))
                    {
                        this.m_beginnings.Add(p_intendedNodes[0]);
                        this.m_endings.Add(p_intendedNodes[p_intendedNodes.Length - 1]);
                        this.m_startingNode = this.m_beginnings[0];
                        this.m_currentNode = m_startingNode;
                    }
                    else
                    {
                        this.m_startingNode = null;
                        this.m_currentNode = null;
                    }
                    this.m_maxHeight = null;
                    this.m_maxWidth = null;
                    this.m_commited = false;
                }

            #endregion

            #region private

            #endregion

        #endregion

       

    }
}
