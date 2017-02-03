
namespace CSim.Core {
	/// <summary>
	/// Represents the opcodes of the virtual machine.
	/// </summary>
    public abstract class Opcode {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Opcode"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this opcode is going to be executed in.</param>
        protected Opcode(Machine m) {
            this.Machine = m;
        }

		/// <summary>
		/// Execute this opcode.
		/// </summary>
        public abstract void Execute();

        /// <summary>
        /// Gets or sets the machine this opcode is going to be executed in.
        /// </summary>
		/// <value>The <see cref="Machine"/>.</value>
        public Machine Machine {
            get; set;
        }
    }
}
