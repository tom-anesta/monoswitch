using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace monoswitch
{
    public interface ILogicState
    {
        logicStates state
        {
            get;
        }
        logicStates lastState
        {
            get;
        }
    }
}
