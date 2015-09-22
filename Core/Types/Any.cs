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
		public const string TypeName = "any";

		internal Any()
			: base( TypeName, 1 )
		{
		}

		public override string ToString()
		{
			return this.Name;
		}

		public override Literal CreateLiteral(Machine m, byte[] raw)
		{
			throw new TypeMismatchException( L18n.Get( L18n.Id.ErrDerreferencedVoid ) );
		}

		public static Any Get()
		{
			return instance;
		}

		private static Any instance = new Any();
	}
}

