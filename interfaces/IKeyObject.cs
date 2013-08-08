using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace monoswitch
{
    public enum genericStates
    {
        UP, DOWN
    }

    public interface IKeyObject
    {
        Type keyType
        {
            get;
        }
        Object key
        {
            get;
        }
        genericStates state
        {
            get;
            set;
        }
    }
}
