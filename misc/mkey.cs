using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace monoswitch.misc
{
    public class mkey : IKeyObject
    {
        //members
        protected MouseButton m_obj;
        protected Type m_type = typeof(MouseButton);
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
        public mkey(MouseButton obj)
        {
            this.m_obj = obj;
            this.m_state = genericStates.UP;
        }


    }
}
