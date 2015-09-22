using System;

using CSim.Core.Types;
using CSim.Core.Variables;
using CSim.Core.Exceptions;
using CSim.Core.Literals;

namespace CSim.Core
{
    public abstract class Opcode
    {
        protected Opcode(Machine m)
        {
            this.Machine = m;
        }

        public abstract Variable Execute();

        /// <summary>
        /// Gets or sets the machine this opcode is going to be executed.
        /// </summary>
        /// <value>The machine.</value>
        public Machine Machine {
            get; set;
        }
    }
}

