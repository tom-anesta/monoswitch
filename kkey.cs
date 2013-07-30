using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace monoswitch
{
    public class kkey : IKeyObject
    {
        //members
        protected Keys m_obj;
        protected Type m_type = typeof(Keys);
        protected genericStates m_state;

        //properties
        public Object key
        {
            get
            {
                return this.m_obj as Object;
            }
        }
        public Type keyType
        {
            get
            {
                return this.m_type;
            }
        }
        public genericStates state
        {
            get
            {
                return this.m_state;
            }
            set
            {
                this.m_state = value;
            }
        }

        //methods
        public kkey(Keys obj)
        {
            this.m_obj = obj;
            this.m_state = genericStates.UP;
        }

    }
}
