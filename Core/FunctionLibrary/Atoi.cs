﻿using System;
using System.Collections.ObjectModel;

using CSim.Core;
using CSim.Core.Functions;
using CSim.Core.Variables;
using CSim.Core.Opcodes;
using CSim.Core.Literals;

namespace CSim.Core.FunctionLibrary {
	/// <summary>
	/// This is the atoi function.
	/// Signature: int atoi(char * x);
	/// </summary>
	public sealed class Atoi: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "atoi";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Atoi(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), printFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Atoi Get(Machine m)
		{
			if ( instance == null ) {
				printFormalParams = new Variable[] {
					new PtrVariable( new Id( @"x" ), m.TypeSystem.GetCharType(), m )
				};

				instance = new Atoi( m );
			}

			return instance;
		}

		public override void Execute(RValue[] realParams)
		{
			Variable param = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );
			int valueFromStr = Convert.ToInt32( param.LiteralValue.Value );
			Variable result = new TempVariable( this.Machine.TypeSystem.GetIntType() );
			result.LiteralValue = new IntLiteral( this.Machine, valueFromStr );

			this.Machine.ExecutionStack.Push( result );
		}

		private static Atoi instance = null;
		private static Variable[] printFormalParams;
	}
}
