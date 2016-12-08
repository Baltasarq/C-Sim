
namespace CSim.Core {
	using System;
	using CSim.Core;
	using CSim.Core.Variables;

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

		/// <summary>
		/// Coerce the specified vble to type t, by creating another vble.
		/// </summary>
		/// <param name="t">The type to coerce the value, as Type.</param>
		/// <param name="vble">A Variable which is to be coerced.</param>
		/// <returns>>A new variable with coerced type, or the same variable.</returns>
		public static Variable Coerce(CSim.Core.Type t, Variable vble)
		{
			Variable toret = vble;
			Id id = new Id( "a" );

			id.SetIdWithoutChecks( "coerced_" + vble.Name.Name );

			if ( t != vble.Type ) {
				toret = new Variable( id, t, vble.Machine );
				toret.Address = vble.Address;
			}		

			return toret;
		}
    }
}

