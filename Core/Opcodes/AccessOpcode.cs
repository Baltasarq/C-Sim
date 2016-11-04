﻿using System;

namespace CSim.Core.Opcodes {
	using System;

	using CSim.Core.Variables;
	using CSim.Core.Types.Primitives;
	using CSim.Core.Literals;
	using CSim.Core.Types;
	using CSim.Core.Exceptions;

	public class AccessOpcode: Opcode {
		public const char OpcodeValue = (char) 0xE0;

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

					// If the vble at the right is a reference, dereference it
					if ( vbleAsRef != null  ) {
						vble = vbleAsRef.PointedVble;
					}
					else
					if ( vble.Type.IsArithmetic()
					  || vble is PtrVariable )
					{
						Variable vbleOld = vble;
						long access = Convert.ToInt64( vble.LiteralValue.Value );
						vble = this.Machine.TDS.LookForAddress( access );

						if ( vble == null ) {
								throw new UnknownVbleException( "*" + vbleOld.Name.Name + " == " + access + "??" );
						}
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
