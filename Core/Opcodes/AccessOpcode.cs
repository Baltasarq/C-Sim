using System;

namespace CSim.Core.Opcodes {
	using System;

	using CSim.Core.Variables;
	using CSim.Core.Types.Primitives;
	using CSim.Core.Literals;
	using CSim.Core.Types;
	using CSim.Core.Exceptions;

	/// <summary>
	/// Access opcode with '*', i.e. int * v = &x; => *x = 5;
	/// </summary>
	public class AccessOpcode: Opcode {
		/// <summary>The opcode id.</summary>
		public const char OpcodeValue = (char) 0xE0;

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Opcodes.AccessOpcode"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this opcode will be executed in.</param>
		/// <param name="levels">Number of indirection levels, i.e. **x, *X or ***x</param>
		public AccessOpcode(Machine m, int levels)
			:base(m)
		{
			this.Levels = levels;
		}

		/// <summary>
		/// Returns the a variable, accessing its address with '*'
		/// </summary>
		public override void Execute()
		{
			Variable vble = this.Machine.TDS.SolveToVariable( this.Machine.ExecutionStack.Pop() );

			if ( vble != null ) {
				for(int i = 0; i < this.Levels; ++i) {
					var vbleAsRef = vble as RefVariable;
					var vbleType = vble.Type as Ptr;

					// If the vble at the right is a reference, dereference it
					if ( vbleAsRef != null  ) {
						vble = vbleAsRef.PointedVble;
					}
					else
					if ( vbleType != null ) {
						long address = Convert.ToInt64( vble.LiteralValue.Value );
						vble = new InPlaceTempVariable( vbleType.DerreferencedType, this.Machine );
						vble.Address = address;
						this.Machine.TDS.AddVariableInPlace( vble );
					}
					else {
						throw new TypeMismatchException( vble.ToString() );
					}
				}

				// Store in the temp vble and end
				this.Machine.ExecutionStack.Push( vble );
			} else {
				throw new EngineException( "invalid rvalue" );
			}

			return;
		}

		/// <summary>
		/// Gets the number of access levels
		/// </summary>
		/// <value>The levels of accessing, as an int.</value>
		public int Levels {
			get; set;
		}
	}
}
