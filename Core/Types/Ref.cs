using System;
using System.Collections;
using System.Collections.Generic;

namespace CSim.Core.Types
{
    /// <summary>
    /// Represents references. References are a kind of pointer,
    /// that cannot be changed to point to another variable, and does not
	/// need special operators, such as * and &amp;.
    /// </summary>
	public class Ref: Ptr {
		/// <summary>The reference part for the name of the type.</summary>
		public const string RefTypeNamePart = "&";

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Types.Ref"/> class.
		/// </summary>
		/// <param name="associatedType">Associated <see cref="CSim.Core.Type"/>.</param>
		/// <param name="wordSize">The word size of the machine.</param>
        internal Ref(Type associatedType, int wordSize)
			: base( associatedType.Name + RefTypeNamePart, associatedType, wordSize )
        {
        }

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Types.Ref"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Types.Ref"/>.</returns>
		public override string ToString()
		{
			return this.Name;
		}
	}
}

