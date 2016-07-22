
namespace CSim.Core {
	using System;
	using CSim.Core;

    public abstract class Opcode {
        protected Opcode(Machine m) {
            this.Machine = m;
        }

        public abstract void Execute();

        /// <summary>
        /// Gets or sets the machine this opcode is going to be executed.
        /// </summary>
        /// <value>The machine.</value>
        public Machine Machine {
            get; set;
        }
    }
}

