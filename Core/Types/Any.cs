using System;
using System.Collections;
using System.Collections.Generic;

using CSim.Core.Literals;
using CSim.Core.Exceptions;

namespace CSim.Core.Types
{
	/// <summary>
	/// Infinite union of all types.
	/// </summary>
	public class Any: Type
	{
		/// <summary>The name of the type.</summary>
		public const string TypeName = "any";

		internal Any()
			: base( TypeName, 1 )
		{
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="CSim.Core.Types.Any"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> with the name of the type: <see cref="TypeName"/>.</returns>
		public override string ToString()
		{
			return this.Name;
		}

		/// <summary>
		/// Creates a literal for this type, given a byte sequence.
		/// The particularity here is that this type is Any, so the more agnostic type is char.
		/// </summary>
		/// <returns>A <see cref="CharLiteral"/>.</returns>
		/// <param name="raw">The sequence of bytes containing the value in memory. Only the fist one matters.</param>
		/// <param name="m">The machine to create the literal for.</param>
		public override Literal CreateLiteral(Machine m, byte[] raw)
		{
			return new CharLiteral( m, (char) raw[0] );
		}

		/// <summary>
		/// Gets the Any type. This type does not depend in any form from the machine's structure.
		/// </summary>
		public static Any Get()
		{
			return instance;
		}

		private static Any instance = new Any();
	}
}

